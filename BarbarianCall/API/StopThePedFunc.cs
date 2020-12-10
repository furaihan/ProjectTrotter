using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StopThePed.API;
using StopThePed;

namespace BarbarianCall.API
{
    internal static class StopThePedFunc
    {
        public static void CallService(EStopThePedUnitServices serviceType)
        {
            switch (serviceType)
            {
                case EStopThePedUnitServices.Insurance: Functions.callInsuranceService(); break;
                case EStopThePedUnitServices.AnimalControl: Functions.callAnimalControl(); break;
                case EStopThePedUnitServices.Coroner: Functions.callCoroner(); break;
                case EStopThePedUnitServices.PrisonerTransport: Functions.callPoliceTransport(); break;
                case EStopThePedUnitServices.TowTruck: Functions.callTowService(); break;
                default: throw new NotSupportedException("Selected services is not supported by StopThePed");
            }
        }
        public static STPVehicleStatus GetVehicleRegistration(Rage.Vehicle veh) => Functions.getVehicleInsuranceStatus(veh);
        public static STPVehicleStatus GetVehicleInsurance(Rage.Vehicle veh) => Functions.getVehicleInsuranceStatus(veh);
        public static void SetVehicleRegistration(Rage.Vehicle veh, STPVehicleStatus status) => Functions.setVehicleRegistrationStatus(veh, status);
        public static void SetVehicleInsurance(Rage.Vehicle veh, STPVehicleStatus status) => Functions.setVehicleInsuranceStatus(veh, status);
        public static void SetPedUnderDrugsInfluence(Rage.Ped ped, bool set) => Functions.setPedUnderDrugsInfluence(ped, set);
        public static bool IsPedUnderDrugsInfluence(Rage.Ped ped) => Functions.isPedUnderDrugsInfluence(ped);
        public static void SetPedAlcoholLimit(Rage.Ped ped, bool set) => Functions.setPedAlcoholOverLimit(ped, set);
        public static bool IsPedAlcoholOverLimit(Rage.Ped ped) => Functions.isPedAlcoholOverLimit(ped);
        public static void RequestPedCheck(bool playingRadio) => Functions.requestDispatchPedCheck(playingRadio);
        public static void RequestVehicleCheck(bool playingRadio) => Functions.requestDispatchVehiclePlateCheck(playingRadio);
        public enum EStopThePedUnitServices
        {
            Insurance,
            AnimalControl,
            Coroner,
            PrisonerTransport,
            TowTruck,
        }
    }
}
