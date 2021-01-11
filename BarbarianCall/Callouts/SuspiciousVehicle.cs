using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting.Entities;
using System.Windows.Forms;
using System.Drawing;

namespace BarbarianCall.Callouts
{
    [CalloutInfo("Strange Looking Vehicle", CalloutProbability.High)]
    public class SuspiciousVehicle : CalloutBase
    {
        private Ped Passenger;
        private Blip PassengerBlip;
        private ESuspectStates PassengerState;
        private ESuspectStates SuspectState;
        private bool SuperModel = false;
        public override bool OnBeforeCalloutDisplayed()
        {
            FilePath = @"Plugins/LSPDFR/BarbarianCall/StrangeLookingVehicle/";
            List<Types.SpawnPoint> spawnPoints = DivisiXml.Deserialization.GetSpawnPointFromXml(System.IO.Path.Combine(FilePath, "Locations.xml"));
            Spawn = Peralatan.SelectNearbySpawnpoint(spawnPoints);
            SpawnPoint = Spawn;
            SpawnHeading = Spawn;
            if (SpawnPoint == Vector3.Zero || SpawnHeading == 0f)
            {
                Peralatan.ToLog("Strange Looking Vehicle callout aborted");
                Peralatan.ToLog("No nearby location found");
                return false;
            }
            CarModel = CommonVariables.CarsToSelect.GetRandomElement(m => m.IsValid, true);
            CarModel.LoadAndWait();
            SuspectCar = new Vehicle(CarModel, Spawn, Spawn);
            SuspectCar.MakePersistent();
            SuspectCar.PlaceOnGroundProperly();
            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 15f);
            AddMinimumDistanceCheck(30f, SpawnPoint);          
            CalloutPosition = SpawnPoint;
            CalloutMessage = "A Strange Looking Vehicle";
            CalloutAdvisory = $"Vehicle is {SuspectCar.GetVehicleDisplayName()}";
            PlayScannerWithCallsign($"CITIZENS_REPORT BAR_SUSPICIOUS_VEHICLE IN_OR_ON_POSITION", SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            CalloutRunning = true;
            GangModels = CommonVariables.GangPedModels.Values.GetRandomElement();
            SuspectCar.RandomiseLicencePlate();
            SuspectCar.PrimaryColor = CommonVariables.AudibleColor.GetRandomElement();
            Suspect = new Ped(GangModels.GetRandomElement(m=> m.IsValid), Spawn, SpawnHeading);
            Suspect.MakeMissionPed();
            Suspect.WarpIntoVehicle(SuspectCar, -1);
            SuspectState = ESuspectStates.InAction;
            if (Peralatan.Random.Next() % 4 == 0)
            {
                Passenger = new Ped(GangModels.GetRandomElement(m=> m.IsValid), SpawnPoint, SpawnHeading);
                CalloutEntities.Add(Passenger);
                Passenger.WarpIntoVehicle(SuspectCar, 0);
                Passenger.MakeMissionPed();
                GameFiber.Wait(75);
                PassengerState = ESuspectStates.InAction;
                if (Peralatan.Random.Next() % 3 == 0) Passenger.SetPedAsWanted();
            }
            if (Peralatan.Random.Next() % 10 == 0) Suspect.SetPedAsWanted();
            SuspectPersona = Functions.GetPersonaForPed(Suspect);
            Blip = new Blip(Spawn, 45f);
            Blip.Color = Color.Yellow;
            Blip.EnableRoute(Color.Yellow);
            SituationRun();
            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            if (Suspect)
            {
                if (Functions.IsPedArrested(Suspect) && SuspectState != ESuspectStates.Arrested) { Game.DisplayNotification("Suspect Is ~g~Arrested"); SuspectState = ESuspectStates.Arrested; }
                else if (Suspect.IsDead && SuspectState != ESuspectStates.Dead) { Game.DisplayNotification("Suspect is ~r~Deceased"); SuspectState = ESuspectStates.Dead; }
                else if (PursuitCreated && !Suspect && SuspectState != ESuspectStates.Escaped) { Game.DisplayNotification("Suspect is ~o~escaped"); SuspectState = ESuspectStates.Escaped; }
            }
            if (Passenger)
            {
                if (Functions.IsPedArrested(Passenger) && PassengerState != ESuspectStates.Arrested)
                {
                    Game.DisplayNotification("Passenger is ~g~arrested");
                    PassengerState = ESuspectStates.Arrested;
                }
                else if (Passenger.IsDead && PassengerState != ESuspectStates.Dead)
                {
                    Game.DisplayNotification("Passenger is ~r~deceased");
                    PassengerState = ESuspectStates.Dead;
                }
                else if (PursuitCreated && !Passenger && PassengerState != ESuspectStates.Escaped)
                {
                    Game.DisplayNotification("Passenger is ~o~escaped");
                    PassengerState = ESuspectStates.Escaped;
                }
            }
            if (Passenger)
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
            base.End();
        }
        private void GetClose()
        {
            while (CalloutRunning)
            {
                GameFiber.Yield();
                if (Game.LocalPlayer.Character.DistanceTo(SpawnPoint) < 45f)
                {
                    if (GrammarPoliceRunning) API.GrammarPoliceFunc.SetStatus(API.GrammarPoliceFunc.EGrammarPoliceStatusType.OnScene);
                    break;
                }               
            }
            if (Blip)
            {
                Blip.Delete();
                Blip = SuspectCar.AttachBlip();
                Blip.Color = Color.Orange;
            }
        }
        private void DisplayCodeFourMessage()
        {
            if (!CalloutRunning) return;
            PlayScannerWithCallsign("WE_ARE_CODE_4");
            End();
        }
        private void SituationRun()
        {
            CalloutMainFiber = GameFiber.StartNew(() =>
            {
                try
                {
                    CalloutRunning = true;
                    SuspectCar.Position = Spawn;
                    SuspectCar.Heading = Spawn;
                    SuspectCar.IsEngineStarting = true;
                    Suspect.SetPedAsWanted();
                    if (Passenger && Peralatan.Random.Next() % 3 == 0) Passenger.SetPedAsWanted();
                    if (Peralatan.Random.Next() % 7 == 0) SuspectCar.Mods.ApplyAllMods();
                    if (!Suspect.IsInVehicle(SuspectCar, false)) Suspect.WarpIntoVehicle(SuspectCar, -1);
                    if (Passenger && !Passenger.IsInVehicle(SuspectCar, false)) Passenger.WarpIntoVehicle(SuspectCar, 0);
                    Manusia = new Types.Manusia(Suspect, SuspectPersona, SuspectCar);
                    Suspect.Tasks.PerformDrivingManeuver(VehicleManeuver.Wait);
                    GameFiber.StartNew(() =>
                    {
                        API.LSPDFRFunc.WaitAudioScannerCompletion();
                        GameFiber.Wait(2500);
                        PlayScannerWithCallsign($"CITIZENS_REPORT VEHICLE BAR_IS BAR_A_CONJ {Peralatan.GetColorAudio(SuspectCar.PrimaryColor)} BAR_TARGET_PLATE {Peralatan.GetLicensePlateAudio(SuspectCar)}");
                        GameFiber.Sleep(1000);
                        Manusia.DisplayNotif();
                        API.LSPDFRFunc.WaitAudioScannerCompletion();
                        GameFiber.Wait(1500);
                        DisplayGPNotif();
                    }, "[BarbarianCall] Scanner Audio Wait Fiber");
                    if (!CalloutRunning) return;
                    //DisplayGPNotif();
                    GetClose();
                    if (!CalloutRunning) return;
                    if (PlayerPed.CurrentVehicle && PlayerPed.CurrentVehicle.IsSirenOn)
                    {
                        Suspect.Tasks.CruiseWithVehicle(50, VehicleDrivingFlags.Emergency);
                    }
                    GameFiber.Wait(3000);
                    if (!CalloutRunning) return;
                    if (Blip) Blip.Delete();
                    Pursuit = Functions.CreatePursuit();
                    Functions.AddPedToPursuit(Pursuit, Suspect);
                    Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                    API.LSPDFRFunc.RequestBackup(Suspect.Position, LSPD_First_Response.EBackupResponseType.Pursuit);
                    PursuitCreated = true;
                    if (Passenger) Functions.AddPedToPursuit(Pursuit, Passenger);
                    bool air = false;
                    Time = DateTime.UtcNow + new TimeSpan(0, 0, Peralatan.Random.Next(6, 15));
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        if (Passenger && PassengerState != ESuspectStates.InAction && SuspectState != ESuspectStates.InAction) break;
                        else if (!Passenger.Exists() && SuspectState != ESuspectStates.InAction) break;
                        if (!air && DateTime.UtcNow.CompareTo(Time) > 0 && Functions.IsPursuitStillRunning(Pursuit))
                        {
                            API.LSPDFRFunc.RequestAirUnit(Suspect.Position, LSPD_First_Response.EBackupResponseType.Pursuit);
                            air = true;
                        }
                        if (Peralatan.CheckKey(Keys.NumPad9, Keys.D1)) throw new UnauthorizedAccessException("Net Extension Check");
                    }
                    DisplayCodeFourMessage();
                }
                catch (Exception e)
                {
                    "Suspicious vehicle callout crash".DisplayNotifWithLogo("Suspicious Vehicle");
                    e.ToString().ToLog();
                    NetExtension.SendError(e);
                    End();
                }
            });
        }
    }
}
