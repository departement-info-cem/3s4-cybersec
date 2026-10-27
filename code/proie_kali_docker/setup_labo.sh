#!/bin/bash
# ===========================================================
# Generateur du Laboratoire 2.9 (Mac/Linux).
# Cree tous les fichiers du lab + start_labo.sh/stop_labo.sh,
# puis lance start_labo.sh.
# Genere par build_generators.py -- ne pas editer a la main.
# ===========================================================
set -e
cd "$(dirname "$0")"
echo "Creation des fichiers du laboratoire..."
mkdir -p proie kali
cat > proie/Dockerfile <<'LABO__FILE__EOF__'
# ==============================================================================
# Machine Proie : serveur SSH cible du CTF + bureau web léger (noVNC + Fluxbox)
# Base debian-slim volontairement minimale pour un téléchargement/boot rapide.
# ==============================================================================
FROM debian:bookworm-slim

ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update && apt-get install -y --no-install-recommends \
    openssh-server \
    fail2ban \
    rsyslog \
    iptables \
    sudo \
    procps \
    iproute2 \
    nano \
    # --- Bureau web XFCE + écran de connexion (xdm) + noVNC ---
    xfce4 \
    xfce4-terminal \
    mousepad \
    dbus-x11 \
    xfonts-base \
    xdm \
    tigervnc-standalone-server \
    novnc \
    websockify \
    && rm -rf /var/lib/apt/lists/* \
    && mkdir -p /run/sshd \
    # Page d'accueil : connexion auto au bureau
    && printf '<!doctype html><meta http-equiv="refresh" content="0; url=vnc.html?autoconnect=true&resize=remote">' \
       > /usr/share/novnc/index.html

# Locale UTF-8 : accents corrects dans nano / mousepad / terminal (aussi en SSH).
ENV LANG=C.UTF-8
RUN printf 'LANG=C.UTF-8\n' > /etc/default/locale && \
    printf 'LANG=C.UTF-8\n' >> /etc/environment && \
    mkdir -p /root/.config && \
    printf '[Default Applications]\ntext/plain=org.xfce.mousepad.desktop\n' \
       > /root/.config/mimeapps.list

# Écran de connexion graphique (xdm) : plus d'auto-login root.
# xdm lance Xvnc sur :1 (port 5901) et affiche un login ; après authentification,
# la session de l'utilisateur démarre XFCE (via ~/.xsession, copié depuis /etc/skel).
# root n'a pas de mot de passe -> pas de login GUI possible (admin via 'docker exec').
RUN printf ':1 local /usr/bin/Xvnc :1 -geometry 1360x768 -depth 24 -SecurityTypes None -AlwaysShared -rfbport 5901 -ac\n' \
        > /etc/X11/xdm/Xservers && \
    sed -i 's/^DisplayManager.requestPort.*/DisplayManager.requestPort: 0/' /etc/X11/xdm/xdm-config && \
    printf 'exec startxfce4\n' > /etc/skel/.xsession && \
    printf 'exec startxfce4\n' > /root/.xsession

COPY init_proie.sh /root/init_proie.sh
COPY entrypoint.sh /usr/local/bin/entrypoint.sh

# Commandes de bascule fail2ban pour l'atelier (Phase 1 OFF / Phase 2 ON)
COPY activer_fail2ban.sh /usr/local/bin/activer_fail2ban
COPY desactiver_fail2ban.sh /usr/local/bin/desactiver_fail2ban

RUN chmod +x /usr/local/bin/entrypoint.sh \
    /usr/local/bin/activer_fail2ban \
    /usr/local/bin/desactiver_fail2ban

# 22 = SSH (cible) ; 3000 = bureau web interne (publié en 3001 côté hôte)
EXPOSE 22 3000

ENTRYPOINT ["/usr/local/bin/entrypoint.sh"]
LABO__FILE__EOF__
cat > proie/entrypoint.sh <<'LABO__FILE__EOF__'
#!/bin/bash
set -e

# Journalisation (auth.log attendu par fail2ban et par le scénario)
touch /var/log/auth.log
chmod 640 /var/log/auth.log
service rsyslog start || rsyslogd

# Clés d'hôte SSH
ssh-keygen -A

# Initialisation unique du scénario CTF (comptes, flags, clés, config sshd).
# '|| true' : init_proie.sh finit par un restart systemctl inutile en conteneur.
if [ ! -f /etc/.ctf_initialized ]; then
    bash /root/init_proie.sh || true
    touch /etc/.ctf_initialized
