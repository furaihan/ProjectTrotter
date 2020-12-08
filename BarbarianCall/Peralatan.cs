using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using Rage.Native;
using System.Windows.Forms;
using RAGENativeUI;
using System.Drawing;
using BarbarianCall;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;

namespace BarbarianCall
{
    internal static class Peralatan
    {
        internal static void SelectNearbyLocationsWithHeading(List<Vector3> listLoc, List<float> listHead, out Vector3 sp, out float heading)
        {
            sp = Vector3.Zero;
            heading = 0f;
            try
            {
                List<Vector3> locsToSelect = new List<Vector3>();
                Vector3 perLocation;
                var playerPos = Game.LocalPlayer.Character.Position;
                if (listLoc.Count != listHead.Count) return;
                ToLog($"Calculating the best location for callout");
                for (int i = 0; i < listLoc.Count; i++)
                {
                    perLocation = listLoc[i];
                    if (perLocation.TravelDistanceTo(playerPos) > 2500f) continue;
                    if (perLocation.DistanceTo(playerPos) < 1000f && perLocation.DistanceTo(playerPos) > 300f)
                    {
                        locsToSelect.Add(perLocation);
                    }
                }
                if (locsToSelect.Count > 0)
                {
                    sp = locsToSelect[MathHelper.GetRandomInteger(0, locsToSelect.Count - 1)];
                }
                else
                {
                    foreach (Vector3 l in listLoc)
                    {
                        if (l.DistanceTo(playerPos) < 1500f && l.DistanceTo(playerPos) > 250f)
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
        internal static void ToLog(this string micin) => Game.LogTrivial("[BarbarianCall]: " + micin);
        internal static string GetLicensePlateAudio(string licensePlate)
        {
            int count = 0;
            string metu = string.Empty;
            if (licensePlate.Length > 8) return string.Empty;
            foreach (char c in licensePlate)
            {
                count++;
                if (count == 1) metu = metu + "BAR_" + c.ToString().ToUpper() + "_HIGH ";
                else if (count == 8) metu = metu + "BAR_" + c.ToString().ToUpper() + "_LOW";
                else metu = metu + "BAR_" + c.ToString().ToUpper() + " ";
            }
            return metu;
        }
        internal static void RandomiseLicencePlate(this Vehicle vehicle)
        {
            if (vehicle.Exists())
            {
                vehicle.LicensePlate = MathHelper.GetRandomInteger(9).ToString() +
                                       MathHelper.GetRandomInteger(9).ToString() +
                                       Convert.ToChar(MathHelper.GetRandomInteger(0, 25) + 65) +
                                       Convert.ToChar(MathHelper.GetRandomInteger(0, 25) + 65) +
                                       Convert.ToChar(MathHelper.GetRandomInteger(0, 25) + 65) +
                                       MathHelper.GetRandomInteger(9).ToString() +
                                       MathHelper.GetRandomInteger(9).ToString() +
                                       MathHelper.GetRandomInteger(9).ToString();
                ToLog($"{vehicle.PrimaryColor.Name} {vehicle.Model.Name} license plate changed to {vehicle.LicensePlate}");
                GameFiber.Sleep(1);
            }
        }
        internal static string GetZoneName(this Vector3 pos)
        {
            string gameName = NativeFunction.Natives.GET_NAME_OF_ZONE<string>(pos.X, pos.Y, pos.Z);
            return Game.GetLocalizedString(gameName);
        }
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
            if (talker.IsValid() && talker.Exists() && talker.IsInAnyVehicle(false))
            {
                talker.Tasks.PlayAnimation("special_ped@jessie@monologue_1@monologue_1f", "jessie_ig_1_p1_heydudes555_773", 1f, AnimationFlags.Loop);
            }
            GameFiber.StartNew(delegate
            {
                Game.DisplaySubtitle(Dialogue[0], 10000);
                for (int i = 1; i < Dialogue.Count; i++)
                {
                    while (Speaking)
                    {
                        GameFiber.Yield();
                        if (Game.IsKeyDown(Keys.Y)) break;
                    }
                    var dir = Game.LocalPlayer.Character.Position - talker.Position;
                    dir.Normalize();
                    talker.Tasks.AchieveHeading(MathHelper.ConvertDirectionToHeading(dir));
                    Game.DisplaySubtitle(Dialogue[i], 10000);
                    if (!Speaking) break;
                }
                Speaking = false;
                NativeFunction.Natives.SET_PED_CAN_SWITCH_WEAPON(Game.LocalPlayer.Character, true);
                if (talker.IsValid() && talker.Exists()) talker.Tasks.ClearImmediately();
            });
        }
        internal static void MakeMissionPed(this Ped ped, bool invincible = false)
        {
            ped.MakePersistent();
            ped.BlockPermanentEvents = true;
            ped.Money = 1;
            ped.Health = 200;
            ped.Armor = 0;
            ped.IsInvincible = invincible;
        }
        internal static void DisplayNotifWithLogo(this string msg, string calloutName = "", string textureName = "WEB_LOSSANTOSPOLICEDEPT") => 
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
        internal static Model GetRandomModel(this IEnumerable<string> list)
        {
            var list1 = list.ToList();
            var ret = new Model(list1[MathHelper.GetRandomInteger(0, list1.Count - 1)]);
            if (!ret.IsValid) $"{ret.Name} is invalid".ToLog();
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

        internal static string GetPedHeadshotTxd(this Ped ped)
        {
            try
            {
                unsafe
                {
                    string ret;
                    uint handle = (uint)NativeFunction.CallByHash<uint>(0x953563ce563143af, ped);
                    int count = 0;
                    while (!NativeFunction.Natives.IS_PEDHEADSHOT_READY<bool>(handle) && !NativeFunction.Natives.IS_PEDHEADSHOT_VALID<bool>(handle))
                    {
                        count++;
                        GameFiber.Yield();
                        if (count >= 40) break;
                    }
                    if (NativeFunction.Natives.IS_PEDHEADSHOT_READY<bool>(handle) && NativeFunction.Natives.IS_PEDHEADSHOT_VALID<bool>(handle))
                    {
                        ret = NativeFunction.Natives.GET_PEDHEADSHOT_TXD_STRING<string>(handle);
                        return ret;
                    }
                    else
                    {
                        $"{handle} Mugshot is invalid".ToLog();
                        return "WEB_LOSSANTOSPOLICEDEPT";
                    }
                }
            } catch (Exception e)
            {
                "Failed to get mugshot of the ped".ToLog();
                e.Message.ToLog();
                return "WEB_LOSSANTOSPOLICEDEPT";
            }
        }
        internal static void DisplayNotificationsWithPedHeadshot(this Ped ped, string subtitle, string text, string title = "Barbarian Call")
        {
            GameFiber.StartNew(() =>
            {
                try
                {
                    uint headshotHandle = NativeFunction.Natives.x953563CE563143AF<uint>(ped); //RegisterPedHeadshotTransparent
                    int startTime = Environment.TickCount;
                    GameFiber.WaitUntil(() => NativeFunction.Natives.IsPedheadshotReady<bool>(headshotHandle), 10000);
                    string txd = NativeFunction.Natives.GetPedheadshotTxdString<string>(headshotHandle);
                    string txn = txd;                  
                    Game.DisplayNotification(txn, txd, title, subtitle, text);
                    NativeFunction.Natives.UnregisterPedheadshot<uint>(headshotHandle);
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

        public static Random Random = new Random(MathHelper.GetRandomInteger(1000, 10000));
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
                return default(T);

            if (shuffle) list.Shuffle();
            return list[Random.Next(list.Count)];
        }

        public static T GetRandomElement<T>(this IEnumerable<T> enumarable, bool shuffle = false)
        {
            if (enumarable == null || enumarable.Count() <= 0)
                return default(T);

            T[] array = enumarable.ToArray();
            return GetRandomElement(array, shuffle);
        }

        public static T GetRandomElement<T>(this Enum items)
        {
            if (typeof(T).BaseType != typeof(Enum))
                throw new InvalidCastException();

            var types = Enum.GetValues(typeof(T));
            return GetRandomElement<T>(types.Cast<T>());
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
        internal static void PlaceWaypoint(this Vector3 pos) => PlaceWaypoint(new Vector2(pos.X, pos.Y));
        internal static void PlaceWaypoint(this Vector2 pos) => NativeFunction.Natives.SET_NEW_WAYPOINT(pos.X, pos.Y);
        internal static void RemoveWaypoint() => NativeFunction.Natives.SET_WAYPOINT_OFF();
        internal static void SetPedAsWanted(this Ped ped)
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
            Functions.SetPersonaForPed(ped, newWantedPersona);
        }
        internal static bool GetClosestVehicleNodeWithheading(Vector3 pos, out Vector3 outpos, out float heading) => NativeFunction.Natives.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING<bool>(pos.X, pos.Y, pos.Z, out outpos, out heading, 0, 3, 0);
        internal static bool GetRoadSidePointWithHeading(this Vector3 pos, out Vector3 outPos,out float outHeading)
        {
            outPos = Vector3.Zero;
            outHeading = float.NaN;
            try
            {
                if (NativeFunction.Natives.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING<bool>(pos.X, pos.Y, pos.Z, out Vector3 nodePos, out float nodeHeading, 0, 3, 0)) //GetNTHClosestVehicleNodeWithHeadingFavourDirections
                {
                    if (NativeFunction.Natives.xA0F8A7517A273C05<bool>(nodePos.X, nodePos.Y, nodePos.Z, nodeHeading, out Vector3 rsPos)) //GetRoadSidePointWithHeading
                    {
                        outPos = rsPos;
                        outHeading = nodeHeading;
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                e.ToString().ToLog();
            }
            return false;
        }
        internal static bool GetRoadSidePointFavourDirections(Vector3 pos, Vector3 favoredPos, out Vector3 outPos, out float heading)
        {
            heading = float.NaN;
            outPos = Vector3.Zero;
            try
            {
                if (NativeFunction.Natives.x45905BE8654AE067<bool>(pos.X, pos.Y, pos.Z, favoredPos.X, favoredPos.Y, favoredPos.Z, 1, out Vector3 nodePos, out float nodeHeading, 0, 0x40400000, 0)) //GetNTHClosestVehicleNodeFavourDirection
                {
                    if (NativeFunction.Natives.xA0F8A7517A273C05<bool>(nodePos.X, nodePos.Y, nodePos.Z, nodeHeading, out Vector3 rsp)) //GetRoadSidePointWithHeading
                    {
                        heading = nodeHeading;
                        outPos = rsp;
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                e.ToString().ToLog();
            }
            return false;
        }
    }
}
