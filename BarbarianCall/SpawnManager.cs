using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using Rage.Native;

namespace BarbarianCall
{
    internal static class SpawnManager
    {       
        internal static bool GetClosestVehicleNodeWithheading(Vector3 pos, out Vector3 outpos, out float heading) => NativeFunction.Natives.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING<bool>(pos.X, pos.Y, pos.Z, out outpos, out heading, 0, 3, 0);
        internal static bool GetRoadSidePointWithHeading(this Vector3 pos, out Vector3 outPos, out float outHeading)
        {
            outPos = Vector3.Zero;
            outHeading = float.NaN;
            try
            {
                unsafe
                {
                    if (NativeFunction.Natives.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING<bool>(pos.X, pos.Y, pos.Z, out Vector3 nodePos, out float nodeHeading, 0, 3, 0)) //GetNTHClosestVehicleNodeWithHeadingFavourDirections
                    {
                        if (NativeFunction.Natives.xA0F8A7517A273C05<bool>(nodePos.X, nodePos.Y, nodePos.Z, nodeHeading, out Vector3 rsPos)) //GetRoadSidePointWithHeading
                        {
                            outPos = rsPos;
                            outHeading = nodeHeading;
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.ToString().ToLog();
            }
            return false;
        }
        internal static bool GetRoadSidePointFavourDirections(Vector3 pos, Vector3 favoredPos, out Vector3 outPos, out float heading)
        {
            heading = float.NaN;
            outPos = Vector3.Zero;
            try
            {
                if (NativeFunction.Natives.x45905BE8654AE067<bool>(pos.X, pos.Y, pos.Z, favoredPos.X, favoredPos.Y, favoredPos.Z, 1, out Vector3 nodePos, out float nodeHeading, 0, 0x40400000, 0)) //GetNTHClosestVehicleNodeFavourDirection
                {
                    if (NativeFunction.Natives.xA0F8A7517A273C05<bool>(nodePos.X, nodePos.Y, nodePos.Z, nodeHeading, out Vector3 rsp)) //GetRoadSidePointWithHeading
                    {
                        heading = nodeHeading;
                        outPos = rsp;
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                e.ToString().ToLog();
            }
            return false;
        }
        internal static SpawnPoint GetVehicleSpawnPoint(Vector3 pos, float minimalDistance, float maximumDistance)
        {
            DateTime start = DateTime.Now;
            for (int i = 0; i < 600; i++)
            {
                Vector3 v = pos.Around(minimalDistance, maximumDistance);
                if (i % 12 == 0) GameFiber.Yield();
                if (NativeFunction.Natives.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING<bool>(v.X, v.Y, v.Z, out Vector3 nodeP, out float nodeH, 5, 3.0, 0))
                {
                    if (nodeP.DistanceTo(pos) < minimalDistance || nodeP.DistanceTo(pos) > maximumDistance) continue;
                    if (nodeP.TravelDistanceTo(pos) < maximumDistance * 3f)
                    {
                        SpawnPoint ret = new SpawnPoint(nodeP, nodeH);
                        $"Vehicle Spawn found {ret}".ToLog();
                        $"Process took {(DateTime.Now - start).TotalMilliseconds} ms".ToLog();
                        return ret;
                    }
                }
            }
            return SpawnPoint.Zero;
        }
        internal static SpawnPoint GetPedSpawnPoint(Vector3 pos, float minimalDistance, float maximumDistance)
        {
            DateTime start = DateTime.Now;
            for (int i = 0; i < 600; i++)
            {
                Vector3 v = pos.Around(minimalDistance, maximumDistance);
                if (i % 15 == 0) GameFiber.Yield();
                if (NativeFunction.Natives.GET_SAFE_COORD_FOR_PED<bool>(v.X, v.Y, v.Z, true, out Vector3 nodeP, 0))
                {
                    if (nodeP.DistanceTo(pos) < minimalDistance || nodeP.DistanceTo(pos) > maximumDistance) continue;
                    if (nodeP.TravelDistanceTo(pos) < maximumDistance * 3f)
                    {
                        SpawnPoint ret = new SpawnPoint(nodeP, Extension.GetRandomAbsoluteSingle(1, 360));
                        $"Ped Spawn found {ret} Distance: {ret.DistanceTo(pos)}".ToLog();
                        $"{i} Process took {(DateTime.Now - start).TotalMilliseconds} ms".ToLog();
                        return ret;
                    }
                }
            }
            for (int i = 0; i < 600; i++)
            {
                Vector3 v = pos.Around(minimalDistance, maximumDistance + Extension.GetRandomAbsoluteSingle(1f, 5f));
                if (i % 30 == 0) GameFiber.Yield();
                if (NativeFunction.Natives.GET_SAFE_COORD_FOR_PED<bool>(v.X, v.Y, v.Z, false, out Vector3 nodeP, 0))
                {
                    if (nodeP.DistanceTo(pos) < minimalDistance || nodeP.DistanceTo(pos) > maximumDistance) continue;
                    if (nodeP.TravelDistanceTo(pos) < maximumDistance * 3f)
                    {
                        SpawnPoint ret = new SpawnPoint(nodeP, Extension.GetRandomAbsoluteSingle(1, 360));
                        $"Ped safe spawn found {ret} Distance: {ret.DistanceTo(pos)}".ToLog();
                        $"{i + 600} Process took {(DateTime.Now - start).TotalMilliseconds} ms".ToLog();
                        return ret;
                    }
                }
            }
            $"Safe coord not found".ToLog();
            $"1200 process took {(DateTime.Now - start).TotalMilliseconds} ms".ToLog();
            return SpawnPoint.Zero;
        }
    }
    public class SpawnPoint : ISpatial, IEquatable<SpawnPoint>, IEquatable<Vector3>, IFormattable
    {
        public Vector3 Position { get; set; }
        public float Heading { get; set; }
        public string Description { get; set; } = null;
        /// <summary>
        /// Gets a spawnpoint with position = <see cref="Vector3.Zero"/> and heading = 0f
        /// </summary>
        /// <value>Gets a spawnpoint with position = <see cref="Vector3.Zero"/> and heading = 0f</value>
        public static SpawnPoint Zero
        {
            get
            {
                return new SpawnPoint();
            }
        }
        public SpawnPoint GroundPosition
        {
            get
            {
                float? zpos = World.GetGroundZ(Position, true, true);
                Vector3 ground = Position;
                ground.Z = zpos ?? Position.Z;
                return new SpawnPoint(ground, Heading);
            }
        }
        public SpawnPoint()
        {
            Position = Vector3.Zero;
            Heading = 0f;
        }
        public SpawnPoint(Vector3 position, float heading)
        {
            Position = position;
            Heading = heading;
        }
        public SpawnPoint(Vector2 vector2, float z, float heading)
        {
            Position = new Vector3(vector2, z);
            Heading = heading;
        }
        public SpawnPoint(float[] values, float heading)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length != 3)
                throw new ArgumentOutOfRangeException("values", "There must be three and only three input values for Vector3.");
            Position = new Vector3(values[0], values[1], values[2]);
            Heading = heading;
        }
        public SpawnPoint(float value, float heading)
        {
            Position = new Vector3(value);
            Heading = heading;
        }

        public float DistanceTo(Vector3 position) => Position.DistanceTo(position);

        public float DistanceTo(ISpatial spatialObject) => Position.DistanceTo(spatialObject);

        public float DistanceTo2D(Vector3 position) => Position.DistanceTo2D(position);

        public float DistanceTo2D(ISpatial spatialObject)=> Position.DistanceTo2D(spatialObject);

        public float TravelDistanceTo(Vector3 position) => Position.TravelDistanceTo(position);

        public float TravelDistanceTo(ISpatial spatialObject) => Position.TravelDistanceTo(spatialObject);
        public override int GetHashCode()
        {
            unchecked
            {
                var hashVector = Position.GetHashCode();
                var hashHeading = Heading.GetHashCode();
                return hashVector ^ hashHeading;
            }
        }
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            } else if (obj.GetType() == typeof(Vector3))
            {
                return Equals((Vector3)obj);
            }
            return Equals((SpawnPoint)obj);
        }
        public bool Equals(SpawnPoint other) => Position == other.Position && Heading == other.Heading;

        public bool Equals(Vector3 other) => Position == other;

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "[X:{0} Y:{1} Z:{2}] Heading:{3}", Position.X, Position.Y, Position.Z, Heading);
        }
        public string ToString(string format)
        {
            if (format == null)
                return ToString();

            return string.Format(CultureInfo.CurrentCulture, "[X:{0} Y:{1} Z:{2}] Heading:{3}", Position.X.ToString(format, CultureInfo.CurrentCulture),
                Position.Y.ToString(format, CultureInfo.CurrentCulture), Position.Z.ToString(format, CultureInfo.CurrentCulture), Heading.ToString(format, CultureInfo.CurrentCulture));
        }
        public string ToString(IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "[X:{0} Y:{1} Z:{2}] Heading: {3}", Position.X, Position.Y, Position.Z, Heading);
        }
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return ToString(formatProvider);

            return string.Format(formatProvider, "[X:{0} Y:{1} Z:{2}] Heading:{3}", Position.X.ToString(format, formatProvider),
                Position.Y.ToString(format, formatProvider), Position.Z.ToString(format, formatProvider), Heading.ToString(format, formatProvider));
        }

        public static bool operator ==(SpawnPoint left, SpawnPoint right) => left.Position == right.Position && left.Heading == right.Heading;
        public static bool operator !=(SpawnPoint left, SpawnPoint right) => !(left.Position == right.Position && left.Heading == right.Heading);
        public static implicit operator Vector3(SpawnPoint s) => s.Position;
        public static implicit operator float[](SpawnPoint s) => new float[] {s.Position.X, s.Position.Y, s.Position.Z};
        public static implicit operator float (SpawnPoint s) => s.Heading;
    }
}
