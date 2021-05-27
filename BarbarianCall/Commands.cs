﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using Rage;
using Rage.ConsoleCommands.AutoCompleters;
using Rage.Attributes;
using BarbarianCall.Extensions;
using BarbarianCall.Types;
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
            Freemode.FreemodePed freemodePed = new(pos, heading, isMale ? LSPD_First_Response.Gender.Male : LSPD_First_Response.Gender.Female);
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
        [ConsoleCommand(Name = "PlayAudioStream", Description = "This will open an UI to select audio list")]
        public static void PlayStream([ConsoleCommandParameter(AutoCompleterType = typeof(ConsoleCommandAutoCompleterVehicleAliveOnly))] Vehicle entity)
        {
            if (!entity)
            {
                "That entity doesn't exist".ToLog();
                return;
            }
            bool draw = true;
            GameFiber.StartNew(() =>
            {
                Game.LogTrivial("Close the console");
                GameFiber.Wait(20);
                List<Sound.Stream> streams = new()
                {
                    new Sound.Stream("Distant_Sirens_Rappel", "FBI_HEIST_FINALE_CHOPPER"),
                    new Sound.Stream("CAR_CRASH_OFF_CLIFF_STREAM", "EXILE_2_SOUNDS"),
                    new Sound.Stream("FLASHBACK_01", "MICHAEL1_FLASHBACK_SOUNDSET"),
                    new Sound.Stream("FLASHBACK_02", "MICHAEL1_FLASHBACK_SOUNDSET"),
                    new Sound.Stream("FLASHBACK_03", "MICHAEL1_FLASHBACK_SOUNDSET"),
                    new Sound.Stream("Gold_Cart_Push_Anim_01", "BIG_SCORE_3B_SOUNDS"),
                    new Sound.Stream("Gold_Cart_Push_Anim_02", "BIG_SCORE_3B_SOUNDS"),
                    new Sound.Stream("CHI_2_FARMHOUSE_INTRO", "CHINESE2_FARMHOUSE_INTRODUCTION"),
                    new Sound.Stream("Boats_Jump", "EXILE_3_SOUNDS"),
                    new Sound.Stream("CHI_2_FARMHOUSE_INTRO", "CHINESE2_FARMHOUSE_INTRODUCTION"),
                    new Sound.Stream("INTRO_STREAM", "DIRT_RACES_SOUNDSET"),
                    new Sound.Stream("Player_Ride", "DLC_IND_ROLLERCOASTER_SOUNDS"),
                    new Sound.Stream("Ambient_Ride", "DLC_IND_ROLLERCOASTER_SOUNDS"),
                    new Sound.Stream("STASH_TOXIN_STREAM", "FBI_05_SOUNDS"),
                    new Sound.Stream("DRILL_WALL", "BIG_SCORE_3B_SOUNDS"),
                    new Sound.Stream("Construction_Site_Stream", "FBI_HEIST_SOUNDSET"),
                    new Sound.Stream("Walla_Normal", "DLC_H3_Arcade_Walla_Sounds"),
                    new Sound.Stream("casino_walla", "DLC_VW_Casino_Interior_Sounds"),
                    new Sound.Stream("WINDOWWASHERFALL_MASTER"),
                    new Sound.Stream("AFT_SON_PORN"),
                    new Sound.Stream("FAM2_BOAT_PARTY_MASTER"),
                    new Sound.Stream("MARIACHI", "MINUTE_MAN_01_SOUNDSET"),
                    new Sound.Stream("MARTIN_1_DAMAGED_PLANE_MASTER"),
                };
                UIMenu menu = new("Sound Name", "");
                menu.WidthOffset = 220;
                menu.AllowCameraMovement = true;
                menu.MouseControlsEnabled = false;
                menu.RemoveBanner();
                menu.DescriptionSeparatorColor = Color.Red;
                foreach (Sound.Stream stream in streams)
                {
                    menu.AddItem(new UIMenuItem($"{stream.Name} {stream.SoundSet}"));
                }
                menu.RefreshIndex();
                menu.OnItemSelect += (m, s, i) =>
                {
                    $"Selected {s.Text}".ToLog();
                    draw = false;
                    var txt = s.Text.Split(' ');
                    Sound.Stream stream1 = new(txt[0], txt[1]);
                    stream1.ToString().ToLog();
                    stream1.LoadAndWait();
                    stream1.PlayFromEntity(entity);
                };
                menu.Visible = true;
                while (draw)
                {
                    GameFiber.Yield();
                    if (menu.Visible)
                    {
                        menu.ProcessControl();
                        menu.ProcessMouse();
                    }
                    if (menu.Visible)
                    {
                        menu.Draw();
                    }
                    if (!menu.Visible) menu.Visible = true;
                }
            });
        }
        [ConsoleCommand(Name = "StopAudioStream", Description = "Stop any audio stream that currently played on")]
        public static void StopStream() => Sound.Stream.StopAnyStream();
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
                Checkpoint checkpoint = new(Checkpoint.CheckpointIcon.CylinderTripleArrow4, pros, 2f, 200f, Color.LightCoral, Color.DarkMagenta, true);
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
                choiceUI.LineHeight = 18;
                choiceUI.BackgroundColor = Color.Chocolate;
                choiceUI.TextColor = Color.White;
                choiceUI.Opacity = 150;
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
    }
}
