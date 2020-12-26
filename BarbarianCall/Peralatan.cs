using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Rage;
using Rage.Native;
using System.Windows.Forms;
using RAGENativeUI;
using System.Drawing;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;

namespace BarbarianCall
{
    internal static class Peralatan
    {
        public static Random Random = new Random(DateTime.Now.Ticks.GetHashCode());
        public static System.Globalization.CultureInfo CultureInfo = System.Globalization.CultureInfo.CurrentCulture;

        internal static SpawnPoint SelectNearbySpawnpoint(List<SpawnPoint> spawnPoints)
        {
            SelectNearbyLocationsWithHeading(spawnPoints.Select(s => s.Position).ToList(), spawnPoints.Select(s => s.Heading).ToList(), out var vector3, out var heading);
            return new SpawnPoint(vector3, heading);
        }
        internal static void SelectNearbyLocationsWithHeading(List<Vector3> listLoc, List<float> listHead, out Vector3 sp, out float heading)
        {
            sp = Vector3.Zero;
            heading = 0f;
            try
            {
                var playerPos = Game.LocalPlayer.Character.Position;
                if (listLoc.Count != listHead.Count) return;
                ToLog($"Calculating the best location for callout");
                List<Vector3> locsToSelect = listLoc.Where(l => l.DistanceTo(playerPos) < 1000f && l.DistanceTo(playerPos) > 300f && l.TravelDistanceTo(playerPos) < 2000f).ToList();
                if (locsToSelect.Count > 0)
                {
                    sp = locsToSelect.GetRandomElement(true);
                }
                else
                {
                    foreach (Vector3 l in listLoc)
                    {
                        if (l.DistanceTo(playerPos) < 1200f && l.DistanceTo(playerPos) > 250f)
                        {
                            if (l.TravelDistanceTo(playerPos) > 3000) continue;
                            sp = l;
                            break;
                        }
                    }
                }
                if (listLoc.Contains(sp)) heading = listHead[listLoc.IndexOf(sp)];
                else heading = 0f;

                if (sp != Vector3.Zero && heading != 0f)
                {
                    ToLog($"Location found X:{sp.X} Y:{sp.Y} Z:{sp.Z}. Heading: {heading}");
                    ToLog($"Location found in {GetZoneName(sp)} near {World.GetStreetName(sp)}");
                }
            }
            catch (Exception e)
            {
                sp = Vector3.Zero;
                heading = 0f;
                "Failed when try to select nearby locations".ToLog();
                e.Message.ToLog();
            }
            return;
        }
        internal static void Print(this string msg) => Game.Console.Print(msg);
        internal static void ToLog(this string micin) => ToLog(micin, false);
        internal static void ToLog(this string micin, bool makeUppercase)
        {
            string text = makeUppercase ? micin.ToUpper() : micin;
            Game.LogTrivial(makeUppercase ? "[BARBARIAN-CALL]: " : "[BarbarianCall]: " + text);
        }
        internal static string GetLicensePlateAudio(Vehicle veh) => GetLicensePlateAudio(veh.LicensePlate);
        internal static string GetLicensePlateAudio(string licensePlate)
        {
            int count = 0;
            string metu = string.Empty;
            if (licensePlate.Length != 8) return string.Empty;
            foreach (char c in licensePlate)
            {
                count++;
                if (count == 1) metu = metu + "BAR_" + c.ToString().ToUpper() + "_HIGH ";
                else if (count == 8) metu = metu + "BAR_" + c.ToString().ToUpper() + "_LOW";
                else metu = metu + "BAR_" + c.ToString().ToUpper() + " ";
            }
            Game.Console.Print(metu);
            return metu;
        }
        internal static string GetColorAudio(Color color)
        {
            ToLog("Trying to get color audio");
            var audibleArgb = (from x in CommonVariables.AudibleColor select x.ToArgb()).ToList();
            Color selected;
            if (audibleArgb.Contains(color.ToArgb()))
            {
                ToLog("Color match, converting to scanner audio");
                selected = CommonVariables.AudibleColor[audibleArgb.ToList().IndexOf(color.ToArgb())];
                var ret = "COLOR_" + selected.Name.AddSpacesToSentence().Replace(" ", "_").ToUpper();
                ret.ToLog();
                return ret;
            }
            ToLog($"Color not match {color.ToArgb()}");
            return string.Empty;
        }
        internal static void RandomiseLicencePlate(this Vehicle vehicle)
        {
            if (vehicle.Exists())
            {
                vehicle.LicensePlate = 
                                    Random.Next(9).ToString() +
                                    Random.Next(9).ToString() +
                                    Convert.ToChar(Random.Next(0, 25) + 65) +
                                    Convert.ToChar(Random.Next(0, 25) + 65) +
                                    Convert.ToChar(Random.Next(0, 25) + 65) +
                                    Random.Next(9).ToString() +
                                    Random.Next(9).ToString() +
                                    Random.Next(9).ToString();
                ToLog($"{vehicle.PrimaryColor.Name} {Game.GetLocalizedString(vehicle.Model.Name)} license plate changed to {vehicle.LicensePlate}");
                ToLog($"{GetColorAudio(vehicle.PrimaryColor)}");
                Game.Console.Print($"{vehicle.GetVehicleDisplayName()}");
                GameFiber.Sleep(1);
            }
        }
        internal static string GetZoneName(this ISpatial spatial) => GetZoneName(spatial.Position);
        internal static string GetZoneName(this Vector3 pos)
        {
            string gameName = NativeFunction.Natives.GET_NAME_OF_ZONE<string>(pos.X, pos.Y, pos.Z);
            return Game.GetLocalizedString(gameName);
        }
        internal static string GetVehicleDisplayName(this Vehicle veh) => NativeFunction.Natives.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL<string>(veh.Model.Hash);
        public static string GetCardinalDirectionLowDetailedAudio(this Entity e)
        {
            float degrees = e.Heading;
            string[] cardinals = { "DIRECTION_BOUND_NORTH", "DIRECTION_BOUND_WEST", "DIRECTION_BOUND_SOUTH", "DIRECTION_BOUND_EAST", "DIRECTION_BOUND_NORTH" };
            return cardinals[(int)Math.Round(((double)degrees % 360) / 90)];
        }
        internal static bool Speaking;
        internal static void HandleSpeech(List<string> Dialogue, Ped talker)
        {
            ToLog("Speech Started");
            Speaking = true;
            var playerPos = Game.LocalPlayer.Character.Position;
            var playerHead = Game.LocalPlayer.Character.Heading;
            List<string> modifiedDialogue = new List<string>();
            Dialogue.ForEach(cakap=>
            {
                var modified = cakap;
                if (modified.Contains("Officer:")) modified = modified.Replace("Officer:", "~b~Officer~s~:");
                else if (modified.Contains("Witness:")) modified = modified.Replace("Witness:", "~o~Witness~s~:");
                else if (modified.Contains("Suspect:")) modified = modified.Replace("Suspect:", "~r~Suspect~s~:");
                else if (modified.Contains("Paramedic:")) modified = modified.Replace("Paramedic:", "~g~Paramedic~s~:");
                else if (modified.Contains("Medic:")) modified = modified.Replace("Medic:", "~g~Medic~s~:");
                modifiedDialogue.Add(modified);
            });
            (modifiedDialogue.Count == Dialogue.Count).ToString().ToLog();
            NativeFunction.Natives.SET_PED_CAN_SWITCH_WEAPON(Game.LocalPlayer.Character, false);
            GameFiber.StartNew(delegate
            {
                while (Speaking)
                {
                    GameFiber.Yield();
                    if (Vector3.Distance(Game.LocalPlayer.Character.Position, playerPos) > 2.5f)
                    {
                        Game.LocalPlayer.Character.Tasks.FollowNavigationMeshToPosition(playerPos, playerHead, 1.5f).WaitForCompletion(1000);
                    }
                    if (Game.LocalPlayer.Character.IsInAnyVehicle(false))
                    {
                        Game.LocalPlayer.Character.Tasks.LeaveVehicle(LeaveVehicleFlags.None).WaitForCompletion(1800);
                    }
                }
            });
            if (talker.Exists() && !talker.IsInAnyVehicle(false))
            {
                talker.Tasks.AchieveHeading(talker.GetHeadingTowards(Game.LocalPlayer.Character));
                talker.Tasks.PlayAnimation("special_ped@jessie@monologue_1@monologue_1f", "jessie_ig_1_p1_heydudes555_773", 1f, AnimationFlags.Loop | AnimationFlags.SecondaryTask);
            }
            for (int i = 0; i < modifiedDialogue.Count; i++)
            {
                while (Speaking)
                {
                    GameFiber.Yield();
                    if (Game.IsKeyDown(Keys.Y)) break;
                }
                talker.Tasks.AchieveHeading(talker.GetHeadingTowards(Game.LocalPlayer.Character));
                Game.DisplaySubtitle(modifiedDialogue[i], 10000);
                if (!Speaking) break;
            }
            Speaking = false;
            NativeFunction.Natives.SET_PED_CAN_SWITCH_WEAPON(Game.LocalPlayer.Character, true);
            if (talker.IsValid() && talker.Exists()) talker.Tasks.ClearImmediately();
        }
        internal static void MakeMissionPed(this Ped ped, bool invincible = false)
        {
            ped.MakePersistent();
            ped.BlockPermanentEvents = true;
            ped.Money = 1;
            ped.Health = 500;
            ped.Armor = 50;
            ped.Opacity = 1.0f;
            ped.IsVisible = true;
            ped.IsInvincible = invincible;
        }
        internal static void DisplayNotifWithLogo(this string msg, string calloutName = "", string textureName = "WEB_LOSSANTOSPOLICEDEPT", string textureDict = " WEB_LOSSANTOSPOLICEDEPT") => 
            Game.DisplayNotification(textureName, textureName, "~y~BarbarianCall~s~", "~y~" + calloutName + "~s~", msg);
        internal static void DisplayNotifWithLogo(this string msg, out uint notifId, string calloutName = "", string textureName = "WEB_LOSSANTOSPOLICEDEPT") =>
            notifId = Game.DisplayNotification(textureName, textureName, "~y~BarbarianCall~s~", "~y~" + calloutName + "~s~", msg);
        internal static IEnumerable<Ped> GetNearbyPedByRadius(this Vector3 pos, float radius)
        {
            List<Ped> peds = new List<Ped>();
            foreach (Ped ped in World.GetAllPeds())
            {
                if (ped != Game.LocalPlayer.Character && ped.Exists() && ped.IsValid() && ped.IsHuman &&
                    !ped.IsInAnyVehicle(false) && ped.Position.DistanceTo(pos) < radius && ped.IsAlive && !ped.IsGettingIntoVehicle && !ped.CreatedByTheCallingPlugin)
                {
                    peds.Add(ped);
                }
            }
            return peds;
        }
        internal static IEnumerable<Vehicle> GetNearbyVehicleByRadius(this Vector3 pos, float radius)
        {
            List<Vehicle> vehs = new List<Vehicle>();
            foreach (Vehicle veh in World.GetAllVehicles())
            {
                if (veh != Game.LocalPlayer.Character.CurrentVehicle && veh != Game.LocalPlayer.Character.LastVehicle && veh.Exists() && veh.IsValid() &&
                    !veh.IsPoliceVehicle && !veh.CreatedByTheCallingPlugin && veh.Position.DistanceTo(pos) < radius)
                {
                    vehs.Add(veh);
                }
            }
            return vehs;
        }
        internal static string FormatKeyBinding(Keys modifierKey, Keys key)
            => modifierKey == Keys.None ? $"~{key.GetInstructionalId()}~" :
                                          $"~{modifierKey.GetInstructionalId()}~ ~+~ ~{key.GetInstructionalId()}~";
        internal static bool CheckKey(Keys modifierKey, Keys key)
        {
            bool keyboardInputCheck = NativeFunction.CallByHash<int>(0x0CF2B696BBF945AE) == 0;
            if (!keyboardInputCheck)
            {
                if (Game.IsKeyDown(key) && modifierKey == Keys.None) return true;
                if (Game.IsKeyDownRightNow(modifierKey) && Game.IsKeyDown(key)) return true;
            }
            return false;
        }
        internal static Model GetRandomModel(this IEnumerable<string> list)
        {
            var list1 = list.ToList();
            string selected = list1[MathHelper.GetRandomInteger(0, list1.Count - 1)];
            var ret = new Model(selected);
            if (!ret.IsValid) $"{selected} is invalid".ToLog();
            $"Selected model is {ret.Name}".ToLog();
            return ret;
        }
        internal static Color GetRandomColor(this IEnumerable<Color> list, out string tostr)
        {
            var colorList = list.ToList();
            var ret = colorList[MathHelper.GetRandomInteger(0, colorList.Count - 1)];
            if (!ret.IsKnownColor) $"{ret.Name} is invalid color".ToLog();
            $"Selected color is {ret.Name}".ToLog();
            tostr = ret.Name;
            return ret;
        }
        internal static void InjectRandomItemToVehicle(this Vehicle vehicle)
        {
            try
            {
                if (!Initialization.IsLSPDFRPluginRunning("StopThePed")) return;
                if (vehicle.Exists())
                {
                    string selected;
                    int rand1 = MathHelper.GetRandomInteger(1, 10);
                    if (rand1 < 3)
                    {
                        selected = "~r~" + CommonVariables.DangerousVehicleItems.GetRandomElement();
                    }
                    else if (rand1 > 2 && rand1 < 6)
                    {
                        selected = "~y~" + CommonVariables.SuspiciousItems.GetRandomElement();
                    }
                    else
                    {
                        selected = "~g~" + CommonVariables.CommonItems.GetRandomElement();
                    }
                    vehicle.Metadata.searchTrunk = selected;
                    if (Random.Next(1, 6800) % 2 == 0)
                    {
                        vehicle.Metadata.searchDriver = "~y~" + CommonVariables.SuspiciousItems.GetRandomElement();
                    }
                    else
                    {
                        vehicle.Metadata.searchDriver = "~g~" + CommonVariables.CommonItems.GetRandomElement();
                    }
                    vehicle.Metadata.searchPassenger = "~g~" + CommonVariables.CommonItems.GetRandomElement();
                }
            } catch (Exception e)
            {
                $"Failed to inject item to vehicle {vehicle.Model.Name}".ToLog();
                e.Message.ToLog();
            }          
        }
        /// <summary>
        /// Gets the heading towards an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="towardsEntity">Entity to face to</param>
        /// <returns>the heading towards an entity</returns>
        public static float GetHeadingTowards(this ISpatial spatial, ISpatial towards)
        {
            return GetHeadingTowards(spatial, towards.Position);
        }


