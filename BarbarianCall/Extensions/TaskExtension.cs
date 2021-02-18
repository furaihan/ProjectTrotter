using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rage;
using Rage.Native;

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
        public static void PlayVehicleAnimation(this Vehicle vehicle, string animDict, string animName) => NativeFunction.Natives.TASK_VEHICLE_PLAY_ANIM(vehicle, animDict, animName);
        public static void PlayEntityAnim(this Entity entity, AnimationDictionary animDict, string animName, bool loop = false, bool stayInAnim = false)
        {
            animDict.LoadAndWait();
            if (entity) NativeFunction.Natives.PLAY_ENTITY_ANIM(entity, animName, animDict.Name, 4f, loop, stayInAnim, 0, 0f, 0);
        }
        public static void StopEntityAnimation(this Entity entity, AnimationDictionary animDict, string animName) => NativeFunction.Natives.STOP_ENTITY_ANIM(entity, animName, animDict.Name, 0);
        public static bool IsEntityPlayingAnim(this Entity entity, string animDict, string animName) => NativeFunction.Natives.x1F0B79228E461EC9<bool>(entity, animDict, animName, 3); //IS_ENTITY_PLAYING_ANIM
    }
}
