﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Rage;
using BarbarianCall.Extensions;

namespace BarbarianCall
{
    //Variable-Only class
    public class Globals
    {
        public static Model[] CarsToSelect =
        {
            "ASEA", "STANIER", "INTRUDER", "PREMIER", "TAILGATER", "WASHINGTON", "ASTEROPE", "EMPEROR", "GLENDALE", "FUGITIVE", "EMPEROR2", "FQ2", "BALLER", "CAVALCADE",
            "SULTAN", "NEON", "KURUMA", "SCHAFTER2", "SCHAFTER3", "BUFFALO", "REVOLTER", "RAIDEN", "ORACLE", "ORACLE2", "JACKAL", "FELON", "FELON2", "F620", "HUNTLEY", "MESA",
            "HABANERO", "LANDSTALKER", "DYNASTY", "FAGALOA", "TULIP", "DILETTANTE", "NEON", "RAPIDGT", "PEYOTE", "MANANA", "TORNADO", "RUMPO", "YOUGA", "SPEEDO", "VOLTIC", "CYCLONE",
            "TROPOS", "GRANGER", "RADI", "FUGITIVE", "COGNOSCENTI", "INGOT", "SENTINEL", "SANDKING"
        };
        public static Model[] MotorBikesToSelect = { "AKUMA", "HEXER", "BAGGER", "BATI", "PCJ", "NEMESIS", "VINDICATOR", "THRUST", "FAGGIO", "VADER", "HAKUCHOU", "DOUBLE", "BF400", "hakuchou",
        "FAGGIO", "LECTRO", "INNOVATION"};
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
        public static List<string> DangerousVehicleItems = new()
        {
            "a small cardboard filled with crystal methamphetamine", "an automatic rifle", "a small bag with 250 grams of cocain inside", "a bottle crates with full of molotov cocktail",
            "an USB with several hacking software", "an USB that contains a collection of child pornography videos", "a withcraft doll", "a withcraft doll and a photo of the mayor of Los Santos",
            "a heavvy sniper mk2", "a dismantled combat mg mk2", "50kg of unmarked powder", "several fake ID Card", "several stolen driver license", "several stolen vehicle registration"
        };
        public static List<string> DangerousPedItem = new()
        {
            "a plastic of ectasy", "a plastic of MDMA", "a plastic of cocaine", "a plastic of marijuana", "a plastic of cannibis", "a plastic of LSD", "a plastic of morphine", "a plastic of fentanyl",
            "a plastic of methamphetamine", "a plastic of tramadol", "a plastic of ketamine", "a plastic of PCP", "a plastic of opium", "a revolver", "a knife", "suppressor sniper",
            "molotov cocktail", "chains", "a sickle", "a folded money containing an unknown white powder"
        };
        public static List<string> SuspiciousItems = new()
        {
            "an underwear", "a pair of socks and an underwear", "suppressor sniper", "machete", "a stun gun", "survival knife", "an airsoft gun", "a hacking book",
            "a bag with several photos of different people", "a note with several phone numbers", "a pirated music album", "an USB with several pirated software",
            "severeal jewelry", "a bottle crates full of empty bottle", "several car keys", "several IFruit phones", "a hacking book", "a map of los santos with some heist-related note",
            "a map of vangelico jewelry store with some suspicious mark", "several surgical tools", "several bag of blood", "some mystical stuff", "several nails wrapped with newspaper",
            "several medicine"
        };
        public static List<string> CommonItems = new()
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
        public static Model[] AudibleCarModel = Extension.GetAudibleVehicleModel().Where(m => m.IsSuitableCar()).ToArray();
        public static Dictionary<string, List<Model>> GangPedModels = new()
        {
            {"Mexican", new List<Model>(){ "g_m_y_mexgang_01", "g_m_y_mexgoon_01", "g_m_y_mexgoon_02", "g_m_y_mexgoon_03", "g_m_y_mexgoon_03", "g_m_m_mexboss_01", "g_m_m_mexboss_02" } },
            {"Ballas", new List<Model>() { "g_m_y_ballasout_01", "g_m_y_ballaeast_01", "g_m_y_ballaorig_01", "ig_ballasog"} },
            {"Families", new List<Model>(){ "g_m_y_famca_01", "mp_m_famdd_01", "g_m_y_famdnf_01", "g_m_y_famfor_01" } },
            {"Salvadoran", new List<Model>(){ "g_m_y_salvagoon_01", "g_m_y_salvagoon_02", "g_m_y_salvagoon_03", "g_m_y_salvaboss_01" } },
            {"Chinese", new List<Model>(){ "g_m_m_chigoon_01", "g_m_m_chigoon_02", "g_m_m_chicold_01", "g_m_m_chiboss_01" } },
            {"Korean", new List<Model>(){ "g_m_y_korean_01", "g_m_y_korean_02", "g_m_y_korlieut_01", "g_m_m_korboss_01" } },
            {"Armenian", new List<Model>(){ "g_m_m_armboss_01", "g_m_m_armgoon_01", "g_m_y_armgoon_02", "g_m_m_armlieut_01" } },
            {"The Lost MC", new List<Model>(){ "g_m_y_lost_01", "g_m_y_lost_02", "g_m_y_lost_03" } },
        };
        public static List<uint?> RegisteredPedHeadshot = new();
        public static Dictionary<string, int> RoadSpeedLimit = new()
        {
            { "Joshua Rd", 50 },
            { "East Joshua Road", 50 },
            { "Marina Dr", 35 },
            { "Alhambra Dr", 35 },
            { "Niland Ave", 35 },
            { "Zancudo Ave", 35 },
            { "Armadillo Ave", 35 },
            { "Algonquin Blvd", 35 },
            { "Mountain View Dr", 35 },
            { "Cholla Springs Ave", 35 },
            { "Panorama Dr", 40 },
            { "Lesbos Ln", 35 },
            { "Calafia Rd", 30 },
            { "North Calafia Way", 30 },
            { "Cassidy Trail", 25 },
            { "Seaview Rd", 35 },
            { "Grapeseed Main St", 35 },
            { "Grapeseed Ave", 35 },
            { "Joad Ln", 35 },
            { "Union Rd", 40 },
            { "O'Neil Way", 25 },
            { "Senora Fwy", 65 },
            { "Catfish View", 35 },
            { "Great Ocean Hwy", 60 },
            { "Paleto Blvd", 35 },
            { "Duluoz Ave", 35 },
            { "Procopio Dr", 35 },
            { "Cascabel Ave", 30 },
            { "Procopio Promenade", 25 },
            { "Pyrite Ave", 30 },
            { "Fort Zancudo Approach Rd", 25 },
            { "Barbareno Rd", 30 },
            { "Ineseno Road", 30 },
            { "West Eclipse Blvd", 35 },
            { "Playa Vista", 30 },
            { "Bay City Ave", 30 },
            { "Del Perro Fwy", 65 },
            { "Equality Way", 30 },
            { "Red Desert Ave", 30 },
            { "Magellan Ave", 25 },
            { "Sandcastle Way", 30 },
            { "Vespucci Blvd", 40 },
            { "Prosperity St", 30 },
            { "San Andreas Ave", 40 },
            { "North Rockford Dr", 35 },
            { "South Rockford Dr", 35 },
            { "Marathon Ave", 30 },
            { "Boulevard Del Perro", 35 },
            { "Cougar Ave", 30 },
            { "Liberty St", 30 },
            { "Bay City Incline", 40 },
            { "Conquistador St", 25 },
            { "Cortes St", 25 },
            { "Vitus St", 25 },
            { "Aguja St", 25 },
            { "Goma St", 25 },
            { "Melanoma St", 25 },
            { "Palomino Ave", 35 },
            { "Invention Ct", 25 },
            { "Imagination Ct", 25 },
            { "Rub St", 25 },
            { "Tug St", 25 },
            { "Ginger St", 30 },
            { "Lindsay Circus", 30 },
            { "Calais Ave", 35 },
            { "Adam's Apple Blvd", 40 },
            { "Alta St", 40 },
            { "Integrity Way", 30 },
            { "Swiss St", 30 },
            { "Strawberry Ave", 40 },
            { "Capital Blvd", 30 },
            { "Crusade Rd", 30 },
            { "Innocence Blvd", 40 },
            { "Davis Ave", 40 },
            { "Little Bighorn Ave", 35 },
            { "Roy Lowenstein Blvd", 35 },
            { "Jamestown St", 30 },
            { "Carson Ave", 35 },
            { "Grove St", 30 },
            { "Brouge Ave", 30 },
            { "Covenant Ave", 30 },
            { "Dutch London St", 40 },
            { "Signal St", 30 },
            { "Elysian Fields Fwy", 50 },
            { "Plaice Pl", 30 },
            { "Chum St", 40 },
            { "Chupacabra St", 30 },
            { "Miriam Turner Overpass", 30 },
            { "Autopia Pkwy", 35 },
            { "Exceptionalists Way", 35 },
            { "La Puerta Fwy", 60 },
            { "New Empire Way", 30 },
            { "Runway1", 00},
            { "Greenwich Pkwy", 35 },
            { "Kortz Dr", 30 },
            { "Banham Canyon Dr", 40 },
            { "Buen Vino Rd", 40 },
            { "Route 68", 55 },
            { "Zancudo Grande Valley", 40 },
            { "Zancudo Barranca", 40 },
            { "Galileo Rd", 40 },
            { "Mt Vinewood Dr", 40 },
            { "Marlowe Dr", 40 },
            { "Milton Rd", 35 },
            { "Kimble Hill Dr", 35 },
            { "Normandy Dr", 35 },
            { "Hillcrest Ave", 35 },
            { "Hillcrest Ridge Access Rd", 35 },
            { "North Sheldon Ave", 35 },
            { "Lake Vinewood Dr", 35 },
            { "Lake Vinewood Est", 35 },
            { "Baytree Canyon Rd", 40 },
            { "North Conker Ave", 35 },
            { "Wild Oats Dr", 35 },
            { "Whispymound Dr", 35 },
            { "Didion Dr", 35 },
            { "Cox Way", 35 },
            { "Picture Perfect Drive", 35 },
            { "South Mo Milton Dr", 35 },
            { "Cockingend Dr", 35 },
            { "Mad Wayne Thunder Dr", 35 },
            { "Hangman Ave", 35 },
            { "Dunstable Ln", 35 },
            { "Dunstable Dr", 35 },
            { "Greenwich Way", 35 },
            { "Greenwich Pl", 35 },
            { "Hardy Way", 35 },
            { "Richman St", 35 },
            { "Ace Jones Dr", 35 },
            { "Los Santos Freeway", 65 },
            { "Senora Rd", 40 },
            { "Nowhere Rd", 25 },
            { "Smoke Tree Rd", 35 },
            { "Cholla Rd", 35 },
            { "Cat Claw Ave", 35 },
            { "Senora Way", 40 },
            { "Palomino Fwy", 60 },
            { "Shank St", 25 },
            { "Macdonald St", 35 },
            { "Route 68 Approach", 55 },
            { "Vinewood Park Dr", 35 },
            { "Vinewood Blvd", 40 },
            { "Mirror Park Blvd", 35 },
            { "Glory Way", 35 },
            { "Bridge St", 35 },
            { "West Mirror Drive", 35 },
            { "Nikola Ave", 35 },
            { "East Mirror Dr", 35 },
            { "Nikola Pl", 25 },
            { "Mirror Pl", 35 },
            { "El Rancho Blvd", 40 },
            { "Olympic Fwy", 60 },
            { "Fudge Ln", 25 },
            { "Amarillo Vista", 25 },
            { "Labor Pl", 35 },
            { "El Burro Blvd", 35 },
            { "Sustancia Rd", 45 },
            { "South Shambles St", 30 },
            { "Hanger Way", 30 },
            { "Orchardville Ave", 30 },
            { "Popular St", 40 },
            { "Buccaneer Way", 45 },
            { "Abattoir Ave", 35 },
            { "Voodoo Place", 30 },
            { "Mutiny Rd", 35 },
            { "South Arsenal St", 35 },
            { "Forum Dr", 35 },
            { "Morningwood Blvd", 35 },
            { "Dorset Dr", 40 },
            { "Caesars Place", 25 },
            { "Spanish Ave", 30 },
            { "Portola Dr", 30 },
            { "Edwood Way", 25 },
            { "San Vitus Blvd", 40 },
            { "Eclipse Blvd", 35 },
            { "Gentry Lane", 30 },
            { "Las Lagunas Blvd", 40 },
            { "Power St", 40 },
            { "Mt Haan Rd", 40 },
            { "Elgin Ave", 40 },
            { "Hawick Ave", 35 },
            { "Meteor St", 30 },
            { "Alta Pl", 30 },
            { "Occupation Ave", 35 },
            { "Carcer Way", 40 },
            { "Eastbourne Way", 30 },
            { "Rockford Dr", 35 },
            { "Abe Milton Pkwy", 35 },
            { "Laguna Pl", 30 },
            { "Sinners Passage", 30 },
            { "Atlee St", 30 },
            { "Sinner St", 30 },
            { "Supply St", 30 },
            { "Amarillo Way", 35 },
            { "Tower Way", 35 },
            { "Decker St", 35 },
            { "Tackle St", 25 },
            { "Low Power St", 35 },
            { "Clinton Ave", 35 },
            { "Fenwell Pl", 35 },
            { "Utopia Gardens", 25 },
            { "Cavalry Blvd", 35 },
            { "South Boulevard Del Perro", 35 },
            { "Americano Way", 25 },
            { "Sam Austin Dr", 25 },
            { "East Galileo Ave", 35 },
            { "Galileo Park", 35 },
            { "West Galileo Ave", 35 },
            { "Tongva Dr", 40 },
            { "Zancudo Rd", 35 },
            { "Movie Star Way", 35 },
            { "Heritage Way", 35 },
            { "Perth St", 25 },
            { "Chianski Passage", 30 },
            { "Lolita Ave", 35 },
            { "Meringue Ln", 35 },
            { "Strangeways Dr", 30 },
        };
    }
}