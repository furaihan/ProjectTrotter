﻿using BarbarianCall.API;
using BarbarianCall.Extensions;
using BarbarianCall.Freemode;
using BarbarianCall.Types;
using BarbarianCall.SupportUnit;
using LSPD_First_Response.Mod.Callouts;
using LSPDFR = LSPD_First_Response.Mod.API.Functions;
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Diagnostics;

namespace BarbarianCall.Callouts
{
    [CalloutInfo("Wanted Felon On The Loose", CalloutProbability.High)]
    class WantedFelonOnTheLoose : CalloutBase
    {
        private FreemodePed Driver;
        private FreemodePed Passenger1;
        private FreemodePed Passenger2;
        private FreemodePed Passenger3;
        private ESuspectStates DriverState;
        private ESuspectStates Passenger1State;
        private ESuspectStates Passenger2State;
        private ESuspectStates Passenger3State;
        bool CanEnd = false;
        private int ArrestedCount = 0;
        private int DeadCount = 0;
        private int EscapedCount = 0;
        private bool speedChanged = false;
        private RelationshipGroup Criminal;
        private HeliSupport heli;
        public override bool OnBeforeCalloutDisplayed()
        {
            DeclareVariable();
            CalloutRunning = false;
            PursuitCreated = false;
            Spawn = SpawnManager.GetVehicleSpawnPoint(PlayerPed.Position, 425, 725, true);
            if (Spawn == Spawnpoint.Zero) Spawn = SpawnManager.GetVehicleSpawnPoint2(PlayerPed.Position, 425, 725);
            if (Spawn == Spawnpoint.Zero) Spawn = SpawnManager.GetVehicleSpawnPoint3(PlayerPed.Position, 425, 725);
            if (Spawn == Spawnpoint.Zero) Spawn = SpawnManager.GetVehicleSpawnPoint(PlayerPed.Position, 350, 800);
            if (Spawn == Spawnpoint.Zero) Spawn = SpawnManager.GetVehicleSpawnPoint2(PlayerPed.Position, 350, 800);
            if (Spawn == Spawnpoint.Zero) Spawn = SpawnManager.GetVehicleSpawnPoint3(PlayerPed.Position, 350, 800);
            if (Spawn == Spawnpoint.Zero)
            {
                $"{GetType().Name} | Spawnpoint is not found, cancelling the callout".ToLog();
                return false;
            }
            Position = Spawn;
            SpawnHeading = Spawn;
            CarModel = Globals.AudibleCarModel.GetRandomElement(m => m.IsInCdImage && m.IsVehicle && m.IsCar && !m.IsBigVehicle && m.NumberOfSeats >= 4 && !m.IsLawEnforcementVehicle && !m.IsEmergencyVehicle, true);
            CarModel.LoadAndWait();
            SuspectCar = new Vehicle(CarModel, Spawn, Spawn);
            SuspectCar.SetRandomColor();
            SuspectCar.MakePersistent();
            SuspectCar.PlaceOnGroundProperly();
            CalloutAdvisory = string.Format("Vehicle Is: {0} {1}", SuspectCar.GetColor().PrimaryColorName, SuspectCar.GetDisplayName());
            CalloutMessage = "A Wanted Felon On The Loose";
            FriendlyName = "Wanted Felon On The Loose";
            PlayScannerWithCallsign("WE_HAVE CRIME_WANTED_FELON_ON_THE_LOOSE IN_OR_ON_POSITION", Spawn);
            CalloutPosition = Spawn;
            AddMinimumDistanceCheck(100, Spawn);
            ShowCalloutAreaBlipBeforeAccepting(Spawn, 80);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            CalloutRunning = true;
            Criminal = new RelationshipGroup("CRIMINAL");
            SuspectCar.RandomiseLicensePlate();
            SuspectCar.Metadata.BAR_Entity = true;
            Blip = new Blip(Spawn, 150)
            {
                Color = Yellow
            };
            Blip.EnableRoute(Yellow);
            Peralatan.ToLog($"{GetType().Name} | Preparing suspect...");
            Driver = new FreemodePed(Spawn, SpawnHeading, LSPD_First_Response.Gender.Male);
            Passenger1 = new FreemodePed(Position, SpawnHeading, LSPD_First_Response.Gender.Male);
            Passenger2 = new FreemodePed(Position, SpawnHeading, LSPD_First_Response.Gender.Male);
            Passenger3 = new FreemodePed(Position, SpawnHeading, LSPD_First_Response.Gender.Male);
            GameFiber.SleepUntil(() => Driver && Passenger1 && Passenger2 && Passenger3, 3000);
            Driver.MakePersistent();
            Passenger1.MakePersistent();
            Passenger2.MakePersistent();
            Passenger3.MakePersistent();
            SuspectCar.MakePersistent();
            Driver.SetRobberComponent();
            Passenger1.SetRobberComponent();
            Passenger2.SetRobberComponent();
            Passenger3.SetRobberComponent();
            CalloutEntities.Add(Driver);
            CalloutEntities.Add(Passenger1);
            CalloutEntities.Add(Passenger2);
            CalloutEntities.Add(Passenger3);
            Driver.WarpIntoVehicle(SuspectCar, -1);
            Passenger1.WarpIntoVehicle(SuspectCar, 0);
            Passenger2.WarpIntoVehicle(SuspectCar, 1);
            Passenger3.WarpIntoVehicle(SuspectCar, 2);
            DriverState = ESuspectStates.InAction;
            Passenger1State = ESuspectStates.InAction;
            Passenger2State = ESuspectStates.InAction;
            Passenger3State = ESuspectStates.InAction;
            var drivePos = SpawnManager.GetRandomFleeingPoint(Driver);
            Driver.VehicleMission(PlayerPed, MissionType.Flee, 35f, VehicleDrivingFlags.Normal, 350f, 15f, true);
            int num = Peralatan.Random.Next(4);
            switch (num)
            {
                case 0:
                    SituationGetOutAndRun();
                    break;
                case 1:
                    SituationWar();
                    break;
                case 2:
                    SituationPursuit();
                    break;
                default:
                    SituationTrafficStopWar();
                    break;
            }
            CalloutMainFiber.Start();
            GameFiber.StartNew(() =>
            {
                Ped wanted = (new[] { Driver, Passenger1, Passenger2, Passenger3 }).GetRandomElement();
                Manusia = new Manusia(wanted, LSPDFRFunc.GetPedPersona(wanted), SuspectCar);
                LSPDFRFunc.WaitAudioScannerCompletion();
                GameFiber.Sleep(1000);
                Manusia.DisplayNotif();
                GameFiber.Wait(1500);
                LSPDFRFunc.WaitAudioScannerCompletion();
                LSPDFRFunc.PlayScannerAudio($"SUSPECT_IS DRIVING_A {SuspectCar.GetColorAudio()} {Peralatan.GetPoliceScannerAudio(SuspectCar)} BAR_TARGET_PLATE {Peralatan.GetLicensePlateAudio(SuspectCar)}", true);
                GameFiber.Wait(2500);
                DisplayGPNotif();
            });
            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            if (CalloutRunning)
            {
                if (DriverState == ESuspectStates.InAction && Driver && Driver.IsDead)
                {
                    DriverState = ESuspectStates.Dead;
                    DeadCount++;
                    Blip blip = Driver ? Driver.GetAttachedBlips().Where(b => CalloutBlips.Contains(b)).FirstOrDefault() : null;
                    if (blip) blip.Delete();
                }
                if (Passenger1State == ESuspectStates.InAction && Passenger1 && Passenger1.IsDead)
                {
                    Passenger1State = ESuspectStates.Dead;
                    DeadCount++;
                    Blip blip = Passenger1 ? Passenger1.GetAttachedBlips().Where(b => CalloutBlips.Contains(b)).FirstOrDefault() : null;
                    if (blip) blip.Delete();
                }
                if (Passenger2State == ESuspectStates.InAction && Passenger2 && Passenger2.IsDead)
                {
                    Passenger2State = ESuspectStates.Dead;
                    DeadCount++;
                    Blip blip = Passenger2 ? Passenger2.GetAttachedBlips().Where(b => CalloutBlips.Contains(b)).FirstOrDefault() : null;
                    if (blip) blip.Delete();
                }
                if (Passenger3State == ESuspectStates.InAction && Passenger3 && Passenger3.IsDead)
                {
                    Passenger3State = ESuspectStates.Dead;
                    DeadCount++;
                    Blip blip = Passenger3 ? Passenger3.GetAttachedBlips().Where(b => CalloutBlips.Contains(b)).FirstOrDefault() : null;
                    if (blip) blip.Delete();
                }
                if (DriverState == ESuspectStates.InAction && Driver && LSPDFR.IsPedArrested(Driver))
                {
                    DriverState = ESuspectStates.Arrested;
                    ArrestedCount++;
                    Blip blip = Driver ? Driver.GetAttachedBlips().Where(b => CalloutBlips.Contains(b)).FirstOrDefault() : null;
                    if (blip) blip.Delete();
                }
                if (Passenger1State == ESuspectStates.InAction && Passenger1 && LSPDFR.IsPedArrested(Passenger1))
                {
                    Passenger1State = ESuspectStates.Arrested;
                    ArrestedCount++;
                    Blip blip = Passenger1 ? Passenger1.GetAttachedBlips().Where(b => CalloutBlips.Contains(b)).FirstOrDefault() : null;
                    if (blip)
                        blip.Delete();
                }
                if (Passenger2State == ESuspectStates.InAction && Passenger2 && LSPDFR.IsPedArrested(Passenger2))
                {
                    Passenger2State = ESuspectStates.Arrested;
                    ArrestedCount++;
                    Blip blip = Passenger2 ? Passenger2.GetAttachedBlips().Where(b => CalloutBlips.Contains(b)).FirstOrDefault() : null;
                    if (blip)
                        blip.Delete();
                }
                if (Passenger3State == ESuspectStates.InAction && Passenger3 && LSPDFR.IsPedArrested(Passenger3))
                {
                    Passenger3State = ESuspectStates.Arrested;
                    ArrestedCount++;
                    Blip blip = Passenger3 ? Passenger3.GetAttachedBlips().Where(b => CalloutBlips.Contains(b)).FirstOrDefault() : null;
                    if (blip)
                        blip.Delete();
                }
                if (DriverState == ESuspectStates.InAction && !Driver)
                {
                    DriverState = ESuspectStates.Escaped;
                    EscapedCount++;
                }
                if (Passenger1State == ESuspectStates.InAction && !Passenger1)
                {
                    Passenger1State = ESuspectStates.Escaped;
                    EscapedCount++;
                }
                if (Passenger2State == ESuspectStates.InAction && !Passenger2)
                {
                    Passenger2State = ESuspectStates.Escaped;
                    EscapedCount++;
                }
                if (Passenger3State == ESuspectStates.InAction && !Passenger3)
                {
                    Passenger3State = ESuspectStates.Escaped;
                    EscapedCount++;
                }
                if (DriverState != ESuspectStates.InAction &&
                    Passenger1State != ESuspectStates.InAction &&
                    Passenger2State != ESuspectStates.InAction &&
                    Passenger3State != ESuspectStates.InAction)
                    CanEnd = true;
                if (PursuitCreated && !speedChanged)
                {
                    SuspectCar.TopSpeed = 125f;
                    speedChanged = true;
                }
            }
        }
        public override void End()
        {
            if (heli != null) heli?.CleanUp();
            base.End();
        }
        public void GetClose()
        {
            var curPos = SuspectCar.Position;
            StopWatch = Stopwatch.StartNew();
            Stopwatch heliTimer = Stopwatch.StartNew();
            Stopwatch stuckTimer = new();
            bool heliCalled = false;
            bool notifDisp = false;
            while (CalloutRunning)
            {
                Rage.Debug.DrawLine(PlayerPed.IsInAnyVehicle(false) ? PlayerPed.CurrentVehicle.FrontPosition : PlayerPed.FrontPosition, SuspectCar.FrontPosition, Color.DeepPink);
                Marker.DrawMarker(SuspectCar, MarkerType.UpsideDownCone, Color.LightSkyBlue, new Vector3(1, 1, 1), 1);
                GameFiber.Yield();
                if (!SuspectCar)
                {
                    CalloutRunning = false;
                    End();
                    break;
                }
                if (StopWatch.ElapsedMilliseconds > 20000 && SuspectCar.DistanceToSquared(curPos) > 2500f)
                {
                    if (!heliCalled)
                    {
                        curPos = SuspectCar.Position;
                        if (Blip) Blip.Delete();
                        Blip = new Blip(SuspectCar.Position.Around2D(20f), 150f)
                        {
                            Color = Yellow
                        };
                        Blip.EnableRoute(Yellow);
                    }                   
                    LSPDFRFunc.PlayScannerAudioUsingPosition(string.Format("SUSPECT_HEADING {0} IN_OR_ON_POSITION", SuspectCar.GetCardinalDirectionLowDetailedAudio()), SuspectCar.Position);
                    StopWatch.Restart();
                }
                if (PlayerPed.DistanceToSquared(SuspectCar) < 625f)
                {
                    if (Blip) Blip.Delete();
                    if (heli != null) heli?.CleanUp();
                    break;
                }
                SuspectCar.Repair();
                SuspectCar.IsDriveable = true;
                if (SuspectCar.IsUpsideDown) SuspectCar.PlaceOnGroundProperly();
                if (SuspectCar.IsStuckOnRoof() || SuspectCar.IsInAir || !SuspectCar.IsOnAllWheels || SuspectCar.IsInWater)
                {
                    if (stuckTimer.IsRunning)
                    {
                        if (stuckTimer.ElapsedMilliseconds > 6000) stuckTimer.Restart();
                    }
                    else
                    {
                        "Suspect stuck, recalibrating..".ToLog();
                        stuckTimer.Start();
                    }
                }
                if (stuckTimer.IsRunning && stuckTimer.ElapsedMilliseconds > 5000 && (SuspectCar.IsStuckOnRoof() || SuspectCar.IsInAir || !SuspectCar.IsOnAllWheels || SuspectCar.IsInWater))
                {
                    Spawnpoint sp = SpawnManager.GetVehicleSpawnPoint(curPos, 20.0f, 50.0f);
                    if (sp == Spawnpoint.Zero) sp = SpawnManager.GetVehicleSpawnPoint(curPos, 25.0f, 60.0f);
                    if (sp == Spawnpoint.Zero) sp = SpawnManager.GetVehicleSpawnPoint(curPos, 20.0f, 75.0f);
                    if (sp == Spawnpoint.Zero) sp = SpawnManager.GetVehicleSpawnPoint(curPos, 15.0f, 100.0f);
                    if (sp == Spawnpoint.Zero) sp = SpawnManager.GetVehicleSpawnPoint(curPos, 10.0f, 150.0f);
                    if (sp == Spawnpoint.Zero) continue;
                    SuspectCar.Position = sp;
                    SuspectCar.Heading = sp;
                    SuspectCar.PlaceOnGroundProperly();
                    stuckTimer.Reset();
                }
                if (!heliCalled && heliTimer.ElapsedMilliseconds > 180000)
                {
                    if (!notifDisp)
                    {
                        $"Tired of searching? Press ~b~Right Alt Key~s~ to call an air unit to help you find the suspect"
                            .DisplayNotifWithLogo(fadeIn: true, blink: true, hudColor: RAGENativeUI.HudColor.TechGreenDark, icon: NotificationIcon.RightJumpingArrow, subtitle: "Wanted Felon Callout");
                        notifDisp = true;
                    }
                    if (!heliCalled && Game.IsKeyDown(System.Windows.Forms.Keys.RMenu))
                    {
                        heli = new HeliSupport(SuspectCar);
                        if (Blip) Blip.Delete();
                        Blip = new Blip(SuspectCar)
                        {
                            IsRouteEnabled = true,
                            RouteColor = Yellow,
                            Color = Color.Red,
                            Name = "Felon",
                        };
                        heliCalled = true;
                    }                   
                }
            }
        }
        private void DisplayCodeFourMessage()
        {
            if (!CalloutRunning) return;
            string message = string.Format("~b~Total Suspect~s~: 4~n~~g~Arrested~s~: {0}~n~~o~Dead~s~: {1}~n~~r~Escaped~s~: {2}", ArrestedCount, DeadCount, EscapedCount);
            message.DisplayNotifWithLogo("~g~Suspect Summary");
            PlayScannerWithCallsign("WE_ARE_CODE_4");
            End();
        }
        public void SetRelationship()
        {
            Criminal.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
            Criminal.SetRelationshipWith(RelationshipGroup.Medic, Relationship.Hate);
            Criminal.SetRelationshipWith(RelationshipGroup.Fireman, Relationship.Hate);
            Criminal.SetRelationshipWith(RelationshipGroup.Player, Relationship.Hate);
            RelationshipGroup.Cop.SetRelationshipWith(Criminal, Relationship.Hate);
            RelationshipGroup.Fireman.SetRelationshipWith(Criminal, Relationship.Hate);
            RelationshipGroup.Medic.SetRelationshipWith(Criminal, Relationship.Hate);
        }
        public void SituationWar()
        {
            CalloutMainFiber = new GameFiber(() =>
            {
                try
                {
                    List<FreemodePed> Suspects = new() { Driver, Passenger1, Passenger2, Passenger3 };                   
                    if (Suspects.All(s => s)) Suspects.ForEach(s => s.RelationshipGroup = Criminal);
                    GameFiber.Wait(75);
                    SetRelationship();
                    if (Suspects.All(s => s)) Suspects.ForEach(s => s.SetPedAsWanted());
                    GameFiber.Wait(75);
                    if (Suspects.All(s => s)) Suspects.ForEach(s => s.MakeMissionPed());
                    GameFiber.Wait(75);
                    GetClose();
                    if (!CalloutRunning) return;
                    if (Driver) Driver.VehicleMission(PlayerPed, MissionType.Stop, 5f, (VehicleDrivingFlags)1076627627, -1f, 1f, true).WaitForCompletion(550);
                    if (Suspects.All(s => s)) Suspects.ForEach(s => s.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen));
                    if (Suspects.All(s => s)) GameFiber.WaitUntil(() => Suspects.All(s => s.IsOnFoot), 5000);
                    GameFiber.Wait(75);
                    if (Suspects.All(s => s)) Suspects.ForEach(s =>
                     {
                         var blip = new Blip(s)
                         {
                             Color = Color.Red,
                             Scale = 0.75f,
                             IsFriendly = false,
                         };
                         CalloutBlips.Add(blip);
                     });
                    GameFiber.Wait(75);
                    if (Suspects.All(s => s)) Suspects.ForEach(s => s.Inventory.GiveNewWeapon(WeaponHashes.GetRandomElement(true), -1, true));
                    GameFiber.Wait(75);
                    if (Suspects.All(s => s)) Suspects.ForEach(s => 
                    {
                        s.Tasks.FightAgainstClosestHatedTarget(1000f);
                    });
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        Suspects.ForEach(s =>
                        {
                            if (s)
                            {
                                if (s.IsTaskActive(PedTask.DoNothing))
                                {
                                    s.Tasks.FightAgainstClosestHatedTarget(1000f);
                                    GameFiber.Sleep(2500);
                                }
                            }
                        });
                        if (CanEnd) break;
                    }
                    DisplayCodeFourMessage();
                }
                catch (Exception e)
                {
                    $"{GetType().Name} callout crashes".ToLog();
                    e.ToString().ToLog();
                    NetExtension.SendError(e);
                    $"{GetType().Name} callout crashed, please send your log".DisplayNotifWithLogo("Officer Stabbed");
                    End();
                }
            });
        }
        public void SituationGetOutAndRun()
        {
            CalloutMainFiber = new GameFiber(() =>
            {
                try
                {
                    List<FreemodePed> Suspects = new() { Driver, Passenger1, Passenger2, Passenger3 };                        
                    if (Suspects.All(s => s)) Suspects.ForEach(s => s.RelationshipGroup = Criminal);
                    GameFiber.Wait(75);
                    SetRelationship();
                    if (Suspects.All(s => s)) Suspects.ForEach(s => s.SetPedAsWanted());
                    GameFiber.Wait(75);
                    if (Suspects.All(s => s)) Suspects.ForEach(s => s.MakeMissionPed());
                    GameFiber.Wait(75);
                    GetClose();
                    if (!CalloutRunning) return;
                    double ch = Peralatan.Random.NextDouble();
                    string.Format("Get out scenario: {0}", ch).ToLog();
                    List<Ped> getOutPeds;
                    if (ch < 0.275)
                    {
                        getOutPeds = new List<Ped>() { SuspectCar.Passengers.GetRandomElement() };
                    }
                    else if (ch < 0.7980)
                    {
                        getOutPeds = SuspectCar.Passengers.GetRandomNumberOfElements(2).ToList();
                    }
                    else
                    {
                        getOutPeds = SuspectCar.Passengers.ToList();
                    }
                    getOutPeds.ForEach(p => { if (p) p.Tasks.LeaveVehicle(LeaveVehicleFlags.BailOut); });
                    GameFiber.SleepUntil(() => getOutPeds.All(p => p && !p.IsInAnyVehicle(false)), 2500);
                    getOutPeds.ForEach(p => { if (p) p.Tasks.Flee(PlayerPed, 1000f, -1); });
                    Pursuit = LSPDFR.CreatePursuit();
                    Suspects.ForEach(p =>
                    {
                        if (p)
                        {
                            p.Inventory.GiveNewWeapon(WeaponHashes.GetRandomElement(), -1, false);
                            LSPDFR.AddPedToPursuit(Pursuit, p);
                            var item = "~r~" + Globals.DangerousPedItem.GetRandomElement();
                            if (StopThePedRunning) StopThePedFunc.InjectPedItem(p, item);
                            LSPDFR.AddPedContraband(p, LSPD_First_Response.Engine.Scripting.Entities.ContrabandType.Misc, item);
                        }
                    });
                    if (StopThePedRunning) StopThePedFunc.InjectVehicleItem(SuspectCar, "~r~" + Globals.DangerousVehicleItems.GetRandomElement(), StopThePedFunc.EStopThePedVehicleSearch.SearchTrunk);
                    SuspectCar.TopSpeed = 125f;
                    GameFiber.Wait(3500);
                    PursuitCreated = true;
                    if (GrammarPoliceRunning) GrammarPoliceFunc.SetStatus(GrammarPoliceFunc.EGrammarPoliceStatusType.InPursuit);
                    LSPDFR.SetPursuitIsActiveForPlayer(Pursuit, true);
                    LSPDFRFunc.RequestBackup(Driver.Position, LSPD_First_Response.EBackupResponseType.Pursuit);
                    StopWatch = Stopwatch.StartNew();
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        if (CanEnd) break;
                        if (StopWatch.ElapsedMilliseconds > 6000L && LSPDFR.IsPursuitStillRunning(Pursuit))
                        {
                            LSPDFRFunc.RequestAirUnit(Driver.Position, LSPD_First_Response.EBackupResponseType.Pursuit);
                            LSPDFRFunc.RequestBackup(PlayerPed.Position, LSPD_First_Response.EBackupResponseType.Pursuit);
                            StopWatch.Reset();
                        }
                    }
                    DisplayCodeFourMessage();
                }
                catch (Exception e)
                {
                    $"{GetType().Name} callout crashes".ToLog();
                    e.ToString().ToLog();
                    NetExtension.SendError(e);
                    $"{GetType().Name} callout crashed, please send your log".DisplayNotifWithLogo("Officer Stabbed");
                    End();
                }
            });
        }
        private void SituationTrafficStopWar()
        {
            CalloutMainFiber = new GameFiber(() =>
            {
                try
                {
                    List<FreemodePed> Suspects = new() { Driver, Passenger1, Passenger2, Passenger3 };
                    if (Suspects.All(s => s)) Suspects.ForEach(s => s.RelationshipGroup = Criminal);
                    GameFiber.Wait(75);
                    SetRelationship();
                    if (Suspects.All(s => s)) Suspects.ForEach(s => s.SetPedAsWanted());
                    GameFiber.Wait(75);
                    if (Suspects.All(s => s)) Suspects.ForEach(s => s.MakeMissionPed());
                    GameFiber.Wait(75);
                    GetClose();
                    if (!CalloutRunning) return;
                    if (Blip) Blip.Delete();
                    Blip = SuspectCar.AttachBlip();
                    Blip.Color = Color.Red;
                    StopWatch = Stopwatch.StartNew();
                    Game.DisplayHelp("Please perform a pullover on a ~r~suspect");
                    bool pulloverDetected = false;
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        if (StopWatch.ElapsedMilliseconds > 18000L && !pulloverDetected)
                        {
                            Game.DisplayHelp("Please perform a pullover on a ~r~suspect");
                            StopWatch.Restart();
                        }
                        if (LSPDFR.IsPlayerPerformingPullover())
                        {
                            PullOver = LSPDFR.GetCurrentPullover();
                            if (PullOver != null && Suspects.Contains(LSPDFR.GetPulloverSuspect(PullOver)))
                            {
                                if (!pulloverDetected) Peralatan.ToLog("Pullover on suspect detected, waiting player to leave his vehicle");
                                pulloverDetected = true;
                                if (PlayerPed.IsOnFoot && PlayerPed.Speed > 0f)
                                {
                                    Peralatan.ToLog("Player has leave his vehicle, breaking the loop");
                                    LSPDFR.ForceEndCurrentPullover();
                                    Suspects.ForEach(s => s.MakeMissionPed());
                                    break;
                                }
                            }
                        }
                        if (LSPDFR.GetActivePursuit() != null)
                        {
                            Pursuit = LSPDFR.GetActivePursuit();
                            var pursuitPeds = LSPDFR.GetPursuitPeds(Pursuit);
                            if (pursuitPeds.Any(s => s && Suspects.Contains(s)))
                            {
                                Peralatan.ToLog("Suspect is fleeing, setting up a pursuit");
                                PursuitCreated = true;
                                Suspects.ForEach(s =>
                                {
                                    if (s)
                                    {
                                        s.Inventory.GiveNewWeapon(WeaponHashes.GetRandomElement(true), -1, false);
                                        if (!pursuitPeds.Contains(s)) LSPDFR.AddPedToPursuit(Pursuit, s);
                                        var ppa = LSPDFR.GetPedPursuitAttributes(s);
                                        ppa.AverageFightTime = 30;
                                    }
                                });
                                LSPDFR.SetPursuitIsActiveForPlayer(Pursuit, true);
                                LSPDFR.SetPursuitCopsCanJoin(Pursuit, true);
                                LSPDFRFunc.RequestBackup(PlayerPed.Position, LSPD_First_Response.EBackupResponseType.Pursuit);
                                break;
                            }
                        }
                    }
                    if (!CalloutRunning) return;
                    if (!PursuitCreated)
                    {
                        Peralatan.ToLog("Telling all suspect to leave their vehicle");
                        if (Suspects.All(s => s)) Suspects.ForEach(s => s.Tasks.LeaveVehicle(LeaveVehicleFlags.None));
                        GameFiber.SleepUntil(() => Suspects.All(s => s && s.IsOnFoot), 3500);
                        Suspects.ForEach(s =>
                        {
                            if (s)
                            {
                                if (s.IsInVehicle(SuspectCar, true))
                                {
                                    s.Tasks.LeaveVehicle(SuspectCar, LeaveVehicleFlags.WarpOut);
                                }
                                s.Inventory.GiveNewWeapon(WeaponHashes.GetRandomElement(true), -1, true);
                                s.PlayAmbientSpeech(Speech.GENERIC_FUCK_YOU);
                                s.Tasks.FightAgainstClosestHatedTarget(1000f);
                            }
                        });
                    }
                    StopWatch = Stopwatch.StartNew();
                    int waitTime = Peralatan.Random.Next(6500, 10001);
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        if (CanEnd) break;
                        Suspects.ForEach(s =>
                        {
                            if (s)
                            {
                                if (s.IsTaskActive(PedTask.DoNothing))
                                {
                                    s.Tasks.FightAgainstClosestHatedTarget(1000f);
                                    GameFiber.Sleep(2500);
                                }
                            }
                        });
                        if (PursuitCreated && StopWatch.ElapsedMilliseconds > waitTime)
                        {
                            LSPDFRFunc.RequestAirUnit(SuspectCar.Position, LSPD_First_Response.EBackupResponseType.Pursuit);
                            StopWatch.Reset();
                        }
                    }
                    DisplayCodeFourMessage();
                }
                catch (Exception e)
                {
                    $"{GetType().Name} callout crashes".ToLog();
                    e.ToString().ToLog();
                    NetExtension.SendError(e);
                    $"{GetType().Name} callout crashed, please send your log".DisplayNotifWithLogo("Officer Stabbed");
                    End();
                }
            });
        }
        private void SituationPursuit()
        {
            CalloutMainFiber = new GameFiber(() =>
            {
                try
                {
                    List<FreemodePed> Suspects = new() { Driver, Passenger1, Passenger2, Passenger3 };
                    FreemodePed wanted = Suspects.GetRandomElement(fp => fp);
                    if (wanted) Manusia = new Manusia(wanted, LSPDFRFunc.GetPedPersona(wanted), SuspectCar);
                    GameFiber.StartNew(() =>
                    {
                        LSPDFRFunc.WaitAudioScannerCompletion();
                        GameFiber.Sleep(1000);
                        if (Manusia != null) Manusia.DisplayNotif();
                        GameFiber.Wait(1500);
                        LSPDFRFunc.WaitAudioScannerCompletion();
                        LSPDFRFunc.PlayScannerAudio(string.Format("VEHICLE BAR_IS BAR_A_CONJ {0} {1} BAR_TARGET_PLATE {2}",
                            SuspectCar.GetColorAudio(), Peralatan.GetPoliceScannerAudio(SuspectCar), Peralatan.GetLicensePlateAudio(SuspectCar)), true);
                        GameFiber.Wait(2500);
                        DisplayGPNotif();
                    });
                    if (Suspects.All(s => s)) Suspects.ForEach(s => s.RelationshipGroup = Criminal);
                    GameFiber.Wait(75);
                    SetRelationship();
                    if (Suspects.All(s => s)) Suspects.ForEach(s => s.SetPedAsWanted());
                    GameFiber.Wait(75);
                    if (Suspects.All(s => s)) Suspects.ForEach(s => s.MakeMissionPed());
                    GameFiber.Wait(75);
                    GetClose();
                    if (!CalloutRunning) return;
                    if (Blip) Blip.Delete();
                    Pursuit = LSPDFR.CreatePursuit();
                    Suspects.ForEach(s =>
                    {
                        if (s)
                        {
                            LSPDFR.AddPedToPursuit(Pursuit, s);
                            var att = LSPDFR.GetPedPursuitAttributes(s);
                            att.AverageFightTime = 15;
                            if (s == Driver)
                            {
                                att.SurrenderChancePittedAndCrashed = 10f;
                                att.SurrenderChancePittedAndSlowedDown = 10f;
                                att.SurrenderChanceTireBurst = 10f;
                            }
                        }
                    });
                    PursuitCreated = true;
                    LSPDFR.SetPursuitIsActiveForPlayer(Pursuit, true);
                    LSPDFR.SetPursuitLethalForceForced(Pursuit, true);
                    LSPDFR.SetPursuitCopsCanJoin(Pursuit, true);
                    LSPDFRFunc.RequestBackup(SuspectCar.Position, LSPD_First_Response.EBackupResponseType.Pursuit);
                    StopWatch = Stopwatch.StartNew();
                    bool heliRequested = false;
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        if (StopWatch.ElapsedMilliseconds > 8500 && LSPDFR.IsPursuitStillRunning(Pursuit) && !heliRequested)
                        {
                            LSPDFRFunc.RequestAirUnit(SuspectCar.Position, LSPD_First_Response.EBackupResponseType.Pursuit);
                            StopWatch.Stop();
                            heliRequested = true;
                        }
                        if (CanEnd) break;
                    }
                    DisplayCodeFourMessage();
                }
                catch (Exception e)
                {
                    $"{GetType().Name} callout crashes".ToLog();
                    e.ToString().ToLog();
                    NetExtension.SendError(e);
                    $"{GetType().Name} callout crashed, please send your log".DisplayNotifWithLogo("Officer Stabbed");
                    End();
                }
            });
        }
    }
}
