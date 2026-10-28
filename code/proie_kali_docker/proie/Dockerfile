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
    printf 'vncconfig -nowin &\nexec startxfce4\n' > /etc/skel/.xsession && \
    printf 'vncconfig -nowin &\nexec startxfce4\n' > /root/.xsession

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
