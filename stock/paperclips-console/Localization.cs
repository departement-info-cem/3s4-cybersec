using System.Collections.Generic;

namespace PaperclipsConsole
{
    public enum Language
    {
        French,
        English
    }

    public static class Localization
    {
        public static Language CurrentLanguage { get; set; } = Language.French;

        private static readonly Dictionary<string, Dictionary<Language, string>> Strings = new Dictionary<string, Dictionary<Language, string>>
        {
            // Menu
            { "Menu_Continue", new Dictionary<Language, string> { { Language.French, "Continuer la partie" }, { Language.English, "Continue Game" } } },
            { "Menu_NewGame", new Dictionary<Language, string> { { Language.French, "Nouvelle partie" }, { Language.English, "New Game" } } },
            { "Menu_Quit", new Dictionary<Language, string> { { Language.French, "Quitter" }, { Language.English, "Quit" } } },
            { "Menu_Language", new Dictionary<Language, string> { { Language.French, "Langue / Language" }, { Language.English, "Language / Langue" } } },
            { "Menu_Encryption", new Dictionary<Language, string> { { Language.French, "Chiffrement Sauvegarde: {0}" }, { Language.English, "Save Encryption: {0}" } } },
            { "Menu_Enabled", new Dictionary<Language, string> { { Language.French, "Activé" }, { Language.English, "Enabled" } } },
            { "Menu_Disabled", new Dictionary<Language, string> { { Language.French, "Désactivé" }, { Language.English, "Disabled" } } },
            { "Menu_Prompt", new Dictionary<Language, string> { { Language.French, "Que voulez-vous faire?" }, { Language.English, "What would you like to do?" } } },
            { "Menu_MoreChoices", new Dictionary<Language, string> { { Language.French, "(Utilisez les flèches pour naviguer)" }, { Language.English, "(Use arrow keys to navigate)" } } },
            { "Menu_Warning", new Dictionary<Language, string> { { Language.French, "ATTENTION" }, { Language.English, "WARNING" } } },
            { "Menu_OverwriteWarning", new Dictionary<Language, string> { { Language.French, "Commencer une nouvelle partie [red]écrasera[/] votre sauvegarde actuelle!\n\nÊtes-vous sûr de vouloir continuer?" }, { Language.English, "Starting a new game will [red]overwrite[/] your current save!\n\nAre you sure you want to continue?" } } },
            { "Menu_Confirmation", new Dictionary<Language, string> { { Language.French, "CONFIRMATION" }, { Language.English, "CONFIRMATION" } } },
            { "Menu_ConfirmNewGame", new Dictionary<Language, string> { { Language.French, "Confirmer nouvelle partie?" }, { Language.English, "Confirm new game?" } } },
            { "Menu_Yes", new Dictionary<Language, string> { { Language.French, "o" }, { Language.English, "y" } } },
            { "Menu_No", new Dictionary<Language, string> { { Language.French, "n" }, { Language.English, "n" } } },

            // Game Messages
            { "Msg_GameLoaded", new Dictionary<Language, string> { { Language.French, "Partie chargée avec succès!" }, { Language.English, "Game loaded successfully!" } } },
            { "Msg_LoadError", new Dictionary<Language, string> { { Language.French, "Erreur lors du chargement: sauvegarde invalide ou mal chiffrée.\nLe fichier a été conservé dans {0}\nUne nouvelle partie sera créée. Appuyez sur une touche..." }, { Language.English, "Error loading game: invalid or badly encrypted save.\nThe file was kept in {0}\nA new game will be created. Press any key..." } } },
            { "Msg_NewGameStarted", new Dictionary<Language, string> { { Language.French, "Nouvelle partie créée avec $20 de départ!" }, { Language.English, "New game created with $20 start!" } } },
            { "Msg_GameSaved", new Dictionary<Language, string> { { Language.French, "\n[Partie sauvegardée]" }, { Language.English, "\n[Game Saved]" } } },
            { "Msg_SaveError", new Dictionary<Language, string> { { Language.French, "\nErreur lors de la sauvegarde: {0}" }, { Language.English, "\nError saving game: {0}" } } },
            { "Msg_NoWire", new Dictionary<Language, string> { { Language.French, "\nPas assez de wire!" }, { Language.English, "\nNot enough wire!" } } },
            { "Msg_NoFunds", new Dictionary<Language, string> { { Language.French, "\nPas assez de fonds!" }, { Language.English, "\nNot enough funds!" } } },
            { "Msg_NoTrust", new Dictionary<Language, string> { { Language.French, "\nPas assez de trust!" }, { Language.English, "\nNot enough trust!" } } },
            { "Msg_TrustIncreased", new Dictionary<Language, string> { { Language.French, "\n*** TRUST AUGMENTÉ! Nouveau trust: {0} ***" }, { Language.English, "\n*** TRUST INCREASED! New trust: {0} ***" } } },
            { "Msg_SaveQuit", new Dictionary<Language, string> { { Language.French, "\nSauvegarder avant de quitter? (O/N)" }, { Language.English, "\nSave before quitting? (Y/N)" } } },

            // UI Headers
            { "UI_ProductionBusiness", new Dictionary<Language, string> { { Language.French, "PRODUCTION & BUSINESS" }, { Language.English, "PRODUCTION & BUSINESS" } } },
            { "UI_ComputingResources", new Dictionary<Language, string> { { Language.French, "COMPUTING & RESOURCES" }, { Language.English, "COMPUTING & RESOURCES" } } },
            { "UI_Resources", new Dictionary<Language, string> { { Language.French, " Ressources " }, { Language.English, " Resources " } } },
            { "UI_Computing", new Dictionary<Language, string> { { Language.French, " Computing " }, { Language.English, " Computing " } } },

            // UI Labels
            { "Lbl_Paperclips", new Dictionary<Language, string> { { Language.French, "Paperclips:" }, { Language.English, "Paperclips:" } } },
            { "Lbl_ClipsPerSec", new Dictionary<Language, string> { { Language.French, "Clips/sec:" }, { Language.English, "Clips/sec:" } } },
            { "Lbl_Funds", new Dictionary<Language, string> { { Language.French, "Fonds:" }, { Language.English, "Funds:" } } },
            { "Lbl_PricePerClip", new Dictionary<Language, string> { { Language.French, "Prix/clip:" }, { Language.English, "Price/clip:" } } },
            { "Lbl_Demand", new Dictionary<Language, string> { { Language.French, "Demande:" }, { Language.English, "Demand:" } } },
            { "Lbl_Inventory", new Dictionary<Language, string> { { Language.French, "Inventaire:" }, { Language.English, "Inventory:" } } },
            { "Lbl_Wire", new Dictionary<Language, string> { { Language.French, "Wire:" }, { Language.English, "Wire:" } } },
            { "Lbl_WirePrice", new Dictionary<Language, string> { { Language.French, "Wire prix:" }, { Language.English, "Wire cost:" } } },
            { "Lbl_AutoClippers", new Dictionary<Language, string> { { Language.French, "AutoClippers:" }, { Language.English, "AutoClippers:" } } },
            { "Lbl_MegaClippers", new Dictionary<Language, string> { { Language.French, "MegaClippers:" }, { Language.English, "MegaClippers:" } } },
            { "Lbl_Marketing", new Dictionary<Language, string> { { Language.French, "Marketing:" }, { Language.English, "Marketing:" } } },
            { "Lbl_Level", new Dictionary<Language, string> { { Language.French, "Niv. {0}" }, { Language.English, "Lvl. {0}" } } },
            { "Lbl_Trust", new Dictionary<Language, string> { { Language.French, "Trust:" }, { Language.English, "Trust:" } } },
            { "Lbl_Processors", new Dictionary<Language, string> { { Language.French, "Processeurs:" }, { Language.English, "Processors:" } } },
            { "Lbl_Memory", new Dictionary<Language, string> { { Language.French, "Mémoire:" }, { Language.English, "Memory:" } } },
            { "Lbl_Operations", new Dictionary<Language, string> { { Language.French, "Operations:" }, { Language.English, "Operations:" } } },
            { "Lbl_Creativity", new Dictionary<Language, string> { { Language.French, "Créativité:" }, { Language.English, "Creativity:" } } },
            { "Lbl_Used", new Dictionary<Language, string> { { Language.French, "Utilisé" }, { Language.English, "Used" } } },
            { "Lbl_Free", new Dictionary<Language, string> { { Language.French, "Libre" }, { Language.English, "Free" } } },

            // Commands
            { "Cmd_Clips", new Dictionary<Language, string> { { Language.French, "Clips" }, { Language.English, "Clips" } } },
            { "Cmd_Wire", new Dictionary<Language, string> { { Language.French, "Wire" }, { Language.English, "Wire" } } },
            { "Cmd_Auto", new Dictionary<Language, string> { { Language.French, "Auto" }, { Language.English, "Auto" } } },
            { "Cmd_Mega", new Dictionary<Language, string> { { Language.French, "Mega" }, { Language.English, "Mega" } } },
            { "Cmd_Marketing", new Dictionary<Language, string> { { Language.French, "Marketing" }, { Language.English, "Marketing" } } },
            { "Cmd_Price", new Dictionary<Language, string> { { Language.French, "Prix" }, { Language.English, "Price" } } },
            { "Cmd_Proc", new Dictionary<Language, string> { { Language.French, "Proc" }, { Language.English, "Proc" } } },
            { "Cmd_Mem", new Dictionary<Language, string> { { Language.French, "Mem" }, { Language.English, "Mem" } } },
            { "Cmd_Save", new Dictionary<Language, string> { { Language.French, "Save" }, { Language.English, "Save" } } },
            { "Cmd_Quit", new Dictionary<Language, string> { { Language.French, "Quit" }, { Language.English, "Quit" } } },
            { "Cmd_Menu", new Dictionary<Language, string> { { Language.French, "Menu" }, { Language.English, "Menu" } } },
            { "Cmd_SpaceMenu", new Dictionary<Language, string> { { Language.French, "[grey]Space[/]Menu" }, { Language.English, "[grey]Space[/]Menu" } } },

            // Intro/Outro
            { "Intro_Objective", new Dictionary<Language, string> { { Language.French, "[cyan]Objectif:[/] Produire des paperclips et dominer le marché" }, { Language.English, "[cyan]Objective:[/] Produce paperclips and dominate the market" } } },
            { "Intro_QuickCommands", new Dictionary<Language, string> { { Language.French, "[yellow]COMMANDES RAPIDES:[/]" }, { Language.English, "[yellow]QUICK COMMANDS:[/]" } } },
            { "Intro_Cmd_P", new Dictionary<Language, string> { { Language.French, "  [green]P[/] - Créer un paperclip" }, { Language.English, "  [green]P[/] - Make a paperclip" } } },
            { "Intro_Cmd_W", new Dictionary<Language, string> { { Language.French, "  [green]W[/] - Acheter du wire" }, { Language.English, "  [green]W[/] - Buy wire" } } },
            { "Intro_Cmd_Price", new Dictionary<Language, string> { { Language.French, "  [green]+/-[/] - Modifier le prix" }, { Language.English, "  [green]+/-[/] - Adjust price" } } },
            { "Intro_MenuHint", new Dictionary<Language, string> { { Language.French, "[grey]Appuyez sur [green]Space[/] pour le menu complet[/]" }, { Language.English, "[grey]Press [green]Space[/] for full menu[/]" } } },
            { "Intro_Start", new Dictionary<Language, string> { { Language.French, "[grey]Appuyez sur une touche pour commencer...[/]" }, { Language.English, "[grey]Press any key to start...[/]" } } },
            { "Outro_Thanks", new Dictionary<Language, string> { { Language.French, "[green]Merci d'avoir joué à Universal Paperclips![/]" }, { Language.English, "[green]Thanks for playing Universal Paperclips![/]" } } },
            { "Outro_Saved", new Dictionary<Language, string> { { Language.French, "[grey]Vos données ont été sauvegardées.[/]" }, { Language.English, "[grey]Your data has been saved.[/]" } } },
            { "Outro_SeeYou", new Dictionary<Language, string> { { Language.French, "\n  [yellow]À bientôt![/]\n" }, { Language.English, "\n  [yellow]See you soon![/]\n" } } },
        };

        public static string Get(string key)
        {
            if (Strings.TryGetValue(key, out var dict))
            {
                if (dict.TryGetValue(CurrentLanguage, out var value))
                {
                    return value;
                }
            }
            return key;
        }

        public static string Get(string key, params object[] args)
        {
            var format = Get(key);
            return string.Format(format, args);
        }
    }
}
