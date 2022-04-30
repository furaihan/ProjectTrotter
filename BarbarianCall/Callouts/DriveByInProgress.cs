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
        private Model model1;
        private Model model2;
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
            var carModelPool = World.EnumerateVehicles().Where(x=> x.IsCar && x.Model.NumberOfSeats >= 4).Select(x=> x.Model).ToList();
            if (!carModelPool.Any())
            {
                $"{GetType().Name} | Cannot find suitable car model".ToLog();
                return false;
            }
            model1 = carModelPool.GetRandomElement();
            model2 = carModelPool.GetRandomElement();
            Spawn = SpawnManager.GetVehicleSpawnPoint5(PlayerPed.Position, 425, 725, true);
            if (Spawn == Spawnpoint.Zero) Spawn = SpawnManager.GetVehicleSpawnPoint5(PlayerPed.Position, 425, 825);
            if (Spawn == Spawnpoint.Zero) Spawn = SpawnManager.GetVehicleSpawnPoint5(PlayerPed.Position, 420, 850);
            if (Spawn == Spawnpoint.Zero) Spawn = SpawnManager.GetVehicleSpawnPoint5(PlayerPed.Position, 325, 875);
            if (Spawn == Spawnpoint.Zero)
            {
                $"{GetType().Name} | Spawnpoint is not found, cancelling the callout".ToLog();
                return false;
            }
            var tempSp = World.GetNextPositionOnStreet(Spawn.Position);
            var tempH = SpawnManager.GetRoadHeading(tempSp);
            spawn2 = new Spawnpoint(tempSp, tempH);
            Gang1Model = Globals.GangPedModels.Values.ToList().GetRandomElement();
            Gang2Model = Globals.GangPedModels.Values.ToList().GetRandomElement(m=> m != Gang1Model);
            Position = Spawn;
            SpawnHeading = Spawn;
            CalloutAdvisory = "";
            CalloutMessage = "Drive-By In Progress";
            FriendlyName = "Drive-By In Progress";
            PlayScannerWithCallsign("WE_HAVE CRIME_GUNFIRE IN_OR_ON_POSITION", Spawn);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            veh1 = new Vehicle(model1, Spawn, Spawn);
            veh2 = new Vehicle(model2, spawn2, spawn2);
            FillVehicle(veh1, Gang1Model, RelationshipGroup.Gang1);
            FillVehicle(veh2, Gang2Model, RelationshipGroup.Gang2);
            uint[] heliModelsToChooseFrom = { 0x2F03547B, 0x2C75F0DD, 0x1517D4D9 };
            chasingHeli = new Vehicle(heliModelsToChooseFrom.GetRandomElement(), (Spawn.Position + new Vector3(0, 0, 650)).Around2D(Peralatan.Random.Next(350, 500)));
            chasingHeli.CreateRandomDriver();
            RelationshipGroup.Gang1.SetRelationshipWith(RelationshipGroup.Gang2, Relationship.Hate);
            RelationshipGroup.Gang1.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
            RelationshipGroup.Gang1.SetRelationshipWith(PlayerPed.RelationshipGroup, Relationship.Hate);
            RelationshipGroup.Gang2.SetRelationshipWith(RelationshipGroup.Gang1, Relationship.Hate);
            RelationshipGroup.Gang2.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
            RelationshipGroup.Gang2.SetRelationshipWith(PlayerPed.RelationshipGroup, Relationship.Hate);
            chasingHeli.Driver.MakeMissionPed();
            chasingHeli.Driver.Tasks.PerformDrivingManeuver(VehicleManeuver.Wait);
            L.SetPedAsCop(chasingHeli.Driver);
            L.SetCopAsBusy(chasingHeli.Driver, true);
            searchingBlip = new Blip(veh1.Position.Around2D(50f), 125f);
            CalloutBlips.Add(searchingBlip);
            CalloutEntities.AddRange(new Entity[] { veh1, veh2, chasingHeli, chasingHeli.Driver });
            lastPositionRevealed = veh1.Position;
            updatePositionTime = Globals.GameTimer;
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
            Gang1Model.Clear();
            Gang2Model.Clear();
            base.End();
        }
        public void LogikaInformatika()
        {
            CalloutMainFiber = new GameFiber(() =>
            {
                Tasker();
                while (true)
                {
                    GameFiber.Yield();
                    switch (status)
                    {
                        case 1:
                            var dis1 = PlayerPed.DistanceToSquared(veh1);
                            var dis2 = PlayerPed.DistanceToSquared(veh2);
                            var closest = dis1 < dis2 ? veh1 : veh2;
                            if (PlayerPed.DistanceToSquared(closest) < 100f)
                            {
                                status = 2;
                                break;
                            }
                            if ((Globals.GameTimer - updatePositionTime) > 50000 && veh1.DistanceToSquared(lastPositionRevealed) > 10000)
                            {
                                updatePositionTime = Globals.GameTimer;
                                lastPositionRevealed = veh1.Position;
                                L.PlayScannerAudioUsingPosition(string.Format("SUSPECT_HEADING {0} IN_OR_ON_POSITION",
                                    SuspectCar.GetCardinalDirectionLowDetailedAudio()), SuspectCar.Position);
                                searchingBlip.Position = veh1.Position.Around2D(50f);
                            }
                            bool taskMonitor = veh1.GetActiveMissionType() == MissionType.Flee && veh2.GetActiveMissionType() != MissionType.Escort && 
                            chasingHeli.GetActiveMissionType() != MissionType.Circle;
                            if (!taskMonitor) Tasker();
                            break;
                        case 2:
                            if (searchingBlip) searchingBlip.Delete();
                            Pursuit = L.CreatePursuit();
                            foreach (Ped ped in veh1.Occupants.Concat(veh2.Occupants))
                            {
                                L.AddPedToPursuit(Pursuit, ped);
                                L.SetPursuitDisableAIForPed(ped, true);
                            }
                            L.SetPursuitIsActiveForPlayer(Pursuit, true);
                            L.RequestBackup(veh1.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.SwatTeam);
                            status = 3;
                            break;
                        case 3:
                            foreach (Ped ped in suspectInAction)
                            {
                                if (ped)
                                {
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
                            break;
                        case 4:
                            DisplaySummary();
                            break;
                    }
                }              
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
            End();
        }

        void FillVehicle(Vehicle vehicle, List<Model> models, RelationshipGroup relationshipGroup)
        {
            if (vehicle)
            {
                while (vehicle.FreeSeatsCount > 0)
                {
                    Ped ped = new Ped(models.GetRandomElement(), Spawn, spawn2);
                    ped.MakeMissionPed();
                    ped.RelationshipGroup = relationshipGroup;
                    ped.MaxHealth = 750;
                    ped.Health = 750;
                    involved++;
                    suspectInAction.Add(ped);
                    CalloutEntities.Add(ped);
                    NativeFunction.Natives.TASK_WARP_PED_INTO_VEHICLE(ped, vehicle, -2);
                }
            }
        }
        void Tasker()
        {
            veh1.Driver.VehicleMission(veh2, MissionType.Flee, veh1.TopSpeed, Globals.Sheeesh, 35f, 5f, true);
            veh2.Driver.VehicleMission(veh1, MissionType.Escort, veh2.TopSpeed, Globals.Sheeesh, 10f, 5f, true);
            chasingHeli.Driver.HeliMission(chasingHeli, veh1, null, Vector3.Zero, MissionType.Circle, 50f, 20f, -1.0f, 50, 30, 0);
            foreach (Ped ped in veh1.Passengers.Concat(veh2.Passengers))
            {
                ped.CombatAgainstHatedTargetAroundPed(350f);
            }
        }
    }
}