        /// <summary>
        /// Gets the heading towards a position
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="towardsPosition">Position to face to</param>
        /// <returns>the heading towards a position</returns>
        public static float GetHeadingTowards(this ISpatial spatial, Vector3 towardsPosition)
        {
            return GetHeadingTowards(spatial.Position, towardsPosition);
        }


        /// <summary>
        /// Gets the heading towards an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="towardsEntity">Entity to face to</param>
        /// <returns>the heading towards an entity</returns>
        public static float GetHeadingTowards(this Vector3 position, Vector3 towardsPosition)
        {
            Vector3 directionFromEntityToPosition = (towardsPosition - position);
            directionFromEntityToPosition.Normalize();

            float heading = MathHelper.ConvertDirectionToHeading(directionFromEntityToPosition);
            return heading;
        }

        /// <summary>
        /// Gets the heading towards an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="towardsEntity">Entity to face to</param>
        /// <returns>the heading towards an entity</returns>
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
                    uint headshotHandle = NativeFunction.Natives.RegisterPedheadshot<uint>(ped);
                    DateTime endTime = DateTime.Now + new TimeSpan(0, 0, 10);
                    var start = DateTime.Now;                   
                    while (true)
                    {
                        GameFiber.Yield();
                        if (NativeFunction.Natives.IsPedheadshotReady<bool>(headshotHandle))
                        {
                            $"Ped Headshot found with handle {headshotHandle}".ToLog();
                            break;
                        }
                        if (DateTime.Now >= endTime) break;
                    }
                    string txd = NativeFunction.Natives.GetPedheadshotTxdString<string>(headshotHandle);
                    string txn = txd;                
                    Game.DisplayNotification(txn, txd, title, subtitle, text);
                    //GameFiber.Wait(200);
                    NativeFunction.Natives.UnregisterPedheadshot<uint>(headshotHandle);
                    TimeSpan duration = DateTime.Now - start;
                    $"Register ped headshot transparent is took {duration.TotalMilliseconds} ms".ToLog();
                }
                catch (Exception e)
                {
                    Game.DisplayNotification("srange_gen", "blanktrophy_gold", title, subtitle, text);
                    "Display notification with mugshot error".ToLog();
                    e.ToString().ToLog();
                    e.Message.ToLog();
                }
            });
        }
        internal static string GetPedHeadshotTexture(this Ped ped, out uint? Handle, string failedReturn = "WEB_LOSSANTOSPOLICEDEPT")
        {
            try
            {
                uint headshotHandle = NativeFunction.Natives.RegisterPedheadshot<uint>(ped); //RegisterPedHeadshotTransparent
                int startTime = Environment.TickCount;
                DateTime endTime = DateTime.Now + new TimeSpan(0, 0, 10);
                while (true)
                {
                    GameFiber.Yield();
                    if (NativeFunction.Natives.IsPedheadshotReady<bool>(headshotHandle))
                    {
                        $"Ped Headshot found with handle {headshotHandle}".ToLog();
                        break;
                    }
                    if (DateTime.Now >= endTime) break;
                }
                string txd = NativeFunction.Natives.GetPedheadshotTxdString<string>(headshotHandle);
                Handle = headshotHandle;
                return txd;
            }
            catch (Exception)
            {
                "Get ped headshot failed".ToLog();
            }
            Handle = null;
            return failedReturn;
        }
        internal static void UnregisterPedHeadshot(this uint? handle)
        {
            if (handle.HasValue)
            {
                NativeFunction.Natives.UnregisterPedheadshot<uint>(handle.Value);
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

        public static T GetRandomElement<T>(this Enum items) where T : Enum
        {
            if (typeof(T).BaseType != typeof(Enum))
                throw new InvalidCastException();

            var types = Enum.GetValues(typeof(T));
            return GetRandomElement(types.Cast<T>());
        }

        public static IList<T> GetRandomNumberOfElements<T>(this IList<T> list, int numOfElements, bool shuffle = false)
        {
            List<T> givenList = new List<T>(list);
            List<T> l = new List<T>();
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
            List<T> givenList = new List<T>(enumarable);
            List<T> l = new List<T>();
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
            StringBuilder newText = new StringBuilder(text.Length * 2);
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
            Persona newWantedPersona = new Persona(pedPersona.Forename, pedPersona.Surname, pedPersona.Gender, pedPersona.Birthday)
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
    }
}
