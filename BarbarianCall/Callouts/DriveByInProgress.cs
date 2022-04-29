using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbarianCall.Types;
using Rage;
using Rage.Native;
using L = LSPD_First_Response.Mod.API.Functions;

namespace BarbarianCall.Callouts
{
    public class DriveByInProgress : CalloutBase
    {
        /*
         * 1 = enroute
         * 2 = 
         * 3 = 
         * 4 = 
         * 5 = 
         */
        private int status = 0;
        private Model model1;
        private Model model2;
        private Spawnpoint spawn2;
        public bool CanEnd = false;
        private List<Model> Gang1Model = new List<Model>();
        private List<Model> Gang2Model = new List<Model>();
        private Vehicle veh1;
        private Vehicle veh2;
        public override bool OnBeforeCalloutDisplayed()
        {
            var carModelPool = World.EnumerateVehicles().Where(x=> x.IsCar && x.Model.NumberOfSeats >= 4).Select(x=> x.Model).ToList();
            if (!carModelPool.Any())
            {
                $"{GetType().Name} | Cannot find suitable car model".ToLog();
                return false;
            }
            model1 = carModelPool.GetRandomElement();
            model2 = carModelPool.GetRandomElement();
            Spawn = SpawnManager.GetVehicleSpawnPoint5(PlayerPed.Position, 425, 725, true);
            if (Spawn == Spawnpoint.Zero) Spawn = SpawnManager.GetVehicleSpawnPoint5(PlayerPed.Position, 425, 825);
            if (Spawn == Spawnpoint.Zero) Spawn = SpawnManager.GetVehicleSpawnPoint5(PlayerPed.Position, 420, 850);
            if (Spawn == Spawnpoint.Zero) Spawn = SpawnManager.GetVehicleSpawnPoint5(PlayerPed.Position, 325, 875);
            if (Spawn == Spawnpoint.Zero)
            {
                $"{GetType().Name} | Spawnpoint is not found, cancelling the callout".ToLog();
                return false;
            }
            var tempSp = World.GetNextPositionOnStreet(Spawn.Position);
            var tempH = SpawnManager.GetRoadHeading(tempSp);
            spawn2 = new Spawnpoint(tempSp, tempH);
            Gang1Model = Globals.GangPedModels.Values.ToList().GetRandomElement();
            Gang2Model = Globals.GangPedModels.Values.ToList().GetRandomElement(m=> m != Gang1Model);
            Position = Spawn;
            SpawnHeading = Spawn;
            CalloutAdvisory = "";
            CalloutMessage = "Drive-By In Progress";
            FriendlyName = "Drive-By In Progress";
            PlayScannerWithCallsign("WE_HAVE CRIME_GUNFIRE IN_OR_ON_POSITION", Spawn);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            veh1 = new Vehicle(model1, Spawn, Spawn);
            veh2 = new Vehicle(model2, spawn2, spawn2);
            FillVehicle(veh1, Gang1Model, RelationshipGroup.Gang1);
            FillVehicle(veh2, Gang2Model, RelationshipGroup.Gang2);
            RelationshipGroup.Gang1.SetRelationshipWith(RelationshipGroup.Gang2, Relationship.Hate);
            RelationshipGroup.Gang1.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
            RelationshipGroup.Gang1.SetRelationshipWith(PlayerPed.RelationshipGroup, Relationship.Hate);
            RelationshipGroup.Gang2.SetRelationshipWith(RelationshipGroup.Gang1, Relationship.Hate);
            RelationshipGroup.Gang2.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
            RelationshipGroup.Gang2.SetRelationshipWith(PlayerPed.RelationshipGroup, Relationship.Hate);
            LogikaInformatika();
            CalloutMainFiber.Start();
            return base.OnCalloutAccepted();
        }
        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }
        public override void Process()
        {
            base.Process();
        }
        public override void End()
        {
            base.End();
        }
        public void LogikaInformatika()
        {
            CalloutMainFiber = new GameFiber(() =>
            {
                switch (status)
                {
                    case 1:

                        break;
                }
            });
        }
        protected override void CleanUp()
        {
            // TODO: implement CleanUp()
        }

        void FillVehicle(Vehicle vehicle, List<Model> models, RelationshipGroup relationshipGroup)
        {
            if (vehicle)
            {
                while (vehicle.FreeSeatsCount > 0)
                {
                    Ped ped = new Ped(models.GetRandomElement(), Spawn, spawn2);
                    ped.RelationshipGroup = relationshipGroup;
                    NativeFunction.Natives.TASK_WARP_PED_INTO_VEHICLE(ped, vehicle, -2);
                }
            }
        }
    }
}
