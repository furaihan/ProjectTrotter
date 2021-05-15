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
            Spawn = SpawnManager.GetPedSpawnPoint(PlayerPed, 350, 950);
            if (Spawn == Spawnpoint.Zero)
            {
                Displayed = false;
                return false;
            }
            var nameRnd = PersonaHelper.GetRandomName(LSPD_First_Response.Gender.Male);
            var model = Extension.GetRandomMaleModel();
            Persona clone = Persona.FromExistingPed(World.GetAllPeds().Where(x => x && x.Model.Hash == model.Hash).GetRandomElement());
            clone.Forename = nameRnd.Item1;
            clone.Surname = nameRnd.Item2;
            Civilian = new Ped(model, Spawn, Spawn);
            CalloutEntities.Add(Civilian);
            Civilian.MakeMissionPed();
            LSPDFR.SetPersonaForPed(Civilian, clone);
            CalloutPosition = Spawn;
            CalloutMessage = "Heart Attack Civilian";
            CalloutAdvisory = $"The Civilian name is {nameRnd.Item1} {nameRnd.Item2}";
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
            Civilian.Tasks.PlayAnimation("random@drunk_driver_1", "drunk_fall_over", 4.0f, AnimationFlags.StayInEndFrame | AnimationFlags.Loop | AnimationFlags.RagdollOnCollision);
            Civilian.IsInvincible = true;
            CalloutRunning = true;
            MainSituation();
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
            while(CalloutRunning)
            {
                GameFiber.Yield();
                var traceStart = PlayerPed.IsInAnyVehicle(false) ? PlayerPed.CurrentVehicle.FrontPosition : PlayerPed.FrontPosition;
                Extension.DrawLine(traceStart, Civilian.Position, Color.Red);
                var raycast = World.TraceLine(traceStart, Civilian.Position, (TraceFlags)(1 | 2 | 4 | 8 | 16 | 256));
                if (Civilian && raycast.Hit && raycast.HitEntity && raycast.HitEntity == Civilian)
                {
                    "Raycast hit civilian".ToLog();
                    break;
                }
                else if (Civilian && Civilian.Position.IsOnScreen())
                {
                    "Civilian is on screen".ToLog();
                    break;
                }
                else if (Civilian && PlayerPed.DistanceTo(Civilian) < 30f) break;
            }
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
                    var task1 = Paramedic1.DriveVehicleWithNavigationMesh(CalloutPosition, VehicleDrivingFlags.Emergency, stoppingRange: 10f);
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
                    Paramedic1.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                    Paramedic2.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen).WaitForCompletion(3000);
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
