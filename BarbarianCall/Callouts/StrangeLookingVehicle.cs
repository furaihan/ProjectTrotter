using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
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
        private List<Ped> nearbyPeds;
        private bool CalloutRunning = false;
        private CalloutState state;
        private string vehColor;
        private Model vehModel;

        public override bool OnBeforeCalloutDisplayed()
        {
            var path = @"Plugins/LSPDFR/BarbarianCall/StrangeLookingVehicle/Locations.xml";
            DivisiXml.Deserialization.GetDataFromXml(path, out List<Vector3> locationToSelect, out List<float> headingToSelect);
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
            Functions.PlayScannerAudioUsingPosition("DISP_ATTENTION_UNIT CITIZENS_REPORT BAR_SUSPICIOUS_VEHICLE IN_OR_ON_POSITION", SpawnPoint);
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
            $"One of the citizens in {SpawnPoint.GetZoneName()} reported that they saw a vehicle that seemed suspicious, go there and check the vehicle. please be careful".DisplayNotifWithLogo(
                "Strange Looking Vehicle");
            state = CalloutState.EnRoute;
            susVeh = new Vehicle(vehModel, SpawnPoint, SpawnHeading);
            susVeh.PrimaryColor = CommonVariables.CommonUnderstandableColor.GetRandomColor();
            susVeh.SecondaryColor = susVeh.PrimaryColor;
            susVeh.RandomiseLicencePlate();
            susVeh.MakePersistent();
            driver = susVeh.CreateRandomDriver();
            driver.MakeMissionPed();
            if (MathHelper.GetRandomInteger(1, 100) <= 30)
            {
                passenger = new Ped(SpawnPoint);
                passenger.WarpIntoVehicle(susVeh, 0);
                passenger.MakeMissionPed();
            }
            blip = new Blip(SpawnPoint, 45f);
            blip.Color = System.Drawing.Color.Yellow;
            blip.EnableRoute(System.Drawing.Color.Yellow);
            GameFiber.Sleep(2000);
            Functions.PlayScannerAudio("UNITS_RESPOND_CODE_2");
            GameFiber.Sleep(2000);
            try
            {
                vehColor = susVeh.GetVehicleColor();
            } catch
            {
                vehColor = "weirdly colored";
            }
            Functions.PlayScannerAudio($"ATTENTION_ALL_UNITS CITIZENS_REPORT SUSPECT_IS DRIVING_A COLOR_{vehColor} {susVeh.Model.Name.ToUpper()} BAR_TARGET_PLATE " 
                + Peralatan.GetLicensePlateAudio(susVeh.LicensePlate) + " BAR_PROCEED_WITH_CAUTION");
            if (!CalloutRunning) Kosongan();
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
            CalloutRunning = false;
            base.End();
        }
        private void Kosongan()
        {
            CalloutRunning = true;
            GameFiber.StartNew(delegate
            {
                bool njeblug = false;
                GetClose();
                if (passenger.Exists()) passenger.Delete();
                driver.Delete();
                if (susVeh.Position.GetNearbyPedByRadius(15f).Count() > 15) njeblug = true;
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
        private void GetClose()
        {
            while (CalloutRunning)
            {
                GameFiber.Yield();
                GameFiber.Sleep(12000);
                Game.DisplayHelp($"The suspect's vehicle is {vehColor} {susVeh.Model.Name}", 4500);
                if (Game.LocalPlayer.Character.Position.DistanceTo(susVeh.Position) < 45f)
                {
                    state = CalloutState.OnScene;
                    break;
                }
            }
            if (blip.Exists()) blip.Delete();
            blip = susVeh.AttachBlip();
            blip.Color = susVeh.PrimaryColor;
            blip.Scale = 0.72f;
            blip.EnableRoute(System.Drawing.Color.LightYellow);
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
