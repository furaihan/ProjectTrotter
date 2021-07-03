namespace BarbarianCall
{
    using BarbarianCall.Extensions;
    using BarbarianCall.Types;
    using Rage;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    //Variable-Only class
    /// <summary>
    /// Defines the <see cref="Globals" />.
    /// </summary>
    public class Globals
    {
        /// <summary>
        /// Defines the CarsToSelect.
        /// </summary>
        public static Model[] CarsToSelect =
        {
            "ASEA", "STANIER", "INTRUDER", "PREMIER", "TAILGATER", "WASHINGTON", "ASTEROPE", "EMPEROR", "GLENDALE", "FUGITIVE", "EMPEROR2", "FQ2", "BALLER", "CAVALCADE",
            "SULTAN", "NEON", "KURUMA", "SCHAFTER2", "SCHAFTER3", "BUFFALO", "REVOLTER", "RAIDEN", "ORACLE", "ORACLE2", "JACKAL", "FELON", "FELON2", "F620", "HUNTLEY", "MESA",
            "HABANERO", "LANDSTALKER", "DYNASTY", "FAGALOA", "TULIP", "DILETTANTE", "NEON", "RAPIDGT", "PEYOTE", "MANANA", "TORNADO", "RUMPO", "YOUGA", "SPEEDO", "VOLTIC", "CYCLONE",
            "TROPOS", "GRANGER", "RADI", "FUGITIVE", "COGNOSCENTI", "INGOT", "SENTINEL", "SANDKING", "DUNE", "FUSILADE", "INTRUDER", "INFERNUS", "BUCCANEER", "BUCCANEER2", "DOMINATOR", "DOMINATOR2",
            "DOMINATOR3", "DOMINATOR4", "DOMINATOR5", "COMET2", "COMET3", "COMET4", "COMET5", "BANSHEE" 
        };

        /// <summary>
        /// Defines the MotorBikesToSelect.
        /// </summary>
        public static Model[] MotorBikesToSelect = { "AKUMA", "HEXER", "BAGGER", "BATI", "PCJ", "NEMESIS", "VINDICATOR", "THRUST", "FAGGIO", "VADER", "HAKUCHOU", "DOUBLE", "BF400", "hakuchou",
        "FAGGIO", "LECTRO", "INNOVATION"};

        /// <summary>
        /// Defines the CommonUnderstandableColor.
        /// </summary>
        public static Color[] CommonUnderstandableColor =
        {
            Color.Purple, Color.Yellow, Color.LimeGreen, Color.Red, Color.Blue, Color.Green, Color.Pink, Color.Gray, Color.Aqua, Color.Magenta, Color.Black, Color.Cyan,
            Color.Brown, Color.Chocolate, Color.Gold, Color.HotPink, Color.Violet, Color.Lavender
        };

        /// <summary>
        /// Defines the AudibleColor.
        /// </summary>
        public static Color[] AudibleColor =
        {
            Color.Aqua, Color.Beige, Color.Black, Color.Blue, Color.Brown, Color.DarkBlue, Color.DarkGreen, Color.DarkOrange, Color.DarkRed, Color.Gold, Color.Green,
            Color.LightBlue, Color.LightGreen, Color.LightYellow, Color.Maroon, Color.Orange, Color.Pink, Color.Purple, Color.Red, Color.Silver, Color.White, Color.Yellow
        };

        /// <summary>
        /// Defines the DangerousVehicleItems.
        /// </summary>
        public static List<string> DangerousVehicleItems = new()
        {
            "a small cardboard filled with crystal methamphetamine",
            "an automatic rifle",
            "a small bag with 250 grams of cocain inside",
            "a bottle crates with full of molotov cocktail",
            "an USB with several hacking software",
            "an USB that contains a collection of child pornography videos",
            "a withcraft doll",
            "a withcraft doll and a photo of the mayor of Los Santos",
            "a heavvy sniper mk2",
            "a dismantled combat mg mk2",
            "50kg of unmarked powder",
            "several fake ID Card",
            "several stolen driver license",
            "several stolen vehicle registration"
        };

        /// <summary>
        /// Defines the DangerousPedItem.
        /// </summary>
        public static List<string> DangerousPedItem = new()
        {
            "a plastic of ectasy",
            "a plastic of MDMA",
            "a plastic of cocaine",
            "a plastic of marijuana",
            "a plastic of cannibis",
            "a plastic of LSD",
            "a plastic of morphine",
            "a plastic of fentanyl",
            "a plastic of methamphetamine",
            "a plastic of tramadol",
            "a plastic of ketamine",
            "a plastic of PCP",
            "a plastic of opium",
            "a revolver",
            "a knife",
            "suppressor sniper",
            "molotov cocktail",
            "chains",
            "a sickle",
            "a folded money containing an unknown white powder"
        };

        /// <summary>
        /// Defines the SuspiciousItems.
        /// </summary>
        public static List<string> SuspiciousItems = new()
        {
            "an underwear",
            "a pair of socks and an underwear",
            "suppressor sniper",
            "machete",
            "a stun gun",
            "survival knife",
            "an airsoft gun",
            "a hacking book",
            "a bag with several photos of different people",
            "a note with several phone numbers",
            "a pirated music album",
            "an USB with several pirated software",
            "severeal jewelry",
            "a bottle crates full of empty bottle",
            "several car keys",
            "several IFruit phones",
            "a hacking book",
            "a map of los santos with some heist-related note",
            "a map of vangelico jewelry store with some suspicious mark",
            "several surgical tools",
            "several bag of blood",
            "some mystical stuff",
            "several nails wrapped with newspaper",
            "several medicine"
        };

        /// <summary>
        /// Defines the CommonItems.
        /// </summary>
        public static List<string> CommonItems = new()
        {
            "speaker",
            "religion-related book",
            "a portable fan",
            "a nail clipper",
            "a car key",
            "a remote",
            "a barbie doll",
            "an eraser",
            "a ballpoint",
            "a pencil",
            "several pencil",
            "several ballpoint",
            "a bandage",
            "several bandage",
            "first aid kit",
            "a sunglasses",
            "several sunglasses",
            "a pair of socks",
            "a pair of shoes",
            "a programming tutorial book",
            "a recipe book",
            "some journal",
            "today's newspaper",
            "some old newspaper",
            "yesterday's newspaper",
            "a medicated oil",
            "hand sanitizer",
            "a medicine",
            "a candy",
            "a pack of candy",
            " an ID card"
        };

        /// <summary>
        /// Defines the DonutModel.
        /// </summary>
        public static string[] DonutModel =
        {
            "prop_donut_01", "prop_donut_02", "prop_donut_02b", "prop_amb_donut"
        };

        /// <summary>
        /// Defines the AudibleCarModel.
        /// </summary>
        public static Model[] AudibleCarModel;
        public static Dictionary<uint, string> AudioHash = new Dictionary<uint, string>();

        /// <summary>
        /// Defines the GangPedModels.
        /// </summary>
        public static Dictionary<string, List<Model>> GangPedModels = new()
        {
            { "Mexican", new List<Model>() { "g_m_y_mexgang_01", "g_m_y_mexgoon_01", "g_m_y_mexgoon_02", "g_m_y_mexgoon_03", "g_m_y_mexgoon_03", "g_m_m_mexboss_01", "g_m_m_mexboss_02" } },
            { "Ballas", new List<Model>() { "g_m_y_ballasout_01", "g_m_y_ballaeast_01", "g_m_y_ballaorig_01", "ig_ballasog" } },
            { "Families", new List<Model>() { "g_m_y_famca_01", "mp_m_famdd_01", "g_m_y_famdnf_01", "g_m_y_famfor_01" } },
            { "Salvadoran", new List<Model>() { "g_m_y_salvagoon_01", "g_m_y_salvagoon_02", "g_m_y_salvagoon_03", "g_m_y_salvaboss_01" } },
            { "Chinese", new List<Model>() { "g_m_m_chigoon_01", "g_m_m_chigoon_02", "g_m_m_chicold_01", "g_m_m_chiboss_01" } },
            { "Korean", new List<Model>() { "g_m_y_korean_01", "g_m_y_korean_02", "g_m_y_korlieut_01", "g_m_m_korboss_01" } },
            { "Armenian", new List<Model>() { "g_m_m_armboss_01", "g_m_m_armgoon_01", "g_m_y_armgoon_02", "g_m_m_armlieut_01" } },
            { "The Lost MC", new List<Model>() { "g_m_y_lost_01", "g_m_y_lost_02", "g_m_y_lost_03" } },
        };

        /// <summary>
        /// Defines the RegisteredPedHeadshot.
        /// </summary>
        public static List<uint?> RegisteredPedHeadshot = new();

        /// <summary>
        /// Defines the Normal.
        /// </summary>
        public static VehicleDrivingFlags Normal = (VehicleDrivingFlags)786603;

        /// <summary>
        /// Defines the Hospitals.
        /// </summary>
        public static List<Spawnpoint> Hospitals = new()
        {
            new Spawnpoint(new Vector3(302.67f, -1434.97f, 29.8f), 226.77f),
            new Spawnpoint(new Vector3(-655.09f, 306.63f, 82.66f), 351.63f),
            new Spawnpoint(new Vector3(-468.15f, -338.61f, 34.37f), 340.77f),
            new Spawnpoint(new Vector3(364.79f, -591.24f, 28.69f), 156.37f),
            new Spawnpoint(new Vector3(-863.01f, -301.81f, 39.06f), 237.494f),
            new Spawnpoint(new Vector3(1157.52f, -1514.13f, 34.69f), 267.04f),
            new Spawnpoint(new Vector3(1828.68f, 3693.94f, 34.22f), 299.74f),
            new Spawnpoint(new Vector3(-239.51f, 6333.98f, 32.43f), 39.85f)
        };

        /// <summary>
        /// Gets the PlayerPedName.
        /// </summary>
        public static string PlayerPedName=> LSPD_First_Response.Mod.API.Functions.GetPersonaForPed(Game.LocalPlayer.Character).FullName;

        /// <summary>
        /// Gets the PlayerPedSurName.
        /// </summary>
        public static string PlayerPedSurName=> LSPD_First_Response.Mod.API.Functions.GetPersonaForPed(Game.LocalPlayer.Character).Surname;

        /// <summary>
        /// Gets the PlayerPedForeName.
        /// </summary>
        public static string PlayerPedForeName=> LSPD_First_Response.Mod.API.Functions.GetPersonaForPed(Game.LocalPlayer.Character).Forename;

        /// <summary>
        /// Defines the MaleVoiceName.
        /// </summary>
        public static List<string> MaleVoiceName = new List<string>()
        {
            "A_M_M_AFRIAMER_01_BLACK_FULL_01",
            "A_M_M_BEACH_01_BLACK_MINI_01",
            "A_M_M_BEACH_01_LATINO_FULL_01",
            "A_M_M_BEACH_01_LATINO_MINI_01",
            "A_M_M_BEACH_01_WHITE_FULL_01",
            "A_M_M_BEACH_01_WHITE_MINI_02",
            "A_M_M_BEACH_02_BLACK_FULL_01",
            "A_M_M_BEACH_02_WHITE_FULL_01",
            "A_M_M_BEACH_02_WHITE_MINI_01",
            "A_M_M_BEACH_02_WHITE_MINI_02",
            "A_M_M_BEVHILLS_01_BLACK_FULL_01",
            "A_M_M_BEVHILLS_01_BLACK_MINI_01",
            "A_M_M_BEVHILLS_01_WHITE_FULL_01",
            "A_M_M_BEVHILLS_01_WHITE_MINI_01",
            "A_M_M_BEVHILLS_02_BLACK_FULL_01",
            "A_M_M_BEVHILLS_02_BLACK_MINI_01",
            "A_M_M_BEVHILLS_02_WHITE_FULL_01",
            "A_M_M_BEVHILLS_02_WHITE_MINI_01",
            "A_M_M_BUSINESS_01_BLACK_FULL_01",
            "A_M_M_BUSINESS_01_BLACK_MINI_01",
            "A_M_M_BUSINESS_01_WHITE_FULL_01",
            "A_M_M_BUSINESS_01_WHITE_MINI_01",
            "A_M_M_EASTSA_01_LATINO_FULL_01",
            "A_M_M_EASTSA_01_LATINO_MINI_01",
            "A_M_M_EASTSA_02_LATINO_FULL_01",
            "A_M_M_EASTSA_02_LATINO_MINI_01",
            "A_M_M_FARMER_01_WHITE_MINI_01",
            "A_M_M_FARMER_01_WHITE_MINI_02",
            "A_M_M_FARMER_01_WHITE_MINI_03",
            "A_M_M_FATLATIN_01_LATINO_FULL_01",
            "A_M_M_FATLATIN_01_LATINO_MINI_01",
            "A_M_M_GENERICMALE_01_WHITE_MINI_01",
            "A_M_M_GENERICMALE_01_WHITE_MINI_02",
            "A_M_M_GENERICMALE_01_WHITE_MINI_03",
            "A_M_M_GENERICMALE_01_WHITE_MINI_04",
            "A_M_M_GENFAT_01_LATINO_FULL_01",
            "A_M_M_GENFAT_01_LATINO_MINI_01",
            "A_M_M_GOLFER_01_BLACK_FULL_01",
            "A_M_M_GOLFER_01_WHITE_FULL_01",
            "A_M_M_GOLFER_01_WHITE_MINI_01",
            "A_M_M_HASJEW_01_WHITE_MINI_01",
            "A_M_M_HILLBILLY_01_WHITE_MINI_01",
            "A_M_M_HILLBILLY_01_WHITE_MINI_02",
            "A_M_M_HILLBILLY_01_WHITE_MINI_03",
            "A_M_M_HILLBILLY_02_WHITE_MINI_01",
            "A_M_M_HILLBILLY_02_WHITE_MINI_02",
            "A_M_M_HILLBILLY_02_WHITE_MINI_03",
            "A_M_M_HILLBILLY_02_WHITE_MINI_04",
            "A_M_M_INDIAN_01_INDIAN_MINI_01",
            "A_M_M_KTOWN_01_KOREAN_FULL_01",
            "A_M_M_KTOWN_01_KOREAN_MINI_01",
            "A_M_M_MALIBU_01_BLACK_FULL_01",
            "A_M_M_MALIBU_01_LATINO_FULL_01",
            "A_M_M_MALIBU_01_LATINO_MINI_01",
            "A_M_M_MALIBU_01_WHITE_FULL_01",
            "A_M_M_MALIBU_01_WHITE_MINI_01",
            "A_M_M_POLYNESIAN_01_POLYNESIAN_FULL_01",
            "A_M_M_POLYNESIAN_01_POLYNESIAN_MINI_01",
            "A_M_M_SALTON_01_WHITE_FULL_01",
            "A_M_M_SALTON_02_WHITE_FULL_01",
            "A_M_M_SALTON_02_WHITE_MINI_01",
            "A_M_M_SALTON_02_WHITE_MINI_02",
            "A_M_M_SKATER_01_BLACK_FULL_01",
            "A_M_M_SKATER_01_WHITE_FULL_01",
            "A_M_M_SKATER_01_WHITE_MINI_01",
            "A_M_M_SKIDROW_01_BLACK_FULL_01",
            "A_M_M_SOCENLAT_01_LATINO_FULL_01",
            "A_M_M_SOCENLAT_01_LATINO_MINI_01",
            "A_M_M_SOUCENT_01_BLACK_FULL_01",
            "A_M_M_SOUCENT_02_BLACK_FULL_01",
            "A_M_M_SOUCENT_03_BLACK_FULL_01",
            "A_M_M_SOUCENT_04_BLACK_FULL_01",
            "A_M_M_SOUCENT_04_BLACK_MINI_01",
            "A_M_M_STLAT_02_LATINO_FULL_01",
            "A_M_M_TENNIS_01_BLACK_MINI_01",
            "A_M_M_TENNIS_01_WHITE_MINI_01",
            "A_M_M_TOURIST_01_WHITE_MINI_01",
            "A_M_M_TRAMP_01_BLACK_FULL_01",
            "A_M_M_TRAMP_01_BLACK_MINI_01",
            "A_M_M_TRAMPBEAC_01_BLACK_FULL_01",
            "A_M_M_TRANVEST_01_WHITE_MINI_01",
            "A_M_M_TRANVEST_02_LATINO_FULL_01",
            "A_M_M_TRANVEST_02_LATINO_MINI_01",
            "A_M_O_BEACH_01_WHITE_FULL_01",
            "A_M_O_BEACH_01_WHITE_MINI_01",
            "A_M_O_GENSTREET_01_WHITE_FULL_01",
            "A_M_O_GENSTREET_01_WHITE_MINI_01",
            "A_M_O_SALTON_01_WHITE_FULL_01",
            "A_M_O_SALTON_01_WHITE_MINI_01",
            "A_M_O_SOUCENT_01_BLACK_FULL_01",
            "A_M_O_SOUCENT_02_BLACK_FULL_01",
            "A_M_O_SOUCENT_03_BLACK_FULL_01",
            "A_M_O_TRAMP_01_BLACK_FULL_01",
            "A_M_Y_BEACH_01_CHINESE_FULL_01",
            "A_M_Y_BEACH_01_CHINESE_MINI_01",
            "A_M_Y_BEACH_01_WHITE_FULL_01",
            "A_M_Y_BEACH_01_WHITE_MINI_01",
            "A_M_Y_BEACH_02_LATINO_FULL_01",
            "A_M_Y_BEACH_02_WHITE_FULL_01",
            "A_M_Y_BEACH_03_BLACK_FULL_01",
            "A_M_Y_BEACH_03_BLACK_MINI_01",
            "A_M_Y_BEACH_03_WHITE_FULL_01",
            "A_M_Y_BEACHVESP_01_CHINESE_FULL_01",
            "A_M_Y_BEACHVESP_01_CHINESE_MINI_01",
            "A_M_Y_BEACHVESP_01_WHITE_FULL_01",
            "A_M_Y_BEACHVESP_02_WHITE_FULL_01",
            "A_M_Y_BEACHVESP_02_WHITE_MINI_01",
            "A_M_Y_BEVHILLS_01_BLACK_FULL_01",
            "A_M_Y_BEVHILLS_01_WHITE_FULL_01",
            "A_M_Y_BEVHILLS_02_BLACK_FULL_01",
            "A_M_Y_BEVHILLS_02_WHITE_FULL_01",
            "A_M_Y_BEVHILLS_02_WHITE_MINI_01",
            "A_M_Y_BUSICAS_01_WHITE_MINI_01",
            "A_M_Y_BUSINESS_01_BLACK_FULL_01",
            "A_M_Y_BUSINESS_01_BLACK_MINI_01",
            "A_M_Y_BUSINESS_01_CHINESE_FULL_01",
            "A_M_Y_BUSINESS_01_WHITE_FULL_01",
            "A_M_Y_BUSINESS_01_WHITE_MINI_02",
            "A_M_Y_BUSINESS_02_BLACK_FULL_01",
            "A_M_Y_BUSINESS_02_BLACK_MINI_01",
            "A_M_Y_BUSINESS_02_WHITE_FULL_01",
            "A_M_Y_BUSINESS_02_WHITE_MINI_01",
            "A_M_Y_BUSINESS_02_WHITE_MINI_02",
            "A_M_Y_BUSINESS_03_BLACK_FULL_01",
            "A_M_Y_BUSINESS_03_WHITE_MINI_01",
            "A_M_Y_DOWNTOWN_01_BLACK_FULL_01",
            "A_M_Y_EASTSA_01_LATINO_FULL_01",
            "A_M_Y_EASTSA_01_LATINO_MINI_01",
            "A_M_Y_EASTSA_02_LATINO_FULL_01",
            "A_M_Y_EPSILON_01_BLACK_FULL_01",
            "A_M_Y_EPSILON_01_KOREAN_FULL_01",
            "A_M_Y_EPSILON_01_WHITE_FULL_01",
            "A_M_Y_EPSILON_02_WHITE_MINI_01",
            "A_M_Y_GAY_01_BLACK_FULL_01",
            "A_M_Y_GAY_01_LATINO_FULL_01",
            "A_M_Y_GAY_02_WHITE_MINI_01",
            "A_M_Y_GENSTREET_01_CHINESE_FULL_01",
            "A_M_Y_GENSTREET_01_CHINESE_MINI_01",
            "A_M_Y_GENSTREET_01_WHITE_FULL_01",
            "A_M_Y_GENSTREET_01_WHITE_MINI_01",
            "A_M_Y_GENSTREET_02_BLACK_FULL_01",
            "A_M_Y_GENSTREET_02_LATINO_FULL_01",
            "A_M_Y_GENSTREET_02_LATINO_MINI_01",
            "A_M_Y_GOLFER_01_WHITE_FULL_01",
            "A_M_Y_GOLFER_01_WHITE_MINI_01",
            "A_M_Y_HASJEW_01_WHITE_MINI_01",
            "A_M_Y_HIPPY_01_WHITE_FULL_01",
            "A_M_Y_HIPPY_01_WHITE_MINI_01",
            "A_M_Y_HIPSTER_01_BLACK_FULL_01",
            "A_M_Y_HIPSTER_01_WHITE_FULL_01",
            "A_M_Y_HIPSTER_01_WHITE_MINI_01",
            "A_M_Y_HIPSTER_02_WHITE_FULL_01",
            "A_M_Y_HIPSTER_02_WHITE_MINI_01",
            "A_M_Y_HIPSTER_03_WHITE_FULL_01",
            "A_M_Y_HIPSTER_03_WHITE_MINI_01",
            "A_M_Y_KTOWN_01_KOREAN_FULL_01",
            "A_M_Y_KTOWN_01_KOREAN_MINI_01",
            "A_M_Y_KTOWN_02_KOREAN_FULL_01",
            "A_M_Y_KTOWN_02_KOREAN_MINI_01",
            "A_M_Y_LATINO_01_LATINO_MINI_01",
            "A_M_Y_LATINO_01_LATINO_MINI_02",
            "A_M_Y_MEXTHUG_01_LATINO_FULL_01",
            "A_M_Y_MUSCLBEAC_01_BLACK_FULL_01",
            "A_M_Y_MUSCLBEAC_01_WHITE_FULL_01",
            "A_M_Y_MUSCLBEAC_01_WHITE_MINI_01",
            "A_M_Y_MUSCLBEAC_02_CHINESE_FULL_01",
            "A_M_Y_MUSCLBEAC_02_LATINO_FULL_01",
            "A_M_Y_POLYNESIAN_01_POLYNESIAN_FULL_01",
            "A_M_Y_RACER_01_WHITE_MINI_01",
            "A_M_Y_ROLLERCOASTER_01_MINI_01",
            "A_M_Y_ROLLERCOASTER_01_MINI_02",
            "A_M_Y_ROLLERCOASTER_01_MINI_03",
            "A_M_Y_ROLLERCOASTER_01_MINI_04",
            "A_M_Y_RUNNER_01_WHITE_FULL_01",
            "A_M_Y_RUNNER_01_WHITE_MINI_01",
            "A_M_Y_SALTON_01_WHITE_MINI_01",
            "A_M_Y_SALTON_01_WHITE_MINI_02",
            "A_M_Y_SKATER_01_WHITE_FULL_01",
            "A_M_Y_SKATER_01_WHITE_MINI_01",
            "A_M_Y_SKATER_02_BLACK_FULL_01",
            "A_M_Y_SOUCENT_01_BLACK_FULL_01",
            "A_M_Y_SOUCENT_02_BLACK_FULL_01",
            "A_M_Y_SOUCENT_03_BLACK_FULL_01",
            "A_M_Y_SOUCENT_04_BLACK_FULL_01",
            "A_M_Y_SOUCENT_04_BLACK_MINI_01",
            "A_M_Y_STBLA_01_BLACK_FULL_01",
            "A_M_Y_STBLA_02_BLACK_FULL_01",
            "A_M_Y_STLAT_01_LATINO_FULL_01",
            "A_M_Y_STLAT_01_LATINO_MINI_01",
            "A_M_Y_STWHI_01_WHITE_FULL_01",
            "A_M_Y_STWHI_01_WHITE_MINI_01",
            "A_M_Y_STWHI_02_WHITE_FULL_01",
            "A_M_Y_STWHI_02_WHITE_MINI_01",
            "A_M_Y_SUNBATHE_01_BLACK_FULL_01",
            "A_M_Y_SUNBATHE_01_WHITE_FULL_01",
            "A_M_Y_SUNBATHE_01_WHITE_MINI_01",
            "A_M_Y_TRIATHLON_01_MINI_01",
            "A_M_Y_TRIATHLON_01_MINI_02",
            "A_M_Y_TRIATHLON_01_MINI_03",
            "A_M_Y_TRIATHLON_01_MINI_04",
            "A_M_Y_VINEWOOD_01_BLACK_FULL_01",
            "A_M_Y_VINEWOOD_01_BLACK_MINI_01",
            "A_M_Y_VINEWOOD_02_WHITE_FULL_01",
            "A_M_Y_VINEWOOD_02_WHITE_MINI_01",
            "A_M_Y_VINEWOOD_03_LATINO_FULL_01",
            "A_M_Y_VINEWOOD_03_LATINO_MINI_01",
            "A_M_Y_VINEWOOD_03_WHITE_FULL_01",
            "A_M_Y_VINEWOOD_03_WHITE_MINI_01",
            "A_M_Y_VINEWOOD_04_WHITE_FULL_01",
            "A_M_Y_VINEWOOD_04_WHITE_MINI_01",
            "JEROME",
        };
        public static List<string> FemaleVoiceName = new List<string>()
        {
            "A_F_M_BEACH_01_WHITE_FULL_01",
            "S_F_Y_AIRHOSTESS_01_BLACK_FULL_01",
            "S_F_Y_AIRHOSTESS_01_BLACK_FULL_02",
            "S_F_Y_AIRHOSTESS_01_WHITE_FULL_01",
            "S_F_Y_AIRHOSTESS_01_WHITE_FULL_02",
            "A_F_M_BEACH_01_WHITE_MINI_01",
            "A_F_M_BEVHILLS_01_WHITE_FULL_01",
            "A_F_M_BEVHILLS_01_WHITE_MINI_01",
            "A_F_M_BEVHILLS_01_WHITE_MINI_02",
            "A_F_M_BEVHILLS_02_BLACK_FULL_01",
            "A_F_M_BEVHILLS_02_BLACK_MINI_01",
            "A_F_M_BEVHILLS_02_WHITE_FULL_01",
            "A_F_M_BEVHILLS_02_WHITE_FULL_02",
            "A_F_M_BEVHILLS_02_WHITE_MINI_01",
            "A_F_M_BODYBUILD_01_BLACK_FULL_01",
            "A_F_M_BODYBUILD_01_BLACK_MINI_01",
            "A_F_M_BODYBUILD_01_WHITE_FULL_01",
            "A_F_M_BODYBUILD_01_WHITE_MINI_01",
            "A_F_M_BUSINESS_02_WHITE_MINI_01",
            "A_F_M_DOWNTOWN_01_BLACK_FULL_01",
            "A_F_M_EASTSA_01_LATINO_FULL_01",
            "A_F_M_EASTSA_01_LATINO_MINI_01",
            "A_F_M_EASTSA_02_LATINO_FULL_01",
            "A_F_M_EASTSA_02_LATINO_MINI_01",
            "A_F_M_FATWHITE_01_WHITE_FULL_01",
            "A_F_M_FATWHITE_01_WHITE_MINI_01",
            "A_F_M_KTOWN_01_KOREAN_FULL_01",
            "A_F_M_KTOWN_01_KOREAN_MINI_01",
            "A_F_M_KTOWN_02_CHINESE_MINI_01",
            "A_F_M_KTOWN_02_KOREAN_FULL_01",
            "A_F_M_SALTON_01_WHITE_FULL_01",
            "A_F_M_SALTON_01_WHITE_FULL_02",
            "A_F_M_SALTON_01_WHITE_FULL_03",
            "A_F_M_SALTON_01_WHITE_MINI_01",
            "A_F_M_SALTON_01_WHITE_MINI_02",
            "A_F_M_SALTON_01_WHITE_MINI_03",
            "A_F_M_SKIDROW_01_BLACK_FULL_01",
            "A_F_M_SKIDROW_01_BLACK_MINI_01",
            "A_F_M_SKIDROW_01_WHITE_FULL_01",
            "A_F_M_SKIDROW_01_WHITE_MINI_01",
            "A_F_M_SOUCENT_01_BLACK_FULL_01",
            "A_F_M_SOUCENT_02_BLACK_FULL_01",
            "A_F_M_TOURIST_01_WHITE_MINI_01",
            "A_F_M_TRAMP_01_WHITE_FULL_01",
            "A_F_M_TRAMP_01_WHITE_MINI_01",
            "A_F_M_TRAMPBEAC_01_BLACK_FULL_01",
            "A_F_M_TRAMPBEAC_01_BLACK_MINI_01",
            "A_F_M_TRAMPBEAC_01_WHITE_FULL_01",
            "A_F_O_GENSTREET_01_WHITE_MINI_01",
            "A_F_O_INDIAN_01_INDIAN_MINI_01",
            "A_F_O_KTOWN_01_KOREAN_FULL_01",
            "A_F_O_KTOWN_01_KOREAN_MINI_01",
            "A_F_O_SALTON_01_WHITE_FULL_01",
            "A_F_O_SALTON_01_WHITE_MINI_01",
            "A_F_O_SOUCENT_01_BLACK_FULL_01",
            "A_F_O_SOUCENT_02_BLACK_FULL_01",
            "A_F_Y_BEACH_01_BLACK_MINI_01",
            "A_F_Y_BEACH_01_WHITE_FULL_01",
            "A_F_Y_BEACH_01_WHITE_MINI_01",
            "A_F_Y_BEACH_BLACK_FULL_01",
            "A_F_Y_BEVHILLS_01_WHITE_FULL_01",
            "A_F_Y_BEVHILLS_01_WHITE_MINI_01",
            "A_F_Y_BEVHILLS_02_WHITE_FULL_01",
            "A_F_Y_BEVHILLS_02_WHITE_MINI_01",
            "A_F_Y_BEVHILLS_02_WHITE_MINI_02",
            "A_F_Y_BEVHILLS_03_WHITE_FULL_01",
            "A_F_Y_BEVHILLS_03_WHITE_MINI_01",
            "A_F_Y_BEVHILLS_04_WHITE_FULL_01",
            "A_F_Y_BEVHILLS_04_WHITE_MINI_01",
            "A_F_Y_BUSINESS_01_WHITE_FULL_01",
            "A_F_Y_BUSINESS_01_WHITE_MINI_01",
            "A_F_Y_BUSINESS_01_WHITE_MINI_02",
            "A_F_Y_BUSINESS_02_WHITE_FULL_01",
            "A_F_Y_BUSINESS_02_WHITE_MINI_01",
            "A_F_Y_BUSINESS_03_CHINESE_FULL_01",
            "A_F_Y_BUSINESS_03_CHINESE_MINI_01",
            "A_F_Y_BUSINESS_03_LATINO_FULL_01",
            "A_F_Y_BUSINESS_04_BLACK_FULL_01",
            "A_F_Y_BUSINESS_04_BLACK_MINI_01",
            "A_F_Y_BUSINESS_04_WHITE_MINI_01",
            "A_F_Y_EASTSA_01_LATINO_FULL_01",
            "A_F_Y_EASTSA_01_LATINO_MINI_01",
            "A_F_Y_EASTSA_02_WHITE_FULL_01",
            "A_F_Y_EASTSA_03_LATINO_FULL_01",
            "A_F_Y_EASTSA_03_LATINO_MINI_01",
            "A_F_Y_EPSILON_01_WHITE_MINI_01",
            "A_F_Y_FITNESS_01_WHITE_FULL_01",
            "A_F_Y_FITNESS_01_WHITE_MINI_01",
            "A_F_Y_FITNESS_02_BLACK_FULL_01",
            "A_F_Y_FITNESS_02_BLACK_MINI_01",
            "A_F_Y_FITNESS_02_WHITE_FULL_01",
            "A_F_Y_FITNESS_02_WHITE_MINI_01",
            "A_F_Y_GOLFER_01_WHITE_FULL_01",
            "A_F_Y_GOLFER_01_WHITE_MINI_01",
            "A_F_Y_HIKER_01_WHITE_FULL_01",
            "A_F_Y_HIKER_01_WHITE_MINI_01",
            "A_F_Y_HIPSTER_01_WHITE_FULL_01",
            "A_F_Y_HIPSTER_01_WHITE_MINI_01",
            "A_F_Y_HIPSTER_02_WHITE_FULL_01",
            "A_F_Y_HIPSTER_02_WHITE_MINI_01",
            "A_F_Y_HIPSTER_02_WHITE_MINI_02",
            "A_F_Y_HIPSTER_03_WHITE_FULL_01",
            "A_F_Y_HIPSTER_03_WHITE_MINI_01",
            "A_F_Y_HIPSTER_03_WHITE_MINI_02",
            "A_F_Y_HIPSTER_04_WHITE_FULL_01",
            "A_F_Y_HIPSTER_04_WHITE_MINI_01",
            "A_F_Y_HIPSTER_04_WHITE_MINI_02",
            "A_F_Y_INDIAN_01_INDIAN_MINI_01",
            "A_F_Y_INDIAN_01_INDIAN_MINI_02",
            "A_F_Y_ROLLERCOASTER_01_MINI_01",
            "A_F_Y_ROLLERCOASTER_01_MINI_02",
            "A_F_Y_ROLLERCOASTER_01_MINI_03",
            "A_F_Y_ROLLERCOASTER_01_MINI_04",
            "A_F_Y_SKATER_01_WHITE_FULL_01",
            "A_F_Y_SKATER_01_WHITE_MINI_01",
            "A_F_Y_SOUCENT_01_BLACK_FULL_01",
            "A_F_Y_SOUCENT_02_BLACK_FULL_01",
            "A_F_Y_SOUCENT_03_LATINO_FULL_01",
            "A_F_Y_SOUCENT_03_LATINO_MINI_01",
            "A_F_Y_TENNIS_01_BLACK_MINI_01",
            "A_F_Y_TENNIS_01_WHITE_MINI_01",
            "A_F_Y_TOURIST_01_BLACK_FULL_01",
            "A_F_Y_TOURIST_01_BLACK_MINI_01",
            "A_F_Y_TOURIST_01_LATINO_FULL_01",
            "A_F_Y_TOURIST_01_LATINO_MINI_01",
            "A_F_Y_TOURIST_01_WHITE_FULL_01",
            "A_F_Y_TOURIST_01_WHITE_MINI_01",
            "A_F_Y_TOURIST_02_WHITE_MINI_01",
            "A_F_Y_VINEWOOD_01_WHITE_FULL_01",
            "A_F_Y_VINEWOOD_01_WHITE_MINI_01",
            "A_F_Y_VINEWOOD_02_WHITE_FULL_01",
            "A_F_Y_VINEWOOD_02_WHITE_MINI_01",
            "A_F_Y_VINEWOOD_03_CHINESE_FULL_01",
            "A_F_Y_VINEWOOD_03_CHINESE_MINI_01",
            "A_F_Y_VINEWOOD_04_WHITE_FULL_01",
            "A_F_Y_VINEWOOD_04_WHITE_MINI_01",
            "A_F_Y_VINEWOOD_04_WHITE_MINI_02",
        };
        internal static Dictionary<WeaponAsset, List<string>> WeaponMKIIAndItsComponents = new Dictionary<WeaponAsset, List<string>>()
        {
            {
                0x84D6FAFD, new List<string>()
                {
                    "COMPONENT_BULLPUPRIFLE_MK2_CLIP_02",
                    "COMPONENT_AT_AR_SUPP",
                    "COMPONENT_AT_AR_AFGRIP_02",
                    "COMPONENT_AT_AR_FLSH",
                    "COMPONENT_AT_BP_BARREL_02",
                    "COMPONENT_AT_SCOPE_SMALL_MK2",
                }
            },
            {
                0x969C3D67, new List<string>()
                {
                    "COMPONENT_SPECIALCARBINE_MK2_CLIP_02",
                    "COMPONENT_AT_AR_FLSH",
                    "COMPONENT_AT_SCOPE_MEDIUM_MK2",
                    "COMPONENT_AT_AR_SUPP_02",
                    "COMPONENT_AT_AR_AFGRIP_02",
                    "COMPONENT_AT_SC_BARREL_02"
                }
            },
            {
                0x394F415C, new List<string>()
                {
                    "COMPONENT_ASSAULTRIFLE_MK2_CLIP_02",
                    "COMPONENT_AT_AR_AFGRIP_02",
                    "COMPONENT_AT_AR_FLSH",
                    "COMPONENT_AT_SCOPE_MEDIUM_MK2",
                    "COMPONENT_AT_AR_SUPP_02",
                    "COMPONENT_AT_AR_BARREL_02",
                }
            },
            {
                0xFAD1F1C9, new List<string>()
                {
                    "COMPONENT_CARBINERIFLE_MK2_CLIP_02",
                    "COMPONENT_AT_AR_AFGRIP_02",
                    "COMPONENT_AT_AR_FLSH",
                    "COMPONENT_AT_SCOPE_MEDIUM_MK2",
                    "COMPONENT_AT_AR_SUPP",
                    "COMPONENT_AT_CR_BARREL_02",
                }
            },
            {
                0x555AF99A, new List<string>()
                {
                    "COMPONENT_PUMPSHOTGUN_MK2_CLIP_HOLLOWPOINT",
                    "COMPONENT_AT_SCOPE_SMALL_MK2",
                    "COMPONENT_AT_AR_FLSH",
                    "COMPONENT_AT_SR_SUPP_03",
                }
            },
            {
                0xDBBD7280, new List<string>()//Combat-MG MK2
                {
                    "COMPONENT_COMBATMG_MK2_CLIP_02",
                    "COMPONENT_AT_AR_AFGRIP_02",
                    "COMPONENT_AT_SCOPE_MEDIUM_MK2",
                    "COMPONENT_AT_MUZZLE_07",
                    "COMPONENT_AT_MG_BARREL_02",
                }
            },
        };
    }
}
