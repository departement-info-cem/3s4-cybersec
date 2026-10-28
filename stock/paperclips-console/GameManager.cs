using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace PaperclipsConsole
{
    public class GameManager
    {
        // Expose state publicly for Spectre.Console display
        public GameState State => state;
        
        private GameState state;
        private readonly string saveDirectory;
        private readonly string savePath;
        private DateTime lastUpdate;
        private DateTime lastAutoSave;
        private DateTime lastDisplay;
        private Random random;
        private bool needsRedraw = true;
        private static readonly CultureInfo invCulture = CultureInfo.InvariantCulture;

        // Hardcoded Key and IV for AES
        private static readonly byte[] AesKey = new byte[]
        {
            0xD1, 0x68, 0x34, 0x00, 0x74, 0xEE, 0x08, 0xBF,
            0x6E, 0xE6, 0xB3, 0x09, 0xCE, 0x5C, 0x9C, 0x2D,
            0xBA, 0x46, 0x4A, 0x70, 0xBA, 0x8B, 0xA3, 0x99,
            0x6B, 0x1C, 0xDC, 0x7D, 0x9B, 0x57, 0x07, 0x97
        };
        
        private static readonly byte[] AesIV = new byte[] 
        { 
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 
            0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F 
        };

        public GameManager()
        {
            saveDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "papaclip"
            );
            savePath = Path.Combine(saveDirectory, "data.json");
            random = new Random();
            // Ne pas charger automatiquement
            lastUpdate = DateTime.Now;
            lastAutoSave = DateTime.Now;
            lastDisplay = DateTime.Now;
            
            // Initialize state (try load or create default)
            state = TryLoadState() ?? CreateNewGameState();
            Localization.CurrentLanguage = state.Language;
        }

        public bool SaveFileExists()
        {
            return File.Exists(savePath);
        }

        public void StartNewGame()
        {
            // Preserve encryption setting if possible
            bool encrypt = state?.EncryptSave ?? false;
            state = CreateNewGameState();
            state.EncryptSave = encrypt;
            SaveGame();
        }

        private GameState TryLoadState()
        {
            if (!File.Exists(savePath)) return null;
            
            try
            {
                string json;
                try 
                {
                    // Try reading as plain text first
                    json = File.ReadAllText(savePath);
                    // Validate if it's JSON
                    JsonDocument.Parse(json);
                }
                catch (Exception)
                {
                    // If not valid JSON or read error, try decrypting
                    byte[] encryptedBytes = File.ReadAllBytes(savePath);
                    json = DecryptStringFromBytes_Aes(encryptedBytes, AesKey, AesIV);
                }

                return JsonSerializer.Deserialize<GameState>(json);
            }
            catch
            {
                return null;
            }
        }

        public void LoadGame()
        {
            // State is already loaded in constructor, but we might want to reload if file changed
            // or just confirm it's loaded.
            // For simplicity, we'll just re-load to be safe and print the message.
            
            var loadedState = TryLoadState();
            if (loadedState != null)
            {
                state = loadedState;
                Localization.CurrentLanguage = state.Language;
                Console.WriteLine(Localization.Get("Msg_GameLoaded"));
            }
            else
            {
                // Keep the invalid save so the player can inspect it, and leave the message on screen
                File.Copy(savePath, savePath + ".invalide", true);
                Console.WriteLine(Localization.Get("Msg_LoadError", savePath + ".invalide"));
                Console.ReadKey(true);
                state = CreateNewGameState();
                SaveGame();
            }
        }

        private GameState CreateNewGameState()
        {
            return new GameState
            {
                Funds = 20.00,
                Wire = 1000,
                Margin = 0.25,
                Clips = 0,
                UnusedClips = 0,
                Demand = 10,
                UnsoldClips = 0,
                ClipmakerLevel = 0,
                ClipperCost = 5.00,
                MarketingLevel = 1,
                AdCost = 100.00,
                WireCost = 20,
                WireAmount = 1000,
                Processors = 1,
                Memory = 1,
                Operations = 0,
                Trust = 2,
                NextTrust = 1000,
                Creativity = 0,
                TotalClipsProduced = 0,
                MegaClipperLevel = 0,
                MegaClipperCost = 500,
                Language = Localization.CurrentLanguage
            };
        }

        // Helper methods for consistent number formatting
        private string FormatInt(long number)
        {
            return number.ToString("N0", invCulture);
        }

        private string FormatMoney(double amount)
        {
            return amount.ToString("F2", invCulture);
        }

        private string FormatDecimal(double amount, int decimals = 2)
        {
            return amount.ToString($"F{decimals}", invCulture);
        }

        public void SaveGame()
        {
            try
            {
                Directory.CreateDirectory(saveDirectory);
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(state, options);
                
                if (state.EncryptSave)
                {
                    byte[] encrypted = EncryptStringToBytes_Aes(json, AesKey, AesIV);
                    File.WriteAllBytes(savePath, encrypted);
                    Console.WriteLine(Localization.Get("Msg_GameSaved") + " [Encrypted]");
                }
                else
                {
                    File.WriteAllText(savePath, json);
                    Console.WriteLine(Localization.Get("Msg_GameSaved"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(Localization.Get("Msg_SaveError", ex.Message));
            }
        }

        private static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            // IV parameter is ignored — a fresh IV is generated per-encryption and prepended to the ciphertext

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.GenerateIV();

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        byte[] encrypted = msEncrypt.ToArray();
                        byte[] result = new byte[aesAlg.IV.Length + encrypted.Length];
                        Buffer.BlockCopy(aesAlg.IV, 0, result, 0, aesAlg.IV.Length);
                        Buffer.BlockCopy(encrypted, 0, result, aesAlg.IV.Length, encrypted.Length);
                        return result;
                    }
                }
            }
        }

        private static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            // IV parameter is ignored — extract per-message IV from the beginning of cipherText

            string plaintext = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;

                int ivLen = aesAlg.BlockSize / 8;
                if (cipherText.Length < ivLen) throw new ArgumentException("cipherText too short");
                byte[] ivBytes = new byte[ivLen];
                byte[] actualCipher = new byte[cipherText.Length - ivLen];
                Buffer.BlockCopy(cipherText, 0, ivBytes, 0, ivLen);
                Buffer.BlockCopy(cipherText, ivLen, actualCipher, 0, actualCipher.Length);

                aesAlg.IV = ivBytes;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(actualCipher))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        public void MakePaperclip()
        {
            if (state.Wire >= 1)
            {
                state.Wire--;
                state.Clips++;
                state.TotalClipsProduced++;
                state.UnsoldClips++;
                UpdateTrust();
                needsRedraw = true;
            }
            else
            {
                Console.WriteLine(Localization.Get("Msg_NoWire"));
            }
        }

        public void BuyWire()
        {
            if (state.Funds >= state.WireCost)
            {
                state.Funds -= state.WireCost;
                state.Wire += state.WireAmount;
                state.WireCost = Math.Round(state.WireCost * 1.05, 2);
                needsRedraw = true;
            }
            else
            {
                Console.WriteLine(Localization.Get("Msg_NoFunds"));
            }
        }

        public void BuyAutoClipper()
        {
            if (state.Funds >= state.ClipperCost)
            {
                state.Funds -= state.ClipperCost;
                state.ClipmakerLevel++;
                state.ClipperCost = Math.Round(Math.Pow(1.1, state.ClipmakerLevel) + 5, 2);
                needsRedraw = true;
            }
            else
            {
                Console.WriteLine(Localization.Get("Msg_NoFunds"));
            }
        }

        public void BuyMegaClipper()
        {
            if (state.Funds >= state.MegaClipperCost)
            {
                state.Funds -= state.MegaClipperCost;
                state.MegaClipperLevel++;
                state.MegaClipperCost = Math.Round(Math.Pow(1.07, state.MegaClipperLevel) * 1000, 2);
                needsRedraw = true;
            }
            else
            {
                Console.WriteLine(Localization.Get("Msg_NoFunds"));
            }
        }

        public void BuyMarketing()
        {
            if (state.Funds >= state.AdCost)
            {
                state.Funds -= state.AdCost;
                state.MarketingLevel++;
                state.AdCost = Math.Floor(state.AdCost * 2);
                needsRedraw = true;
            }
            else
            {
                Console.WriteLine(Localization.Get("Msg_NoFunds"));
            }
        }

        public void RaisePrice()
        {
            state.Margin = Math.Round(state.Margin + 0.01, 2);
            needsRedraw = true;
        }

        public void LowerPrice()
        {
            if (state.Margin >= 0.01)
            {
                state.Margin = Math.Round(state.Margin - 0.01, 2);
                needsRedraw = true;
            }
        }

        public void AddProcessor()
        {
            if (state.Trust > 0)
            {
                state.Processors++;
                state.Trust--;
                needsRedraw = true;
            }
            else
            {
                Console.WriteLine(Localization.Get("Msg_NoTrust"));
            }
        }

        public void AddMemory()
        {
            if (state.Trust > 0)
            {
                state.Memory++;
                state.Trust--;
                needsRedraw = true;
            }
            else
            {
                Console.WriteLine(Localization.Get("Msg_NoTrust"));
            }
        }

        private void UpdateTrust()
        {
            if (state.TotalClipsProduced >= state.NextTrust)
            {
                state.Trust++;
                state.NextTrust = (long)(state.NextTrust * 1.618);
                Console.WriteLine(Localization.Get("Msg_TrustIncreased", state.Trust));
            }
        }

        public void Update()
        {
            var now = DateTime.Now;
            var elapsed = (now - lastUpdate).TotalSeconds;
            
            if (elapsed >= 0.1)
            {
                ProduceClips(elapsed);
                SellClips();
                UpdateOperations(elapsed);
                UpdateDemand();
                
                if ((now - lastAutoSave).TotalSeconds >= 30)
                {
                    SaveGame();
                    lastAutoSave = now;
                }
                
                lastUpdate = now;
            }
        }

        private void ProduceClips(double deltaTime)
        {
            if (state.Wire > 0 && state.ClipRate > 0)
            {
                state.PartialClips += state.ClipRate * deltaTime * 10;
                
                if (state.PartialClips >= 1)
                {
                    int actualClips = (int)Math.Min(state.PartialClips, state.Wire);
                    
                    state.Wire -= actualClips;
                    state.Clips += actualClips;
                    state.TotalClipsProduced += actualClips;
                    state.UnsoldClips += actualClips;
                    state.PartialClips -= actualClips;
                    UpdateTrust();
                }
            }
        }

        private void SellClips()
        {
            if (state.UnsoldClips > 0)
            {
                double chanceOfPurchase = state.Demand / 100.0;
                if (random.NextDouble() < chanceOfPurchase)
                {
                    int clipsSold = Math.Min(
                        (int)(0.7 * Math.Pow(state.Demand, 1.15)),
                        (int)state.UnsoldClips
                    );
                    
                    state.UnsoldClips -= clipsSold;
                    state.Funds += clipsSold * state.Margin;
                }
            }
        }

        private void UpdateOperations(double deltaTime)
        {
            if (state.Operations < state.MaxOps)
            {
                double opCycle = state.Processors / 10.0 * deltaTime * 10;
                int opBuf = state.MaxOps - (int)state.Operations;
                
                if (opCycle > opBuf)
                    opCycle = opBuf;
                
                state.Operations += (long)opCycle;
            }
        }

        private void UpdateDemand()
        {
            double marketing = Math.Pow(1.1, state.MarketingLevel - 1);
            state.Demand = (int)((0.8 / state.Margin) * marketing * 10);
        }

        public void DisplayStatus()
        {
            // Only redraw every 200ms to reduce flicker
            var now = DateTime.Now;
            if ((now - lastDisplay).TotalMilliseconds < 200 && !needsRedraw)
                return;
            
            lastDisplay = now;
            needsRedraw = false;

            Console.SetCursorPosition(0, 0);
            
            // Header
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    UNIVERSAL PAPERCLIPS - Console Edition                    ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════════════════════════╣");
            
            // Main stats - Side by side (fixed width with invariant culture)
            string line1 = string.Format(invCulture, "║ Paperclips: {0,12}  │  Wire: {1,10} inches        │ Clips/s: {2,6:F2} ║", 
                state.Clips, state.Wire, state.ClipRate);
            Console.WriteLine(line1);
            
            string line2 = string.Format(invCulture, "║ Fonds: ${0,16:F2}  │  Prix: ${1,6:F2}              │ Demande: {2,4}%   ║",
                state.Funds, state.Margin, state.Demand);
            Console.WriteLine(line2);
            
            string line3 = string.Format(invCulture, "║ Inventaire: {0,9}  │  Wire à vendre: ${1,6:F2}           │ x{2,5}  ║",
                state.UnsoldClips, state.WireCost, state.WireAmount);
            Console.WriteLine(line3);
            
            Console.WriteLine("╠═══════════════════════════════╦══════════════════════════╦═══════════════════╣");
            
            // Production column | Business column | Computing column
            Console.WriteLine($"║ PRODUCTION                    ║ BUSINESS                 ║ COMPUTING         ║");
            Console.WriteLine("╟───────────────────────────────╫──────────────────────────╫───────────────────╢");
            
            // Line 1
            string autoClip = string.Format(invCulture, "AutoClip: {0,3} (${1,7:F2})", 
                state.ClipmakerLevel, state.ClipperCost);
            string marketing = string.Format(invCulture, "Marketing: Niv.{0,2}", 
                state.MarketingLevel);
            string trust = string.Format(invCulture, "Trust: {0,2}→{1,7}", 
                state.Trust, FormatInt(state.NextTrust));
            Console.WriteLine($"║ {autoClip,-29} ║ {marketing,-24} ║ {trust,-17} ║");
            
            // Line 2
            string megaClip = "";
            if (state.MegaClipperLevel > 0 || state.Funds >= 500)
            {
                megaClip = string.Format(invCulture, "MegaClip: {0,3} (${1,7:F0})", 
                    state.MegaClipperLevel, state.MegaClipperCost);
            }
            string adCost = string.Format(invCulture, "Coût: ${0,10:F2}", state.AdCost);
            string proc = string.Format(invCulture, "Proc: {0,2} Mem: {1,2}", 
                state.Processors, state.Memory);
            Console.WriteLine($"║ {megaClip,-29} ║ {adCost,-24} ║ {proc,-17} ║");
            
            // Line 3
            string ops = string.Format(invCulture, "Ops: {0,7}/{1,7}", 
                FormatInt(state.Operations), FormatInt(state.MaxOps));
            Console.WriteLine($"║                               ║                          ║ {ops,-17} ║");
            
            if (state.Creativity > 0 || state.Processors >= 5)
            {
                string creat = string.Format(invCulture, "Créativité: {0,5}", state.Creativity);
                Console.WriteLine($"║                               ║                          ║ {creat,-17} ║");
            }
            
            Console.WriteLine("╠═══════════════════════════════╩══════════════════════════╩═══════════════════╣");
            Console.WriteLine("║ [P]Make  [W]Wire  [A]Auto  [G]Mega  [M]Marketing  [+/-]Prix  [T]Proc  [Y]Mem ║");
            Console.WriteLine("║ [S]Save  [Q]Quit  [Ctrl+M]Menu                                               ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════╝");
            
            // Add blank lines to clear any leftover text
            for (int i = 0; i < 5; i++)
                Console.WriteLine("                                                                                ");
        }

        public void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║                      {Localization.Get("Cmd_Menu").PadRight(30)}        ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine($"  {Localization.Get("UI_ProductionBusiness")}:");
            Console.WriteLine($"    [P]  {Localization.Get("Cmd_Clips")} (1 wire)");
            Console.WriteLine($"    [A]  {Localization.Get("Cmd_Auto")} (${state.ClipperCost:F2})");
            if (state.Funds >= 500 || state.MegaClipperLevel > 0)
                Console.WriteLine($"    [G]  {Localization.Get("Cmd_Mega")} (${state.MegaClipperCost:F2})");
            Console.WriteLine();
            Console.WriteLine($"  {Localization.Get("UI_Resources")}:");
            Console.WriteLine($"    [W]  {Localization.Get("Cmd_Wire")} ({state.WireAmount} inches, ${state.WireCost:F2})");
            Console.WriteLine();
            Console.WriteLine($"  VENTES:");
            Console.WriteLine($"    [+]  {Localization.Get("Cmd_Price")} (+)");
            Console.WriteLine($"    [-]  {Localization.Get("Cmd_Price")} (-)");
            Console.WriteLine($"    [M]  {Localization.Get("Cmd_Marketing")} - {Localization.Get("Lbl_Level", state.MarketingLevel)} (${state.AdCost:F2})");
            Console.WriteLine();
            Console.WriteLine($"  {Localization.Get("UI_Computing")}:");
            Console.WriteLine($"    [T]  {Localization.Get("Cmd_Proc")} ({Localization.Get("Lbl_Trust")} {state.Trust})");
            Console.WriteLine($"    [Y]  {Localization.Get("Cmd_Mem")} ({Localization.Get("Lbl_Trust")} {state.Trust})");
            Console.WriteLine();
            Console.WriteLine($"  SYSTÈME:");
            Console.WriteLine($"    [S]  {Localization.Get("Cmd_Save")}");
            Console.WriteLine($"    [Q]  {Localization.Get("Cmd_Quit")}");
            Console.WriteLine();
            Console.WriteLine("  Appuyez sur une touche pour continuer...");
            Console.ReadKey(true);
        }

        public void ProcessInput(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.P:
                    MakePaperclip();
                    break;
                case ConsoleKey.W:
                    BuyWire();
                    break;
                case ConsoleKey.A:
                    BuyAutoClipper();
                    break;
                case ConsoleKey.G:
                    if (state.Funds >= 500 || state.MegaClipperLevel > 0)
                        BuyMegaClipper();
                    break;
                case ConsoleKey.M:
                    BuyMarketing();
                    break;
                case ConsoleKey.OemPlus:
                case ConsoleKey.Add:
                    RaisePrice();
                    break;
                case ConsoleKey.OemMinus:
                case ConsoleKey.Subtract:
                    LowerPrice();
                    break;
                case ConsoleKey.T:
                    AddProcessor();
                    break;
                case ConsoleKey.Y:
                    AddMemory();
                    break;
                case ConsoleKey.S:
                    SaveGame();
                    break;
            }
        }

        public bool IsRunning { get; set; } = true;

        public void CheckQuit(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.Q)
            {
                Console.Clear();
                Console.WriteLine(Localization.Get("Msg_SaveQuit"));
                var response = Console.ReadKey(true);
                if (response.Key == ConsoleKey.O || response.Key == ConsoleKey.Y)
                {
                    SaveGame();
                }
                IsRunning = false;
            }
        }

        public void CheckMenu(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.Spacebar)
            {
                ShowMenu();
            }
        }
    }
}
