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

# ----- PROIE : construite puis démarrée EN PREMIER (dispo le plus vite possible) -----
echo "🔨 Construction de la machine Proie (écran de connexion + SSH)..."
docker build -t ctf-proie ./proie

echo "▶️ Démarrage immédiat de la proie (192.168.220.128)..."
# -p 2222:22 : expose le SSH de la proie (localhost:2222 et LAN) pour l'attaque.
docker run -d \
  --name ctf-proie \
  --hostname proie-ssh \
  --cap-add=NET_ADMIN \
  --net labo_net \
  --ip 192.168.220.128 \
  -p 3001:3000 \
  -p 2222:22 \
  ctf-proie

# Attendre que la proie soit prête (auth.log créé par l'entrypoint)
echo "⏳ Attente de la proie..."
for i in $(seq 1 20); do
  if docker exec ctf-proie test -f /var/log/auth.log 2>/dev/null; then break; fi
  sleep 1
done

# Ouvre une nouvelle fenêtre Terminal qui suit auth.log (tentatives SSH en direct).
# Ctrl-C dans cette fenêtre -> shell admin de la proie.
echo "🖥️  Ouverture du terminal de surveillance (auth.log)..."
osascript -e 'tell application "Terminal" to do script "docker exec -it ctf-proie bash -lc '\''tail -f /var/log/auth.log; exec bash'\''"' >/dev/null 2>&1 \
  || echo "   (ouvrez-le manuellement : docker exec -it ctf-proie bash -lc 'tail -f /var/log/auth.log')"

# ----- KALI : construite et démarrée ensuite -----
echo "🔨 Construction de la machine Kali GUI (192.168.220.129)..."
docker build -t ctf-kali-gui ./kali

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
