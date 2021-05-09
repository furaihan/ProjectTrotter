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
        public static Vector3 GameplayCameraPosition => Natives.GET_GAMEPLAY_CAM_COORD<Vector3>();
        public static Rotator GameplayCameraRotation => ((Vector3)Natives.GET_GAMEPLAY_CAM_ROT<Vector3>(2)).ToRotator();
        public static Vector3 GameplayCameraRotationVector => Natives.GET_GAMEPLAY_CAM_ROT<Vector3>(2);
        public static Quaternion GameplayCameraQuarternion => GameplayCameraRotation.ToQuaternion();
        public static float GameplayCameraHeading => Natives.GET_GAMEPLAY_CAM_RELATIVE_HEADING<float>();
        public static float GameplayCameraFOV => Natives.GET_GAMEPLAY_CAM_FOV<float>();
        internal static Vector3 RotationToDirection(this Rotator rotator) => RotationToDirection(rotator.ToVector());
        internal static Vector3 RotationToDirection(this Vector3 rotation)
        {
            var z = DegreeToRadian(rotation.Z);
            var x = DegreeToRadian(rotation.X);
            var num = Math.Abs(Math.Cos(x));
            return new Vector3
            {
                X = (float)(-Math.Sin(z) * num),
                Y = (float)(Math.Cos(z) * num),
                Z = (float)Math.Sin(x),
            };
        }
        internal static Rotator DirectionToRotator(this Vector3 direction)
        {
            direction.Normalize();
            float xx = RadianToDegree((float)Math.Atan2(direction.Z, direction.Y));
            float yy = 0;
            float zz = RadianToDegree((float)Math.Atan2(direction.X, direction.Y));
            return new Vector3(xx, yy, zz).ToRotator();
        }
        public static float DegreeToRadian(this float degree) => (float)(degree * (Math.PI / 180.0f));
        public static float RadianToDegree(this float radian) => (float)(radian * (180.0f / Math.PI));
        private static Vector3 ScreenToWorldUsingGameplayCamera(Vector2 screeenCoord)
        {
            Vector3 camPos = Natives.GET_GAMEPLAY_CAM_COORD<Vector3>();
            Vector3 camRot = Natives.GET_GAMEPLAY_CAM_ROT<Vector3>(2);

            Vector3 camForward = camRot.RotationToDirection();
            Vector3 rotUp = camRot + new Vector3(10, 0, 0);
            Vector3 rotDown = camRot + new Vector3(-10, 0, 0);
            Vector3 rotLeft = camRot + new Vector3(0, 0, -10);
            Vector3 rotRight = camRot + new Vector3(0, 0, -10);

            Vector3 camRight = RotationToDirection(rotRight) - RotationToDirection(rotLeft);
            Vector3 camUp = RotationToDirection(rotUp) - RotationToDirection(rotDown);

            float rollRad = -DegreeToRadian(camRot.Y);

            Vector3 camRightRoll = (camRight * (float)Math.Cos(rollRad)) - (camUp * (float)Math.Sin(rollRad));
            Vector3 camUpRoll = (camRight * (float)Math.Sin(rollRad)) + (camUp * (float)Math.Cos(rollRad));

            var point3D = camPos + (camForward * 10.0f) + camRightRoll + camUpRoll;
            if (!WorldToScreenRelative(point3D, out Vector2 point2D)) return camPos + (camForward * 10.0f);
            if (!WorldToScreenRelative(camPos + (camForward * 10f), out Vector2 point2DZero)) return camPos + (camForward * 10.0f);

            const double eps = 0.001;
            if (Math.Abs(point2D.X - point2DZero.X) < eps || Math.Abs(point2D.Y - point2DZero.Y) < eps) return camPos + (camForward * 10.0f);
            float x = (screeenCoord.X - point2DZero.X) / (point2D.X - point2DZero.X);
            float y = (screeenCoord.Y - point2DZero.Y) / (point2D.Y - point2DZero.Y);
            return camPos + (camForward * 10f) + (camRightRoll * x) + (camUpRoll * y);
        }
        public static Vector3 RaycastGameplayCamForCoord(Vector2 screenCoord, params Entity[] entityToIgnore)
        {
            const float raycastToDist = 100.0f;

            Vector3 source3D = Natives.GET_GAMEPLAY_CAM_COORD<Vector3>();
            Vector3 target3D = ScreenToWorldUsingGameplayCamera(screenCoord);
            Vector3 dir = target3D - source3D;
            dir.Normalize();

            if (entityToIgnore?.Contains(Game.LocalPlayer.Character) == true)
            {
                if (Game.LocalPlayer.Character.IsInAnyVehicle(false))
                {
                    entityToIgnore[Array.IndexOf(entityToIgnore, Game.LocalPlayer.Character)] = Game.LocalPlayer.Character.CurrentVehicle;
                }
            }

            var res = World.TraceLine(source3D,
                source3D + dir * raycastToDist, TraceFlags.IntersectWorld | TraceFlags.IntersectVehicles | TraceFlags.IntersectPedsSimpleCollision
                 | TraceFlags.IntersectPeds | TraceFlags.IntersectObjects | TraceFlags.IntersectFoliage, entityToIgnore);
            return res.Hit ? res.HitPosition : source3D + (dir * raycastToDist);
        }
        public static Vector3 GetGameplayCamRaycastCoord()
        {
            Camera camera = new(false)
            {
                Position = GameplayCameraPosition,
                Rotation = GameplayCameraRotation,
                FOV = GameplayCameraFOV,
                Heading = GameplayCameraHeading,               
            };
            if (World.ConvertScreenPositionToTrace(camera, new Vector2(0, 0), out Vector3 traceStart, out Vector3 traceDir))
            {
                Vector3 traceEnd = traceStart + traceDir * 1000f;
                HitResult hitResult = World.TraceLine(traceStart, traceEnd, TraceFlags.IntersectEverything, Game.LocalPlayer.Character);
                if (hitResult.Hit)
                {
                    if (camera.IsValid()) camera.Delete();
                    return hitResult.HitPosition;
                }
                if (camera.IsValid()) camera.Delete();
                return traceStart + (traceDir * 100f);
            }
            if (camera.IsValid()) camera.Delete();
            return Game.LocalPlayer.Character.Position;
        }
        private static bool WorldToScreenRelative(Vector3 worldCoords, out Vector2 screenCoords)
        {
            if (!Natives.GET_SCREEN_COORD_FROM_WORLD_COORD<bool>(worldCoords.X, worldCoords.Y, worldCoords.Z, out float screenCoordsX, out float screenCoordsY))
            {
                screenCoords = new Vector2();
                return false;
            }
            screenCoords.X = (screenCoordsX - 0.5f) * 2.0f;
            screenCoords.Y = (screenCoordsY - 0.5f) * 2.0f;
            return true;
        }
        /// <summary>
        /// Gets an offset position from this position in the given <paramref name="heading"/> direction by a given <paramref name="offset"/> amount.
        /// </summary>
        /// <remarks>
        /// Written by alexguirre. 
        /// </remarks>
        public static Vector3 GetOffset(this Vector3 from, float heading, Vector3 offset)
        {
            float radians = MathHelper.ConvertDegreesToRadians(heading);

            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            float resultX = offset.X * cos - offset.Y * sin;
            float resultY = offset.X * sin + offset.Y * cos;

            return new Vector3(from.X + resultX, from.Y + resultY, from.Z + offset.Z);
        }
        public static bool IsAheadPosition(this ISpatial spatial, ISpatial targetSpatial, Vector3 direction) => IsAheadPosition(spatial.Position, targetSpatial.Position, direction);
        public static bool IsAheadPosition(this Vector3 vector3, Vector3 targetVector, Vector3 direction)
        {
            direction.Normalize();
            float heading1 = MathHelper.ConvertDirectionToHeading(direction);
            float heading2 = targetVector.GetHeadingTowards(vector3);
            return Math.Abs(heading1 - heading2) <= 15f;
        }
        public static bool IsBehindPosition(this ISpatial spatial, ISpatial targetSpatial, Vector3 direction) => IsBehindPosition(spatial.Position, targetSpatial.Position, direction);
        public static bool IsBehindPosition(this Vector3 vector3, Vector3 targetVector, Vector3 direction)
        {
            direction.Normalize();
            float heading1 = MathHelper.ConvertDirectionToHeading(direction);
            float heading2 = targetVector.GetHeadingTowards(vector3);
            return (Math.Abs(heading1 - heading2) - 180) <= 15f;
        }
        internal static float FloatDiff(this float first, float second) => Math.Abs(first - second);
        internal static float HeightDiff(this ISpatial first, ISpatial second) => first.Position.Z.FloatDiff(second.Position.Z);
        internal static float HeightDiff(this Vector3 first, Vector3 second) => first.Z.FloatDiff(second.Z);
        internal static float HeightDiff(this ISpatial first, Vector3 second) => first.Position.Z.FloatDiff(second.Z);
        internal static float HeightDiff(this Vector3 first, ISpatial second) => first.Z.FloatDiff(second.Position.Z);
        internal static float HeadingDiff(this Entity first, Entity second) => first.Heading.FloatDiff(second.Heading);
        internal static float HeadingDiff(this float first, Entity second) => first.FloatDiff(second.Heading);
        internal static float HeadingDiff(this Entity first, float second) => first.Heading.FloatDiff(second);
    }
}
