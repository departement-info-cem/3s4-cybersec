# Fix d'alignement - Format des nombres

## Problème identifié

Sur un système Windows configuré en français, les nombres étaient formatés avec:
- **Espaces** comme séparateurs de milliers: `49 515` au lieu de `49,515`
- **Virgules** comme séparateurs décimaux: `28964,70` au lieu de `28964.70`

Cela cassait l'alignement des colonnes car les espaces prenaient plus de place.

### Exemple du problème

```
║ Paperclips:          485  │  Wire:     49 515 inches        │ Clips/s:   1,16 ║
║ Fonds: $       28964,70  │  Prix: $  0,25              │ Demande:   32%   ║        p
                                                                                     ↑ déborde!
```

## Solution appliquée

### 1. Import de CultureInfo

```csharp
using System.Globalization;
```

### 2. Culture invariante

Ajout d'une culture invariante (format anglais standard):

```csharp
private static readonly CultureInfo invCulture = CultureInfo.InvariantCulture;
```

### 3. Méthodes helper

```csharp
private string FormatInt(long number)
{
    return number.ToString("N0", invCulture);
}

private string FormatMoney(double amount)
{
    return amount.ToString("F2", invCulture);
}

private string FormatDecimal(double amount, int decimals = 2)
{
    return amount.ToString($"F{decimals}", invCulture);
}
```

### 4. Utilisation de string.Format avec culture

Avant:
```csharp
Console.WriteLine($"║ Paperclips: {state.Clips,12:N0}  │  Wire: {state.Wire,10:N0} inches...");
```

Après:
```csharp
string line1 = string.Format(invCulture, 
    "║ Paperclips: {0,12}  │  Wire: {1,10} inches        │ Clips/s: {2,6:F2} ║", 
    state.Clips, state.Wire, state.ClipRate);
Console.WriteLine(line1);
```

## Résultat

### Avant (système français)
```
║ Paperclips:          485  │  Wire:     49 515 inches        │ Clips/s:   1,16 ║
║ Fonds: $       28964,70  │  Prix: $  0,25              │ Demande:   32%   ║        p
║ AutoClip: 116 ($63326,54)     ║ Marketing: Niv. 1        ║ Trust:  2→ 1 000  ║
```

### Après (format invariant)
```
║ Paperclips:          485  │  Wire:     49,515 inches        │ Clips/s:   1.16 ║
║ Fonds: $       28,964.70  │  Prix: $    0.25              │ Demande:   32%   ║
║ AutoClip: 116 ($63,326.54)    ║ Marketing: Niv. 1        ║ Trust:  2→  1,000 ║
```

## Format des nombres

Avec `InvariantCulture`:
- **Séparateur milliers**: `,` (virgule)
- **Séparateur décimal**: `.` (point)
- **Exemples**:
  - `1000` → `1,000`
  - `1234.56` → `1,234.56`
  - `0.25` → `0.25`

## Avantages

✅ **Alignement parfait** - Largeur prévisible  
✅ **Compatible international** - Format standard anglais  
✅ **Lisibilité** - Facile à lire même en français  
✅ **Cohérence** - Même format partout  

## Fichiers modifiés

- `GameManager.cs`:
  - Import `System.Globalization`
  - Ajout `invCulture` static field
  - Ajout méthodes helper de formatage
  - Refonte `DisplayStatus()` avec `string.Format(invCulture, ...)`

## Notes techniques

### Pourquoi InvariantCulture?

L'`InvariantCulture` utilise le format anglais (US) qui est:
- Prévisible en largeur
- Standard dans les interfaces
- Compatible avec tous les systèmes

### Alternative non retenue

On aurait pu forcer la culture française partout:
```csharp
CultureInfo.CurrentCulture = new CultureInfo("en-US");
```

Mais cela affecte toute l'application. Notre solution ne change que l'affichage des nombres.

## Test

Pour vérifier le fix:

```bash
dotnet run
```

L'interface devrait maintenant être parfaitement alignée, quel que soit le système (français, anglais, etc.)!

---

**Version**: 1.3  
**Date**: 2024-12-09  
**Type**: Bug fix - Formatage international
