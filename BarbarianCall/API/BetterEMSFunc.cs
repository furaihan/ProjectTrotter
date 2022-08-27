using BetterEMS.API;
using Rage;
using System.Collections.Generic;

namespace BarbarianCall.API
{
    internal static class BetterEMSFunc
    {
        public static void CallAmbulance(Vector3 location) => EMSFunctions.RespondToLocation(location);
        public static void RequestAmbulancePickup(Ped ped) 
        { 
            if (ped.Exists())
                EMSFunctions.PickUpPatient(ped);
        }
        public static uint GetOriginalDeathWeaponAssetHash(Ped p)
        {
            LinkedList<int> test = new LinkedList<int>();
            if (p && p.IsDead)
            {
                return EMSFunctions.GetOriginalDeathWeaponAsset(p).Hash;
            }
            else
            {
                return 0;
            }

        }
        public static void SetPedDeathDetails(Ped ped, string injuredName, string causeOfDeath, uint dethTime, float surviveProbability)
        {
            EMSFunctions.OverridePedDeathDetails(ped, injuredName, causeOfDeath, dethTime, surviveProbability);
        }

        public static bool HasBeenTreated(Ped p)
        {
            bool? result = EMSFunctions.DidEMSRevivePed(p);
            if (result.HasValue && result != null)
                return result.Value;
            return false;
        }
    }
}
