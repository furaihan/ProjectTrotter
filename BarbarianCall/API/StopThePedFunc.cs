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
                case EStopThePedUnitServices.Insurance: StopThePed.API.Functions.callInsuranceService(); break;
                case EStopThePedUnitServices.AnimalControl: StopThePed.API.Functions.callAnimalControl(); break;
                case EStopThePedUnitServices.Coroner: StopThePed.API.Functions.callCoroner(); break;
                case EStopThePedUnitServices.PrisonerTransport: StopThePed.API.Functions.callPoliceTransport(); break;
                case EStopThePedUnitServices.TowTruck: StopThePed.API.Functions.callTowService(); break;
                default: throw new NotSupportedException("Selected services is not supported by StopThePed");
            }
        }
        public static STPVehicleStatus GetVehicleRegistration(Rage.Vehicle veh) => StopThePed.API.Functions.getVehicleInsuranceStatus(veh);
        public static STPVehicleStatus GetVehicleInsurance(Rage.Vehicle veh) => StopThePed.API.Functions.getVehicleInsuranceStatus(veh);
        public static void SetVehicleRegistration(Rage.Vehicle veh, STPVehicleStatus status) => StopThePed.API.Functions.setVehicleRegistrationStatus(veh, status);
        public static void SetVehicleInsurance(Rage.Vehicle veh, STPVehicleStatus status) => StopThePed.API.Functions.setVehicleInsuranceStatus(veh, status);
        public static void SetPedUnderDrugsInfluence(Rage.Ped ped, bool set) => StopThePed.API.Functions.setPedUnderDrugsInfluence(ped, set);
        public static bool IsPedUnderDrugsInfluence(Rage.Ped ped) => StopThePed.API.Functions.isPedUnderDrugsInfluence(ped);
        public static void SetPedAlcoholOverLimit(Rage.Ped ped, bool set) => StopThePed.API.Functions.setPedAlcoholOverLimit(ped, set);
        public static bool IsPedAlcoholOverLimit(Rage.Ped ped) => StopThePed.API.Functions.isPedAlcoholOverLimit(ped);
        public static void RequestPedCheck(bool playingRadio) => StopThePed.API.Functions.requestDispatchPedCheck(playingRadio);
        public static void RequestVehicleCheck(bool playingRadio) => StopThePed.API.Functions.requestDispatchVehiclePlateCheck(playingRadio);
        public static bool IsPedStoppedWithSTP(Rage.Ped ped) => StopThePed.API.Functions.isPedStopped(ped);
        public static void InjectPedDangerousItem(Rage.Ped ped)
        {
            var items = CommonVariables.DangerousPedItem.GetRandomNumberOfElements(Peralatan.Random.Next(1, 3), true).ToList();
            items.Shuffle();
            ped.Metadata.searchPed = "~r~" + items.GetRandomElement() + "~s~";
            StopThePed.API.Functions.injectPedSearchItems(ped);
        }
        public static void InjectPedItem(Rage.Ped ped, string item) => ped.Metadata.searchPed = item;
        public static void InjectVehicleItem(Rage.Vehicle vehicle, string item, EStopThePedVehicleSearch vehicleSearch)
        {
            switch (vehicleSearch)
            {
                case EStopThePedVehicleSearch.SearchDriver: vehicle.Metadata.searchDriver = item; break;
                case EStopThePedVehicleSearch.SearchPassenger: vehicle.Metadata.searchPassenger = item; break;
                case EStopThePedVehicleSearch.SearchTrunk: vehicle.Metadata.searchTrunk = item; break;
            }
        }

        public enum EStopThePedVehicleSearch
        {
            SearchTrunk,
            SearchDriver,
            SearchPassenger
        }
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