fi

# --- Écran de connexion graphique (xdm) accessible par navigateur (noVNC) ---
# xdm affiche un login et démarre lui-même Xvnc sur :1 (port 5901). Plus d'auto-
# login root : chacun se connecte avec ses identifiants (root n'a pas de mot de
# passe -> admin uniquement via 'docker exec'). Après login, XFCE démarre.
mkdir -p /tmp/.X11-unix && chmod 1777 /tmp/.X11-unix
rm -f /tmp/.X1-lock /tmp/.X11-unix/X1 2>/dev/null || true
xdm -nodaemon >/var/log/xdm.log 2>&1 &
sleep 3
# websockify sert le client noVNC (port 3000 interne) et proxifie vers Xvnc (5901)
websockify --web=/usr/share/novnc 3000 localhost:5901 >/var/log/novnc.log 2>&1 &

echo "=========================================================="
echo "🎯 Conteneur Proie-SSH + Bureau web prêt !"
echo "   SSH    : 192.168.220.128:22"
echo "   Bureau : http://localhost:3001"
echo "=========================================================="

# Services cibles du CTF
service ssh start

# fail2ban reste DÉSACTIVÉ au démarrage (Phase 1 : le brute-force hydra doit
# aboutir). Pour la Phase 2, l'activer avec :  activer_fail2ban
# (et le recouper avec : desactiver_fail2ban)

