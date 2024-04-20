using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Rage;
using N = Rage.Native.NativeFunction;
using System.Windows.Forms;
using RAGENativeUI;
using System.Drawing;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using ProjectTrotter.Types;
using System.Reflection;
using ProjectTrotter.Extensions;

namespace ProjectTrotter
{
    internal static class GenericUtils
    {
        public static Random Random = new(
            N.Natives.xF2D49816A804D134<int>(1000, 90080) ^ N.Natives.xD53343AA4FB7DD28<int>(2500, 91800)
            );
        public static System.Globalization.CultureInfo CultureInfo = System.Globalization.CultureInfo.CurrentCulture;
        private static Ped PlayerPed => Game.LocalPlayer.Character;
        internal static Spawnpoint SelectNearbySpawnpoint(List<Spawnpoint> spawnPoints, float maxDistance = 800f, float minDistance = 300f)
        {
            float max = maxDistance * maxDistance;
            float min = minDistance * minDistance;
            try
            {
                Logger.Log("Calculating the best location for callout");
                List<Spawnpoint> suitable = spawnPoints.Where(sp => Vector3.DistanceSquared(sp,PlayerPed)  < max && Vector3.DistanceSquared(sp, PlayerPed) > min
                 && sp.Position.HeightDiff(Game.LocalPlayer.Character) < 35f).ToList();
                if (suitable.Count > 0)
                {
                    Logger.Log($"Found {suitable.Count} suitable location, choosing a random location from that list");
                    Spawnpoint selected = suitable.GetRandomElement(true);
                    Logger.Log(string.Format("Location selected is {0} in {1}", selected, GetZoneName(selected.Position)));
                    return selected;
                }
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("We have problem when selecting a spawnpoint | {0}", e.Message));
                Logger.Log(e.ToString());
                NetExtension.SendError(e);
            }         
            return Spawnpoint.Zero;
        }

        internal static string GetRandomUnitNumber()
        {
            // List of unit names
            List<string> unitNames = new List<string>()
            {
                "adam", "boy", "charles", "david", "edward", "frank", "george", "henry", "ida", "john", "king",
                "lincoln", "mary", "noah", "ocean", "paul", "queen", "robert", "sam", "tom", "union", "victor",
                "william", "xray", "young", "zebra"
            };

            // Generate a random unit number
            int randomNumber1 = Random.Next(1, 11);
            string randomUnitName = unitNames.GetRandomElement();
            int randomNumber2 = Random.Next(1, 25);

            return $"{randomNumber1}-{randomUnitName}-{randomNumber2}";
        }

