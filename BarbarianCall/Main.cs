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
    public class Main : Plugin
    {
        public override void Finally()
        {
            foreach (Entity entity in World.GetAllEntities())
            {
                if (entity && entity.CreatedByTheCallingPlugin)
                {
                    if (entity.Metadata.BAR_Entity != null && entity.Metadata.BAR_Entity)
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
            "Unloaded successfully".ToLog();
        }

        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += Functions_OnOnDutyStateChanged;
            "BarbarianCall Has beeen loaded, go on duty to start the callouts".ToLog();
        }

        private void Functions_OnOnDutyStateChanged(bool onDuty)
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(Initialization.LSPDFRResolveEventHandler);
            Functions.RegisterCallout(typeof(Callouts.SuspiciousVehicle));
            Functions.RegisterCallout(typeof(Callouts.OfficerStabbed));
            Functions.RegisterCallout(typeof(Callouts.TaxiRefusePay));
            Functions.RegisterCallout(typeof(Callouts.WantedFelonOnTheLoose));
            Initialization.Initialize();
        }
    }
}