# Garde le conteneur en vie et affiche les tentatives de connexion
tail -f /var/log/auth.log
LABO__FILE__EOF__
cat > proie/init_proie.sh <<'LABO__FILE__EOF__'
#!/bin/bash
PAYLOAD="H4sIAJGC4GoC/61Y3U4jyRW+r6c4eMjaINo2hmE03lm0xpisswxYNjNSEmWtcncZ107/TVW1gQnJXaJEijT3q0TZuyy5yE1ucu832RdIHiFfVTfYGMMm0lpgtbtOnTp/33dO97O12kjGtRHXE6aFIU8wOaZfUmm986Z7WCIvFlSnX31KZiJiRiT8SUKl7//yJ+ooJTJFTXorMhmG4gOJy9mNnxmhyBekfSVTQ3wqfNJZkFDFfduDaL2+UbK6LqWhbTaWjBVqP/6TurE0kodScyOTmIIMmmY3MVcyofbZEfVUIoXXU7ObgBtrgIjJTzKlq9VqibFxoijTMEHGBC0wZJSMyJ9wFUpBAZ/K4FMKEpxOBEfXSAbw1e4o0Sf7tUBMa3EWhnf+5h+7zoOAvIg8TXcRu93p5OBGkMTi1hd3eDPlWl8kKijRNYxwvwLmc0OvXlG5c3pUpn2qTZJI1Jx8bRzy8+2quTTsP3/95oaOjls/pW3EuN15/Wsn4d1q/A1jB4pPE7hwlhHXCEKUqiSSmkLhfhj4W265GLgsZEgl4orb2vA44CqoMuZ5HnVPDrvtDvVO3/TpuEXt7sFxhxpUQeQ2CALsZ2UuKUCyZt/SAcL5PoMOJCdKDAWCrEmCZjeGQ8SoJKUU6ZlKLUehqLJuSFGZE26lSYx8vs/KMqSwzKdWHsGCIDITw3BeGFgac22M4v47G4uSPeRLuM8qtUyrmkY2Rc0GwUrr2j3hDUIV50cgCJGIZ3+zx6RcuQBoLmE5aoUh+gyHXxR10syrZXUy/EmUBLRXrz+yXuQcAWteyBgIaNS3Gz+cc8g7JY2ljDeKjGPdm+uzGZeo9q+TbHaDtL+JUduw3hj+PuMxsBZwm3STZF9bQLgUBdK3OIq5VMKmT3Py87Q8nvodqhSAydPfLtDD6RxInN0gkwvFhNygDvS8EHwUlwQ49RZFiDYhE5ySbBRK7IMU01maKhkBpKjUsfQnEpdYMEKbvBBqJkprhQ3DW31C2zDRGmsZI2LHDk2rAQicWiIYDL6gUZjY4gyFxUMcg2ESqEN9mawIFFYcGmydMnvoRAkeWJkoC4GL2Y2uUh/skklXkNNEhhliV/EM7cKhS8hFG7BiXj827ZZlViV1uXLurz4oi8f9ZjyIZLzd2GFKaMGzRr2xx/yrkVBa+JmSRrAoiQ2cCV++ZL44F6kTSXdRf3UVDP3Jrgq3d1go4+xyGAEzQjGtJ0O3XwzdLgV9YvjiBUhZiQsehsPRlc0qO58k2gxlPAQvDvVEhCEDDIPkYqgyxFmxd25voofT52zKwwn28uE5rC8C5cKwu/ukiwWMitXm3JwfhlKxx8V2ZwlOOwWcChlvrheQ6lz6cAb1dEekynaxdLnMATyHRNyopIiFJy7TMJHGtakNSoE4UgKADKRaBa4OHZwOBlRxLSjH1aG9JFv0IEObXRyk8qamr7SZfReJKg2WeXZs6WynAZiB8iCkbEWHsxtuEmmvm0xGMNCRr91lu4Gyha/ywv/iKlAcl5YxUN9jNEorxYEjbMJpa6wHsvRdMW1RbiSIGVDWiAgsnP0DCS/YNJuKc3QSd5LlFhgC9pfT2Y1wgHRwRmESmiPU29mA+xMI3U9bdQT6ztKhi04N9EysZyMaljMjkQDsa6LLbm9QO0mlM9cdpSFjOGAJErGZEZfVYuTw0w23xX3w63YG+PxVtzdM7RCx33zKBvc9xJ8IGs+fb7+k3xa33okrqG1sUB+ZU0JafkHYA6gEC1a0jOGtdR02KjHGgEDePpXenPT6p2ed9lnnkHr97tvWWYe+7PycjrrHndKCqXPCuH/gjnUdSPON8JBpW49ueQthdyFeKJG535hRPLmoKd+0EIQFHrvDXcH3jyFrmdMeSrB+6+Tw9PWw1xoMPluvGEVe4FO55f2Cex/q3su1z5+t/+SrTzbL9MqNXJkCESQRUD4BGZPno8A3CjZwBjfXFzTeYwMWvQPiyEsLY4oKgudMYVobP7hdm+d00/Kfh6icI5rg99tUP72LvBMqYTJ+75joCcFqmo3uCGpBgmdmkij5QbiM6CL6Xj/PTdN9P/Qmj/qLu6g/WNpbsfTgrOVwrSx+Fk2fDMD/g5u5d0+X130Dlp19SmrvaamHFq3uILmwreHd4QhcVTSRj7/Pm8hR96R1XDQSJ+q50z2cji5yNPvjcbfdPWuddU9PBitnco3xIR/LtXuqyaeWKsjWQhaTErpLbEkZUxqoozfhuG0nwYHtR0rayWo+7DiSO+IybIx4bEdeN/KAErXjnsXx5LGqWnJ1dREtCT0InTB+DY0hE9VYGPbZj/Rh33/z7b//9ZEGnf7bDrpn9+Ss0z/p0CG6af/08E3bhpk8arXbsz8MqN8ZnPU7EKJ8HztLMjsIo6ujPU/tcxD3/dl3miw3AxJAhLYdyvZeOyvH9oHT3sinzylXtTA5d+Cp4qLK2kXi0HHlWLu2g2TEicgCZMg9DmzZOXDrtsq38rBXf7yQ2ITiET2whF7Wta+ebfaKR8FWZh9WjfTdLFLdrK1eoCuha+U8ZRbQ+MdUmMRjeb6kNxuBKx5qXXH7f9J5DZ0H3E6J1c3r/GKpcK4fUWHfRZwrkYJsqQQtr/nlwHBlslSXVu5YeG6/59LCRniy8Iv2ms/rze36Y06IUAunLm9GK3aWaH9/9V77YsMOciLyTYhZQNudric35m8a6PqaVgoFi1JFL8NQWVqvl9gEk2KirtAob9+c/Pl39LoYgtwbEvsI/ndgwM2lGAHtq5M1qrTvXszosi1+i4UAT+4qk2ajWmL/BTkpugYPEgAA"
echo "$PAYLOAD" | base64 -d | gunzip | bash -s -- "$0"
LABO__FILE__EOF__
cat > proie/activer_fail2ban.sh <<'LABO__FILE__EOF__'
#!/bin/bash
# ==============================================================================
# PHASE 2 de l'atelier : active la défense fail2ban sur la proie.
# Après activation, 5 échecs SSH en 10 min -> IP bannie 10 min (réglages par défaut).
# ==============================================================================
service fail2ban start 2>/dev/null || fail2ban-client start 2>/dev/null
sleep 1
echo "=========================================================="
if fail2ban-client status sshd >/dev/null 2>&1; then
    echo "🛡️  fail2ban ACTIVÉ (Phase 2)"
    echo "   Règle : 5 échecs SSH / 10 min  ->  ban de 10 min"
    echo "   Voir les IP bannies : fail2ban-client status sshd"
