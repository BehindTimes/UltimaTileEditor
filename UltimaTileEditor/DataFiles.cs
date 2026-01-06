using System;
using System.Collections.Generic;
using System.Text;

namespace UltimaTileEditor
{
    internal class DataFiles
    {
        public static readonly List<string> Ultima5Images = [ "CREATE", "END1", "END2", "ENDSC", "STARTSC",
            "STORY1", "STORY2", "STORY3", "STORY4", "STORY5", "STORY6", "TEXT", "ULTIMA" ];

        public static readonly List<string> Ultima5Tiles = [ "TILES" ];

        public static readonly List<string> Ultima5Masked = [ "ITEMS", "MON0", "MON1", "MON2", "MON3", "MON4", "MON5", "MON6", "MON7"];

        public static readonly List<string> Ultima5DngImage = ["DNG1", "DNG2", "DNG3" ];

        public static readonly List<string> Ultima4EGATileFiles = [ "SHAPES" ];

        public static readonly List<string> Ultima4Charset = ["CHARSET"];

        public static readonly List<string> Ultima4RLE = ["START", "KEY7", "RUNE_0", "RUNE_1", "RUNE_2", "RUNE_3", "RUNE_4", "RUNE_5",
            "STONCRCL", "HONESTY", "COMPASSN", "VALOR", "JUSTICE", "SACRIFIC", "HONOR",
            "SPIRIT", "HUMILITY", "TRUTH", "LOVE", "COURAGE"];

        public static readonly List<string> Ultima4LZW = ["ABACUS", "ANIMATE", "GYPSY", "INSIDE", "OUTSIDE",
            "PORTAL", "TREE", "WAGON", "HONCOM", "SACHONOR", "SPIRHUM", "VALJUS", "TITLE"];

        public static readonly List<string> Ultima3Files = ["SHAPES"];

        public static readonly List<string> Ultima2Files = ["ULTIMAII"];

        public static readonly List<string> Ultima1EGAFiles = ["EGAMOND", "EGATILES", "EGATOWN"];

        public static readonly List<string> Ultima1CGAFiles = ["CGAMOND", "CGATILES", "CGATOWN"];

        public static readonly List<string> Ultima1T1KFiles = ["T1KMOND", "T1KTILES", "T1KTOWN"];
    }
}
