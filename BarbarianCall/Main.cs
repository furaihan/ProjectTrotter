using LSPD_First_Response.Mod.API;
using Rage;
using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace BarbarianCall
{
    public class Main : Plugin
    {
        private static StaticFinalizer Finalizer;
        public override void Finally()
        {
            foreach (Entity entity in World.GetAllEntities().Where(e=> e.CreatedByTheCallingPlugin))
            {
                if (entity && entity.CreatedByTheCallingPlugin)
                {
                    if (entity.Metadata.BAR_Entity != null && entity.Metadata.BAR_Entity == true)
                    {
                        Blip[] blips = entity.GetAttachedBlips();
                        foreach (Blip blip in blips)
                        {
                            if (blip) blip.Delete();
                        }
                        if (entity) entity.Delete();
                    }
                }
            }
            Globals.RegisteredPedHeadshot.ForEach(Peralatan.UnregisterPedHeadshot);
            "Unloaded successfully".ToLog();           
        }

        public override void Initialize()
        {
            Finalizer = new StaticFinalizer(Finally);
            StringBuilder sb = new();
            sb.AppendLine("BarbarianCall - An LSPDFR Callout Plugins, Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            sb.AppendLine($"This log started on {DateTime.Now.ToLongDateString()} - {DateTime.Now.ToLongTimeString()}");
            sb.AppendLine(TimeZoneInfo.Local.DisplayName);
            sb.AppendLine(System.Globalization.CultureInfo.CurrentCulture.DisplayName);
            string path = Path.Combine("Plugins", "LSPDFR", "BarbarianCall", "BarbarianCall.log");
            using (StreamWriter sw = new(path, false))
            {
                sw.WriteLine(sb.ToString());
                sw.Close();
            }
            Game.LogTrivial("BarbarianCall log file has been successfully initialized");
            Functions.OnOnDutyStateChanged += Functions_OnOnDutyStateChanged;
            "BarbarianCall Has beeen loaded, go on duty to start the callouts".ToLog();
        }

        private void Functions_OnOnDutyStateChanged(bool onDuty)
        {
            if (onDuty)
            {                
                Type[] callouts = { typeof(Callouts.SuspiciousVehicle), typeof(Callouts.OfficerStabbed), typeof(Callouts.TaxiRefusePay), typeof(Callouts.WantedFelonOnTheLoose), 
                    typeof(Callouts.MassStreetFighting), typeof(Callouts.Prostitution), typeof(Callouts.HeartAttackCivilian), typeof(Callouts.ArmoredPersonInVehicle), 
                    typeof(Callouts.StolenBoatTrailer), typeof(Callouts.DriveByInProgress)};
                foreach (Type callout in callouts)
                {
                    Logger.ToLog(string.Format("Registering {0} callout", callout.Name));
                    Functions.RegisterCallout(callout);
                    Logger.ToLog(string.Format("{0} has been registered successfully", callout.Name));
                }
                $"Adding Console Command".ToLog();
                Game.AddConsoleCommands(new[] { typeof(Commands) });
                Initialization.Initialize();
            }          
        }
    }
}
