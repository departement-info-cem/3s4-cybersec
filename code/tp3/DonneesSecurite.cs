// https://crackstation.net/
// https://www.mscs.dal.ca/~selinger/md5collision/

using System;

namespace consoleApp
{
    class DonneesSecurite
    {
        public static string Encrypter(string input)
        {
            string result = "";
            foreach (char c in input)
            {
                char t = (char)(c * 2);
                result += t;
            }
            return result;
        }

        public static string Decrypter(string input)
        {
            string result = "";
            foreach (char c in input)
            {
                char t = (char)(c / 2);
                result += t;
            }
            return result;
        }

        // fonctions permettant le hachage des mots de passe
        public static string HacherLeMotDePasse(string input)
        {
            // Utilise la chaîne d'entrée pour calculer le hachage MD5
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToHexString(hashBytes); // Requiert .NET 5+
            }
        }
        
        public static bool VerifierLeMotDePasse(string motDePasse, string hache)
        {
            // Calcule le hachage MD5 du mot de passe fourni et le compare avec le haché stocké
            return hache == DonneesSecurite.HacherLeMotDePasse(motDePasse);
        }
        
    }
}
