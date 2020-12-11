using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Engine.Scripting.Entities;
using System.Drawing;
using BarbarianCall.Types;

namespace BarbarianCall.Callouts
{
    [CalloutInfo("Officer Stabbed", CalloutProbability.Medium)]
    public class OfficerStabbed : CalloutBase
    {
        private Ped officer;
        private Ped passenger;
        private Vehicle offVeh;
        private Model susVehModel;
        private Persona DriverPersona;
        private Persona PassengerPersona;
        private string susVehColor;
        private ESuspectStates SuspectState;
        private ESuspectStates PassengerState;

        private readonly string[] CityCarModels = new string[] { "POLICE", "POLICE2", "POLICE3", "POLICE4" };
        private readonly string[] CountrysideCarModels = new string[] { "SHERIFF", "SHERIFF2" };
        private readonly string path = @"Plugins/LSPDFR/BarbarianCall/OfficerStabbed/";

        private Ped AmbulancePed1;
        private Ped AmbulancePed2;
        private Vehicle Ambulance;
        private readonly Model AmbulanceModel = new Model("AMBULANCE");
        private readonly Model AmbulancePedModel = new Model(0xB353629E);
        private readonly Model NotepadModel = new Model("prop_notepad_02");
        private Rage.Object Notepad;

        private static readonly string[] tendToDeadIdles = { "idle_a", "idle_b", "idle_c" };
        private static readonly string[] injuredBodyParts = { "Belly", "Right Arm", "Left Arm", PedBoneId.LeftThigh.ToString(), PedBoneId.RightThigh.ToString(), "Left Calf", "Right Calf" };
        private static readonly uint[] weapons = { 0x1B06D571, 0xBFE256D4, 0x5EF9FEC4, 0x22D8FE39, 0x99AEEB3B, 0x2B5EF5EC, 0x78A97CD0, 0x1D073A89, 0x555AF99A };

