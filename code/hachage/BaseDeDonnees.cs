using Microsoft.Data.Sqlite;

namespace MiniComptes;

static class BaseDeDonnees
{
    // La BD est un simple fichier SQLite rangé quelque part dans le profil de l'utilisateur.
    private static readonly string CheminBD = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "MiniComptes",
        "comptes.db");

    private static SqliteConnection OuvrirConnexion()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(CheminBD)!);
        var connexion = new SqliteConnection("Data Source=" + CheminBD);
        connexion.Open();
        return connexion;
    }

    public static void CreerTables()
    {
        using var connexion = OuvrirConnexion();
        var commande = connexion.CreateCommand();
        commande.CommandText =
            "CREATE TABLE IF NOT EXISTS MUtilisateur (nom VARCHAR(200) PRIMARY KEY, motDePasse VARCHAR(200))";
        commande.ExecuteNonQuery();
    }

    public static bool UtilisateurExiste(string nom)
    {
        return LireHachage(nom) != null;
    }

    public static void AjouterUtilisateur(string nom, string hachage)
    {
        using var connexion = OuvrirConnexion();
        var commande = connexion.CreateCommand();
        commande.CommandText = "INSERT INTO MUtilisateur (nom, motDePasse) VALUES ($nom, $motDePasse)";
        commande.Parameters.AddWithValue("$nom", nom);
        commande.Parameters.AddWithValue("$motDePasse", hachage);
        commande.ExecuteNonQuery();
    }

    public static string? LireHachage(string nom)
    {
        using var connexion = OuvrirConnexion();
        var commande = connexion.CreateCommand();
        commande.CommandText = "SELECT motDePasse FROM MUtilisateur WHERE nom = $nom";
        commande.Parameters.AddWithValue("$nom", nom);
        return commande.ExecuteScalar() as string;
    }

    public static List<string> ListerUtilisateurs()
    {
        using var connexion = OuvrirConnexion();
        var commande = connexion.CreateCommand();
        commande.CommandText = "SELECT nom FROM MUtilisateur ORDER BY nom";
        var noms = new List<string>();
        using var lecteur = commande.ExecuteReader();
        while (lecteur.Read())
        {
            noms.Add(lecteur.GetString(0));
        }
        return noms;
    }
}
