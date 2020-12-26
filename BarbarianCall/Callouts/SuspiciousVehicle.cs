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
    public class SuspiciousVehicle : CalloutBase
    {
        private Ped Passenger;
        private Blip PassengerBlip;
        private ESuspectStates PassengerState;
        private ESuspectStates SuspectState;
        public override bool OnBeforeCalloutDisplayed()
        {
            FilePath = @"Plugins/LSPDFR/BarbarianCall/StrangeLookingVehicle/";
            var spawnPoints = DivisiXml.Deserialization.GetSpawnPointFromXml(System.IO.Path.Combine(FilePath, "Locations.xml"));
            Spawn = Peralatan.SelectNearbySpawnpoint(spawnPoints);
            SpawnPoint = Spawn;
            SpawnHeading = Spawn;
            if (SpawnPoint == Vector3.Zero || SpawnHeading == 0f)
            {
                Peralatan.ToLog("Strange Looking Vehicle callout aborted");
                Peralatan.ToLog("No nearby location found");
                return false;
            }
            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 15f);
            AddMinimumDistanceCheck(30f, SpawnPoint);
            CarModel = CommonVariables.CarsToSelect.GetRandomModel();
            CarModel.LoadAndWait();
            CalloutPosition = SpawnPoint;
            CalloutMessage = "A Strange Looking Vehicle";
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS CITIZENS_REPORT BAR_SUSPICIOUS_VEHICLE IN_OR_ON_POSITION", SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            CalloutRunning = true;
            SuspectCar = new Vehicle(CommonVariables.CarsToSelect.GetRandomElement(), SpawnPoint, SpawnHeading);
            SuspectCar.MakePersistent();
            SuspectCar.RandomiseLicencePlate();
            SuspectCar.PlaceOnGroundProperly();
            Suspect = SuspectCar.CreateRandomDriver();
            Suspect.MakeMissionPed();
            if (Peralatan.Random.Next() % 4 == 0)
            {
                Passenger = new Ped(SpawnPoint, SpawnHeading);
                Passenger.WarpIntoVehicle(SuspectCar, 0);
                Passenger.MakeMissionPed();
                GameFiber.Wait(75);
                if (Peralatan.Random.Next() % 3 == 0) Passenger.SetPedAsWanted();
            }
            return base.OnCalloutAccepted();
        }

    }
}
