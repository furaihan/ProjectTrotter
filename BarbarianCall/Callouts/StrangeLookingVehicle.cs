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

namespace BarbarianCall.Callouts
{
    [CalloutInfo("Strange Looking Vehicle", CalloutProbability.High)]
    public class StrangeLookingVehicle : Callout
    {
        private Ped driver;
        private Ped passenger;
        private Vehicle susVeh;
        private Vector3 SpawnPoint;
        private float SpawnHeading;
        private LHandle pursuit;
        private bool pursuitCreated = false;
        private Blip blip;
        private bool CalloutRunning = false;
        private CalloutState state;
        private string vehColor;
        private Model vehModel;
        private Persona driverPersona;
        private Persona passengerPersona;

        public override bool OnBeforeCalloutDisplayed()
        {
            var path = @"Plugins/LSPDFR/BarbarianCall/StrangeLookingVehicle/";
            DivisiXml.Deserialization.GetDataFromXml(path + "Locations.xml", out List<Vector3> locationToSelect, out List<float> headingToSelect);
            Peralatan.SelectNearbyLocationsWithHeading(locationToSelect, headingToSelect, out SpawnPoint, out SpawnHeading);
            if (SpawnPoint == Vector3.Zero || SpawnHeading == 0f)
            {
                Peralatan.ToLog("Strange Looking Vehicle callout aborted");
                Peralatan.ToLog("No nearby location found");
                return false;
            }
            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 15f);
            AddMinimumDistanceCheck(30f, SpawnPoint);           
            vehModel = CommonVariables.CarsToSelect.GetRandomModel();
            vehModel.LoadAndWait();
            CalloutPosition = SpawnPoint;
            CalloutMessage = "A Strange Looking Vehicle";
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS CITIZENS_REPORT BAR_SUSPICIOUS_VEHICLE IN_OR_ON_POSITION", SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            foreach (Vehicle v in SpawnPoint.GetNearbyVehicleByRadius(10f))
            {
                if (v.Exists() && !v.CreatedByTheCallingPlugin)
                {
                    $"Deleting {v.Model.Name} because too close with callout scene".ToLog();
                    v.Delete();
                }
            }
            $"One of the citizens in ~g~{SpawnPoint.GetZoneName()}~s~ reported that they saw a vehicle that seemed ~y~suspicious~s~, go there and check the vehicle. ~g~please be careful".
                DisplayNotifWithLogo("Strange Looking Vehicle");
            state = CalloutState.EnRoute;
            susVeh = new Vehicle(vehModel, SpawnPoint, SpawnHeading);
            susVeh.PrimaryColor = CommonVariables.CommonUnderstandableColor.GetRandomColor(out vehColor);
            susVeh.SecondaryColor = susVeh.PrimaryColor;
            susVeh.RandomiseLicencePlate();
            susVeh.MakePersistent();
            driver = susVeh.CreateRandomDriver();
            driver.MakeMissionPed();
            driverPersona = Functions.GetPersonaForPed(driver);
            if (MathHelper.GetRandomInteger(1, 100) <= 30)
            {
                passenger = new Ped(SpawnPoint);
                passenger.WarpIntoVehicle(susVeh, 0);
                passenger.MakeMissionPed();
                Functions.GetPersonaForPed(passenger);
            }
            blip = new Blip(SpawnPoint, 45f);
            blip.Color = System.Drawing.Color.Yellow;
            blip.EnableRoute(System.Drawing.Color.Yellow);
            GameFiber.Sleep(2000);
            Functions.PlayScannerAudio("UNITS_RESPOND_CODE_2");
            if (!CalloutRunning)
            {
                int scenario = MathHelper.GetRandomInteger(1, 2500);
                if (scenario % 2 == 0)
                {
                    Kosongan();
                }
                else
                {
                    Ngantuk();
                }
            }
            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
           if (Game.IsKeyDown(Keys.End) || Game.LocalPlayer.Character.IsDead)
            {
                GameFiber.StartNew(DisplayCodeFourMessage);
            }
           if (passenger.Exists())
            {
                if (Functions.IsPedArrested(passenger))
                {
                    Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS SUSPECT_IN_CUSTODY", passenger.Position);
                }
            }
            base.Process();
        }
        public override void End()
        {
            if (driver.Exists() && driver.IsAlive) driver.Dismiss();
            if (passenger.Exists()) passenger.Dismiss();
            if (blip.Exists()) blip.Delete();
            if (susVeh.Exists()) susVeh.Dismiss();
            if (Initialization.IsLSPDFRPluginRunning("GrammarPolice", new Version("1.3.1.0")))
                GrammarPolice.API.Functions.Available(false, false);
            CalloutRunning = false;
            Peralatan.Speaking = false;
            base.End();
        }
        private void Kosongan()
        {
            CalloutRunning = true;
            GameFiber.StartNew(delegate
            {
                GameFiber.Sleep(2000);
                Functions.PlayScannerAudio($"ATTENTION_ALL_UNITS CITIZENS_REPORT SUSPECT_IS DRIVING_A COLOR_{vehColor} {susVeh.Model.Name.ToUpper()} BAR_TARGET_PLATE "
                    + Peralatan.GetLicensePlateAudio(susVeh.LicensePlate) + " BAR_PROCEED_WITH_CAUTION");
                bool njeblug = false;
                int nearbyPedCount;
                GetClose();
                if (passenger.Exists()) passenger.Delete();
                driver.Delete();
                nearbyPedCount = susVeh.Position.GetNearbyPedByRadius(15f).Count();
                $"Nearby ped: {nearbyPedCount}".ToLog();
                if (nearbyPedCount > 15) njeblug = true;
                while (CalloutRunning)
                {
                    GameFiber.Yield();
                    if (Game.LocalPlayer.Character.Position.DistanceTo(susVeh.Position) < 10f) break;
                }
                if (njeblug)
                {
                    susVeh.Explode();
                    GameFiber.Sleep(5000);
                    DisplayCodeFourMessage();
                }
                else
                {
                    "Vehicle is unoccupied, please investigate the vehicle".DisplayNotifWithLogo("Strange Looking Vehicle");
                    if (MathHelper.GetRandomInteger(1, 10) < 4) susVeh.IsStolen = true;
                    if (Initialization.IsLSPDFRPluginRunning("StopThePed"))
                    {
                        susVeh.Metadata.searchDriver = "~r~ an automatic rifle";
                        if (MathHelper.GetRandomInteger(1, 100) < 30 )
                        {
                            susVeh.IsStolen = false;
                            StopThePed.API.Functions.setVehicleRegistrationStatus(susVeh, StopThePed.API.STPVehicleStatus.None);
                        }
                    }
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        Game.DisplayHelp($"Press {Peralatan.FormatKeyBinding(Keys.None, Keys.End)} to end the callout", 2500);
                        GameFiber.Sleep(12000);
                        if (Initialization.IsLSPDFRPluginRunning("StopThePed"))
                        {
                            StopThePed.API.Events.searchVehicleEvent += (Vehicle veh) =>
                            {
                                if (veh = susVeh)
                                {
                                    "if you found something suspicious, you can tow this vehicle to police station for further investigation".DisplayNotifWithLogo("Strange Looking Vehicle");
                                }
                            };
                        }
                        if (susVeh.Position.DistanceTo(SpawnPoint) > 45f && susVeh.Driver != Game.LocalPlayer.Character)
                        {
                            DisplayCodeFourMessage();
                            break;
                        }
                    }
                }
            });
        }
        private void Ngantuk()
        {
            CalloutRunning = true;
            GameFiber.StartNew(delegate
            {
                GameFiber.Sleep(2000);
                Functions.PlayScannerAudio($"ATTENTION_ALL_UNITS CITIZENS_REPORT SUSPECT_IS DRIVING_A COLOR_{vehColor} {susVeh.Model.Name.ToUpper()} BAR_TARGET_PLATE "
                    + Peralatan.GetLicensePlateAudio(susVeh.LicensePlate) + " BAR_PROCEED_WITH_CAUTION");
                GetClose();
                while (CalloutRunning)
                {
                    GameFiber.Yield();
                    if (Game.LocalPlayer.Character.Position.DistanceTo(susVeh.Position) < 10f) break;
                }
                Game.HideHelp();
                while (CalloutRunning)
                {
                    GameFiber.Yield();
                    if (Game.LocalPlayer.Character.Position.DistanceTo(driver.Position) < 3.3f)
                    {
                        Game.DisplayHelp($"Press {Peralatan.FormatKeyBinding(Keys.None, Keys.Y)} to talk with the driver");
                        if (Game.IsKeyDown(Keys.Y)) 
                        {
                            if (driver.Exists() && driver.IsAlive) driver.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen).WaitForCompletion(2000);
                            if (passenger.Exists() && passenger.IsAlive)
                            {
                                passenger.Tasks.LeaveVehicle(LeaveVehicleFlags.None).WaitForCompletion(2000);
                                var random1 = MathHelper.GetRandomInteger(1, 10);
                                GameFiber.Sleep(1);
                                if (random1 < 4)
                                {
                                    if (Initialization.IsLSPDFRPluginRunning("StopThePed")) StopThePed.API.Functions.setPedUnderDrugsInfluence(passenger, true);
                                    passengerPersona.Wanted = true;
                                    var random2 = MathHelper.GetRandomInteger(1, 10);
                                    if (random2 < 3)
                                    {
                                        pursuit = Functions.CreatePursuit();
                                        Functions.AddPedToPursuit(pursuit, passenger);
                                        Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                                        pursuitCreated = true;
                                        Functions.RequestBackup(passenger.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
                                        Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_RESIST_ARREST IN_OR_ON_POSITION", passenger.Position);
                                    }                                   
                                }
                            }                                
                            break;
                        }
                    }
                    else Game.DisplayHelp($"Move closer to the driver");
                }
                List<string> cakap = new List<string>
                {
                    "~b~Officer~s~: Hello, what are you doing here?",
                    "~y~Suspect~s~: Oh my god, I was surprised by your presence here, sir",
                    "~b~Officer~s~: Just answer my question, what are you doing here?",
                    "~y~Suspect~s~: I was sleepy while on the way sir, I decided to stop here for a moment to release my sleepiness",
                    "~b~Officer~s~: You know what, your vehicle here has been reported as a suspicious vehicle",
                    "~y~Suspect~s~: I don't intend to do anything sir, I just want to sleep",
                    "~y~[CONVERSATION IS OVER]"
                };
                Game.LocalPlayer.Character.PlayAmbientSpeech("Generic_Hi");
                if (!pursuitCreated) Peralatan.HandleSpeech(cakap, driver);
            });
        }
        private void GetClose()
        {
            int counter = 0;
            while (CalloutRunning)
            {
                GameFiber.Yield();
                counter++;
                if (counter % 600 == 0) Game.DisplayHelp($"The suspect's vehicle is {vehColor} {susVeh.Model.Name}", 4500);
                if (Game.LocalPlayer.Character.Position.DistanceTo(susVeh.Position) < 45f)
                {
                    state = CalloutState.OnScene;
                    if (Initialization.IsLSPDFRPluginRunning("GrammarPolice", new Version("1.3.1.0")))
                        GrammarPolice.API.Functions.Scene(false, false);
                    break;
                }
            }
            if (blip.Exists()) blip.Delete();
            blip = susVeh.AttachBlip();
            blip.Color = susVeh.PrimaryColor;
            blip.Scale = 0.72f;
            blip.EnableRoute(blip.Color);
        }
        private void DisplayCodeFourMessage()
        {
            if (CalloutRunning)
            {              
                state = CalloutState.Finish;
                GameFiber.Sleep(4000);
                Game.DisplayNotification("~g~Code 4");

                Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH WE_ARE_CODE FOUR NO_FURTHER_UNITS_REQUIRED");
                End();
            }
        }
        private enum CalloutState
        {
            EnRoute,
            OnScene,
            TalkWithSuspect,
            Pursuit,
            AfterTalk,
            Finish
        }
    }
}
