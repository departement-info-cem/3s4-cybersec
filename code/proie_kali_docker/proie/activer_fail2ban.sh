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
