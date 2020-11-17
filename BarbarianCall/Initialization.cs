using LSPD_First_Response.Mod.API;
using Rage;
using System;
using System.Collections.Generic;
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
            });
        }
        public static bool IsLSPDFRPluginRunning(string Plugin, Version minversion = null)
        {
            foreach (Assembly assembly in Functions.GetAllUserPlugins())
            {
                AssemblyName an = assembly.GetName(); if (an.Name.ToLower() == Plugin.ToLower())
                {
                    if (minversion == null || an.Version.CompareTo(minversion) >= 0) { return true; }
                }
            }
            return false;
        }
    }
}