        public override bool OnBeforeCalloutDisplayed()
        {
            PursuitCreated = false;
            CalloutRunning = false;
            
            DivisiXml.Deserialization.GetDataFromXml(path + "Locations.xml", out List<Vector3> locationToSelect, out List<float> headingToSelect);
            Peralatan.SelectNearbyLocationsWithHeading(locationToSelect, headingToSelect, out SpawnPoint, out SpawnHeading);
            if (SpawnPoint == Vector3.Zero || SpawnHeading == 0f)
            {
                Peralatan.ToLog("Officer Stabbed callout aborted");
                Peralatan.ToLog("No nearby location found");
                return false;
            }           
            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 20f);
            AddMinimumDistanceCheck(30f, SpawnPoint);
            susVehModel = CommonVariables.CarsToSelect.GetRandomModel();
            susVehModel.LoadAndWait();
            CalloutPosition = SpawnPoint;
            CalloutMessage = "Officer Stabbed";
            CalloutAdvisory = $"Suspect vehicle is {susVehModel.Name}";
            uint zoneHash = Rage.Native.NativeFunction.CallByHash<uint>(0x7ee64d51e8498728, SpawnPoint.X, SpawnPoint.Y, SpawnPoint.Z);
            if (UltimateBackupRunning)
            {
                Tuple<Vehicle, List<Ped>> ubApi;
                var offRand = Peralatan.Random.Next(0, 100);
                if (offRand <= 60) ubApi = API.UBFunc.GetUnit(API.UBFunc.EUltimateBackupUnitType.LocalPatrol, SpawnPoint, 1);
                else if (offRand > 60 && offRand <= 90) ubApi = API.UBFunc.GetUnit(API.UBFunc.EUltimateBackupUnitType.StatePatrol, SpawnPoint, 1);
                else ubApi = API.UBFunc.GetUnit(API.UBFunc.EUltimateBackupUnitType.LocalSwat, SpawnPoint, 1);
                offVeh = ubApi.Item1;
                officer = ubApi.Item2[0];
            }
            else
            {
                if (Game.GetHashKey("city") == zoneHash)
                {
                    var copCarModel = CityCarModels[MathHelper.GetRandomInteger(0, CityCarModels.Length - 1)];
                    offVeh = new Vehicle(copCarModel, SpawnPoint, SpawnHeading);
                }
                else
                {
                    var copCarModel = CountrysideCarModels[MathHelper.GetRandomInteger(0, CountrysideCarModels.Length - 1)];
                    offVeh = new Vehicle(copCarModel, SpawnPoint, SpawnHeading);
                }
                officer = offVeh.CreateRandomDriver();
            }
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS BAR_CODE_99 BAR_CRIME_STABBED IN_OR_ON_POSITION", SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            CalloutStates = ECalloutStates.EnRoute;
            officer.MakeMissionPed();
            officer.CanRagdoll = true;
            if (!officer.IsInVehicle(offVeh, false)) officer.WarpIntoVehicle(offVeh, -1);
            offVeh.MakePersistent();
            offVeh.RandomiseLicencePlate();
            offVeh.IsVisible = true;
            officer.IsVisible = true;
            Blip = new Blip(SpawnPoint, 45f);
            Blip.Color = Color.Yellow;
            Blip.EnableRoute(Color.Yellow);
            bool found = false;
            for (int i = 0; i < 50; i++)
            {
                var around = SpawnPoint.Around(300f);
                if (around.GetRoadSidePointWithHeading(out Vector3 tmpS, out float tmpH))
                {
                    SuspectCar = new Vehicle(susVehModel, tmpS, tmpH);
                    found = true;
                    break;
                }
            }
            if (!found) SuspectCar = new Vehicle(susVehModel, World.GetNextPositionOnStreet(SpawnPoint.Around(300f)));
            SuspectCar.PrimaryColor = CommonVariables.CommonUnderstandableColor.GetRandomColor(out susVehColor);
            SuspectCar.RandomiseLicencePlate();
            Suspect = SuspectCar.CreateRandomDriver();
            Suspect.MakeMissionPed();
            Suspect.RelationshipGroup = new RelationshipGroup("CRIMINAL");
            Suspect.SetPedAsWanted();
            Suspect.MaxHealth = 500;
            DriverPersona = Functions.GetPersonaForPed(Suspect);
            if (MathHelper.GetRandomInteger(1, 10) > 7)
            {
                GameFiber.Sleep(1);
                passenger = new Ped(SpawnPoint, SpawnHeading);
                passenger.WarpIntoVehicle(SuspectCar, 0);
                passenger.MakeMissionPed();
                passenger.RelationshipGroup = Suspect.RelationshipGroup;
                passenger.MaxHealth = 2500;
                passenger.SetPedAsWanted();
                PassengerPersona = Functions.GetPersonaForPed(passenger);
            }
            Functions.AddPedContraband(Suspect, ContrabandType.Weapon, "Knife");
            Game.SetRelationshipBetweenRelationshipGroups("CRIMINAL", "COP", Relationship.Hate);
            Game.SetRelationshipBetweenRelationshipGroups("COP", "CRIMINAL", Relationship.Hate);
            Game.SetRelationshipBetweenRelationshipGroups("CRIMINAL", "PLAYER", Relationship.Hate);
            Proses3();
            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            if (Game.IsKeyDown(System.Windows.Forms.Keys.End))
            {
                Game.DisplayNotification("Callout force ended");
                GameFiber.StartNew(End);
            }
            if (Functions.IsPedArrested(Suspect)) { Game.DisplayNotification("Suspect Is ~g~Arrested"); SuspectState = ESuspectStates.Arrested; }
            else if (Suspect.IsDead) { Game.DisplayNotification("Suspect is ~r~Deceased"); SuspectState = ESuspectStates.Dead; }
            else if (PursuitCreated && !Suspect.Exists()) { Game.DisplayNotification("Suspect is ~o~escaped"); SuspectState = ESuspectStates.Escaped; }
            if (passenger.Exists())
            {
                if (Functions.IsPedArrested(Suspect))
                {
                    Game.DisplayNotification("Passenger is ~g~arrested");
                    PassengerState = ESuspectStates.Arrested;
                }
                else if (passenger.IsDead)
                {
                    Game.DisplayNotification("Passenger is ~r~deceased");
                    PassengerState = ESuspectStates.Dead;
                }
                else if (PursuitCreated && !passenger.Exists())
                {
                    Game.DisplayNotification("Passenger is ~o~escaped");
                    PassengerState = ESuspectStates.Escaped;
                }
            }
            if (passenger.Exists())
            {
                if ((SuspectState == ESuspectStates.Arrested || SuspectState == ESuspectStates.Dead || SuspectState == ESuspectStates.Escaped) && 
                    (PassengerState == ESuspectStates.Escaped || PassengerState == ESuspectStates.Dead || PassengerState == ESuspectStates.Arrested))
                {
                    Game.DisplayNotification($"We Are ~g~Code 4~s~ suspect is {SuspectState} and passenger is {PassengerState}");
                    CalloutStates = ECalloutStates.Finish;
                    GameFiber.StartNew(DisplayCodeFourMessage);
                }
            }
            else
            {
                if (SuspectState == ESuspectStates.Arrested || SuspectState == ESuspectStates.Dead || SuspectState == ESuspectStates.Escaped)
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
            if (Suspect.Exists() && Suspect.IsAlive && !Functions.IsPedArrested(Suspect)) Suspect.Dismiss();
            if (passenger.Exists() && passenger.IsAlive && !Functions.IsPedArrested(passenger)) passenger.Dismiss();
            if (Blip.Exists()) Blip.Delete();
            if (SuspectCar.Exists()) SuspectCar.Dismiss();
            if (officer.Exists())
            {
                if (CalloutStates == ECalloutStates.Finish)
                {
                    if (officer.IsAlive)
                    {
                        Peralatan.DisplayNotifWithLogo("~y~Stabbed Officer Status Update:~s~~n~~g~Rescued~s~ by paramedic", "Officer Stabbed");
                        officer.Dismiss();
                    }
                    else
                    {
                        Peralatan.DisplayNotifWithLogo("~y~Stabbed Officer Status Update:~r~Deceased~s~", "Officer Stabbed");
                        officer.Dismiss();
                    }
                }
                else
                {
                    if (officer.IsAlive) officer.Dismiss();
                    else officer.Delete();
                }
            }
            if (offVeh.Exists()) offVeh.Dismiss();
            susVehModel.Dismiss();
            if (AmbulanceModel.IsLoaded) AmbulanceModel.Dismiss();
            if (AmbulancePedModel.IsLoaded) AmbulancePedModel.Dismiss();
            if (NotepadModel.IsLoaded) NotepadModel.Dismiss();
            if (Notepad.Exists()) { Notepad.Detach(); GameFiber.Wait(75); Notepad.Delete(); }
            GameFiber.StartNew(() =>
            {
                if (Ambulance.Exists())
                {
                    AmbulancePed1.Tasks.FollowNavigationMeshToPosition(Ambulance.Position + Ambulance.LeftPosition * 1.8f, Ambulance.Heading, 10f);
                    AmbulancePed2.Tasks.FollowNavigationMeshToPosition(Ambulance.Position + Ambulance.RightPosition * 1.8f, Ambulance.Heading, 10f).WaitForCompletion(7000);
                    Ambulance.LockStatus = VehicleLockStatus.Unlocked;
                    AmbulancePed1.Tasks.EnterVehicle(Ambulance, -1);
                    AmbulancePed2.Tasks.EnterVehicle(Ambulance, 0).WaitForCompletion(3500);
                    if (!AmbulancePed1.IsInVehicle(Ambulance, false)) AmbulancePed1.Tasks.EnterVehicle(Ambulance, -1, EnterVehicleFlags.WarpTo).WaitForCompletion(200);
                    if (!AmbulancePed2.IsInVehicle(Ambulance, false)) AmbulancePed2.Tasks.EnterVehicle(Ambulance, 0, EnterVehicleFlags.WarpTo).WaitForCompletion(200);
                    if (!AmbulancePed1.IsInVehicle(Ambulance, false)) AmbulancePed1.WarpIntoVehicle(Ambulance, -1);
                    if (!AmbulancePed2.IsInVehicle(Ambulance, false)) AmbulancePed2.WarpIntoVehicle(Ambulance, 0);
                    GameFiber.Sleep(2000);
                    Ambulance.Occupants.ToList().ForEach(p => p.Dismiss());
                    Ambulance.Dismiss();
                }
            });          
            base.End();
        }
        private void DisplayCodeFourMessage()
        {
            if (!CalloutRunning) return;
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH WE_ARE_CODE FOUR NO_FURTHER_UNITS_REQUIRED");
            End();
        }
        private void GetClose()
        {
            int counter = 0;
            while (CalloutRunning)
            {
                GameFiber.Yield();
                counter++;
                if (Game.LocalPlayer.Character.DistanceTo(officer) < 45f)
                {
                    if (GrammarPoliceRunning) API.GrammarPoliceFunc.SetStatus(API.GrammarPoliceFunc.EGrammarPoliceStatusType.OnScene);
                    break;
                }          
            }
            if (Blip.Exists())
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
                    offVeh.Position = SpawnPoint;
                    offVeh.Heading = SpawnHeading;
                    officer.Tasks.LeaveVehicle(LeaveVehicleFlags.None).WaitForCompletion(4000);
                    if (officer.IsInVehicle(offVeh, false)) officer.Position = offVeh.GetOffsetPosition(Vector3.RelativeLeft * 2f);
                    officer.Kill();
                    uint deathTime = Game.GameTime;
                    CalloutRunning = true;
                    GetClose();
                    "We have send an ambulance to your location".DisplayNotifWithLogo("Officer Stabbed");
                    if (Initialization.IsLSPDFRPluginRunning("BetterEMS"))
                    {
                        API.BetterEMSFunc.SetPedDeathDetails(officer, injuredBodyParts.GetRandomElement(), "Stabbed with a knife", deathTime, 0.8f);
                        API.BetterEMSFunc.RequestAmbulancePickup(officer);
                    }
                    else if (UltimateBackupRunning) API.UBFunc.CallUnit(API.UBFunc.EUltimateBackupResponseType.Ambulance);
                    else Functions.RequestBackup(officer.Position, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.Ambulance);
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        if (Game.LocalPlayer.Character.Position.DistanceTo(SpawnPoint) < 13f) break;
                    }
                    Functions.PlayPlayerRadioAction(Functions.GetPlayerRadioAction(), 5000);
                    Game.HideHelp();
                    Game.DisplayHelp("~o~Stand by for suspect details", true);
                    GameFiber.Sleep(Peralatan.Random.Next(8000, 10000));
                    Game.HideHelp();
                    Game.DisplaySubtitle("~g~Dispatch~s~: the Ambulance will handle the officer you can go and chase the suspect");
                    Suspect.DisplayNotificationsWithPedHeadshot("Suspect Details", $"~y~Name~s~: {DriverPersona.FullName}~n~Suspect is driving a ~y~{susVehColor} {SuspectCar.Model.Name.ToUpper()}~s~ in " +
                        $"~g~{Suspect.Position.GetZoneName()}~s~~n~~y~License Plate~s~: {SuspectCar.LicensePlate}");
                    Suspect.Tasks.CruiseWithVehicle(18f, VehicleDrivingFlags.AllowMedianCrossing | VehicleDrivingFlags.DriveAroundPeds |
                VehicleDrivingFlags.DriveAroundVehicles | VehicleDrivingFlags.DriveAroundVehicles);
                    Functions.PlayScannerAudioUsingPosition($"BAR_TARGET_PLATE {Peralatan.GetLicensePlateAudio(SuspectCar)} SUSPECT_LAST_SEEN IN_OR_ON_POSITION", Suspect.Position);
                    if (Blip.Exists()) Blip.Delete();
                    Blip = new Blip(SuspectCar.Position.Around(10f), 80f);
                    Blip.Color = Color.Yellow;
                    Blip.EnableRoute(Color.Yellow);
                    var currSusPos = Suspect.Position;
                    int updateCount = 0;
                    while (CalloutRunning)
                    {
                        Suspect.Health = Suspect.MaxHealth;
                        if (passenger.Exists()) passenger.Health = passenger.MaxHealth;
                        if (Suspect.DistanceTo(currSusPos) > 80f)
                        {
                            updateCount++;
                            currSusPos = Suspect.Position;
                            if (Blip.Exists()) Blip.Delete();
                            Blip = new Blip(currSusPos.Around2D(10f), 80f);
                            Blip.Color = Color.Yellow;
                            Blip.EnableRoute(Color.Yellow);
                            if (updateCount % 5 == 0)
                            {
                                Functions.PlayScannerAudioUsingPosition($"SUSPECT_HEADING {Suspect.GetCardinalDirectionLowDetailedAudio()} IN_OR_ON_POSITION", currSusPos);
                                Suspect.DisplayNotificationsWithPedHeadshot("Suspect Details", $"~y~Name~s~: {DriverPersona.FullName}~n~Suspect is driving a ~y~{susVehColor} {SuspectCar.Model.Name.ToUpper()}~s~~n~" +
                                    $"Suspect Location: ~g~{Suspect.Position.GetZoneName()}~s~ Near ~g~{World.GetStreetName(Suspect.Position)}~n~~y~License Plate~s~: {SuspectCar.LicensePlate}");
                            }                        
                        }
                        if (Game.LocalPlayer.Character.DistanceTo(Suspect) < 15f)
                        {
                            var chaseScenario = Peralatan.Random.Next(7250);
                            if (chaseScenario % 2 == 0)
                            {
                                Pursuit = Functions.CreatePursuit();
                                Functions.AddPedToPursuit(Pursuit, Suspect);
                                if (passenger.Exists() && passenger.IsInVehicle(SuspectCar, false))
                                {
                                    if (Peralatan.Random.Next(1500) % 2 == 0) passenger.Tasks.LeaveVehicle(SuspectCar, LeaveVehicleFlags.BailOut);
                                    Functions.AddPedToPursuit(Pursuit, passenger);
                                }
                                Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                                Functions.RequestBackup(Suspect.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
                                PursuitCreated = true;
                            }
                            else
                            {
                                Game.DisplayHelp("~y~Perform a pullover on a ~r~suspect", 8500);
                            }
                            break;
                        }
                        GameFiber.Yield();
                    }
                    if (PursuitCreated)
                    {
                        if (GrammarPoliceRunning) GrammarPolice.API.Functions.InPursit(true, true);
                        if (Blip.Exists()) Blip.Delete();
                        Blip = Suspect.AttachBlip();
                        Blip.Color = Color.Firebrick;
                        Blip.IsFriendly = false;
                        Blip.EnableRoute(Blip.Color);
                        GameFiber.Wait(7000);
                        if (Functions.IsPursuitStillRunning(Pursuit)) Functions.RequestBackup(Suspect.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.NooseAirUnit);
                    }
                    else
                    {
                        GameFiber.SleepUntil(() => Functions.IsPlayerPerformingPullover(), -1);
                        Peralatan.DisplayNotifWithLogo("~r~Arrest the suspect", "~r~Officer Stabbed");
                    }
                } catch (Exception e)
                {
                    "Officer Stabbed callout crashes".ToLog();
                    e.ToString().ToLog();
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
                    offVeh.Position = SpawnPoint;
                    offVeh.Heading = SpawnHeading;
                    SuspectCar.Position = offVeh.Position + offVeh.ForwardVector * 9f;
                    SuspectCar.Heading = SpawnHeading;
                    officer.Tasks.LeaveVehicle(LeaveVehicleFlags.None).WaitForCompletion(5000);
                    if (officer.IsInVehicle(offVeh, false)) officer.Position = offVeh.GetOffsetPosition(Vector3.RelativeLeft * 2f);
                    officer.Kill();
                    uint deathTime = Game.GameTime;
                    GetClose();
                    "We have send an ambulance to your location".DisplayNotifWithLogo("Officer Stabbed");
                    if (Initialization.IsLSPDFRPluginRunning("BetterEMS"))
                    {
                        API.BetterEMSFunc.SetPedDeathDetails(officer, injuredBodyParts.GetRandomElement(), "Stabbed with a knife", deathTime, 0.8f);
                        API.BetterEMSFunc.CallAmbulance(officer.Position);
                    }
                    else if (UltimateBackupRunning) API.UBFunc.CallUnit(API.UBFunc.EUltimateBackupResponseType.Ambulance);
                    else Functions.RequestBackup(officer.Position, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.Ambulance);
                    if (passenger.Exists() && passenger.IsInAnyVehicle(false)) passenger.Tasks.LeaveVehicle(LeaveVehicleFlags.None);
                    if (Suspect.IsInAnyVehicle(false)) Suspect.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen).WaitForCompletion(5000);
                    if (Suspect.IsInAnyVehicle(false)) Suspect.Tasks.LeaveVehicle(LeaveVehicleFlags.WarpOut).WaitForCompletion();
                    if (passenger.Exists()) passenger.Tasks.Cower(-1);
                    if (Blip.Exists()) Blip.Delete();
                    Blip = Suspect.AttachBlip();
                    Blip.Color = Color.Red;
                    Blip.Scale = 0.80f;
                    Suspect.Tasks.FollowNavigationMeshToPosition(officer.Position + Vector3.RelativeLeft, officer.Heading - 180f, 5f).WaitForCompletion(6000);
                    Suspect.Tasks.AchieveHeading(Suspect.GetHeadingTowards(officer)).WaitForCompletion(800);
                    Suspect.Tasks.PlayAnimation("amb@medic@standing@tendtodead@idle_a", tendToDeadIdles.GetRandomElement(), 2.0f, AnimationFlags.Loop);
                    Suspect.PlayAmbientSpeech(null, Speech.GENERIC_SHOCKED_HIGH, 0, SpeechModifier.ForceShouted);
                    Game.DisplayHelp($"~y~Approach the suspect and press {Peralatan.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.Y)} to talk");
                    List<string> conv = new List<string>()
                    {
                        "~r~Suspect~s~: " + proses2Sorry.GetRandomElement(),
                        "~b~Officer~s~: there is must be a reason this officer doing that",
                        "~r~Suspect~s~: You are all the same, a police officer should be protect the society, instead of being oppressive",
                        "~b~Officer~s~: Come on give me your hand, let's continue the conversation at the police station",
                    };
                    while (CalloutRunning)
                    {
                        if (Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                        {
                            if (PlayerPed.DistanceTo(Suspect) < 15f) break;
                            else Game.DisplayHelp("~y~Please get ~g~closer~y~ to the ~r~suspect~y~ first");
                        }
                        GameFiber.Yield();
                    }
                    Suspect.Tasks.ClearImmediately();
                    Suspect.Tasks.AchieveHeading(Suspect.GetHeadingTowards(PlayerPed)).WaitForCompletion(800);
                    Peralatan.HandleSpeech(conv, Suspect);
                    if (passenger.Exists())
                    {
                        GameFiber.StartNew(() =>
                        {
                            if (PlayerPed.LastVehicle.Exists() && PlayerPed.LastVehicle.DistanceTo(SpawnPoint) < 85f)
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
                            GameFiber.SleepUntil(() => passenger.Tasks.CurrentTaskStatus == Rage.TaskStatus.None || !CalloutRunning, -1);
                        });
                    }
                    var rands = Peralatan.Random.Next(1, 100);
                    $"Officer Stabbed scenario {rands}".ToLog();
                    if (rands < 50)
                    {
                        Suspect.Tasks.PutHandsUp(-1, PlayerPed);
                        Game.DisplayHelp("~y~Suspect has ~g~surrendered~s~, now ~o~handcuff~y~ the ~r~suspect");
                        GameFiber.SleepUntil(() => Functions.IsPedBeingCuffed(Suspect) || Functions.IsPedArrested(Suspect), -1);
                    }
                    else if (rands < 75)
                    {
                        Suspect.MaxHealth = Peralatan.Random.Next(1250, 2850);
                        Suspect.Health = Suspect.MaxHealth;
                        $"Health scenario {Suspect.MaxHealth}".ToLog();
                        Suspect.Armor = 500;
                        Suspect.Inventory.GiveNewWeapon(new WeaponAsset(weapons.GetRandomElement()), -1, true);
                        Suspect.Tasks.FightAgainst(PlayerPed);
                    } else
                    {
                        Suspect.Tasks.FollowNavigationMeshToPosition(SuspectCar.Position + SuspectCar.RightPosition * -1.8f, SuspectCar.Heading, 10f).WaitForCompletion(-1);
                        Suspect.Tasks.EnterVehicle(SuspectCar, -1, EnterVehicleFlags.AllowJacking);
                        Pursuit = Functions.CreatePursuit();
                        Functions.AddPedToPursuit(Pursuit, Suspect);
                        Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                        if (Blip.Exists() && Blip.Entity == Suspect) Blip.EnableRoute(Blip.Color);
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
                    "Officer Stabbed callout crashed, please send your log".DisplayNotifWithLogo("Officer Stabbed");
                    End();
                }
            });
        }
        private List<string> proses2Sorry = new List<string>()
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
                    offVeh.Position = SpawnPoint;
                    offVeh.Heading = SpawnHeading;
                    if (Peralatan.Random.Next(100) % 2 == 0) 
                    { 
                        if (offVeh.Doors[0].IsValid()) 
                            offVeh.Doors[0].Open(true); 
                    }
                    GameFiber.Wait(75);
                    if (Peralatan.Random.Next(250) % 2 == 0) 
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
                    Ambulance.RandomiseLicencePlate();
                    if (UltimateBackupRunning)
                    {
                        AmbulancePed1 = API.UBFunc.GetPed(API.UBFunc.EUltimateBackupUnitType.Ambulance, SpawnPoint, SpawnHeading);
                        AmbulancePed2 = API.UBFunc.GetPed(API.UBFunc.EUltimateBackupUnitType.Ambulance, SpawnPoint, SpawnHeading);
                    }
                    else
                    {
                        AmbulancePedModel.LoadAndWait();
                        AmbulancePed1 = new Ped(AmbulancePedModel, SpawnPoint, SpawnHeading);
                        AmbulancePed2 = new Ped(AmbulancePedModel, SpawnPoint, SpawnHeading);
                    }
                    AmbulancePed1.RelationshipGroup = RelationshipGroup.Medic;
                    AmbulancePed2.RelationshipGroup = RelationshipGroup.Medic;
                    AmbulancePed1.MakeMissionPed(true);
                    AmbulancePed2.MakeMissionPed(true);
                    Notepad = new Rage.Object(NotepadModel, Vector3.Zero);
                    Notepad.AttachTo(AmbulancePed2, AmbulancePed2.GetBoneIndex(PedBoneId.LeftPhHand), Vector3.Zero, Rotator.Zero);
                    AmbulancePed1.Position = medPos1; AmbulancePed1.Heading = AmbulancePed1.GetHeadingTowards(officer);
                    GameFiber.Wait(75);
                    AmbulancePed2.Position = AmbulancePed1.Position + AmbulancePed2.ForwardVector * 2.225f; AmbulancePed2.Heading = AmbulancePed2.GetHeadingTowards(officer);
                    AmbulancePed1.Tasks.PlayAnimation("amb@medic@standing@tendtodead@idle_a", tendToDeadIdles.GetRandomElement(), 2.0f, AnimationFlags.Loop);
                    GameFiber.Wait(75);
                    AmbulancePed2.Tasks.PlayAnimation("amb@medic@standing@timeofdeath@base", "base", 2.0f, AnimationFlags.Loop);
                    officer.Kill();
                    GetClose();
                    Suspect.Tasks.CruiseWithVehicle(20f, VehicleDrivingFlags.Emergency);
                    Game.DisplayHelp($"~y~Get ~o~closer~y~ to the ~g~paramedic~y~ and press {Peralatan.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.Y)}~s~ ~y~to talk");
                    while (CalloutRunning)
                    {
                        if (Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                        {
                            if (PlayerPed.DistanceTo(AmbulancePed1) < 6f)
                            {
                                Game.DisplaySubtitle("~b~Officer~s~: Is he alright? Where is the suspect?");
                                break;
                            }
                            else Game.DisplayHelp("Please get ~o~closer with ~g~paramedic");
                        }
                        GameFiber.Yield();
                    }
                    List<string> ambulanceConversation = new List<string>()
                     {           
                        "~g~Paramedic~s~: Calm down officer, We can handle this, now you can search for the suspect",           
                        $"~g~Paramedic~s~: All i know he is driving a ~y~{susVehColor} {susVehModel.Name}~s~ with license plate ~g~{SuspectCar.LicensePlate}~s~",
                        "~b~Officer~s~: Thank you for the information i will search the suspect now",
                        "~g~Paramedic~s~: Please be careful, suspect may bring some weapon",
                        "~b~Officer~s~: Sure, thank you"
                    };
                    var amb1Heading = AmbulancePed1.Heading;
                    Peralatan.HandleSpeech(ambulanceConversation, AmbulancePed1);
                    Functions.PlayScannerAudio($"ATTENTION_ALL_UNITS BAR_TARGET_PLATE {Peralatan.GetLicensePlateAudio(SuspectCar)}");
                    GameFiber.StartNew(() =>
                    {
                        AmbulancePed1.Tasks.FollowNavigationMeshToPosition(medPos1, amb1Heading, 1f).WaitForCompletion(-1);
                        AmbulancePed1.Tasks.PlayAnimation("amb@medic@standing@tendtodead@idle_a", tendToDeadIdles.GetRandomElement(), 2.0f, AnimationFlags.Loop);
                    });
                    Game.DisplayHelp("~y~Standby for suspect details");
                    GameFiber.Wait(Peralatan.Random.Next(7500, 10000));
                    if (Suspect.DistanceTo(PlayerPed) > 850f || Suspect.TravelDistanceTo(PlayerPed) > 1250f)
                    {
                        for (int i = 0; i < 50; i++)
                        {
                            if (SpawnPoint.Around(350, 450).GetRoadSidePointWithHeading(out Vector3 tmpP, out float tmpH))
                            {
                                SuspectCar.Position = tmpP;
                                SuspectCar.Heading = tmpH;
                            }
                            else SuspectCar.Position = World.GetNextPositionOnStreet(SpawnPoint.Around(350, 450));
                        }
                    }
                    Suspect.DisplayNotificationsWithPedHeadshot("Suspect Details", $"~y~Name~s~: {DriverPersona.FullName}~n~Suspect is driving a ~y~{susVehColor} {SuspectCar.Model.Name.ToUpper()}~s~ in " +
                        $"~g~{Suspect.Position.GetZoneName()}~s~~n~~y~License Plate~s~: {SuspectCar.LicensePlate}");
                    Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS SUSPECT_LAST_SEEN IN_OR_ON_POSITION", Suspect.Position);
                    int updateCount = 0;
                    var currSusPos = Suspect.Position;
                    while (CalloutRunning)
                    {
                        Suspect.Health = Suspect.MaxHealth;
                        if (passenger.Exists()) passenger.Health = passenger.MaxHealth;
                        if (Suspect.DistanceTo(currSusPos) > 80f)
                        {
                            updateCount++;
                            currSusPos = Suspect.Position;
                            if (Blip.Exists()) Blip.Delete();
                            Blip = new Blip(currSusPos.Around2D(10f), 80f);
                            Blip.Color = Color.Yellow;
                            Blip.EnableRoute(Color.Yellow);
                            if (updateCount % 5 == 0)
                            {
                                Functions.PlayScannerAudioUsingPosition($"SUSPECT_HEADING {Suspect.GetCardinalDirectionLowDetailedAudio()} IN_OR_ON_POSITION", currSusPos);
                                Suspect.DisplayNotificationsWithPedHeadshot("Suspect Details", $"~y~Name~s~: {DriverPersona.FullName}~n~Suspect is driving a ~y~{susVehColor} {SuspectCar.Model.Name.ToUpper()}~s~~n~" +
                                    $"Suspect Location: ~g~{Suspect.Position.GetZoneName()}~s~ Near ~g~{World.GetStreetName(Suspect.Position)}~n~~y~License Plate~s~: {SuspectCar.LicensePlate}");
                            }
                        }
                        if (Game.LocalPlayer.Character.DistanceTo(Suspect) < 15f)
                        {
                            var chaseScenario = Peralatan.Random.Next(1, 1000);
                            $"Suspect chase scenario : {chaseScenario}".ToLog();
                            if (chaseScenario > 250)
                            {
                                Pursuit = Functions.CreatePursuit();
                                Functions.AddPedToPursuit(Pursuit, Suspect);
                                if (passenger.Exists() && passenger.IsInVehicle(SuspectCar, false))
                                {
                                    if (Peralatan.Random.Next(1500) % 2 == 0) passenger.Tasks.LeaveVehicle(SuspectCar, LeaveVehicleFlags.BailOut);
                                    Functions.AddPedToPursuit(Pursuit, passenger);
                                }
                                Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                                Functions.RequestBackup(Suspect.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
                                PursuitCreated = true;
                            }
                            else
                            {
                                Game.DisplayHelp("Suspect found. ~y~Perform a pullover on a ~r~suspect ~y~ and handle the situation as you see fit", 8500);
                            }
                            break;
                        }
                        GameFiber.Yield();
                    }
                    if (Peralatan.Random.Next(1, 100) > 30) { if (officer.Exists() && officer.IsDead) officer.Resurrect(); }
                    if (Blip.Exists()) Blip.Delete();
                    Blip = Suspect.AttachBlip();
                    Blip.Color = Color.Firebrick;
                    Blip.IsFriendly = false;
                    if (PursuitCreated)
                    {
                        if (GrammarPoliceRunning) GrammarPolice.API.Functions.InPursit(true, true);                       
                        Blip.EnableRoute(Blip.Color);
                        GameFiber.Wait(Peralatan.Random.Next(4500, 7850));
                        if (Functions.IsPursuitStillRunning(Pursuit)) Functions.RequestBackup(Suspect.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.NooseAirUnit);
                    }
                    else
                    {
                        while (CalloutRunning)
                        {
                            if (Functions.IsPlayerPerformingPullover())
                            {
                                PullOver = Functions.GetCurrentPullover();
                                if (Functions.GetPulloverSuspect(PullOver) == Suspect)
                                break;
                            }
                            GameFiber.Yield();
                        }
                        Peralatan.DisplayNotifWithLogo("~r~Arrest the suspect", "~y~Officer Stabbed");
                    }
                }
                catch (Exception e)
                {
                    "Officer stabbed callout crashed".ToLog();
                    e.ToString().ToLog();
                    "Officer Stabbed callout crashed, please send your log".DisplayNotifWithLogo("Officer Stabbed");
                    End();
                }
            });
        }
        private void Proses4()
        {
            GameFiber.StartNew(() =>
            {
                try
                {
                    Suspect.DisplayNotificationsWithPedHeadshot("~y~Driver Details", $"~y~Name~s~: {DriverPersona.FullName}~n~" +
                        $"~y~BirthDay~s~: {DriverPersona.Birthday}");
                    passenger.DisplayNotificationsWithPedHeadshot("~y~Passenger Details", $"~y~Name~s~: {PassengerPersona.FullName}~n~" +
                        $"~y~BirthDay~s~: {PassengerPersona.Birthday}");
                    $"~y~Model~s~: {Game.GetLocalizedString(SuspectCar.Model.Name)}~n~~y~Color~s~: {susVehColor}~n~~y~License Plate~s~: {SuspectCar.LicensePlate}".DisplayNotifWithLogo("Vehicle Details");
                    CalloutRunning = true;
                    offVeh.Position = SpawnPoint;
                    offVeh.Heading = SpawnHeading;
                    if (!passenger.Exists())
                    {
                        passenger = new Ped(SpawnPoint);
                        passenger.MakeMissionPed();
                        passenger.SetPedAsWanted();
                        passenger.RelationshipGroup = Suspect.RelationshipGroup;
                        passenger.WarpIntoVehicle(SuspectCar, 0);
                        PassengerPersona = Functions.GetPersonaForPed(passenger);
                        passenger.MaxHealth = 2500;
                        passenger.Armor = 500;
                    }
                    SuspectCar.Position = offVeh.Position + offVeh.ForwardVector * 9f; SuspectCar.Heading = SpawnHeading;
                    Suspect.MaxHealth = 2500;
                    Suspect.Armor = 500;
                    officer.Tasks.LeaveVehicle(LeaveVehicleFlags.WarpOut).WaitForCompletion(500);
                    officer.Position = offVeh.GetOffsetPosition(Vector3.RelativeLeft * 2f);
                    officer.Kill();
                    GetClose();
                    if (Suspect.IsInVehicle(SuspectCar, false)) Suspect.Tasks.LeaveVehicle(LeaveVehicleFlags.None);
                    if (passenger.IsInVehicle(SuspectCar, false)) passenger.Tasks.LeaveVehicle(LeaveVehicleFlags.None).WaitForCompletion();
                    Suspect.Inventory.GiveNewWeapon(new WeaponAsset(weapons.GetRandomElement()), -1, true);
                    GameFiber.Wait(75);
                    passenger.Inventory.GiveNewWeapon(new WeaponAsset(weapons.GetRandomElement()), -1, true);
                    Suspect.Tasks.FightAgainst(PlayerPed);
                    passenger.Tasks.FightAgainst(PlayerPed);
                    GameFiber.Wait(2000);
                    Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS BAR_CODE_99 IN_OR_ON_POSITION", PlayerPed.Position);
                }
                catch (Exception e)
                {
                    e.Message.ToLog();
                    "Officer stabbed callout crashed".ToLog();
                    e.ToString().ToLog();
                    "Officer Stabbed callout crashed, please send your log".DisplayNotifWithLogo("Officer Stabbed");
                }
            });
        }
    }
}
