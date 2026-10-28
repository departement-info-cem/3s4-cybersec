using System;
using System.Threading;
using Spectre.Console;

namespace PaperclipsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // Configuration
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Universal Paperclips - Console Edition";
            
            var game = new GameManager();
            
            // Show start menu with Spectre.Console
            var choice = StartMenu.ShowMenu(game.SaveFileExists(), game.State);
            
            if (choice == MenuChoice.Quit)
            {
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine(Localization.Get("Outro_SeeYou"));
                Thread.Sleep(1000);
                return;
            }

            // Load or start new game based on choice
            if (choice == MenuChoice.Continue)
            {
                game.LoadGame();
            }
            else // NewGame
            {
                game.StartNewGame();
            }

            // Show brief intro with Spectre.Console
            AnsiConsole.Clear();
            
            var introPanel = new Panel(
                $"{Localization.Get("Intro_Objective")}\n\n" +
                $"{Localization.Get("Intro_QuickCommands")}\n" +
                $"{Localization.Get("Intro_Cmd_P")}\n" +
                $"{Localization.Get("Intro_Cmd_W")}\n" +
                $"{Localization.Get("Intro_Cmd_Price")}\n\n" +
                $"{Localization.Get("Intro_MenuHint")}")
            {
                Header = new PanelHeader(" UNIVERSAL PAPERCLIPS ", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Cyan1)
            };
            
            AnsiConsole.Write(introPanel);
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine(Localization.Get("Intro_Start"));
            Console.ReadKey(true);

            // Clear screen before starting
            AnsiConsole.Clear();

            Thread gameLoop = new Thread(() =>
            {
                while (game.IsRunning)
                {
                    game.Update();
                    Thread.Sleep(100);
                }
            });
            gameLoop.Start();

            while (game.IsRunning)
            {
                // Use Spectre.Console display instead of old DisplayStatus
                SpectreDisplay.ShowStatus(game.State);
                
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    
                    game.CheckQuit(key);
                    if (!game.IsRunning) break;
                    
                    game.CheckMenu(key);
                    game.ProcessInput(key);
                }
                
                Thread.Sleep(50);
            }

            gameLoop.Join();
            
            AnsiConsole.Clear();
            var endPanel = new Panel($"{Localization.Get("Outro_Thanks")}\n{Localization.Get("Outro_Saved")}")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Green)
            };
            AnsiConsole.Write(endPanel);
            Thread.Sleep(2000);
        }
    }
}
