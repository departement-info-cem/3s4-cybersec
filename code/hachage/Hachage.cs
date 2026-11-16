using System.Security.Cryptography;
using System.Text;

namespace MiniComptes;

// Tout le hachage des mots de passe de l'application se trouve dans ce fichier.
// C'est le seul fichier à modifier pour passer à bcrypt.
static class Hachage
{
    // Calcule le hachage à stocker dans la BD pour un nouveau mot de passe.
    public static string HacherMotDePasse(string motDePasse)
    {
        byte[] octets = Encoding.UTF8.GetBytes(motDePasse);
        byte[] hachage = MD5.HashData(octets);
        return Convert.ToHexString(hachage);
    }

    // Vérifie si le mot de passe tapé correspond au hachage stocké dans la BD.
    public static bool VerifierMotDePasse(string motDePasse, string hachageStocke)
    {
        return HacherMotDePasse(motDePasse) == hachageStocke;
    }
}
