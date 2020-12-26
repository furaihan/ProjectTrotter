using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateBackup.API;
using Rage;

namespace BarbarianCall.API
{
    internal static class UltimateBackupFunc
    {
        public static Tuple<Vehicle, Ped> GetUnit(EUltimateBackupUnitType unitType, Vector3 location)
        {
            var ub = GetUnit(unitType, location, 1);
            Tuple<Vehicle, Ped> ret = new Tuple<Vehicle, Ped>(ub.Item1, ub.Item2[0]);
            return ret;
        }
        public static Tuple<Vehicle, List<Ped>> GetUnit(EUltimateBackupUnitType backupType , Vector3 location, int numPeds)
        {
            return backupType switch
            {
                EUltimateBackupUnitType.LocalPatrol => UltimateBackup.API.Functions.getLocalPatrolUnit(location, numPeds),
                EUltimateBackupUnitType.StatePatrol => UltimateBackup.API.Functions.getStatePatrolUnit(location, numPeds),
                EUltimateBackupUnitType.Ambulance => UltimateBackup.API.Functions.getAmbulanceUnit(location, numPeds),
                EUltimateBackupUnitType.Coroner => UltimateBackup.API.Functions.getCoronerUnit(location, numPeds),
                EUltimateBackupUnitType.Firetruk => UltimateBackup.API.Functions.getFireTruckUnit(location, numPeds),
                EUltimateBackupUnitType.LocalSwat => UltimateBackup.API.Functions.getLocalSWATUnit(location, numPeds),
                EUltimateBackupUnitType.NOOSESwat => UltimateBackup.API.Functions.getNooseSWATUnit(location, numPeds),
                EUltimateBackupUnitType.PoliceTransport => UltimateBackup.API.Functions.getPoliceTransportUnit(location, numPeds),
                _ => UltimateBackup.API.Functions.getLocalPatrolUnit(location, numPeds),
            };
        }
        public static Ped GetPed(EUltimateBackupUnitType unitType, Vector3 location, float heading)
        {
            return unitType switch
            {
                EUltimateBackupUnitType.LocalPatrol => UltimateBackup.API.Functions.getLocalPatrolPed(location, heading),
                EUltimateBackupUnitType.Ambulance => UltimateBackup.API.Functions.getAmbulancePed(location, heading),
                EUltimateBackupUnitType.StatePatrol => UltimateBackup.API.Functions.geStatePatrolPed(location, heading),
                _ => throw new NotSupportedException("selected unit type is not supported"),
            };
        }
        public static void CallUnit(EUltimateBackupCallType responseType) => CallUnit(responseType, false);
        public static void CallUnit(EUltimateBackupCallType responseType, bool stateBackup)
        {
            switch (responseType)
            {
                case EUltimateBackupCallType.Code2: UltimateBackup.API.Functions.callCode2Backup(false, stateBackup); break;
                case EUltimateBackupCallType.Code3: UltimateBackup.API.Functions.callCode3Backup(false, stateBackup); break;
                case EUltimateBackupCallType.Ambulance: UltimateBackup.API.Functions.callAmbulance(); break;
                case EUltimateBackupCallType.Firetruk: UltimateBackup.API.Functions.callFireDepartment(); break;
                case EUltimateBackupCallType.Pursuit: UltimateBackup.API.Functions.callPursuitBackup(false, stateBackup); break;
                case EUltimateBackupCallType.TrafficStop: UltimateBackup.API.Functions.callTrafficStopBackup(false, stateBackup); break;
                case EUltimateBackupCallType.FelonyStop: UltimateBackup.API.Functions.callFelonyStopBackup(false, stateBackup); break;
                case EUltimateBackupCallType.Panic: UltimateBackup.API.Functions.callPanicButtonBackup(false); break;
                case EUltimateBackupCallType.SpikeStrips: UltimateBackup.API.Functions.callSpikeStripsBackup(); break;
                case EUltimateBackupCallType.RoadBlock: UltimateBackup.API.Functions.callRoadBlockBackup(); break;
                case EUltimateBackupCallType.K9: UltimateBackup.API.Functions.callK9Backup(false, stateBackup); break;
                case EUltimateBackupCallType.Female: UltimateBackup.API.Functions.callFemaleBackup(false, stateBackup); break;
                default: UltimateBackup.API.Functions.callCode2Backup(); break;

            }
        }
        public enum EUltimateBackupUnitType
        {
            LocalPatrol,
            StatePatrol,
            Ambulance,
            Coroner,
            Firetruk,
            LocalSwat,
            NOOSESwat,
            PoliceTransport,
        }
        public enum EUltimateBackupCallType
        {
            Code2,
            Code3,
            Ambulance,
            Firetruk,
            Pursuit,
            TrafficStop,
            FelonyStop,
            Panic,
            SpikeStrips,
            RoadBlock,
            K9,
            Female,
        }
    }
}
