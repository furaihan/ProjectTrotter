﻿using System;
using System.Collections.Generic;
using System.Linq;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Engine.Scripting.Entities;
using System.Drawing;
using ProjectTrotter.Types;
using System.IO;
using ProjectTrotter.Extensions;

namespace ProjectTrotter.Callouts
{
    [CalloutInfo("Officer Stabbed", CalloutProbability.Medium)]
    public class OfficerStabbed : CalloutBase
    {
        private Ped officer;
        private Ped passenger;
        private Vehicle offVeh;
        private Model susVehModel;
        //private Persona SuspectPersona;
        private string susVehColor;
        private ESuspectStates SuspectState;
        private ESuspectStates PassengerState;
        private WeaponDescriptor passengerWeapon;

        private readonly string[] CityCarModels = new string[] { "POLICE", "POLICE2", "POLICE3", "POLICE4" };
        private readonly string[] CountrysideCarModels = new string[] { "SHERIFF", "SHERIFF2" };

        private Ped Paramedic1;
        private Ped Paramedic2;
        private Vehicle Ambulance;
        private readonly Model AmbulanceModel = new("AMBULANCE");
        private readonly Model AmbulancePedModel = new(0xB353629E);
        private readonly Model NotepadModel = new("prop_notepad_02");
        private Rage.Object Notepad;

        private readonly string[] tendToDeadIdles = { "idle_a", "idle_b", "idle_c" };
        private readonly string[] injuredBodyParts = { "Belly", "Right Arm", "Left Arm", "Left Thigh", "Right Thigh", "Left Calf", "Right Calf" };

