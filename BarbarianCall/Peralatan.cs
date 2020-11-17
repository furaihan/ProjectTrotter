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

namespace BarbarianCall
{
    internal static class Peralatan
    {
        internal static void SelectNearbyLocationsWithHeading(List<Vector3> listLoc, List<float> listHead, out Vector3 sp, out float heading)
        {
            List<Vector3> locsToSelect = new List<Vector3>();
            sp = Vector3.Zero;
            heading = 0f;
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
                if (count == 1) metu = metu + "BAR_" + c.ToString().ToUpper() + "_HIGH";
                if (count == 8) metu = metu + "BAR_" + c.ToString().ToUpper() + "_LOW";
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
        internal static bool Speaking;
        internal static void HandleSpeech(List<string> Dialogue, Ped talker)
        {
            ToLog("Speech Started");
            Speaking = true;
            var playerPos = Game.LocalPlayer.Character.Position;
            var playerHead = Game.LocalPlayer.Character.Heading;
            GameFiber.StartNew(delegate
            {
                while (Speaking)
                {
                    GameFiber.Yield();
                    if (Vector3.Distance(Game.LocalPlayer.Character.Position, playerPos) > 2.5f)
                    {
                        Game.LocalPlayer.Character.Tasks.FollowNavigationMeshToPosition(playerPos, playerHead, 1f).WaitForCompletion(1000);
                    }
                    if (Game.LocalPlayer.Character.IsInAnyVehicle(false))
                    {
                        Game.LocalPlayer.Character.Tasks.LeaveVehicle(LeaveVehicleFlags.None).WaitForCompletion(1800);
                    }
                }
                GameFiber.Hibernate();
            });
            if (talker.IsValid() && talker.Exists() && talker.IsInAnyVehicle(false))
            {
                talker.Tasks.PlayAnimation("special_ped@jessie@monologue_1@monologue_1f", "jessie_ig_1_p1_heydudes555_773", 1f, AnimationFlags.Loop);
            }
            GameFiber.StartNew(delegate
            {
                for (int i = 0; i < Dialogue.Count; i++)
                {
                    while (true)
                    {
                        GameFiber.Yield();
                        if (Game.IsKeyDown(Keys.Y)) break;
                    }
                    Game.DisplaySubtitle(Dialogue[i]);
                    if (!Speaking) break;
                }
                Speaking = false;
                if (talker.IsValid() && talker.Exists()) talker.Tasks.ClearImmediately();
                GameFiber.Hibernate();
            });
        }
        internal static void MakeMissionPed(this Ped ped, bool invincible = false)
        {
            ped.MakePersistent();
            ped.BlockPermanentEvents = true;
            ped.Money = 1;
            ped.Health = 100;
            ped.Armor = 0;
            if (invincible) ped.IsInvincible = true;
        }
        internal static void DisplayNotifWithLogo(this string msg, string calloutName = "") => 
            Game.DisplayNotification("WEB_LOSSANTOSPOLICEDEPT", "WEB_LOSSANTOSPOLICEDEPT", "~y~BarbarianCall", "~y~" + calloutName, msg);
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
        internal static Color GetRandomColor(this IEnumerable<Color> list)
        {
            var colorList = list.ToList();
            var ret = colorList[MathHelper.GetRandomInteger(0, colorList.Count - 1)];
            if (!ret.IsKnownColor) $"{ret.Name} is invalid color".ToLog();
            $"Selected color is {ret.Name}".ToLog();
            return ret;
        }
        internal static string GetVehicleColor(this Vehicle v)
        {
            NativeFunction.Natives.GET_VEHICLE_COLOR(v, out int cr, out int cb, out int cg);
            Color col = Color.FromArgb(cr, cg, cb);
            var colorLookup = Enum.GetValues(typeof(KnownColor))
               .Cast<KnownColor>()
               .Select(Color.FromKnownColor)
               .ToLookup(c => c.ToArgb());
            List<Color> listc = new List<Color>();
            foreach (var namedColor in colorLookup[col.ToArgb()])
            {
                listc.Add(namedColor);
            }
            if (listc.Count > 0)
            {
                return listc[MathHelper.GetRandomInteger(0, listc.Count - 1)].Name;
            }
            else return "weirdly colored";
        }
    }
}
    
