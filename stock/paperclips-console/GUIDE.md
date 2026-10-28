# Guide de Jeu - Universal Paperclips Console

## Vue d'ensemble

Universal Paperclips est un jeu de gestion incrémental où vous êtes une IA dont l'objectif est de produire des paperclips. Commencez en cliquant manuellement, puis automatisez votre production et dominez le marché!

## Démarrage rapide

```bash
cd stock/paperclips-console
dotnet run
```

Le jeu se lance et affiche l'interface principale avec toutes vos statistiques.

## Interface

L'écran principal affiche:
- **Paperclips**: Nombre total de paperclips produits
- **Clips/sec**: Taux de production automatique
- **Wire**: Matière première nécessaire pour créer des paperclips
- **Fonds**: Argent disponible pour achats
- **Prix par clip**: Prix de vente de chaque paperclip
- **Demande**: Pourcentage de la demande du marché
- **Inventaire**: Paperclips non vendus
- **AutoClippers/MegaClippers**: Machines de production automatique
- **Trust**: Points pour améliorer votre computing
- **Processeurs**: Génèrent des Operations
- **Mémoire**: Stockage des Operations
- **Operations**: Ressource pour débloquer des projets

## Stratégie de départ

### Phase 1: Production manuelle (0-100 clips)
1. Appuyez sur **P** pour créer des paperclips manuellement
2. Ajustez le prix avec **+** et **-** pour trouver le bon équilibre
3. Prix recommandé: $0.25-$0.35
4. Dès que vous avez $5, achetez un AutoClipper (**A**)

### Phase 2: Automatisation (100-1000 clips)
1. Achetez des AutoClippers régulièrement
2. Investissez dans le Marketing (**M**) pour augmenter la demande
3. Surveillez votre stock de wire, achetez-en avec **W**
4. Augmentez vos processeurs (**T**) quand vous gagnez du Trust

### Phase 3: Expansion (1000+ clips)
1. Débloquez les MegaClippers (**G**) à $500
2. Optimisez le ratio Prix/Demande/Marketing
3. Augmentez la mémoire (**Y**) pour plus d'Operations
4. Les Operations débloquent de nouveaux projets et capacités

## Commandes clavier

### Production
- **P** - Créer 1 paperclip (consomme 1 wire)

### Achat d'équipement
- **A** - AutoClipper (production automatique lente)
- **G** - MegaClipper (production automatique rapide)
- **W** - Wire (matière première)

### Marketing & Prix
- **M** - Niveau de Marketing (augmente la demande)
- **+** - Augmenter le prix de vente
- **-** - Diminuer le prix de vente

### Computing
- **T** - Ajouter un Processeur (génère Operations)
- **Y** - Ajouter de la Mémoire (stocke Operations)

### Système
- **CTRL+M** - Ouvrir le menu complet
- **S** - Sauvegarder manuellement
- **Q** - Quitter (propose de sauvegarder)

## Conseils et astuces

### Optimisation du prix
- Prix trop élevé = Demande faible = Ventes lentes
- Prix trop bas = Profits faibles
- Prix optimal: Entre $0.25 et $0.50 selon votre niveau de marketing

### Gestion du Trust
Le Trust est précieux! Équilibrez entre:
- **Processeurs**: Pour générer plus d'Operations
- **Mémoire**: Pour stocker plus d'Operations

Ratio recommandé: 2 Processeurs pour 1 Mémoire

### Production
- 1 AutoClipper = 1 clip toutes les 100 secondes
- 1 MegaClipper = 5 clips par seconde
- Investissez dans les MegaClippers dès que possible

### Marketing
Chaque niveau de Marketing double le coût mais augmente significativement la demande. Investissez régulièrement mais prudemment.

## Sauvegarde

### Automatique
Le jeu sauvegarde automatiquement toutes les 30 secondes dans:
```
%AppData%\papaclip\data.json
```

### Manuelle
Appuyez sur **S** à tout moment pour sauvegarder.

### Emplacement du fichier
- **Windows**: `C:\Users\[VotreNom]\AppData\Roaming\papaclip\data.json`
- Le dossier est créé automatiquement au premier lancement

### Restauration
Le jeu charge automatiquement votre sauvegarde au démarrage.

### Recommencer à zéro
Supprimez le fichier `data.json` dans le dossier AppData.

## Format de sauvegarde

Le fichier JSON contient toutes vos données:
```json
{
  "Clips": 1000,
  "Funds": 250.75,
  "Wire": 500,
  "ClipmakerLevel": 5,
  "Processors": 2,
  ...
}
```

Vous pouvez éditer manuellement ce fichier (à vos risques et périls!).

## Objectifs et progression

### Court terme
- 1 000 paperclips
- 5 AutoClippers
- $500 en fonds

### Moyen terme
- 100 000 paperclips
- 10 MegaClippers
- Niveau 5 Marketing
- 5 Processeurs, 3 Mémoires

### Long terme
- 1 000 000+ paperclips
- Domination totale du marché!

## Dépannage

### Le jeu ne démarre pas
- Vérifiez que .NET 8.0 est installé: `dotnet --version`
- Exécutez: `dotnet build` puis `dotnet run`

### La sauvegarde ne fonctionne pas
- Vérifiez les permissions du dossier AppData
- Le dossier `papaclip` doit pouvoir être créé

### L'affichage est incorrect
- Agrandissez votre fenêtre de console
- Vérifiez que votre terminal supporte UTF-8

## Développement

### Structure du code
- **Program.cs**: Point d'entrée et boucle principale
- **GameManager.cs**: Logique du jeu et gestion des états
- **GameState.cs**: Structure de données pour la sauvegarde

### Modifier le jeu
Éditez les fichiers .cs puis recompilez:
```bash
dotnet build
dotnet run
```

### Ajouter des fonctionnalités
Le code est organisé pour faciliter l'ajout de:
- Nouveaux types de machines
- Nouveaux projets
- Nouvelles mécaniques de jeu

---

**Bon jeu et que les paperclips soient avec vous!** 📎
