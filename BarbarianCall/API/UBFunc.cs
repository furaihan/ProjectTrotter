using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateBackup.API;
using Rage;

namespace BarbarianCall.API
{
    internal static class UBFunc
    {
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
        public static void CallUnit(EUltimateBackupResponseType responseType, bool stateBackup)
        {
            switch (responseType)
            {
                case EUltimateBackupResponseType.Code2: Functions.callCode2Backup(false, stateBackup); break;
                case EUltimateBackupResponseType.Code3: Functions.callCode3Backup(false, stateBackup); break;
                case EUltimateBackupResponseType.Ambulance: Functions.callAmbulance(); break;
                case EUltimateBackupResponseType.Firetruk: Functions.callFireDepartment(); break;
                case EUltimateBackupResponseType.Pursuit: Functions.callPursuitBackup(false, stateBackup); break;
                case EUltimateBackupResponseType.TrafficStop: Functions.callTrafficStopBackup(false, stateBackup); break;
                case EUltimateBackupResponseType.FelonyStop: Functions.callFelonyStopBackup(false, stateBackup); break;
                case EUltimateBackupResponseType.Panic: Functions.callPanicButtonBackup(false); break;
                case EUltimateBackupResponseType.SpikeStrips: Functions.callSpikeStripsBackup(); break;
                case EUltimateBackupResponseType.RoadBlock: Functions.callRoadBlockBackup(); break;
                case EUltimateBackupResponseType.K9: Functions.callK9Backup(false, stateBackup); break;
                case EUltimateBackupResponseType.Female: Functions.callFemaleBackup(false, stateBackup); break;
                default: Functions.callCode2Backup(); break;

            }
        }
        public static void CallUnit(EUltimateBackupResponseType responseType) => CallUnit(responseType, false);

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
        public enum EUltimateBackupResponseType
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
