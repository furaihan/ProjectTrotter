using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Rage;
using LSPD_First_Response;

namespace BarbarianCall.API
{
    public static class Functions
    {
        public static void DisplayPedDetails()
        {
            $"Executing DisplayPedDetails. Request from {Assembly.GetCallingAssembly().GetName().FullName}".ToLog();
            if (Types.Manusia.CurrentManusia == null)
            {
                "Ped description is null".ToLog();
                return;
            }
            else if (!LSPD_First_Response.Mod.API.Functions.IsCalloutRunning())
            {
                Game.DisplayHelp("No callout running detected");
                return;
            }
            Types.Manusia.CurrentManusia.DisplayNotif();
        }
    }
}
