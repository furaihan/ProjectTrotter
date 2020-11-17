using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Engine.Scripting.Entities;

namespace BarbarianCall.Callouts
{
    [CalloutInfo("Officer Stabbed", CalloutProbability.Medium)]
    public class OfficerStabbed : Callout
    {
        private Ped suspect;
        private Ped officer;
        private Ped passengger;
        private Vehicle susVeh;
        private Vehicle offVeh;
        private Blip blip;
        private bool CalloutRunning = false;
        private bool PursuitCreated = false;
        private Vector3 SpawnPoint;
        private float SpawnHeading;
        private LHandle pursuit;
        private string vehColor;
        private Model susVehModel;
        private Persona DriverPersona;
        private Persona PassenggerPersona;
        private IEnumerable<Ped> nearbyPed;
        private string[] CityCarModels = new string[] { "POLICE", "POLICE2", "POLICE3", "POLICE4" };
        private string[] CountrysideCarModels = new string[] { "SHERIFF", "SHERIFF2" };
        private string path = @"Plugins/LSPDFR/BarbarianCall/OfficerStabbed/";
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
            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 15f);
            AddMinimumDistanceCheck(30f, SpawnPoint);
            susVehModel = CommonVariables.CarsToSelect.GetRandomModel();
            susVehModel.LoadAndWait();
            CalloutPosition = SpawnPoint;
            CalloutMessage = "Officer Stabbed";
            uint zoneHash = Rage.Native.NativeFunction.CallByHash<uint>(0x7ee64d51e8498728, SpawnPoint.X, SpawnPoint.Y, SpawnPoint.Z);
            susVehModel = CommonVariables.CarsToSelect.GetRandomModel();
            susVehModel.LoadAndWait();
            if (Initialization.IsLSPDFRPluginRunning("UltimateBackup", new Version("1.8.2.1")))
            {
                Tuple<Vehicle, List<Ped>> ubApi = UltimateBackup.API.Functions.getLocalPatrolUnit(SpawnPoint, 1);
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
            offVeh.IsVisible = false;
            officer.IsVisible = false;
            officer.Tasks.LeaveVehicle(LeaveVehicleFlags.WarpOut);
            officer.Kill();
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS BAR_CODE_99 BAR_OFFICER_STABBED IN_OR_ON_POSITION", SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            officer.MakeMissionPed();
            offVeh.MakePersistent();
            offVeh.RandomiseLicencePlate();
            offVeh.IsVisible = true;
            officer.IsVisible = true;
            blip = new Blip(SpawnPoint, 45f);
            blip.Color = System.Drawing.Color.Yellow;
            blip.EnableRoute(System.Drawing.Color.Yellow);

            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            base.Process();
        }
        public override void End()
        {
            base.End();
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
                    if (Initialization.IsLSPDFRPluginRunning("GrammarPolice", new Version("1.3.1.0")))
                        GrammarPolice.API.Functions.Scene(false, false);
                    break;
                }
            }
        }
    }
}
