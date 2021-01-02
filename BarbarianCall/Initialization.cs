using LSPD_First_Response.Mod.API;
using Rage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Headers;

namespace BarbarianCall
{
    public class EntryPoint
    {
        public static void Main()
        {
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
                $"Found male model: {CommonVariables.MaleModel.Length}".ToLog();
                $"Found common weapon {Callouts.CalloutBase.WeaponHashes.Where(h => new Model(h).Name != null).ToList().Count}".ToLog();
                $"RAGENativeUi in installed: {IsRageNativeUIInstalled()}".ToLog();
                "Prepering to create pause menu".ToLog();               
                Menus.PauseMenu.CreatePauseMenu();
            });
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
                return false;
            }
            catch (Exception e)
            {
                e.ToString().ToLog();
                return false;
            }
        }
        private static void CheckPluginRunning()
        {
            System.Globalization.CultureInfo.CurrentCulture.ToString().ToLog();
            TimeZoneInfo.Local.StandardName.ToLog();
            "=========================================================================================================".ToLog();
            "".ToLog();
            "".ToLog();
            "VERSION INFO".ToLog();
            $"LSPD First Response : {Extension.GetFileVersion(@"Plugins/LSPD First Response.dll")}".ToLog();
            $"RAGENativeUI : {Extension.GetFileVersion("RAGENativeUI.dll")}".ToLog();
            $"RAGEPluginHook : {Extension.GetFileVersion("RAGEPluginHook.exe")}".ToLog();
            $"ScriptHookV : {Extension.GetFileVersion("ScriptHookV.dll")}".ToLog();
            $"StopThePed : {Extension.GetFileVersion(@"Plugins/LSPDFR/StopThePed.dll")}".ToLog();
            $"UltimateBackup : {Extension.GetFileVersion(@"Plugins/LSPDFR/UltimateBackup.dll")}".ToLog();
            $"GrammarPolice : {Extension.GetFileVersion(@"Plugins/LSPDFR/GrammarPolice.dll")}".ToLog();
            "".ToLog();
            "".ToLog();
            "=========================================================================================================".ToLog();
        }
        internal static bool IsRageNativeUIInstalled() => File.Exists("RAGENativeUI.dll");       
    }
}