        public override bool OnBeforeCalloutDisplayed()
        {
            FilePath = @"Plugins/LSPDFR/BarbarianCall/Locations/";
            PursuitCreated = false;
            CalloutRunning = false;
            DeclareVariable();
            
            Spawn = GenericUtils.SelectNearbySpawnpoint(DivisiXml.Deserialization.GetSpawnPointFromXml(Path.Combine(FilePath, "TrafficStop.xml")));
            Position = Spawn;
            SpawnHeading = Spawn;
            if (Position == Vector3.Zero || SpawnHeading == 0f)
            {
                Logger.Log("Officer Stabbed callout aborted");
                Logger.Log("No nearby location found");
                return false;
            }           
            ShowCalloutAreaBlipBeforeAccepting(Position, 40f);
            AddMinimumDistanceCheck(30f, Position);
            if (MyRandom.Next() % 10 == 0) susVehModel = Globals.MotorBikesToSelect.GetRandomElement(m => m.IsValid && m.NumberOfSeats == 2, true);
            else susVehModel = Globals.CarsToSelect.GetRandomElement(m => m.IsValid, true);
            susVehModel.LoadAndWait();
            CalloutPosition = Position;
            CalloutMessage = "Officer Stabbed";
            CalloutAdvisory = $"Suspect vehicle is {susVehModel.GetDisplayName()}";
            uint zoneHash = Rage.Native.NativeFunction.Natives.GET_HASH_OF_MAP_AREA_AT_COORDS<uint>(Position.X, Position.Y, Position.Z);
            if (UltimateBackupRunning)
            {
                Tuple<Vehicle, Ped> ubApi;
                int offRand = MyRandom.Next(0, 100);
                if (offRand <= 60) ubApi = API.UltimateBackupFunc.GetUnit(API.EUltimateBackupUnitType.LocalPatrol, Position);
                else if (offRand < 90) ubApi = API.UltimateBackupFunc.GetUnit(API.EUltimateBackupUnitType.StatePatrol, Position);
                else ubApi = API.UltimateBackupFunc.GetUnit(API.EUltimateBackupUnitType.LocalSwat, Position);
                offVeh = ubApi.Item1;
                officer = ubApi.Item2;
            }
            else
            {
                if (Game.GetHashKey("city") == zoneHash)
                {
                    string copCarModel = CityCarModels.GetRandomElement();
                    offVeh = new Vehicle(copCarModel, Position, SpawnHeading);
                }
                else
                {
                    string copCarModel = CountrysideCarModels.GetRandomElement();
                    offVeh = new Vehicle(copCarModel, Position, SpawnHeading);
                }
                officer = offVeh.CreateRandomDriver();
            }
            officer.MakeMissionPed();
            offVeh.MakePersistent();
            PlayScannerWithCallsign("WE_HAVE BAR_CRIME_STABBED IN_OR_ON_POSITION", Position);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            CalloutStates = ECalloutStates.EnRoute;
            officer.MakeMissionPed();
            officer.CanRagdoll = true;
            officer.Metadata.BAR_Entity = true;
            if (!officer.IsInVehicle(offVeh, false)) officer.WarpIntoVehicle(offVeh, -1);
            Functions.SetPedAsCop(officer);
            offVeh.MakePersistent();
            offVeh.RandomizeLicensePlate();
            offVeh.IsVisible = true;
            officer.IsVisible = true;
            Blip = new Blip(Position, 45f);
            Blip.Color = Color.Yellow;
            Blip.EnableRoute(Color.Yellow);
            Spawnpoint tempSpawn = SpawnManager.GetVehicleSpawnPoint(Position, 250f, 350f);
            if (tempSpawn != Spawnpoint.Zero) SuspectCar = new Vehicle(susVehModel, tempSpawn, tempSpawn);
            else SuspectCar = new Vehicle(susVehModel, World.GetNextPositionOnStreet(Position.Around(300f)));
            SuspectCar.SetRandomColor();
            SuspectCar.RandomizeLicensePlate();
            SuspectCar.Metadata.BAR_Entity = true;
            Suspect = SuspectCar.CreateRandomDriver();
            Suspect.MakeMissionPed();
            Suspect.Metadata.BAR_Entity = true;
            Suspect.RelationshipGroup = new RelationshipGroup("CRIMINAL");
            Suspect.SetPedAsWanted();
            Suspect.MaxHealth = 500;
            Suspect.Inventory.GiveNewWeapon(WeaponHash.Knife, -1, false);
            SuspectPersona = Functions.GetPersonaForPed(Suspect);
            if (MyRandom.Next(1, 10) > 7)
            {
                GameFiber.Sleep(1);
                passenger = new Ped(Position, SpawnHeading);
                passenger.WarpIntoVehicle(SuspectCar, 0);
                passenger.MakeMissionPed();
                passenger.Metadata.BAR_Entity = true;
                passenger.RelationshipGroup = Suspect.RelationshipGroup;
                passenger.MaxHealth = 2500;
                passenger.SetPedAsWanted();
                GameFiber.Wait(10);
                if (MyRandom.Next(8740) % 5 == 0)
                {
                    passengerWeapon = passenger.Inventory.GiveNewWeapon(0xDBBD7280, -1, false);
                }
            }
            susVehColor = SuspectCar.GetColor().PrimaryColorName;
            SuspectState = ESuspectStates.InAction;
            PassengerState = ESuspectStates.InAction;
            Functions.AddPedContraband(Suspect, ContrabandType.Weapon, "Knife");
            //Game.SetRelationshipBetweenRelationshipGroups("CRIMINAL", "COP", Relationship.Hate);
            //Game.SetRelationshipBetweenRelationshipGroups("COP", "CRIMINAL", Relationship.Hate);
            Game.SetRelationshipBetweenRelationshipGroups("CRIMINAL", "PLAYER", Relationship.Hate);
            if (Initialization.IsLSPDFRPluginRunning("BetterEMS"))
            {
                API.BetterEMSFunc.SetPedDeathDetails(officer, injuredBodyParts.GetRandomElement(), "Stabbed with a knife", Game.GameTime, 0.8f);
            }
            int scenario = MyRandom.Next(1, 4000);
            $"Callout Scenario: {scenario}".ToLog();
            CalloutRunning = true;
            if (scenario <= 1000) Proses();
            else if (scenario <= 2000) Proses2();
            else if (scenario <= 3000) Proses3();
            else if (scenario <= 4000) Proses4();
            else Proses();
            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            //if (Game.IsKeyDown(System.Windows.Forms.Keys.End))
            //{
               // GameFiber.StartNew(End);
            //}
            if (Suspect)
            {
                if (Functions.IsPedArrested(Suspect) && SuspectState != ESuspectStates.Arrested) { Game.DisplayNotification("Suspect Is ~g~Arrested"); SuspectState = ESuspectStates.Arrested; }
                else if (Suspect.IsDead && SuspectState != ESuspectStates.Dead) { Game.DisplayNotification("Suspect is ~r~Deceased"); SuspectState = ESuspectStates.Dead; }
                else if (PursuitCreated && !Suspect && SuspectState != ESuspectStates.Escaped) { Game.DisplayNotification("Suspect is ~o~escaped"); SuspectState = ESuspectStates.Escaped; }
            }         
            if (passenger)
            {
                if (Functions.IsPedArrested(passenger) && PassengerState != ESuspectStates.Arrested)
                {
                    Game.DisplayNotification("Passenger is ~g~arrested");
                    PassengerState = ESuspectStates.Arrested;
                }
                else if (passenger.IsDead && PassengerState != ESuspectStates.Dead)
                {
                    Game.DisplayNotification("Passenger is ~r~deceased");
                    PassengerState = ESuspectStates.Dead;
                }
                else if (PursuitCreated && !passenger && PassengerState != ESuspectStates.Escaped)
                {
                    Game.DisplayNotification("Passenger is ~o~escaped");
                    PassengerState = ESuspectStates.Escaped;
                }
            }
            if (passenger)
            {
                if (SuspectState != ESuspectStates.InAction && PassengerState != ESuspectStates.InAction)
                {
                    Game.DisplayNotification($"We Are ~g~Code 4~s~ suspect is ~o~{SuspectState}~s~ and passenger is ~o~{PassengerState}");
                    CalloutStates = ECalloutStates.Finish;
                    GameFiber.StartNew(DisplayCodeFourMessage);
                }
            }
            else
            {
                if (SuspectState != ESuspectStates.InAction)
                {
                    Game.DisplayNotification($"We Are ~g~Code 4~s~ suspect is {SuspectState}");
                    CalloutStates = ECalloutStates.Finish;
                    GameFiber.StartNew(DisplayCodeFourMessage);
                }
            }
            base.Process();
        }
        public override void End()
        {
            CalloutRunning = false;
            if (Suspect && Suspect.IsAlive && !Functions.IsPedArrested(Suspect)) Suspect.Dismiss();
            if (passenger && passenger.IsAlive && !Functions.IsPedArrested(passenger)) passenger.Dismiss();
            if (Blip) Blip.Delete();
            if (SuspectCar) SuspectCar.Dismiss();
            if (officer)
            {
                if (CalloutStates == ECalloutStates.Finish)
                {
                    if (officer.IsAlive)
                    {
                        GenericUtils.DisplayNotifWithLogo("~y~Stabbed Officer Status Update:~s~ ~n~~g~Rescued~s~ by paramedic", "Officer Stabbed");
                        officer.Dismiss();
                    }
                    else
                    {
                        GenericUtils.DisplayNotifWithLogo("~y~Stabbed Officer Status Update: ~r~Deceased~s~", "Officer Stabbed");
                        officer.Dismiss();
                    }
                }
                else
                {
                    if (officer.IsAlive) officer.Dismiss();
                    else officer.Delete();
                }
            }
            if (offVeh) offVeh.Dismiss();
            susVehModel.Dismiss();
            if (AmbulanceModel.IsLoaded) AmbulanceModel.Dismiss();
            if (AmbulancePedModel.IsLoaded) AmbulancePedModel.Dismiss();
            if (NotepadModel.IsLoaded) NotepadModel.Dismiss();
            if (Notepad) { Notepad.Detach(); GameFiber.Wait(75); Notepad.Delete(); }
            GameFiber.StartNew(() =>
            {
                if (Ambulance || Paramedic1 || Paramedic2)
                {
                    if (Paramedic1 && Ambulance) Paramedic1.Tasks.FollowNavigationMeshToPosition(Ambulance.Position + Ambulance.LeftPosition * 1.8f, Ambulance.Heading, 10f);
                    if (Paramedic2 && Ambulance) Paramedic2.Tasks.FollowNavigationMeshToPosition(Ambulance.Position + Ambulance.RightPosition * 1.8f, Ambulance.Heading, 10f).WaitForCompletion(7000);
                    if (Ambulance) Ambulance.LockStatus = VehicleLockStatus.Unlocked;
                    if (Paramedic1) Paramedic1.Tasks.EnterVehicle(Ambulance, -1);
                    if (Paramedic2) Paramedic2.Tasks.EnterVehicle(Ambulance, 0).WaitForCompletion(3500);
                    if (Paramedic1 && !Paramedic1.IsInVehicle(Ambulance, false)) Paramedic1.Tasks.EnterVehicle(Ambulance, -1, EnterVehicleFlags.WarpTo).WaitForCompletion(2000);
                    if (Paramedic2 && !Paramedic2.IsInVehicle(Ambulance, false)) Paramedic2.Tasks.EnterVehicle(Ambulance, 0, EnterVehicleFlags.WarpTo).WaitForCompletion(2000);
                    if (Paramedic1 && !Paramedic1.IsInVehicle(Ambulance, false)) Paramedic1.WarpIntoVehicle(Ambulance, -1);
                    if (Paramedic2 && !Paramedic2.IsInVehicle(Ambulance, false)) Paramedic2.WarpIntoVehicle(Ambulance, 0);
                    GameFiber.Sleep(2000);
                    if (Ambulance) Ambulance.Occupants.ToList().ForEach(p => { if (p) p.Dismiss(); });
                    if (Ambulance) Ambulance.Dismiss();
                    GameFiber.Sleep(2000);
                    if (Paramedic1) Paramedic1.Dismiss();
                    if (Paramedic2) Paramedic2.Dismiss();
                }
            });
            base.End();
        }
        protected override void CleanUp()
        {
            // TODO: implement CleanUp()
        }
        private void DisplayCodeFourMessage()
        {
            if (!CalloutRunning) return;
            PlayScannerWithCallsign("WE_ARE_CODE_4");
            End();
        }
        private void GetClose()
        {
            while (CalloutRunning)
            {
                GameFiber.Yield();
                if (officer && Game.LocalPlayer.Character.DistanceToSquared(officer) < 2025f)
                {
                    if (GrammarPoliceRunning) API.GrammarPoliceFunc.SetStatus(API.GrammarPoliceFunc.EGrammarPoliceStatusType.OnScene);
                    break;
                }
                else if (!officer)
                {
                    "Officer Stabbed callout crash or force ended too early".DisplayNotifWithLogo("~y~Officer Stabbed");
                    GameFiber.StartNew(End);
                }
            }
            if (Blip)
            {
                Blip.Delete();
                Blip = officer.AttachBlip();
                Blip.Color = Color.Green;
            }
        }
        private void Proses()
        {
            CalloutMainFiber = GameFiber.StartNew(delegate
            {
                try
                {
                    offVeh.Position = Position;
                    offVeh.Heading = SpawnHeading;
                    officer.Tasks.LeaveVehicle(LeaveVehicleFlags.None).WaitForCompletion(4000);
                    if (officer.IsInVehicle(offVeh, false)) officer.Position = offVeh.GetOffsetPosition(Vector3.RelativeLeft * 2f);
                    officer.Kill();
                    uint deathTime = Game.GameTime;
                    CalloutRunning = true;
                    GetClose();
                    if (!CalloutRunning) return;
                    officer.SetPositionZ(World.GetGroundZ(officer.Position, false, false) ?? officer.Position.Z);
                    "We have send an ambulance to your location".DisplayNotifWithLogo("Officer Stabbed");
                    SendCIMessage("Ambulance is en route to the scene");
                    if (Initialization.IsLSPDFRPluginRunning("BetterEMS"))
                    {
                        API.BetterEMSFunc.CallAmbulance(Position);
                    }
                    else Functions.RequestBackup(officer.Position, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.Ambulance);
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        if (Game.LocalPlayer.Character.Position.DistanceToSquared(Position) < 169f) break;
                    }
                    if (!CalloutRunning) return;
                    Functions.PlayPlayerRadioAction(Functions.GetPlayerRadioAction(), 5000);
                    Game.HideHelp();
                    Game.DisplayHelp("~o~Stand by for suspect details", true);
                    GameFiber.Sleep(MyRandom.Next(8000, 10000));
                    Game.HideHelp();
                    Game.DisplaySubtitle("~g~Dispatch~s~: the Ambulance will handle the officer you can go and chase the suspect");
                    Suspect.Tasks.CruiseWithVehicle(18f, VehicleDrivingFlags.AllowMedianCrossing | VehicleDrivingFlags.DriveAroundPeds |
                VehicleDrivingFlags.DriveAroundVehicles | VehicleDrivingFlags.DriveAroundVehicles);
                    PlayScannerWithCallsign($"BAR_TARGET_PLATE {GenericUtils.GetLicensePlateAudio(SuspectCar)} SUSPECT_LAST_SEEN IN_OR_ON_POSITION", Suspect.Position);
                    SendCIMessage($"Suspect is driving a {susVehColor} {SuspectCar.GetDisplayName()} with license plate {SuspectCar.LicensePlate}");
                    if (Blip) Blip.Delete();
                    Blip = new Blip(SuspectCar.Position.Around(20f), 80f);
                    Blip.Color = Color.Yellow;
                    Blip.EnableRoute(Color.Yellow);
                    Vector3 currSusPos = Suspect.Position;
                    Timer = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    Manusia = new Manusia(Suspect, SuspectPersona, SuspectCar);
                    Manusia.DisplayNotif();
                    SendCIMessage($"Suspect is spotted near {Suspect.GetZoneNameLSPDFR()}");
                    GameFiber.StartNew(() =>
                    {
                        GameFiber.Wait(5000);
                        if (CalloutRunning) DisplayGPNotif();
                        GameFiber.Wait(60000);
                        if (CalloutRunning) DisplayGPNotif();
                    }, "[BarbarianCall] Notification Fiber");
                    while (CalloutRunning)
                    {
                        Suspect.Health = Suspect.MaxHealth;
                        if (passenger) passenger.Health = passenger.MaxHealth;
                        if (Suspect.DistanceToSquared(currSusPos) > 6400f)
                        {
                            currSusPos = Suspect.Position;
                            if (Blip) Blip.Delete();
                            Blip = new Blip(currSusPos.Around2D(20f), 80f);
                            Blip.Color = Color.Yellow;
                            Blip.EnableRoute(Color.Yellow);
                            if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - Timer > 15000L)
                            {
                                Timer = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                                Functions.PlayScannerAudioUsingPosition($"SUSPECT_HEADING {Suspect.GetCardinalDirectionLowDetailedAudio()} IN_OR_ON_POSITION", currSusPos);
                                //Game.DisplayHelp($"Press {Peralatan.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.Y)} to see the suspect details");
                                //Suspect.DisplayNotificationsWithPedHeadshot("Suspect Details", $"~y~Name~s~: {DriverPersona.FullName}~n~Suspect is driving a ~y~{susVehColor} {SuspectCar.Model.Name.ToUpper()}~s~~n~" +
                                  //  $"Suspect Location: ~g~{Suspect.Position.GetZoneName()}~s~ Near ~g~{World.GetStreetName(Suspect.Position)}~n~~y~License Plate~s~: {SuspectCar.LicensePlate}");
                            }                           
                        }
                        if (Game.LocalPlayer.Character.DistanceToSquared(Suspect) < 225f)
                        {
                            int chaseScenario = MyRandom.Next(7250);
                            if (PlayerPed.IsInAnyVehicle(false) && PlayerPed.CurrentVehicle.IsSirenOn)
                            {
                                chaseScenario = 2;
                            }
                            if (chaseScenario % 2 == 0)
                            {
                                SendCIMessage("Suspect is fleeing");
                                Pursuit = Functions.CreatePursuit();
                                Functions.AddPedToPursuit(Pursuit, Suspect);
                                if (passenger && passenger.IsInVehicle(SuspectCar, false))
                                {
                                    if (MyRandom.Next(1500) % 2 == 0) passenger.Tasks.LeaveVehicle(SuspectCar, LeaveVehicleFlags.BailOut);
                                    Functions.AddPedToPursuit(Pursuit, passenger);
                                }
                                Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                                SendCIMessage("Warning nearby units to help");
                                Functions.RequestBackup(Suspect.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
                                SendCIMessage($"Unit {GenericUtils.GetRandomUnitNumber()} is responding");
                                PursuitCreated = true;
                            }
                            else
                            {
                                Game.DisplayHelp("Suspect Found, ~y~Perform pullover on a ~r~suspect", 8500);
                            }
                            break;
                        }
                        GameFiber.Yield();
                    }
                    if (!CalloutRunning) return;
                    if (Blip) Blip.Delete();
                    Blip = Suspect.AttachBlip();
                    Blip.Color = Color.Red;
                    Blip.IsFriendly = false;
                    if (PursuitCreated)
                    {
                        if (GrammarPoliceRunning) API.GrammarPoliceFunc.SetStatus(API.GrammarPoliceFunc.EGrammarPoliceStatusType.InPursuit);
                        if (Blip) Blip.Delete();
                        Blip = Suspect.AttachBlip();
                        Blip.Color = Color.Firebrick;
                        Blip.IsFriendly = false;
                        Blip.EnableRoute(Blip.Color);
                        GameFiber.Wait(7000);
                        if (Functions.IsPursuitStillRunning(Pursuit)) Functions.RequestBackup(Suspect.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.NooseAirUnit);
                    }
                    else
                    {
                        Timer = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                        while (CalloutRunning)
                        {
                            if (Functions.IsPlayerPerformingPullover())
                            {
                                PullOver = Functions.GetCurrentPullover();
                                if (Functions.GetPulloverSuspect(PullOver) == Suspect)
                                {
                                    break;
                                }
                            }
                            if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - Timer > 12000L)
                            {
                                Timer = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                                Game.DisplayHelp("Suspect found. ~y~Perform pullover on a ~r~suspect ~s~and then~r~bring the suspect to jail~s~, ~g~please be careful~s~, suspect may armed with ~o~weapon", 8500);
                            }
                            GameFiber.Yield();
                        }
                        if (!CalloutRunning) return;
                    }
                } catch (Exception e)
                {
                    "Officer Stabbed callout crashes".ToLog();
                    e.ToString().ToLog();
                    NetExtension.SendError(e);
                    "Officer Stabbed callout crashed, please send your log".DisplayNotifWithLogo("Officer Stabbed");
                    End();
                }
            });
        }
        private void Proses2()
        {
            CalloutMainFiber = GameFiber.StartNew(() =>
            {
                try
                {
                    CalloutRunning = true;
                    offVeh.Position = Position;
                    offVeh.Heading = SpawnHeading;
                    SuspectCar.Position = offVeh.Position + offVeh.ForwardVector * 9f;
                    SuspectCar.Heading = SpawnHeading;
                    Manusia = new Manusia(Suspect, SuspectPersona, SuspectCar);
                    Manusia.DisplayNotif();
                    officer.Tasks.LeaveVehicle(LeaveVehicleFlags.None).WaitForCompletion(5000);
                    if (officer.IsInVehicle(offVeh, false)) officer.Position = offVeh.GetOffsetPosition(Vector3.RelativeLeft * 2f);
                    officer.Kill();
                    uint deathTime = Game.GameTime;
                    DisplayGPNotif();
                    GetClose();
                    officer.SetPositionZ(World.GetGroundZ(officer.Position, false, false) ?? officer.Position.Z);
                    if (!CalloutRunning) return;
                    "We have send an ambulance to your location".DisplayNotifWithLogo("Officer Stabbed");
                    SendCIMessage("Ambulance is en route to the scene");
                    if (Initialization.IsLSPDFRPluginRunning("BetterEMS"))
                    {
                        API.BetterEMSFunc.SetPedDeathDetails(officer, injuredBodyParts.GetRandomElement(), "Stabbed with a knife", deathTime, 0.8f);
                        API.BetterEMSFunc.CallAmbulance(officer.Position);
                    }
                    else Functions.RequestBackup(officer.Position, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.Ambulance);
                    if (passenger && passenger.IsInAnyVehicle(false)) passenger.Tasks.LeaveVehicle(LeaveVehicleFlags.None);
                    if (Suspect.IsInAnyVehicle(false)) Suspect.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen).WaitForCompletion(5000);
                    if (Suspect.IsInAnyVehicle(false)) Suspect.Tasks.LeaveVehicle(LeaveVehicleFlags.WarpOut).WaitForCompletion();
                    if (passenger) passenger.Tasks.Cower(-1);
                    if (Blip) Blip.Delete();
                    Blip = Suspect.AttachBlip();
                    Blip.Color = Color.Red;
                    Blip.Scale = 0.80f;
                    Suspect.Tasks.FollowNavigationMeshToPosition(officer.Position + Vector3.RelativeLeft, officer.Heading - 180f, 5f).WaitForCompletion(6000);
                    Suspect.Tasks.AchieveHeading(Suspect.GetHeadingTowards(officer)).WaitForCompletion(800);
                    Suspect.Tasks.PlayAnimation("amb@medic@standing@tendtodead@idle_a", tendToDeadIdles.GetRandomElement(), 2.0f, AnimationFlags.Loop);
                    Suspect.PlayAmbientSpeech(null, Speech.GENERIC_SHOCKED_HIGH, 0, SpeechModifier.ForceShouted);
                    Game.DisplayHelp($"~y~Approach the suspect and press {GenericUtils.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.Y)} to talk");
                    Timer = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    List<string> conv = new()
                    {
                        "~r~Suspect~s~: " + proses2Sorry.GetRandomElement(),
                        "~b~Officer~s~: there is must be a reason this officer doing that",
                        "~r~Suspect~s~: You are all the same, a police officer should be protect the society, instead of being oppressive",
                        "~b~Officer~s~: Come on give me your hand, let's continue the conversation at the police station",
                    };
                    while (CalloutRunning)
                    {
                        if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - Timer > 10000L)
                        {
                            Timer = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                            Game.DisplayHelp($"~y~Approach the suspect and press {GenericUtils.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.Y)} to talk");
                        }
                        if (Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                        {
                            if (PlayerPed.DistanceToSquared(Suspect) < 225f) break;
                            else Game.DisplayHelp("~y~Please get ~g~closer~y~ to the ~r~suspect~y~ first");
                        }
                        GameFiber.Yield();
                    }
                    if (!CalloutRunning) return;
                    Suspect.Tasks.ClearImmediately();
                    Suspect.Tasks.AchieveHeading(Suspect.GetHeadingTowards(PlayerPed)).WaitForCompletion(800);
                    GenericUtils.HandleSpeech(conv, Suspect);
                    if (passenger)
                    {
                        GameFiber.StartNew(() =>
                        {
                            if (PlayerPed.LastVehicle.Exists() && PlayerPed.LastVehicle.DistanceTo(Position) < 85f)
                            {
                                passenger.Tasks.FollowNavigationMeshToPosition(PlayerPed.LastVehicle.GetOffsetPosition(Vector3.RelativeBack * 2f), PlayerPed.LastVehicle.Heading - 180f, 10f).WaitForCompletion(-1);
                                passenger.Tasks.Cower(-1);
                            }
                            else
                            {
                                passenger.Tasks.FollowNavigationMeshToPosition(offVeh.Position + offVeh.RearPosition * 2f, offVeh.Heading - 180f, 10f).WaitForCompletion(-1);
                                passenger.Tasks.Cower(-1);
                            }
                            Game.DisplayNotification("~y~Passenger is ~g~giving up~y~, handle the passenger as you see fit");
                            PassengerState = ESuspectStates.Arrested;
                            GameFiber.SleepUntil(() => passenger.Tasks.CurrentTaskStatus == TaskStatus.None || !CalloutRunning, -1);
                        });
                    }
                    int rands = MyRandom.Next(1, 100);
                    $"Officer Stabbed scenario {rands}".ToLog();
                    if (rands < 50)
                    {
                        Suspect.Tasks.PutHandsUp(-1, PlayerPed);
                        Game.DisplayHelp("~y~Suspect has ~g~surrendered~s~, now ~o~handcuff~y~ the ~r~suspect");
                        GameFiber.SleepUntil(() => Functions.IsPedBeingCuffed(Suspect) || Functions.IsPedArrested(Suspect), -1);
                    }
                    else if (rands < 75)
                    {
                        Suspect.MaxHealth = MyRandom.Next(1250, 2850);
                        Suspect.Health = Suspect.MaxHealth;
                        $"Health scenario {Suspect.MaxHealth}".ToLog();
                        Suspect.Armor = 500;
                        Suspect.Inventory.GiveNewWeapon(new WeaponAsset(WeaponHashes.GetRandomElement()), -1, true);
                        Suspect.Tasks.FightAgainst(PlayerPed);
                    } else
                    {
                        Suspect.Tasks.FollowNavigationMeshToPosition(SuspectCar.Position + SuspectCar.RightPosition * -1.8f, SuspectCar.Heading, 10f).WaitForCompletion(-1);
                        Suspect.Tasks.EnterVehicle(SuspectCar, -1, EnterVehicleFlags.AllowJacking);
                        Pursuit = Functions.CreatePursuit();
                        Functions.AddPedToPursuit(Pursuit, Suspect);
                        Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                        if (Blip && Blip.Entity == Suspect) Blip.EnableRoute(Blip.Color);
                        Functions.RequestBackup(Suspect.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
                        if (GrammarPoliceRunning) API.GrammarPoliceFunc.SetStatus(API.GrammarPoliceFunc.EGrammarPoliceStatusType.InPursuit);
                        PursuitCreated = true;
                    }
                    if (PursuitCreated)
                    {
                        GameFiber.Wait(7500); if (Functions.IsPursuitStillRunning(Pursuit)) Functions.RequestBackup(Suspect.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.NooseAirUnit);
                    }
                }
                catch (Exception e)
                {
                    "Officer stabbed callout crashed".ToLog();
                    e.ToString().ToLog();
                    NetExtension.SendError(e);
                    "Officer Stabbed callout crashed, please send your log".DisplayNotifWithLogo("Officer Stabbed");
                    End();
                }
            });
        }
        private List<string> proses2Sorry = new()
        {
            "I'm sorry, I had to do this, this officer was pointed his gun at me",
            "I'm sorry, I was blackmailed by the officer, I had to stab him",
            "I'm really sorry officer, this person right here was about to shot me, so i decided to stab him with my knife"
        };
        private void Proses3()
        {
            CalloutMainFiber = GameFiber.StartNew(() =>
            {
                try
                {
                    AmbulanceModel.LoadAndWait();
                    NotepadModel.LoadAndWait();
                    CalloutRunning = true;
                    offVeh.Position = Position;
                    offVeh.Heading = SpawnHeading;
                    if (MyRandom.Next(100) % 2 == 0) 
                    { 
                        if (offVeh.Doors[0].IsValid()) 
                            offVeh.Doors[0].Open(true); 
                    }
                    GameFiber.Wait(75);
                    if (MyRandom.Next(250) % 2 == 0) 
                    { 
                        if (offVeh.HasBone("window_lf")) 
                            offVeh.Windows[0].Smash(); 
                    }
                    GameFiber.Wait(75);
                    officer.Tasks.LeaveVehicle(LeaveVehicleFlags.WarpOut).WaitForCompletion(200);
                    $"Officer is in vehicle: {officer.IsInAnyVehicle(false)}".ToLog();
                    officer.Position = offVeh.GetOffsetPosition(Vector3.RelativeLeft * 2f);
                    Vector3 medPos1 = officer.Position + Vector3.RelativeRight;
                    Ambulance = new Vehicle(AmbulanceModel, offVeh.GetOffsetPositionFront(9f), SpawnHeading)
                    {
                        IsPersistent = true,
                        LockStatus = VehicleLockStatus.Locked,
                    };
                    Ambulance.RandomizeLicensePlate();
                    if (UltimateBackupRunning)
                    {
                        Paramedic1 = API.UltimateBackupFunc.GetPed(API.EUltimateBackupUnitType.Ambulance, Position, SpawnHeading);
                        Paramedic2 = API.UltimateBackupFunc.GetPed(API.EUltimateBackupUnitType.Ambulance, Position, SpawnHeading);
                    }
                    else
                    {
                        AmbulancePedModel.LoadAndWait();
                        Paramedic1 = new Ped(AmbulancePedModel, Position, SpawnHeading);
                        Paramedic2 = new Ped(AmbulancePedModel, Position, SpawnHeading);
                    }
                    Paramedic1.RelationshipGroup = RelationshipGroup.Medic;
                    Paramedic2.RelationshipGroup = RelationshipGroup.Medic;
                    Paramedic1.MakeMissionPed(true);
                    Paramedic2.MakeMissionPed(true);
                    Notepad = new Rage.Object(NotepadModel, Vector3.Zero);
                    Notepad.AttachTo(Paramedic2, Paramedic2.GetBoneIndex(PedBoneId.LeftPhHand), Vector3.Zero, Rotator.Zero);
                    Paramedic1.Position = medPos1; Paramedic1.Heading = Paramedic1.GetHeadingTowards(officer);
                    GameFiber.Wait(75);
                    Paramedic2.Position = Paramedic1.Position + Paramedic2.ForwardVector * 2.225f; Paramedic2.Heading = Paramedic2.GetHeadingTowards(officer);
                    Paramedic1.Tasks.PlayAnimation("amb@medic@standing@tendtodead@idle_a", tendToDeadIdles.GetRandomElement(), 2.0f, AnimationFlags.Loop);
                    GameFiber.Wait(75);
                    Paramedic2.Tasks.PlayAnimation("amb@medic@standing@timeofdeath@base", "base", 2.0f, AnimationFlags.Loop);
                    officer.Kill();
                    CalloutEntities.Add(Notepad);
                    CalloutEntities.Add(Paramedic1);
                    CalloutEntities.Add(Paramedic2);
                    CalloutEntities.Add(Ambulance);
                    GetClose();
                    if (!CalloutRunning) return;
                    officer.SetPositionZ(World.GetGroundZ(officer.Position, false, false) ?? officer.Position.Z);
                    Suspect.Tasks.CruiseWithVehicle(20f, VehicleDrivingFlags.Emergency);
                    
                    Game.DisplayHelp($"~y~Get ~o~closer~y~ to the ~g~paramedic~y~ and press {GenericUtils.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.Y)}~s~ ~y~to talk");
                    Timer = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    while (CalloutRunning)
                    {
                        if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - Timer > 10000L)
                        {
                            Timer = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                            Game.DisplayHelp($"~y~Get ~o~closer~y~ to the ~g~paramedic~y~ and press {GenericUtils.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.Y)}~s~ ~y~to talk", false);
                        }
                        if (Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                        {
                            if (PlayerPed.DistanceToSquared(Paramedic1) < 18f)
                            {
                                Game.DisplaySubtitle($"~b~{Globals.PlayerPedName}~s~: Is he alright? Where is the suspect?");
                                break;
                            }
                            else Game.DisplayHelp("Please get ~o~closer with ~g~paramedic");
                        }
                        GameFiber.Yield();
                    }
                    if (!CalloutRunning) return;
                    List<string> ambulanceConversation = new()
                     {           
                        "~g~Paramedic~s~: Calm down officer, We can handle this, now you can search for the suspect",           
                        $"~g~Paramedic~s~: All i know he is driving a ~y~{susVehColor}-colored {SuspectCar.GetDisplayName()}~s~ with license plate ~g~{SuspectCar.LicensePlate}~s~",
                        $"~b~{Globals.PlayerPedName}~s~: Thank you for the information i will search the suspect now",
                        "~g~Paramedic~s~: Please be careful, suspect may bring some weapon",
                        $"~b~{Globals.PlayerPedName}~s~: Sure, thank you"
                    };
                    float amb1Heading = Paramedic1.Heading;
                    GenericUtils.HandleSpeech(ambulanceConversation, Paramedic1);
                    SendCIMessage($"Suspect is driving a {susVehColor} {SuspectCar.GetDisplayName()} with license plate {SuspectCar.LicensePlate}");
                    PlayScannerWithCallsign($"BAR_TARGET_PLATE {GenericUtils.GetLicensePlateAudio(SuspectCar)}");
                    GameFiber.StartNew(() =>
                    {
                        Paramedic1.Tasks.FollowNavigationMeshToPosition(medPos1, amb1Heading, 1f).WaitForCompletion(-1);
                        Paramedic1.Tasks.PlayAnimation("amb@medic@standing@tendtodead@idle_a", tendToDeadIdles.GetRandomElement(), 2.0f, AnimationFlags.Loop);
                    });
                    Game.DisplayHelp("~y~Standby for suspect details");
                    GameFiber.Wait(MyRandom.Next(7500, 10000));
                    GameFiber.WaitUntil(() => !Functions.GetIsAudioEngineBusy(), 5000);
                    if (Suspect.DistanceTo(PlayerPed) > 850f || Suspect.TravelDistanceTo(PlayerPed) > 1250f)
                    {
                        Spawnpoint closer = SpawnManager.GetVehicleSpawnPoint(Position, 350, 650);
                        if (closer == Spawnpoint.Zero) closer = new Spawnpoint(World.GetNextPositionOnStreet(Position.Around(350, 650)), 0f);
                        SuspectCar.Position = closer;
                        SuspectCar.Heading = closer;
                    }
                    PlayScannerWithCallsign("SUSPECT_LAST_SEEN IN_OR_ON_POSITION", Suspect.Position);
                    Vector3 currSusPos = Suspect.Position;
                    Timer = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    Manusia = new Manusia(Suspect, SuspectPersona, SuspectCar);
                    Manusia.DisplayNotif();
                    while (CalloutRunning)
                    {
                        Suspect.Health = Suspect.MaxHealth;
                        if (passenger) passenger.Health = passenger.MaxHealth;
                        if (Suspect.DistanceToSquared(currSusPos) > 6400f)
                        {
                            currSusPos = Suspect.Position;
                            if (Blip) Blip.Delete();
                            Blip = new Blip(currSusPos.Around2D(10f), 80f);
                            Blip.Color = Color.Yellow;
                            Blip.EnableRoute(Color.Yellow);
                            if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - Timer > 15000L)
                            {
                                Timer = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                                Functions.PlayScannerAudioUsingPosition($"SUSPECT_HEADING {Suspect.GetCardinalDirectionLowDetailedAudio()} IN_OR_ON_POSITION", currSusPos);                               
                            }                         
                        }
                        if (Game.LocalPlayer.Character.DistanceToSquared(Suspect) < 225f)
                        {
                            int chaseScenario = MyRandom.Next(1, 1000);
                            $"Suspect chase scenario : {chaseScenario}".ToLog();
                            if (chaseScenario > 250)
                            {
                                Pursuit = Functions.CreatePursuit();
                                Functions.AddPedToPursuit(Pursuit, Suspect);
                                if (passenger && passenger.IsInVehicle(SuspectCar, false))
                                {
                                    if (MyRandom.Next(1500) % 2 == 0) passenger.Tasks.LeaveVehicle(SuspectCar, LeaveVehicleFlags.BailOut);
                                    Functions.AddPedToPursuit(Pursuit, passenger);
                                }
                                Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                                Functions.RequestBackup(Suspect.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
                                PursuitCreated = true;
                            }
                            else
                            {
                                Game.DisplayHelp("Suspect found. ~y~Perform a pullover on a ~r~suspect ~y~ and arrest the suspect", 8500);
                            }
                            break;
                        }
                        GameFiber.Yield();
                    }
                    if (!CalloutRunning) return;
                    if (MyRandom.Next(1, 100) > 30) { if (officer && officer.IsDead) officer.Resurrect(); }
                    if (Blip) Blip.Delete();
                    Blip = Suspect.AttachBlip();
                    Blip.Color = Color.Firebrick;
                    Blip.IsFriendly = false;
                    if (PursuitCreated)
                    {
                        if (GrammarPoliceRunning) API.GrammarPoliceFunc.SetStatus(API.GrammarPoliceFunc.EGrammarPoliceStatusType.InPursuit);                 
                        Blip.EnableRoute(Blip.Color);
                        GameFiber.Wait(MyRandom.Next(4500, 7850));
                        if (Functions.IsPursuitStillRunning(Pursuit)) Functions.RequestBackup(Suspect.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.NooseAirUnit);
                    }
                    else
                    {
                        Timer = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                        while (CalloutRunning)
                        {
                            if (Functions.IsPlayerPerformingPullover())
                            {
                                PullOver = Functions.GetCurrentPullover();
                                if (Functions.GetPulloverSuspect(PullOver) == Suspect)
                                break;
                            }
                            if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - Timer > 12000L)
                            {
                                Timer = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                                Game.DisplayHelp("Suspect found. ~y~Perform a pullover on a ~r~suspect ~s~and then handcuff the suspect, ~g~please be careful~s~, suspect may armed with ~o~weapon", 8500);
                            }
                            GameFiber.Yield();
                        }
                        if (!CalloutRunning) return;
                        "Pullover detected, waiting to for player to arrest the suspect".ToLog();
                    }
                }
                catch (Exception e)
                {
                    "Officer stabbed callout crashed".ToLog();
                    e.ToString().ToLog();
                    NetExtension.SendError(e);
                    "Officer Stabbed callout crashed, please send your log".DisplayNotifWithLogo("Officer Stabbed");
                    End();
                }
            });
        }
        private void Proses4()
        {
            CalloutMainFiber = GameFiber.StartNew(() =>
            {
                try
                {                   
                    offVeh.Position = Position;
                    offVeh.Heading = SpawnHeading;
                    if (!passenger)
                    {
                        passenger = new Ped(Position);
                        passenger.MakeMissionPed();
                        passenger.SetPedAsWanted();
                        passenger.RelationshipGroup = Suspect.RelationshipGroup;
                        passenger.WarpIntoVehicle(SuspectCar, 0);
                        passenger.MaxHealth = 2500;
                        passenger.Armor = 500;
                    }
                    SuspectCar.Position = offVeh.Position + offVeh.ForwardVector * 9f; SuspectCar.Heading = SpawnHeading;
                    Suspect.MaxHealth = 2500;
                    Suspect.Armor = 500;
                    officer.Tasks.LeaveVehicle(LeaveVehicleFlags.WarpOut).WaitForCompletion(500);
                    officer.Position = offVeh.GetOffsetPosition(Vector3.RelativeLeft * 2f);
                    officer.Kill();
                    Manusia = new Manusia(Suspect, SuspectPersona, SuspectCar);
                    Manusia.DisplayNotif();
                    GameFiber.Wait(2000);
                    DisplayGPNotif();
                    CalloutRunning = true;
                    GetClose();
                    if (!CalloutRunning) return;
                    officer.SetPositionZ(World.GetGroundZ(officer.Position, false, false) ?? officer.Position.Z);
                    if (Suspect.IsInVehicle(SuspectCar, false)) Suspect.Tasks.LeaveVehicle(LeaveVehicleFlags.None);
                    if (passenger.IsInVehicle(SuspectCar, false)) passenger.Tasks.LeaveVehicle(LeaveVehicleFlags.None).WaitForCompletion();
                    Suspect.Inventory.GiveNewWeapon(new WeaponAsset(WeaponHashes.GetRandomElement()), -1, true);
                    GameFiber.Wait(75);
                    if (passengerWeapon != null && passenger.Inventory.Weapons.Contains(passengerWeapon)) passenger.Inventory.EquippedWeapon = passengerWeapon;
                    else passenger.Inventory.GiveNewWeapon(new WeaponAsset(WeaponHashes.GetRandomElement()), -1, true);
                    Suspect.Tasks.FightAgainst(PlayerPed);
                    passenger.Tasks.FightAgainst(PlayerPed);
                    SendCIMessage("Shots Fired!!");
                    GameFiber.Wait(2000);
                    Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS BAR_CODE_99 IN_OR_ON_POSITION", PlayerPed.Position);
                }
                catch (Exception e)
                {
                    e.Message.ToLog();
                    "Officer stabbed callout crashed".ToLog();
                    e.ToString().ToLog();
                    NetExtension.SendError(e);
                    "Officer Stabbed callout crashed, please send your log".DisplayNotifWithLogo("Officer Stabbed");
                }
            });
        }
        private void SituationDashCam()
        {
            CalloutMainFiber = GameFiber.StartNew(() =>
            {
                try
                {
                    offVeh.Position = Position;
                    offVeh.Heading = SpawnHeading;
                    if (!passenger)
                    {
                        passenger = new Ped(Position);
                        passenger.MakeMissionPed();
                        passenger.SetPedAsWanted();
                        passenger.RelationshipGroup = Suspect.RelationshipGroup;
                        passenger.WarpIntoVehicle(SuspectCar, 0);
                        passenger.MaxHealth = 2500;
                        passenger.Armor = 500;
                    }            
                }
                catch (Exception e)
                {
                    e.Message.ToLog();
                    "Officer stabbed callout crashed".ToLog();
                    e.ToString().ToLog();
                    NetExtension.SendError(e);
                    "Officer Stabbed callout crashed, please send your log".DisplayNotifWithLogo("Officer Stabbed");
                }
            });
        }
    }
}
