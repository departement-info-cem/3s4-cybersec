# Examen partie 1. Intro à la cybersécurité

## Question générale

Un hacker a obtenu le hash du mot de passe d'un compte. Quelle devrait être la première étape pour trouver le mot de passe? Quelle technique peut-il utiliser si ça ne fonctionne pas? Est-ce que ça change quelque chose si le site utilise un sel ou pas? (expliquer pourquoi)

- **4 point**, un point par question et un point pour la justifiation de la 3e question. 

```
Réponse:









```


## CVSS 4.0

Etant donné la situation suivante, on te demande 
- de choisir la valeur de chacun des critères du CVSS 4.0 et d'expliquer chaque réponse
- on ne nous demande PAS de calculer le score 
- le point n'est pas accordé si l'explication n'est pas claire même si la valeur est la bonne
- On considère qu'il n'y a pas d'effet sur des systèmes subséquents, donc PAS de SC, SI, SA

```
Supposons un site d'échange de photos de chats. Ce site a une vulnérabilité car il permet à un utilisateur d'effacer une photo qui n'est pas la notre en modifiant la requête.

Un horrible hacker qui n'aime pas les chats pourrait créer une application qui demande à tout les usagers de suivre leurs comptes. L'application obtiendrait ensuite les ids de toutes leurs photos et pourrait ensuite envoyer des requêtes pour les effacer.
```

**1 point** : Vecteur d'attaque (Réseau, Adjacent, Physique, Local) et explication
```
Réponse:






```

**1 point** : Complexité d'attaque (Faible, Haute) et explication
```
Réponse:






```

**1 point** : Conditions requises (Aucune, Présente) et explication
```
Réponse:






```

**1 point** : Privilèges requis (Aucun, Bas, Elevé) et explication
```
Réponse:






```

**1 point** : Interaction de l'utilisateur (Aucune, Passive, Active) et explication
```
Réponse:






```

**1 point** : Impact sur la confidentialité (VC) (Aucun, Bas, Elevé) et explication
```
Réponse:






```

**1 point** : Impact sur l'intégrité (VI) (Aucun, Bas, Elevé) et explication
```
Réponse:






```

**1 point** : Impact sur la disponibilité (VA) (Aucun, Bas, Elevé) et explication
```
Réponse:









```

RAPPEL: On considère qu'il n'y a pas d'impacte sur des systèmes dépendants pour cet exemple.



## Faille / exploit / correctif

On te demande de décrire la situation suivante en termes de faille de sécurité, d'exploit et de correctif:
```
Mathieu sait que le mot de passe "CrevetePitacheEphemreAku4t1qu3" est excellent, mais comme il n'a pas une bonne mémoire, il l'utilise pour TOUT ses comptes.
Il travaille dans une entreprise importante où il utilise son courriel (information publique) et son mot de passe favoris.
```

1 point : identifie la faille de sécurité présente dans la situation décrite
```
Réponse:






```

2 points : décrire l'exploit au meilleur de tes connaissances
```
Réponse:






```

2 points :  quel serait le correctif que Mathieu peut mettre en place, avec un exemple d'outil pour le faire
```
Réponse:







```

2 points :  quels seraient les correctifs pour l'entreprise pour se protéger des employées comme Mathieu
```
Réponse:







```
