#!/bin/bash
echo "🛑 Arrêt et suppression des conteneurs du laboratoire..."
docker rm -f ctf-proie ctf-kali-gui 2>/dev/null || true
docker network rm labo_net 2>/dev/null || true
echo "✅ Environnement nettoyé."
