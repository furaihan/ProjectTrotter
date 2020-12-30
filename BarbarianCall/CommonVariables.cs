using BarbarianCall.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Rage;

namespace BarbarianCall
{
    //Variable-Only class
    public class CommonVariables
    {
        public static string[] CarsToSelect =
        {
            "ASEA", "STANIER", "INTRUDER", "PREMIER", "TAILGATER", "WASHINGTON", "ASTEROPE", "EMPEROR", "GLENDALE", "FUGITIVE", "EMPEROR2", "FQ2", "BALLER", "CAVALCADE",
            "SULTAN", "NEON", "KURUMA", "SCHAFTER2", "SCHAFTER3", "BUFFALO", "REVOLTER", "RAIDEN", "ORACLE", "ORACLE2", "JACKAL", "FELON", "FELON2", "F620", "HUNTLEY", "MESA",
            "HABANERO", "LANDSTALKER", "DYNASTY", "FAGALOA", "TULIP", "DILETTANTE"
        };
        public static string[] MotorBikesToSelect = { "AKUMA", "HEXER", "BAGGER", "BATI", "PCJ", "NEMESIS", "VINDICATOR", "THRUST", "FAGGIO", "VADER", "HAKUCHOU", "DOUBLE" };
        public static Color[] CommonUnderstandableColor =
        {
            Color.Purple, Color.Yellow, Color.LimeGreen, Color.Red, Color.Blue, Color.Green, Color.Pink, Color.Gray, Color.Aqua, Color.Magenta, Color.Black, Color.Cyan,
            Color.Brown, Color.Chocolate, Color.Gold, Color.HotPink, Color.Violet, Color.Lavender
        };
        public static Color[] AudibleColor =
        {
            Color.Aqua, Color.Beige, Color.Black, Color.Blue, Color.Brown, Color.DarkBlue, Color.DarkGreen, Color.DarkOrange, Color.DarkRed, Color.Gold, Color.Green,
            Color.LightBlue, Color.LightGreen, Color.LightYellow, Color.Maroon, Color.Orange, Color.Pink, Color.Purple, Color.Red, Color.Silver, Color.White, Color.Yellow
        };
        public static List<string> DangerousVehicleItems = new List<string>
        {
            "a small cardboard filled with crystal methamphetamine", "an automatic rifle", "a small bag with 250 grams of cocain inside", "a bottle crates with full of molotov cocktail",
            "an USB with several hacking software", "an USB that contains a collection of child pornography videos", "a withcraft doll", "a withcraft doll and a photo of the mayor of Los Santos",
            "a heavvy sniper mk2", "a dismantled combat mg mk2", "50kg of unmarked powder", "several fake ID Card", "several stolen driver license", "several stolen vehicle registration"
        };
        public static List<string> DangerousPedItem = new List<string>
        {
            "a plastic of ectasy", "a plastic of MDMA", "a plastic of cocaine", "a plastic of marijuana", "a plastic of cannibis", "a plastic of LSD", "a plastic of morphine", "a plastic of fentanyl",
            "a plastic of methamphetamine", "a plastic of tramadol", "a plastic of ketamine", "a plastic of PCP", "a plastic of opium", "a revolver", "a knife", "suppressor sniper",
            "molotov cocktail", "chains", "a sickle", "a folded money containing an unknown white powder"
        };
        public static List<string> SuspiciousItems = new List<string>
        {
            "an underwear", "a pair of socks and an underwear", "suppressor sniper", "machete", "a stun gun", "survival knife", "an airsoft gun", "a hacking book",
            "a bag with several photos of different people", "a note with several phone numbers", "a pirated music album", "an USB with several pirated software",
            "severeal jewelry", "a bottle crates full of empty bottle", "several car keys", "several IFruit phones", "a hacking book", "a map of los santos with some heist-related note",
            "a map of vangelico jewelry store with some suspicious mark", "several surgical tools", "several bag of blood", "some mystical stuff", "several nails wrapped with newspaper",
            "several medicine"
        };
        public static List<string> CommonItems = new List<string>
        {
            "speaker", "religion-related book", "a portable fan", "a nail clipper", "a car key", "a remote", "a barbie doll", "an eraser", "a ballpoint", "a pencil",
            "several pencil", "several ballpoint", "a bandage", "several bandage", "first aid kit", "a sunglasses", "several sunglasses", "a pair of socks", "a pair of shoes",
            "a programming tutorial book", "a recipe book", "some journal", "today's newspaper", "some old newspaper", "yesterday's newspaper", "a medicated oil", "hand sanitizer",
            "a medicine", "a candy", "a pack of candy", " an ID card"
        };
        public static string[] DonutModel =
        {
            "prop_donut_01", "prop_donut_02", "prop_donut_02b", "prop_amb_donut"
        };
        public static Model[] MaleModel = Model.PedModels.Where(m => m.Name.ToLower().Substring(1, 3) == "_m_").ToArray();
        public static Model[] FemaleModel = Model.PedModels.Where(m => m.Name.ToLower().Substring(1, 3) == "_f_").ToArray();
    }
}
