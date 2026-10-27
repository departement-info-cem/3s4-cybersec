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
