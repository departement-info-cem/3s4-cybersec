using System;
using System.Text.Json.Serialization;

namespace PaperclipsConsole
{
    public class GameState
    {
        public long Clips { get; set; } = 0;
        public long UnusedClips { get; set; } = 0;
        public double Funds { get; set; } = 0;
        public long Wire { get; set; } = 1000;
        public double Margin { get; set; } = 0.25;
        public int Demand { get; set; } = 10;
        public long UnsoldClips { get; set; } = 0;
        public int ClipmakerLevel { get; set; } = 0;
        public double ClipperCost { get; set; } = 5.00;
        public int MarketingLevel { get; set; } = 1;
        public double AdCost { get; set; } = 100.00;
        public double WireCost { get; set; } = 20;
        public int WireAmount { get; set; } = 1000;
        public int Processors { get; set; } = 1;
        public int Memory { get; set; } = 1;
        public long Operations { get; set; } = 0;
        public int Trust { get; set; } = 2;
        public long NextTrust { get; set; } = 1000;
        public int Creativity { get; set; } = 0;
        public long TotalClipsProduced { get; set; } = 0;
        public double PartialClips { get; set; } = 0;
        public int MegaClipperLevel { get; set; } = 0;
        public double MegaClipperCost { get; set; } = 500;
        public Language Language { get; set; } = Language.French;
        public bool EncryptSave { get; set; } = false;
        
        [JsonIgnore]
        public int MaxOps => Memory * 1000;
        
        [JsonIgnore]
        public double ClipRate => ClipmakerLevel / 100.0 + MegaClipperLevel * 5;
    }
}
