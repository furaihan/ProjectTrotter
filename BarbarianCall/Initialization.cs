using LSPD_First_Response.Mod.API;
using Rage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BarbarianCall.AmbientEvent;
using BarbarianCall.Extensions;

[assembly: Rage.Attributes.Plugin("BarbarianCall", Description = "INSTALL IN GTAV/PLUGINS/LSPDFR", Author = "furaihan")]
namespace BarbarianCall
{
    public class EntryPoint
    {
        public static void Main()
        {
            GameFiber.Wait(8000);
            "~r~BARBARIANCALL FAILED TO LOAD, PLEASE PUT THE DLL FILES INSIDE ~g~\"Plugins/LSPDFR\"~r~ FOLDER".DisplayNotifWithLogo("~r~WARNING!!~s~", hudColor: RAGENativeUI.HudColor.RadarDamage);
            return;
        }
    }
    internal static class Initialization
    {
        public static void Initialize()
        {
            GameFiber.StartNew(delegate
            {
                $"RAGENativeUI is installed: {IsRageNativeUIInstalled()}".ToLog();               
                Extension.GetVehicleScannerAudio();
                $"Model that can be played with police scanner: {Globals.ScannerVehicleModel.Length}".ToLog();
                GameFiber.Wait(5600);
                Game.DisplayNotification("BarbarianCalls Loaded ~g~Successfully");
                CheckPluginRunning();
                // try { DivisiXml.Deserialization.LoadPoliceStationLocations(); } catch { "Read Police station error".ToLog(); }
                "Preparing to create pause menu".ToLog();
                Menus.PauseMenu.CreatePauseMenu();
            });
        }


        public static bool IsLSPDFRPluginRunning(string Plugin, Version minVersion = null, bool log = true)
        {
            try
            {
               if (log) $"Checking if {Plugin} is running".ToLog();
                foreach (Assembly assembly in Functions.GetAllUserPlugins())
                {
                    if (string.Equals(assembly.GetName().Name, Plugin, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (minVersion == null || assembly.GetName().Version.CompareTo(minVersion) >= 0)
                        {
                            if (log) $"{assembly.GetName().Name} is detected running in version {assembly.GetName().Version}".ToLog();
                            return true;
                        }
                    }
                }
                if (log) $"{Plugin} is not running or outdated".ToLog();
            }
            catch (Exception e)
            {
                e.ToString().ToLog();
                "IsLSPDFRPluginRunning error".ToLog();
            }
            return false;
        }
        private static void CheckPluginRunning()
        {
            List<string> log = new()
            {
                System.Globalization.CultureInfo.CurrentCulture.ToString(),
                TimeZoneInfo.Local.StandardName,
                "=========================================================================================================",
                "",
                "",
                "VERSION INFO",
                $"BarbarianCall Version {Assembly.GetExecutingAssembly().GetName().Version} compiled on {GetCompileTime():F}",
                $"Game Version: {Game.ProductVersion}",
                $"LSPD First Response : {Extension.GetFileVersion(@"Plugins/LSPD First Response.dll")}",
                $"RAGENativeUI : {Extension.GetFileVersion("RAGENativeUI.dll")}",
                $"RAGEPluginHook : {Extension.GetFileVersion("RAGEPluginHook.exe")}",
                $"ScriptHookV : {Extension.GetFileVersion("ScriptHookV.dll")}",
                $"StopThePed : {Extension.GetFileVersion(@"Plugins/LSPDFR/StopThePed.dll")}",
                $"UltimateBackup : {Extension.GetFileVersion(@"Plugins/LSPDFR/UltimateBackup.dll")}",
                $"GrammarPolice : {Extension.GetFileVersion(@"Plugins/LSPDFR/GrammarPolice.dll")}",
                "",
                "",
                "=========================================================================================================",
            };
            log.ForEach(Logger.ToLog);
        }
        internal static bool IsRageNativeUIInstalled() => File.Exists("RAGENativeUI.dll");
        internal static DateTime GetCompileTime()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime compileTime = new(2000, 1, 1);
            compileTime = compileTime.AddDays(version.Build);
            compileTime = compileTime.AddSeconds(version.Revision * 2);
            return compileTime;
        }
    }
}
