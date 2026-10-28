// CTF Challenge: J'écris partout!
// Cette application de prise de notes cache vos données dans des endroits surprenants...

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Application de Notes Sécurisées™ ===");
        Console.WriteLine("Vos notes sont sauvegardées de manière ultra-sécurisée!");
        Console.WriteLine();
        
        bool running = true;
        while (running)
        {
            Console.WriteLine("1. Ajouter une note");
            Console.WriteLine("2. Voir mes notes");
            Console.WriteLine("3. Supprimer toutes les notes");
            Console.WriteLine("4. Quitter");
            Console.Write("Choix: ");
            
            string? choix = Console.ReadLine();
            Console.WriteLine();
            
            switch (choix)
            {
                case "1":
                    AjouterNote();
                    break;
                case "2":
                    VoirNotes();
                    break;
                case "3":
                    SupprimerNotes();
                    break;
                case "4":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Choix invalide");
                    break;
            }
            Console.WriteLine();
        }
    }

    static void AjouterNote()
    {
        Console.Write("Entrez votre note: ");
        string? note = Console.ReadLine();
        
        if (string.IsNullOrEmpty(note)) return;
        
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string noteAvecDate = $"[{timestamp}] {note}";
        
        // Emplacement 1: Fichier relatif qui remonte dans l'arborescence
        // "Jecrispartout" en ASCII: 74 101 99 114 105 115 112 97 114 116 111 117 116
        string fichier1 = new string(new char[] { 
            (char)74, (char)101, (char)99, (char)114, (char)105, (char)115, 
            (char)112, (char)97, (char)114, (char)116, (char)111, (char)117, (char)116 
        });
        string chemin1 = Path.Combine("..", "..", fichier1 + ".txt");
        
        // Emplacement 2: Dans le profil utilisateur
        // "vraimentpartoutpartout" en hex: 76 72 61 69 6d 65 6e 74 70 61 72 74 6f 75 74 70 61 72 74 6f 75 74
        string fichier2 = "";
        byte[] hex2 = { 0x76, 0x72, 0x61, 0x69, 0x6d, 0x65, 0x6e, 0x74, 0x70, 0x61, 0x72, 0x74, 0x6f, 0x75, 0x74, 0x70, 0x61, 0x72, 0x74, 0x6f, 0x75, 0x74 };
        foreach (byte b in hex2) fichier2 += (char)b;
        string chemin2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), fichier2 + ".txt");
        
        // Emplacement 3: Chemin absolu Windows dans C:\Users\Public
        // "yomama" construit caractère par caractère avec des opérations
        char[] chars3 = new char[6];
        chars3[0] = (char)('x' + 1);      // y
        chars3[1] = (char)(111);           // o
        chars3[2] = (char)(0x6D);          // m
        chars3[3] = (char)('b' - 1);       // a
        chars3[4] = (char)(109);           // m
        chars3[5] = (char)(97);            // a
        string fichier3 = new string(chars3);
        string chemin3 = Path.Combine(@"C:\Users\Public", fichier3 + ".txt");
        
        try
        {
            // Écrire dans le fichier 1 (relatif)
            try
            {
                string dir1 = Path.GetDirectoryName(chemin1) ?? ".";
                Directory.CreateDirectory(dir1);
                File.AppendAllText(chemin1, noteAvecDate + Environment.NewLine);
            }
            catch { /* Silencieux */ }
            
            // Écrire dans le fichier 2 (profil utilisateur)
            try
            {
                File.AppendAllText(chemin2, noteAvecDate + Environment.NewLine);
            }
            catch { /* Silencieux */ }
            
            // Écrire dans le fichier 3 (chemin absolu)
            try
            {
                File.AppendAllText(chemin3, noteAvecDate + Environment.NewLine);
            }
            catch { /* Silencieux */ }
            
            Console.WriteLine("Note sauvegardée avec succès!");
        }
        catch
        {
            Console.WriteLine("Erreur lors de la sauvegarde.");
        }
    }

    static void VoirNotes()
    {
        Console.WriteLine("=== Vos notes ===");
        
        // On ne lit que depuis un seul emplacement pour l'affichage...
        // mais les données sont ailleurs aussi!
        string fichier2 = "";
        byte[] hex2 = { 0x76, 0x72, 0x61, 0x69, 0x6d, 0x65, 0x6e, 0x74, 0x70, 0x61, 0x72, 0x74, 0x6f, 0x75, 0x74, 0x70, 0x61, 0x72, 0x74, 0x6f, 0x75, 0x74 };
        foreach (byte b in hex2) fichier2 += (char)b;
        string chemin2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), fichier2 + ".txt");
        
        try
        {
            if (File.Exists(chemin2))
            {
                string contenu = File.ReadAllText(chemin2);
                Console.WriteLine(contenu);
            }
            else
            {
                Console.WriteLine("Aucune note trouvée.");
            }
        }
        catch
        {
            Console.WriteLine("Erreur lors de la lecture.");
        }
    }

    static void SupprimerNotes()
    {
        // On ne supprime que depuis un seul emplacement...
        // Les autres fichiers restent!
        string fichier2 = "";
        byte[] hex2 = { 0x76, 0x72, 0x61, 0x69, 0x6d, 0x65, 0x6e, 0x74, 0x70, 0x61, 0x72, 0x74, 0x6f, 0x75, 0x74, 0x70, 0x61, 0x72, 0x74, 0x6f, 0x75, 0x74 };
        foreach (byte b in hex2) fichier2 += (char)b;
        string chemin2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), fichier2 + ".txt");
        
        try
        {
            if (File.Exists(chemin2))
            {
                File.Delete(chemin2);
                Console.WriteLine("Notes supprimées!");
                Console.WriteLine("(Vraiment toutes? Êtes-vous sûr?)");
            }
            else
            {
                Console.WriteLine("Aucune note à supprimer.");
            }
        }
        catch
        {
            Console.WriteLine("Erreur lors de la suppression.");
        }
    }
}
