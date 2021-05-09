namespace BarbarianCall.Types
{
    using System;
    using System.Globalization;
    using Rage;
    using static Rage.Native.NativeFunction;
    public class Spawnpoint : ISpatial, IEquatable<Spawnpoint>, IEquatable<Vector3>, IFormattable
    {
        public Vector3 Position { get; set; }
        public float Heading { get; set; }
        public string Description { get; set; } = null;
        /// <summary>
        /// Gets a spawnpoint with position = <see cref="Vector3.Zero"/> and heading = 0f
        /// </summary>
        /// <value>Gets a spawnpoint with position = <see cref="Vector3.Zero"/> and heading = 0f</value>
        public static Spawnpoint Zero
        {
            get
            {
                return new Spawnpoint();
            }
        }
        public Spawnpoint GroundPosition
        {
            get
            {
                float? zpos = World.GetGroundZ(Position, true, true);
                Vector3 ground = Position;
                ground.Z = zpos ?? Position.Z;
                return new Spawnpoint(ground, Heading);
            }
        }
        public Spawnpoint()
        {
            Position = Vector3.Zero;
            Heading = 0f;
        }
        public Spawnpoint(Vector3 position, float heading)
        {
            Position = position;
            Heading = heading;
        }
        public Spawnpoint(params float[] values) 
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length != 4)
                throw new ArgumentOutOfRangeException("values", "first three value must be representing a new Vector3 and the last value is the heading");
            Position = new Vector3(values[0], values[1], values[2]);
            Heading = values[3];
        }

        public float DistanceTo(Vector3 position) => Position.DistanceTo(position);

        public float DistanceTo(ISpatial spatialObject) => Position.DistanceTo(spatialObject.Position);

        public float DistanceTo2D(Vector3 position) => Position.DistanceTo2D(position);

        public float DistanceTo2D(ISpatial spatialObject) => Position.DistanceTo2D(spatialObject);

        public float TravelDistanceTo(Vector3 position) => Position.TravelDistanceTo(position);

        public float TravelDistanceTo(ISpatial spatialObject) => Position.TravelDistanceTo(spatialObject);
        public override int GetHashCode()
        {
            unchecked
            {
                int hashVector = Position.GetHashCode();
                int hashHeading = Heading.GetHashCode();
                return hashVector ^ hashHeading;
            }
        }
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }
            else if (obj.GetType() == typeof(Vector3))
            {
                return Equals((Vector3)obj);
            }
            return Equals((Spawnpoint)obj);
        }
        public bool Equals(Spawnpoint other) => Position == other.Position && Heading == other.Heading;

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

        public static bool operator ==(Spawnpoint left, Spawnpoint right) => left.Position == right.Position && left.Heading == right.Heading;
        public static bool operator !=(Spawnpoint left, Spawnpoint right) => !(left.Position == right.Position && left.Heading == right.Heading);
        public static implicit operator Vector3(Spawnpoint s) => s.Position;
        //public static implicit operator float[](SpawnPoint s) => new float[] { s.Position.X, s.Position.Y, s.Position.Z };
        public static implicit operator float(Spawnpoint s) => s.Heading;
    }
}