else
    echo "⚠️  fail2ban n'a pas démarré (vérifier le service)."
fi
echo "=========================================================="
LABO__FILE__EOF__
cat > proie/desactiver_fail2ban.sh <<'LABO__FILE__EOF__'
#!/bin/bash
# ==============================================================================
# Retour en PHASE 1 : coupe fail2ban et lève TOUS les bannissements.
# ==============================================================================
fail2ban-client unban --all 2>/dev/null || true
service fail2ban stop 2>/dev/null || fail2ban-client stop 2>/dev/null || true
sleep 1
echo "=========================================================="
echo "✅ fail2ban DÉSACTIVÉ (tous les bannissements levés)"
echo "   Le brute-force n'est plus bloqué (Phase 1)."
echo "=========================================================="
LABO__FILE__EOF__
cat > kali/Dockerfile <<'LABO__FILE__EOF__'
# ==============================================================================
# Machine Kali (prédateur) : outils d'attaque + bureau web XFCE "look Kali" (noVNC)
# Base kalilinux/kali-rolling (légère, ~130 Mo) au lieu du webtop linuxserver
# (~2-3 Go), mais habillée comme la vraie Kali (thème Kali-Dark, fond dragon,
# menu whisker + catégories d'outils Kali) via kali-themes / kali-menu.
# ==============================================================================
FROM kalilinux/kali-rolling

ENV DEBIAN_FRONTEND=noninteractive

