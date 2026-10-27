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
