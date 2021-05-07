using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using static Rage.Native.NativeFunction;

namespace BarbarianCall.Extensions
{
    public static class MathExtension
    {
        internal static Vector3 ConvertToDirection(this Vector3 vector3) => ConvertToDirection(vector3.ToRotator());
        internal static Vector3 ConvertToDirection(this Rotator rotator)
        {
            var rz = MathHelper.ConvertDegreesToRadians(rotator.Yaw);
            var rx = MathHelper.ConvertDegreesToRadians(rotator.Pitch);
            float absx = (float)Math.Abs(Math.Cos(rx));
            return new Vector3((float)(-Math.Sin(rz) * absx), (float)(Math.Cos(rz) * absx), (float)Math.Sin(rz));
        }
        internal static Rotator DirectionToRotator(this Vector3 direction)
        {
            direction.Normalize();
            float xx = MathHelper.ConvertRadiansToDegrees(Math.Atan2(direction.Z, direction.Y));
            float yy = 0;
            float zz = MathHelper.ConvertRadiansToDegrees(Math.Atan2(direction.X, direction.Y));
            return new Rotator(xx, yy, zz);
        }
        private static Vector3 WorldToScreenUsingGameplayCamera(Vector2 screeenCoord)
        {
            Vector3 camPosition = Natives.GET_GAMEPLAY_CAM_COORD<Vector3>();
            Rotator camRotation = Natives.GET_GAMEPLAY_CAM_ROT<Rotator>(2);

            Vector3 direction = camRotation.ConvertToDirection();
            Vector3 vector3 = camRotation.ToVector() + new Vector3(10, 0, 0);
            Vector3 vector31 = camRotation.ToVector() + new Vector3(-10, 0, 0);
            Vector3 vector32 = camRotation.ToVector() + new Vector3(0, 0, -10);
            Vector3 dir1 = ConvertToDirection(camRotation.ToVector() + new Vector3(0, 0, 10)) - ConvertToDirection(vector32);
            Vector3 dir2 = ConvertToDirection(vector3) - ConvertToDirection(vector31);
            float rad = -MathHelper.ConvertDegreesToRadians(camRotation.Pitch);
            Vector3 vector33 = (dir1 * (float)Math.Cos(rad)) - (dir2 * (float)Math.Sin(rad));
            Vector3 vector34 = (dir1 * (float)Math.Sin(rad)) + (dir2 * (float)Math.Cos(rad));
            if (!WorldToScreenRelative(camPosition + (direction * 10f) + vector33 + vector34, out Vector2 vector2))
            {
                return camPosition + (direction * 10f);
            }
            if (!WorldToScreenRelative(camPosition + (direction * 10f), out Vector2 vector21))
            {
                return camPosition + (direction * 10f);
            }
            if (Math.Abs(vector2.X - vector21.X) < 0.001f || Math.Abs(vector2.Y - vector21.Y) < 0.001f)
            {
                return camPosition + (direction * 10f);
            }
            float x = (screeenCoord.X - vector21.X) / (vector2.X - vector21.X);
            float y = (screeenCoord.Y - vector21.Y) / (vector2.Y - vector21.Y);
            return (camPosition + (direction * 10f)) + (vector33 * x) + (vector34 * y);
        }
        public static Vector3 RaycastGameplayCamForCoord(Vector2 screenCoord, float maxDistance, params Entity[] entityToIgnore)
        {
            Vector3 vector3 = Natives.GET_GAMEPLAY_CAM_COORD<Vector3>();
            Vector3 world = WorldToScreenUsingGameplayCamera(screenCoord);
            Vector3 vector31 = world - vector3;
            vector31.Normalize();
            var res = World.TraceLine(vector3 + (vector31 * 1f), vector3 + (vector31 * maxDistance), TraceFlags.IntersectWorld | TraceFlags.IntersectVehicles | TraceFlags.IntersectPedsSimpleCollision
                 | TraceFlags.IntersectPeds | TraceFlags.IntersectObjects | TraceFlags.IntersectFoliage, entityToIgnore);
            return res.HitPosition;
        }
        private static bool WorldToScreenRelative(Vector3 worldCoords, out Vector2 screenCoords)
        {
            if (!Natives.GET_SCREEN_COORD_FROM_WORLD_COORD<bool>(worldCoords.X, worldCoords.Y, worldCoords.Z, out screenCoords.X, out screenCoords.Y))
            {
                return false;
            }
            screenCoords.X = (screenCoords.X - 0.5f) * 2.0f;
            screenCoords.Y = (screenCoords.Y - 0.5f) * 2.0f;
            return true;
        }
        internal static float FloatDiff(this float first, float second) => Math.Abs(Math.Abs(first) - Math.Abs(second));
        internal static float HeightDiff(this ISpatial first, ISpatial second) => first.Position.Z.FloatDiff(second.Position.Z);
        internal static float HeightDiff(this Vector3 first, Vector3 second) => first.Z.FloatDiff(second.Z);
        internal static float HeightDiff(this ISpatial first, Vector3 second) => first.Position.Z.FloatDiff(second.Z);
        internal static float HeightDiff(this Vector3 first, ISpatial second) => first.Z.FloatDiff(second.Position.Z);
        internal static float HeadingDiff(this Entity first, Entity second) => first.Heading.FloatDiff(second.Heading);
        internal static float HeadingDiff(this float first, Entity second) => first.FloatDiff(second.Heading);
        internal static float HeadingDiff(this Entity first, float second) => first.Heading.FloatDiff(second);
    }
}
