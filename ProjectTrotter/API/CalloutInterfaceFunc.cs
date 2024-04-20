using System;
using System.IO;
using LSPD_First_Response.Mod.Callouts;
using Func = CalloutInterface.API.Functions;

namespace ProjectTrotter.API
{
    internal static class CalloutInterfaceFunc
    {
        private const string DllPath = @"Plugins/LSPDFR/CalloutInterface.dll";
        private static readonly bool IsValid = File.Exists(DllPath) && Initialization.IsLSPDFRPluginRunning("CalloutInterface", null, false);
        internal static void SendCalloutDetails(Callout callout, string priority, string agency)
        {
            try
            {
                Func.SendCalloutDetails(callout, priority, agency);
            }
            catch (Exception e)
            {
                e.ToString().ToLog();
            }
        }
        internal static void SendMessage(Callout callout, string message)
        {
            try
            {
                Func.SendMessage(callout, message);
            }
            catch (Exception e)
            {
                e.ToString().ToLog();
            }
        }
        static CalloutInterfaceFunc()
        {
            if (IsValid) AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }
        private static System.Reflection.Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.ToLower().StartsWith("calloutinterface, ") && IsValid)
            {
                return System.Reflection.Assembly.Load(File.ReadAllBytes(DllPath));
            }
            return null;
        }
    }
}
