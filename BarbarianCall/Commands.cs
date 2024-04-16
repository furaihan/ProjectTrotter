using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using Rage;
using Rage.ConsoleCommands.AutoCompleters;
using Rage.Attributes;
using BarbarianCall.Types;
using BarbarianCall.SupportUnit;
using BarbarianCall.DivisiXml;
using BarbarianCall.MyPed;

namespace BarbarianCall
{
    public static class Commands
    {
        [ConsoleCommand]
        public static void GetDecalMale()
        {
            Deserialization.GetBadgeFromXml().ForEach(x =>
            {
                $"{x.Item1} - {x.Item2}".ToLog();
            });
        }
        [ConsoleCommand(Name = "BAR_SpawnFreemodePed", Description = "Spawn the freemode ped and the randomise their appearance")]
        public static void SpawnFreemode(bool isMale)
        {
            var pos = Game.LocalPlayer.Character.Position + Game.LocalPlayer.Character.ForwardVector * 8f;
            float heading = Game.LocalPlayer.Character.Heading - 180f;
            FreemodePed freemodePed = new(pos, heading, isMale);
            GameFiber.Wait(2000);
            freemodePed.RandomizeOutfit();
            freemodePed.Dismiss();
        }       
        [ConsoleCommand(Name = "GetPlayerPosFlags", Description = "Gets the flags of the player position")]
        public static void GetFlags()
        {
            Game.LocalPlayer.Character.Position.LogNodePropertiesAndFlags();
        }        
        private enum PNF
        {
            B02_IsFootpath = 1,
            B15_InteractionUnk = 2,
            B14_IsInterior = 4,
            B07_IsWater = 8,
            B17_IsFlatGround = 16,
            FootpathFlat = 17
        }
        [ConsoleCommand(Name = "GetPedPlaceCoord", Description = "Get nearest coordinates to safely places a ped")]
        private static void GetSafeCoordinateForPed([ConsoleCommandParameter(AutoCompleterType = typeof(ConsoleCommandParameterAutoCompleterEnum))] PNF flag)
        {
            GameFiber.StartNew(() =>
            {
                Vector3 pros;
                if (SpawnpointUtils.GetSafeCoordForPed(Game.LocalPlayer.Character.Position, true, out var result, (int)flag)) pros = result;
                else if (SpawnpointUtils.GetSafeCoordForPed(Game.LocalPlayer.Character.Position, false, out var result1, (int)flag)) pros = result1;
                else
                {
                    Game.LogTrivial("Safe Coordinate is not found");
                    return;
                }
                Stopwatch sw = Stopwatch.StartNew();
                Checkpoint checkpoint = new(CheckpointIcon.CylinderTripleArrow4, pros, 2f, 200f, Color.LightCoral, Color.DarkMagenta, true);
                while (true)
                {
                    GameFiber.Yield();
                    if (sw.Elapsed > TimeSpan.FromSeconds(150) || Game.IsKeyDownRightNow(System.Windows.Forms.Keys.D7)) break;
                }
                if (checkpoint) checkpoint.Delete();
            });
        }
        private static List<HeliSupport> Helis = new();
        [ConsoleCommand]
        private static void Command_CallHeli([ConsoleCommandParameter(AutoCompleterType =typeof(ConsoleCommandAutoCompleterVehicleAliveOnly))] Vehicle vehicle)
        {
            if (vehicle)
            {
                HeliSupport heli = new(vehicle);
                Helis.Add(heli);
            }
        }
        [ConsoleCommand]
        private static void Command_CleanUpHeli()
        {
            if (Helis.Any()) Helis.ForEach(x => x?.CleanUp());
            Helis = new List<HeliSupport>();
        }
        [ConsoleCommand]
        private static void ModVehicle([ConsoleCommandParameter(AutoCompleterType = typeof(ConsoleCommandAutoCompleterVehicleAliveOnly))] Vehicle vehicle)
        {
            if (vehicle) vehicle.Mods.ApplyAllMods();
        }
        [ConsoleCommand]
        private static void CallMilitaryHeli()
        {
            MilitaryHeliSupport militaryHeliSupport = new();
        }
        [ConsoleCommand]
        private static void DissmisAllMilitaryHeli()
        {
            MilitaryHeliSupport.InternalList.ForEach(x => x.CleanUp());
        }
        [ConsoleCommand]
        private static void GetGameTimer()
        {
            $"Game Timer: {Globals.GameTimer}".ToLog();
            GameFiber.Wait(5000);
            $"Game Timer: {Globals.GameTimer}".ToLog();
        }
        [ConsoleCommand(Description = "Make you and your current vehicle invincible", Name = "GodModeIsTrue")]
        private static void Jangkrik()
        {
            if (Game.LocalPlayer.Character.CurrentVehicle)
            {
                Game.LocalPlayer.Character.CurrentVehicle.IsInvincible = !Game.LocalPlayer.IsInvincible;
            }
            Game.LocalPlayer.IsInvincible = !Game.LocalPlayer.IsInvincible;
            Game.LogTrivial($"Godmode is set to: {Game.LocalPlayer.IsInvincible}");
        }
        [ConsoleCommand(Description = "In 1942, the Kraton Guards were dissolved by the Japanese government, but in 1970, " +
            "they were restored at the orders of Sultan Hamengkubuwono IX. Ten companies were officially reactivated with " +
            "some changes in their structure. Originally, 13 companies formed the Kraton Guard Regiment. The changes in the " +
            "newer version include changes to dress uniforms, ceremonial weapons, the number of personnel and recruitment " +
            "techniques.", Name = "Randku")]
        private static void MyRand()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 50; i++)
            {
                int a = MyRandom.Next();
                double b = Math.Round((double)a / int.MaxValue, 8);              
                sb.Append(b.ToString().PadRight(10, '0') + "  ");
                if (b > 1) sb.Append("AAAAAAAAAAAAAAAAAA");
                if ((i + 1) % 5 == 0)
                  sb.AppendLine();
            }
            Game.LogTrivial(sb.ToString());
        }
    }
}
