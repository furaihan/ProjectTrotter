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
using RAGENativeUI;
using RAGENativeUI.Elements;
using LSPD_First_Response.Mod.API;

namespace BarbarianCall
{
    public static class Commands
    {
        [ConsoleCommand(Name = "BCGetVehicleColor", Description = "Gets the selected vehicle color and play the scanner audio")]
        public static void BCGetVehicleColor([ConsoleCommandParameter(AutoCompleterType = typeof(ConsoleCommandAutoCompleterVehicleAliveOnly))] Vehicle vehicle)
        {
            if (vehicle)
            {
                VehicleColor vehicleColor = vehicle.GetColor();
                List<string> log = new()
                {
                    $"Vehicle: {vehicle.GetDisplayName()}",
                    $"Manufacturer: {vehicle.GetMakeName()}",
                    $"Primary Color:",
                    $"     Name: {vehicleColor.PrimaryColorName}",
                    $"     RGBA: {vehicleColor.PrimaryColorRGBA}",
                    $"Secondary Color:",
                    $"     Name: {vehicleColor.SecondaryColorName}",
                    $"     RGBA: {vehicleColor.SecondaryColorRGBA}",
                };
                log.ForEach(Game.LogTrivial);
                Game.DisplaySubtitle($"Primary: <font color=\"{ColorTranslator.ToHtml(vehicleColor.PrimaryColorRGBA)}\">{vehicleColor.PrimaryColorName}</font>," +
                    $" Secondary: <font color=\"{ColorTranslator.ToHtml(vehicleColor.SecondaryColorRGBA)}\">{vehicleColor.SecondaryColorName}</font>");
                Functions.PlayScannerAudioUsingPosition(VehiclePaintExtensions.GetPoliceScannerColorAudio(vehicleColor.PrimaryColor), Game.LocalPlayer.Character.Position);
            }
            else Game.LogTrivial("Vehicle doesn't exist");
        }
        [ConsoleCommand(Name = "SpawnFreemodePed", Description = "Spawn the freemode ped and the randomise their appearance")]
        public static void SpawnFreemode([ConsoleCommandParameter(AutoCompleterType = typeof(ConsoleCommandAutoCompleterBoolean))] bool isMale)
        {
            var pos = Game.LocalPlayer.Character.Position + Game.LocalPlayer.Character.ForwardVector * 8f;
            float heading = Game.LocalPlayer.Character.Heading - 180f;
            Freemode.FreemodePed freemodePed = new(pos, heading, isMale);
            GameFiber.Wait(2000);
            freemodePed.Dismiss();
        }
        [ConsoleCommand(Name = "SaveHairColor", Description = "Save hair color of freemode ped")]
        public static void SaveHairColor([ConsoleCommandParameter(AutoCompleterType = typeof(ConsoleCommandParameterAutoCompleter))]string filename)
        {
            int num = Freemode.HeadBlend.GetNumberOfPedHairColors();
            StringBuilder @string = new();
            @string.AppendLine($"This file created in {DateTime.Now.ToLongDateString()} - {DateTime.Now.ToLongTimeString()}");
            List<string> vs = new();
            for (int i = 0; i < num; i++)
            {
                Color color = Freemode.HeadBlend.GetHairColor(i);
                var str = $"{i}+{255}+{color.R}+{color.G}+{color.B}";
                @string.AppendLine(str);
                vs.Add($"<font color=\"{ColorTranslator.ToHtml(color)}\">{i}</font>");
            }
            string path = Path.Combine("Plugins", "LSPDFR", "BarbarianCall", filename);
            File.WriteAllText(path, @string.ToString());
            GameFiber.StartNew(() =>
            {
                int i = 0;
                StringBuilder sb = new();
                GameFiber.Wait(20);
                vs.ForEach(x =>
                {
                    i++;
                    sb.Append(x + " ");
                    if (i == 10)
                    {
                        Game.DisplaySubtitle(sb.ToString());
                        Game.DisplayHelp($"Press {Peralatan.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.J)} to continue");
                        sb.Clear();
                        i = 0;
                        GameFiber.WaitUntil(() => Game.IsKeyDownRightNow(System.Windows.Forms.Keys.J));
                        GameFiber.Wait(500);
                    }
                    
                });
            });
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
        [ConsoleCommand(Name = "DisplayVersusNotification", Description = "This is a native test")]
        private static void DrawVersusNotif()
        {
            GameFiber.StartNew(() =>
            {
                GameFiber.Wait(20);
                Stopwatch stopwatch = Stopwatch.StartNew();
                NativeFunction.Natives.REQUEST_STREAMED_TEXTURE_DICT("CHAR_TREVOR");
                NativeFunction.Natives.REQUEST_STREAMED_TEXTURE_DICT("CHAR_FRANKLIN");
                GameFiber.Wait(2000);
                NativeFunction.Natives.BEGIN_TEXT_COMMAND_THEFEED_POST("");
                NativeFunction.Natives.END_TEXT_COMMAND_THEFEED_POST_VERSUS_TU("CHAR_TREVOR", "CHAR_TREVOR", 25, "CHAR_FRANKLIN", "CHAR_FRANKLIN", 26, (int)HudColor.TrevorDark, (int)HudColor.FranklinDark);
            });            
        }
        [ConsoleCommand(Name = "ActivatePlaceEditor", Description = "Activate place editor menu")]
        private static void ActivatePlaceEditor()
        {
            Game.LogTrivial("Press PageUp to open place editor menu");
            GameFiber.Wait(20);
            GameFiber.StartNew(delegate
            {
                Menus.PlaceEditor.CreateMenu();
            });
        }
        [ConsoleCommand(Name = "ActivateSitAnywhere")]
        private static void ActivateSofaOnTick()
        {
            GameFiber.StartNew(delegate
            {
                SyncSceneTick.ChairSit();
            });
        }
        private static List<HeliSupport> Helis = new List<HeliSupport>();
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
        private static void CallMilitaryHeli([ConsoleCommandParameter(AutoCompleterType = typeof(ConsoleCommandAutoCompleterPedAliveOnly))] Ped ped,
                                            MilitarySupportType type)
        {
            if (ped)
            {
                MilitaryHeliSupport militaryHeliSupport = new MilitaryHeliSupport();
            }
        }
    }
}
