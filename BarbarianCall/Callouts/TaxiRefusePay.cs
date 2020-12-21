using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Engine.Scripting.Entities;
using System.Drawing;
using BarbarianCall.Types;

namespace BarbarianCall.Callouts
{
    [CalloutInfo("Taxi Passenger Refuse Pay", CalloutProbability.High)]
    public class TaxiRefusePay: CalloutBase
    {
        private Ped TaxiDriver;
        private Vehicle Taxi;
        private readonly Model TaxiModel = new Model(0xC703DB5F);
        private readonly string path = @"Plugins/LSPDFR/BarbarianCall/TaxiRefusePay/";
        private RelationshipGroup TaxiRelation;
        private RelationshipGroup CriminalRelation;
        private ESuspectStates SuspectState;
        private bool VictimSafe = true;
        private bool SuspectStopped = false;
        private string[] InsultSpeech = { Speech.GENERIC_CURSE_HIGH, Speech.GENERIC_CURSE_MED, Speech.GENERIC_FUCK_YOU, Speech.GENERIC_INSULT_HIGH, Speech.GENERIC_INSULT_MED };

        private Ped Witness;
        private Blip WitnessBlip;

        private string suspectManWoman = "Man";
        private int suspectCount = 1;
        
        public override bool OnBeforeCalloutDisplayed()
        {
            DivisiXml.Deserialization.GetDataFromXml(path + "Locations.xml", out List<Vector3> locationToSelect, out List<float> headingToSelect);
            Peralatan.SelectNearbyLocationsWithHeading(locationToSelect, headingToSelect, out SpawnPoint, out SpawnHeading);
            if (SpawnPoint == Vector3.Zero || SpawnHeading == 0f)
            {
                Peralatan.ToLog("Officer Stabbed callout aborted");
                Peralatan.ToLog("No nearby location found");
                return false;
            }
            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 20f);
            AddMinimumDistanceCheck(100f, SpawnPoint);
            CalloutPosition = SpawnPoint;
            CalloutMessage = "Taxi Passenger Refuse To Pay";
            SuspectStopped = false;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS CITIZENS_REPORT BAR_CRIME_CIVILIAN_NEEDING_ASSISTANCE IN_OR_ON_POSITION", SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            VictimSafe = true;
            SuspectState = ESuspectStates.InAction;
            CalloutStates = ECalloutStates.EnRoute;
            TaxiModel.LoadAndWait();
            TaxiRelation = new RelationshipGroup("TAXI");
            CriminalRelation = new RelationshipGroup("CRIMINAL");
            Taxi = new Vehicle(TaxiModel, SpawnPoint, SpawnHeading);
            Taxi.MakePersistent();
            Taxi.Mods.ApplyAllMods();
            Taxi.RandomiseLicencePlate();
            TaxiDriver = Taxi.CreateRandomDriver();
            TaxiDriver.MakeMissionPed();
            TaxiDriver.RelationshipGroup = TaxiRelation;
            TaxiDriver.MaxHealth = 1000;
            Suspect = new Ped(SpawnPoint);
            Suspect.WarpIntoVehicle(Taxi, 2);
            Suspect.MakeMissionPed();
            Suspect.RelationshipGroup = CriminalRelation;
            Suspect.MaxHealth = 1500;
            Suspect.FatalInjuryHealthThreshold.ToString().ToLog();
            SuspectPersona = Functions.GetPersonaForPed(Suspect);
            Blip = new Blip(SpawnPoint, 45f);
            Blip.Color = Color.Yellow;
            Blip.EnableRoute(Color.Yellow);
            Game.SetRelationshipBetweenRelationshipGroups(TaxiRelation, CriminalRelation, Relationship.Hate);
            Game.SetRelationshipBetweenRelationshipGroups(TaxiRelation, RelationshipGroup.Cop, Relationship.Respect);
            Game.SetRelationshipBetweenRelationshipGroups(CriminalRelation, TaxiRelation, Relationship.Hate);
            Game.SetRelationshipBetweenRelationshipGroups(CriminalRelation, RelationshipGroup.Cop, Relationship.Dislike);
            Game.SetRelationshipBetweenRelationshipGroups(CriminalRelation, RelationshipGroup.Player, Relationship.Dislike);
            Game.SetRelationshipBetweenRelationshipGroups(RelationshipGroup.Cop, TaxiRelation, Relationship.Neutral);
            Game.SetRelationshipBetweenRelationshipGroups(RelationshipGroup.Cop, CriminalRelation, Relationship.Dislike);
            suspectManWoman = Suspect.IsMale ? "Man" : "Woman";
            SituationDead();
            return base.OnCalloutAccepted();
        }
        public override void End()
        {           
            if (Suspect.Exists() && !Functions.IsPedArrested(Suspect)) Suspect.Dismiss();
            TaxiModel.Dismiss();
            GameFiber.StartNew(() =>
            {
                if (Taxi.Exists() && TaxiDriver.Exists() && !Functions.IsPedArrested(TaxiDriver))
                {
                    TaxiDriver.Tasks.FollowNavigationMeshToPosition(Taxi.LeftPosition, Taxi.Heading, 10f).WaitForCompletion(10000);
                    if (Taxi.Exists() && TaxiDriver.Exists()) TaxiDriver.Tasks.EnterVehicle(Taxi, -1).WaitForCompletion(5000);
                    if (Taxi.Exists() && TaxiDriver.Exists() && TaxiDriver.IsInVehicle(Taxi, false)) TaxiDriver.Tasks.EnterVehicle(Taxi, -1, EnterVehicleFlags.WarpTo).WaitForCompletion(3000);
                    if (Taxi.Exists() && TaxiDriver.Exists() && TaxiDriver.IsInVehicle(Taxi, false)) TaxiDriver.WarpIntoVehicle(Taxi, -1);
                    if (Taxi.Exists()) Taxi.Dismiss();
                    if (TaxiDriver.Exists()) TaxiDriver.Dismiss();
                }
            });
            if (Blip.Exists()) Blip.Delete();
            base.End();
        }
        public override void Process()
        {
            if (Suspect.Exists())
            {
                if (Functions.IsPedArrested(Suspect) && SuspectState == ESuspectStates.InAction)
                {
                    Game.DisplayNotification("Suspect is ~r~arrested");
                    SuspectState = ESuspectStates.Arrested;
                }
                else if (Suspect.IsDead && SuspectState == ESuspectStates.InAction)
                {
                    Game.DisplayNotification("Suspect is ~r~dead");
                    SuspectState = ESuspectStates.Dead;
                }
                else if (PursuitCreated && !Suspect.Exists())
                {
                    Game.DisplayNotification("Suspect is escaped");
                    SuspectState = ESuspectStates.Escaped;
                }
            }           
            if (TaxiDriver.Exists())
            {
                if (TaxiDriver.IsDead && VictimSafe)
                {
                    Game.DisplayNotification("Victim is ~r~dead");
                    VictimSafe = false;
                }
            }
            base.Process();
        }
        private void DisplayCodeFourMessage()
        {
            if (!CalloutRunning) return;
            CalloutStates = ECalloutStates.Finish;
            string victim = VictimSafe ? "~g~safe~s~" : "~r~dead~s~";
            $"~g~We Are Code 4.~s~ ~y~Suspect~s~ is ~o~{SuspectState}~s~ and ~b~victim~s~ is {victim}".DisplayNotifWithLogo("Taxi Passenger Refuse To Pay");
            Functions.PlayScannerAudioUsingPosition("BAR_ALL_UNIT_CODE_FOUR IN_OR_ON_POSITION", PlayerPed.Position);
            End();
        }
        private void GetClose()
        {
            while (CalloutRunning)
            {
                if (PlayerPed.DistanceTo(SpawnPoint) < 45f)
                {
                    if (GrammarPoliceRunning) API.GrammarPoliceFunc.SetStatus(API.GrammarPoliceFunc.EGrammarPoliceStatusType.OnScene);
                    if (Menus.PauseMenu.onSceneAudio.Checked)
                    {
                        Functions.PlayScannerAudio("ATTENTION_ALL_UNITS BAR_ON_SCENE");
                    }
                    CalloutStates = ECalloutStates.OnScene;
                    break;
                }
                GameFiber.Yield();
            }
        }
        private void SituationFight() //Geloood
        {
            GameFiber.StartNew(() =>
            {
                try
                {
                    CalloutRunning = true;
                    if (Suspect.Exists()) Suspect.Delete();
                    Suspect = new Ped(CommonVariables.MaleModel.GetRandomElement(), SpawnPoint, SpawnHeading);
                    Suspect.WarpIntoVehicle(Taxi, 2);
                    SuspectPersona = Functions.GetPersonaForPed(Suspect);
                    GameFiber.Wait(200);
                    Suspect.DisplayNotificationsWithPedHeadshot("Passenger Details", $"~y~Name~s~: {SuspectPersona.FullName}~n~" +
                        $"~y~BirthDay~s~: {SuspectPersona.Birthday.ToShortDateString()}");
                    if (Peralatan.Random.Next(2500, 3500) % 4 == 0)
                    {
                        Suspect.SetPedAsWanted();
                    }
                    Suspect.Tasks.LeaveVehicle(LeaveVehicleFlags.WarpOut).WaitForCompletion(200);
                    TaxiDriver.Tasks.LeaveVehicle(LeaveVehicleFlags.WarpOut).WaitForCompletion(200);
                    GetClose();
                    if (Blip.Exists()) Blip.Delete();
                    Blip = new Blip(Suspect);
                    Blip.Color = Color.Yellow;
                    Blip.Scale = 0.91685f;
                    Suspect.Inventory.Weapons.ToList().ForEach(w => w.Drop());
                    TaxiDriver.Inventory.Weapons.ToList().ForEach(w => w.Drop());
                    Suspect.Inventory.GiveNewWeapon("WEAPON_UNARMED", -1, true);
                    TaxiDriver.Inventory.GiveNewWeapon("WEAPON_UNARMED", -1, true);
                    Suspect.Tasks.FightAgainst(TaxiDriver);
                    TaxiDriver.Tasks.FightAgainst(Suspect);
                    Suspect.PlayAmbientSpeech(null, InsultSpeech.GetRandomElement(), 0, SpeechModifier.AllowRepeat);
                    TaxiDriver.PlayAmbientSpeech(null, InsultSpeech.GetRandomElement(), 0, SpeechModifier.AllowRepeat);
                    Game.DisplayHelp($"~y~Handle the situation as you see fit~n~~y~press {Peralatan.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.End)}~y~ to end the callout~n~" +
                        $"~y~Press {Peralatan.FormatKeyBinding(System.Windows.Forms.Keys.LControlKey, System.Windows.Forms.Keys.Y)}~y~ to see suspect details");
                    Timer = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    GameFiber.StartNew(() =>
                    {
                        while (CalloutRunning)
                        {
                            if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - Timer > 12000L)
                            {
                                Timer = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                Game.DisplayHelp($"~y~Handle the situation as you see fit~n~~y~press {Peralatan.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.End)}~y~ to end the callout~n~" +
                       $"~y~Press {Peralatan.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.Home)}~y~ to see suspect details");
                            }
                            if (Game.IsKeyDown(System.Windows.Forms.Keys.End) || Functions.IsPedStoppedByPlayer(Suspect)) break;
                            if (StopThePedRunning)
                            {
                                if (API.StopThePedFunc.IsPedStoppedWithSTP(Suspect)) break;
                            }
                            if (Peralatan.CheckKey(System.Windows.Forms.Keys.Control, System.Windows.Forms.Keys.Y)) 
                                Suspect.DisplayNotificationsWithPedHeadshot("Passenger Details", $"~y~Name~s~: {SuspectPersona.FullName}~n~" +
                       $"~y~BirthDay~s~: {SuspectPersona.Birthday.ToShortDateString()}");
                            GameFiber.Yield();
                        }
                    });
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        if (Game.IsKeyDown(System.Windows.Forms.Keys.End)) break;
                        if (Functions.IsPedStoppedByPlayer(Suspect) && !SuspectStopped)
                        {
                            Suspect.Tasks.Clear();
                            TaxiDriver.Tasks.Clear();
                            SuspectStopped = true;
                        }
                        if (StopThePedRunning)
                        {
                            if (API.StopThePedFunc.IsPedStoppedWithSTP(Suspect) && !SuspectStopped)
                            {
                                Suspect.Tasks.Clear();
                                TaxiDriver.Tasks.Clear();
                                SuspectStopped = true;
                            }
                        }
                    }
                    DisplayCodeFourMessage();
                }
                catch (Exception e)
                {
                    "Taxi Refuse Pay callout crash".DisplayNotifWithLogo("Taxi Passenger Refuse To Pay");
                    e.ToString().ToLog();
                    End();
                }
            });
        }
        private string[] insultOfficer =
        {
            "Get out of here, this is none of your bussiness",
            "Get the f*ck out of here, or i will shoot",
            "Dont come closer, or i will shoot",
            "Stay away from me or i will shoot",
            "This is not your bussiness, stay away or i will shoot",
        };
        private void SituationHostage()
        {
            GameFiber.StartNew(() =>
            {
                try
                {
                    CalloutRunning = true;
                    Suspect.Tasks.LeaveVehicle(LeaveVehicleFlags.WarpOut);
                    TaxiDriver.Tasks.LeaveVehicle(LeaveVehicleFlags.WarpOut);
                    GameFiber.Wait(200);
                    Suspect.Position = Taxi.RearPosition;
                    TaxiDriver.Position = Taxi.FrontPosition;
                    Suspect.Heading = Suspect.GetHeadingTowards(TaxiDriver);
                    TaxiDriver.Heading = TaxiDriver.GetHeadingTowards(Suspect);
                    GameFiber.Wait(75);
                    Suspect.Inventory.GiveNewWeapon(WeaponHashes.GetRandomElement(), -1, true);
                    TaxiDriver.Inventory.GiveNewWeapon("WEAPON_UNARMED", -1, true);
                    GameFiber.Wait(75);
                    Suspect.Tasks.AimWeaponAt(TaxiDriver, -1);
                    TaxiDriver.Tasks.PutHandsUp(-1, Suspect);
                    Suspect.DisplayNotificationsWithPedHeadshot("Passenger Details", $"~y~Name~s~: {SuspectPersona.FullName}~n~" +
                       $"~y~BirthDay~s~: {SuspectPersona.Birthday.ToShortDateString()}");
                    Timer = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    while (CalloutRunning)
                    {
                        if (PlayerPed.DistanceTo(SpawnPoint) < 45f)
                        {
                            if (GrammarPoliceRunning) API.GrammarPoliceFunc.SetStatus(API.GrammarPoliceFunc.EGrammarPoliceStatusType.OnScene);
                            if (Menus.PauseMenu.onSceneAudio.Checked)
                            {
                                Functions.PlayScannerAudio("ATTENTION_ALL_UNITS BAR_ON_SCENE");
                            }
                            break;
                        }
                        if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - Timer > 10000L)
                        {

                        }
                        GameFiber.Yield();
                    }
                    Game.DisplayHelp("Your goal is to ~g~protect~s~ the ~b~victim~s~ and ~y~arrest~s~ the ~o~suspect~s~ ~g~alive");
                    if (Blip.Exists()) Blip.Delete();
                    Blip = Suspect.AttachBlip();
                    Blip.Color = Color.Red;
                    Blip.Scale = 0.9285f;
                    Game.DisplaySubtitle("~r~Suspect~s~: " + insultOfficer.GetRandomElement());
                    GameFiber.StartNew(() =>
                    {
                        Timer = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        while (CalloutRunning)
                        {
                            if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - Timer > 12000)
                            {
                                Timer = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                Game.DisplayHelp("Your goal is to ~g~protect~s~ the ~b~victim~s~ and ~y~arrest~s~ the ~o~suspect~s~ ~g~alive");
                            }
                            if (Functions.IsPedArrested(Suspect) || TaxiDriver.IsDead) break;
                            GameFiber.Yield();
                        }
                    });
                    int HostageScenario = Peralatan.Random.Next(1, 4000);
                    $"Callout scenario: {HostageScenario}".ToLog();
                    if (HostageScenario < 1000)
                    {
                        Suspect.Tasks.FireWeaponAt(TaxiDriver.GetBonePosition(PedBoneId.Head), -1, FiringPattern.BurstFirePistol);
                        while (CalloutRunning)
                        {
                            GameFiber.Yield();
                            if (TaxiDriver.IsDead) break;
                            if (SuspectState != ESuspectStates.InAction) break;
                        }
                        if (SuspectState == ESuspectStates.InAction)
                        {
                            Pursuit = Functions.CreatePursuit();
                            Functions.AddPedToPursuit(Pursuit, Suspect);
                            Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                            Functions.RequestBackup(Suspect.Position, EBackupResponseType.Pursuit, EBackupUnitType.LocalUnit);
                            Functions.PlayPlayerRadioAction(Functions.GetPlayerRadioAction(), 2000);
                            if (GrammarPoliceRunning) API.GrammarPoliceFunc.SetStatus(API.GrammarPoliceFunc.EGrammarPoliceStatusType.InPursuit);
                            PursuitCreated = true;
                        }
                    }
                    else if (HostageScenario < 2000)
                    {
                        Suspect.Tasks.Flee(PlayerPed, 1000, -1);
                        GameFiber.Wait(2000);
                        GameFiber.StartNew(() =>
                        {
                            TaxiDriver.Tasks.Clear();
                            GameFiber.Wait(2000);
                            if (PlayerPed.LastVehicle.Exists() && PlayerPed.LastVehicle.DistanceTo(TaxiDriver) < 100f)
                            {
                                TaxiDriver.PlayAmbientSpeech(null, Speech.DYING_HELP, 0, SpeechModifier.AllowRepeat);
                                TaxiDriver.Tasks.FollowNavigationMeshToPosition(PlayerPed.LastVehicle.RearPosition, 
                                    MathHelper.GetRandomSingle(PlayerPed.LastVehicle.Heading, PlayerPed.LastVehicle.Heading + 180f), 10f).WaitForCompletion(20000);
                                if (TaxiDriver.Exists()) TaxiDriver.Tasks.Cower(-1);
                            }
                        });
                        Pursuit = Functions.CreatePursuit();
                        Functions.AddPedToPursuit(Pursuit, Suspect);
                        Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                        Functions.RequestBackup(Suspect.Position, EBackupResponseType.Pursuit, EBackupUnitType.LocalUnit);
                        Functions.PlayPlayerRadioAction(Functions.GetPlayerRadioAction(), 2000);
                        if (GrammarPoliceRunning) API.GrammarPoliceFunc.SetStatus(API.GrammarPoliceFunc.EGrammarPoliceStatusType.InPursuit);
                        PursuitCreated = true;
                    }
                    else if (HostageScenario < 3000)
                    {
                        Suspect.Tasks.FightAgainst(PlayerPed).WaitForCompletion(15000);
                        if (SuspectState == ESuspectStates.InAction)
                        {
                            Pursuit = Functions.CreatePursuit();
                            Functions.AddPedToPursuit(Pursuit, Suspect);
                            Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                            Functions.RequestBackup(Suspect.Position, EBackupResponseType.Pursuit, EBackupUnitType.LocalUnit);
                            Functions.PlayPlayerRadioAction(Functions.GetPlayerRadioAction(), 2000);
                            if (GrammarPoliceRunning) API.GrammarPoliceFunc.SetStatus(API.GrammarPoliceFunc.EGrammarPoliceStatusType.InPursuit);
                            PursuitCreated = true;
                        }
                    }
                    else
                    {
                        Suspect.Inventory.GiveNewWeapon(0xA89CB99E, -1, true);
                        Suspect.Tasks.FightAgainst(PlayerPed);
                    }
                    while (CalloutRunning)
                    {
                        if (SuspectState != ESuspectStates.InAction) break;
                        GameFiber.Yield();
                    }
                    DisplayCodeFourMessage();
                }
                catch (Exception e)
                {
                    "Taxi Refuse Pay callout crash".DisplayNotifWithLogo("Taxi Passenger Refuse To Pay");
                    e.ToString().ToLog();
                    End();
                }
            });
        }
        private void SituationDead()
        {
            GameFiber.StartNew(() =>
            {
                try
                {
                    CalloutRunning = true;
                    Suspect.DisplayNotificationsWithPedHeadshot("Passenger Details", $"~y~Name~s~: {SuspectPersona.FullName}~n~" +
                       $"~y~BirthDay~s~: {SuspectPersona.Birthday.ToShortDateString()}");
                    Suspect.Tasks.LeaveVehicle(LeaveVehicleFlags.WarpOut);
                    TaxiDriver.Tasks.LeaveVehicle(LeaveVehicleFlags.WarpOut);
                    TaxiDriver.Kill();
                    Vector3 suspectLocation = Vector3.Zero;
                    Vector3 witnessLocation = Vector3.Zero;
                    GameFiber.Wait(200);
                    for (int i = 0; i < 10; i++)
                    {
                        suspectLocation = SpawnManager.GetNextPositionOnSideWalk(SpawnPoint, 350);
                        witnessLocation = SpawnManager.GetNextPositionOnSideWalk(SpawnPoint, 15);
                        if (suspectLocation != Vector3.Zero) break;
                    }
                    if (suspectLocation == Vector3.Zero) suspectLocation = World.GetNextPositionOnStreet(SpawnPoint.Around(345, 350));
                    if (witnessLocation == Vector3.Zero) witnessLocation = World.GetNextPositionOnStreet(SpawnPoint.Around(25));
                    Suspect.Position = suspectLocation;
                    Witness = new Ped(witnessLocation, witnessLocation.GetHeadingTowards(SpawnPoint));
                    Suspect.Tasks.Wander();
                    GetClose();
                    if (Blip.Exists()) Blip.Delete();
                    Blip = TaxiDriver.AttachBlip();
                    Blip.Color = Color.Red;
                    WitnessBlip = Witness.AttachBlip();
                    WitnessBlip.Color = Color.Orange;
                    Game.DisplayHelp($"~y~Please move closer to the witness, press {Peralatan.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.Y)}~y~ to speak");
                    if (UltimateBackupRunning) API.UltimateBackupFunc.CallUnit(API.UltimateBackupFunc.EUltimateBackupCallType.Ambulance);
                    else Functions.RequestBackup(TaxiDriver.Position, EBackupResponseType.Code3, EBackupUnitType.Ambulance);
                    "Ambulance is en route to the scene".DisplayNotifWithLogo("~y~Taxi Passenger Refuse To Pay");
                    Timer = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        if (PlayerPed.DistanceTo(Witness) < 8f) break;
                        if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - Timer > 12000)
                        {
                            Timer = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                            Game.DisplayHelp($"~y~Please move closer to the witness");
                        }
                    }
                    Game.DisplayHelp($"~y~Press {Peralatan.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.Y)}~y~ to speak");
                    Timer = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        if (Peralatan.CheckKey(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.Y)) break;
                        if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - Timer > 12000)
                        {
                            Timer = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                            Game.DisplayHelp($"~y~Press {Peralatan.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.Y)}~y~ to speak");
                        }
                    }                   
                    List<string> witnessFlow = new List<string>()
                     {
                        "Officer: Hello are you the caller?",
                        "Witness: Yes, i was wandering around and then this incident happen",
                        "Officer: Please descrie what you saw recently",
                        $"Witness: I saw a {suspectManWoman} hit the taxi driver with a gun and then run away",
                        "Officer: Did you see where the suspecty went?",
                        $"Witness: All i know he is running ~y~{Extension.GetCardinalDirectionFullForm(SpawnPoint.GetHeadingTowards(Suspect))}~s~ leads to ~g~{Suspect.Position.GetZoneName()}",
                        "Officer: Did you know how many suspects were involved?",
                        $"Witness: I think there is {suspectCount}",
                        "Officer: Thank you for your information i will find the suspect",
                        "Witness: Okay, be careful officer"
                     };
                    Peralatan.HandleSpeech(witnessFlow, Witness);
                    Game.DisplayHelp("~y~Dispatch is trying to find ~o~suspect~y~ information, please wait...", 10000);
                    GameFiber.Wait(Peralatan.Random.Next(7500, 10000));
                    Game.HideHelp();
                    Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS SUSPECT_LAST_SEEN IN_OR_ON_POSITION", Suspect.Position);
                    Suspect.DisplayNotificationsWithPedHeadshot("Passenger Details", $"~y~Name~s~: {SuspectPersona.FullName}~n~" +
                       $"~y~BirthDay~s~: {SuspectPersona.Birthday.ToShortDateString()}~n~~y~Last Seen~s~: {Suspect.Position.GetZoneName()}");
                    if (Blip.Exists()) Blip.Delete();
                    if (WitnessBlip.Exists()) WitnessBlip.Delete();
                    Blip = new Blip(Suspect.Position.Around(10f), 70f);
                    Blip.Color = Color.Yellow;
                    Blip.EnableRoute(Color.Yellow);
                    Time = DateTime.Now + new TimeSpan(0, 0, 14);
                    while (CalloutRunning)
                    {
                        if (PlayerPed.DistanceTo(Suspect) < 18f) break;
                        if (Time >= DateTime.Now)
                        {
                            Time = DateTime.Now + new TimeSpan(0, 0, 14);
                            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS SUSPECT_LAST_SEEN IN_OR_ON_POSITION", Suspect.Position);
                            Suspect.DisplayNotificationsWithPedHeadshot("Passenger Details", $"~y~Name~s~: {SuspectPersona.FullName}~n~" +
                               $"~y~BirthDay~s~: {SuspectPersona.Birthday.ToShortDateString()}~n~~y~Last Seen~s~: {Suspect.Position.GetZoneName()}");
                        }
                    }
                    if (Peralatan.Random.Next(2150, 3500) % 6 == 0)
                    {
                        Suspect.Tasks.PutHandsUp(-1, PlayerPed);
                        Game.DisplayHelp("~y~Suspect is ~g~surrendered~y~, please perform an ~o~arrest~y~ to the ~r~suspect");
                    }
                    else
                    {
                        Pursuit = Functions.CreatePursuit();
                        Functions.AddPedToPursuit(Pursuit, Suspect);
                        Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                        Functions.RequestBackup(Suspect.Position, EBackupResponseType.Pursuit, EBackupUnitType.LocalUnit);
                        PursuitCreated = true;
                        CalloutStates = ECalloutStates.InPursuit;
                        if (GrammarPoliceRunning) API.GrammarPoliceFunc.SetStatus(API.GrammarPoliceFunc.EGrammarPoliceStatusType.InPursuit);
                    }
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        if (SuspectState != ESuspectStates.InAction) break;
                    }
                    DisplayCodeFourMessage();
                }
                catch (Exception e)
                {
                    "Taxi Refuse Pay callout crash".DisplayNotifWithLogo("Taxi Passenger Refuse To Pay");
                    e.ToString().ToLog();
                    End();
                }
            });
        }       
    }
}
