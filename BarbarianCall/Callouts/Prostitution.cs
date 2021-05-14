using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using Func = LSPD_First_Response.Mod.API.Functions;
using Rage;
using BarbarianCall.Types;
using BarbarianCall.Extensions;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace BarbarianCall.Callouts
{
    [CalloutInfo("Prostitution", CalloutProbability.High)]
    public class Prostitution : CalloutBase
    {
        private Ped Hooker;
        private Spawnpoint RoadSide;
        private readonly List<Model> hookerModels = new() { 0x73DEA88B, 0x028ABF95, 0x14C3E407, 0x031640AC, 0x2F4AEC3E, 0xB920CC2B, 0x84A1B11A, 0x9CF26183 };
        private readonly List<string> scenarios = new() { "WORLD_HUMAN_PROSTITUTE_HIGH_CLASS", "WORLD_HUMAN_PROSTITUTE_LOW_CLASS" };
        private Spawnpoint SolicitationSpawn;
        private BarTimerBar AwarenessBar;
        private TimerBarIcon TimerBarIcon;
        private readonly TimerBarPool pool = new();
        public bool CanEnd = false;
        private bool Hooking = false;
        private bool HookSuccess = false;
        private List<Vehicle> AssignedToHookTask = new();
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutRunning = false;
            PursuitCreated = false;
            DeclareVariable();
            SolicitationSpawn = SpawnManager.GetSolicitationSpawnpoint(PlayerPed.Position, out Spawnpoint _, out RoadSide);
            CalloutPosition = SolicitationSpawn;
            CalloutMessage = "Prostitution";
            FriendlyName = "Solicitation BO";
            CalloutAdvisory = "Be careful, your siren may draw an attention";
            if (CalloutPosition == Spawnpoint.Zero)
            {
                Peralatan.ToLog("Solicitation | No suitable location found after 600 tries");
                Displayed = false;
                return false;
            }
            ShowCalloutAreaBlipBeforeAccepting(CalloutPosition, 120);
            PlayScannerWithCallsign("WE_HAVE CRIME_SOLICITATION IN_OR_ON_POSITION", CalloutPosition);
            return Displayed = base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            Model selected = hookerModels.GetRandomElement(true);
            selected.Load();
            Blip = new Blip(CalloutPosition.Around2D(50f).ToGround(), 80f)
            {
                Color = Color.FromArgb(100, HudColor.Franklin.GetColor()),
                IsRouteEnabled = true,
                RouteColor = Color.FromArgb(180, HudColor.Yellow.GetColor()),
            };
            CalloutBlips.Add(Blip);
            Hooker = new Ped(selected, SolicitationSpawn, SolicitationSpawn);
            Hooker.MakeMissionPed();
            CalloutEntities.Add(Hooker);
            Hooker.PlayScenarioAction(scenarios.GetRandomElement(), false);
            Hooker.MovementAnimationSet = Peralatan.Random.NextDouble() > 0.5f ? "move_f@sexy@a" : "move_f@sexy";
            ClearUnrelatedEntities();
            CalloutMainLogic();
            CalloutMainFiber.Start();
            return Accepted = base.OnCalloutAccepted();
        }
        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }
        public override void Process()
        {
            if (CalloutRunning)
            {

            }
            base.Process();
        }
        public override void End()
        {
            base.End();
        }
        private void DisplayHookerAwarenessBar()
        {
            GameFiber.StartNew(() =>
            {
                TimerBarIcon = TimerBarIcon.Hidden;
                AwarenessBar = new BarTimerBar("Awareness")
                {
                    ForegroundColor = HudColor.Pink.GetColor()
                };
                AwarenessBar.BackgroundColor = Color.FromArgb(120, AwarenessBar.ForegroundColor);
                pool.Add(AwarenessBar);
                while (CalloutRunning)
                {
                    GameFiber.Yield();
                    float percentage = Hooker ? 1 - (PlayerPed.DistanceTo(Hooker) - 15) / 45 : 1;                   
                    HitResult hitResult = World.TraceLine(Hooker.FrontPosition, PlayerPed.Position, TraceFlags.IntersectEverything);
                    if (hitResult.HitEntity && hitResult.HitEntity == Game.LocalPlayer.Character && AwarenessBar.Percentage < 0.75f) percentage += 0.007125f;
                    else if (PlayerPed.IsInCover && AwarenessBar.Percentage > 0) percentage -= 0.006f;
                    if (PlayerPed.IsSprinting && AwarenessBar.Percentage > 0) percentage += 0.0038545f;
                    else if (PlayerPed.IsRunning && AwarenessBar.Percentage > 0) percentage += 0.001225f;
                    AwarenessBar.Percentage = percentage;
                }
            });
            GameFiber.StartNew(() =>
            {
                while (CalloutRunning)
                {
                    GameFiber.Yield();
                    pool.Draw();
                }
            });
        }
        private void GetClose()
        {
            while (CalloutRunning)
            {
                GameFiber.Yield();
                var raycast = World.TraceLine(PlayerPed.IsInAnyVehicle(false) ? PlayerPed.CurrentVehicle.FrontPosition : PlayerPed.FrontPosition, Hooker.Position, TraceFlags.IntersectEverything);
                if (raycast.Hit)
                {
                    if (raycast.HitEntity && raycast.HitEntity == Hooker)
                    {
                        "Raycast Hit Hooker".ToLog();
                        break;
                    }
                }
                if (PlayerPed.DistanceTo(Blip.Position) < 80f || PlayerPed.DistanceTo(Hooker) < 25f) break;
            }
            if (Blip) Blip.Delete();
        }
        private void CalloutMainLogic()
        {
            CalloutMainFiber = new GameFiber(() =>
            {
                try
                {
                    GetClose();
                    if (!CalloutRunning) return;
                    if (Blip) Blip.Delete();
                    World.GetAllEntities().Where(e => e && !CalloutEntities.Contains(e) && !e.CreatedByTheCallingPlugin && e.GetAttachedBlips().Length == 0 &&
                    (e.IsEntityAPed() || e.IsEntityAVehicle()) && !e.Position.IsOnScreen() && e.DistanceTo(CalloutPosition) <= 50f && e != PlayerPed
                    && e != PlayerPed.CurrentVehicle).ToList().ForEach(x => { if (x) x.Delete(); });
                    Blip = new Blip(Hooker.Position, 30f)
                    {
                        Color = Color.FromArgb(120, HudColor.PinkLight.GetColor()),
                    };
                    CalloutBlips.Add(Blip);
                    DisplayHookerAwarenessBar();
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        if (AwarenessBar?.Percentage >= 1 && Game.IsKeyDownRightNow(System.Windows.Forms.Keys.End))
                        {
                            End("Konangan su");
                        }
                        if (!Hooking)
                        {
                            if (SuspectCar)
                            {
                                if (SuspectCar.Driver) SuspectCar.Driver.Dismiss();
                                if (SuspectCar) SuspectCar.Dismiss();
                            }
                            SuspectCar = World.GetAllVehicles().Where(x => x && x.HasDriver && x.IsCar && x.Driver.IsMale && !x.HasPassengers 
                            && x.DistanceTo(Hooker) > 60f && x.DistanceTo(Hooker) < 250f && !x.Position.IsOnScreen() && 
                            x.Driver != PlayerPed && x != PlayerPed.CurrentVehicle).GetRandomElement();
                            if (SuspectCar)
                            {
                                Suspect = SuspectCar.Driver;
                                Suspect.MakeMissionPed();
                                SuspectCar.MakePersistent();
                                SuspectCar.Metadata.BAR_HookVehicle = true;
                                AssignedToHookTask.Add(SuspectCar);
                                CalloutEntities.Add(Suspect);
                                CalloutEntities.Add(SuspectCar);
                                Suspect.Tasks.PerformDrivingManeuver(VehicleManeuver.SwerveRight).WaitForCompletion(600);
                                GameFiber.Wait(Peralatan.Random.Next(5500, 15001));
                                SuspectCar.Repair();
                                Hooking = true;
                                HookProcess(Suspect, SuspectCar);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    e.ToString().ToLog();
                    NetExtension.SendError(e);
                    End();
                }              
            });
        }
        private void HookProcess(Ped suspect, Vehicle suspectVeh)
        {
            GameFiber.StartNew(() =>
            {
                try
                {
                    if (suspect && suspectVeh)
                    {
                        var driveTask = suspect.DriveTo(RoadSide, 0.5f, MathExtension.GetRandomFloatInRange(15f, 21f), (VehicleDrivingFlags)786603/*Normal*/);
                        while (driveTask.IsActive)
                        {
                            GameFiber.Wait(200);
                            var raycast = World.TraceLine(suspectVeh.FrontPosition, Hooker.Position, TraceFlags.IntersectEverything);
                            if (raycast.Hit && raycast.HitEntity == Hooker && suspectVeh.DistanceTo(Hooker) < 60f) suspectVeh.SetForwardSpeed(9f);
                            else if (suspectVeh.DistanceTo(Hooker) < 30f) suspectVeh.SetForwardSpeed(9f);
                            if (suspectVeh.DistanceTo(Hooker) < 10f)
                            {
                                suspect.Tasks.PerformDrivingManeuver(VehicleManeuver.GoForwardStraightBraking).WaitForCompletion(200);
                                suspect.Tasks.PerformDrivingManeuver(VehicleManeuver.SwerveRight).WaitForCompletion(900);
                                suspect.Tasks.PerformDrivingManeuver(VehicleManeuver.SwerveLeft).WaitForCompletion(550);
                                break;
                            }
                        }
                        Hooker.Tasks.Clear();
                        var walkPos = Extension.GetEntryPositionOfVehicleDoor(suspectVeh, Extension.VehicleDoorIndex.FrontRightDoor);
                        Hooker.Tasks.FollowNavigationMeshToPosition(walkPos, walkPos.GetHeadingTowards(suspect), 1f).WaitForCompletion(15000);
                        var chance = Peralatan.Random.NextDouble();
                        if (suspect && suspectVeh && Math.Abs(Func.GetPersonaForPed(suspect).GetAge() - Func.GetPersonaForPed(Hooker).GetAge()) >= 20)
                        {
                            Peralatan.ToLog($"Hook unsuccessfull, Age Different: {Math.Abs(Func.GetPersonaForPed(suspect).GetAge() - Func.GetPersonaForPed(Hooker).GetAge())}");
                            chance = 0.0005;
                        }
                        if (chance > 0.5f)
                        {
                            Hooker.Tasks.EnterVehicle(suspectVeh, 0);
                            var speedRnd = Peralatan.Random.Next(15, 21);
                            suspectVeh.SetForwardSpeed(speedRnd);
                            suspect.WanderWithVehicle(speedRnd, VehicleDrivingFlags.Normal);
                            HookSuccess = true;
                        }
                        else
                        {
                            Hooker.Tasks.FollowNavigationMeshToPosition(CalloutPosition, SolicitationSpawn, 1f);
                        }
                    }
                }
                catch
                {
                    throw;
                }
            });
        }
    }
}
