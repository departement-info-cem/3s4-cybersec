using System;
using System.Collections.Generic;
using Spectre.Console;

namespace PaperclipsConsole
{
    public static class StartMenu
    {
        public static MenuChoice ShowMenu(bool saveExists, GameState state)
        {
            // Menu choices
            while (true)
            {
                AnsiConsole.Clear();
                
                // ASCII Art Title
                var title = new FigletText("PAPERCLIPS")
                    .Centered()
                    .Color(Color.Cyan1);
                AnsiConsole.Write(title);
                
                AnsiConsole.Write(
                    new FigletText("Console Edition")
                    .Centered()
                    .Color(Color.Grey));
                
                AnsiConsole.WriteLine();
                AnsiConsole.WriteLine();

                var choices = new List<string>();
                if (saveExists) choices.Add(Localization.Get("Menu_Continue"));
                choices.Add(Localization.Get("Menu_NewGame"));
                choices.Add(Localization.Get("Menu_Language"));
                
                string encryptionStatus = state.EncryptSave ? Localization.Get("Menu_Enabled") : Localization.Get("Menu_Disabled");
                string encryptionOption = Localization.Get("Menu_Encryption", encryptionStatus);
                choices.Add(encryptionOption);
                
                choices.Add(Localization.Get("Menu_Quit"));
                
                var selection = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title($"[cyan1]{Localization.Get("Menu_Prompt")}[/]")
                        .PageSize(10)
                        .MoreChoicesText($"[grey]{Localization.Get("Menu_MoreChoices")}[/]")
                        .HighlightStyle(new Style(Color.Black, Color.Cyan1, Decoration.Bold))
                        .AddChoices(choices));
                
                // Process choice
                if (selection == Localization.Get("Menu_Continue"))
                {
                    return MenuChoice.Continue;
                }
                else if (selection == Localization.Get("Menu_NewGame"))
                {
                    if (saveExists)
                    {
                        if (ConfirmNewGame()) return MenuChoice.NewGame;
                    }
                    else
                    {
                        return MenuChoice.NewGame;
                    }
                }
                else if (selection == Localization.Get("Menu_Language"))
                {
                    Localization.CurrentLanguage = Localization.CurrentLanguage == Language.French ? Language.English : Language.French;
                }
                else if (selection == encryptionOption)
                {
                    state.EncryptSave = !state.EncryptSave;
                }
                else // Quitter
                {
                    return MenuChoice.Quit;
                }
            }
        }

        private static bool ConfirmNewGame()
        {
            AnsiConsole.Clear();
            
            var panel = new Panel(
                new Markup(
                    $"[red]⚠️  {Localization.Get("Menu_Warning")} ⚠️[/]\n\n" +
                    $"{Localization.Get("Menu_OverwriteWarning")}"))
            {
                Header = new PanelHeader($" [red]{Localization.Get("Menu_Confirmation")}[/] ", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Red),
                Padding = new Padding(2, 1)
            };
            
            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
            
            var prompt = new ConfirmationPrompt($"[yellow]{Localization.Get("Menu_ConfirmNewGame")}[/]")
            {
                Yes = Localization.Get("Menu_Yes")[0],
                No = Localization.Get("Menu_No")[0]
            };
            
            return AnsiConsole.Prompt(prompt);
        }
    }

    public enum MenuChoice
    {
        Continue,
        NewGame,
        Quit
    }
}
