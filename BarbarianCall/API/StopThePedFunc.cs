using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Func = StopThePed.API.Functions;
using StopThePed.API;

namespace BarbarianCall.API
{
    internal static class StopThePedFunc
    {
        private const string DllPath = @"Plugins/LSPDFR/StopThePed.dll";
        private static readonly bool IsValid = File.Exists(DllPath);
        public static void CallService(EStopThePedUnitServices serviceType)
        {
            if (IsValid)
            {
                switch (serviceType)
                {
                    case EStopThePedUnitServices.Insurance: Func.callInsuranceService(); break;
                    case EStopThePedUnitServices.AnimalControl: Func.callAnimalControl(); break;
                    case EStopThePedUnitServices.Coroner: Func.callCoroner(); break;
                    case EStopThePedUnitServices.PoliceTransport: Func.callPoliceTransport(); break;
                    case EStopThePedUnitServices.TowTruck: Func.callTowService(); break;
                    default: throw new NotSupportedException("Selected services is not supported by StopThePed");
                }
            }            
        }
        public static STPVehicleStatus GetVehicleRegistration(Rage.Vehicle veh)
        {
            return Func.getVehicleInsuranceStatus(veh);
        }
        public delegate void VehicleEvent(Rage.Vehicle vehicle);
        public delegate void PedEvent(Rage.Ped ped);
        public delegate void STPEvent();
        public static event VehicleEvent OnVehicleSearch;
        public static STPVehicleStatus GetVehicleInsurance(Rage.Vehicle veh)
        {
            return Func.getVehicleInsuranceStatus(veh);
        }

        public static void SetVehicleRegistration(Rage.Vehicle veh, StopThePedVehicleStatus status)
        {
            switch (status)
            {
                case StopThePedVehicleStatus.None: Func.setVehicleRegistrationStatus(veh, STPVehicleStatus.None); break;
                case StopThePedVehicleStatus.Expired: Func.setVehicleRegistrationStatus(veh, STPVehicleStatus.Expired); break;
                case StopThePedVehicleStatus.Valid: Func.setVehicleRegistrationStatus(veh, STPVehicleStatus.Valid); break;
            }
        }
        public static void SetVehicleInsurance(Rage.Vehicle veh, StopThePedVehicleStatus status)
        {
            switch (status)
            {
                case StopThePedVehicleStatus.None: Func.setVehicleInsuranceStatus(veh, STPVehicleStatus.None); break;
                case StopThePedVehicleStatus.Expired: Func.setVehicleInsuranceStatus(veh, STPVehicleStatus.Expired); break;
                case StopThePedVehicleStatus.Valid: Func.setVehicleInsuranceStatus(veh, STPVehicleStatus.Valid); break;
            }
        }
        public static void RequestPitManuever()
        {
            if (IsValid) Func.requestPIT();
        }

        public static void SetPedUnderDrugsInfluence(Rage.Ped ped, bool set)
        {
            if (IsValid) Func.setPedUnderDrugsInfluence(ped, set);
        }
        public static bool? IsPedUnderDrugsInfluence(Rage.Ped ped)
        {
            return IsValid ? Func.isPedUnderDrugsInfluence(ped) : null;
        }

        public static void SetPedAlcoholOverLimit(Rage.Ped ped, bool set)
        {
            if (IsValid)  Func.setPedAlcoholOverLimit(ped, set);
        }

        public static bool? IsPedAlcoholOverLimit(Rage.Ped ped)
        {
            return IsValid ? Func.isPedAlcoholOverLimit(ped) : null;
        }

        public static void RequestPedCheck(bool playingRadio)
        {
            if (IsValid) Func.requestDispatchPedCheck(playingRadio);
        }

        public static void RequestVehicleCheck(bool playingRadio)
        {
            if (IsValid) Func.requestDispatchVehiclePlateCheck(playingRadio);
        }

        public static bool? IsPedStoppedWithSTP(Rage.Ped ped)
        {
            return IsValid ? Func.isPedStopped(ped) : null;
        }

        public static void InjectPedDangerousItem(Rage.Ped ped)
        {
            List<string> items = Globals.DangerousPedItem.GetRandomNumberOfElements(Peralatan.Random.Next(1, 3), true).ToList();
            items.Shuffle();
            ped.Metadata.searchPed = "~r~" + items.GetRandomElement() + "~s~";
            Func.injectPedSearchItems(ped);
        }
        public static void InjectPedItem(Rage.Ped ped, params string[] items)
        {
            InjectPedItem(ped, string.Join(", ", items));
        }

        public static void InjectPedItem(Rage.Ped ped, string item)
        {
            ped.Metadata.searchPed = item;
        }

        public static void InjectVehicleItem(Rage.Vehicle vehicle, EStopThePedVehicleSearch vehicleSearch, params string[] items)
        {
            InjectVehicleItem(vehicle, string.Join(", ", items), vehicleSearch);
        }

        public static void InjectVehicleItem(Rage.Vehicle vehicle, string item, EStopThePedVehicleSearch vehicleSearch)
        {
            if (IsValid)
            {
                switch (vehicleSearch)
                {
                    case EStopThePedVehicleSearch.SearchDriver: vehicle.Metadata.searchDriver = item; break;
                    case EStopThePedVehicleSearch.SearchPassenger: vehicle.Metadata.searchPassenger = item; break;
                    case EStopThePedVehicleSearch.SearchTrunk: vehicle.Metadata.searchTrunk = item; break;
                    default:
                        int rand = Peralatan.Random.Next(1, 4000);
                        if (rand < 2000) vehicle.Metadata.searchTrunk = item;
                        else if (rand < 3000) vehicle.Metadata.searchDriver = item;
                        else vehicle.Metadata.searchPassenger = item;
                        break;
                }
            }           
        }
        public static void SetPedGunPermit(Rage.Ped ped, bool hasGunPermit)
        {
            ped.Metadata.hasGunPermit = hasGunPermit;
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
            PoliceTransport,
            TowTruck,
        }
        public enum StopThePedVehicleStatus
        {
            None = 0,
            Expired = 1,
            Valid = 2,
        }
        private static bool eventAdded = false;
        static StopThePedFunc()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
            "Executing static wrapper class for STP".ToLog();
            if (IsValid && !eventAdded)
            {
                "Adding STPEvent".ToLog();
                Events.searchVehicleEvent += (v) => OnVehicleSearch?.Invoke(v);
                eventAdded = true;
            }
        }
        private static System.Reflection.Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.ToLower().StartsWith("stoptheped, ") && IsValid)
            {
                return System.Reflection.Assembly.Load(File.ReadAllBytes(DllPath));
            }
            return null;
        }
    }
}
