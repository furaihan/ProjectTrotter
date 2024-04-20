namespace ProjectTrotter.Types
{
    using System;
    using System.Text;
    using System.Globalization;
    using Rage;
    public class Spawnpoint : IEquatable<Spawnpoint>, IEquatable<Vector3>, IFormattable
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
            return ToString("G", CultureInfo.CurrentCulture);
        }
        public string ToString(string format)
        {
            return ToString(format, CultureInfo.CurrentCulture);
        }       
        public string ToString(string format, IFormatProvider formatProvider)
        {
            StringBuilder sb = new();
            string separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;
            sb.Append('<');
            sb.Append("X:");
            sb.Append(Position.X.ToString(format, formatProvider));
            sb.Append(separator);
            sb.Append(' ');
            sb.Append("Y:");
            sb.Append(Position.Y.ToString(format, formatProvider));
            sb.Append(separator);
            sb.Append(' ');
            sb.Append("Z:");
            sb.Append(Position.Z.ToString(format, formatProvider));
            sb.Append('>');
            sb.Append(' ');
            sb.Append("Heading:");
            sb.Append(' ');
            sb.Append(Heading.ToString(format, formatProvider));
            return sb.ToString();
        }

        public static bool operator ==(Spawnpoint left, Spawnpoint right) => left.Position == right.Position && left.Heading == right.Heading;
        public static bool operator !=(Spawnpoint left, Spawnpoint right) => !(left.Position == right.Position && left.Heading == right.Heading);
        public static bool operator ==(Spawnpoint left, Vector3 right) => left.Position == right;
        public static bool operator !=(Spawnpoint left, Vector3 right) => left.Position != right;
        public static implicit operator Vector3(Spawnpoint s) => s.Position;
        //public static implicit operator float[](SpawnPoint s) => new float[] { s.Position.X, s.Position.Y, s.Position.Z };
        public static implicit operator float(Spawnpoint s) => s.Heading;
    }
}
