using System.Text;

namespace MiniComptes;

static class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        BaseDeDonnees.CreerTables();

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== MiniComptes ===");
            Console.WriteLine("1. Créer un compte");
            Console.WriteLine("2. Se connecter");
            Console.WriteLine("3. Lister les comptes");
            Console.WriteLine("4. Quitter");
            Console.Write("Choix: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": CreerCompte(); break;
                case "2": SeConnecter(); break;
                case "3": ListerComptes(); break;
                case "4": case null: return;
                default: Console.WriteLine("Choix invalide."); break;
            }
        }
    }

    static void CreerCompte()
    {
        string nom = LireTexte("Nom d'utilisateur: ");
        if (nom == "")
        {
            Console.WriteLine("Le nom ne peut pas être vide.");
            return;
        }
        if (BaseDeDonnees.UtilisateurExiste(nom))
        {
            Console.WriteLine("Ce nom d'utilisateur existe déjà.");
            return;
        }

        string motDePasse = LireMotDePasse("Mot de passe: ");
        string confirmation = LireMotDePasse("Confirmer le mot de passe: ");
        if (motDePasse != confirmation)
        {
            Console.WriteLine("Les mots de passe ne correspondent pas.");
            return;
        }

        BaseDeDonnees.AjouterUtilisateur(nom, Hachage.HacherMotDePasse(motDePasse));
        Console.WriteLine($"Compte « {nom} » créé.");
    }

    static void SeConnecter()
    {
        string nom = LireTexte("Nom d'utilisateur: ");
        string motDePasse = LireMotDePasse("Mot de passe: ");

        string? hachageStocke = BaseDeDonnees.LireHachage(nom);
        if (hachageStocke != null && Hachage.VerifierMotDePasse(motDePasse, hachageStocke))
        {
            Console.WriteLine($"Bienvenue, {nom}!");
        }
        else
        {
            Console.WriteLine("Nom d'utilisateur ou mot de passe invalide.");
        }
    }

    static void ListerComptes()
    {
        var noms = BaseDeDonnees.ListerUtilisateurs();
        if (noms.Count == 0)
        {
            Console.WriteLine("Aucun compte.");
            return;
        }
        foreach (string nom in noms)
        {
            Console.WriteLine("- " + nom);
        }
    }

    static string LireTexte(string invite)
    {
        Console.Write(invite);
        return Console.ReadLine()?.Trim() ?? "";
    }

    // Lit un mot de passe en affichant des * au lieu des caractères tapés.
    static string LireMotDePasse(string invite)
    {
        Console.Write(invite);
        if (Console.IsInputRedirected)
        {
            return Console.ReadLine() ?? "";
        }

        var motDePasse = new StringBuilder();
        while (true)
        {
            ConsoleKeyInfo touche = Console.ReadKey(intercept: true);
            if (touche.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return motDePasse.ToString();
            }
            if (touche.Key == ConsoleKey.Backspace)
            {
                if (motDePasse.Length > 0)
                {
                    motDePasse.Length--;
                    Console.Write("\b \b");
                }
            }
            else if (!char.IsControl(touche.KeyChar))
            {
                motDePasse.Append(touche.KeyChar);
                Console.Write('*');
            }
        }
    }
}
