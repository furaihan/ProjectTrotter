using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rage;
using Rage.Native;
using LSPDFR = LSPD_First_Response.Mod.API.Functions;
using BarbarianCall.Types;
using BarbarianCall.Freemode;
using BarbarianCall.Extensions;
using BarbarianCall.API;

namespace BarbarianCall.Callouts
{
    [LSPD_First_Response.Mod.Callouts.CalloutInfo("Stolen Boat Trailer", LSPD_First_Response.Mod.Callouts.CalloutProbability.High)]
    public class StolenBoatTrailer : CalloutBase
    {
        private readonly Model longfinModel = "h4_prop_h4_p_boat_01a";
        private readonly Vector3 attachOfsett = new Vector3(-0.0360000134f, -0.578999758f, 0.180000067f);
        private readonly Rotator attachRotation = new Rotator(0f, -2.97000074f, 88.9999924f);
        private Spawnpoint Dock = Spawnpoint.Zero;
        private readonly List<Model> truckModels = new List<Model>()
        {
            "PHANTOM", "PHANTOM2", "PHANTOM3", "HAULER", "HAULER2", "PACKER"
        };
        private readonly Model trailerModel = "TRFLAT";
        private Model truckModel = "PHANTOM2";
        readonly List<List<string>> weaponLoadouts = new List<List<string>>()
        {
            new List<string>() { "WT_SG_ASL", "WT_MACHPIST", "WT_MACHETE", "WT_GNADE" },
            new List<string>() { "WT_MLTRYRFL", "WTU_PIST_50", "WT_KNUCKLE", "WT_GNADE_STK" },
            new List<string>() { "WT_SNIP_RIF", "WT_PIST_AP", "WT_KNIFE", "WT_MOLOTOV" },
            new List<string>() { "WT_SMG", "WT_SNSPISTOL", "WT_KNIFE", "WT_PIPE" },
            new List<string>() { "WT_RIFLE_ASL", "WT_PIST", "WT_MACHETE", "WT_PIPE" },
        };
        private FreemodePed Driver;
        private const string XmlPath = @"Plugins\LSPDFR\BarbarianCall\Locations\TruckSpawn.xml";
        private const string XmlDocks = @"Plugins\LSPDFR\BarbarianCall\Locations\Docks.xml";
        private List<FreemodePed> Associates = new List<FreemodePed>();
        private Vehicle truck;
        private Vehicle trailer;
        private Rage.Object boat;
        public override bool OnBeforeCalloutDisplayed()
        {
            Spawn = Peralatan.SelectNearbySpawnpoint(DivisiXml.Deserialization.GetSpawnPointFromXml(XmlPath), 1000f, 600f);
            if (Spawn == Spawnpoint.Zero) Spawn = SpawnManager.GetVehicleSpawnPoint5(PlayerPed.Position, 1000f, 500f, true);
            if (Spawn == Spawnpoint.Zero)
            {
                return false;
            }
            Position = Spawn.Position;
            SpawnHeading = Spawn.Heading;
            truckModel = truckModels.GetRandomElement();
            CalloutPosition = Spawn;
            CalloutAdvisory = $"Suspect vehicle is {truckModel.GetDisplayName()}";
            CalloutMessage = "Stolen Boat Trailer";
            Criminal = new RelationshipGroup("CRIMINAL");
            ShowCalloutAreaBlipBeforeAccepting(CalloutPosition, 25f);
            return base.OnBeforeCalloutDisplayed(); 
        }
        public override bool OnCalloutAccepted()
        {
            truckModel.LoadAndWait();
            trailerModel.LoadAndWait();
            longfinModel.LoadAndWait();
            $"{GetType().Name} | Preparing suspect".ToLog();
            truck = new Vehicle(truckModel, Spawn, Spawn);
            trailer = new Vehicle(trailerModel, Vector3.Zero);
            boat = new Rage.Object(longfinModel, Vector3.Zero)
            {
                IsCollisionEnabled = false,
                IsInvincible = true,
            };
            truck.Trailer = trailer;
            boat.AttachTo(trailer, -1, attachOfsett, attachRotation);
            CalloutEntities.Add(truck);
            CalloutEntities.Add(trailer);
            CalloutEntities.Add(boat);
            NativeFunction.Natives.x0419B167EE128F33(trailer, 1);
            NativeFunction.Natives.xF3B0E0AED097A3F5(trailer, 0f);
            NativeFunction.Natives.xD3E51C0AB8C26EEE(trailer, 0f);
            Driver = new FreemodePed(Position, true);
            PreparePed(Driver);
            Driver.WarpIntoVehicle(truck, -1);
            OutfitMale.Casual.GetRandomElement()(Driver);
            CalloutEntities.Add(Driver);
            for (int i = 0; i < NativeFunction.Natives.GET_VEHICLE_MAX_NUMBER_OF_PASSENGERS<int>(truck); i++)
            {
                $"{GetType().Name} Creating passenger {i}".ToLog();
                FreemodePed ped = new FreemodePed(Position, true);
                PreparePed(ped);
                ped.WarpIntoVehicle(truck, i);
                Associates.Add(ped);
                CalloutEntities.Add(ped);
                OutfitMale.Casual.GetRandomElement()(Driver);
            }
            Blip = new Blip(CalloutPosition, 25f)
            {
                Color = Yellow,
                IsRouteEnabled = true,
                RouteColor = Yellow,
            };
            CalloutBlips.Add(Blip);
            Logic();
            CalloutMainFiber.Start();
            return base.OnCalloutAccepted();
        }
        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }
        public override void End()
        {
            truckModel.Dismiss();
            trailerModel.Dismiss();
            longfinModel.Dismiss();
            if (stpEventAdded)
            {
                StopThePedFunc.OnVehicleSearch -= SearchVehicleHandler;
            }
            base.End();
        }
        bool stpEventAdded = false;
        private void Logic()
        {
            CalloutMainFiber = new GameFiber(() =>
            {
                try
                {
                    List<Ped> Organization = truck.Occupants.ToList();
                    GetClose();
                    if (!CalloutRunning) return;
                    if (Blip) Blip.Delete();
                    Dock = GetDockPoint();
                    driveTask = Driver.VehicleMission(Dock.Position, MissionType.GoTo, truck.TopSpeed, (VehicleDrivingFlags)1107573356, -1.0f, 10f, true);
                    GameFiber.Wait(5000);
                    if (StopThePedRunning)
                    {
                        StopThePedFunc.OnVehicleSearch += SearchVehicleHandler;
                        stpEventAdded = true;
                    }
                    SetRelationship();
                    GetHandlingData(truck);
                    SetupPursuit();
                    Associates.ForEach(x =>
                    {
                        if (x) x.CombatAgainstHatedTargetAroundPed(100f);
                    });
                    bool arrive = false;
                    bool leave = false;
                    float force = truck.DriveForce;
                    GameFiber.Sleep(2000);
                    GetHandlingData(truck);
                    while (CalloutRunning)
                    {
                        foreach (Ped associate in Associates)
                        {
                            if (associate && associate.IsAlive)
                            {
                                if (!associate.CombatTarget.Exists() || associate.CombatTarget.IsDead || !associate.IsTaskActive(PedTask.CombatClosestTargetInArea))
                                {
                                    associate.CombatAgainstHatedTargetAroundPed(100f);
                                }
                            }
                        }
                        GameFiber.Yield();
                        if (Organization.All(x => (x && (!x.IsAlive || LSPDFR.IsPedArrested(x))) || !x))
                        {                    
                            break;
                        }
                        if (!arrive)
                        {
                            if (truck.GetActiveMissionType() != MissionType.GoTo)
                            {
                                driveTask = Driver.VehicleMission(Dock.Position, MissionType.GoTo, truck.TopSpeed, (VehicleDrivingFlags)1107573356, -1.0f, 10f, true);
                                GameFiber.Wait(2000);
                            }

                            if (truck.DistanceToSquared(Dock.Position) < 625f)
                            {
                                $"Drive task done".ToLog();
                                arrive = true;
                            }
                        }
                        if (arrive && !leave)
                        {
                            Driver.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen).WaitForCompletion(5000);
                            Driver.CombatAgainstHatedTargetAroundPed(500f);
                            Associates.Add(Driver);
                            leave = true;
                        }
                        if (truck.DriveForce != force)
                        {
                            $"Drive force changed".ToLog();
                            truck.DriveForce = force;
                        }
                    }
                    End();
                }
                catch (Exception e)
                {
                    "Stolen Boat Trailer callout crashes".ToLog();
                    e.ToString().ToLog();
                    NetExtension.SendError(e);
                    "Stolen Boat Trailer callout crashed, please send your log".DisplayNotifWithLogo("Stolen Boat Trailer");
                    End();
                }
            });
        }
        bool searched = false;
        private void SearchVehicleHandler(Vehicle vehicle)
        {
            if (vehicle == truck)
            {
                if (searched) return;
                List<string> selectedLoadout = weaponLoadouts.GetRandomElement();
                string text = $"Items inside briefcase:~n~";
                int count = 1;
                foreach (string element in selectedLoadout)
                {
                    text = text + $"{count}. " + Peralatan.GetLabelText(element) + "~n~";
                    count++;
                }
                text.DisplayNotifWithLogo(GetType().Name, title: "Briefcase Inspect", hudColor: RAGENativeUI.HudColor.Red);
                searched = true;
            }
        }

        private Task driveTask = null;
        private void SetupPursuit()
        {
            Pursuit = LSPDFR.CreatePursuit();
            if (Driver)
            {
                LSPDFR.AddPedToPursuit(Pursuit, Driver);
                LSPDFR.SetPursuitDisableAIForPed(Driver, true);
            }
            Associates.ForEach(x =>
            {
                if (x)
                {
                    LSPDFR.AddPedToPursuit(Pursuit, x);
                    LSPDFR.SetPursuitDisableAIForPed(x, true);
                }
            });
            LSPDFR.SetPursuitIsActiveForPlayer(Pursuit, true);
            PursuitCreated = true;
        }
        private void GetClose()
        {
            while (true)
            {
                GameFiber.Yield();
                bool usingSiren = PlayerPed.IsInAnyVehicle(false) && PlayerPed.CurrentVehicle.HasSiren && !PlayerPed.CurrentVehicle.IsSirenSilent;
                if (PlayerPed.DistanceToSquared(Spawn) < (usingSiren ? 2500f : 625f)) break;
            }
        }
        private RelationshipGroup Criminal;
        private void PreparePed(Ped ped)
        {
            if (ped)
            {
                ped.MakeMissionPed();
                ped.Accuracy = Peralatan.Random.Next(80, 100);
                ped.RelationshipGroup = Criminal;
                NativeFunction.Natives.GIVE_DELAYED_WEAPON_TO_PED(ped, 0x22D8FE39, -1, true);
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 5, true);
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 2, true);
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 3, false);
                NativeFunction.Natives.SET_PED_COMBAT_MOVEMENT(ped, 2);
                NativeFunction.Natives.SET_PED_COMBAT_ABILITY(ped, 2);
                NativeFunction.Natives.SET_PED_COMBAT_RANGE(ped, 4);
                NativeFunction.Natives.SET_PED_TARGET_LOSS_RESPONSE(ped, 1);
                NativeFunction.Natives.SET_PED_HIGHLY_PERCEPTIVE(ped, true);
                NativeFunction.Natives.SET_PED_CAN_BE_TARGETTED(ped, true);
                NativeFunction.Natives.SET_PED_SEEING_RANGE(ped, 5 + 100f);
                NativeFunction.Natives.SET_PED_VISUAL_FIELD_PERIPHERAL_RANGE(ped, 400f);
                NativeFunction.Natives.SET_COMBAT_FLOAT(ped, 10, 400f);
            }
        }
        private void SetRelationship()
        {
            Criminal.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
            Criminal.SetRelationshipWith(RelationshipGroup.Medic, Relationship.Hate);
            Criminal.SetRelationshipWith(RelationshipGroup.Fireman, Relationship.Hate);
            Criminal.SetRelationshipWith(PlayerPed.RelationshipGroup, Relationship.Hate);
            RelationshipGroup.Cop.SetRelationshipWith(Criminal, Relationship.Hate);
            RelationshipGroup.Medic.SetRelationshipWith(Criminal, Relationship.Hate);
            RelationshipGroup.Fireman.SetRelationshipWith(Criminal, Relationship.Hate);
        }
        private unsafe void GetHandlingData(Vehicle vehicle)
        {
            try
            {
                var props = vehicle.HandlingData.GetType().GetProperties();
                foreach (var prop in props)
                {
                    try
                    {
                        $"{prop.Name}={prop.GetValue(vehicle.HandlingData, null)}".ToLog();
                    }
                    catch (Exception e)
                    {
                        e.ToString().ToLog();
                        continue;
                    }                   
                }
            }
            catch (Exception e)
            {
                e.ToString().ToLog();
            }           
        }
        private Spawnpoint GetDockPoint()
        {
            List<Spawnpoint> spawnpoints = DivisiXml.Deserialization.GetSpawnPointFromXml(XmlDocks);
            Spawnpoint ret = spawnpoints.OrderByDescending(x => x.Position.DistanceToSquared(CalloutPosition)).Take(4).GetRandomElement();
            $"Docks located in {ret.Position.GetZoneName()} {World.GetStreetName(ret.Position)}".ToLog();
            return ret;
        }
    }
}
