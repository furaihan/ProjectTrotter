﻿using Rage;
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
        public static Task PlayScenarioAction(this Ped ped, Types.PedScenario pedScenario, bool playEnterAnim)
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
        public static void StopEntityAnimation(this Entity entity, AnimationDictionary animDict, string animName) => Natives.STOP_ENTITY_ANIM(entity, animName, animDict.Name, 0);
        public static bool IsEntityPlayingAnim(this Entity entity, AnimationDictionary animDict, string animName) => Natives.x1F0B79228E461EC9<bool>(entity, animDict.Name, animName, 3); //IS_ENTITY_PLAYING_ANIM
        public enum EscortVehicleMode : int
        {
            Behind = -1,
            Ahead,
            Left,
            Right,
            BackLeft,
            BackRight
        }   
    }
}
