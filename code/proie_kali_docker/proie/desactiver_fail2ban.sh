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
