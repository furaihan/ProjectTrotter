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
using BarbarianCall.Types;
using System.Reflection;
using BarbarianCall.Extensions;

namespace BarbarianCall
{
    internal static class Peralatan
    {
        public static Random Random = new(N.Natives.xF2D49816A804D134<int>(1000, 90080));
        public static System.Globalization.CultureInfo CultureInfo = System.Globalization.CultureInfo.CurrentCulture;
        private static Ped PlayerPed => Game.LocalPlayer.Character;

        internal static Spawnpoint SelectNearbySpawnpoint(List<Spawnpoint> spawnPoints, float maxDistance = 800f, float minDistance = 300f)
        {
            try
            {
                ToLog("Calculating the best location for callout");
                List<Spawnpoint> suitable = spawnPoints.Where(sp => Vector3.DistanceSquared(sp,PlayerPed)  < maxDistance && Vector3.DistanceSquared(sp, PlayerPed) > minDistance
                && sp.Position.TravelDistanceTo(Game.LocalPlayer.Character) < maxDistance * 2 && sp.Position.HeightDiff(Game.LocalPlayer.Character) < 35f).ToList();
                if (suitable.Count > 0)
                {
                    ToLog($"Found {suitable.Count} suitable location, choosing a random location from that list");
                    Spawnpoint selected = suitable.GetRandomElement(true);
                    ToLog(string.Format("Location selected is {0} in {1}", selected, GetZoneName(selected.Position)));
                    return selected;
                }
            }
            catch (Exception e)
            {
                ToLog(string.Format("We have problem when selecting a spawnpoint | {0}", e.Message));
                ToLog(e.ToString());
                NetExtension.SendError(e);
            }         
            return Spawnpoint.Zero;
        }
        internal static void Print(this string msg) => Game.Console.Print(msg);
        internal static void ToLog(this string micin) => ToLog(micin, false);
        internal static void ToLog(this string micin, bool makeUppercase)
        {
            string text = makeUppercase ? micin.ToUpper() : micin;
            Game.LogTrivial(makeUppercase ? "[BARBARIAN-CALL]: " + text : "[BarbarianCall]: " + text);
        }
        internal static string GetLicensePlateAudio(Vehicle veh) => GetLicensePlateAudio(veh.LicensePlate);
        internal static string GetLicensePlateAudio(string licensePlate)
        {
            int count = 0;

            StringBuilder lpAudio = new(56, 100);
            foreach (char c in licensePlate)
            {
                count++;
                if (count == 1) lpAudio.Append("BAR_" + c.ToString().ToUpper() + "_HIGH ");
                else if (count == licensePlate.Length) lpAudio.Append("BAR_" + c.ToString().ToUpper() + "_LOW");
                else lpAudio.Append("BAR_" + c.ToString().ToUpper() + " ");
            }
            //Game.Console.Print(audio);
            return lpAudio.ToString();
        }
        internal static string GetColorAudio(this Vehicle vehicle) => GetColorAudio(vehicle.PrimaryColor);
        internal static string GetColorAudio(Color color)
        {
            ToLog("Trying to get color audio");
            List<int> audibleArgb = (from x in Globals.AudibleColor select x.ToArgb()).ToList();
            Color selected;
            if (audibleArgb.Contains(color.ToArgb()))
            {
                ToLog("Color match, converting to scanner audio");
                selected = Globals.AudibleColor[audibleArgb.ToList().IndexOf(color.ToArgb())];
                string ret = "COLOR_" + selected.Name.AddSpacesToSentence().Replace(" ", "_").ToUpper();
#if DEBUG
                ret.Print();
#endif
                return ret;
            }
            ToLog($"Color not match {color.ToArgb()}");
            return string.Empty;
        }
        internal static void RandomiseLicensePlate(this Vehicle vehicle)
        {
            System.Security.Cryptography.RNGCryptoServiceProvider provider = new();
            byte[] box = new byte[4];
            provider.GetBytes(box);
            Random plateRandomizerHandler = new(BitConverter.ToInt32(box, 0));
            if (vehicle)
            {
                string plate =  plateRandomizerHandler.Next(10).ToString() +
                                plateRandomizerHandler.Next(10).ToString() +
                                (char)plateRandomizerHandler.Next(65, 91)  +
                                (char)plateRandomizerHandler.Next(65, 91)  +
                                (char)plateRandomizerHandler.Next(65, 91)  +
                                plateRandomizerHandler.Next(10).ToString() +
                                plateRandomizerHandler.Next(10).ToString() +
                                plateRandomizerHandler.Next(10).ToString();
                vehicle.LicensePlate = plate;
#if DEBUG
                ToLog(string.Format("Set {0} license plate to {1}", vehicle.GetDisplayName(), vehicle.LicensePlate));
#endif
            }           
        }
        internal static string GetDisplayName(this Model vehicleModel)
        {
            string text = N.Natives.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL<string>(vehicleModel.Hash);
            return !string.IsNullOrEmpty(text) && N.Natives.DOES_TEXT_LABEL_EXIST<bool>(text) ? N.Natives.x7B5280EBA9840C72<string>(text) : "CARNOTFOUND";
        }
        internal static string GetDisplayName(this Vehicle vehicle) => GetDisplayName(vehicle.Model);
        internal static string GetMakeName(this Model vehicleModel)
        {
            string text = N.Natives.xF7AF4F159FF99F97<string>(vehicleModel.Hash);
            return !string.IsNullOrEmpty(text) && N.Natives.DOES_TEXT_LABEL_EXIST<bool>(text) ? N.Natives.x7B5280EBA9840C72<string>(text) : "Unknown Manufacturer";
        }
        internal static string GetMakeName(this Vehicle vehicle) => GetMakeName(vehicle.Model);
        internal static string GetPoliceScannerAudio(Vehicle vehicle)
        {
            string makeName = GetMakeName(vehicle);
            string modelName = vehicle.Model.Name;
            var audibles = Globals.AudibleCarModel.Select(m => m.Name);
            modelName.Print();
            if (!audibles.Any(st => st.Equals(modelName, StringComparison.OrdinalIgnoreCase)) && char.IsDigit(modelName.Last()) && !modelName.StartsWith("0X", StringComparison.OrdinalIgnoreCase))
                modelName = modelName.Remove(modelName.Length - 1);
            modelName.Print();
            return makeName == "Unknown Manufacturer" ? vehicle.Model.Name.ToUpper() : $"MANUFACTURER_{makeName.ToUpper()} {modelName.ToUpper()}";
        }
        internal static string GetZoneName(this ISpatial spatial) => GetZoneName(spatial.Position);
        internal static string GetZoneName(this Vector3 pos)
        {
            string gameName = N.Natives.GET_NAME_OF_ZONE<string>(pos.X, pos.Y, pos.Z);
            return Game.GetLocalizedString(gameName);
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
            ToLog("Speech Started");
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
                    if (Vector3.Distance(playerPed.Position, playerPos) > 2.5f)
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
        private static string[] StringToNativeString(string str)
        {
            int stringsNeeded = (str.Length % 99 == 0) ? (str.Length / 99) : ((str.Length / 99) + 1);

            string[] outputString = new string[stringsNeeded];
            for (int i = 0; i < stringsNeeded; i++)
            {
                outputString[i] = str.Substring(i * 99, MathHelper.Clamp(str.Substring(i * 99).Length, 0, 99));
            }
            return outputString;
        }
        internal static uint DisplayNotifWithLogo(this string msg, string subtitle = "", string textureDict = "WEB_LOSSANTOSPOLICEDEPT", string textureName = "WEB_LOSSANTOSPOLICEDEPT", string title = "~y~BarbarianCall~s~", 
            bool fadeIn = false, bool blink = false, HudColor? hudColor = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            while (!N.Natives.HAS_STREAMED_TEXTURE_DICT_LOADED<bool>(textureDict))
            {
                N.Natives.REQUEST_STREAMED_TEXTURE_DICT(textureDict, false);
                GameFiber.Yield();
                if (sw.ElapsedMilliseconds > 1000) break;
            }
            var ss = StringToNativeString(msg);
            if (hudColor.HasValue) N.Natives.x92F0DA1E27DB96DC((int)hudColor.Value); //_THEFEED_SET_NEXT_POST_BACKGROUND_COLOR
            N.Natives.BEGIN_TEXT_COMMAND_THEFEED_POST("CELL_EMAIL_BCON");
            foreach (string st in ss) N.Natives.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME(st);
            N.Natives.END_TEXT_COMMAND_THEFEED_POST_MESSAGETEXT<uint>(textureDict, textureName, fadeIn, 4, title, subtitle);
            return N.Natives.END_TEXT_COMMAND_THEFEED_POST_TICKER<uint>(blink, true);
        }
        internal static string FormatKeyBinding(Keys modifierKey, Keys key)
            => modifierKey == Keys.None ? $"~{key.GetInstructionalId()}~" :
                                          $"~{modifierKey.GetInstructionalId()}~ ~+~ ~{key.GetInstructionalId()}~";
        internal static bool CheckKey(Keys modifierKey, Keys key)
        {
            bool keyboardInputCheck = N.Natives.x0CF2B696BBF945AE<int>() == 0;
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
                    uint headshotHandle = N.Natives.RegisterPedheadshot<uint>(ped);
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
                    string txd = N.Natives.GetPedheadshotTxdString<string>(headshotHandle);
                    Game.DisplayNotification(txd, txd, title, subtitle, text);
                    //GameFiber.Wait(200);
                    Globals.RegisteredPedHeadshot.Add(headshotHandle);
                    N.Natives.UnregisterPedheadshot<uint>(headshotHandle);
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
                uint headshotHandle = N.Natives.RegisterPedheadshot<uint>(ped);
                int startTime = Environment.TickCount;
                Stopwatch sw = Stopwatch.StartNew();
                while (true)
                {
                    GameFiber.Yield();
                    if (N.Natives.x7085228842B13A67<bool>(headshotHandle))
                    {
                        $"Ped Headshot found with handle {headshotHandle}, took {sw.ElapsedMilliseconds} ms".ToLog();
                        break;
                    }
                    if (sw.ElapsedMilliseconds > TimeSpan.FromSeconds(5).TotalMilliseconds)
                    {
                        ToLog(string.Format("failed to get headshot because of timeout. {0}, {1}", Functions.GetPersonaForPed(ped).FullName, ped.Model.Name));
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
                if (N.Natives.IS_PEDHEADSHOT_VALID<bool>(handle.Value)) N.Natives.UnregisterPedheadshot<uint>(handle.Value);
                else ToLog($"headshot with handle {handle.Value} is invalid");
            }
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                int k = Random.Next(n--);
                T temp = list[n];
                list[n] = list[k];
                list[k] = temp;
            }
        }
        public static void ShuffleSecure<T>(this IList<T> list)
        {
            if (list.Count >= byte.MaxValue)
            {
                ToLog("ShuffleSecure is not supported on this list");
                return;
            }
            System.Security.Cryptography.RNGCryptoServiceProvider provider = new();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = box[0] % n;
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static T GetRandomElement<T>(this IList<T> list, bool shuffle = false)
        {
            if (list == null || list.Count <= 0)
                return default;

            if (shuffle) list.Shuffle();
            return list[Random.Next(list.Count)];
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
            List<Model> a = Globals.GangPedModels.Values.GetRandomElement(m => m.All(mm => mm.IsValid), true);
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
