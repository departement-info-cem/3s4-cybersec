// CTF Challenge: Décompilation
// Trouvez le secret caché dans ce programme!

class Program
{
    // Le secret est bien caché... ou pas?
    private static readonly string _secret = "dans le .exe tes secrets tu ne garderas pas";
    
    static void Main(string[] args)
    {
        Console.WriteLine("=== Bienvenue dans le CTF Décompilation ===");
        Console.WriteLine();
        Console.WriteLine("Ce programme contient un secret.");
        Console.WriteLine("Votre mission: trouver la phrase secrète cachée dans l'exécutable.");
        Console.WriteLine();
        Console.WriteLine("Indice: Un décompilateur .NET comme dnSpy ou ILSpy pourrait vous aider...");
        Console.WriteLine();
        
        Console.Write("Entrez le secret pour valider: ");
        string? input = Console.ReadLine();
        
        if (input == _secret)
        {
            Console.WriteLine();
            Console.WriteLine("🎉 BRAVO! Vous avez trouvé le secret!");
            Console.WriteLine("Vous gagnez 1 point au tableau des scores.");
            Console.WriteLine();
            Console.WriteLine("Leçon apprise: Ne jamais stocker de secrets en clair dans le code source.");
            Console.WriteLine("Les applications .NET peuvent être facilement décompilées.");
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine("❌ Ce n'est pas le bon secret. Continuez à chercher!");
        }
        
        Console.WriteLine();
        Console.WriteLine("Appuyez sur une touche pour quitter...");
        Console.ReadKey();
    }
}
