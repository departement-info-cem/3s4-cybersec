using System;
using Spectre.Console;

namespace PaperclipsConsole
{
    public static class SpectreDisplay
    {
        private static bool firstDisplay = true;
        
        public static void ShowStatus(GameState state)
        {
            // Only clear on first display to prevent flicker
            if (firstDisplay)
            {
                AnsiConsole.Clear();
                firstDisplay = false;
            }
            else
            {
                // Move cursor to home instead of clearing
                Console.SetCursorPosition(0, 0);
            }
            
            Console.CursorVisible = false;
            
            Console.CursorVisible = false;
            
            // Don't show title every frame (causes flicker)
            // Title shown only in StartMenu now
            
            // Main panel
            var grid = new Grid();
            grid.AddColumn(new GridColumn().Width(40));
            grid.AddColumn(new GridColumn().Width(40));
            
            // Left column - Production & Business
            var leftPanel = new Panel(CreateProductionTable(state))
            {
                Header = new PanelHeader($" [cyan1]{Localization.Get("UI_ProductionBusiness")}[/] ", Justify.Center),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Cyan1)
            };
            
            // Right column - Computing & Resources
            var rightPanel = new Panel(CreateComputingTable(state))
            {
                Header = new PanelHeader($" [yellow1]{Localization.Get("UI_ComputingResources")}[/] ", Justify.Center),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Yellow1)
            };
            
            grid.AddRow(leftPanel, rightPanel);
            
            AnsiConsole.Write(grid);
            
            // Progress bars
            ShowProgressBars(state);
            
