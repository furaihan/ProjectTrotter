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
        private bool SuspectStopped = false;
        private readonly string[] InsultSpeech = { Speech.GENERIC_CURSE_HIGH, Speech.GENERIC_CURSE_MED, Speech.GENERIC_FUCK_YOU, Speech.GENERIC_INSULT_HIGH, Speech.GENERIC_INSULT_MED };

        private Ped Witness;
        private Vehicle WitnessCar;
        private Blip WitnessBlip;

        private string suspectManWoman = "Man";
        private readonly int suspectCount = 1;
        
        public override bool OnBeforeCalloutDisplayed()
        {
            CheckOtherPluginRunning();
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
            CalloutMessage = "Taxi Refuse Pay";
            SuspectStopped = false;
            PlayScannerWithCallsign("CITIZENS_REPORT BAR_CRIME_CIVILIAN_NEEDING_ASSISTANCE IN_OR_ON_POSITION", SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
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
            TaxiRelation.SetRelationshipWith(RelationshipGroup.Player, Relationship.Respect);
            Game.SetRelationshipBetweenRelationshipGroups(CriminalRelation, TaxiRelation, Relationship.Hate);
            Game.SetRelationshipBetweenRelationshipGroups(CriminalRelation, RelationshipGroup.Cop, Relationship.Hate);
            Game.SetRelationshipBetweenRelationshipGroups(CriminalRelation, RelationshipGroup.Player, Relationship.Hate);
            Game.SetRelationshipBetweenRelationshipGroups(RelationshipGroup.Cop, TaxiRelation, Relationship.Neutral);
            Game.SetRelationshipBetweenRelationshipGroups(RelationshipGroup.Cop, CriminalRelation, Relationship.Neutral);
            suspectManWoman = Suspect.IsMale ? "Man" : "Woman";
            var calloutScenario = Peralatan.Random.Next(1, 300);
            $"Callout scenario {calloutScenario}".ToLog();
            if (Peralatan.Random.Next() % 2 == 0) SituationCommon();
            else if (calloutScenario < 100) SituationFight();
            else if (calloutScenario < 200) SituationHostage();
            else SituationDead();
            return base.OnCalloutAccepted();
        }
        public override void End()
        {
            CalloutRunning = false;
            if (Suspect && !Functions.IsPedArrested(Suspect)) Suspect.Dismiss();
            TaxiModel.Dismiss();
            GameFiber.StartNew(() =>
            {
                if (Taxi && TaxiDriver && TaxiDriver.IsAlive && !Functions.IsPedArrested(TaxiDriver))
                {
                    TaxiDriver.Tasks.FollowNavigationMeshToPosition(Taxi.LeftPosition, Taxi.Heading, 10f).WaitForCompletion(10000);
                    if (Taxi && TaxiDriver) TaxiDriver.Tasks.EnterVehicle(Taxi, -1).WaitForCompletion(5000);
                    if (Taxi && TaxiDriver && TaxiDriver.IsInVehicle(Taxi, false)) TaxiDriver.Tasks.EnterVehicle(Taxi, -1, EnterVehicleFlags.WarpTo).WaitForCompletion(3000);
                    if (Taxi && TaxiDriver && TaxiDriver.IsInVehicle(Taxi, false)) TaxiDriver.WarpIntoVehicle(Taxi, -1);
                    if (Taxi) Taxi.Dismiss();
                    if (TaxiDriver) TaxiDriver.Dismiss();
                }
                if (TaxiDriver) TaxiDriver.Dismiss();
                if (Taxi) Taxi.Dismiss();
            });
            if (Blip) Blip.Delete();
            GameFiber.StartNew(() =>
            {
                if (Witness && WitnessCar && Witness.IsAlive && !Functions.IsPedArrested(Witness))
                {
                    Witness.Tasks.FollowNavigationMeshToPosition(WitnessCar.LeftPosition, WitnessCar.Heading, 10f).WaitForCompletion(10000);
                    if (Witness) Witness.Tasks.EnterVehicle(WitnessCar, -1).WaitForCompletion(10000);
                    if (Witness && !Witness.IsInVehicle(WitnessCar, false)) Witness.Tasks.EnterVehicle(WitnessCar, -1, EnterVehicleFlags.WarpTo).WaitForCompletion(3000);
                    if (Witness && !Witness.IsInVehicle(WitnessCar, false)) Witness.WarpIntoVehicle(WitnessCar, -1);
                    if (Witness) Witness.Dismiss();
                }
                if (WitnessCar && WitnessCar.Driver == null)
                {
                    var pas = WitnessCar.GetPedOnSeat(0);
                    if (pas) pas.WarpIntoVehicle(WitnessCar, -1);
                    if (pas) pas.Dismiss();
                }
                if (WitnessCar) WitnessCar.Occupants.ToList().ForEach(p => { if (p) p.Dismiss(); });
                if (WitnessCar) WitnessCar.Dismiss();
            });           
            base.End();
        }
        public override void Process()
        {            
            if (Suspect)
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
                else if (PursuitCreated && !Suspect)
                {
                    Game.DisplayNotification("Suspect is escaped");
                    SuspectState = ESuspectStates.Escaped;
                }
            }                      
            base.Process();
        }
        private void DisplayCodeFourMessage()
        {
            if (!CalloutRunning) return;
            CalloutStates = ECalloutStates.Finish;
            string victim = TaxiDriver && TaxiDriver.IsAlive? "~g~safe~s~" : "~r~dead~s~";
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
            CalloutMainFiber = GameFiber.StartNew(() =>
            {
                try
                {
                    CalloutRunning = true;
                    if (Suspect) Suspect.Delete();
                    Suspect = new Ped(CommonVariables.MaleModel.GetRandomElement(), SpawnPoint, SpawnHeading);
                    Suspect.WarpIntoVehicle(Taxi, 2);
                    SuspectPersona = Functions.GetPersonaForPed(Suspect);
                    GameFiber.Wait(200);
                    suspectManWoman = Suspect.IsMale ? "Man" : "Woman";
                    Suspect.DisplayNotificationsWithPedHeadshot("Passenger Details", $"~y~Name~s~: {SuspectPersona.FullName}~n~" +
                        $"~y~BirthDay~s~: {SuspectPersona.Birthday.ToShortDateString()}");
                    if (Peralatan.Random.Next(2500, 3500) % 4 == 0)
                    {
                        Suspect.SetPedAsWanted();
                    }
                    Suspect.Tasks.LeaveVehicle(LeaveVehicleFlags.WarpOut).WaitForCompletion(200);
                    TaxiDriver.Tasks.LeaveVehicle(LeaveVehicleFlags.WarpOut).WaitForCompletion(200);
                    GetClose();
                    if (!CalloutRunning) return;
                    if (Blip) Blip.Delete();
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
                    Game.DisplayHelp($"~y~Handle the situation as you see fit~n~~y~press {Peralatan.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.End)}~y~ to end the callout");
                    Timer = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    GameFiber.StartNew(() =>
                    {
                        while (CalloutRunning)
                        {
                            if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - Timer > 12000L)
                            {
                                Timer = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                                Game.DisplayHelp($"~y~Handle the situation as you see fit~n~~y~press {Peralatan.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.End)}~y~ to end the callout");
                            }                                                        
                            GameFiber.Yield();
                            if (SuspectStopped) break;
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
                    NetExtension.SendError(e);
                    End();
                }
            });
        }
        private readonly string[] insultOfficer =
        {
            "Get out of here, this is none of your bussiness",
            "Get the f*ck out of here, or i will shoot",
            "Dont come closer, or i will shoot",
            "Stay away from me or i will shoot",
            "This is not your bussiness, stay away or i will shoot",
        };
        private void SituationHostage()
        {
            CalloutMainFiber = GameFiber.StartNew(() =>
            {
                try
                {
                    CalloutRunning = true;
                    Suspect.Tasks.LeaveVehicle(LeaveVehicleFlags.WarpOut);
                    TaxiDriver.Tasks.LeaveVehicle(LeaveVehicleFlags.WarpOut);
                    GameFiber.Wait(200);
                    Suspect.Position = Taxi.GetOffsetPositionFront(2f);
                    TaxiDriver.Position = Taxi.GetOffsetPositionFront(-2f);
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
                    Time = DateTime.UtcNow + new TimeSpan(0, 0, 20);
                    bool alerted = false;
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
                        if (DateTime.UtcNow >= Time && !Game.IsPaused && !alerted)
                        {
                            PlayScannerWithCallsign("WE_HAVE BAR_CRIME_ASSAULT IN_OR_ON_POSITION", SpawnPoint);
                            "~b~Dispatch~s~: ~y~We have report that the taxi driver is being held at gunpoint, respond ~r~Code 3~s~".DisplayNotifWithLogo("Taxi Passenger Refuse To Pay");
                            alerted = true;
                            GameFiber.WaitUntil(() => !Functions.GetIsAudioEngineBusy(), 8500);
                            Functions.PlayScannerAudio("UNITS_RESPOND_CODE_3");
                        }
                        GameFiber.Yield();
                    }
                    if (!CalloutRunning) return;
                    Game.DisplayHelp("Your goal is to ~g~protect~s~ the ~b~victim~s~ and ~y~arrest~s~ the ~o~suspect~s~ ~g~alive");
                    if (Blip) Blip.Delete();
                    Blip = Suspect.AttachBlip();
                    Blip.Color = Color.Red;
                    Blip.Scale = 0.752145f;
                    var victimBlip = TaxiDriver.AttachBlip();
                    victimBlip.Color = Color.Orange;
                    victimBlip.Scale = 0.75f;
                    CalloutBlips.Add(victimBlip);
                    Game.DisplaySubtitle("~r~Suspect~s~: " + insultOfficer.GetRandomElement());
                    GameFiber.StartNew(() =>
                    {
                        Timer = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                        while (CalloutRunning)
                        {
                            if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - Timer > 12000)
                            {
                                Timer = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                                Game.DisplayHelp("Your goal is to ~g~protect~s~ the ~p~victim~s~ and ~y~arrest~s~ the ~o~suspect~s~ ~g~alive");
                            }
                            if (Functions.IsPedArrested(Suspect) || TaxiDriver.IsDead) break;
                            GameFiber.Yield();
                        }
                    });
                    int HostageScenario = Peralatan.Random.Next(1, 4000);
                    $"Callout scenario: {HostageScenario}".ToLog();
                    while (CalloutRunning)
                    {
                        if (PlayerPed.DistanceTo(Suspect) < 12f || PlayerPed.DistanceTo(TaxiDriver) < 12f) break;
                        GameFiber.Yield();
                    }
                    if (!CalloutRunning) return;
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
                                if (TaxiDriver) TaxiDriver.Tasks.Cower(-1);
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
                        GameFiber.StartNew(() =>
                        {
                            GameFiber.Wait(2500);
                            TaxiDriver.Tasks.Clear();
                            GameFiber.Wait(1500);
                            if (PlayerPed.LastVehicle.Exists() && PlayerPed.LastVehicle.DistanceTo(TaxiDriver) < 100f)
                            {
                                TaxiDriver.PlayAmbientSpeech(null, Speech.DYING_HELP, 0, SpeechModifier.AllowRepeat);
                                TaxiDriver.Tasks.FollowNavigationMeshToPosition(PlayerPed.LastVehicle.RearPosition,
                                    MathHelper.GetRandomSingle(PlayerPed.LastVehicle.Heading, PlayerPed.LastVehicle.Heading + 180f), 10f).WaitForCompletion(20000);
                                if (TaxiDriver) TaxiDriver.Tasks.Cower(-1);
                            }
                        });
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
                    NetExtension.SendError(e);
                    End();
                }
            });
        }
        private void SituationDead()
        {
            CalloutMainFiber = GameFiber.StartNew(() =>
            {
                try
                {
                    CalloutRunning = true;
                    Suspect.DisplayNotificationsWithPedHeadshot("Passenger Details", $"~y~Name~s~: {SuspectPersona.FullName}~n~" +
                       $"~y~BirthDay~s~: {SuspectPersona.Birthday.ToShortDateString()}");
                    Suspect.Tasks.LeaveVehicle(LeaveVehicleFlags.WarpOut);
                    GameFiber.Sleep(200);
                    Suspect.IsVisible = false;
                    Functions.AddPedContraband(Suspect, ContrabandType.Weapon, "Pistol");
                    if (StopThePedRunning) API.StopThePedFunc.InjectPedItem(Suspect, "~r~Pistol");
                    Witness = new Ped(SpawnPoint, SpawnHeading);
                    Witness.MakeMissionPed();
                    WitnessCar = new Vehicle(CommonVariables.CarsToSelect.GetRandomElement(m=> m.IsValid, true),
                        Taxi.Position + Taxi.ForwardVector * 9f, Taxi.Heading);
                    Witness.WarpIntoVehicle(WitnessCar, -1);
                    if (Peralatan.Random.Next() % 5 == 0)
                    {
                        Ped witnPass = new Ped(SpawnPoint, SpawnHeading);
                        witnPass.MakeMissionPed();
                        witnPass.WarpIntoVehicle(WitnessCar, 0);
                        CalloutEntities.Add(witnPass);
                    }
                    GetClose();
                    if (!CalloutRunning) return;
                    TaxiDriver.Kill();
                    if (Blip) Blip.Delete();
                    Blip = TaxiDriver.AttachBlip();
                    Blip.Color = Color.Red;
                    WitnessBlip = Witness.AttachBlip();
                    WitnessBlip.Color = Color.Orange;
                    CalloutBlips.Add(WitnessBlip);
                    Game.DisplayHelp($"~s~Please move closer to the ~o~witness~s~, press {Peralatan.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.Y)}~s~ to speak with the witness");
                    Functions.RequestBackup(TaxiDriver.Position, EBackupResponseType.Code3, EBackupUnitType.Ambulance);
                    "Ambulance is en route to the scene".DisplayNotifWithLogo("~y~Taxi Passenger Refuse To Pay");                  
                    Game.DisplayHelp($"~y~Press {Peralatan.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.Y)}~y~ to speak with the witness");
                    Timer = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        if (Peralatan.CheckKey(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.Y))
                        {
                            if (PlayerPed.DistanceTo(Witness) < 5f && PlayerPed.IsOnFoot) break;
                            else Game.DisplayHelp("~s~Please move ~p~closer~s~ and ~o~leave~s~ from vehicle");
                        }
                        if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - Timer > 12000)
                        {
                            Timer = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                            Game.DisplayHelp($"~s~Please move closer to the ~o~witness~s~, Press {Peralatan.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.Y)}~s~ to speak with the witness");
                        }
                    }
                    if (!CalloutRunning) return;
                    var spawnPoint = SpawnManager.GetPedSpawnPoint(SpawnPoint, 250, 350);
                    if (spawnPoint.Position == Vector3.Zero) spawnPoint.Position = World.GetNextPositionOnStreet(SpawnPoint.Around(250, 650f));
                    Suspect.Position = spawnPoint;
                    Suspect.IsVisible = true;
                    Suspect.Tasks.Wander();
                    List<string> witnessFlow = new List<string>()
                     {
                        "Officer: Hello are you the caller?",
                        "Witness: Yes, i was wandering around and then this incident happen",
                        "Officer: Please describe what you saw recently",
                        $"Witness: I saw a {suspectManWoman} hit the taxi driver with a gun and then run away",
                        "Officer: Did you see where the suspect went?",
                        $"Witness: All i know he is running ~y~{Extension.GetCardinalDirection(Witness.GetHeadingTowards(Suspect), true)}~s~ leads to ~g~{Suspect.Position.GetZoneName()}",
                        "Officer: Did you know how many suspects were involved?",
                        $"Witness: I think there is {suspectCount}",
                        "Officer: Thank you for your information i will find the suspect",
                        "Witness: Okay, be careful officer"
                     };
                    Witness.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                    Time = DateTime.UtcNow;
                    Peralatan.HandleSpeech(witnessFlow, Witness);
                    Game.DisplayHelp("~y~Dispatch is trying to find ~o~suspect~y~ information, please wait...", 10000);
                    $"Conversation is took {(DateTime.UtcNow - Time).TotalSeconds} seconds".ToLog();
                    GameFiber.Wait(Peralatan.Random.Next(7500, 10000));
                    Manusia = new Manusia(Suspect, SuspectPersona);
                    Game.HideHelp();
                    Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS SUSPECT_LAST_SEEN IN_OR_ON_POSITION", Suspect.Position);
                    Suspect.DisplayNotificationsWithPedHeadshot("Passenger Details", $"~y~Name~s~: {SuspectPersona.FullName}~n~" +
                       $"~y~BirthDay~s~: {SuspectPersona.Birthday.ToShortDateString()}~n~~y~Last Seen~s~: {Suspect.Position.GetZoneName()}");
                    if (Blip) Blip.Delete();
                    if (WitnessBlip) WitnessBlip.Delete();
                    Blip = new Blip(Suspect.Position.Around(10f), 70f);
                    Blip.Color = Color.Yellow;
                    Blip.EnableRoute(Color.Yellow);
                    Time = DateTime.UtcNow + new TimeSpan(0, 0, 20);
                    Vector3 curPos = Suspect.Position;
                    GameFiber.StartNew(() =>
                    {
                        GameFiber.Wait(5000);
                        if (CalloutRunning) DisplayGPNotif();
                        GameFiber.Wait(120000);
                        if (CalloutRunning) DisplayGPNotif();
                    });
                    while (CalloutRunning)
                    {
                        if (PlayerPed.DistanceTo(Suspect) < 18f) break;
                        if (DateTime.UtcNow >= Time)
                        {
                            Time = DateTime.UtcNow + new TimeSpan(0, 0, 20);
                            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS SUSPECT_LAST_SEEN IN_OR_ON_POSITION", Suspect.Position);
                            Suspect.DisplayNotificationsWithPedHeadshot("Passenger Details", $"~y~Name~s~: {SuspectPersona.FullName}~n~" +
                               $"~y~BirthDay~s~: {SuspectPersona.Birthday.ToShortDateString()}~n~~y~Last Seen~s~: {Suspect.Position.GetZoneName()}");
                            if (Suspect.DistanceTo(curPos) > 80f)
                            {
                                if (Blip) Blip.Delete();
                                Blip = new Blip(Suspect.Position.Around(8f, 10f), 70f);
                                Blip.Color = Color.Yellow;
                                Blip.EnableRoute(Color.Yellow);
                                curPos = Suspect.Position;
                            }
                        }                       
                        GameFiber.Yield();
                    }
                    if (!CalloutRunning) return;
                    if (Peralatan.Random.Next(2150, 3500) % 8 == 0)
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
                    if (Blip) Blip.Delete();
                    Blip = Suspect.AttachBlip();
                    Blip.Color = Color.Red;
                    bool stealCar = false;
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        if (SuspectState != ESuspectStates.InAction) break;
                        if (PursuitCreated && !stealCar && Suspect.IsInAnyVehicle(false))
                        {
                            stealCar = true;
                            if (!Blip) Blip = Suspect.AttachBlip();
                            Blip.Color = Color.Red;
                            Blip.EnableRoute(Color.Red);
                            Functions.RequestBackup(Suspect.Position, EBackupResponseType.Pursuit, EBackupUnitType.NooseAirUnit);
                        }
                    }
                    DisplayCodeFourMessage();
                }
                catch (Exception e)
                {
                    "Taxi Refuse Pay callout crash".DisplayNotifWithLogo("Taxi Passenger Refuse To Pay");
                    e.ToString().ToLog();
                    NetExtension.SendError(e);
                    End();
                }
            });
        }
        private void SituationCommon()
        {
            CalloutMainFiber = GameFiber.StartNew(() =>
            {
                try
                {
                    CalloutRunning = true;
                    TaxiDriver.WarpIntoVehicle(Taxi, -1);
                    Suspect.Tasks.LeaveVehicle(Taxi, LeaveVehicleFlags.None);
                    if (StopThePedRunning)
                    {
                        var rand1 = Peralatan.Random.Next();
                        var rand2 = Peralatan.Random.Next(1, 1000);
                        if (rand1 % 2 == 0)
                        {
                            API.StopThePedFunc.SetPedAlcoholOverLimit(Suspect, true);
                        }
                        else if (rand2 < 50) API.StopThePedFunc.SetPedUnderDrugsInfluence(Suspect, true);
                        else API.StopThePedFunc.InjectPedDangerousItem(Suspect);
                    }
                    if (Peralatan.Random.Next() % 5 == 0) Suspect.SetPedAsWanted();
                    SuspectPersona = Functions.GetPersonaForPed(Suspect);
                    Manusia = new Manusia(Suspect, SuspectPersona);
                    Manusia.DisplayNotif();
                    DisplayGPNotif();
                    GetClose();
                    if (!CalloutRunning) return;
                    if (Blip) Blip.Delete();
                    Blip = Suspect.AttachBlip();
                    Blip.Color = Color.Yellow;
                    Blip.Scale = 0.75f;
                    Game.DisplayHelp($"Get closer with the driver, and press {Peralatan.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.Y)} to talk");
                    Time = DateTime.UtcNow + TimeSpan;
                    while (CalloutRunning)
                    {
                        if (DateTime.UtcNow > Time)
                        {
                            Time = DateTime.UtcNow + TimeSpan;
                            Game.DisplayHelp($"Get closer with the driver, Press {Peralatan.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.Y)} to talk");
                        }
                        if (Peralatan.CheckKey(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.Y))
                        {
                            if (!PlayerPed.IsInAnyVehicle(false))
                            {
                                if (PlayerPed.DistanceTo(TaxiDriver) < 6f) break;
                                else Game.DisplayHelp("Please move ~g~closer");
                            }
                            else Game.DisplayHelp("~y~Get out~s~ from vehicle first");
                        }
                        GameFiber.Yield();
                    }
                    if (!CalloutRunning) return;
                    TaxiDriver.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen).WaitForCompletion(4000);
                    if (TaxiDriver.IsInVehicle(Taxi, false)) TaxiDriver.Tasks.LeaveVehicle(LeaveVehicleFlags.WarpOut).WaitForCompletion(500);
                    if (Peralatan.Random.Next() % 4 == 0)
                    {
                        Suspect.Tasks.Flee(PlayerPed, 500f, -1);
                        GameFiber.Wait(2000);
                        Pursuit = Functions.CreatePursuit();
                        Functions.AddPedToPursuit(Pursuit, Suspect);
                        Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                        if (GrammarPoliceRunning) API.GrammarPoliceFunc.SetStatus(API.GrammarPoliceFunc.EGrammarPoliceStatusType.InPursuit);
                        PursuitCreated = true;
                        if (Blip) Blip.Color = Color.Red;
                        GameFiber.Wait(200);
                        Functions.RequestBackup(Suspect.Position, EBackupResponseType.Pursuit, EBackupUnitType.LocalUnit);
                        while (CalloutRunning)
                        {
                            GameFiber.Yield();
                            if (SuspectState != ESuspectStates.InAction) break;
                        }
                        DisplayCodeFourMessage();
                        CalloutRunning = false;
                    }
                    else Peralatan.HandleSpeech(commond.GetRandomElement(), TaxiDriver, Suspect);
                    if (!CalloutRunning) return;
                    Game.DisplayHelp("~y~Handle the situation as you see fit, you can do some investigation ~g~(e.g. ped check, frisk, or ask some question)~s~~n~" +
                        $"To end the callout, press and ~y~hold~s~ {Peralatan.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.End)}");
                    DisplayGPNotif();
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        if (Peralatan.CheckKey(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.End))
                        {
                            GameFiber.Sleep(320);
                            if (Game.IsKeyDownRightNow(System.Windows.Forms.Keys.End))
                            {
                                GameFiber.Sleep(1600);
                                if (Game.IsKeyDownRightNow(System.Windows.Forms.Keys.End)) break;
                            }
                            else Game.DisplayHelp($"To end the callout, press and ~y~hold~s~ {Peralatan.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.End)}");
                        }
                        if (SuspectState != ESuspectStates.InAction) break;
                    }
                    DisplayCodeFourMessage();
                }
                catch (Exception e)
                {
                    "Taxi Refuse Pay callout crash".DisplayNotifWithLogo("Taxi Passenger Refuse To Pay");
                    e.ToString().ToLog();
                    NetExtension.SendError(e);
                    End();
                }
            });
        }
        private void SituationNasty()
        {
            CalloutMainFiber = GameFiber.StartNew(() =>
            {
                try
                {

                }
                catch (Exception e)
                {
                    "Taxi Refuse Pay callout crash".DisplayNotifWithLogo("Taxi Passenger Refuse To Pay");
                    e.ToString().ToLog();
                    NetExtension.SendError(e);
                    End();
                }
            });
        }
        private static readonly List<List<string>> commond = new List<List<string>>()
        {
            new List<string>()
            {
                "Victim: Officer, this person right here is refusing to pay me",
                "Suspect: im sorry, i left my wallet at the home earlier, if the driver want to take me back i will i'll pay him double",
                "Victim: No no no!, i can't do that, my working hours are up, i'm tired",
                "Suspect: See officer, i have offered him a solution, but he refused",
                "Victim: Hey, i also need time with my family, if i take you there, my family will be mad at me",
                "Officer: Okey citizen, calm down, lets solve this peacefully",
            },
            new List<string>()
            {
                "Victim: Officer, this person pissed me off, he refuses to pay his bills to me",
                "Suspect: Officer, dont listen to him, he was driving recklessly and when i told him to drive slower, he dont listen",
                "Victim: Thats not a reason for you to not paying me",
                "Suspect: You know, what you did earlier made me worry on the trip, what kind of service is that",
                "Victim: all this time i was driving like that, only you who feel uncomfortable, whats wrong with you",
                "Officer: Enough, lets talk peacefully no need to angry"
            },
            new List<string>()
            {
                "Victim: Officer, i have some garbage here, this person is refuse to pay me",
                "Suspect: What did you say?",
                "Suspect: Officer, listen to me! This driver is always angry with anger during the trip, i feel so annoyed, how could I pay a person who bothered me",
                "Victim: Oh come on, seriously!, you are the one who angry, look at your red face",
                "Suspect: F*ck You",
                "Officer: Hey hey hey, calm down citizens, problems cannot be solved with anger, so calm down and lets talk peacefully",
            }
        };
    }
}
