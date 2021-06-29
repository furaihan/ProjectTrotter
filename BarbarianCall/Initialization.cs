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
            Game.DisplayNotification("~r~BARBARIANCALL FAILED TO LOAD, PLEASE PUT THE DLL FILES INSIDE ~g~\"Plugins/LSPDFR\"~r~ FOLDER");
            return;
        }
    }
    internal static class Initialization
    {
        public static void Initialize()
        {
            GameFiber.StartNew(delegate
            {
                $"RAGENativeUi is installed: {IsRageNativeUIInstalled()}".ToLog();               
                Extension.GetAudibleVehicleModel();
                $"Model that can be played with police scanner: {Globals.AudibleCarModel.Length}".ToLog();
                Globals.AudioHash.ToList().ForEach(x =>
                {
                    Peralatan.ToLog($"Hash: {x.Key}, Name: {x.Value}, Model: {new Model(x.Key).Name}");
                });
                GameFiber.Wait(5600);
                Game.DisplayNotification("BarbarianCalls Loaded ~g~Successfully");
                CheckPluginRunning();
                try { DivisiXml.Deserialization.LoadPoliceStationLocations(); } catch { "Read Police station error".ToLog(); }
                "Prepering to create pause menu".ToLog();
                Menus.PauseMenu.CreatePauseMenu();
            });
            GameFiber.StartNew(delegate
            {
                GameFiber.Wait(7500);
                "Starting fiber for ambient dispatch call event".ToLog();
                DispatchCall.AmbientDispatchCall();
            }, "[BarbarianCall] Ambient Dispatch Call Event Fiber");
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
            log.ForEach(Peralatan.ToLog);
        }
        internal static bool IsRageNativeUIInstalled() => File.Exists("RAGENativeUI.dll");      
    }
}
