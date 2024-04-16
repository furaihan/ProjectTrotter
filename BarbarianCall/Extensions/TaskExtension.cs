using Rage;
using static Rage.Native.NativeFunction;
using System;

namespace BarbarianCall.Extensions
{
    internal static class TaskExtension
    {
        public static Task LookAtEntity(this Ped ped, Entity lookAt, int duration)
        {
            Natives.TASK_LOOK_AT_ENTITY(ped, lookAt, duration, 2048, 3);
            return Task.GetTask(ped, "TASK_LOOK_AT_ENTITY");
        }
        public static Task LookAtCoord(this Ped ped, Vector3 target, int duration)
        {
            Natives.TASK_LOOK_AT_COORD(ped, target.X, target.Y, target.Z, duration, 0, 2);
            return Task.GetTask(ped, "TASK_LOOK_AT_COORD");
        }
        public static Task ShootAtCoord(this Ped ped, Vector3 coordToShoot, int duration, FiringPattern firingPattern)
        {
            Natives.TASK_SHOOT_AT_COORD(ped, coordToShoot.X, coordToShoot.Y, coordToShoot.Z, duration, (uint)firingPattern);
            return Task.GetTask(ped, "TASK_SHOOT_AT_COORD");
        }
        public static Task ChatTo(this Ped ped, Ped target) 
        {
            Natives.TASK_CHAT_TO_PED(ped, target, 16, 0f, 0f, 0f, 0f, 0f);
            return Task.GetTask(ped, "TASK_CHAT_TO_PED");
        }
        public static Task DriveTo(this Ped ped, Vector3 target, float stoppingRange, float speed, VehicleDrivingFlags drivingStyle) => DriveTo(ped, ped.CurrentVehicle, target, stoppingRange, speed, drivingStyle);
        public static Task DriveTo(this Ped ped, Vehicle vehicle, Vector3 target, float stoppingRange, float speed, VehicleDrivingFlags drivingStyle)
        {
            Natives.TASK_VEHICLE_DRIVE_TO_COORD_LONGRANGE(ped, vehicle, target.X, target.Y, target.Z, speed, (int)drivingStyle, stoppingRange);
            return Task.GetTask(ped, "TASK_VEHICLE_DRIVE_TO_COORD_LONGRANGE");
        }
        public static Task ParkVehicle(this Ped ped, Vector3 position, float heading, float radius = 20f, bool keepEngineOn = false) => ParkVehicle(ped, ped.CurrentVehicle, position, heading, radius, keepEngineOn);
        public static Task ParkVehicle(this Ped ped, Vehicle vehicle, Vector3 position, float heading, float radius = 20f, bool keepEngineOn = false)
        {
            int mode = vehicle.HeadingDiff(heading) < 90 ? 1 : 2;
            Natives.TASK_VEHICLE_PARK(ped, vehicle, position.X, position.Y, position.Z, heading, mode, radius, keepEngineOn);
            return Task.GetTask(ped, "TASK_VEHICLE_PARK");
        }
        public static Task FollowToOfsettOfEntity(this Ped ped, Entity target, Vector3 offset, float speed, float stoppingRange = 10.0f, bool persistFollowing = true)
        {
            Natives.TASK_FOLLOW_TO_OFFSET_OF_ENTITY(ped, target, offset.X, offset.Y, offset.Z, speed, -1, stoppingRange, persistFollowing);
            return Task.GetTask(ped, "TASK_FOLLOW_TO_OFFSET_OF_ENTITY");
        }
        public static Task HeliChase(this Ped ped, Entity target, Vector3 offset)
        {
            Natives.TASK_HELI_CHASE(ped, target, offset.X, offset.Y, offset.Z);
            return Task.GetTask(ped, "TASK_HELI_CHASE");
        }
        public static Task WanderWithVehicle(this Ped ped, float speed, VehicleDrivingFlags drivingStyle) => WanderWithVehicle(ped, ped.CurrentVehicle, speed, drivingStyle);
        public static Task WanderWithVehicle(this Ped ped, Vehicle vehicle, float speed, VehicleDrivingFlags drivingStyle)
        {
            Natives.TASK_VEHICLE_DRIVE_WANDER(ped, vehicle, speed, (int)drivingStyle);
            return Task.GetTask(ped, "TASK_VEHICLE_DRIVE_WANDER");
        }
        public static Task GotoCoordAndAimAtHatedEntitiesNearCoord(this Ped ped, Vector3 destination, Vector3 aimAt, float speed, FiringPattern firingPattern, bool shoot, 
            float distanceStop = 0.0f, float noRoadsDistance = 0.0f)
        {
            Natives.TASK_GO_TO_COORD_AND_AIM_AT_HATED_ENTITIES_NEAR_COORD(ped, destination.X, destination.Y, destination.Z, aimAt.X, aimAt.Y, aimAt.Z, speed, shoot,
                distanceStop, noRoadsDistance, true, 0, -957453492, (uint)firingPattern);
            return Task.GetTask(ped, "TASK_GO_TO_COORD_AND_AIM_AT_HATED_ENTITIES_NEAR_COORD");
        }
        public static Task GotoEntityAiming(this Ped ped, Entity target, float distenceToStop, float startAimDistance)
        {
            Natives.TASK_GOTO_ENTITY_AIMING(ped, target, distenceToStop, startAimDistance);
            return Task.GetTask(ped, "TASK_GOTO_ENTITY_AIMING");
        }
        public static Task CombatAgainstHatedTargetAroundPed(this Ped ped, float radius)
        {
            Natives.REGISTER_HATED_TARGETS_AROUND_PED(ped, radius);
            Natives.TASK_COMBAT_HATED_TARGETS_AROUND_PED(ped, radius);
            return Task.GetTask(ped, "TASK_COMBAT_HATED_TARGETS_AROUND_PED");
        }
        public static Task CombatAgainstHatedTargetInArea(this Ped ped, Vector3 area, float radius)
        {
            Natives.TASK_COMBAT_HATED_TARGETS_IN_AREA(ped, area.X, area.Y, area.Z, radius, 0);
            return Task.GetTask(ped, "TASK_COMBAT_HATED_TARGETS_IN_AREA");
        }
        public static Task DriveVehicleWithNavigationMesh(this Ped ped, Vehicle vehicle, Vector3 destination, VehicleDrivingFlags flags, float speed = 30.0f, float stoppingRange = 5.0f)
        {
            Natives.TASK_VEHICLE_GOTO_NAVMESH(ped, vehicle, destination.X, destination.Y, destination.Z, speed, (int)flags, stoppingRange);
            return Task.GetTask(ped, "TASK_VEHICLE_GOTO_NAVMESH");
        }
        public static Task EscortVehicle(this Ped ped, Vehicle targetVehicle, EscortVehicleMode mode, float speed, VehicleDrivingFlags drivingFlags, float minDistance, float noRoadDistance)
            => EscortVehicle(ped, ped.CurrentVehicle, targetVehicle, mode, speed, drivingFlags, minDistance, noRoadDistance);
        public static Task EscortVehicle(this Ped ped, Vehicle vehicleUsedbyPed, Vehicle vehicleToEscort, EscortVehicleMode mode, float speed, VehicleDrivingFlags drivingFlags, float minDistance, float noRoadDistance)
        {
            Natives.TASK_VEHICLE_ESCORT(ped, vehicleUsedbyPed, vehicleToEscort, (int)mode, speed, (int)drivingFlags, minDistance, -1, noRoadDistance);
            return Task.GetTask(ped, "TASK_VEHICLE_ESCORT");
        }
        public static Task DriveVehicleWithNavigationMesh(this Ped ped, Vector3 destination, VehicleDrivingFlags flags, float speed = 30.0f, float stoppingRange = 5.0f) => 
            DriveVehicleWithNavigationMesh(ped, ped.CurrentVehicle, destination, flags, speed, stoppingRange);
        public static Task FaceTo(this Ped ped, Entity target, int duration)
        {
            Natives.TASK_TURN_PED_TO_FACE_ENTITY(ped, target, duration);
            return Task.GetTask(ped, "TASK_TURN_PED_TO_FACE_ENTITY");
        }
        public static Task ThrowProjectile(this Ped ped, Vector3 throwPos)
        {
            Natives.TASK_THROW_PROJECTILE(ped, throwPos.X, throwPos.Y, throwPos.Z, 0, 0);
            return Task.GetTask(ped, "TASK_THROW_PROJECTILE");
        }
        public static Task PlayScenarioAction(this Ped ped, PedScenario pedScenario, bool playEnterAnim)
        {
            string scenarioName = pedScenario.ToString();
            Natives.TASK_START_SCENARIO_IN_PLACE(ped, scenarioName, -1, playEnterAnim);
            return Task.GetTask(ped, "TASK_START_SCENARIO_IN_PLACE");
        }
        public static Task PlayScenarioAction(this Ped ped, string scenario, Vector3 position, float heading, int duration, bool sitScenario, bool teleport = false)
        {
            Natives.TASK_START_SCENARIO_AT_POSITION(ped, scenario, position, heading, duration, sitScenario, teleport);
            return Task.GetTask(ped, "TASK_START_SCENARIO_AT_POSITION");
        }
        public static Task OpenVehicleDoor(this Ped ped, Vehicle vehicle, int timeout, int seatIndex, float speed)
        {
            Natives.TASK_OPEN_VEHICLE_DOOR(ped, vehicle, timeout, seatIndex, speed);
            return Task.GetTask(ped, "TASK_OPEN_VEHICLE_DOOR");
        }
        public static Task VehicleTempAction(this Ped ped, VehicleManeuver vehicleManuever, int timeout) => VehicleTempAction(ped, ped.CurrentVehicle, vehicleManuever, timeout);
        public static Task VehicleTempAction(this Ped ped, Vehicle vehicle, VehicleManeuver vehicleManeuver, int timeMiliseconds)
        {
            Natives.TASK_VEHICLE_TEMP_ACTION(ped, vehicle, (int)vehicleManeuver, timeMiliseconds);
            return Task.GetTask(ped, "TASK_VEHICLE_TEMP_ACTION");
        }
        /// <summary>
        /// Must have targetVehicle, targetPed, OR destination X/Y/Z set
        /// <para></para>Will follow targeted vehicle/ped, or fly to destination
        /// <para></para>Set whichever is not being used to 0
        /// </summary>
        /// <param name="pilot">The pilot <see cref="Ped"/></param>
        /// <param name="heli">The aircraft of the <paramref name="pilot"/> currently in</param>
        /// <param name="targetVeh">The target <see cref="Vehicle"/>, set to <c>null</c> if not used</param>
        /// <param name="targetPed">The target <see cref="Ped"/>, set to <c>null</c> if not used</param>
        /// <param name="destination">The destination, set to <see cref="Vector3.Zero"/> if not used</param>
        /// <param name="type">the mission flag</param>
        /// <param name="maxSpeed">Heli will fly at maxSpeed (up to actual maximum speed defined by the model's handling config)</param>
        /// <param name="radius">Radius affects how closely the heli will follow tracked ped/vehicle, and when circling (mission type 9) sets the radius (in meters) that it will circle the target from</param>
        /// <param name="targetHeading">Heading is -1.0 for default behavior, which will point the nose of the helicopter towards the destination. 
        /// Set a heading and the heli will lock to that direction when near its destination/target, 
        /// but may still turn towards the destination when flying at higher speed from a further distance.</param>
        /// <param name="maxHeight">If minHeight and maxHeight are set, heli will fly between those specified elevations, relative to ground level and any obstructions/buildings below. 
        /// You can specify -1 for either if you only want to specify one. Usually it is easiest to leave maxHeight at -1, and specify a reasonable minHeight to ensure clearance over any obstacles. 
        /// Note this MUST be passed as an INT, not a FLOAT. </param>
        /// <param name="minHeight">If minHeight and maxHeight are set, heli will fly between those specified elevations, relative to ground level and any obstructions/buildings below. 
        /// You can specify -1 for either if you only want to specify one. Usually it is easiest to leave maxHeight at -1, and specify a reasonable minHeight to ensure clearance over any obstacles. 
        /// Note this MUST be passed as an INT, not a FLOAT. </param>
        /// <param name="behaviorFlags"></param>
        /// <returns>The <see cref="Task"/> represent this method</returns>
        /// <remarks>Notes updated by PNWParksFan, May 2021</remarks>
        public static Task HeliMission(this Ped pilot,
                                       Vehicle heli,
                                       Vehicle targetVeh,
                                       Ped targetPed,
                                       Vector3 destination,
                                       MissionType type,
                                       float maxSpeed,
                                       float radius,
                                       float targetHeading,
                                       int maxHeight,
                                       int minHeight,
                                       int behaviorFlags,
                                       float unk3 = 400.0f)
        {
            if (targetPed is null && targetVeh is null && destination == Vector3.Zero)
            {
                throw new InvalidOperationException("Must have targetVehicle, targetPed, OR destination set");
            }
            uint targetPedHandle = targetPed is null ? 0 : targetPed.Handle;
            uint targetVehHandle = targetVeh is null ? 0 : targetVeh.Handle;
            Natives.TASK_HELI_MISSION(pilot, heli, targetVehHandle, targetPedHandle, destination.X, destination.Y, destination.Z, (int)type, maxSpeed, radius, targetHeading, maxHeight, minHeight, unk3, behaviorFlags);
            return Task.GetTask(pilot, "TASK_HELI_MISSION");
        }
        public static Task VehicleMission(this Ped ped, Vehicle target, MissionType missionType, float maxSpeed, VehicleDrivingFlags flags, float minDistance, float stoppingRange, bool againstTraffic) =>
            VehicleMission(ped, ped.CurrentVehicle, target, missionType, maxSpeed, flags, minDistance, stoppingRange, againstTraffic);
        public static Task VehicleMission(this Ped ped, Vehicle vehicle, Vehicle target, MissionType missionType, float maxSpeed, VehicleDrivingFlags flags, float minDistance, float stoppingRange, bool againstTraffic)
        {
            Natives.TASK_VEHICLE_MISSION(ped, vehicle, target, (int)missionType, maxSpeed, (int)flags, minDistance, stoppingRange, againstTraffic);
            return Task.GetTask(ped, "TASK_VEHICLE_MISSION");
        }
        public static Task VehicleMission(this Ped ped, Ped target, MissionType missionType, float maxSpeed, VehicleDrivingFlags flags, float minDistance, float stoppingRange, bool againstTraffic)
            => VehicleMission(ped, ped.CurrentVehicle, target, missionType, maxSpeed, flags, minDistance, stoppingRange, againstTraffic);
        public static Task VehicleMission(this Ped ped, Vehicle vehicle, Ped target, MissionType missionType, float maxSpeed, VehicleDrivingFlags flags, float minDistance, float stoppingRange, bool againstTraffic)
        {
            Natives.TASK_VEHICLE_MISSION_PED_TARGET(ped, vehicle, target, (int)missionType, maxSpeed, (int)flags, minDistance, stoppingRange, againstTraffic);
            return Task.GetTask(ped, "TASK_VEHICLE_MISSION_PED_TARGET");
        }
        public static Task VehicleMission(this Ped ped, Vector3 target, MissionType missionType, float maxSpeed, VehicleDrivingFlags flags, float minDistance, float stoppingRange, bool againstTraffic)
            => VehicleMission(ped, ped.CurrentVehicle, target, missionType, maxSpeed, flags, minDistance, stoppingRange, againstTraffic);
        public static Task VehicleMission(this Ped ped, Vehicle vehicle, Vector3 target, MissionType missionType, float maxSpeed, VehicleDrivingFlags flags, float minDistance, float stoppingRange, bool againstTraffic)
        {
            Natives.TASK_VEHICLE_MISSION_COORS_TARGET(ped, vehicle, target.X, target.Y, target.Z, (int)missionType, maxSpeed, (int)flags, minDistance, stoppingRange, againstTraffic);
            return Task.GetTask(ped, "TASK_VEHICLE_MISSION_COORS_TARGET");
        }
        public static void StopEntityAnimation(this Entity entity, AnimationDictionary animDict, string animName) => Natives.STOP_ENTITY_ANIM(entity, animName, animDict.Name, 0);
        public static bool IsEntityPlayingAnim(this Entity entity, AnimationDictionary animDict, string animName) => Natives.IS_ENTITY_PLAYING_ANIM<bool>(entity, animDict.Name, animName, 3); //IS_ENTITY_PLAYING_ANIM       
    }
}
