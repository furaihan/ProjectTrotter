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
        private readonly TimerBarPool pool = new();
        public bool CanEnd = false;
        private bool Hooking = false;
        private bool HookSuccess = false;
        private readonly List<Vehicle> AssignedToHookTask = new();
        private bool SpawnVehicles = false;
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
            ShowCalloutAreaBlipBeforeAccepting(CalloutPosition, 60);
            AddMaximumDistanceCheck(1000f, CalloutPosition);
            AddMinimumDistanceCheck(50f, CalloutPosition);
            PlayScannerWithCallsign("WE_HAVE CRIME_SOLICITATION IN_OR_ON_POSITION", CalloutPosition);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            $"Model Count: {hookerModels.Count}, Valid: {hookerModels.Where(x => x.IsValid && x.IsInCdImage).Count()}".ToLog();
            Model selected = hookerModels.GetRandomElement(m=>m.IsValid && m.IsInCdImage, true);
            selected.Load();
            selected.LoadCollisionAndWait();
            Blip = new Blip(CalloutPosition.Around2D(50f).ToGround(), 80f)
            {
                Color = Color.FromArgb(100, HudColor.Franklin.GetColor()),
                IsRouteEnabled = true,
                RouteColor = Color.FromArgb(180, HudColor.Yellow.GetColor()),
            };
            CalloutBlips.Add(Blip);
            Hooker = new Ped(selected, SolicitationSpawn, SolicitationSpawn);
            Hooker.MakeMissionPed();
            Hooker.RandomizeVariation();
            CalloutEntities.Add(Hooker);
            "Playing ped scenario action".ToLog();
            Hooker.PlayScenarioAction(scenarios.GetRandomElement(), false);
            "Set the hooker movement animation set".ToLog();
            Hooker.MovementAnimationSet = Peralatan.Random.NextDouble() > 0.5f ? "move_f@sexy@a" : "move_f@femme@";
            "After setting mas".ToLog();
            Game.ProductVersion.ToString().ToLog();
            "After game product ver".ToLog();
            World.GetEntities(CalloutPosition, 30f, GetEntitiesFlags.ConsiderCars).Where(x => x && x.IsVehicle()).Select(x => x as Vehicle).Where(x => x.IsEmpty && x.Speed < 2f && !x.Model.IsLawEnforcementVehicle).
                ToList().ForEach(x =>
               {
                   if (x) x.Delete();
               });
            "Lets go to the main logic of this callout".ToLog();
            CalloutMainLogic();
            "Fiber is created, starting...".ToLog();
            CalloutMainFiber.Start();
            return base.OnCalloutAccepted();
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
            SpawnVehicles = false;
            base.End();
        }
        private void DisplayHookerAwarenessBar()
        {
            GameFiber.StartNew(() =>
            {
                AwarenessBar = new BarTimerBar("Awareness")
                {
                    ForegroundColor = HudColor.Pink.GetColor(),
                };
                AwarenessBar.BackgroundColor = Color.FromArgb(120, AwarenessBar.ForegroundColor);
                AwarenessBar.Accent = Color.Gold;
                AwarenessBar.Markers.Add(new TimerBarMarker(0.72585f, Color.Gold));
                pool.Add(AwarenessBar);
                while (CalloutRunning)
                {
                    try
                    {
                        float percentage = Hooker ? 1 - (PlayerPed.DistanceSquaredTo(Hooker) - 250) / 3400 : 0;
                        if (Hooker.IsHeadingTowards(PlayerPed, 50f)) percentage += 0.0071595f;
                        else if (PlayerPed.IsInCover && AwarenessBar.Percentage > 0) percentage -= 0.006f;
                        if (PlayerPed.IsSprinting && AwarenessBar.Percentage > 0) percentage += 0.0038545f;
                        else if (PlayerPed.IsRunning && AwarenessBar.Percentage > 0) percentage += 0.001225f;
                        if (percentage > 0.725) percentage += (float)(percentage * 0.085);
                        AwarenessBar.Percentage = percentage;
                    }
                    catch { continue; }
                    GameFiber.Yield();                  
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
        private void VehicleSpawner()
        {
            GameFiber.StartNew(delegate
            {
                while (SpawnVehicles && CalloutRunning)
                {
                    GameFiber.Yield();
                    try
                    {
                        var sp = SpawnManager.GetVehicleSpawnPoint(Hooker.Position, 100, 300);
                        if (sp == Spawnpoint.Zero) continue;
                        GameFiber.Wait(Peralatan.Random.Next(3000, 15001));
                        var rv = new Vehicle(m => m.IsSuitableCar(), sp.Position, sp.Heading);
                        var rp = rv.CreateRandomDriver();
                        rv.MarkAsNoLongerNeeded();
                        rp.MarkAsNoLongerNeeded();
                    }
                    catch
                    {
                        continue;
                    }
                }
            });
        }
        private void GetClose()
        {
            while (CalloutRunning)
            {
                GameFiber.Yield();
                var raycast = World.TraceLine(PlayerPed.IsInAnyVehicle(false) ? PlayerPed.CurrentVehicle.FrontPosition : PlayerPed.FrontPosition, Hooker.Position, TraceFlags.None);
                if (raycast.Hit)
                {
                    if (raycast.HitEntity && raycast.HitEntity == Hooker)
                    {
                        "Raycast Hit Hooker".ToLog();
                        Game.DisplaySubtitle("~g~BarbarianCall~s~: Raycast hit hooker");
                        break;
                    }
                }
                if (PlayerPed.DistanceSquaredTo(CalloutPosition) < 6400f || PlayerPed.DistanceSquaredTo(Hooker) < 625f) break;
            }
            if (Blip) Blip.Delete();
        }
        private void CalloutMainLogic()
        {
            "Set the GameFiber content".ToLog();
            CalloutMainFiber = new GameFiber(() =>
            {
                try
                {
                    GetClose();
                    if (!CalloutRunning) return;
                    if (Blip) Blip.Delete();
                    Blip = new Blip(Hooker.Position, 30f)
                    {
                        Color = Color.FromArgb(120, HudColor.PinkLight.GetColor()),
                    };
                    CalloutBlips.Add(Blip);
                    DisplayHookerAwarenessBar();
                    SpawnVehicles = true;
                    VehicleSpawner();
                    while (CalloutRunning)
                    {
                        GameFiber.Wait(1);
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
                            SuspectCar = (Vehicle)World.GetEntities(CalloutPosition, 10f,
                                GetEntitiesFlags.ConsiderGroundVehicles | GetEntitiesFlags.ExcludeEmergencyVehicles | GetEntitiesFlags.ExcludePlayerVehicle | GetEntitiesFlags.ConsiderCars | 
                                GetEntitiesFlags.ExcludeEmptyVehicles).GetRandomElement();
                            if (!SuspectCar) continue;
                            if (!SuspectCar.IsVehicle()) continue;
                            if (SuspectCar && !Func.GetIsAudioEngineBusy()) Func.PlayScannerAudio(SuspectCar.GetColor().PrimaryColor.GetPoliceScannerColorAudio());
                            if (SuspectCar && SuspectCar.DistanceSquaredTo(Hooker) > 100f) continue;
                            if (SuspectCar && SuspectCar.Driver && SuspectCar.Driver.IsMale && !SuspectCar.HasPassengers && !AssignedToHookTask.Contains(SuspectCar) && SuspectCar.Model.IsSuitableCar())
                            {
                                Suspect = SuspectCar.Driver;
                                Suspect.MakeMissionPed();
                                SuspectCar.MakePersistent();
                                SuspectCar.Metadata.BAR_HookVehicle = true;
                                AssignedToHookTask.Add(SuspectCar);
                                CalloutEntities.Add(Suspect);
                                CalloutEntities.Add(SuspectCar);
                                Hooking = true;
                                HookProcess(Suspect, SuspectCar);
                            }
                            else if (SuspectCar)
                            {
                                List<string> log = new()
                                {
                                    $"Car isn't suitable. {SuspectCar.GetDisplayName()}",
                                    $"Color: {SuspectCar.GetColor().PrimaryColorName}",
                                    $"Driver Male: {SuspectCar.Driver && SuspectCar.Driver.IsMale}",
                                    $"Has Passenger: {SuspectCar.HasPassengers}, Count: {SuspectCar.Passengers.Length}",
                                    $"Distance: {Vector3.Distance(SuspectCar.Position, Hooker)}"
                                };
                                log.ForEach(Peralatan.ToLog);
                                AssignedToHookTask.Add(SuspectCar);
                                if (SuspectCar && SuspectCar.Driver) SuspectCar.Driver.Tasks.CruiseWithVehicle(15f, Globals.Normal);
                                GameFiber.Wait(Peralatan.Random.Next(5000, 8000));
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
            "Let the OnCalloutAccepted is taking care of this part".ToLog();
        }
        private void HookProcess(Ped suspect, Vehicle suspectVeh)
        {
            GameFiber.StartNew(() =>
            {
                try
                {
                    if (suspect && suspectVeh)
                    {
                        suspect.Tasks.PerformDrivingManeuver(VehicleManeuver.Wait);
                        if (suspectVeh.DistanceSquaredTo(Hooker) <= 50f)
                        {
                            suspect.Tasks.PerformDrivingManeuver(VehicleManeuver.GoForwardStraightHalfAcceleration).WaitForCompletion(400);
                            suspect.Tasks.PerformDrivingManeuver(VehicleManeuver.GoForwardStraightBraking).WaitForCompletion(100);
                            suspect.Tasks.PerformDrivingManeuver(VehicleManeuver.SwerveRight).WaitForCompletion(900);
                            suspect.Tasks.PerformDrivingManeuver(VehicleManeuver.SwerveAndBrakeLeft).WaitForCompletion(550);
                        }
                        if (suspectVeh.HasRoof) suspectVeh.ConvertibleRoofState = VehicleConvertibleRoofState.Lowering;
                        GameFiber.Wait(2000);
                        Hooker.Tasks.Clear();
                        var walkPos = suspectVeh.Position + Vector3.RelativeLeft * 1.5f;
                        Hooker.Tasks.FollowNavigationMeshToPosition(walkPos, walkPos.GetHeadingTowards(suspect), 1f).WaitForCompletion(15000);
                        var chance = Peralatan.Random.NextDouble();
                        if (suspect && suspectVeh && Math.Abs(Func.GetPersonaForPed(suspect).GetAge() - Func.GetPersonaForPed(Hooker).GetAge()) >= 20)
                        {
                            Peralatan.ToLog($"Hook unsuccessfull, Age Different: {Math.Abs(Func.GetPersonaForPed(suspect).GetAge() - Func.GetPersonaForPed(Hooker).GetAge())}");
                            chance = 0.0005;
                        }
                        if (chance > 0.5f)
                        {
                            Hooker.Tasks.EnterVehicle(suspectVeh, 0).WaitForCompletion();
                            var speedRnd = Peralatan.Random.Next(15, 21);
                            suspectVeh.SetForwardSpeed(speedRnd);
                            suspect.WanderWithVehicle(speedRnd, (VehicleDrivingFlags)0xC00AB);
                            HookSuccess = true;
                        }
                        else
                        {
                            Hooker.Tasks.FollowNavigationMeshToPosition(CalloutPosition, SolicitationSpawn, 1f).WaitForCompletion(10000);
                            if (Hooker.DistanceTo(CalloutPosition) < 2f)
                            {
                                Hooker.Tasks.AchieveHeading(SolicitationSpawn).WaitForCompletion(2000);
                                Hooker.PlayScenarioAction(scenarios.GetRandomElement(), true);
                            }
                            suspectVeh.Dismiss();
                            suspectVeh.Occupants.ToList().ForEach(Extension.MarkAsNoLongerNeeded);
                            Hooking = false;
                        }
                    }
                    Hooking = false;
                }
                catch (Exception e)
                {
                    e.ToString().ToLog();
                    Hooking = false;
                    HookSuccess = false;
                }
            });
        }
    }
}