# Outils de l'atelier 2.9 + bureau web XFCE habillé "Kali" (même pile que la proie)
RUN apt-get update && apt-get install -y --no-install-recommends \
    # --- Outils 2.9 ---
    hydra \
    hydra-gtk \
    ncrack \
    medusa \
    nmap \
    zenmap \
    openssh-client \
    wordlists \
    iputils-ping \
    iproute2 \
    nano \
    ca-certificates \
    curl \
    # --- Bureau web XFCE (menu d'applications + noVNC) ---
    xfce4 \
    xfce4-terminal \
    mousepad \
    dbus-x11 \
    xfonts-base \
    tigervnc-standalone-server \
    novnc \
    websockify \
    # --- Habillage "vraie Kali" (thème, fond, menu d'outils, greffons du panneau) ---
    kali-themes \
    kali-menu \
    xfce4-whiskermenu-plugin \
    xfce4-cpugraph-plugin \
    xfce4-genmon-plugin \
    xfce4-notifyd \
    xfce4-pulseaudio-plugin \
    && rm -rf /var/lib/apt/lists/* \
    # Page d'accueil : connexion auto au bureau
    && printf '<!doctype html><meta http-equiv="refresh" content="0; url=vnc.html?autoconnect=true&resize=remote">' \
       > /usr/share/novnc/index.html

# Locale UTF-8 : accents corrects dans nano / mousepad / terminal.
# mousepad défini comme éditeur par défaut pour les fichiers texte (double-clic).
ENV LANG=C.UTF-8
RUN printf 'LANG=C.UTF-8\n' > /etc/default/locale && \
    printf 'LANG=C.UTF-8\n' >> /etc/environment && \
    mkdir -p /root/.config && \
    printf '[Default Applications]\ntext/plain=org.xfce.mousepad.desktop\n' \
       > /root/.config/mimeapps.list

# Applique la config de bureau Kali (thème Kali-Dark, panneau whisker) à root :
# XFCE lit ces fichiers au premier démarrage.
RUN mkdir -p /root/.config && cp -rn /etc/skel/. /root/ 2>/dev/null; \
    mkdir -p /root/.config/xfce4/xfconf/xfce-perchannel-xml && \
    cp -f /etc/xdg/xfce4/xfconf/xfce-perchannel-xml/xsettings.xml \
          /root/.config/xfce4/xfconf/xfce-perchannel-xml/ && \
    cp -f /etc/xdg/xfce4/panel/default.xml \
          /root/.config/xfce4/xfconf/xfce-perchannel-xml/xfce4-panel.xml && \
    # power-manager-plugin absent en conteneur -> séparateur (évite le dialogue d'erreur)
    sed -i 's/value="power-manager-plugin"/value="separator"/' \
        /root/.config/xfce4/xfconf/xfce-perchannel-xml/xfce4-panel.xml

# Fond d'écran Kali converti en PNG au build. Le décodage JPEG passe par glycin,
# qui échoue en conteneur ; un PNG se charge nativement (loader gdk-pixbuf intégré).
# Conversion via netpbm (léger), retiré ensuite pour ne pas alourdir l'image.
RUN apt-get update && apt-get install -y --no-install-recommends netpbm && \
    jpegtopnm /usr/share/backgrounds/kali/kali-cubes-16x9.jpg | pnmtopng \
      > /usr/share/backgrounds/kali-wallpaper.png && \
    apt-get purge -y netpbm && apt-get autoremove -y && rm -rf /var/lib/apt/lists/*
COPY xfce4-desktop.xml /root/.config/xfce4/xfconf/xfce-perchannel-xml/xfce4-desktop.xml

# Gestionnaire de fichiers + terminal par défaut : sans ça, un double-clic sur les
# icônes du bureau (Home, File System) n'ouvre rien (aucun helper configuré).
COPY helpers.rc /root/.config/xfce4/helpers.rc

# --- Wordlists de l'atelier (mêmes listes utiles que la vraie Kali) ---
# rockyou : fourni compressé par le paquet "wordlists" -> décompressé (chemin attendu).
# fasttrack : petite liste (~220 mdp) récupérée directement (évite le paquet "set" ~500 Mo).
# unix_passwords : liste courte Metasploit.
RUN gunzip -f /usr/share/wordlists/rockyou.txt.gz && \
    curl -sL https://raw.githubusercontent.com/trustedsec/social-engineer-toolkit/master/src/fasttrack/wordlist.txt \
      -o /usr/share/wordlists/fasttrack.txt && \
    mkdir -p /usr/share/metasploit && \
    curl -sL https://raw.githubusercontent.com/rapid7/metasploit-framework/master/data/wordlists/unix_passwords.txt \
      -o /usr/share/metasploit/unix_passwords.txt

# Script de vérification des flags
COPY verifier_flags.sh /usr/local/bin/verifier_flags
RUN chmod +x /usr/local/bin/verifier_flags && \
    cp /usr/local/bin/verifier_flags /root/verifier_flags.sh

COPY entrypoint.sh /usr/local/bin/entrypoint.sh
RUN chmod +x /usr/local/bin/entrypoint.sh

# 3000 = bureau web interne (publié en 3000 côté hôte)
EXPOSE 3000

ENTRYPOINT ["/usr/local/bin/entrypoint.sh"]
LABO__FILE__EOF__
cat > kali/entrypoint.sh <<'LABO__FILE__EOF__'
#!/bin/bash
set -e

# --- Bureau graphique XFCE accessible par navigateur (noVNC) ---
export DISPLAY=:1
export HOME=/root
mkdir -p /tmp/.X11-unix && chmod 1777 /tmp/.X11-unix
rm -f /tmp/.X1-lock /tmp/.X11-unix/X1 2>/dev/null || true
Xvnc :1 -geometry 1360x768 -depth 24 -SecurityTypes None -AlwaysShared -listen tcp \
    >/var/log/xvnc.log 2>&1 &
sleep 2
# Session XFCE : menu d'applications (catégories) + barre des tâches
dbus-launch --exit-with-session startxfce4 >/var/log/xfce.log 2>&1 &
# websockify sert le client noVNC (port 3000 interne) et proxifie vers Xvnc (5901)
websockify --web=/usr/share/novnc 3000 localhost:5901 >/var/log/novnc.log 2>&1 &

# Fond d'écran Kali (PNG -> pas de décodage via glycin, fiable en conteneur).
# Appliqué sur le moniteur réellement actif, avec redémarrage d'xfdesktop au 1er
# lancement (course avec la détection du moniteur sous Xvnc), puis réappliqué si
# la résolution change (ex. redimensionnement depuis le navigateur noVNC).
WALL=/usr/share/backgrounds/kali-wallpaper.png
appliquer_fond() {
  local MON base
  MON=$(xrandr --query 2>/dev/null | awk '/ connected/{print $1; exit}')
  [ -z "$MON" ] && return 1
  for base in "/backdrop/screen0/monitor$MON/workspace0" "/backdrop/screen0/monitor$MON"; do
    xfconf-query -c xfce4-desktop -p "$base/last-image" -s "$WALL" --create -t string 2>/dev/null
    xfconf-query -c xfce4-desktop -p "$base/image-style" -s 5 --create -t int 2>/dev/null
  done
}
(
  export DISPLAY=:1
  for i in $(seq 1 25); do pgrep -x xfdesktop >/dev/null 2>&1 && break; sleep 1; done
  sleep 2
  appliquer_fond
  pkill -x xfdesktop 2>/dev/null; sleep 1; nohup xfdesktop >/dev/null 2>&1 &
  # Réapplique le fond quand la résolution change (resize du navigateur)
  last=$(xrandr 2>/dev/null | awk '/\*/{print $1; exit}')
  while sleep 3; do
    cur=$(xrandr 2>/dev/null | awk '/\*/{print $1; exit}')
    [ -n "$cur" ] && [ "$cur" != "$last" ] && { appliquer_fond; last="$cur"; }
  done
) &

