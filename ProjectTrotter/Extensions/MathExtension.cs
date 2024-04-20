using System;
using System.Linq;
using Rage;
using static Rage.Native.NativeFunction;

namespace ProjectTrotter.Extensions
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
            var z = MathHelper.ConvertDegreesToRadians(rotation.Z);
            var x = MathHelper.ConvertDegreesToRadians(rotation.X);
            var num = Math.Abs(Math.Cos(x));
            return new Vector3
            {
                X = (float)(-Math.Sin(z) * num),
                Y = (float)(Math.Cos(z) * num),
                Z = (float)Math.Sin(x),
            };
        }
        internal static float ToHeading(this Rotator rotator) => MathHelper.ConvertDirectionToHeading(RotationToDirection(rotator));
        internal static Rotator DirectionToRotator(this Vector3 direction)
        {
            direction.Normalize();
            float xx = MathHelper.ConvertRadiansToDegrees((float)Math.Atan2(direction.Z, direction.Y));
            float yy = 0;
            float zz = MathHelper.ConvertRadiansToDegrees((float)Math.Atan2(direction.X, direction.Y));
            return new Vector3(xx, yy, zz).ToRotator();
        }
        /// <summary>
        /// Source: <a href="https://github.com/Guad/MissionCreator/blob/master/ContentCreatorMain/Util.cs#L156">Github</a>
        /// </summary>
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

            float rollRad = -MathHelper.ConvertDegreesToRadians(camRot.Y);

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
        private static bool WorldToScreenRelative(Vector3 worldCoords, out Vector2 screenCoords)
        {
            var output = World.ConvertWorldPositionToScreenPosition(worldCoords);
            output = new Vector2(output.X / Game.Resolution.Width, output.Y / Game.Resolution.Height);
            screenCoords = new Vector2((output.X - 0.5f) * 2, (output.Y - 0.5f) * 2);
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
        public static Vector3 AroundPosition(this Vector3 vector3, float minDistance, float maxDistance)
        {
            float x = MyRandom.NextDouble() > 0.5 ? GetRandomFloatInRange(minDistance, maxDistance) : -GetRandomFloatInRange(minDistance, maxDistance);
            float y = MyRandom.NextDouble() > 0.5 ? GetRandomFloatInRange(minDistance, maxDistance) : -GetRandomFloatInRange(minDistance, maxDistance);
            return vector3 + new Vector3(x, y, 0.0f);
        }
        public static float ToHeading(this Vector3 v) => (float)((Math.Atan2(v.X, -v.Y) + Math.PI) * (180.0 / Math.PI));
        public static Vector3 AroundPosition(this Vector3 v ,float radius)
        {
            return v + new Vector3(GetRandomFloatInRange(-radius, radius), GetRandomFloatInRange(-radius, radius), 0.0f);
        }
        public static Vector3 ForwardVector(this Vector3 vector, float yaw)
        {
            Vector3 right;
            float cos = (float)Math.Cos(yaw + Math.PI / 2.0f);
            right.X = (180f / (float)Math.PI) * cos;
            right.Y = 0f;
            float sin = (float)Math.Sin(yaw + Math.PI / 2.0f);
            right.Z = (180f / (float)Math.PI) * sin;
            return Vector3.Cross(vector, right);
        }
        public static bool IsHeadingTowards(this Entity entity, Entity otherEntity, float degreeTolerance)
        {
            float heading1 = entity.Heading;
            float heading2 = entity.GetHeadingTowards(otherEntity);
            return Math.Abs(heading1 - heading2) <= degreeTolerance;
        }
        public static float DistanceToSquared(this Vector3 vector3, Vector3 to) => Vector3.DistanceSquared(vector3, to);
        public static float DistanceToSquared(this ISpatial spatial, Vector3 to) => Vector3.DistanceSquared(spatial.Position, to);
        public static float DistanceToSquared(this Vector3 vector3, ISpatial spatial) => Vector3.DistanceSquared(vector3, spatial.Position);
        public static float DistanceToSquared(this ISpatial spatial, ISpatial to) => Vector3.DistanceSquared(spatial.Position, to.Position);
        public static float DistanceToSquared2D(this Vector3 vector3, Vector3 to)
        {
            Vector3 from2D = new(vector3.X, vector3.Y, 0.0f);
            Vector3 to2D = new(to.X, to.Y, 0.0f);
            return Vector3.DistanceSquared(from2D, to2D);
        }
        public static float DistanceToSquared2D(this ISpatial spatial, Vector3 to) => DistanceToSquared2D(spatial.Position, to);
        public static float DistanceToSquared2D(this Vector3 vector3, ISpatial spatial) => DistanceToSquared2D(vector3, spatial.Position);
        public static float DistanceToSquared2D(this ISpatial spatial, ISpatial to) => DistanceToSquared2D(spatial.Position, to.Position);
        internal static float GetRandomFloatInRange(float startRange, float endRange) => Natives.GET_RANDOM_FLOAT_IN_RANGE<float>(startRange, endRange);
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
