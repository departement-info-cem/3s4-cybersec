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
# Presse-papiers partagé : relie la sélection X11 (CLIPBOARD) au presse-papiers
# VNC, pour que le copier-coller passe entre le conteneur et le navigateur (noVNC).
vncconfig -nowin >/var/log/vncconfig.log 2>&1 &
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
