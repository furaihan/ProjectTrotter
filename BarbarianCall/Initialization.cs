using LSPD_First_Response.Mod.API;
using Rage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BarbarianCall.AmbientEvent;
using BarbarianCall.Extensions;

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
        public static Assembly LSPDFRResolveEventHandler(object sender, ResolveEventArgs args) 
        { 
            foreach (Assembly assembly in Functions.GetAllUserPlugins()) 
            { 
                if (args.Name.ToLower().Contains(assembly.GetName().Name.ToLower())) 
                { 
                    return assembly; 
                } 
            } 
            return null; 
        }
        public static void Initialize()
        {
            GameFiber.StartNew(delegate
            {
                GameFiber.Wait(5600);
                if (!Types.GameOffsets.Init())
                {
                    $"Game Version is not supported".ToLog();
                    $"Supported Version: {new Version(1, 0, 2189, 0)}".ToLog();
                    $"Current Version: {Game.ProductVersion}".ToLog();
                    Game.UnloadActivePlugin();
                }
                Game.DisplayNotification("BarbarianCalls Loaded ~g~Successfully");
                CheckPluginRunning();
                $"Found male model: {Globals.MaleModel.Length}".ToLog();
                $"Found common weapon {Callouts.CalloutBase.WeaponHashes.Where(h => new Model(h).Name != null).ToList().Count}".ToLog();
                $"RAGENativeUi in installed: {IsRageNativeUIInstalled()}".ToLog();
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


        public static bool IsLSPDFRPluginRunning(string Plugin, Version minVersion = null)
        {
            try
            {
                foreach (Assembly assembly in Functions.GetAllUserPlugins())
                {
                    if (string.Equals(assembly.GetName().Name, Plugin, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (minVersion == null || assembly.GetName().Version.CompareTo(minVersion) >= 0)
                        return true;
                    }
                }
                $"{Plugin} is not running or outdated".ToLog();
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