echo "=========================================================="
echo "🛡️  Conteneur Kali (prédateur) + Bureau XFCE prêt !"
echo "   Bureau : http://localhost:3000"
echo "   Menu Applications → Sécurité / Système pour hydra, xHydra, nmap..."
echo "=========================================================="

# Garde le conteneur en vie
tail -f /var/log/xvnc.log
LABO__FILE__EOF__
cat > kali/verifier_flags.sh <<'LABO__FILE__EOF__'
#!/bin/bash
# ==============================================================================
# Vérificateur de Flags - CTF Proie-Prédateur (Atelier 2.9)
# ==============================================================================

H1="6670b13fca50343d12ef30e65cb7d85a604994ef14f1bba23b341592eebd8aae" # Flag 1 (alice)
H2="f55935390243a1c8672d18a0a1aef174c942433b2848baef411951dd96689aad" # Flag 2 (bob)
H3="babf9890dc3d2f3ebef75a5a1cad5a973992b7d0f453f0e7a7beabceb3d65ad1" # Flag 3 (charlie)
H4="b1a6092567963ea676cb5f30e98998e2558cf98d02bf75f31264daddd702d886" # Flag 4 (david)

SCORE=0

check_flag() {
    local num=$1
    local user=$2
    local expected_hash=$3
    read -p "🚩 Entrez le Flag #$num ($user) : " input_flag
    input_flag=$(echo -n "$input_flag" | xargs)
    local actual_hash=$(echo -n "$input_flag" | sha256sum | awk '{print $1}')
    
    if [ "$actual_hash" == "$expected_hash" ]; then
        echo "   ✅ Exact ! (+0.20 pt)"
        SCORE=$((SCORE + 20))
    else
        echo "   ❌ Flag invalide ou incomplet (Format attendu : CTF{...})"
    fi
}

echo "=================================================="
echo "   🕵️‍♂️ VALIDATEUR DE MISSION : PROIE-PRÉDATEUR"
echo "=================================================="
check_flag 1 "alice" "$H1"
check_flag 2 "bob" "$H2"
check_flag 3 "charlie" "$H3"
check_flag 4 "david" "$H4"
echo "=================================================="
echo "📊 Score CTF (Phase 1) : ${SCORE}% (sur 80% max avant Phase 2)"
echo "=================================================="
LABO__FILE__EOF__
cat > kali/xfce4-desktop.xml <<'LABO__FILE__EOF__'
<?xml version="1.0" encoding="UTF-8"?>
<!--
  Fond d'écran Kali (PNG, pour éviter le décodage JPEG via glycin qui échoue en
  conteneur) appliqué dès le premier affichage.
  Xvnc nomme toujours sa sortie "VNC-0" ; on couvre aussi "monitor0" par sécurité.
-->
<channel name="xfce4-desktop" version="1.0">
  <property name="backdrop" type="empty">
    <property name="screen0" type="empty">
      <property name="monitorVNC-0" type="empty">
        <property name="workspace0" type="empty">
          <property name="color-style" type="int" value="0"/>
          <property name="image-style" type="int" value="5"/>
          <property name="last-image" type="string" value="/usr/share/backgrounds/kali-wallpaper.png"/>
        </property>
      </property>
      <property name="monitor0" type="empty">
        <property name="workspace0" type="empty">
          <property name="color-style" type="int" value="0"/>
          <property name="image-style" type="int" value="5"/>
          <property name="last-image" type="string" value="/usr/share/backgrounds/kali-wallpaper.png"/>
        </property>
      </property>
    </property>
  </property>
