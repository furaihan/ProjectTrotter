namespace BarbarianCall
{
    using BarbarianCall.Types;
    using Rage;
    using Rage.Native;
    using System.Collections.Generic;
    using System.Linq;
    using System;

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
            "SULTAN", "NEON", "KURUMA", "BUFFALO", "REVOLTER", "RAIDEN", "ORACLE", "JACKAL", "FELON", "F620", "HUNTLEY", "MESA", "JESTER4", "SULTAN3",
            "HABANERO", "LANDSTALKER", "DYNASTY", "FAGALOA", "TULIP", "DILETTANTE", "NEON", "RAPIDGT", "PEYOTE", "MANANA", "TORNADO", "RUMPO", "YOUGA", "SPEEDO", "VOLTIC", "CYCLONE",
            "TROPOS", "GRANGER", "RADI", "FUGITIVE", "COGNOSCENTI", "INGOT", "SENTINEL", "SANDKING", "DUNE", "FUSILADE", "INTRUDER", "INFERNUS", "BUCCANEER", "BUCCANEER2", "DOMINATOR",
            "DOMINATOR7", "COMET6", "BANSHEE", "CALICO", "GROWLER", "CYPHER", "REMUS", "VECTRE", "EUROS", "ZR350" 
        };

        /// <summary>
        /// Defines the MotorBikesToSelect.
        /// </summary>
        public static Model[] MotorBikesToSelect = { "AKUMA", "HEXER", "BAGGER", "BATI", "PCJ", "NEMESIS", "VINDICATOR", "THRUST", "FAGGIO", "VADER", "HAKUCHOU", "DOUBLE", "BF400",
        "FAGGIO", "LECTRO", "INNOVATION", "MANCHEZ2"};

        public static TimeZoneInfo MyTimeZoneInfo = TimeZoneInfo.CreateCustomTimeZone("Jakarta Standard Time", new TimeSpan(TimeSpan.TicksPerHour * 7), "Waktu Indonesia Barat", "Waktu Indonesia Barat");

        /// <summary>
        /// Defines the DangerousVehicleItems.
        /// </summary>
        public static List<string> DangerousVehicleItems = new()
        {
            "a small cardboard filled with crystal methamphetamine",
            "an automatic rifle",
            "a small bag with 250 grams of cocain inside",
            "a bottle crates with full of molotov cocktail",
            "a USB with several hacking software",
            "a USB that contains a collection of child pornography videos",
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
        /// Defines the AudibleCarModel.
        /// </summary>
        public static Model[] AudibleCarModel;
        public static Dictionary<uint, string> AudioHash = new();

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
        internal static Dictionary<int, List<int>> AtasanCowokPolos = new()
        {
            {351, new List<int>() {0,1,2,3,4} },
            {345, new List<int>() {0,1,2,3,4,5,6,7 } },
            {0, new List<int>() {0,1,2,3,4,5,7,8,11} },
            {1, new List<int>() {0, 1, 3,4,5,6,7,8,11,12,14} },
            {8, new List<int>() {0, 10, 13, 14} },
            {238, new List<int>() {0, 1,2,4,5} },
            {241, new List<int>() {0,1,2,3,4,5} },
            {242, new List<int>() {0,1,2,3,4,5} },
            {22, new List<int>() {0,1,2} },
            {44, new List<int>() {0,1,2,3} },
            {80, new List<int>() {0,1,2} },
            {146, Enumerable.Range(0,8).ToList() },
        };
        internal static Dictionary<int, List<int>> JaketCowok = new()
        {
            {3, Enumerable.Range(0, 15).ToList() },
            {7, Enumerable.Range(0, 15).ToList() },
            {59, Enumerable.Range(0,3).ToList() },
            {355, Enumerable.Range(0, 25).ToList() },
            {347, Enumerable.Range(0, 25).ToList() },
            {346, Enumerable.Range(0, 25).ToList() },
            {70, Enumerable.Range(0, 11).ToList() },
            {88, Enumerable.Range(0, 11).ToList() },
            {151, Enumerable.Range(0, 5).ToList() },
            {240, Enumerable.Range(0, 5).ToList() },
            {388, Enumerable.Range(0, 5).ToList() },
            {376, Enumerable.Range(0, 2).ToList() },
        };
        internal static Dictionary<int, List<int>> UndershirtMale = new()
        {
            {0, new List<int>() {0,1,2,3,4,5,7,8,11 } },
            {1, new List<int>() {0,1,3,4,5,6,7,8,11,12,14,15 } },
            {41, Enumerable.Range(0, 4).ToList() },
        };
        internal static Dictionary<int, List<int>> BawahanCowok = new()
        {
            {7, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14, 15 } },
            {8, new List<int>() {0, 3,4, 14 } },
            {10, new List<int>() {0,1,2 } },
            {9, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15} },
            {15, Enumerable.Range(0,15).ToList() },
            {23, Enumerable.Range(0, 12).ToList() },
            {105, Enumerable.Range(0, 11).ToList() }
        };
        internal static Dictionary<int, List<int>> KacamataCowok = new()
        {
            {1, new List<int>() {0,1,2,3,4,5,6,7} },
            {7, new List<int>() {0,1,2,3,4,5,6,7,8,9} },
            {15, new List<int>() {0,1,2,3,4,5,6,7,8,9} },
            {17, new List<int>() {0,1,2,3,4,5,6,7,8,9} },
        };
        internal static Dictionary<int, List<int>> TopiCowok = new()
        {
            {7, new List<int>() {0,1,2,3,4,5,6,7} },
            {142, Enumerable.Range(0, 25).ToList() },
            {143, Enumerable.Range(0, 25).ToList() },
            {151, Enumerable.Range(0, 7).ToList() },
            {152, Enumerable.Range(0, 7).ToList() },
            {15, Enumerable.Range(0, 7).ToList() },
            {94, Enumerable.Range(0, 9).ToList() },
            {95, Enumerable.Range(0, 9).ToList() },
        };
        internal static Dictionary<int, List<int>> AlasKaki = new()
        {
            {1, Enumerable.Range(0,14).ToList() },
            {3, Enumerable.Range(0,15).ToList() },
            {5, new List<int>() {0,1,2,3 } },
            {16, Enumerable.Range(0,11).ToList() },
            {40, Enumerable.Range(0,11).ToList() }
        };
        private static List<Tuple<string, string>> _decalBadge;
        private static List<Tuple<string, string>> _decalBadgeFemale;
        internal static List<Tuple<string, string>> DecalBadge => _decalBadge ??= DivisiXml.Deserialization.GetBadgeFromXml();
        internal static List<Tuple<string, string>> DecalBadgeFemale => _decalBadgeFemale ??= DivisiXml.Deserialization.GetBadgeFromXml("female");
        /// <summary>
        /// Defines the RegisteredPedHeadshot.
        /// </summary>
        public static List<uint?> RegisteredPedHeadshot = new();

        /// <summary>
        /// Defines the Normal.
        /// </summary>
        public const VehicleDrivingFlags Normal = (VehicleDrivingFlags)786603;
        public const VehicleDrivingFlags AvoidTraffic = (VehicleDrivingFlags)786468;
        public const VehicleDrivingFlags IgnoreLights = (VehicleDrivingFlags)2883621;
        public const VehicleDrivingFlags AvoidTrafficExtremely = (VehicleDrivingFlags)6;
        public const VehicleDrivingFlags Rushed = (VehicleDrivingFlags)1074528293;
        internal const VehicleDrivingFlags Rushed2 = VehicleDrivingFlags.DriveAroundPeds | VehicleDrivingFlags.DriveAroundVehicles | VehicleDrivingFlags.DriveAroundObjects | VehicleDrivingFlags.AllowWrongWay | VehicleDrivingFlags.AllowMedianCrossing;
        internal const VehicleDrivingFlags Sheeesh = (VehicleDrivingFlags)1107573356;

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
        /// Gets the game timer
        /// </summary>
        internal static int GameTimer => NativeFunction.Natives.GET_GAME_TIMER<int>();

        /// <summary>
        /// Defines the MaleVoiceName.
        /// </summary>
        public static List<string> MaleVoiceName = new()
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
        public static List<string> FemaleVoiceName = new()
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
        internal static Dictionary<WeaponAsset, List<string>> WeaponMKIIAndItsComponents = new()
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
