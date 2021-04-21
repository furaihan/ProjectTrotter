using Rage;
using Rage.Native;
using System;

namespace BarbarianCall.Extensions
{
    internal static class TaskExtension
    {
        public static Task LookAtEntity(this Ped ped, Entity lookAt, int duration)
        {
            NativeFunction.Natives.TASK_LOOK_AT_ENTITY(ped, lookAt, duration, 2048, 3);
            return Task.GetTask(ped, "TASK_LOOK_AT_ENTITY");
        }
        public static Task LookAtCoord(this Ped ped, Vector3 target, int duration)
        {
            NativeFunction.Natives.TASK_LOOK_AT_COORD(ped, target.X, target.Y, target.Z, duration, 0, 2);
            return Task.GetTask(ped, "TASK_LOOK_AT_COORD");
        }
        public static Task ShootAtCoord(this Ped ped, Vector3 coordToShoot, int duration, FiringPattern firingPattern)
        {
            NativeFunction.Natives.TASK_SHOOT_AT_COORD(ped, coordToShoot.X, coordToShoot.Y, coordToShoot.Z, duration, (uint)firingPattern);
            return Task.GetTask(ped, "TASK_SHOOT_AT_COORD");
        }
        public static Task ChatTo(this Ped ped, Ped target) 
        {
            NativeFunction.Natives.TASK_CHAT_TO_PED(ped, target, 16, 0f, 0f, 0f, 0f, 0f);
            return Task.GetTask(ped, "TASK_CHAT_TO_PED");
        }
        public static Task DriveTo(this Ped ped, Vector3 target, float radius, float speed, VehicleDrivingFlags drivingStyle) => DriveTo(ped, ped.CurrentVehicle, target, radius, speed, drivingStyle);
        public static Task DriveTo(this Ped ped, Vehicle vehicle, Vector3 target, float radius, float speed, VehicleDrivingFlags drivingStyle)
        {
            NativeFunction.Natives.TASK_VEHICLE_DRIVE_TO_COORD_LONGRANGE(ped, vehicle, target.X, target.Y, target.Z, speed, (int)drivingStyle, radius);
            return Task.GetTask(ped, "TASK_VEHICLE_DRIVE_TO_COORD_LONGRANGE");
        }
        public static Task ParkVehicle(this Ped ped, Vector3 position, float heading, float radius = 20f, bool keepEngineOn = false) => ParkVehicle(ped, ped.CurrentVehicle, position, heading, radius, keepEngineOn);
        public static Task ParkVehicle(this Ped ped, Vehicle vehicle, Vector3 position, float heading, float radius = 20f, bool keepEngineOn = false)
        {
            int mode = vehicle.HeadingDiff(heading) < 90 ? 1 : 2;
            NativeFunction.Natives.TASK_VEHICLE_PARK(ped, vehicle, position.X, position.Y, position.Z, heading, mode, radius, keepEngineOn);
            return Task.GetTask(ped, "TASK_VEHICLE_PARK");
        }
        public static Task FollowToOfsettOfEntity(this Ped ped, Entity target, Vector3 offset, float speed, float stoppingRange = 10.0f, bool persistFollowing = true)
        {
            NativeFunction.Natives.TASK_FOLLOW_TO_OFFSET_OF_ENTITY(ped, target, offset.X, offset.Y, offset.Z, speed, -1, stoppingRange, persistFollowing);
            return Task.GetTask(ped, "TASK_FOLLOW_TO_OFFSET_OF_ENTITY");
        }
        public static Task HeliChase(this Ped ped, Entity target, Vector3 offset)
        {
            NativeFunction.Natives.TASK_HELI_CHASE(ped, target, offset.X, offset.Y, offset.Z);
            return Task.GetTask(ped, "TASK_HELI_CHASE");
        }
        public static Task WanderWithVehicle(this Ped ped, float speed, VehicleDrivingFlags drivingStyle) => WanderWithVehicle(ped, ped.CurrentVehicle, speed, drivingStyle);
        public static Task WanderWithVehicle(this Ped ped, Vehicle vehicle, float speed, VehicleDrivingFlags drivingStyle)
        {
            NativeFunction.Natives.TASK_VEHICLE_DRIVE_WANDER(ped, vehicle, speed, (int)drivingStyle);
            return Task.GetTask(ped, "TASK_VEHICLE_DRIVE_WANDER");
        }
        public static Task GotoCoordAndAimAtHatedEntitiesNearCoord(this Ped ped, Vector3 destination, Vector3 aimAt, float speed, FiringPattern firingPattern, bool shoot, 
            float distanceStop = 0.0f, float noRoadsDistance = 0.0f)
        {
            NativeFunction.Natives.TASK_GO_TO_COORD_AND_AIM_AT_HATED_ENTITIES_NEAR_COORD(ped, destination.X, destination.Y, destination.Z, aimAt.X, aimAt.Y, aimAt.Z, speed, shoot,
                distanceStop, noRoadsDistance, true, 0, -957453492, (uint)firingPattern);
            return Task.GetTask(ped, "TASK_GO_TO_COORD_AND_AIM_AT_HATED_ENTITIES_NEAR_COORD");
        }
        public static Task CombatAgainstHatedTargetAroundPed(this Ped ped, float radius)
        {
            NativeFunction.Natives.TASK_COMBAT_HATED_TARGETS_AROUND_PED(ped, radius);
            return Task.GetTask(ped, "TASK_COMBAT_HATED_TARGETS_AROUND_PED");
        }
        public static Task CombatAgainstHatedTargetInArea(this Ped ped, Vector3 area, float radius)
        {
            NativeFunction.Natives.TASK_COMBAT_HATED_TARGETS_IN_AREA(ped, area.X, area.Y, area.Z, radius, 0);
            return Task.GetTask(ped, "TASK_COMBAT_HATED_TARGETS_IN_AREA");
        }
        public static Task DriveVehicleWithNavigationMesh(this Ped ped, Vehicle vehicle, Vector3 destination, VehicleDrivingFlags flags, float speed = 30.0f)
        {
            NativeFunction.Natives.TASK_VEHICLE_GOTO_NAVMESH(ped, vehicle, destination.X, destination.Y, destination.Z, speed, (int)flags, 5.0f);
            return Task.GetTask(ped, "TASK_VEHICLE_GOTO_NAVMESH");
        }
        public static Task EscortVehicle(this Ped ped, Vehicle targetVehicle, EscortVehicleMode mode, float speed, VehicleDrivingFlags drivingFlags, float minDistance, float noRoadDistance)
            => EscortVehicle(ped, ped.CurrentVehicle, targetVehicle, mode, speed, drivingFlags, minDistance, noRoadDistance);
        public static Task EscortVehicle(this Ped ped, Vehicle vehicleUsedbyPed, Vehicle vehicleToEscort, EscortVehicleMode mode, float speed, VehicleDrivingFlags drivingFlags, float minDistance, float noRoadDistance)
        {
            NativeFunction.Natives.TASK_VEHICLE_ESCORT(ped, vehicleUsedbyPed, vehicleToEscort, (int)mode, speed, (int)drivingFlags, minDistance, -1, noRoadDistance);
            return Task.GetTask(ped, "TASK_VEHICLE_ESCORT");
        }
        public static Task DriveVehicleWithNavigationMesh(this Ped ped, Vector3 destination, VehicleDrivingFlags flags, float speed = 30.0f) => 
            DriveVehicleWithNavigationMesh(ped, ped.CurrentVehicle, destination, flags, speed);
        public static Task FaceTo(this Ped ped, Entity target, int duration)
        {
            NativeFunction.Natives.TASK_TURN_PED_TO_FACE_ENTITY(ped, target, duration);
            return Task.GetTask(ped, "TASK_TURN_PED_TO_FACE_ENTITY");
        }
        public static void PlayVehicleAnimation(this Vehicle vehicle, string animDict, string animName) => NativeFunction.Natives.TASK_VEHICLE_PLAY_ANIM(vehicle, animDict, animName);
        public static void PlayEntityAnim(this Entity entity, AnimationDictionary animDict, string animName, bool loop = false, bool stayInAnim = false)
        {
            animDict.LoadAndWait();
            if (entity) NativeFunction.Natives.PLAY_ENTITY_ANIM(entity, animName, animDict.Name, 4f, loop, stayInAnim, 0, 0f, 0);
        }
        public static void StopEntityAnimation(this Entity entity, AnimationDictionary animDict, string animName) => NativeFunction.Natives.STOP_ENTITY_ANIM(entity, animName, animDict.Name, 0);
        public static bool IsEntityPlayingAnim(this Entity entity, string animDict, string animName) => NativeFunction.Natives.x1F0B79228E461EC9<bool>(entity, animDict, animName, 3); //IS_ENTITY_PLAYING_ANIM
        public enum EscortVehicleMode : int
        {
            Behind = -1,
            Ahead,
            Left,
            Right,
            BackLeft,
            BackRight
        }
        [Flags]
        public enum VehicleDrivingStyleFlags : uint
        {
            None = 0,
            FollowTrafficRule = 1,
            YieldToPeds = 2,
            AvoidVehicle = 4,
            AvoidPeds = 8,
            AvoidObjects = 16,
            Unk1 = 32,
            Unk2 = 64,
            StopAtTrafficLights = 128,
            UseBlinker = 256 ,
            AllowWrongWay = 512,
            Backwards = 1024,
            Unk3 = 2048,
            TakeShortestPath = 262144,
            AvoidOffroad = 524288, //??
            IgnoreRoads = 4194304,
            StraightToDestination = 16777216,
            AvoidHighways = 536870912,
        }
    }
}
