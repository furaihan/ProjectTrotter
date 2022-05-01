using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using Rage;
using Rage.Native;
using Rage.ConsoleCommands.AutoCompleters;
using Rage.Attributes;
using BarbarianCall.Extensions;
using BarbarianCall.Types;
using BarbarianCall.SupportUnit;
using BarbarianCall.DivisiXml;
using RAGENativeUI;
using RAGENativeUI.Elements;
using LSPD_First_Response.Mod.API;

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
            Freemode.FreemodePed freemodePed = new(pos, heading, isMale);
            GameFiber.Wait(2000);
            freemodePed.RandomizeOutfit();
            freemodePed.Dismiss();
        }       
        [ConsoleCommand(Name = "GetPlayerPosFlags", Description = "Gets the flags of the player position")]
        public static void GetFlags()
        {
            Game.LocalPlayer.Character.Position.GetFlags();
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
                if (SpawnManager.GetSafeCoordForPed(Game.LocalPlayer.Character.Position, true, out var result, (int)flag)) pros = result;
                else if (SpawnManager.GetSafeCoordForPed(Game.LocalPlayer.Character.Position, false, out var result1, (int)flag)) pros = result1;
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
        [ConsoleCommand]
        private static void GetSpawnPoint(int durationMillisecond)
        {
            var ppos = Game.LocalPlayer.Character.Position;
            Spawnpoint[] spawnpoints =
            {
                SpawnManager.GetVehicleSpawnPoint(ppos, 300, 500),
                SpawnManager.GetVehicleSpawnPoint2(ppos, 300, 500),
                SpawnManager.GetVehicleSpawnPoint3(ppos, 300, 500),
                SpawnManager.GetVehicleSpawnPoint4(ppos, 300, 500),
                SpawnManager.GetVehicleSpawnPoint5(ppos, 300, 500),
            };
            List<Checkpoint> checkpoints = new(); 
            for (int i = 0; i < spawnpoints.Length; i++)
            {
                Spawnpoint v = spawnpoints[i];
                if (v != Spawnpoint.Zero)
                {
                    Checkpoint cp = new(CheckpointIcon.CylinderTripleArrow, v.Position, v.Position.ForwardVector(v.Heading), 5f, 60f, HudColor.Blue.GetColor(), HudColor.PureWhite.GetColor(), false);
                    checkpoints.Add(cp);
                    $"{i + 1}. {v}".ToLog();
                }
                else $"Spawnpoint number {i + 1} is not found".ToLog();
            }
            GameFiber.Wait(durationMillisecond);
            checkpoints.ForEach(x => x.Delete());
        }
        [ConsoleCommand(Name = "DisplayUIChoice", Description = "Display AssortedCallout-Like UI")]
        private static void DisplayUI(int timeoutMilisecond)
        {
            GameFiber.StartNew(() =>
            {
                List<string> files = Directory.EnumerateFiles(Path.Combine("lspdfr", "audio", "scanner", "STREETS")).ToList();
                PopupChoiceUI choiceUI = new(files.GetRandomNumberOfElements(4, true).ToList(), "Choose One", true);
                choiceUI.LineHeight = 30;
                choiceUI.BackgroundColor = Color.Chocolate;
                choiceUI.TextColor = Color.White;
                choiceUI.Opacity = 150;
                choiceUI.BackgroundSize = new Size(520, 680);
                choiceUI.Point = new Point(30, 40);
                choiceUI.TextScale = 0.40f;
                choiceUI.Process();
                Stopwatch sw = Stopwatch.StartNew();
                string selected = "Timeout";
                while (true)
                {
                    GameFiber.Yield();
                    choiceUI.Draw();
                    if (sw.ElapsedMilliseconds > timeoutMilisecond) break;
                    if (Game.IsKeyDown(System.Windows.Forms.Keys.D1))
                    {
                        selected = choiceUI.Choices[0];
                        break;
                    }
                    else if (Game.IsKeyDown(System.Windows.Forms.Keys.D2))
                    {
                        selected = choiceUI.Choices[1];
                        break;
                    }
                    else if (Game.IsKeyDown(System.Windows.Forms.Keys.D3))
                    {
                        selected = choiceUI.Choices[2];
                        break;
                    }
                    else if (Game.IsKeyDown(System.Windows.Forms.Keys.D4))
                    {
                        selected = choiceUI.Choices[3];
                        break;
                    }
                }
                Game.DisplaySubtitle($"Selected: {selected}");
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
    }
}
