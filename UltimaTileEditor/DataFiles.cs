using System;
using System.Collections.Generic;
using System.Text;

namespace UltimaTileEditor
{
    internal class DataFiles
    {
        public static readonly List<string> Ultima5CharFiles = [ "IBM", "RUNES" ];

        public static readonly List<string> Ultima5Pict = [ "CREATE", "END1", "END2", "ENDSC", "STARTSC",
            "STORY1", "STORY2", "STORY3", "STORY4", "STORY5", "STORY6", "TEXT", "ULTIMA" ];
        public static readonly List<string> Ultima5PictImage = [
            "CREATE_0", "CREATE_1", "CREATE_2", "CREATE_3", "CREATE_4", "CREATE_5", "CREATE_6", "CREATE_7", "CREATE_8", "CREATE_9", "CREATE_10",
            "END1_0", "END1_1", "END1_2",
            "END2_0", "END2_1", "END2_2",
            "ENDSC_0",
            "STARTSC_0", "STARTSC_1", "STARTSC_2",
            "STORY1_0", "STORY1_1", "STORY1_2",
            "STORY2_0", "STORY2_1", "STORY2_2", "STORY2_3",
            "STORY3_0", "STORY3_1",
            "STORY4_0", "STORY4_1",
            "STORY5_0", "STORY5_1",
            "STORY6_0", "STORY6_1", "STORY6_2", "STORY6_3", "STORY6_4", "STORY6_5", "STORY6_6", "STORY6_7",
            "TEXT_0", "TEXT_1", "TEXT_2", "TEXT_3", "TEXT_4", "TEXT_5",
            "ULTIMA_0", "ULTIMA_1", "ULTIMA_2", "ULTIMA_3", "ULTIMA_4" ];

        public static readonly List<string> Ultima5Tiles = [ "TILES" ];

        public static readonly List<string> Ultima5Masked = ["ITEMS", "MON0", "MON1", "MON2", "MON3", "MON4", "MON5", "MON6", "MON7"];
        public static readonly List<string> Ultima5MaskedImage = ["ITEMS_0", "ITEMS_1", "ITEMS_2", "ITEMS_3",
            "ITEMS_4", "ITEMS_5", "ITEMS_6", "ITEMS_7", "ITEMS_8", "ITEMS_9", "ITEMS_10", "ITEMS_11", "ITEMS_12",
            "ITEMS_13", "ITEMS_14", "ITEMS_15", "ITEMS_16", "ITEMS_17", "ITEMS_18", "ITEMS_19",
            "MON0_0", "MON0_1", "MON0_2", "MON0_3", "MON0_4", "MON0_5",
            "MON1_0", "MON1_1", "MON1_2", "MON1_3", "MON1_4", "MON1_5",
            "MON2_0", "MON2_1", "MON2_2", "MON2_3", "MON2_4", "MON2_5",
            "MON3_0", "MON3_1", "MON3_2", "MON3_3", "MON3_4", "MON3_5",
            "MON4_0", "MON4_1", "MON4_2", "MON4_3", "MON4_4", "MON4_5",
            "MON5_0", "MON5_1", "MON5_2", "MON5_3", "MON5_4", "MON5_5",
            "MON6_0", "MON6_1", "MON6_2", "MON6_3", "MON6_4", "MON6_5",
            "MON7_0", "MON7_1", "MON7_2", "MON7_3", "MON7_4", "MON7_5" ];

        public static readonly List<string> Ultima5Dng = ["DNG1", "DNG2", "DNG3" ];
        public static readonly List<string> Ultima5DngImage = ["DNG1_0", "DNG1_1", "DNG1_2", "DNG1_3", "DNG1_4", "DNG1_5", "DNG1_6", "DNG1_7",
        "DNG1_9","DNG1_10","DNG1_11","DNG1_12","DNG1_13","DNG1_14","DNG1_15","DNG1_16","DNG1_17",
        "DNG1_18","DNG1_19","DNG1_20","DNG1_21","DNG1_22","DNG1_23","DNG1_25","DNG1_26","DNG1_27",
        "DNG2_0", "DNG2_1", "DNG2_2", "DNG2_3", "DNG2_4", "DNG2_5", "DNG2_6", "DNG2_7",
        "DNG2_9","DNG2_10","DNG2_11","DNG2_12","DNG2_13","DNG2_14","DNG2_15","DNG2_16","DNG2_17",
        "DNG2_18","DNG2_19","DNG2_20","DNG2_21","DNG2_22","DNG2_23","DNG2_25","DNG2_26","DNG2_27",
        "DNG3_0", "DNG3_1", "DNG3_2", "DNG3_3", "DNG3_4", "DNG3_5", "DNG3_6", "DNG3_7",
        "DNG3_9","DNG3_10","DNG3_11","DNG3_12","DNG3_13","DNG3_14","DNG3_15","DNG3_16","DNG3_17",
        "DNG3_18","DNG3_19","DNG3_20","DNG3_21","DNG3_22","DNG3_23","DNG3_25","DNG3_26","DNG3_27"];

        public static readonly List<string> Ultima4EGATileFiles = [ "SHAPES" ];

        public static readonly List<string> Ultima4Charset = ["CHARSET"];

        public static readonly List<string> Ultima4RLE = ["START", "KEY7", "RUNE_0", "RUNE_1", "RUNE_2", "RUNE_3", "RUNE_4", "RUNE_5",
            "STONCRCL", "HONESTY", "COMPASSN", "VALOR", "JUSTICE", "SACRIFIC", "HONOR",
            "SPIRIT", "HUMILITY", "TRUTH", "LOVE", "COURAGE"];

        public static readonly List<string> Ultima4LZW = ["ABACUS", "ANIMATE", "GYPSY", "INSIDE", "OUTSIDE",
            "PORTAL", "TREE", "WAGON", "HONCOM", "SACHONOR", "SPIRHUM", "VALJUS", "TITLE"];

        public static readonly List<string> Ultima3Files = ["SHAPES"];

        public static readonly List<string> Ultima3Charset = ["CHARSET"];

        public static readonly List<string> Ultima3Pictures = ["BLANK", "EXOD"];

        public static readonly List<string> Ultima3Animate = ["ANIMATE"];

        public static readonly List<string> Ultima2Files = ["ULTIMAII"];

        public static readonly List<string> Ultima2Pictures = ["PICDNG", "PICDRA", "PICMIN", "PICOUT", "PICSPA", "PICTWN"];

        public static readonly List<string> Ultima1EGAFiles = ["EGAMOND", "EGATILES", "EGATOWN"];

        public static readonly List<string> Ultima1CGAFiles = ["CGAMOND", "CGATILES", "CGATOWN"];

        public static readonly List<string> Ultima1T1KFiles = ["T1KMOND", "T1KTILES", "T1KTOWN"];

        public static readonly List<string> Ultima1Image = ["CASTLE"];

        public static readonly List<string> Ultima1Ending = ["NIF"];
    }
}
