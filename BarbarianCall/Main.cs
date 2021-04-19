using LSPD_First_Response.Mod.API;
using Rage;
using System;
using System.Linq;

namespace BarbarianCall
{
    public class Main : Plugin
    {
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
            Functions.OnOnDutyStateChanged += Functions_OnOnDutyStateChanged;
            "BarbarianCall Has beeen loaded, go on duty to start the callouts".ToLog();
        }

        private void Functions_OnOnDutyStateChanged(bool onDuty)
        {
            if (onDuty)
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(Initialization.LSPDFRResolveEventHandler);
                Type[] callouts = { typeof(Callouts.SuspiciousVehicle), typeof(Callouts.OfficerStabbed), typeof(Callouts.TaxiRefusePay), typeof(Callouts.WantedFelonOnTheLoose), typeof(Callouts.MassStreetFighting) };
                foreach (Type callout in callouts)
                {
                    Peralatan.ToLog(string.Format("Loading {0} callout", callout.Name));
                    Functions.RegisterCallout(callout);
                    Peralatan.ToLog(string.Format("{0} has been loaded successfully", callout.Name));
                }
                Initialization.Initialize();
            }          
        }
    }
}
