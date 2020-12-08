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
        private bool driverArrested = false;
        private bool passengerArrested = false;
        private enum CalloutState { EnRoute, OnScene, Pursuit, Finish}

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
                if (state != CalloutState.Finish) GameFiber.StartNew(End);
                else DisplayCodeFourMessage();
            }
           if (state != CalloutState.Finish)
            {
                if (driver.Exists() && Functions.IsPedArrested(driver) && !driverArrested)
                {
                    driverArrested = true;
                    Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS SUSPECT_IN_CUSTODY IN_OR_ON_POSITION", driver.Position);
                }
                if (passenger.Exists() && Functions.IsPedArrested(passenger) && !passengerArrested)
                {
                    passengerArrested = true;
                    Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS SUSPECT_IN_CUSTODY IN_OR_ON_POSITION", passenger.Position);
                }
            }         
            base.Process();
        }
        public override void End()
        {
            if (state == CalloutState.Finish)
            {
                if (driver.Exists() && driver.IsAlive) driver.Dismiss();
                if (passenger.Exists()) passenger.Dismiss();
                if (blip.Exists()) blip.Delete();
                if (susVeh.Exists()) susVeh.Dismiss();
            }
            else
            {
                if (driver.Exists() && driver.IsAlive) driver.Delete();
                if (passenger.Exists()) passenger.Delete();
                if (blip.Exists()) blip.Delete();
                if (susVeh.Exists()) susVeh.Delete();
            }
            if (Initialization.IsLSPDFRPluginRunning("GrammarPolice"))
                GrammarPolice.API.Functions.Available(true, false);
            CalloutRunning = false;
            Peralatan.Speaking = false;
            base.End();
        }
        private void Kosongan()
        {
            CalloutRunning = true;
            GameFiber.StartNew(delegate
            {
                while (Functions.GetIsAudioEngineBusy())
                {
                    GameFiber.Yield();
                }
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
                        susVeh.InjectRandomItemToVehicle();
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
                while (Functions.GetIsAudioEngineBusy())
                {
                    GameFiber.Yield();
                }
                Functions.PlayScannerAudio($"ATTENTION_ALL_UNITS CITIZENS_REPORT SUSPECT_IS DRIVING_A COLOR_{vehColor} {susVeh.Model.Name.ToUpper()} BAR_TARGET_PLATE "
                    + Peralatan.GetLicensePlateAudio(susVeh.LicensePlate) + " BAR_PROCEED_WITH_CAUTION");
                GetClose();
                while (CalloutRunning)
                {
                    GameFiber.Yield();
                    if (Game.LocalPlayer.Character.Position.DistanceTo(susVeh.Position) < 10f) break;
                }
                Game.HideHelp();
                susVeh.InjectRandomItemToVehicle();
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
                                    passenger.SetPedAsWanted();
                                    var random2 = MathHelper.GetRandomInteger(1, 10);
                                    if (random2 < 3)
                                    {
                                        passenger.Metadata.searchPed = CommonVariables.DangerousVehicleItems.GetRandomElement();
                                        pursuit = Functions.CreatePursuit();
                                        Functions.AddPedToPursuit(pursuit, passenger);
                                        Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                                        pursuitCreated = true;
                                        Functions.RequestBackup(passenger.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
                                        Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_RESIST_ARREST IN_OR_ON_POSITION", passenger.Position);
                                    }
                                    else if (random2 > 2 && random2 < 6)
                                    {
                                        passenger.Metadata.searchPed = CommonVariables.DangerousPedItem.GetRandomElement();
                                    }
                                    else passenger.Metadata.stpAlcoholDetected = true;
                                }
                                else if (random1 > 3 && random1 < 7)
                                {
                                    if (Peralatan.Random.Next(50) % 2 == 0) passenger.Metadata.searchPed = CommonVariables.DangerousPedItem.GetRandomElement();
                                    else passenger.Metadata.searchPed = CommonVariables.SuspiciousItems.GetRandomElement();
                                }
                                else passenger.Metadata.searchPed = CommonVariables.CommonItems.GetRandomElement();
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
                    "~b~Officer~s~: You know what, your vehicle has been reported as a suspicious vehicle",
                    "~y~Suspect~s~: I don't intend to do anything sir, I just want to sleep",
                    "~b~Officer~s~: Hmm, do you consent if i search your vehicle",
                    "~y~Suspect~s~: Absolutely, go ahead officer",
                    "~y~[CONVERSATION IS OVER]"
                };
                Game.LocalPlayer.Character.PlayAmbientSpeech("Generic_Hi");
                if (!pursuitCreated)
                {
                    Peralatan.HandleSpeech(cakap, driver);
                    "Proceed with further ~o~investigation~s~ ~y~(you can do record check or frisk the suspect)".DisplayNotifWithLogo("SuspiciousVehicle");
                    uint notif = 0;
                    if (Initialization.IsLSPDFRPluginRunning("StopThePed"))
                    {
                        StopThePed.API.Events.searchVehicleEvent += (Vehicle veh) =>
                        {
                            if (veh = susVeh)
                            {
                                if (notif != 0) Game.RemoveNotification(notif);
                                "if you found something suspicious, you can arrest the suspect".DisplayNotifWithLogo(out notif ,"Strange Looking Vehicle");
                            }
                        };
                        StopThePed.API.Events.patDownPedEvent += (Ped Ped) =>
                        {
                            if (Ped == driver || Ped == passenger)
                            {
                                if (notif != 0) Game.RemoveNotification(notif);
                                "if you found something suspicious, you can arrest the suspect".DisplayNotifWithLogo(out notif, "Strange Looking Vehicle");
                            }
                        };
                    }
                }
                while (CalloutRunning)
                {
                    GameFiber.Yield();
                    if (passenger.Exists())
                    {
                        if (driver.Exists() && driverArrested && passengerArrested )
                        {
                            "Attention! ~g~driver~s~ is ~r~arrested~s~ and ~g~passenger~s~ is ~r~arrested~s~".DisplayNotifWithLogo("A Strange Looking Vehicle");
                            DisplayCodeFourMessage();
                        }
                        else if (driver.Exists() && driverArrested && passenger.IsDead)
                        {
                            "Attention! ~g~driver~s~ is ~r~dead~s~ and ~g~passenger~s~ is ~r~arrested~s~".DisplayNotifWithLogo("A Strange Looking Vehicle");
                            DisplayCodeFourMessage();
                        }
                        else if ((driver.Exists() && driver.IsDead && passengerArrested))
                        {
                            "Attention! ~g~driver~s~ is ~r~arrested~s~ and ~g~passenger~s~ is ~r~dead~s~".DisplayNotifWithLogo("A Strange Looking Vehicle");
                            DisplayCodeFourMessage();
                        }
                        else if (driver.Exists() && driver.IsDead && passenger.IsDead)
                        {
                            "Attention! ~g~driver~s~ is ~r~dead~s~ and ~g~passenger~s~ is ~r~dead~s~".DisplayNotifWithLogo("A Strange Looking Vehicle");
                            DisplayCodeFourMessage();
                        }
                    }
                    else
                    {
                        if (driver.Exists() && !passenger.Exists())
                        {
                            if (driver.IsDead)
                            {
                                "Attention!~g~driver~s~ is ~r~dead~s~".DisplayNotifWithLogo("A Strange Looking Vehicle");
                                DisplayCodeFourMessage();
                            }
                            else if (driverArrested)
                            {
                                "Attention! ~g~driver~s~ is ~r~arrested~s~".DisplayNotifWithLogo("A Strange Looking Vehicle");
                                DisplayCodeFourMessage();
                            }
                        }
                    }
                }
            });
        }
        private void GetClose()
        {
            int counter = 0;
            while (CalloutRunning)
            {
                GameFiber.Yield();
                counter++;
                if (counter % 300 == 0) Game.DisplayHelp($"~y~The suspect's vehicle is ~c~{vehColor}~s~ ~g~{susVeh.Model.Name}~s~", 6500);
                if (Game.LocalPlayer.Character.Position.DistanceTo(susVeh.Position) < 45f)
                {
                    state = CalloutState.OnScene;
                    if (Initialization.IsLSPDFRPluginRunning("GrammarPolice"))
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
    }
}
