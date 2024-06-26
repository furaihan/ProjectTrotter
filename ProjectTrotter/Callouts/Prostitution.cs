﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Drawing;
using Func = LSPD_First_Response.Mod.API.Functions;
using Rage;
using ProjectTrotter.Types;
using ProjectTrotter.Extensions;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace ProjectTrotter.Callouts
{
    [LSPD_First_Response.Mod.Callouts.CalloutInfo("Prostitution", LSPD_First_Response.Mod.Callouts.CalloutProbability.High)]
    public class Prostitution : CalloutBase
    {
        private Ped Hooker;
        private Spawnpoint RoadSide;
        private readonly List<Model> hookerModels = new() { 0x73DEA88B, 0x028ABF95, 0x14C3E407, 0x031640AC, 0x2F4AEC3E, 0xB920CC2B, 0x84A1B11A, 0x9CF26183 };
        private readonly List<PedScenario> scenarios = new() { PedScenario.WORLD_HUMAN_PROSTITUTE_HIGH_CLASS, PedScenario.WORLD_HUMAN_PROSTITUTE_LOW_CLASS };
        private Spawnpoint SolicitationSpawn;
        private BarTimerBar AwarenessBar;
        private readonly TimerBarPool pool = new();
        public bool CanEnd = false;
        private bool Hooking = false;
        private bool HookSuccess = false;
        private bool displayTimerBar = false;
        private readonly List<Vehicle> AssignedToHookTask = new();
        private bool SpawnVehicles = false;
        private bool caught = false;
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
            if (CalloutPosition == Vector3.Zero)
            {
                Logger.Log("Solicitation | No suitable location found after 600 tries");
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
            Hooker.MovementAnimationSet = MyRandom.NextDouble() > 0.5f ? "move_f@sexy@a" : "move_f@femme@";
            "After setting mas".ToLog();
            Game.ProductVersion.ToString().ToLog();
            "After game product ver".ToLog();
            World.GetEntities(CalloutPosition, 30f, GetEntitiesFlags.ConsiderGroundVehicles | GetEntitiesFlags.ConsiderCars | GetEntitiesFlags.ExcludePlayerPed | GetEntitiesFlags.ExcludePlayerVehicle | GetEntitiesFlags.ExcludeEmergencyVehicles)
                .Where(x => x && x.IsVehicle()).Select(x => x as Vehicle).Where(x => x.IsEmpty && x.Speed < 2f && !x.Model.IsLawEnforcementVehicle).
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
            displayTimerBar = false;
            AssignedToHookTask.ForEach(x =>
            {
                if (x)
                {
                    foreach (Ped occupant in x.Occupants)
                    {
                        if (occupant) occupant.Dismiss();
                    }
                    if (x) x.Dismiss();
                }
            });
            base.End();
        }
        protected override void CleanUp()
        {
            // TODO: implement CleanUp()
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
                while (displayTimerBar)
                {
                    try
                    {
                        float percentage = Hooker ? 1 - (PlayerPed.DistanceToSquared(Hooker) - 250) / 3400 : 0;
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
                while (displayTimerBar)
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
                        var sp = SpawnManager.GetVehicleSpawnPoint(PlayerPed.Position, 100, 300);
                        if (sp == Spawnpoint.Zero) continue;
                        GameFiber.Wait(MyRandom.Next(3000, 15001));
                        var randomVehicle = new Vehicle(m => m.IsSuitableCar() && Globals.ScannerVehicleModel.Contains(m) && m.IsValid, sp.Position, sp.Heading);
                        var randomPed = randomVehicle.CreateRandomDriver();
                        CalloutEntities.Add(randomVehicle);
                        CalloutEntities.Add(randomPed);
                        randomVehicle.RandomizeLicensePlate();
                        randomVehicle.MarkAsNoLongerNeeded();
                        randomPed.MarkAsNoLongerNeeded();
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
                var raycast = World.TraceCapsule(PlayerPed.IsInAnyVehicle(false) ? PlayerPed.CurrentVehicle.FrontPosition : PlayerPed.FrontPosition, Hooker.Position, 2f, (TraceFlags)511, Game.LocalPlayer.Character);
                if (raycast.Hit)
                {
                    if (raycast.HitEntity && raycast.HitEntity == Hooker)
                    {
                        "Raycast Hit Hooker".ToLog();
                        Game.DisplaySubtitle("~g~BarbarianCall~s~: Raycast hit hooker");
                        break;
                    }
                }
                if (PlayerPed.DistanceToSquared(CalloutPosition) < 6400f || PlayerPed.DistanceToSquared(Hooker) < 625f) break;
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
                    displayTimerBar = true;
                    DisplayHookerAwarenessBar();
                    SpawnVehicles = true;
                    VehicleSpawner();
                    StopWatch = new(); StopWatch.Start();
                    bool audioPlayed = false;
                    while (CalloutRunning)
                    {
                        GameFiber.Wait(1);
                        if (AwarenessBar?.Percentage >= 1 && Game.IsKeyDownRightNow(System.Windows.Forms.Keys.End))
                        {
                            End("Suspect is aware");
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
                            if (SuspectCar && SuspectCar.DistanceToSquared(Hooker) > 100f) continue;
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
                                log.ForEach(Logger.ToLog);
                                AssignedToHookTask.Add(SuspectCar);
                                if (SuspectCar && SuspectCar.Driver) SuspectCar.Driver.Tasks.CruiseWithVehicle(15f, Globals.Normal);
                                GameFiber.Wait(MyRandom.Next(5000, 8000));
                            }
                        }
                        if (HookSuccess)
                        {
                            if (!audioPlayed)
                            {
                                audioPlayed = true;
                                GameFiber.StartNew(() =>
                                {
                                    API.LSPDFRFunc.WaitAudioScannerCompletion();
                                    API.LSPDFRFunc.PlayScannerAudio($"VEHICLE BAR_IS BAR_A_CONJ {SuspectCar.GetColor().PrimaryColor.GetPoliceScannerColorAudio()} BAR_TARGET_PLATE {GenericUtils.GetLicensePlateAudio(SuspectCar)}");
                                });
                            }                           
                            if (StopWatch.ElapsedMilliseconds > 5000 && !caught)
                            {
                                StopWatch.Restart();
                                Game.DisplayHelp("Perform a traffic stop on a ~r~suspect~s~");
                            }
                            if (PlayerPed.DistanceToSquared(SuspectCar) < 100f && !caught)
                            {
                                caught = true;
                                var chance = MyRandom.NextDouble();
                                if (Hooker && Hooker.IsInAnyVehicle(false)) Hooker.Tasks.LeaveVehicle(LeaveVehicleFlags.BailOut).WaitForCompletion(1500);
                                Pursuit = Func.CreatePursuit();
                                Func.AddPedToPursuit(Pursuit, Hooker);
                                Func.AddPedToPursuit(Pursuit, Suspect);
                                Func.SetPursuitIsActiveForPlayer(Pursuit, true);
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
        private Checkpoint checkpoint;
        private void HookProcess(Ped suspect, Vehicle suspectVeh)
        {
            GameFiber.StartNew(() =>
            {
                try
                {
                    var topSpeed = suspectVeh.TopSpeed;
                    VehicleClass[] richClasses = { VehicleClass.Sport, VehicleClass.SportClassic, VehicleClass.Super };
                    if (suspect && suspectVeh)
                    {
                        if (checkpoint) checkpoint.Delete();
                        checkpoint = new Checkpoint(CheckpointIcon.Cylinder3, Spawn, 2f, 15f, VehiclePaint.Matte_Lime_Green.GetColor(), Color.White, true);
                        suspect.Tasks.PerformDrivingManeuver(VehicleManeuver.Wait).WaitForCompletion(500);
                        suspectVeh.SteeringAngle = -12f;
                        suspectVeh.TopSpeed = 3f;
                        suspect.VehicleTempAction(VehicleManeuver.GoForwardWithCustomSteeringAngle, 10000);
                        Stopwatch parkSw = Stopwatch.StartNew();
                        while (CalloutRunning)
                        {
                            GameFiber.Yield();
                            var park = SpawnpointUtils.GetRoadSidePointWithHeading(suspectVeh);
                            if (park != Vector3.Zero)
                            {
                                checkpoint.Position = park;
                                if (suspectVeh.DistanceToSquared(park) < 15f) break;
                            }
                            else
                            {
                                Logger.Log("No suitable park found. retrying...", LogLevel.Debug);
                                continue;
                            }
                            if (parkSw.ElapsedMilliseconds > 10000)
                            {
                                Logger.Log("Abort Park: Timeout");
                                break;
                            }
                        }
                        if (checkpoint) checkpoint.Delete();
                        suspect.Tasks.PerformDrivingManeuver(VehicleManeuver.Wait).WaitForCompletion(500);
                        suspect.Tasks.PerformDrivingManeuver(VehicleManeuver.GoForwardStraightHalfAcceleration).WaitForCompletion(1000);
                        suspect.Tasks.PerformDrivingManeuver(VehicleManeuver.Wait);
                        if (suspectVeh.HasRoof) suspectVeh.ConvertibleRoofState = VehicleConvertibleRoofState.Lowering;
                        GameFiber.Wait(2000);
                        Hooker.Tasks.Clear();
                        var walkPos = suspectVeh.Position + Vector3.RelativeRight * 1.5f;
                        Hooker.Tasks.FollowNavigationMeshToPosition(walkPos, walkPos.GetHeadingTowards(suspect), 1f).WaitForCompletion(15000);
                        suspectVeh.Windows[3].RollDown();
                        var chance = MyRandom.NextDouble();
                        if (suspect && suspectVeh && Math.Abs(Func.GetPersonaForPed(suspect).GetAge() - Func.GetPersonaForPed(Hooker).GetAge()) >= 20)
                        {
                            Logger.Log($"Hook unsuccessful, Age Different: {Math.Abs(Func.GetPersonaForPed(suspect).GetAge() - Func.GetPersonaForPed(Hooker).GetAge())}");
                            chance = 0.0005;
                        }
                        else if (richClasses.Any(x => suspectVeh.Class == x))
                        {
                            Logger.Log($"Suspect vehicle is special, chance of success increased");
                            if (MyRandom.NextDouble() < 0.8095285f)
                            {
                                chance = 0.9f;
                            }
                        }
                        var str = $"Chance: {chance:0.000}, ";
                        if (chance > 0.5f)
                        {
                            Logger.Log(str + "Hook Successful");
                            Hooker.Tasks.EnterVehicle(suspectVeh, 0).WaitForCompletion();
                            var speedRnd = MyRandom.Next(15, 21);
                            suspect.WanderWithVehicle(speedRnd, (VehicleDrivingFlags)0xC00AB);
                            SuspectCar = suspectVeh;
                            Suspect = suspect;
                            HookSuccess = true;
                            displayTimerBar = false;
                        }
                        else
                        {
                            Logger.Log(str + "Hook Unsuccessful");
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
                        suspectVeh.TopSpeed = topSpeed;
                    }
                    else Hooking = false;
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
