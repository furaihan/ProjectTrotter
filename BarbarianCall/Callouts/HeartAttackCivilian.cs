using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Rage;
using LSPDFR = LSPD_First_Response.Mod.API.Functions;
using LSPD_First_Response.Engine.Scripting.Entities;
using BarbarianCall.Extensions;
using BarbarianCall.Types;
using BarbarianCall.API;

namespace BarbarianCall.Callouts
{
    [LSPD_First_Response.Mod.Callouts.CalloutInfo("Heart Attack Civilian", LSPD_First_Response.Mod.Callouts.CalloutProbability.High)]
    public class HeartAttackCivilian : CalloutBase
    {
        public Ped Civilian;
        private Ped Paramedic1;
        private Ped Paramedic2;
        private Vehicle Ambulance;
        private enum CalloutState
        {
            EnRoute,
            OnScene,
            CivilianEnterAmbulance,
            EscortingAmbulance,
        }

        private new CalloutState State;
        public override bool OnBeforeCalloutDisplayed()
        {
            Spawn = SpawnManager.GetPedSpawnPoint(PlayerPed, 300,500);
            if (Spawn == Spawnpoint.Zero)
            {
                Displayed = false;
                return false;
            }
            if (PlayerPed.Position.GetZoneName().ToLower() == "island")
            {
                "Cayo Perico Island is not supported yet".ToLog();
                Displayed = false;
                return false;
            }
            var nameRnd = PersonaHelper.GetRandomName(LSPD_First_Response.Gender.Male);
            var model = Extension.GetRandomMaleModel();
            "Creating male ped".ToLog();
            Civilian = new Ped(model, Spawn, Spawn);
            CalloutEntities.Add(Civilian);
            Civilian.MakeMissionPed();
            "Cloning ped persona".ToLog();
            Persona clone = Persona.FromExistingPed(Civilian);
            clone.Forename = nameRnd.Item1;
            clone.Surname = nameRnd.Item2;
            $"Setting persona for ped {Civilian.Model.Name}".ToLog();
            LSPDFR.SetPersonaForPed(Civilian, clone);
            CalloutPosition = Spawn;
            CalloutMessage = "Heart Attack Civilian";
            CalloutAdvisory = $"The Civilian name is {nameRnd.Item1} {nameRnd.Item2}";
            PlayScannerWithCallsign("BAR_CODE_99 BAR_CRIME_CIVILIAN_NEEDING_ASSISTANCE IN_OR_ON_POSITION", CalloutPosition);
            ShowCalloutAreaBlipBeforeAccepting(CalloutPosition, 30f);
            AddMinimumDistanceCheck(100f, CalloutPosition);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            Blip = new Blip(Spawn.Position.Around2D(20f).ToGround(), 25)
            {
                Color = Yellow,
                IsRouteEnabled = true
            };
            "Playing fall animation".ToLog();
            Civilian.PlayScenarioAction(PedScenario.WORLD_HUMAN_STUPOR, true);
            Civilian.IsInvincible = true;
            CalloutRunning = true;
            "Getting Callout Main Fiber content".ToLog();
            MainSituation();
            "Starting Fiber".ToLog();
            CalloutMainFiber.Start();
            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            base.Process();
        }
        public override void End()
        {
            if (State != CalloutState.EscortingAmbulance)
            {

            }
            base.End();
        }
        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }
        private void GetClose()
        {
            State = CalloutState.EnRoute;
            while (CalloutRunning)
            {
                GameFiber.Yield();
                var traceStart = PlayerPed.IsInAnyVehicle(false) ? PlayerPed.CurrentVehicle.FrontPosition : PlayerPed.FrontPosition;
                Extension.DrawLine(traceStart, Civilian.Position, Color.Red);
                var raycast = World.TraceLine(traceStart, Civilian.Position, TraceFlags.None);
                if (Civilian && raycast.Hit && raycast.HitEntity && raycast.HitEntity == Civilian)
                {
                    "Raycast hit civilian".ToLog();
                    break;
                }                
                else if (Civilian && PlayerPed.DistanceToSquared(Civilian) < 900f) break;
            }
            $"Distance: {PlayerPed.DistanceTo(Civilian)}".ToLog();
            State = CalloutState.OnScene;
            if (Blip) Blip.Delete();
            Blip = Civilian.AttachBlip();
            Blip.Color = Color.Orange;
            Blip.SetBlipName("Heart Attack Civilian");
        }
        private void MainSituation()
        {
            CalloutMainFiber = new GameFiber(() =>
            {
                try
                {
                    GetClose();
                    if (!CalloutRunning) return;
                    Spawnpoint ambulanceSpawn = SpawnManager.GetVehicleSpawnPoint(CalloutPosition, 100, 150);
                    if (ambulanceSpawn == Spawnpoint.Zero) ambulanceSpawn = SpawnManager.GetVehicleSpawnPoint(CalloutPosition, 80, 200);
                    if (UltimateBackupRunning)
                    {
                        var ubAmb = UltimateBackupFunc.GetUnit(UltimateBackupFunc.EUltimateBackupUnitType.Ambulance, ambulanceSpawn, 2);
                        Paramedic1 = ubAmb.Item2.First();
                        Paramedic2 = ubAmb.Item2.Last();
                        Ambulance = ubAmb.Item1;
                    }
                    else
                    {
                        Ambulance = new Vehicle("AMBULANCE", ambulanceSpawn);
                        Paramedic1 = new Ped("s_m_m_paramedic_01", ambulanceSpawn, ambulanceSpawn);
                        Paramedic2 = new Ped("s_m_m_paramedic_01", ambulanceSpawn, ambulanceSpawn);
                        Paramedic1.RandomizeVariation();
                        Paramedic2.RandomizeVariation();
                        if (Paramedic1) Paramedic1.WarpIntoVehicle(Ambulance, -1);
                        if (Paramedic2) Paramedic2.WarpIntoVehicle(Ambulance, 0);
                    }
                    if (Ambulance) Ambulance.Heading = ambulanceSpawn;
                    List<Entity> ambulanceEntities = new() { Ambulance, Paramedic1, Paramedic2 };
                    ambulanceEntities.ForEach(e => e.MakePersistent());
                    ambulanceEntities.ForEach(e => CalloutEntities.Add(e));
                    if (Paramedic1) Paramedic1.BlockPermanentEvents = true;
                    if (Paramedic2) Paramedic2.BlockPermanentEvents = true;
                    var task1 = Paramedic1.DriveTo(CalloutPosition, 10f, MathExtension.GetRandomFloatInRange(15, 21), VehicleDrivingFlags.Emergency);
                    "~g~Ambulance~s~ is en route to your current location".DisplayNotifWithLogo("Heart Attack Civilian", fadeIn: true, blink: true, hudColor: RAGENativeUI.HudColor.RedLight);
                    StopWatch = new();
                    StopWatch.Start();
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        if (!task1.IsActive || StopWatch.ElapsedMilliseconds > 60000) 
                        {
                            $"Task Status: {task1.Status}, Elapsed: {StopWatch.ElapsedMilliseconds} ms".ToLog();
                            break; 
                        }
                    }
                    if (!CalloutRunning) return;
                    Vector3 wpos = Civilian.Position + Vector3.RelativeRight;
                    if (Paramedic1) Paramedic1.Tasks.FollowNavigationMeshToPosition(Civilian.Position + Civilian.RightVector, Paramedic1.GetHeadingTowards(Civilian), 10f);
                    if (Paramedic2) Paramedic2.Tasks.FollowNavigationMeshToPosition(Civilian.Position + Civilian.RightVector * -1f, Paramedic2.GetHeadingTowards(Civilian), 10f).WaitForCompletion(10000);
                    if (Paramedic1 && Paramedic1.DistanceTo(Civilian) > 5f) Paramedic1.SetPositionWithSnap(Civilian.Position + Civilian.RightVector);
                    if (Paramedic2 && Paramedic2.DistanceTo(Civilian) > 5f) Paramedic2.SetPositionWithSnap(Civilian.Position + Civilian.RightVector * -1f);
                    if (Paramedic1) Paramedic1.Tasks.AchieveHeading(Paramedic1.GetHeadingTowards(Civilian));
                    if (Paramedic2) Paramedic2.Tasks.AchieveHeading(Paramedic2.GetHeadingTowards(Civilian)).WaitForCompletion(1000);
                    if (Paramedic1) Paramedic1.PlayScenarioAction(PedScenario.CODE_HUMAN_MEDIC_KNEEL, true);
                    if (Paramedic2) Paramedic2.PlayScenarioAction(PedScenario.CODE_HUMAN_MEDIC_TEND_TO_DEAD, true);
                    GameFiber.Wait(Peralatan.Random.Next(5000, 10000));
                    if (Paramedic1) Paramedic1.Tasks.Clear();
                    if (Paramedic2) Paramedic2.Tasks.Clear();
                    if (Civilian) Civilian.Tasks.Clear();
                    GameFiber.Wait(2000);
                    Rage.Task walkTask = null;
                    if (Paramedic1) Paramedic1.Tasks.EnterVehicle(Ambulance, 10000, -1, 10f, EnterVehicleFlags.None);
                    if (Civilian) walkTask = Civilian.Tasks.FollowNavigationMeshToPosition(Ambulance.RearPosition + Vector3.RelativeBack, Ambulance.Heading, 0.5f);
                    if (Paramedic2) Paramedic2.OpenVehicleDoor(Ambulance, 10000, 2, 10f).WaitForCompletion();
                    if (walkTask != null) walkTask.WaitForCompletion(10000);
                    if (Civilian) Civilian.Tasks.EnterVehicle(Ambulance, 2).WaitForCompletion(10000);
                    if (Paramedic1 && Paramedic1.IsInVehicle(Ambulance, false)) Paramedic1.WarpIntoVehicle(Ambulance, -1);
                    if (Paramedic2 && Paramedic2.IsInVehicle(Ambulance, false)) Paramedic2.WarpIntoVehicle(Ambulance, 0);
                    if (Civilian && Civilian.IsInVehicle(Ambulance, false)) Civilian.WarpIntoVehicle(Ambulance, 2);
                    Vector3 hospital = Globals.Hospitals.OrderBy(x => Vector3.DistanceSquared(PlayerPed.Position, x.Position)).FirstOrDefault();
                    Peralatan.DisplayNotifWithLogo("Please escort this ambulance to the hospital", icon: Peralatan.NotificationIcon.Email, hudColor: RAGENativeUI.HudColor.BlueLight);
                    Blip = new Blip(hospital)
                    {
                        IsRouteEnabled = true,
                        Color = Yellow,
                        Name = "Hospital",
                    };
                    GameFiber.SleepUntil(() => PlayerPed.IsInAnyPoliceVehicle, -1);
                    if (!CalloutRunning) return;
                    if (Paramedic1) Paramedic1.EscortVehicle(PlayerPed.CurrentVehicle, TaskExtension.EscortVehicleMode.Behind, 35f, VehicleDrivingFlags.Emergency, 5f, 3f);
                    Vehicle cv = PlayerPed.CurrentVehicle;
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        if (PlayerPed.CurrentVehicle != cv)
                        {
                            cv = PlayerPed.CurrentVehicle;
                            if (Paramedic1) Paramedic1.EscortVehicle(PlayerPed.CurrentVehicle, TaskExtension.EscortVehicleMode.Behind, 35f, VehicleDrivingFlags.Emergency, 5f, 3f);
                        }                       
                        if (Ambulance.FrontPosition.DistanceToSquared(cv.RearPosition) < 4f || Ambulance.DistanceToSquared(cv) < 9f)
                        {
                            if (Paramedic1) Paramedic1.Tasks.PerformDrivingManeuver(VehicleManeuver.Wait).WaitForCompletion(200);
                            if (Paramedic1) Paramedic1.EscortVehicle(PlayerPed.CurrentVehicle, TaskExtension.EscortVehicleMode.Behind, 35f, VehicleDrivingFlags.Emergency, 5f, 3f);
                        }
                        if (PlayerPed.DistanceToSquared(hospital) < 625f) break;
                    }
                    if (!CalloutRunning) return;
                    PlayerPed.Tasks.PerformDrivingManeuver(VehicleManeuver.Wait);
                    Paramedic1.Tasks.PerformDrivingManeuver(VehicleManeuver.Wait);
                    Game.FadeScreenOut(3000, true);
                    Vehicle[] car = World.GetEntities(GetEntitiesFlags.ConsiderGroundVehicles | GetEntitiesFlags.ConsiderCars)
                    .Where(x => x && x.IsVehicle()).Select(x => x as Vehicle).Where(x => x.IsEmpty && x.Speed < 1f).ToArray();
                    Vehicle vehicle = car.Where(x => x.DistanceTo(hospital) < 50f).GetRandomElement();
                    Spawnpoint sp = new Spawnpoint(vehicle.Position, vehicle.Heading);
                    cv = PlayerPed.CurrentVehicle;
                    PlayerPed.CurrentVehicle.Position = sp;
                    PlayerPed.CurrentVehicle.Heading = sp;
                    PlayerPed.WarpIntoVehicle(cv, -1);
                    Game.FadeScreenIn(3000, true);
                    "~g~Congratulation~s~ you have saved the patient".DisplayNotifWithLogo(icon: Peralatan.NotificationIcon.Email, hudColor: RAGENativeUI.HudColor.GreenDark);
                    if (!CalloutRunning) return;
                    End();
                }
                catch (Exception e)
                {
                    e.ToString().ToLog();
                    NetExtension.SendError(e);
                    End(e.Message);
                }
            });
        }
    }
}