            // Commands
            ShowCommands();
        }

        private static Table CreateProductionTable(GameState state)
        {
            var table = new Table()
                .Border(TableBorder.None)
                .HideHeaders();
            
            table.AddColumn(new TableColumn("").Width(18));
            table.AddColumn(new TableColumn("").Width(20));
            
            // Clips
            table.AddRow(
                $"[bold]{Localization.Get("Lbl_Paperclips")}[/]",
                $"[green]{state.Clips:N0}[/]"
            );
            
            table.AddRow(
                $"[bold]{Localization.Get("Lbl_ClipsPerSec")}[/]",
                $"[green]{state.ClipRate:F2}[/]"
            );
            
            table.AddEmptyRow();
            
            // Funds & Business
            table.AddRow(
                $"[bold]{Localization.Get("Lbl_Funds")}[/]",
                $"[yellow]${state.Funds:N2}[/]"
            );
            
            table.AddRow(
                $"[bold]{Localization.Get("Lbl_PricePerClip")}[/]",
                $"[yellow]${state.Margin:F2}[/]"
            );
            
            table.AddRow(
                $"[bold]{Localization.Get("Lbl_Demand")}[/]",
                $"[yellow]{state.Demand}%[/]"
            );
            
            table.AddRow(
                $"[bold]{Localization.Get("Lbl_Inventory")}[/]",
                $"[yellow]{state.UnsoldClips:N0}[/]"
            );
            
            table.AddEmptyRow();
            
            // Wire
            table.AddRow(
                $"[bold]{Localization.Get("Lbl_Wire")}[/]",
                $"[cyan]{state.Wire:N0}[/] inches"
            );
            
            table.AddRow(
                $"[bold]{Localization.Get("Lbl_WirePrice")}[/]",
                $"[cyan]${state.WireCost:F2}[/] x{state.WireAmount}"
            );
            
            table.AddEmptyRow();
            
            // AutoClippers
            if (state.ClipmakerLevel > 0 || state.Funds >= 5)
            {
                table.AddRow(
                    $"[bold]{Localization.Get("Lbl_AutoClippers")}[/]",
                    $"[green]{state.ClipmakerLevel}[/] (${state.ClipperCost:N2})"
                );
            }
            
            // MegaClippers
            if (state.MegaClipperLevel > 0 || state.Funds >= 500)
            {
                table.AddRow(
                    $"[bold]{Localization.Get("Lbl_MegaClippers")}[/]",
                    $"[green]{state.MegaClipperLevel}[/] (${state.MegaClipperCost:N0})"
                );
            }
            
            // Marketing
            table.AddEmptyRow();
            table.AddRow(
                $"[bold]{Localization.Get("Lbl_Marketing")}[/]",
                $"[yellow]{Localization.Get("Lbl_Level", state.MarketingLevel)}[/] (${state.AdCost:N2})"
            );
            
            return table;
        }

        private static Table CreateComputingTable(GameState state)
        {
            var table = new Table()
                .Border(TableBorder.None)
                .HideHeaders();
            
            table.AddColumn(new TableColumn("").Width(18));
            table.AddColumn(new TableColumn("").Width(20));
            
            // Trust
            table.AddRow(
                $"[bold]{Localization.Get("Lbl_Trust")}[/]",
                $"[cyan]{state.Trust}[/] → [dim]{state.NextTrust:N0}[/]"
            );
            
            table.AddEmptyRow();
            
            // Processors & Memory
            table.AddRow(
                $"[bold]{Localization.Get("Lbl_Processors")}[/]",
                $"[green]{state.Processors}[/]"
            );
            
            table.AddRow(
                $"[bold]{Localization.Get("Lbl_Memory")}[/]",
                $"[green]{state.Memory}[/]"
            );
            
            table.AddEmptyRow();
            
            // Operations
            table.AddRow(
                $"[bold]{Localization.Get("Lbl_Operations")}[/]",
                $"[yellow]{state.Operations:N0}[/] / {state.MaxOps:N0}"
            );
            
            // Creativity
            if (state.Creativity > 0 || state.Processors >= 5)
            {
                table.AddRow(
                    $"[bold]{Localization.Get("Lbl_Creativity")}[/]",
                    $"[magenta]{state.Creativity:N0}[/]"
                );
            }
            
            return table;
        }

        private static void ShowProgressBars(GameState state)
        {
            AnsiConsole.WriteLine();
            
            // Wire usage bar (as percentage of initial 1000)
            if (state.Wire > 0)
            {
                var wirePercent = Math.Min(100.0, (double)state.Wire / 1000.0 * 100.0);
                
                var wireBar = new BreakdownChart()
                    .Width(60)
                    .ShowPercentage()
                    .AddItem(Localization.Get("Cmd_Wire"), wirePercent, Color.Cyan1)
                    .AddItem(Localization.Get("Lbl_Used"), 100 - wirePercent, Color.Grey23);
                
                AnsiConsole.Write(new Panel(wireBar)
                {
                    Header = new PanelHeader(Localization.Get("UI_Resources"), Justify.Center),
                    Border = BoxBorder.None
                });
            }
            
            // Operations bar
            if (state.MaxOps > 0)
            {
                var opsPercent = Math.Min(100.0, (double)state.Operations / (double)state.MaxOps * 100.0);
                
                var opsBar = new BreakdownChart()
                    .Width(60)
                    .ShowPercentage()
                    .AddItem("Ops", opsPercent, Color.Yellow1)
                    .AddItem(Localization.Get("Lbl_Free"), 100 - opsPercent, Color.Grey23);
                
                AnsiConsole.Write(new Panel(opsBar)
                {
                    Header = new PanelHeader(Localization.Get("UI_Computing"), Justify.Center),
                    Border = BoxBorder.None
                });
            }
            
            AnsiConsole.WriteLine();
        }

        private static void ShowCommands()
        {
            var commandTable = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Grey)
                .HideHeaders()
                .Collapse();
            
            commandTable.AddColumn(new TableColumn(""));
            
            commandTable.AddRow($"[cyan]P[/]{Localization.Get("Cmd_Clips")} [cyan]W[/]{Localization.Get("Cmd_Wire")} [cyan]A[/]{Localization.Get("Cmd_Auto")} [cyan]G[/]{Localization.Get("Cmd_Mega")} [cyan]M[/]{Localization.Get("Cmd_Marketing")} [cyan]+/-[/]{Localization.Get("Cmd_Price")} [cyan]T[/]{Localization.Get("Cmd_Proc")} [cyan]Y[/]{Localization.Get("Cmd_Mem")}");
            commandTable.AddRow($"[yellow]S[/]{Localization.Get("Cmd_Save")} [red]Q[/]{Localization.Get("Cmd_Quit")} {Localization.Get("Cmd_SpaceMenu")}");
            
            AnsiConsole.Write(commandTable);
        }

        public static void ShowMenu(bool saveExists)
        {
            AnsiConsole.Clear();
            
            var title = new FigletText("PAPERCLIPS")
                .Centered()
                .Color(Color.Cyan1);
            AnsiConsole.Write(title);
            
            AnsiConsole.WriteLine();
            AnsiConsole.Write(
                new FigletText("Console Edition")
                .Centered()
                .Color(Color.Grey));
            
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();
            
            var choices = saveExists
                ? new[] { "Continuer la partie", "Nouvelle partie", "Quitter" }
                : new[] { "Nouvelle partie", "Quitter" };
            
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[cyan1]Que voulez-vous faire?[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(Color.Black, Color.White))
                    .AddChoices(choices));
            
            // Return choice as MenuChoice enum would go here
            // For now this is a demo
        }

        public static bool ConfirmNewGame()
        {
            AnsiConsole.Clear();
            
            var panel = new Panel(
                new Markup(
                    "[red]⚠️  ATTENTION ⚠️[/]\n\n" +
                    "Commencer une nouvelle partie [red]écrasera[/] votre sauvegarde actuelle!\n\n" +
                    "Êtes-vous sûr de vouloir continuer?"))
            {
                Header = new PanelHeader(" [red]CONFIRMATION[/] ", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Red)
            };
            
            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
            
            return AnsiConsole.Confirm("Confirmer nouvelle partie?", false);
        }
    }
}
