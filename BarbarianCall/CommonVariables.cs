using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbarianCall
{
    //Variable-Only class
    public class CommonVariables
    {
        public static string[] CarsToSelect =
        {
            "ASEA", "STANIER", "INTRUDER", "PREMIER", "WAILGATER", "WASHINGTON", "ASTEROPE", "EMPEROR", "GLENDALE", "FUGITIVE", "EMPEROR2", "FQ2", "BALLER", "CAVALCADE",
            "SULTAN", "NEON", "KURUMA", "SCHAFTER2", "SCHAFTER3", "BUFFALO", "REVOLTER", "RAIDEN", "ORACLE", "ORACLE2", "JACKAL", "FELON", "FELON2", "F620", "HUNTLEY", "MESA",
            "HABANERO", "LANDSTALKER", "DYNASTY", "FAGALOA", "TULIP", "DILETTANTE"
        };
        public static Color[] CommonUnderstandableColor =
        {
            Color.Purple, Color.Yellow, Color.LimeGreen, Color.Red, Color.Blue, Color.Green, Color.Pink, Color.Gray, Color.Aqua, Color.Magenta, Color.Black, Color.Cyan,
            Color.Brown, Color.Chocolate, Color.Gold, Color.HotPink
        };
        public static List<string> DangerousVehicleItems = new List<string>
        {
            "a small cardboard filled with crystal methamphetamine", "an automatic rifle", "a small bag with 250 grams of cocain inside", "a bottle crates with full of molotov cocktail",
            "an USB with several hacking software", "an USB that contains a collection of child pornography videos", "a withcraft doll", "a withcraft doll and a photo of the mayor of Los Santos",
            "a heavvy sniper mk2", "a dismantled combat mg mk2", "50kg of unmarked powder"
        };
        public static string[] DonutModel =
        {
            "prop_donut_01", "prop_donut_02", "prop_donut_02b", "prop_amb_donut"
        };
    }
}