        internal static string GetLicensePlateAudio(Vehicle veh) => GetLicensePlateAudio(veh.LicensePlate);
        internal static string GetLicensePlateAudio(string licensePlate)
        {
            int count = 0;

            StringBuilder lpAudio = new(56, 100);
            foreach (char c in licensePlate.ToUpper())
            {
                count++;
                lpAudio.Append($"BAR_{c}{(count == 1 ? "_HIGH" : count == licensePlate.Length ? "_LOW" : string.Empty)}");             
                lpAudio.Append(' ');
            }
            return lpAudio.ToString();
        }
        internal static string GetColorAudio(this Vehicle vehicle)
        {
            return vehicle.GetColor().PrimaryColor.GetPoliceScannerColorAudio();
        }
        internal static string GetRandomPlateNumber()
        {
            StringBuilder plate = new(8);
            plate.Append(MyRandom.Next(10));
            plate.Append(MyRandom.Next(10));
            plate.Append((char)MyRandom.Next(65, 91));
            plate.Append((char)MyRandom.Next(65, 91));
            plate.Append((char)MyRandom.Next(65, 91));
            plate.Append(MyRandom.Next(10));
            plate.Append(MyRandom.Next(10));
            plate.Append(MyRandom.Next(10));
            return plate.ToString();
        }
        internal static void RandomizeLicensePlate(this Vehicle vehicle)
        {
            if (vehicle)
            {
                string plate = GetRandomPlateNumber();
                Logger.Log(string.Format("Set {0} license plate to {1}", vehicle.GetDisplayName(), plate));
            }           
        }
        internal static string GetLabelText(string gxtEntry) => !string.IsNullOrEmpty(gxtEntry) && N.Natives.DOES_TEXT_LABEL_EXIST<bool>(gxtEntry) ?
            N.Natives.GET_FILENAME_FOR_AUDIO_CONVERSATION<string>(gxtEntry) : string.Empty;
        internal static string GetDisplayName(this Model vehicleModel)
        {
            string text = N.Natives.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL<string>(vehicleModel.Hash);
            return !string.IsNullOrEmpty(text) && N.Natives.DOES_TEXT_LABEL_EXIST<bool>(text) ?
                N.Natives.GET_FILENAME_FOR_AUDIO_CONVERSATION<string>(text) : "CARNOTFOUND";
        }
        internal static string GetDisplayName(this Vehicle vehicle) => GetDisplayName(vehicle.Model);
        internal static string GetMakeName(this Model vehicleModel, string emptyReturn = "Unknown Manufacturer")
        {
            string text = N.Natives.GET_MAKE_NAME_FROM_VEHICLE_MODEL<string>(vehicleModel.Hash);
            return !string.IsNullOrEmpty(text) && N.Natives.DOES_TEXT_LABEL_EXIST<bool>(text) ? 
                N.Natives.GET_FILENAME_FOR_AUDIO_CONVERSATION<string>(text) : emptyReturn;
        }
        internal static string GetMakeName(this Vehicle vehicle, string emptyReturn = "Unknown Manufacturer") => GetMakeName(vehicle.Model, emptyReturn);
        internal static string GetPoliceScannerAudio(Vehicle vehicle)
        {
            string makeName = GetMakeName(vehicle);
            if (Globals.AudioHash.ContainsKey(vehicle.Model.Hash))
            {
                return makeName == "Unknown Manufacturer" ? Globals.AudioHash[vehicle.Model.Hash].ToUpper() : $"MANUFACTURER_{makeName.ToUpper()} {Globals.AudioHash[vehicle.Model.Hash].ToUpper()}";
            }
            string vehClass = vehicle.Class switch
            {
                VehicleClass.Sport => "VEHICLE_CATEGORY_SPORTS_CAR",
                VehicleClass.SportClassic => "VEHICLE_CATEGORY_SPORTS_CAR",
                VehicleClass.Super => "VEHICLE_CATEGORY_PERFORMANCE_CAR",
                VehicleClass.Boat => "VEHICLE_CATEGORY_BOAT",
                VehicleClass.Commercial => "VEHICLE_CATEGORY_TRUCK",
                VehicleClass.Coupe => "VEHICLE_CATEGORY_COUPE",
                VehicleClass.Cycle => "VEHICLE_CATEGORY_BICYCLE",
                VehicleClass.Emergency => "VEHICLE_CATEGORY_POLICE_CAR",
                VehicleClass.Helicopter => "VEHICLE_CATEGORY_HELICOPTER",
                VehicleClass.Industrial => "VEHICLE_CATEGORY_INDUSTRIAL_VEHICLE",
                VehicleClass.Military => "VEHICLE_CATEGORY_MILITARY_VEHICLE",
                VehicleClass.Motorcycle => "VEHICLE_CATEGORY_MOTORCYCLE",
                VehicleClass.Muscle => "VEHICLE_CATEGORY_MUSCLE_CAR",
                VehicleClass.OffRoad => "VEHICLE_CATEGORY_OFF_ROAD_VEHICLE",
                VehicleClass.Sedan => "VEHICLE_CATEGORY_SEDAN",
                VehicleClass.Service => "VEHICLE_CATEGORY_SERVICE_VEHICLE",
                VehicleClass.SUV => "VEHICLE_CATEGORY_SUV",
                VehicleClass.Utility => "VEHICLE_CATEGORY_UTILITY_VEHICLE",
                VehicleClass.Van => "VEHICLE_CATEGORY_VAN",
                _ => ""
            };
            return makeName == "Unknown Manufacturer" ? vehClass : $"{makeName} {vehClass}";
        }
        internal static string GetZoneName(this ISpatial spatial) => GetZoneName(spatial.Position);
        internal static string GetZoneName(this Vector3 pos)
        {
            string gameName = N.Natives.GET_NAME_OF_ZONE<string>(pos.X, pos.Y, pos.Z);
            return Game.GetLocalizedString(gameName);
        }
        internal static string GetZoneNameLSPDFR(this ISpatial spatial) => GetZoneNameLSPDFR(spatial.Position);
        internal static string GetZoneNameLSPDFR(this Vector3 pos)
        {
            var zone = Functions.GetZoneAtPosition(pos);
            return zone.RealAreaName;
        }
        public static string GetCardinalDirectionLowDetailedAudio(this Entity e)
        {
            float degrees = e.Heading;
            string[] cardinals = { "DIRECTION_BOUND_NORTH", "DIRECTION_BOUND_WEST", "DIRECTION_BOUND_SOUTH", "DIRECTION_BOUND_EAST", "DIRECTION_BOUND_NORTH" };
            return cardinals[(int)Math.Round((double)degrees % 360 / 90)];
        }
        internal static bool Speaking;
        internal static void HandleSpeech(List<string> Dialogue, params Ped[] talkers)
        {
            Logger.Log("Speech Started");
            Speaking = true;
            Ped playerPed = Game.LocalPlayer.Character;
            Vector3 playerPos = Game.LocalPlayer.Character.Position;
            float playerHead = Game.LocalPlayer.Character.Heading;
            IDictionary<string, string> valuePairs = new Dictionary<string, string>()
            {
                {"Officer:", "~b~Officer~s~:" },
                {"Commander:", "~b~Commander~s~:" },
                {"Dispatch:", "~b~Dispatch~s~:" },
                {"Witness:", "~o~Witness~s~:" },
                {"Suspect:", "~r~Suspect~s~:" },
                {"Paramedic:", "~g~Paramedic~s~:" },
                {"Medic:", "~g~Medic~s~:" },
                {"EMS Officer:", "~g~EMS Officer~s~:" },
                {"Victim:", "~o~Victim~s~:" },
                {"Hostage:", "~o~Hostage~s~:" },
                {"Citizen:", "~o~Citizen~s~:" },
            };
            List<string> modifiedDialogue = new();
            Dialogue.ForEach(cakap =>
            {
                string modified = cakap;
                if (valuePairs.Any(st => modified.StartsWith(st.Key)))
                {
                    string key = valuePairs.Keys.Where(s => modified.StartsWith(s)).FirstOrDefault();
                    if (valuePairs.TryGetValue(key, out string val)) modified = modified.Replace(key, val);
                    else $"Try get value error {key}".ToLog();
                    //Game.Console.Print("IDict: " + modified);
                }
                modifiedDialogue.Add(modified);
            });
            (modifiedDialogue.Count == Dialogue.Count).ToString().ToLog();
            WeaponDescriptor currentWeapon = playerPed.Inventory.EquippedWeapon;
            playerPed.Inventory.GiveNewWeapon("WEAPON_UNARMED", -1, true);
            N.Natives.SET_PED_CAN_SWITCH_WEAPON(playerPed, false);
            GameFiber.StartNew(delegate
            {
                while (Speaking)
                {
                    GameFiber.Yield();
                    if (Vector3.DistanceSquared(playerPed.Position, playerPos) > 6.25f)
                    {
                        playerPed.Tasks.FollowNavigationMeshToPosition(playerPos, playerHead, 1.5f).WaitForCompletion(1000);
                    }
                    if (playerPed.IsInAnyVehicle(false))
                    {
                        playerPed.Tasks.LeaveVehicle(LeaveVehicleFlags.None).WaitForCompletion(1800);
                    }
                }
            }, "[BarbarianCall] Player Position Handler Fiber");
            if (talkers.All(p => p && p.IsInAnyVehicle(false)))
            {
                talkers.ToList().ForEach(p => p.Tasks.AchieveHeading(p.GetHeadingTowards(playerPed)));
                GameFiber.Wait(75);
                talkers.ToList().ForEach(p => p.Tasks.PlayAnimation("special_ped@jessie@monologue_1@monologue_1f", "jessie_ig_1_p1_heydudes555_773", 4f, AnimationFlags.Loop | AnimationFlags.SecondaryTask));
            }
            for (int i = 0; i < modifiedDialogue.Count; i++)
            {
                while (Speaking)
                {
                    GameFiber.Yield();
                    if (Game.IsKeyDown(Keys.Y)) break;
                }
                talkers.ToList().ForEach(p => p.Tasks.AchieveHeading(p.GetHeadingTowards(playerPed)));
                Game.DisplaySubtitle(modifiedDialogue[i], 10000);
                if (!Speaking) break;
            }
            Speaking = false;
            N.Natives.SET_PED_CAN_SWITCH_WEAPON(playerPed, true);
            if (currentWeapon != null && playerPed.Inventory.Weapons.Contains(currentWeapon)) playerPed.Inventory.EquippedWeapon = currentWeapon;
            talkers.Where(EntityExtensions.Exists).ToList().ForEach(p => p.Tasks.Clear());
        }
        private static Rage.Object MobilePhone;
        internal static void ToggleMobilePhone(this Ped ped)
        {
            if (MobilePhone) MobilePhone.Delete();
            var currentWeapon = ped.Inventory.EquippedWeapon;
            N.Natives.SET_PED_CAN_SWITCH_WEAPON(ped, false);
            ped.Inventory.GiveNewWeapon(new WeaponAsset("WEAPON_UNARMED"), -1, true);
            MobilePhone = new Rage.Object(new Model("prop_police_phone"), new Vector3(0, 0, 0));
            int boneIndex = ped.GetBoneIndex(PedBoneId.RightPhHand);
            if (MobilePhone) MobilePhone.AttachTo(ped, boneIndex, new Vector3(0f, 0f, 0f), new Rotator(0f, 0f, 0f));
            ped.Tasks.PlayAnimation(new AnimationDictionary("cellphone@"), "cellphone_call_listen_base", 1.45f, AnimationFlags.UpperBodyOnly | AnimationFlags.SecondaryTask | AnimationFlags.StayInEndFrame);
            GameFiber.Sleep(3000);
            ped.Tasks.PlayAnimation(new AnimationDictionary("cellphone@"), "cellphone_call_out", 1f, AnimationFlags.UpperBodyOnly | AnimationFlags.SecondaryTask);
            GameFiber.Sleep(200);
            N.Natives.SET_PED_CAN_SWITCH_WEAPON(ped, true);
            if (currentWeapon != null) ped.Inventory.EquippedWeapon = currentWeapon;
            if (MobilePhone) MobilePhone.Detach();
            if (MobilePhone) MobilePhone.Delete();
        }
        internal static void MakeMissionPed(this Ped ped, bool invincible = false)
        {
            ped.MakePersistent();
            ped.BlockPermanentEvents = true;
            ped.Money = 1;
            ped.Health = 500;
            ped.MaxHealth = 500;
            ped.Armor = 50;
            ped.Opacity = 1.0f;
            ped.Metadata.BAR_Entity = ped.IsVisible = true;
            ped.IsInvincible = invincible;
            //$"Set {ped.Model.Name} as mission ped. {ped.Health} - {ped.MaxHealth} - {ped.FatalInjuryHealthThreshold}".ToLog();
        }             
        internal static uint DisplayNotifWithLogo(this string msg, string subtitle = "", string textureDict = "WEB_LOSSANTOSPOLICEDEPT", string textureName = "WEB_LOSSANTOSPOLICEDEPT", string title = "~y~BarbarianCall~s~", 
            bool fadeIn = false, bool blink = false, HudColor? hudColor = null, NotificationIcon icon = NotificationIcon.Nothing)
        {
            Stopwatch sw = Stopwatch.StartNew();
            while (!N.Natives.HAS_STREAMED_TEXTURE_DICT_LOADED<bool>(textureDict))
            {
                N.Natives.REQUEST_STREAMED_TEXTURE_DICT(textureDict, false);
                GameFiber.Yield();
                if (sw.ElapsedMilliseconds > 1000) break;
            }
            if (hudColor.HasValue) N.Natives.THEFEED_SET_BACKGROUND_COLOR_FOR_NEXT_POST((int)hudColor.Value);
            N.Natives.BEGIN_TEXT_COMMAND_THEFEED_POST("CELL_EMAIL_BCON");
            AddLongString(msg);
            N.Natives.END_TEXT_COMMAND_THEFEED_POST_MESSAGETEXT<uint>(textureDict, textureName, fadeIn, (int)icon, title, subtitle);
            return N.Natives.END_TEXT_COMMAND_THEFEED_POST_TICKER<uint>(blink, true);
        }
        public static void DisplayHelpTextWithGXTEntriesThisFrame(params string[] gxtEntries)
        {
            N.Natives.BEGIN_TEXT_COMMAND_DISPLAY_HELP(gxtEntries[0]);
            if (gxtEntries.Length > 1)
            {
                for (int i = 1; i < gxtEntries.Length; i++)
                {
                    N.Natives.ADD_TEXT_COMPONENT_SUBSTRING_TEXT_LABEL(gxtEntries[i]);
                }
            }
            N.Natives.END_TEXT_COMMAND_DISPLAY_HELP(0, 0, 1, -1);
;        }
        public static void AddLongString(string str)
        {
            const int strLen = 99;
            for (int i = 0; i < str.Length; i += strLen)
            {
                string substr = str.Substring(i, Math.Min(strLen, str.Length - i));
                N.Natives.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME(substr);
            }
        }
        internal static string FormatKeyBinding(Keys modifierKey, Keys key)
            => modifierKey == Keys.None ? $"~{key.GetInstructionalId()}~" :
                                          $"~{modifierKey.GetInstructionalId()}~ ~+~ ~{key.GetInstructionalId()}~";
        internal static bool CheckKey(Keys modifierKey, Keys key)
        {
            bool keyboardInputCheck = N.Natives.UPDATE_ONSCREEN_KEYBOARD<int>() == 0;
            if (!keyboardInputCheck)
            {
                if (Game.IsKeyDown(key) && modifierKey == Keys.None) return true;
                if (Game.IsKeyDownRightNow(modifierKey) && Game.IsKeyDown(key)) return true;
            }
            return false;
        }
        public static float GetHeadingTowards(this ISpatial spatial, ISpatial towards)
        {
            return GetHeadingTowards(spatial, towards.Position);
        }
        public static float GetHeadingTowards(this ISpatial spatial, Vector3 towardsPosition)
        {
            return GetHeadingTowards(spatial.Position, towardsPosition);
        }
        public static float GetHeadingTowards(this Vector3 position, Vector3 towardsPosition)
        {
            Vector3 directionFromEntityToPosition = towardsPosition - position;
            directionFromEntityToPosition.Normalize();

            float heading = MathHelper.ConvertDirectionToHeading(directionFromEntityToPosition);
            return heading;
        }
        public static float GetHeadingTowards(this Vector3 position, ISpatial towards)
        {
            return GetHeadingTowards(position, towards.Position);
        }
        internal static void DisplayNotificationsWithPedHeadshot(this Ped ped, string subtitle, string text, string title = "~y~Barbarian Call")
        {
            GameFiber.StartNew(() =>
            {
                try
                {
                    "Attempting to register ped headshot".ToLog();
                    ped.GetVariation(1, out int draw, out int tex);
                    ped.SetVariation(1, 0, 0);
                    uint headshotHandle = N.Natives.REGISTER_PEDHEADSHOT_HIRES<uint>(ped);
                    var timer = new TimeSpan(0, 0, 10);
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    while (true)
                    {
                        GameFiber.Yield();
                        if (N.Natives.x7085228842B13A67<bool>(headshotHandle))
                        {
                            $"Ped Headshot found with handle {headshotHandle}".ToLog();
                            break;
                        }
                        if (stopwatch.Elapsed > timer) break;
                    }
                    string txd = N.Natives.GET_PEDHEADSHOT_TXD_STRING<string>(headshotHandle);
                    ped.SetVariation(1, draw, tex);
                    Game.DisplayNotification(txd, txd, title, subtitle, text);
                    //GameFiber.Wait(200);
                    Globals.RegisteredPedHeadshot.Add(headshotHandle);
                    N.Natives.UNREGISTER_PEDHEADSHOT<uint>(headshotHandle);
                    $"Register ped headshot is took {stopwatch.ElapsedMilliseconds} ms".ToLog();
                }
                catch (Exception e)
                {
                    DisplayNotifWithLogo(text, subtitle, "srange_gen", "blanktrophy_gold");
                    "Display notification with mugshot error".ToLog();
                    e.ToString().ToLog();
                    e.Message.ToLog();
                }
            });
        }
        internal static string GetPedHeadshotTexture(this Ped ped, out uint? Handle, string failedReturn = "WEB_LOSSANTOSPOLICEDEPT")
        {
            Handle = null;
            try
            {
                if (!ped) throw new Rage.Exceptions.InvalidHandleableException(ped);
                uint headshotHandle = N.Natives.REGISTER_PEDHEADSHOT<uint>(ped);
                int startTime = Environment.TickCount;
                Stopwatch sw = Stopwatch.StartNew();
                while (true)
                {
                    GameFiber.Yield();
                    if (N.Natives.IS_PEDHEADSHOT_READY<bool>(headshotHandle))
                    {
                        $"Ped Headshot found with handle {headshotHandle}, took {sw.ElapsedMilliseconds} ms".ToLog();
                        break;
                    }
                    if (sw.ElapsedMilliseconds > TimeSpan.FromSeconds(5).TotalMilliseconds)
                    {
                        Logger.Log(string.Format("failed to get headshot because of timeout. {0}, {1}", Functions.GetPersonaForPed(ped).FullName, ped.Model.Name));
                        return failedReturn;
                    }
                }
                string txd = N.Natives.GetPedheadshotTxdString<string>(headshotHandle);
                Handle = headshotHandle;
                Globals.RegisteredPedHeadshot.Add(headshotHandle);
                return txd;
            }
            catch (Exception e)
            {
                "Get ped headshot failed".ToLog();
                e.ToString().ToLog();
            }
            return failedReturn;
        }
        internal static void UnregisterPedHeadshot(this uint? handle)
        {
            if (handle.HasValue)
            {
                if (N.Natives.IS_PEDHEADSHOT_VALID<bool>(handle.Value)) N.Natives.UNREGISTER_PEDHEADSHOT<uint>(handle.Value);
                else Logger.Log($"headshot with handle {handle.Value} is invalid");
            }
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                int k = MyRandom.Next(n--);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        public static T GetRandomElement<T>(this IList<T> list, bool shuffle = false)
        {
            if (list == null || list.Count <= 0)
                return default;

            if (shuffle) list.Shuffle();
            return list[MyRandom.Next(list.Count)];
        }

        public static T GetRandomElement<T>(this IEnumerable<T> enumarable, bool shuffle = false)
        {
            if (enumarable == null || enumarable.Count() <= 0)
                return default;

            T[] array = enumarable.ToArray();
            return GetRandomElement(array, shuffle);
        }

        public static T GetRandomElement<T>(this Enum items)
        {
            if (typeof(T).BaseType != typeof(Enum))
                throw new InvalidCastException();
            Array types = Enum.GetValues(typeof(T));
            return GetRandomElement(types.Cast<T>());
        }
        public static T GetRandomElement<T>(this IEnumerable<T> items, Predicate<T> predicate, bool shuffle = false)
        {
            List<T> sorted = items.ToList().FindAll(predicate);
            return sorted.GetRandomElement(shuffle);
        }

        public static IList<T> GetRandomNumberOfElements<T>(this IList<T> list, int numOfElements, bool shuffle = false)
        {
            List<T> givenList = new(list);
            List<T> l = new();
            for (int i = 0; i < numOfElements; i++)
            {
                T t = givenList.GetRandomElement(shuffle);
                givenList.Remove(t);
                l.Add(t);
            }
            return l;
        }
        public static IEnumerable<T> GetRandomNumberOfElements<T>(this IEnumerable<T> enumarable, int numOfElements, bool shuffle = false)
        {
            List<T> givenList = new(enumarable);
            List<T> l = new();
            for (int i = 0; i < numOfElements; i++)
            {
                T t = givenList.Except(l).GetRandomElement(shuffle);
                l.Add(t);
            }
            return l;
        }
        internal static string AddSpacesToSentence(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";
            StringBuilder newText = new(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                    newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }
        internal static void SetPedAsWanted(this Ped ped, out Persona newPersona)
        {
            Persona pedPersona = Functions.GetPersonaForPed(ped);
            Persona newWantedPersona = new(pedPersona.Forename, pedPersona.Surname, pedPersona.Gender, pedPersona.Birthday)
            {
                Wanted = true,
                Citations = pedPersona.Citations,
                ELicenseState = pedPersona.ELicenseState,
                TimesStopped = pedPersona.TimesStopped,
                RuntimeInfo = pedPersona.RuntimeInfo
            };
            newPersona = newWantedPersona;
            Functions.SetPersonaForPed(ped, newWantedPersona);
            $"Setting ped {ped.Model.Name} {newWantedPersona.FullName} as wanted".ToLog();
            return;
        }
        internal static void SetPedAsWanted(this Ped ped) => SetPedAsWanted(ped, out Persona _);
        [Obsolete]
        internal static string GetCarColor(this Vehicle vehicle)
        {
            if (vehicle)
            {
                try
                {
                    vehicle.Metadata.BAR_Entity = true;
                    PropertyInfo[] cname = typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);
                    List<Color> colour = cname.Select(c => Color.FromKnownColor((KnownColor)Enum.Parse(typeof(KnownColor), c.Name))).ToList();
                    List<int> cint = colour.Select(c => c.ToArgb()).ToList();
                    if (cint.Contains(vehicle.PrimaryColor.ToArgb()))
                    {
                        return cname[cint.IndexOf(vehicle.PrimaryColor.ToArgb())].Name.AddSpacesToSentence();
                    }
                }
                catch (Exception e)
                {
                    "Get car color error".ToLog();
                    e.ToString().ToLog();
                }
                $"{vehicle.GetDisplayName()} color is unknown, Argb: {vehicle.PrimaryColor.ToArgb()}".ToLog();
                return "Weirdly colored";
            }
            return string.Empty;            
        }
    }
}