</channel>
LABO__FILE__EOF__
cat > kali/helpers.rc <<'LABO__FILE__EOF__'
FileManager=thunar
TerminalEmulator=xfce4-terminal
LABO__FILE__EOF__
cat > start_labo.sh <<'LABO__FILE__EOF__'
#!/bin/bash
# ==============================================================================
# Lanceur universel du Laboratoire 2.9 (Fonctionne avec ou sans Docker Compose)
# ==============================================================================
set -e

echo "🚀 Préparation de l'environnement Docker (Proie + Kali GUI)..."

# Nettoyage d'anciens conteneurs s'ils existent
docker rm -f ctf-proie ctf-kali-gui 2>/dev/null || true

# Création du réseau 192.168.220.0/24 s'il n'existe pas
if ! docker network inspect labo_net >/dev/null 2>&1; then
    echo "🌐 Création du réseau labo_net (192.168.220.0/24)..."
    docker network create --subnet=192.168.220.0/24 labo_net
fi

echo "🔨 Construction de la machine Proie (bureau web + SSH, 192.168.220.128)..."
docker build -t ctf-proie ./proie

echo "🔨 Construction de la machine Kali GUI (192.168.220.129)..."
docker build -t ctf-kali-gui ./kali

echo "▶️ Démarrage du serveur cible ctf-proie (bureau web + SSH)..."
# -p 22:22 : expose le SSH de la proie sur le LAN (IP du Mac, port SSH standard 22)
# pour que d'autres postes du local puissent l'attaquer.
docker run -d \
  --name ctf-proie \
  --hostname proie-ssh \
  --cap-add=NET_ADMIN \
  --net labo_net \
  --ip 192.168.220.128 \
  -p 3001:3000 \
  -p 2222:22 \
  ctf-proie

echo "▶️ Démarrage de Kali Linux (Interface Graphique Web)..."
# --privileged : requis pour le bureau XFCE de Kali. Kali décode les icônes
# via glycin, qui s'isole dans bwrap (namespaces mount + réseau). Docker bloque
# ces opérations par défaut -> pas d'icônes -> XFCE plante (écran noir).
# Conteneur de labo jetable en local : --privileged est acceptable ici.
docker run -d \
  --name ctf-kali-gui \
  --hostname kali-predateur \
  --privileged \
  --net labo_net \
  --ip 192.168.220.129 \
  -p 3000:3000 \
  --shm-size="1gb" \
  ctf-kali-gui

echo "======================================================================"
echo "✅ Laboratoire démarré avec succès !"
echo "🖥️  Interface Graphique Kali : Ouvrez http://localhost:3000 dans votre navigateur"
echo "🖥️  Interface Graphique Proie: Ouvrez http://localhost:3001 dans votre navigateur"
echo "🎯 Cible Proie SSH (interne): 192.168.220.128 (port 22)"
LAN_IP=$(ipconfig getifaddr en0 2>/dev/null || hostname -I 2>/dev/null | awk '{print $1}')
echo "🌐 Cible Proie SSH (LAN)    : ${LAN_IP:-<IP_DU_MAC>} port 22"
echo "     → depuis un autre poste : ssh <user>@${LAN_IP:-<IP_DU_MAC>}"
echo "     → hydra ... ${LAN_IP:-<IP_DU_MAC>} ssh"
echo "🛡️  Terminal Admin Proie     : docker exec -it ctf-proie bash"
echo "======================================================================"
LABO__FILE__EOF__
cat > stop_labo.sh <<'LABO__FILE__EOF__'
#!/bin/bash
echo "🛑 Arrêt et suppression des conteneurs du laboratoire..."
docker rm -f ctf-proie ctf-kali-gui 2>/dev/null || true
docker network rm labo_net 2>/dev/null || true
echo "✅ Environnement nettoyé."
LABO__FILE__EOF__
chmod +x proie/*.sh kali/*.sh start_labo.sh stop_labo.sh
echo "Fichiers crees. Demarrage du laboratoire..."
./start_labo.sh
