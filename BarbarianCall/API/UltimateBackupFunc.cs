using System;
using System.Collections.Generic;
using Func = UltimateBackup.API.Functions;
using Rage;

namespace BarbarianCall.API
{
    internal static class UltimateBackupFunc
    {
        public static Tuple<Vehicle, Ped> GetUnit(EUltimateBackupUnitType unitType, Vector3 location)
        {
            Tuple<Vehicle, List<Ped>> ub = GetUnit(unitType, location, 1);
            Tuple<Vehicle, Ped> ret = new(ub.Item1, ub.Item2[0]);
            return ret;
        }
        public static Tuple<Vehicle, List<Ped>> GetUnit(EUltimateBackupUnitType backupType , Vector3 location, int numPeds)
        {
            return backupType switch
            {
                EUltimateBackupUnitType.LocalPatrol => Func.getLocalPatrolUnit(location, numPeds),
                EUltimateBackupUnitType.StatePatrol => Func.getStatePatrolUnit(location, numPeds),
                EUltimateBackupUnitType.Ambulance => Func.getAmbulanceUnit(location, numPeds),
                EUltimateBackupUnitType.Coroner => Func.getCoronerUnit(location, numPeds),
                EUltimateBackupUnitType.Firetruk => Func.getFireTruckUnit(location, numPeds),
                EUltimateBackupUnitType.LocalSwat => Func.getLocalSWATUnit(location, numPeds),
                EUltimateBackupUnitType.NOOSESwat => Func.getNooseSWATUnit(location, numPeds),
                EUltimateBackupUnitType.PoliceTransport => Func.getPoliceTransportUnit(location, numPeds),
                _ => Func.getLocalPatrolUnit(location, numPeds),
            };
        }
        public static Ped GetPed(EUltimateBackupUnitType unitType, Vector3 location, float heading)
        {
            return unitType switch
            {
                EUltimateBackupUnitType.LocalPatrol => Func.getLocalPatrolPed(location, heading),
                EUltimateBackupUnitType.Ambulance => Func.getAmbulancePed(location, heading),
                EUltimateBackupUnitType.StatePatrol => Func.geStatePatrolPed(location, heading),
                _ => throw new NotSupportedException("selected unit type is not supported"),
            };
        }
        public static void CallUnitBackup(EUltimateBackupCallType responseType) => CallUnitBackup(responseType, false);
        public static void CallUnitBackup(EUltimateBackupCallType responseType, bool stateBackup)
        {
            switch (responseType)
            {
                case EUltimateBackupCallType.Code2: Func.callCode2Backup(false, stateBackup); break;
                case EUltimateBackupCallType.Code3: Func.callCode3Backup(false, stateBackup); break;
                case EUltimateBackupCallType.Ambulance: Func.callAmbulance(); break;
                case EUltimateBackupCallType.Firetruk: Func.callFireDepartment(); break;
                case EUltimateBackupCallType.Pursuit: Func.callPursuitBackup(false, stateBackup); break;
                case EUltimateBackupCallType.TrafficStop: Func.callTrafficStopBackup(false, stateBackup); break;
                case EUltimateBackupCallType.FelonyStop: Func.callFelonyStopBackup(false, stateBackup); break;
                case EUltimateBackupCallType.Panic: Func.callPanicButtonBackup(false); break;
                case EUltimateBackupCallType.SpikeStrips: Func.callSpikeStripsBackup(); break;
                case EUltimateBackupCallType.RoadBlock: Func.callRoadBlockBackup(); break;
                case EUltimateBackupCallType.K9: Func.callK9Backup(false, stateBackup); break;
                case EUltimateBackupCallType.Female: Func.callFemaleBackup(false, stateBackup); break;
                case EUltimateBackupCallType.Swat: Func.callCode3SwatBackup(false, false); break;
                case EUltimateBackupCallType.Noose: Func.callCode3SwatBackup(false, true); break;
                default: Func.callCode2Backup(); break;

            }
        }
        public static void DissmissAllBackup() => Func.dismissAllBackupUnits();        
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
        Swat,
        Noose
    }
}
