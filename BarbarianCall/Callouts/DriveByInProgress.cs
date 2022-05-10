using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbarianCall.Types;
using BarbarianCall.Extensions;
using BarbarianCall.API;
using Rage;
using Rage.Native;
using L = LSPD_First_Response.Mod.API.Functions;
using LSPD_First_Response.Mod.Callouts;

namespace BarbarianCall.Callouts
{
    [CalloutInfo("Drive-By In Progress", CalloutProbability.High)]
    public class DriveByInProgress : CalloutBase
    {
        /*
         * 1 = 
         * 2 = 
         * 3 = 
         * 4 = 
         * 5 = 
         */
        private int status = 0;
        private int involved = 0;
        private int deadCount = 0;
        private int arrestedCount = 0;
        private int escapedCount = 0;
        private List<Ped> suspectInAction = new List<Ped>();
        private List<Ped> allSuspect = new List<Ped>();
        private Model model1;
        private Model model2;
        private Ped driver1;
        private Ped driver2;
        private Ped pilot;
        private Spawnpoint spawn2;
        public bool CanEnd = false;
        private List<Model> Gang1Model = new List<Model>();
        private List<Model> Gang2Model = new List<Model>();
        private Vehicle veh1;
        private Vehicle veh2;
        private Vehicle chasingHeli;
        private Blip searchingBlip;
        private Vector3 lastPositionRevealed;
        private int updatePositionTime;
        public override bool OnBeforeCalloutDisplayed()
        {
            var carModelPool = new List<Model>(World.EnumerateVehicles().Where(x=> x.IsCar && !x.IsBig && !x.Model.IsBigVehicle && 
            !x.Model.IsLawEnforcementVehicle && x.Model.NumberOfSeats >= 4).OrderByDescending(x=> x.TopSpeed).Select(x=> x.Model).Distinct().ToList());
            if (carModelPool.Count < 3)
            {
                $"{GetType().Name} | Cannot find suitable car model".ToLog();
                return false;
            }
            int rnd = Peralatan.Random.Next(2, carModelPool.Count);
            model1 = carModelPool[rnd];
            model2 = carModelPool[rnd - 2];
            $"Car Model: {model1.Name} & {model2.Name}".ToLog();
            Spawn = SpawnManager.GetVehicleSpawnPoint5(PlayerPed.Position, 900, 985);
            if (Spawn == Spawnpoint.Zero) Spawn = SpawnManager.GetVehicleSpawnPoint5(PlayerPed.Position, 425, 825);
            if (Spawn == Spawnpoint.Zero) Spawn = SpawnManager.GetVehicleSpawnPoint5(PlayerPed.Position, 420, 850);
            if (Spawn == Spawnpoint.Zero) Spawn = SpawnManager.GetVehicleSpawnPoint5(PlayerPed.Position, 325, 875);
            if (Spawn == Spawnpoint.Zero)
            {
                $"{GetType().Name} | Spawnpoint is not found, cancelling the callout".ToLog();
                return false;
            }
            var tempSp = World.GetNextPositionOnStreet(Spawn.Position.Around2D(25f));
            var tempH = SpawnManager.GetRoadHeading(tempSp);
            spawn2 = new Spawnpoint(tempSp, tempH);
            Gang1Model = new List<Model>(Globals.GangPedModels.Values.ToList().GetRandomElement());
            Gang2Model = new List<Model>(Globals.GangPedModels.Values.ToList().GetRandomElement(m=> m != Gang1Model));
            Gang1Model.ForEach(x => x.LoadAndWait());
            Gang2Model.ForEach(x => x.LoadAndWait());
            Position = Spawn;
            SpawnHeading = Spawn;
            CalloutAdvisory = "Shots Fired!";
            CalloutMessage = "Drive-By In Progress";
            FriendlyName = "Drive-By In Progress";
            PlayScannerWithCallsign("WE_HAVE CRIME_GUNFIRE IN_OR_ON_POSITION", Spawn);
            ShowCalloutAreaBlipBeforeAccepting(Spawn.Position, 80);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            veh1 = new Vehicle(model1, Spawn, Spawn);
            veh2 = new Vehicle(model2, spawn2, spawn2);
            veh1.MakePersistent();
            veh2.MakePersistent();
            veh1.RandomiseLicensePlate();
            veh2.RandomiseLicensePlate();
            SetHealth(veh1, 5000);
            SetHealth(veh2, 5000);        
            FillVehicle(veh1, Gang1Model, RelationshipGroup.Gang1);
            FillVehicle(veh2, Gang2Model, RelationshipGroup.Gang2);
            uint[] heliModelsToChooseFrom = { 0x2F03547B, 0x2C75F0DD, 0x1517D4D9 };
            chasingHeli = new Vehicle(heliModelsToChooseFrom.GetRandomElement(), (Spawn.Position + new Vector3(0, 0, 650)).Around2D(Peralatan.Random.Next(350, 800)));
            pilot = chasingHeli.CreateRandomDriver();
            chasingHeli.MakePersistent();
            RelationshipGroup.Gang1.SetRelationshipWith(RelationshipGroup.Gang2, Relationship.Hate);
            RelationshipGroup.Gang1.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
            RelationshipGroup.Gang1.SetRelationshipWith(PlayerPed.RelationshipGroup, Relationship.Hate);
            RelationshipGroup.Gang2.SetRelationshipWith(RelationshipGroup.Gang1, Relationship.Hate);
            RelationshipGroup.Gang2.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
            RelationshipGroup.Gang2.SetRelationshipWith(PlayerPed.RelationshipGroup, Relationship.Hate);
            chasingHeli.Driver.MakeMissionPed();
            chasingHeli.Driver.Tasks.PerformDrivingManeuver(VehicleManeuver.Wait);
            SetHealth(chasingHeli, 5500);
            chasingHeli.IsBulletProof = true;
            L.SetPedAsCop(chasingHeli.Driver);
            L.SetCopAsBusy(chasingHeli.Driver, true);
            GameFiber.Sleep(2000);
            searchingBlip = new Blip(veh1.Position.Around2D(50f), 125f);
            searchingBlip.Color = Yellow;
            searchingBlip.Alpha = 0.80f;
            searchingBlip.EnableRoute(Yellow);
            CalloutBlips.Add(searchingBlip);
            driver1 = allSuspect[0];
            driver2 = allSuspect[veh1.Model.NumberOfSeats];
            CalloutEntities.AddRange(new Entity[] { veh1, veh2, chasingHeli, pilot });
            lastPositionRevealed = veh1.Position;
            updatePositionTime = Globals.GameTimer;
            suspectInAction = new List<Ped>(allSuspect);
            LogikaInformatika();
            CalloutMainFiber.Start();
            return base.OnCalloutAccepted();
        }
        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }
        public override void Process()
        {
            base.Process();
        }
        public override void End()
        {
            Gang1Model.ForEach(x => x.Dismiss());
            Gang2Model.ForEach(x => x.Dismiss());
            model1.Dismiss();
            model2.Dismiss();
            Gang1Model.Clear();
            Gang2Model.Clear();
            base.End();
        }
        public void LogikaInformatika()
        {
            CalloutMainFiber = new GameFiber(() =>
            {
                $"Callout main fiber started".ToLog();
                GameFiber.WaitUntil(() => allSuspect.All(x => x.Exists()));
                GameFiber.Sleep(5000);
                $"All Suspect prepared".ToLog();
                veh1.PlaceOnGroundProperly();
                veh2.PlaceOnGroundProperly();
                Tasker();
                var statusLog = status;
                var previousStatus = status;
                try
                {
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        switch (status)
                        {
                            case 0:
                                $"Started callout logic. {veh1.TopSpeed}.{veh2.TopSpeed}".ToLog();
                                SendCIMessage($"2 Vehicles Involved");
                                SendCIMessage($"First one is {veh1.GetDisplayName()} - {veh1.LicensePlate} with {veh1.Occupants.Length} occupants");
                                SendCIMessage($"Second one is {veh2.GetDisplayName()} - {veh2.LicensePlate} with {veh2.Occupants.Length} occupants");
                                status = 1;
                                break;
                            case 1:
                                var dis1 = PlayerPed.DistanceToSquared(veh1);
                                var dis2 = PlayerPed.DistanceToSquared(veh2);
                                var closest = dis1 < dis2 ? veh1 : veh2;
                                if (PlayerPed.DistanceToSquared(closest) < 625f)
                                {
                                    status = 2;
                                    break;
                                }
                                if ((Globals.GameTimer - updatePositionTime) > 50000 && veh1.DistanceToSquared(lastPositionRevealed) > 10000)
                                    status = 0b111;
                                bool taskMonitor = veh1.GetActiveMissionType() == MissionType.GoTo && veh2.GetActiveMissionType() == MissionType.Escort &&
                                chasingHeli.GetActiveMissionType() == MissionType.Escort;
                                if (!taskMonitor)
                                {
                                    previousStatus = 1;
                                    status = 5;
                                }
                                break;
                            case 2:
                                if (searchingBlip) searchingBlip.Delete();
                                SendCIMessage("Suspect spotted, initiating pursuit");
                                Pursuit = L.CreatePursuit();
                                foreach (Ped ped in allSuspect)
                                {
                                    L.AddPedToPursuit(Pursuit, ped);
                                    L.SetPursuitDisableAIForPed(ped, true);
                                }
                                L.SetPursuitIsActiveForPlayer(Pursuit, true);
                                L.SetPursuitAsCalledIn(Pursuit);
                                L.AddCopToPursuit(Pursuit, chasingHeli.Driver);
                                L.RequestBackup(veh1.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.SwatTeam);
                                SendCIMessage($"Unit {Peralatan.GetRandomUnitNumber()} is dispatching");
                                GameFiber.StartNew(() =>
                                {
                                    GameFiber.Wait(Peralatan.Random.Next(5000, 12500));
                                    if (CalloutRunning)
                                    {
                                        L.RequestBackup(veh1.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.SwatTeam);
                                        SendCIMessage($"Unit {Peralatan.GetRandomUnitNumber()} is dispatching");
                                    }
                                    GameFiber.Wait(Peralatan.Random.Next(3000, 5500));
                                    if (Peralatan.Random.Next(2) == 1)
                                    {
                                        if (CalloutRunning)
                                        {
                                            L.RequestBackup(veh1.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.SwatTeam);
                                            SendCIMessage($"Unit {Peralatan.GetRandomUnitNumber()} is dispatching");
                                        }
                                        GameFiber.Wait(3000);
                                        if (Peralatan.Random.Next(5) == 3 && CalloutRunning)
                                        {
                                            L.RequestBackup(veh1.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.SwatTeam);
                                            SendCIMessage($"Unit {Peralatan.GetRandomUnitNumber()} is dispatching");
                                        }
                                    }
                                });
                                status = 3;
                                break;
                            case 3:
                                foreach (Ped ped in allSuspect)
                                {
                                    if (ped)
                                    {
                                        if (!suspectInAction.Contains(ped)) continue;
                                        if (ped.IsDead)
                                        {
                                            deadCount++;
                                            suspectInAction.Remove(ped);
                                        }
                                        else if (L.IsPedArrested(ped))
                                        {
                                            arrestedCount++;
                                            suspectInAction.Remove(ped);
                                        }
                                        else if (!ped.IsValid())
                                        {
                                            escapedCount++;
                                            suspectInAction.Remove(ped);
                                        }
                                    }
                                    if (!suspectInAction.Any()) status = 4;
                                }
                                taskMonitor = veh1.GetActiveMissionType() == MissionType.GoTo && veh2.GetActiveMissionType() == MissionType.Escort &&
                                chasingHeli.GetActiveMissionType() == MissionType.Escort;
                                if (!taskMonitor)
                                {
                                    previousStatus = 3;
                                    status = 5;
                                }
                                break;
                            case 4:
                                DisplaySummary();
                                status = -1;
                                break;
                            case 5:
                                bool idle = true;
                                if (NeedToReassignDriveTask(driver1, veh1, MissionType.GoTo))
                                {
                                    $"Reassign task to the driver of the first vehicle. {veh1.GetActiveMissionType()}".ToLogDebug();
                                    driver1.VehicleMission(veh1, GetGotoPoint(), MissionType.GoTo, 50f, Globals.Sheeesh, 1000f, 5f, true);
                                    idle = false;
                                }
                                if (NeedToReassignDriveTask(driver2, veh2, MissionType.Escort))
                                {
                                    $"Reassign task to the driver of the second vehicle. {veh2.GetActiveMissionType()}".ToLogDebug();
                                    driver2.VehicleMission(veh2, veh1, MissionType.Escort, veh2.TopSpeed, Globals.Sheeesh, 10f, 5f, true);
                                    idle = false;
                                }
                                if (NeedToReassignDriveTask(pilot, chasingHeli, MissionType.Escort))
                                {
                                    $"Reassign task to the pilot. {chasingHeli.GetActiveMissionType()}".ToLogDebug();
                                    pilot.HeliMission(chasingHeli, veh1, null, Vector3.Zero, MissionType.Escort, 50f, 80f, -1.0f, 50, 30, 0);
                                    idle = false;
                                }
                                if (!idle)
                                {
                                    GameFiber.Sleep(500);
                                }
                                status = previousStatus;
                                break;
                            case 6:
                                foreach (Ped ped in veh1.Passengers.Concat(veh2.Passengers))
                                {
                                    ped.CombatAgainstHatedTargetAroundPed(350f);
                                }
                                status = previousStatus;
                                break;
                            case 7:
                                {
                                    L.PlayScannerAudioUsingPosition(string.Format("SUSPECT_HEADING {0} IN_OR_ON_POSITION",
                                        veh1.GetCardinalDirectionLowDetailedAudio()), veh1.Position);
                                    searchingBlip.Position = veh1.Position.Around2D(50f);
                                    searchingBlip.Color = Yellow;
                                    searchingBlip.Alpha = 0.80f;
                                    searchingBlip.EnableRoute(Yellow);
                                    var speed = MathHelper.ConvertMetersPerSecondToKilometersPerHourRounded((veh1.Speed + veh2.Speed) / 2);
                                    SendCIMessage($"Suspect last seen: {L.GetZoneAtPosition(veh1.Position).RealAreaName}. Speed is: {speed}");
                                    updatePositionTime = Globals.GameTimer;
                                    lastPositionRevealed = veh1.Position;
                                    status = 1;
                                    break;
                                }
                        }
                        if (statusLog != status)
                        {
                            $"Switching to case {status}".ToLogDebug();
                            statusLog = status;
                        }
                        if (status == -1) break;
                    }
                }
                catch (Exception e)
                {
                    $"{GetType().Name} callout crashes".ToLog();
                    e.ToString().ToLog();
                    NetExtension.SendError(e);
                    $"{GetType().Name} callout crashed, please send your log".DisplayNotifWithLogo("Mass Street Fighting");
                    End($"exception: {e.Message}");
                }              
                End();
            });
        }
        protected override void CleanUp()
        {
            // TODO: implement CleanUp()
        }
        private void DisplaySummary()
        {
            $"Suspect Count~s~: {involved}~n~~g~Arrested~s~: {arrestedCount}~n~~r~Dead~s~: {deadCount}~n~~o~Escaped~s~: {escapedCount}".
                DisplayNotifWithLogo("Mass Street Fighting", hudColor: RAGENativeUI.HudColor.GreenDark);
        }

        void FillVehicle(Vehicle vehicle, List<Model> models, RelationshipGroup relationshipGroup)
        {
            uint[] drivebyWeapon = { 0x1B06D571, 0xBFE256D4, 0x5EF9FEC4, 0x22D8FE39, 0x99AEEB3B, 0xBFD21232, 0xD205520E, 0x83839C4, 0x2B5EF5EC, 0x13532244, 0xDB1AA450, 0xBD248B55 };
            if (vehicle)
            {
                for (int i = -1; i < vehicle.Model.NumberOfSeats - 1; i++)
                {
                    Model model = models.GetRandomElement();
#if DEBUG
                    $"Model: {model.Name}".ToLog();
#endif
                    Ped ped = new Ped(model, Spawn.Position, Spawn.Heading);
                    ped.MakeMissionPed();
                    ped.RelationshipGroup = relationshipGroup;
                    ped.MaxHealth = 750;
                    ped.Health = 750;
                    ped.Inventory.GiveNewWeapon(drivebyWeapon.GetRandomElement(), -1, true);
                    ped.GetCombatProperty().CanDoDrivebys = true;
                    ped.GetCombatProperty().CanLeaveVehicle = false;
                    involved++;
                    allSuspect.Add(ped);
                    CalloutEntities.Add(ped);
                    NativeFunction.Natives.TASK_WARP_PED_INTO_VEHICLE(ped, vehicle, i);
                }
            }
        }
        void Tasker()
        {
            GameFiber.Sleep(500);
            driver1.VehicleMission(veh1, GetGotoPoint(), MissionType.GoTo, veh1.TopSpeed, Globals.Sheeesh, 1000f, 5f, true);
            driver2.VehicleMission(veh2, veh1, MissionType.Escort, veh2.TopSpeed, Globals.Sheeesh, 10f, 5f, true);
            pilot.HeliMission(chasingHeli, veh1, null, Vector3.Zero, MissionType.Escort, 50f, 80f, -1.0f, 50, 30, 0);
            foreach (Ped ped in veh1.Passengers.Concat(veh2.Passengers))
            {
                ped.CombatAgainstHatedTargetAroundPed(350f);
            }
        }
        void SetHealth(Vehicle vehicle, int amount)
        {
            if (vehicle)
            {
                vehicle.MaxHealth = amount;
                vehicle.Health = amount;
                vehicle.EngineHealth = amount;
                vehicle.FuelTankHealth = amount;
            }
        }
        private const string path = @"Plugins\LSPDFR\BarbarianCall\Locations\TrafficStop.xml";
        private Spawnpoint GetGotoPoint()
        {
            List<Spawnpoint> spawnpoints = DivisiXml.Deserialization.GetSpawnPointFromXml(path);
            Spawnpoint ret = spawnpoints.OrderByDescending(x => x.Position.DistanceToSquared(veh1.Position)).Take(4).GetRandomElement();
            $"Docks located in {L.GetZoneAtPosition(ret.Position).RealAreaName} {World.GetStreetName(ret.Position)}".ToLog();
            spawnpoints.Clear();
            return ret;
        }
        private bool NeedToReassignDriveTask(Ped ped, Vehicle vehicle, MissionType missionType) => ped && ped.IsAlive && vehicle && vehicle.IsAlive 
            && vehicle.GetActiveMissionType() != missionType;
    }
}
