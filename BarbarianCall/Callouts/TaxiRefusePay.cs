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
            SituationFight();
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
                    Blip.Color = Color.Orange;
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
                            if (Peralatan.CheckKey(System.Windows.Forms.Keys.LControlKey, System.Windows.Forms.Keys.Y)) 
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
                    Suspect.Tasks.AimWeaponAt(TaxiDriver, -1);
                    TaxiDriver.Tasks.PutHandsUp(-1, Suspect);
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
                        Suspect.Tasks.FireWeaponAt(TaxiDriver.GetBonePosition(PedBoneId.Head), 15, FiringPattern.BurstFirePistol);
                    }
                    else if (HostageScenario < 2000)
                    {
                        Suspect.
                    }
                    while (CalloutRunning)
                    {
                        if (SuspectState != ESuspectStates.InAction) DisplayCodeFourMessage();
                    }
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
