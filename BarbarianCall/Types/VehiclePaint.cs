﻿using System;
using System.Linq;
using System.Drawing;

namespace BarbarianCall.Types
{
    public enum VehiclePaint
    {
        Unknown = -1,
        Metallic_Black,
        Metallic_Graphite_Black,
        Metallic_Black_Steal,
        Metallic_Dark_Silver,
        Metallic_Silver,
        Metallic_Blue_Silver,
        Metallic_Steel_Gray,
        Metallic_Shadow_Silver,
        Metallic_Stone_Silver,
        Metallic_Midnight_Silver,
        Metallic_Gun_Metal,
        Metallic_Anthracite_Grey,
        Matte_Black,
        Matte_Gray,
        Matte_Light_Grey,
        Util_Black,
        Util_Black_Poly,
        Util_Dark_Silver,
        Util_Silver,
        Util_Gun_Metal,
        Util_Shadow_Silver,
        Worn_Black,
        Worn_Graphite,
        Worn_Silver_Grey,
        Worn_Silver,
        Worn_Blue_Silver,
        Worn_Shadow_Silver,
        Metallic_Red,
        Metallic_Torino_Red,
        Metallic_Formula_Red,
        Metallic_Blaze_Red,
        Metallic_Graceful_Red,
        Metallic_Garnet_Red,
        Metallic_Desert_Red,
        Metallic_Cabernet_Red,
        Metallic_Candy_Red,
        Metallic_Sunrise_Orange,
        Metallic_Classic_Gold,
        Metallic_Orange,
        Matte_Red,
        Matte_Dark_Red,
        Matte_Orange,
        Matte_Yellow,
        Util_Red,
        Util_Bright_Red,
        Util_Garnet_Red,
        Worn_Red,
        Worn_Golden_Red,
        Worn_Dark_Red,
        Metallic_Dark_Green,
        Metallic_Racing_Green,
        Metallic_Sea_Green,
        Metallic_Olive_Green,
        Metallic_Green,
        Metallic_Gasoline_Blue_Green,
        Matte_Lime_Green,
        Util_Dark_Green,
        Util_Green,
        Worn_Dark_Green,
        Worn_Green,
        Worn_Sea_Wash,
        Metallic_Midnight_Blue,
        Metallic_Dark_Blue,
        Metallic_Saxony_Blue,
        Metallic_Blue,
        Metallic_Mariner_Blue,
        Metallic_Harbor_Blue,
        Metallic_Diamond_Blue,
        Metallic_Surf_Blue,
        Metallic_Nautical_Blue,
        Metallic_Bright_Blue,
        Metallic_Purple_Blue,
        Metallic_Spinnaker_Blue,
        Metallic_Ultra_Blue,
        Metallic_Bright_Blue2,
        Util_Dark_Blue,
        Util_Midnight_Blue,
        Util_Blue,
        Util_Sea_Foam_Blue,
        Uil_Lightning_Blue,
        Util_Maui_Blue_Poly,
        Util_Bright_Blue,
        Matte_Dark_Blue,
        Matte_Blue,
        Matte_Midnight_Blue,
        Worn_Dark_Blue,
        Worn_Blue,
        Worn_Light_Blue,
        Metallic_Taxi_Yellow,
        Metallic_Race_Yellow,
        Metallic_Bronze,
        Metallic_Yellow_Bird,
        Metallic_Lime,
        Metallic_Champagne,
        Metallic_Pueblo_Beige,
        Metallic_Dark_Ivory,
        Metallic_Choco_Brown,
        Metallic_Golden_Brown,
        Metallic_Light_Brown,
        Metallic_Straw_Beige,
        Metallic_Moss_Brown,
        Metallic_Biston_Brown,
        Metallic_Beechwood,
        Metallic_Dark_Beechwood,
        Metallic_Choco_Orange,
        Metallic_Beach_Sand,
        Metallic_Sun_Bleeched_Sand,
        Metallic_Cream,
        Util_Brown,
        Util_Medium_Brown,
        Util_Light_Brown,
        Metallic_White,
        Metallic_Frost_White,
        Worn_Honey_Beige,
        Worn_Brown,
        Worn_Dark_Brown,
        Worn_Straw_Beige,
        Brushed_Steel,
        Brushed_Black_Steel,
        Brushed_Aluminium,
        Chrome,
        Worn_Off_White,
        Util_Off_White,
        Worn_Orange,
        Worn_Light_Orange,
        Metallic_Securicor_Green,
        Worn_Taxi_Yellow,
        Police_Car_Blue,
        Matte_Green,
        Matte_Brown,
        Worn_Orange2,
        Matte_White,
        Worn_White,
        Worn_Olive_Army_Green,
        Pure_White,
        Hot_Pink,
        Salmon_Pink,
        Metallic_Vermillion_Pink,
        Orange,
        Green,
        Blue,
        Mettalic_Black_Blue,
        Metallic_Black_Purple,
        Metallic_Black_Red,
        Hunter_Green,
        Metallic_Purple,
        Metaillic_V_Dark_Blue,
        MODSHOP_BLACK1,
        Matte_Purple,
        Matte_Dark_Purple,
        Metallic_Lava_Red,
        Matte_Forest_Green,
        Matte_Olive_Drab,
        Matte_Desert_Brown,
        Matte_Desert_Tan,
        Matte_Foilage_Green,
        DEFAULT_ALLOY_COLOR,
        Epsilon_Blue,
        MP100_GOLD,
        MP100_GOLD_SATIN,
        MP100_GOLD_SPEC,
    }
    public static class VehiclePaintExtensions
    {
        public static Color GetColor(this VehiclePaint vehiclePaint) => vehiclePaint switch
        {
            VehiclePaint.Unknown => Color.Empty,
            VehiclePaint.Metallic_Black => Color.FromArgb(255, 8, 8, 8),
            VehiclePaint.Metallic_Graphite_Black => Color.FromArgb(255, 15, 15, 15),
            VehiclePaint.Metallic_Black_Steal => Color.FromArgb(255, 28, 30, 33),
            VehiclePaint.Metallic_Dark_Silver => Color.FromArgb(255, 41, 44, 46),
            VehiclePaint.Metallic_Silver => Color.FromArgb(255, 90, 94, 102),
            VehiclePaint.Metallic_Blue_Silver => Color.FromArgb(255, 119, 124, 135),
            VehiclePaint.Metallic_Steel_Gray => Color.FromArgb(255, 81, 84, 89),
            VehiclePaint.Metallic_Shadow_Silver => Color.FromArgb(255, 50, 59, 71),
            VehiclePaint.Metallic_Stone_Silver => Color.FromArgb(255, 51, 51, 51),
            VehiclePaint.Metallic_Midnight_Silver => Color.FromArgb(255, 31, 34, 38),
            VehiclePaint.Metallic_Gun_Metal => Color.FromArgb(255, 35, 41, 46),
            VehiclePaint.Metallic_Anthracite_Grey => Color.FromArgb(255, 18, 17, 16),
            VehiclePaint.Matte_Black => Color.FromArgb(255, 5, 5, 5),
            VehiclePaint.Matte_Gray => Color.FromArgb(255, 18, 18, 18),
            VehiclePaint.Matte_Light_Grey => Color.FromArgb(255, 47, 50, 51),
            VehiclePaint.Util_Black => Color.FromArgb(255, 8, 8, 8),
            VehiclePaint.Util_Black_Poly => Color.FromArgb(255, 18, 18, 18),
            VehiclePaint.Util_Dark_Silver => Color.FromArgb(255, 32, 34, 36),
            VehiclePaint.Util_Silver => Color.FromArgb(255, 87, 89, 97),
            VehiclePaint.Util_Gun_Metal => Color.FromArgb(255, 35, 41, 46),
            VehiclePaint.Util_Shadow_Silver => Color.FromArgb(255, 50, 59, 71),
            VehiclePaint.Worn_Black => Color.FromArgb(255, 15, 16, 18),
            VehiclePaint.Worn_Graphite => Color.FromArgb(255, 33, 33, 33),
            VehiclePaint.Worn_Silver_Grey => Color.FromArgb(255, 91, 93, 94),
            VehiclePaint.Worn_Silver => Color.FromArgb(255, 136, 138, 153),
            VehiclePaint.Worn_Blue_Silver => Color.FromArgb(255, 105, 113, 135),
            VehiclePaint.Worn_Shadow_Silver => Color.FromArgb(255, 59, 70, 84),
            VehiclePaint.Metallic_Red => Color.FromArgb(255, 105, 0, 0),
            VehiclePaint.Metallic_Torino_Red => Color.FromArgb(255, 138, 11, 0),
            VehiclePaint.Metallic_Formula_Red => Color.FromArgb(255, 107, 0, 0),
            VehiclePaint.Metallic_Blaze_Red => Color.FromArgb(255, 97, 16, 9),
            VehiclePaint.Metallic_Graceful_Red => Color.FromArgb(255, 74, 10, 10),
            VehiclePaint.Metallic_Garnet_Red => Color.FromArgb(255, 71, 14, 14),
            VehiclePaint.Metallic_Desert_Red => Color.FromArgb(255, 56, 12, 0),
            VehiclePaint.Metallic_Cabernet_Red => Color.FromArgb(255, 38, 3, 11),
            VehiclePaint.Metallic_Candy_Red => Color.FromArgb(255, 99, 0, 18),
            VehiclePaint.Metallic_Sunrise_Orange => Color.FromArgb(255, 128, 40, 0),
            VehiclePaint.Metallic_Classic_Gold => Color.FromArgb(255, 110, 79, 45),
            VehiclePaint.Metallic_Orange => Color.FromArgb(255, 189, 72, 0),
            VehiclePaint.Matte_Red => Color.FromArgb(255, 120, 0, 0),
            VehiclePaint.Matte_Dark_Red => Color.FromArgb(255, 54, 0, 0),
            VehiclePaint.Matte_Orange => Color.FromArgb(255, 171, 63, 0),
            VehiclePaint.Matte_Yellow => Color.FromArgb(255, 222, 126, 0),
            VehiclePaint.Util_Red => Color.FromArgb(255, 82, 0, 0),
            VehiclePaint.Util_Bright_Red => Color.FromArgb(255, 140, 4, 4),
            VehiclePaint.Util_Garnet_Red => Color.FromArgb(255, 74, 16, 0),
            VehiclePaint.Worn_Red => Color.FromArgb(255, 89, 37, 37),
            VehiclePaint.Worn_Golden_Red => Color.FromArgb(255, 117, 66, 49),
            VehiclePaint.Worn_Dark_Red => Color.FromArgb(255, 33, 8, 4),
            VehiclePaint.Metallic_Dark_Green => Color.FromArgb(255, 0, 18, 7),
            VehiclePaint.Metallic_Racing_Green => Color.FromArgb(255, 0, 26, 11),
            VehiclePaint.Metallic_Sea_Green => Color.FromArgb(255, 0, 33, 30),
            VehiclePaint.Metallic_Olive_Green => Color.FromArgb(255, 31, 38, 30),
            VehiclePaint.Metallic_Green => Color.FromArgb(255, 0, 56, 5),
            VehiclePaint.Metallic_Gasoline_Blue_Green => Color.FromArgb(255, 11, 65, 69),
            VehiclePaint.Matte_Lime_Green => Color.FromArgb(255, 65, 133, 3),
            VehiclePaint.Util_Dark_Green => Color.FromArgb(255, 15, 31, 21),
            VehiclePaint.Util_Green => Color.FromArgb(255, 2, 54, 19),
            VehiclePaint.Worn_Dark_Green => Color.FromArgb(255, 22, 36, 25),
            VehiclePaint.Worn_Green => Color.FromArgb(255, 42, 54, 37),
            VehiclePaint.Worn_Sea_Wash => Color.FromArgb(255, 69, 92, 86),
            VehiclePaint.Metallic_Midnight_Blue => Color.FromArgb(255, 0, 13, 20),
            VehiclePaint.Metallic_Dark_Blue => Color.FromArgb(255, 0, 16, 41),
            VehiclePaint.Metallic_Saxony_Blue => Color.FromArgb(255, 28, 47, 79),
            VehiclePaint.Metallic_Blue => Color.FromArgb(255, 0, 27, 87),
            VehiclePaint.Metallic_Mariner_Blue => Color.FromArgb(255, 59, 78, 120),
            VehiclePaint.Metallic_Harbor_Blue => Color.FromArgb(255, 39, 45, 59),
            VehiclePaint.Metallic_Diamond_Blue => Color.FromArgb(255, 149, 178, 219),
            VehiclePaint.Metallic_Surf_Blue => Color.FromArgb(255, 62, 98, 122),
            VehiclePaint.Metallic_Nautical_Blue => Color.FromArgb(255, 28, 49, 64),
            VehiclePaint.Metallic_Bright_Blue => Color.FromArgb(255, 0, 85, 196),
            VehiclePaint.Metallic_Purple_Blue => Color.FromArgb(255, 26, 24, 46),
            VehiclePaint.Metallic_Spinnaker_Blue => Color.FromArgb(255, 22, 22, 41),
            VehiclePaint.Metallic_Ultra_Blue => Color.FromArgb(255, 14, 49, 109),
            VehiclePaint.Metallic_Bright_Blue2 => Color.FromArgb(255, 57, 90, 131),
            VehiclePaint.Util_Dark_Blue => Color.FromArgb(255, 9, 20, 46),
            VehiclePaint.Util_Midnight_Blue => Color.FromArgb(255, 15, 16, 33),
            VehiclePaint.Util_Blue => Color.FromArgb(255, 21, 42, 82),
            VehiclePaint.Util_Sea_Foam_Blue => Color.FromArgb(255, 50, 70, 84),
            VehiclePaint.Uil_Lightning_Blue => Color.FromArgb(255, 21, 37, 99),
            VehiclePaint.Util_Maui_Blue_Poly => Color.FromArgb(255, 34, 59, 161),
            VehiclePaint.Util_Bright_Blue => Color.FromArgb(255, 31, 31, 161),
            VehiclePaint.Matte_Dark_Blue => Color.FromArgb(255, 3, 14, 46),
            VehiclePaint.Matte_Blue => Color.FromArgb(255, 15, 30, 115),
            VehiclePaint.Matte_Midnight_Blue => Color.FromArgb(255, 0, 28, 50),
            VehiclePaint.Worn_Dark_Blue => Color.FromArgb(255, 42, 55, 84),
            VehiclePaint.Worn_Blue => Color.FromArgb(160, 48, 60, 94),
            VehiclePaint.Worn_Light_Blue => Color.FromArgb(255, 59, 103, 150),
            VehiclePaint.Metallic_Taxi_Yellow => Color.FromArgb(255, 245, 137, 15),
            VehiclePaint.Metallic_Race_Yellow => Color.FromArgb(255, 217, 166, 0),
            VehiclePaint.Metallic_Bronze => Color.FromArgb(255, 74, 52, 27),
            VehiclePaint.Metallic_Yellow_Bird => Color.FromArgb(255, 162, 168, 39),
            VehiclePaint.Metallic_Lime => Color.FromArgb(255, 86, 143, 0),
            VehiclePaint.Metallic_Champagne => Color.FromArgb(255, 87, 81, 75),
            VehiclePaint.Metallic_Pueblo_Beige => Color.FromArgb(255, 41, 27, 6),
            VehiclePaint.Metallic_Dark_Ivory => Color.FromArgb(255, 38, 33, 23),
            VehiclePaint.Metallic_Choco_Brown => Color.FromArgb(255, 18, 13, 7),
            VehiclePaint.Metallic_Golden_Brown => Color.FromArgb(255, 51, 33, 17),
            VehiclePaint.Metallic_Light_Brown => Color.FromArgb(255, 61, 48, 35),
            VehiclePaint.Metallic_Straw_Beige => Color.FromArgb(255, 94, 83, 67),
            VehiclePaint.Metallic_Moss_Brown => Color.FromArgb(255, 55, 56, 43),
            VehiclePaint.Metallic_Biston_Brown => Color.FromArgb(255, 34, 25, 24),
            VehiclePaint.Metallic_Beechwood => Color.FromArgb(255, 87, 80, 54),
            VehiclePaint.Metallic_Dark_Beechwood => Color.FromArgb(255, 36, 19, 9),
            VehiclePaint.Metallic_Choco_Orange => Color.FromArgb(255, 59, 23, 0),
            VehiclePaint.Metallic_Beach_Sand => Color.FromArgb(255, 110, 98, 70),
            VehiclePaint.Metallic_Sun_Bleeched_Sand => Color.FromArgb(251, 153, 141, 115),
            VehiclePaint.Metallic_Cream => Color.FromArgb(255, 207, 192, 165),
            VehiclePaint.Util_Brown => Color.FromArgb(255, 31, 23, 9),
            VehiclePaint.Util_Medium_Brown => Color.FromArgb(251, 61, 49, 29),
            VehiclePaint.Util_Light_Brown => Color.FromArgb(255, 102, 88, 71),
            VehiclePaint.Metallic_White => Color.FromArgb(255, 240, 240, 240),
            VehiclePaint.Metallic_Frost_White => Color.FromArgb(255, 179, 185, 201),
            VehiclePaint.Worn_Honey_Beige => Color.FromArgb(255, 97, 95, 85),
            VehiclePaint.Worn_Brown => Color.FromArgb(255, 36, 30, 26),
            VehiclePaint.Worn_Dark_Brown => Color.FromArgb(255, 23, 20, 19),
            VehiclePaint.Worn_Straw_Beige => Color.FromArgb(255, 59, 55, 47),
            VehiclePaint.Brushed_Steel => Color.FromArgb(255, 59, 64, 69),
            VehiclePaint.Brushed_Black_Steel => Color.FromArgb(255, 26, 30, 33),
            VehiclePaint.Brushed_Aluminium => Color.FromArgb(255, 94, 100, 107),
            VehiclePaint.Chrome => Color.FromArgb(255, 0, 0, 0),
            VehiclePaint.Worn_Off_White => Color.FromArgb(255, 176, 176, 176),
            VehiclePaint.Util_Off_White => Color.FromArgb(255, 153, 153, 153),
            VehiclePaint.Worn_Orange => Color.FromArgb(255, 181, 101, 25),
            VehiclePaint.Worn_Light_Orange => Color.FromArgb(255, 196, 92, 51),
            VehiclePaint.Metallic_Securicor_Green => Color.FromArgb(255, 71, 120, 60),
            VehiclePaint.Worn_Taxi_Yellow => Color.FromArgb(255, 186, 132, 37),
            VehiclePaint.Police_Car_Blue => Color.FromArgb(255, 42, 119, 161),
            VehiclePaint.Matte_Green => Color.FromArgb(255, 36, 48, 34),
            VehiclePaint.Matte_Brown => Color.FromArgb(255, 107, 95, 84),
            VehiclePaint.Worn_Orange2 => Color.FromArgb(255, 201, 110, 52),
            VehiclePaint.Matte_White => Color.FromArgb(255, 217, 217, 217),
            VehiclePaint.Worn_White => Color.FromArgb(255, 240, 240, 240),
            VehiclePaint.Worn_Olive_Army_Green => Color.FromArgb(255, 63, 66, 40),
            VehiclePaint.Pure_White => Color.FromArgb(255, 255, 255, 255),
            VehiclePaint.Hot_Pink => Color.FromArgb(255, 176, 18, 89),
            VehiclePaint.Salmon_Pink => Color.FromArgb(255, 246, 151, 153),
            VehiclePaint.Metallic_Vermillion_Pink => Color.FromArgb(255, 143, 47, 85),
            VehiclePaint.Orange => Color.FromArgb(255, 194, 102, 16),
            VehiclePaint.Green => Color.FromArgb(255, 105, 189, 69),
            VehiclePaint.Blue => Color.FromArgb(255, 0, 174, 239),
            VehiclePaint.Mettalic_Black_Blue => Color.FromArgb(255, 0, 1, 8),
            VehiclePaint.Metallic_Black_Purple => Color.FromArgb(255, 5, 0, 8),
            VehiclePaint.Metallic_Black_Red => Color.FromArgb(255, 8, 0, 0),
            VehiclePaint.Hunter_Green => Color.FromArgb(255, 86, 87, 81),
            VehiclePaint.Metallic_Purple => Color.FromArgb(255, 50, 6, 66),
            VehiclePaint.Metaillic_V_Dark_Blue => Color.FromArgb(255, 0, 8, 15),
            VehiclePaint.MODSHOP_BLACK1 => Color.FromArgb(255, 8, 8, 8),
            VehiclePaint.Matte_Purple => Color.FromArgb(255, 50, 6, 66),
            VehiclePaint.Matte_Dark_Purple => Color.FromArgb(255, 5, 0, 8),
            VehiclePaint.Metallic_Lava_Red => Color.FromArgb(255, 107, 11, 0),
            VehiclePaint.Matte_Forest_Green => Color.FromArgb(255, 18, 23, 16),
            VehiclePaint.Matte_Olive_Drab => Color.FromArgb(255, 50, 51, 37),
            VehiclePaint.Matte_Desert_Brown => Color.FromArgb(255, 59, 53, 45),
            VehiclePaint.Matte_Desert_Tan => Color.FromArgb(255, 112, 102, 86),
            VehiclePaint.Matte_Foilage_Green => Color.FromArgb(255, 43, 48, 43),
            VehiclePaint.DEFAULT_ALLOY_COLOR => Color.FromArgb(255, 65, 67, 71),
            VehiclePaint.Epsilon_Blue => Color.FromArgb(255, 102, 144, 181),
            VehiclePaint.MP100_GOLD => Color.FromArgb(255, 71, 57, 27),
            VehiclePaint.MP100_GOLD_SATIN => Color.FromArgb(255, 71, 57, 27),
            VehiclePaint.MP100_GOLD_SPEC => Color.FromArgb(255, 255, 216, 89),
            _ => throw new ArgumentOutOfRangeException(nameof(vehiclePaint), vehiclePaint, $"Invalid {nameof(VehiclePaint)}"),
        };
        public static string GetName(this VehiclePaint vehiclePaint)
        {
            string str = vehiclePaint.ToString();
            if (char.IsDigit(str.Last())) str.Remove(str.LastIndexOf(str.Last()) - 1);
            str = str.Replace('_', ' ');
            return str;
        }
        public static string GetPoliceScannerColorAudio(this VehiclePaint vehiclePaint)
        {
            var str = vehiclePaint switch
            {
                VehiclePaint.Metallic_Black => "COLOR_BLACK",
                VehiclePaint.Metallic_Graphite_Black => "COLOR_GRAPHITE",
                VehiclePaint.Metallic_Black_Steal => "COLOR_DARK COLOR_GREY",
                VehiclePaint.Metallic_Dark_Silver => "COLOR_DARK COLOR_GREY",
                VehiclePaint.Metallic_Silver => "COLOR_SILVER",
                VehiclePaint.Metallic_Blue_Silver => "COLOR_SILVER",
                VehiclePaint.Metallic_Steel_Gray => "COLOR_SILVER",
                VehiclePaint.Metallic_Shadow_Silver => "COLOR_DARK COLOR_GREY",
                VehiclePaint.Metallic_Stone_Silver => "COLOR_SILVER",
                VehiclePaint.Metallic_Midnight_Silver => "COLOR_SILVER",
                VehiclePaint.Metallic_Gun_Metal => "COLOR_GREY",
                VehiclePaint.Metallic_Anthracite_Grey => "COLOR_DARK COLOR_GRAPHITE",
                VehiclePaint.Matte_Black => "COLOR_BLACK",
                VehiclePaint.Matte_Gray => "COLOR_GREY",
                VehiclePaint.Matte_Light_Grey => "COLOR_LIGHT COLOR_GREY",
                VehiclePaint.Util_Black => "COLOR_BLACK",
                VehiclePaint.Util_Black_Poly => "COLOR_DARK COLOR_GRAPHITE",
                VehiclePaint.Util_Dark_Silver => "COLOR_LIGHT COLOR_GRAPHITE",
                VehiclePaint.Util_Silver => "COLOR_SILVER",
                VehiclePaint.Util_Gun_Metal => "COLOR_GREY",
                VehiclePaint.Util_Shadow_Silver => "COLOR_GREY",
                VehiclePaint.Worn_Black => "COLOR_BLACK",
                VehiclePaint.Worn_Graphite => "COLOR_GRAPHITE",
                VehiclePaint.Worn_Silver_Grey => "COLOR_GREY",
                VehiclePaint.Worn_Silver => "COLOR_LIGHT COLOR_SILVER",
                VehiclePaint.Worn_Blue_Silver => "COLOR_SILVER",
                VehiclePaint.Worn_Shadow_Silver => "COLOR_GREY",
                VehiclePaint.Metallic_Red => "COLOR_BRIGHT COLOR_RED",
                VehiclePaint.Metallic_Torino_Red => "COLOR_BRIGHT COLOR_RED",
                VehiclePaint.Metallic_Formula_Red => "COLOR_RED",
                VehiclePaint.Metallic_Blaze_Red => "COLOR_RED",
                VehiclePaint.Metallic_Graceful_Red => "COLOR_RED",
                VehiclePaint.Metallic_Garnet_Red => "COLOR_RED",
                VehiclePaint.Metallic_Desert_Red => "COLOR_RED",
                VehiclePaint.Metallic_Cabernet_Red => "COLOR_DARK COLOR_RED",
                VehiclePaint.Metallic_Candy_Red => "COLOR_BRIGHT COLOR_RED",
                VehiclePaint.Metallic_Sunrise_Orange => "COLOR_DARK COLOR_ORANGE",
                VehiclePaint.Metallic_Classic_Gold => "COLOR_LIGHT COLOR_BEIGE",
                VehiclePaint.Metallic_Orange => "COLOR_BRIGHT COLOR_ORANGE",
                VehiclePaint.Matte_Red => "COLOR_RED",
                VehiclePaint.Matte_Dark_Red => "COLOR_DARK COLOR_RED",
                VehiclePaint.Matte_Orange => "COLOR_ORANGE",
                VehiclePaint.Matte_Yellow => "COLOR_YELLOW",
                VehiclePaint.Util_Red => "COLOR_RED",
                VehiclePaint.Util_Bright_Red => "COLOR_BRIGHT COLOR_RED",
                VehiclePaint.Util_Garnet_Red => "COLOR_RED",
                VehiclePaint.Worn_Red => "COLOR_RED",
                VehiclePaint.Worn_Golden_Red => "COLOR_LIGHT COLOR_RED",
                VehiclePaint.Worn_Dark_Red => "COLOR_DARK COLOR_RED",
                VehiclePaint.Metallic_Dark_Green => "COLOR_DARK COLOR_GREEN",
                VehiclePaint.Metallic_Racing_Green => "COLOR_DARK COLOR_GREEN",
                VehiclePaint.Metallic_Sea_Green => "COLOR_GREEN",
                VehiclePaint.Metallic_Olive_Green => "COLOR_GREEN",
                VehiclePaint.Metallic_Green => "COLOR_GREEN",
                VehiclePaint.Metallic_Gasoline_Blue_Green => "COLOR_GREEN",
                VehiclePaint.Matte_Lime_Green => "COLOR_BRIGHT COLOR_GREEN",
                VehiclePaint.Util_Dark_Green => "COLOR_DARK COLOR_GREEN",
                VehiclePaint.Util_Green => "COLOR_GREEN",
                VehiclePaint.Worn_Dark_Green => "COLOR_DARK COLOR_GREEN",
                VehiclePaint.Worn_Green => "COLOR_GREEN",
                VehiclePaint.Worn_Sea_Wash => "COLOR_LIGHT COLOR_GREEN",
                VehiclePaint.Metallic_Midnight_Blue => "COLOR_DARK COLOR_BLUE",
                VehiclePaint.Metallic_Dark_Blue => "COLOR_DARK COLOR_BLUE",
                VehiclePaint.Metallic_Saxony_Blue => "COLOR_BLUE",
                VehiclePaint.Metallic_Blue => "COLOR_BLUE",
                VehiclePaint.Metallic_Mariner_Blue => "COLOR_BLUE",
                VehiclePaint.Metallic_Harbor_Blue => "COLOR_BLUE",
                VehiclePaint.Metallic_Diamond_Blue => "COLOR_LIGHT COLOR_BLUE",
                VehiclePaint.Metallic_Surf_Blue => "COLOR_BLUE",
                VehiclePaint.Metallic_Nautical_Blue => "COLOR_BLUE",
                VehiclePaint.Metallic_Bright_Blue => "COLOR_LIGHT COLOR_BLUE",
                VehiclePaint.Metallic_Purple_Blue => "COLOR_BLUE",
                VehiclePaint.Metallic_Spinnaker_Blue => "COLOR_BLUE",
                VehiclePaint.Metallic_Ultra_Blue => "COLOR_BLUE",
                VehiclePaint.Metallic_Bright_Blue2 => "COLOR_BRIGHT COLOR_BLUE",
                VehiclePaint.Util_Dark_Blue => "COLOR_DARK COLOR_BLUE",
                VehiclePaint.Util_Midnight_Blue => "COLOR_DARK COLOR_BLUE",
                VehiclePaint.Util_Blue => "COLOR_BLUE",
                VehiclePaint.Util_Sea_Foam_Blue => "COLOR_BLUE",
                VehiclePaint.Uil_Lightning_Blue => "COLOR_BLUE",
                VehiclePaint.Util_Maui_Blue_Poly => "COLOR_BLUE",
                VehiclePaint.Util_Bright_Blue => "COLOR_BRIGHT COLOR_BLUE",
                VehiclePaint.Matte_Dark_Blue => "COLOR_DARK COLOR_BLUE",
                VehiclePaint.Matte_Blue => "COLOR_BLUE",
                VehiclePaint.Matte_Midnight_Blue => "COLOR_DARK COLOR_BLUE",
                VehiclePaint.Worn_Dark_Blue => "COLOR_DARK COLOR_BLUE",
                VehiclePaint.Worn_Blue => "COLOR_LIGHT COLOR_BLUE",
                VehiclePaint.Worn_Light_Blue => "COLOR_LIGHT COLOR_BLUE",
                VehiclePaint.Metallic_Taxi_Yellow => "COLOR_YELLOW",
                VehiclePaint.Metallic_Race_Yellow => "COLOR_YELLOW",
                VehiclePaint.Metallic_Bronze => "COLOR_DARK COLOR_ORANGE",
                VehiclePaint.Metallic_Yellow_Bird => "COLOR_LIGHT COLOR_YELLOW",
                VehiclePaint.Metallic_Lime => "COLOR_BRIGHT COLOR_GREEN",
                VehiclePaint.Metallic_Champagne => "COLOR_BEIGE",
                VehiclePaint.Metallic_Pueblo_Beige => "COLOR_BEIGE",
                VehiclePaint.Metallic_Dark_Ivory => "COLOR_BROWN",
                VehiclePaint.Metallic_Choco_Brown => "COLOR_DARK COLOR_BROWN",
                VehiclePaint.Metallic_Golden_Brown => "COLOR_DARK COLOR_BROWN",
                VehiclePaint.Metallic_Light_Brown => "COLOR_LIGHT COLOR_BROWN",
                VehiclePaint.Metallic_Straw_Beige => "COLOR_LIGHT COLOR_BEIGE",
                VehiclePaint.Metallic_Moss_Brown => "COLOR_BROWN",
                VehiclePaint.Metallic_Biston_Brown => "COLOR_BROWN",
                VehiclePaint.Metallic_Beechwood => "COLOR_LIGHT COLOR_BROWN",
                VehiclePaint.Metallic_Dark_Beechwood => "COLOR_DARK COLOR_BROWN",
                VehiclePaint.Metallic_Choco_Orange => "COLOR_DARK COLOR_ORANGE",
                VehiclePaint.Metallic_Beach_Sand => "COLOR_LIGHT COLOR_BROWN",
                VehiclePaint.Metallic_Sun_Bleeched_Sand => "COLOR_LIGHT COLOR_BEIGE",
                VehiclePaint.Metallic_Cream => "COLOR_LIGHT COLOR_BEIGE",
                VehiclePaint.Util_Brown => "COLOR_DARK COLOR_BROWN",
                VehiclePaint.Util_Medium_Brown => "COLOR_BROWN",
                VehiclePaint.Util_Light_Brown => "COLOR_LIGHT COLOR_BROWN",
                VehiclePaint.Metallic_White => "COLOR_WHITE",
                VehiclePaint.Metallic_Frost_White => "COLOR_WHITE",
                VehiclePaint.Worn_Honey_Beige => "COLOR_LIGHT COLOR_BEIGE",
                VehiclePaint.Worn_Brown => "COLOR_DARK COLOR_BROWN",
                VehiclePaint.Worn_Dark_Brown => "COLOR_DARK COLOR_BROWN",
                VehiclePaint.Worn_Straw_Beige => "COLOR_BEIGE",
                VehiclePaint.Brushed_Steel => "COLOR_DARK COLOR_GREY",
                VehiclePaint.Brushed_Black_Steel => "COLOR_GREY",
                VehiclePaint.Brushed_Aluminium => "COLOR_LIGHT COLOR_GREY",
                VehiclePaint.Chrome => "COLOR_BRIGHT COLOR_GREY",
                VehiclePaint.Worn_Off_White => "COLOR_WHITE",
                VehiclePaint.Util_Off_White => "COLOR_WHITE",
                VehiclePaint.Worn_Orange => "COLOR_ORANGE",
                VehiclePaint.Worn_Light_Orange => "COLOR_ORANGE",
                VehiclePaint.Metallic_Securicor_Green => "COLOR_LIGHT COLOR_GREEN",
                VehiclePaint.Worn_Taxi_Yellow => "COLOR_YELLOW",
                VehiclePaint.Police_Car_Blue => "COLOR_BLUE",
                VehiclePaint.Matte_Green => "COLOR_DARK COLOR_GREEN",
                VehiclePaint.Matte_Brown => "COLOR_BROWN",
                VehiclePaint.Worn_Orange2 => "COLOR_BRIGHT COLOR_ORANGE",
                VehiclePaint.Matte_White => "COLOR_WHITE",
                VehiclePaint.Worn_White => "COLOR_WHITE",
                VehiclePaint.Worn_Olive_Army_Green => "COLOR_GREEN",
                VehiclePaint.Pure_White => "COLOR_WHITE",
                VehiclePaint.Hot_Pink => "COLOR_BRIGHT COLOR_PINK",
                VehiclePaint.Salmon_Pink => "COLOR_PINK",
                VehiclePaint.Metallic_Vermillion_Pink => "COLOR_BRIGHT COLOR_PINK",
                VehiclePaint.Orange => "COLOR_LIGHT COLOR_ORANGE",
                VehiclePaint.Green => "COLOR_GREEN",
                VehiclePaint.Blue => "COLOR_BLUE",
                VehiclePaint.Mettalic_Black_Blue => "COLOR_DARK COLOR_BLUE",
                VehiclePaint.Metallic_Black_Purple => "COLOR_DARK COLOR_BLUE",
                VehiclePaint.Metallic_Black_Red => "COLOR_DARK COLOR_RED",
                VehiclePaint.Hunter_Green => "COLOR_DARK COLOR_GREEN",
                VehiclePaint.Metallic_Purple => "COLOR_RED",
                VehiclePaint.Metaillic_V_Dark_Blue => "COLOR_DARK COLOR_BLUE",
                VehiclePaint.MODSHOP_BLACK1 => "COLOR_BLACK",
                VehiclePaint.Matte_Purple => "COLOR_DARK COLOR_GREEN",
                VehiclePaint.Matte_Dark_Purple => "COLOR_DARK COLOR_GREEN",
                VehiclePaint.Metallic_Lava_Red => "COLOR_RED",
                VehiclePaint.Matte_Forest_Green => "COLOR_DARK COLOR_GREEN",
                VehiclePaint.Matte_Olive_Drab => "COLOR_DARK COLOR_GREEN",
                VehiclePaint.Matte_Desert_Brown => "COLOR_LIGHT COLOR_BEIGE",
                VehiclePaint.Matte_Desert_Tan => "COLOR_LIGHT COLOR_BEIGE",
                VehiclePaint.Matte_Foilage_Green => "COLOR_DARK COLOR_GREEN",
                VehiclePaint.DEFAULT_ALLOY_COLOR => "COLOR_LIGHT COLOR_GREY",
                VehiclePaint.Epsilon_Blue => "COLOR_LIGHT COLOR_BLUE",
                VehiclePaint.MP100_GOLD => "COLOR_LIGHT COLOR_BEIGE",
                VehiclePaint.MP100_GOLD_SATIN => "COLOR_LIGHT COLOR_BEIGE",
                VehiclePaint.MP100_GOLD_SPEC => "COLOR_LIGHT COLOR_BEIGE",
                _ => string.Empty,
            };
            string[] metal = new[] { "metallic", "mettalic", "Metaillic" };
            if (metal.Any(x=> vehiclePaint.ToString().ToLower().Contains(x)))
            {
                str = "COLOR_METALLIC " + str;
            }
            return str;
        }
    }
}
