using BarbarianCall.API;
using BarbarianCall.Extensions;
using BarbarianCall.FreemodeUtil;
using BarbarianCall.Types;
using LSPD_First_Response.Mod.Callouts;
using LSPDFR = LSPD_First_Response.Mod.API.Functions;
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

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
        private readonly RelationshipGroup CriminalRelationShip = "CRIMINAL";
        private int ArrestedCouunt = 0;
        private int DeadCount = 0;
        private int EscapedCount = 0;
        public override bool OnBeforeCalloutDisplayed()
        {
            CheckOtherPluginRunning();
            Spawn = SpawnManager.GetVehicleSpawnPoint(PlayerPed, 425, 725);
            if (Spawn == Spawnpoint.Zero) Spawn = SpawnManager.GetVehicleSpawnPoint(PlayerPed, 350, 650);
            if (Spawn == Spawnpoint.Zero) Spawn.Position = World.GetNextPositionOnStreet(PlayerPed.Position.Around2D(425, 725));
            SpawnPoint = Spawn;
            SpawnHeading = Spawn;
            CarModel = CommonVariables.CarsToSelect.GetRandomElement(m => m.IsValid && CommonVariables.AudibleCarModel.Contains(m) && m.NumberOfSeats == 4, true);
            CarModel.LoadAndWait();
            SuspectCar = new Vehicle(CarModel, SpawnPoint, SpawnHeading);
            SuspectCar.PrimaryColor = CommonVariables.AudibleColor.GetRandomElement();
            SuspectCar.MakePersistent();
            SuspectCar.PlaceOnGroundProperly();
            CalloutAdvisory = string.Format("Vehicle Is: {0} {1}", SuspectCar.GetCarColor(), SuspectCar.GetVehicleDisplayName());
            CalloutMessage = "A Wanted Felon On The Loose";
            PlayScannerWithCallsign("WE_HAVE CRIME_WANTED_FELON_ON_THE_LOOSE IN_OR_ON_POSITION", Spawn);
            CalloutPosition = Spawn;
            AddMinimumDistanceCheck(100, Spawn);
            ShowCalloutAreaBlipBeforeAccepting(Spawn, 80);

            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            CalloutRunning = true;
            SuspectCar.RandomiseLicensePlate();
            Driver = new FreemodePed(Spawn, SpawnHeading, LSPD_First_Response.Gender.Male);
            Passenger1 = new FreemodePed(SpawnPoint, SpawnHeading, LSPD_First_Response.Gender.Male);
            Passenger2 = new FreemodePed(SpawnPoint, SpawnHeading, LSPD_First_Response.Gender.Male);
            Passenger3 = new FreemodePed(SpawnPoint, SpawnHeading, LSPD_First_Response.Gender.Male);
            Driver.MakePersistent();
            Passenger1.MakePersistent();
            Passenger2.MakePersistent();
            Passenger3.MakePersistent();
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
            Driver.Tasks.CruiseWithVehicle(45, VehicleDrivingFlags.Normal);
            Blip = new Blip(Spawn, 80);
            Blip.Color = Yellow;
            Blip.EnableRoute(Yellow);
            SituationWar();
            CalloutMainFiber.Start();
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
                }
                if (Passenger1State == ESuspectStates.InAction && Passenger1 && Passenger1.IsDead)
                { 
                    Passenger1State = ESuspectStates.Dead;
                    DeadCount++; 
                }
                if (Passenger2State == ESuspectStates.InAction && Passenger2 && Passenger2.IsDead)
                {
                    Passenger2State = ESuspectStates.Dead;
                    DeadCount++;
                }
                if (Passenger3State == ESuspectStates.InAction && Passenger3 && Passenger3.IsDead)
                {
                    Passenger3State = ESuspectStates.Dead;
                    DeadCount++;
                }
                if (DriverState == ESuspectStates.InAction && Driver && LSPDFR.IsPedArrested(Driver)) 
                { 
                    DriverState = ESuspectStates.Arrested;
                    ArrestedCouunt++; 
                }
                if (Passenger1State == ESuspectStates.InAction && Passenger1 && LSPDFR.IsPedArrested(Passenger1)) 
                { 
                    Passenger1State = ESuspectStates.Arrested; 
                    ArrestedCouunt++; 
                }
                if (Passenger2State == ESuspectStates.InAction && Passenger2 && LSPDFR.IsPedArrested(Passenger2)) {
                    Passenger2State = ESuspectStates.Arrested;
                    ArrestedCouunt++;
                }
                if (Passenger3State == ESuspectStates.InAction && Passenger3 && LSPDFR.IsPedArrested(Passenger3)) {
                    Passenger3State = ESuspectStates.Arrested;
                    ArrestedCouunt++;
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
                if (Passenger2State == ESuspectStates.InAction && !Passenger2) {
                    Passenger2State = ESuspectStates.Escaped;
                    EscapedCount++;
                }
                if (Passenger3State == ESuspectStates.InAction && !Passenger3) {
                    Passenger3State = ESuspectStates.Escaped;
                    EscapedCount++;
                }
                if (DriverState != ESuspectStates.InAction && Passenger1State != ESuspectStates.InAction && Passenger2State != ESuspectStates.InAction && Passenger3State != ESuspectStates.InAction) CanEnd = true;
            }
        }
        public void GetClose()
        {
            var curPos = SuspectCar.Position;
            StopWatch = System.Diagnostics.Stopwatch.StartNew();
            while (CalloutRunning)
            {
                GameFiber.Yield();
                if (!SuspectCar)
                {
                    CalloutRunning = false;
                    End();
                    break;
                }
                if (StopWatch.ElapsedMilliseconds > 20000 && SuspectCar.DistanceTo(curPos) > 50f)
                {
                    StopWatch.Restart();
                    curPos = SuspectCar.Position;
                    if (Blip) Blip.Delete();
                    Blip = new Blip(SuspectCar.Position, 80f);
                    Blip.Color = Yellow;
                    Blip.EnableRoute(Yellow);
                    LSPDFRFunc.PlayScannerAudioUsingPosition(string.Format("SUSPECT_HEADING {0} IN_OR_ON_POSITION", SuspectCar.GetCardinalDirectionLowDetailedAudio()), SuspectCar.Position, true);
                }
                if (PlayerPed.DistanceTo(SuspectCar) < 25f && SuspectCar.IsOnScreen)
                {
                    if (Blip) Blip.Delete();
                    break;
                }
            }
        }
        private void DisplayCodeFourMessage()
        {
            if (!CalloutRunning) return;
            string message = string.Format("~y~Total Suspect~s~: 4~n~~y~Arrested~s~: {0}~n~~y~Dead~s~: {1}~n~~y~Escaped~s~: {2}", ArrestedCouunt, DeadCount, EscapedCount);
            message.DisplayNotifWithLogo("~g~Suspect Summary");
            PlayScannerWithCallsign("WE_ARE_CODE_4");
            End();
        }
        public void SetRelationship()
        {
            RelationshipGroup.Cop.SetRelationshipWith(CriminalRelationShip, Relationship.Hate);
            RelationshipGroup.Medic.SetRelationshipWith(CriminalRelationShip, Relationship.Hate);
            RelationshipGroup.Fireman.SetRelationshipWith(CriminalRelationShip, Relationship.Hate);
            CriminalRelationShip.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
            CriminalRelationShip.SetRelationshipWith(RelationshipGroup.Medic, Relationship.Hate);
            CriminalRelationShip.SetRelationshipWith(RelationshipGroup.Fireman, Relationship.Hate);
            CriminalRelationShip.SetRelationshipWith(RelationshipGroup.Player, Relationship.Hate);
        }
        public void SituationWar()
        {
            CalloutMainFiber = new GameFiber(() =>
            {
                try
                {
                    Manusia = new Manusia(Driver, LSPDFRFunc.GetPedPersona(Driver), SuspectCar);
                    GameFiber.StartNew(() =>
                    {
                        LSPDFRFunc.WaitAudioScannerCompletion();
                        GameFiber.Sleep(1000);
                        Manusia.DisplayNotif();
                        GameFiber.Wait(1500);
                        LSPDFRFunc.WaitAudioScannerCompletion();
                        LSPDFRFunc.PlayScannerAudio(string.Format("VEHICLE BAR_IS BAR_A_CONJ {0} {1} BAR_TARGET_PLATE {2}",
                            SuspectCar.GetColorAudio(), Peralatan.GetVehicleDisplayAudio(SuspectCar), Peralatan.GetLicensePlateAudio(SuspectCar)), true);
                        GameFiber.Wait(2500);
                        DisplayGPNotif();
                    });
                    List<FreemodePed> Suspects = new List<FreemodePed>() { Driver, Passenger1, Passenger2, Passenger3 };
                    if (Suspects.All(s => s)) Suspects.ForEach(s => s.RelationshipGroup = CriminalRelationShip);
                    GameFiber.Wait(75);
                    SetRelationship();
                    if (Suspects.All(s => s)) Suspects.ForEach(s => s.SetPedAsWanted());
                    GameFiber.Wait(75);
                    if (Suspects.All(s => s)) Suspects.ForEach(s => s.MakeMissionPed());
                    GameFiber.Wait(75);
                    GetClose();
                    if (!CalloutRunning) return;
                    if (Driver) Driver.Tasks.PerformDrivingManeuver(VehicleManeuver.GoForwardStraightBraking).WaitForCompletion(500);
                    if (Suspects.All(s => s)) Suspects.ForEach(s => s.Tasks.LeaveVehicle(LeaveVehicleFlags.None));
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
                    if (Suspects.All(s => s)) Suspects.ForEach(s => s.Tasks.FightAgainst(PlayerPed));
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        if (Suspects.All(s => s)) if (Suspects.Any(s => s.Tasks.CurrentTaskStatus == TaskStatus.Interrupted && s.IsAlive)) Suspects.ForEach(s => s.Tasks.FightAgainst(PlayerPed));
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
