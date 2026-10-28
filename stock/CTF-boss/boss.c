// CTF Challenge: Le boss invincible
// Un jeu de combat impossible à gagner... honnêtement.
//
// Compilé en code natif (C), sans symboles: il faut Ghidra, pas dotPeek.
//   x86_64-w64-mingw32-gcc -O0 -s -static -o CTF-boss.exe boss.c

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdint.h>
#ifdef _WIN32
#include <windows.h>
#endif

#ifndef DEGATS_JOUEUR
#define DEGATS_JOUEUR 1
#endif

#define PV_BOSS_MAX 9999
#define CLE_XOR 0x5A

// Niveau 1: une note de développeur oubliée...
const char note_dev[] = "TODO retirer avant la release! flag de test: CEM{l3s_str1ngs_p4rl3nt}";

// Niveau 2: le code de triche et son flag, masqués par XOR pour que strings ne les trouve pas
static const unsigned char code_triche[] = {
    0x2e, 0x28, 0x35, 0x2a, 0x3c, 0x3b, 0x39, 0x33, 0x36, 0x3f
};
static const unsigned char flag_triche[] = {
    0x19, 0x1f, 0x17, 0x21, 0x22, 0x6a, 0x28, 0x05, 0x2a, 0x6e, 0x29, 0x05, 0x2e, 0x69,
    0x36, 0x36, 0x3f, 0x37, 0x69, 0x34, 0x2e, 0x05, 0x39, 0x6e, 0x39, 0x32, 0x69, 0x27
};

// Niveau 3: le flag de la victoire, chiffré avec un flux pseudo-aléatoire
static const unsigned char flag_victoire[] = {
    0x66, 0x8c, 0x02, 0xb1, 0xd3, 0x8b, 0x18, 0xfd, 0x81, 0xf8, 0x46, 0x5e,
    0xe8, 0x93, 0x82, 0xc5, 0x85, 0x7a, 0x08, 0xf6, 0xf9, 0xd3, 0xb1
};

static void demasquer(const unsigned char *src, size_t n, char *dest)
{
    for (size_t i = 0; i < n; i++)
        dest[i] = (char)(src[i] ^ CLE_XOR);
    dest[n] = '\0';
}

static void dechiffrer_victoire(char *dest)
{
    uint32_t etat = 0xC0FFEE ^ 0x1337;
    size_t n = sizeof(flag_victoire);
    for (size_t i = 0; i < n; i++)
    {
        etat = etat * 1103515245u + 12345u;
        dest[i] = (char)(flag_victoire[i] ^ ((etat >> 16) & 0xFF));
    }
    dest[n] = '\0';
}

static void lire_ligne(char *ligne, size_t taille)
{
    if (fgets(ligne, (int)taille, stdin) == NULL)
        exit(0);
    ligne[strcspn(ligne, "\r\n")] = '\0';
}

static void victoire(void)
{
    char flag[64];
    dechiffrer_victoire(flag);
    printf("\n*** VICTOIRE! Le boss s'effondre. ***\n");
    printf("Voici votre récompense: %s\n", flag);
}

static void defaite(void)
{
    printf("\nLe boss vous a vaincu. Game over.\n");
    printf("(Indice: ce combat ne se gagne pas en jouant...)\n");
}

int main(void)
{
#ifdef _WIN32
    SetConsoleOutputCP(CP_UTF8);
#endif
    if (getenv("BOSS_DEBUG") != NULL)
        puts(note_dev);

    char code[sizeof(code_triche) + 1];
    demasquer(code_triche, sizeof(code_triche), code);

    int pv_joueur = 100;
    int pv_boss = PV_BOSS_MAX;
    int potions = 3;
    int degats = DEGATS_JOUEUR;
    int degats_boss = 7;
    char ligne[64];

    printf("=== Le boss invincible ===\n");
    printf("Un boss de %d points de vie vous barre la route.\n", PV_BOSS_MAX);

    for (;;)
    {
        printf("\nVous: %d PV | Potions: %d | Boss: %d PV\n", pv_joueur, potions, pv_boss);
        printf("1. Attaquer  2. Boire une potion  3. Fuir\n> ");
        lire_ligne(ligne, sizeof(ligne));

        if (strcmp(ligne, code) == 0)
        {
            char flag[sizeof(flag_triche) + 1];
            demasquer(flag_triche, sizeof(flag_triche), flag);
            printf("Mode triche activé! %s\n", flag);
            printf("...mais le boss a toujours %d PV. Il va falloir tricher plus fort.\n", pv_boss);
            continue;
        }

        if (strcmp(ligne, "1") == 0)
        {
            pv_boss -= degats;
            printf("Vous frappez le boss: -%d PV.\n", degats);
        }
        else if (strcmp(ligne, "2") == 0 && potions > 0)
        {
            potions--;
            pv_joueur += 15;
            printf("Vous buvez une potion: +15 PV.\n");
        }
        else if (strcmp(ligne, "3") == 0)
        {
            printf("Vous tentez de fuir... le boss vous rattrape.\n");
        }
        else
        {
            printf("Action invalide, vous perdez votre tour.\n");
        }

        if (pv_boss <= 0)
        {
            victoire();
            break;
        }

        pv_joueur -= degats_boss;
        printf("Le boss vous frappe: -%d PV.\n", degats_boss);
        if (pv_joueur <= 0)
        {
            defaite();
            break;
        }
    }

    printf("\nAppuyez sur Entrée pour quitter...");
    getchar();
    return 0;
}
