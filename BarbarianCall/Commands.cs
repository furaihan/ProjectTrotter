using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using Rage;
using Rage.ConsoleCommands.AutoCompleters;
using Rage.Attributes;
using BarbarianCall.Extensions;
using BarbarianCall.Types;
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
                    }
                    
                });
            });
        }
        [ConsoleCommand(Name = "GetPlayerPosFlags", Description = "Gets the flags of the player position")]
        public static void GetFlags()
        {
            Game.LocalPlayer.Character.Position.GetFlags();
        }

    }
}
