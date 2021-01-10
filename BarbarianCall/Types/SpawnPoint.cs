namespace BarbarianCall.Types
{
    using System;
    using System.Globalization;
    using Rage;
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
        //public static implicit operator float[](SpawnPoint s) => new float[] { s.Position.X, s.Position.Y, s.Position.Z };
        public static implicit operator float(SpawnPoint s) => s.Heading;
    }
}
