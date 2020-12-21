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
                EUltimateBackupUnitType.LocalPatrol => Functions.getLocalPatrolUnit(location, numPeds),
                EUltimateBackupUnitType.StatePatrol => Functions.getStatePatrolUnit(location, numPeds),
                EUltimateBackupUnitType.Ambulance => Functions.getAmbulanceUnit(location, numPeds),
                EUltimateBackupUnitType.Coroner => Functions.getCoronerUnit(location, numPeds),
                EUltimateBackupUnitType.Firetruk => Functions.getFireTruckUnit(location, numPeds),
                EUltimateBackupUnitType.LocalSwat => Functions.getLocalSWATUnit(location, numPeds),
                EUltimateBackupUnitType.NOOSESwat => Functions.getNooseSWATUnit(location, numPeds),
                EUltimateBackupUnitType.PoliceTransport => Functions.getPoliceTransportUnit(location, numPeds),
                _ => Functions.getLocalPatrolUnit(location, numPeds),
            };
        }
        public static Ped GetPed(EUltimateBackupUnitType unitType, Vector3 location, float heading)
        {
            return unitType switch
            {
                EUltimateBackupUnitType.LocalPatrol => Functions.getLocalPatrolPed(location, heading),
                EUltimateBackupUnitType.Ambulance => Functions.getAmbulancePed(location, heading),
                EUltimateBackupUnitType.StatePatrol => Functions.geStatePatrolPed(location, heading),
                _ => throw new NotSupportedException("selected unit type is not supported"),
            };
        }
        public static void CallUnit(EUltimateBackupCallType responseType) => CallUnit(responseType, false);
        public static void CallUnit(EUltimateBackupCallType responseType, bool stateBackup)
        {
            switch (responseType)
            {
                case EUltimateBackupCallType.Code2: Functions.callCode2Backup(false, stateBackup); break;
                case EUltimateBackupCallType.Code3: Functions.callCode3Backup(false, stateBackup); break;
                case EUltimateBackupCallType.Ambulance: Functions.callAmbulance(); break;
                case EUltimateBackupCallType.Firetruk: Functions.callFireDepartment(); break;
                case EUltimateBackupCallType.Pursuit: Functions.callPursuitBackup(false, stateBackup); break;
                case EUltimateBackupCallType.TrafficStop: Functions.callTrafficStopBackup(false, stateBackup); break;
                case EUltimateBackupCallType.FelonyStop: Functions.callFelonyStopBackup(false, stateBackup); break;
                case EUltimateBackupCallType.Panic: Functions.callPanicButtonBackup(false); break;
                case EUltimateBackupCallType.SpikeStrips: Functions.callSpikeStripsBackup(); break;
                case EUltimateBackupCallType.RoadBlock: Functions.callRoadBlockBackup(); break;
                case EUltimateBackupCallType.K9: Functions.callK9Backup(false, stateBackup); break;
                case EUltimateBackupCallType.Female: Functions.callFemaleBackup(false, stateBackup); break;
                default: Functions.callCode2Backup(); break;

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
