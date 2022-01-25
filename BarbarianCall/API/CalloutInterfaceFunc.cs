using System;
using LSPD_First_Response.Mod.Callouts;
using Func = CalloutInterface.API.Functions;

namespace BarbarianCall.API
{
    internal static class CalloutInterfaceFunc
    {
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
    }
}
