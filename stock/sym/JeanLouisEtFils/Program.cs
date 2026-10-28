// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;


using System;
using System.Collections.Generic;
using Sharprompt;
using JeanLouisEtFils;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Bienvenue dans l'gestionnaire de passwords à Jean-Louis!");
        Console.WriteLine("Top secure, nous on encrypte avec AES-256!");
        bool quit = false;
        do
        {
            quit = MainMenu();
        } while (!quit);
    }
    
    public static bool MainMenu() {
        var choix = Prompt.Select("Quelle action veux-tu effectuer ? " , 
            new[] { 
                "Ajouter un nouveau mot de passe sécurisé",
                "Tout supprimer",
                "Récupérer un mot de passe sécurisé que j'ai oublié",
                "Quitter"
            });
        switch (choix)
        {
            case "Ajouter un nouveau mot de passe sécurisé": Ajouter(); break;
            case "Tout supprimer": Vider(); break;
            case "Récupérer un mot de passe sécurisé que j'ai oublié": Recuperer(); break;
            case "Quitter": Quitter(); break;
        }
        return false;
    }

    private static void Ajouter()
    {
        string userHomeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string filePath = System.IO.Path.Combine(userHomeDirectory, "encrypted-passwords.txt");

        // if the file encrypted-passwords.txt does not exist in the user's home directory, create it
        if (!System.IO.File.Exists(filePath))
        {
            System.IO.File.Create(filePath).Dispose();
        }
        
        Formulaires.NouveauMotDePasse result = Prompt.Bind<Formulaires.NouveauMotDePasse>();
        if (result.MotDePasse != result.MotDePasseConfirmation)
        {
            Console.WriteLine("Désolé, les mots de passe ne correspondent pas");
            return;
        }
        // encrypter le mot de passe avec AES
        AesCrypto aes = new AesCrypto();
        string encrypted = aes.Encrypt(result.MotDePasse);
        // ajouter le mot de passe encrypté dans le fichier encrypted-passwords.txt avec le site sur la même ligne apres un @
        System.IO.File.AppendAllText(filePath, encrypted + " @ " + result.Site + Environment.NewLine);
        Console.Clear();
        Console.WriteLine("Mot de passe ajouté avec succès!");
    }

    private static void Vider()
    {
        string userHomeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string filePath = System.IO.Path.Combine(userHomeDirectory, "encrypted-passwords.txt");
        // empty the file encrypted-passwords.txt
        System.IO.File.WriteAllText(filePath, string.Empty);
    }

    private static void Recuperer()
    {
        Console.Clear();
        Console.WriteLine("Voici les instructions pour récupérer un mot de passe");
        Console.WriteLine("1. Localiser le fichier encrypted-passwords.txt");
        Console.WriteLine("2. Commencer un courriel à l'adresse jean-louis@jlf.com");
        Console.WriteLine("3. Ajouter le fichier en pièce jointe");
        Console.WriteLine("4. Envoyer le courriel");
        Console.WriteLine("Nous vous répondrons dans les 48 heures, parole de Jean-Louis!");
    }

    private static void Quitter()
    {
        Environment.Exit(0);
    }
}