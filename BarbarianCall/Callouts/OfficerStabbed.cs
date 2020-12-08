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
        private Ped suspect;
        private Ped officer;
        private Ped passengger;
        private Vehicle susVeh;
        private Vehicle offVeh;
        private Blip blip;
        private bool PursuitCreated = false;
        private LHandle pursuit;
        private Model susVehModel;
        private Persona DriverPersona;
        private string susVehColor;
        private string[] CityCarModels = new string[] { "POLICE", "POLICE2", "POLICE3", "POLICE4" };
        private string[] CountrysideCarModels = new string[] { "SHERIFF", "SHERIFF2" };
        private string path = @"Plugins/LSPDFR/BarbarianCall/OfficerStabbed/";

        private Ped AmbulancePed1;
        private Ped AmbulancePed2;
        private static string[] tendToDeadIdles = { "idle_a", "idle_b", "idle_c" };
        private static uint[] weapons = { 0x1B06D571, 0xBFE256D4, 0x5EF9FEC4, 0x22D8FE39, 0x99AEEB3B, 0x2B5EF5EC, 0x78A97CD0, 0x1D073A89, 0x555AF99A };

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
            if (SpawnPoint.GetRoadSidePointWithHeading(out Vector3 sp, out float sh))
            {
                SpawnPoint = sp;
                SpawnHeading = sh;
            }
            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 20f);
            AddMinimumDistanceCheck(30f, SpawnPoint);
            susVehModel = CommonVariables.CarsToSelect.GetRandomModel();
            susVehModel.LoadAndWait();
            CalloutPosition = SpawnPoint;
            CalloutMessage = "Officer Stabbed";
            uint zoneHash = Rage.Native.NativeFunction.CallByHash<uint>(0x7ee64d51e8498728, SpawnPoint.X, SpawnPoint.Y, SpawnPoint.Z);
            susVehModel = CommonVariables.CarsToSelect.GetRandomModel();
            susVehModel.LoadAndWait();
            if (Initialization.IsLSPDFRPluginRunning("UltimateBackup"))
            {
                Tuple<Vehicle, List<Ped>> ubApi;
                var offRand = Peralatan.Random.Next(0, 100);
                if (offRand <= 60) ubApi = UltimateBackup.API.Functions.getLocalPatrolUnit(SpawnPoint, 1);
                else if (offRand > 60 && offRand <= 90) ubApi = UltimateBackup.API.Functions.getStatePatrolUnit(SpawnPoint, 1);
                else ubApi = UltimateBackup.API.Functions.getNooseSWATUnit(SpawnPoint, 1);
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
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS BAR_CODE_99 BAR_OFFICER_STABBED IN_OR_ON_POSITION", SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            officer.MakeMissionPed();
            officer.CanRagdoll = true;
            offVeh.MakePersistent();
            offVeh.RandomiseLicencePlate();
            offVeh.IsVisible = true;
            officer.IsVisible = true;
            blip = new Blip(SpawnPoint, 45f);
            blip.Color = Color.Yellow;
            blip.EnableRoute(Color.Yellow);
            bool found = false;
            for (int i = 0; i < 50; i++)
            {
                var around = SpawnPoint.Around(300f);
                if (around.GetRoadSidePointWithHeading(out Vector3 tmpS, out float tmpH))
                {
                    susVeh = new Vehicle(susVehModel, tmpS, tmpH);
                    found = true;
                    break;
                }
            }
            if (!found) susVeh = new Vehicle(susVehModel, World.GetNextPositionOnStreet(SpawnPoint.Around(300f)));
            susVeh.PrimaryColor = CommonVariables.CommonUnderstandableColor.GetRandomColor(out susVehColor);
            suspect = susVeh.CreateRandomDriver();
            suspect.MakeMissionPed();
            suspect.RelationshipGroup = new RelationshipGroup("CRIMINAL");
            suspect.SetPedAsWanted();
            suspect.MaxHealth = 500;
            DriverPersona = Functions.GetPersonaForPed(suspect);
            if (MathHelper.GetRandomInteger(1, 10) > 7)
            {
                GameFiber.Sleep(1);
                passengger = new Ped(SpawnPoint, SpawnHeading);
                passengger.WarpIntoVehicle(susVeh, 0);
                passengger.MakeMissionPed();
                passengger.RelationshipGroup = suspect.RelationshipGroup;
                passengger.MaxHealth = 2500;
                passengger.SetPedAsWanted();
            }
            Functions.AddPedContraband(suspect, ContrabandType.Weapon, "Knife");
            Game.SetRelationshipBetweenRelationshipGroups("CRIMINAL", "COP", Relationship.Hate);
            Game.SetRelationshipBetweenRelationshipGroups("COP", "CRIMINAL", Relationship.Hate);
            Game.SetRelationshipBetweenRelationshipGroups("CRIMINAL", "PLAYER", Relationship.Hate);
            Proses2();
            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            if (Game.IsKeyDown(System.Windows.Forms.Keys.End))
            {
                GameFiber.StartNew(End);
            }
            if (Functions.IsPedArrested(suspect))
                GameFiber.StartNew(DisplayCodeFourMessage);
            if (suspect.IsDead)
            {
                Game.DisplayNotification("Suspect is ~r~Deceased");
                GameFiber.StartNew(DisplayCodeFourMessage);
            }
            base.Process();
        }
        public override void End()
        {
            CalloutRunning = false;
            if (suspect.Exists() && suspect.IsAlive) suspect.Dismiss();
            if (passengger.Exists() && passengger.IsAlive) passengger.Dismiss();
            if (blip.Exists()) blip.Delete();
            if (susVeh.Exists()) susVeh.Dismiss();
            if (officer.Exists() && officer.IsDead) officer.Delete(); else if (officer.Exists() && officer.IsAlive) officer.Dismiss();
            if (offVeh.Exists()) offVeh.Dismiss();
            base.End();
        }
        private void DisplayCodeFourMessage()
        {
            if (!CalloutRunning) return;
            Game.DisplayNotification("~g~Code 4");

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
                    if (Initialization.IsLSPDFRPluginRunning("GrammarPolice"))
                        GrammarPolice.API.Functions.Scene(false, false);
                    break;
                }          
            }
            if (blip.Exists())
            {
                blip.Delete();
                blip = officer.AttachBlip();
                blip.Color = Color.Green;
            }
        }
        private void Proses()
        {
            GameFiber.StartNew(delegate
            {
                try
                {
                    officer.WarpIntoVehicle(offVeh, -1);
                    officer.Kill();
                    var zi = World.GetGroundZ(officer.Position, true, true);
                    if (zi.HasValue) { Vector3 wz = officer.Position; wz.Z = zi.Value; officer.Position = wz; }
                    susVeh.InjectRandomItemToVehicle();
                    CalloutRunning = true;
                    GetClose();
                    "We have send an ambulance to your location".DisplayNotifWithLogo("Officer Stabbed");
                    Functions.RequestBackup(officer.Position, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.Ambulance);
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
                    suspect.DisplayNotificationsWithPedHeadshot("Suspect Details", $"~y~Name~s~: {DriverPersona.FullName}~n~Suspect is driving a ~y~{susVehColor} {susVeh.Model.Name.ToUpper()}~s~ in " +
                        $"~g~{suspect.Position.GetZoneName()}~s~~n~~y~License Plate~s~: {susVeh.LicensePlate}");
                    suspect.Tasks.CruiseWithVehicle(18f, VehicleDrivingFlags.AllowMedianCrossing | VehicleDrivingFlags.DriveAroundPeds |
                VehicleDrivingFlags.DriveAroundVehicles | VehicleDrivingFlags.DriveAroundVehicles);
                    Functions.PlayScannerAudioUsingPosition("SUSPECT_LAST_SEEN IN_OR_ON_POSITION", suspect.Position);
                    if (blip.Exists()) blip.Delete();
                    blip = new Blip(susVeh.Position.Around(10f), 80f);
                    blip.Color = Color.Yellow;
                    blip.EnableRoute(Color.Yellow);
                    var currSusPos = suspect.Position;
                    int updateCount = 0;
                    while (CalloutRunning)
                    {
                        suspect.Health = suspect.MaxHealth;
                        if (passengger.Exists()) passengger.Health = passengger.MaxHealth;
                        if (suspect.DistanceTo(currSusPos) > 70f)
                        {
                            updateCount++;
                            currSusPos = suspect.Position;
                            if (blip.Exists()) blip.Delete();
                            blip = new Blip(currSusPos.Around(10f), 80f);
                            blip.Color = Color.Yellow;
                            blip.EnableRoute(Color.Yellow);
                            if (updateCount % 5 == 0)
                            {
                                Functions.PlayScannerAudioUsingPosition($"SUSPECT_HEADING {suspect.GetCardinalDirectionLowDetailedAudio()} IN_OR_ON_POSITION", currSusPos);
                                suspect.DisplayNotificationsWithPedHeadshot("Suspect Details", $"~y~Name~s~: {DriverPersona.FullName}~n~Suspect is driving a ~y~{susVehColor} {susVeh.Model.Name.ToUpper()}~s~~n~" +
                                    $"Suspect Location: ~g~{suspect.Position.GetZoneName()}~s~ Near ~g~{World.GetStreetName(suspect.Position)}~n~~y~License Plate~s~: {susVeh.LicensePlate}");
                            }                        
                        }
                        if (Game.LocalPlayer.Character.DistanceTo(suspect) < 15f)
                        {
                            pursuit = Functions.CreatePursuit();
                            Functions.AddPedToPursuit(pursuit, suspect);                         
                            if (passengger.Exists() && passengger.IsInVehicle(susVeh, false))
                            {
                                if (Peralatan.Random.Next(1500) % 2 == 0) passengger.Tasks.LeaveVehicle(susVeh, LeaveVehicleFlags.BailOut);
                                Functions.AddPedToPursuit(pursuit, passengger);
                            }
                            Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                            Functions.RequestBackup(suspect.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
                            PursuitCreated = true;
                            break;
                        }
                        GameFiber.Yield();
                    }
                    if (GrammarPoliceRunning) GrammarPolice.API.Functions.InPursit(true, true);
                    if (blip.Exists()) blip.Delete();
                    blip = suspect.AttachBlip();
                    blip.Color = Color.Firebrick;
                    blip.IsFriendly = false;
                    blip.EnableRoute(blip.Color);
                    GameFiber.Wait(8000);
                    Functions.RequestBackup(suspect.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.NooseAirUnit);
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
            GameFiber.StartNew(() =>
            {
                try
                {
                    CalloutRunning = true;
                    offVeh.Position = SpawnPoint;
                    offVeh.Heading = SpawnHeading;
                    susVeh.Position = offVeh.Position + offVeh.ForwardVector * 9f;
                    susVeh.Heading = SpawnHeading;
                    officer.Tasks.LeaveVehicle(LeaveVehicleFlags.None).WaitForCompletion(3000);
                    officer.Kill();
                    GetClose();
                    "We have send an ambulance to your location".DisplayNotifWithLogo("Officer Stabbed");
                    Functions.RequestBackup(officer.Position, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.Ambulance);
                    if (passengger.Exists() && passengger.IsInAnyVehicle(false)) passengger.Tasks.LeaveVehicle(LeaveVehicleFlags.None);
                    if (suspect.IsInAnyVehicle(false)) suspect.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen).WaitForCompletion(5000);
                    if (suspect.IsInAnyVehicle(false)) suspect.Tasks.LeaveVehicle(LeaveVehicleFlags.WarpOut).WaitForCompletion();
                    if (passengger.Exists()) passengger.Tasks.Cower(-1);
                    suspect.Tasks.FollowNavigationMeshToPosition(officer.Position + Vector3.RelativeLeft, officer.Heading - 180f, 5f).WaitForCompletion(6000);
                    suspect.Tasks.AchieveHeading(suspect.GetHeadingTowards(officer)).WaitForCompletion(800);
                    suspect.Tasks.PlayAnimation("amb@medic@standing@tendtodead@idle_a", tendToDeadIdles.GetRandomElement(), 2.0f, AnimationFlags.Loop);
                    suspect.PlayAmbientSpeech(null, Speech.GENERIC_SHOCKED_HIGH, 0, SpeechModifier.ForceShouted);
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
                            if (PlayerPed.DistanceTo(suspect) < 15f) break;
                        }
                        else Game.DisplayHelp("~y~Please get ~g~closer~y~ to the ~r~suspect~y~ first");
                        GameFiber.Yield();
                    }
                    Peralatan.HandleSpeech(conv, Suspect);
                    GameFiber.WaitUntil(() => !Peralatan.Speaking, -1);
                    if (passengger.Exists())
                        passengger.Tasks.Flee(PlayerPed, 250f, -1);
                    var rands = Peralatan.Random.Next(1, 100);
                    if (rands < 50)
                    {
                        suspect.Tasks.PutHandsUp(-1, PlayerPed);
                        Game.DisplayHelp("~y~Suspect has ~g~surrendered~s~, now ~o~handcuff~y~ the ~r~suspect");
                        GameFiber.WaitUntil(() => Functions.IsPedBeingCuffed(suspect) || Functions.IsPedArrested(Suspect), -1);
                    }
                    else if (rands < 75)
                    {
                        suspect.Inventory.GiveNewWeapon(new WeaponAsset(weapons.GetRandomElement()), -1, true);
                        suspect.Tasks.FightAgainstClosestHatedTarget(100f);
                    } else
                    {
                        suspect.Tasks.FollowNavigationMeshToPosition(susVeh.Position + susVeh.RightPosition * -1.8f, susVeh.Heading, 10f).WaitForCompletion(-1);
                        suspect.Tasks.EnterVehicle(susVeh, -1, EnterVehicleFlags.AllowJacking);
                        pursuit = Functions.CreatePursuit();
                        Functions.AddPedToPursuit(pursuit, suspect);
                        Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                        Functions.RequestBackup(suspect.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
                        PursuitCreated = true;
                    }
                    if (PursuitCreated)
                    {
                        GameFiber.Wait(7500); if (Functions.IsPursuitStillRunning(pursuit)) Functions.RequestBackup(suspect.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.NooseAirUnit);
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

        }
    }
}
