using LSPD_First_Response.Mod.API;
using Rage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BarbarianCall
{
    public class EntryPoint
    {
        public static void Main()
        {
            Game.DisplayNotification("~r~BARBARIANCALLS FAILED TO LOAD, PLEASE PUT THE DLL FILES INSIDE ~g~\"Plugins/LSPDFR\"~r~ FOLDER");
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
                Game.DisplayNotification("BarbarianCalls Loaded ~g~Successfully");
                CheckPluginRunning();
                $"RAGENativeUi in installed: {IsRageNativeUIInstalled()}".ToLog();
                "Prepering to create pause menu".ToLog();
                Menus.PauseMenu.CreatePauseMenu();
            });
        }
        public static bool IsLSPDFRPluginRunning(string Plugin)
        {
            try
            {
                foreach (Assembly assembly in Functions.GetAllUserPlugins())
                {
                    if (string.Equals(assembly.GetName().Name, Plugin, StringComparison.CurrentCultureIgnoreCase))
                    {
                        $"Plugin : {Plugin} Assembly : {assembly.GetName().Name} is running".ToLog();
                        return true;
                    }
                }
                $"{Plugin} is not running".ToLog();
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
        }
        internal static bool IsRageNativeUIInstalled() => File.Exists("RAGENativeUI.dll");
    }
}
