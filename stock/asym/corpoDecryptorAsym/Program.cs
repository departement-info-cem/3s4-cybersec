using corpoDecryptoAsym;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "--generer-cles")
        {
            string dossier = Directory.GetCurrentDirectory();
            RSACrypto.GenerateKeys(dossier);
            Console.WriteLine($"Paire de clés générée dans {dossier}:");
            Console.WriteLine($"  {RSACrypto.FichierClePublique} (peut être distribuée)");
            Console.WriteLine($"  {RSACrypto.FichierClePrivee} (à garder secrète!)");
            return;
        }

        // La clé privée doit se trouver à côté de l'exécutable
        string cheminCle = Path.Combine(AppContext.BaseDirectory, RSACrypto.FichierClePrivee);
        if (!File.Exists(cheminCle))
        {
            Console.WriteLine($"Clé privée introuvable: {cheminCle}");
            Console.WriteLine("Cet outil fonctionne seulement sur le poste sécurisé de Jean-Louis et fils.");
            Console.WriteLine("Pour générer votre propre paire de clés: corpoDecryptoAsym.exe --generer-cles");
            Pause();
            return;
        }

        RSACrypto rsa = new RSACrypto(File.ReadAllText(cheminCle));

        Console.WriteLine("Chemin du fichier à déchiffrer:");
        string path = Console.ReadLine() ?? "";
        foreach (string line in File.ReadAllLines(path))
        {
            string[] parties = line.Split(" @ ");
            string decrypted = rsa.Decrypt(parties[0]);
            Console.WriteLine(decrypted + " @ " + parties[1]);
        }
        Pause();
    }

    // Garde la console ouverte quand on lance l'exécutable par double-clic
    private static void Pause()
    {
        Console.WriteLine("Appuyez sur Entrée pour quitter");
        Console.ReadLine();
    }
}
