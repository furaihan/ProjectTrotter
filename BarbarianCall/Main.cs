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
            Functions.RegisterCallout(typeof(Callouts.StrangeLookingVehicle));
            Initialization.Initialize();
        }
    }
}
