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
