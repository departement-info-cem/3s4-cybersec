# Universal Paperclips - Console Edition

Version console C# du jeu Universal Paperclips avec gestion des décisions stratégiques.

## Installation

1. Assurez-vous d'avoir .NET 6.0 ou supérieur installé
2. Ouvrez un terminal dans le dossier du projet
3. Exécutez: `dotnet run`

## Commandes

### Touches rapides (toujours actives)
- **P** - Créer un paperclip (consomme 1 wire)
- **W** - Acheter du wire
- **A** - Acheter un AutoClipper
- **G** - Acheter un MegaClipper (disponible après $500)
- **M** - Acheter un niveau de Marketing
- **+** - Augmenter le prix de vente
- **-** - Diminuer le prix de vente
- **T** - Ajouter un Processeur (consomme 1 Trust)
- **Y** - Ajouter de la Mémoire (consomme 1 Trust)
- **S** - Sauvegarder la partie
- **Q** - Quitter le jeu

### Menu
- **CTRL+M** - Ouvrir le menu des commandes complètes

## Sauvegarde

La partie est automatiquement sauvegardée toutes les 30 secondes dans:
`%AppData%\papaclip\data.json`

Vous pouvez également sauvegarder manuellement avec la touche **S**.

## Mécaniques de jeu

### Production
- Cliquez sur **P** pour créer manuellement des paperclips
- Achetez des AutoClippers pour automatiser la production
- Les MegaClippers produisent 500x plus vite

### Économie
- Vendez vos paperclips pour gagner de l'argent
- Ajustez le prix pour optimiser les ventes
- La demande dépend du prix et du niveau de marketing

### Computing
- Les Processeurs génèrent des Operations
- La Mémoire augmente le stockage max d'Operations
- Le Trust se gagne en atteignant des paliers de production

## Objectif

Produire le maximum de paperclips et optimiser votre stratégie de production et de vente!

---

*Inspiré du jeu original Universal Paperclips par Frank Lantz*
