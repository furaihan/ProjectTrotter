using System;
using System.Drawing;

namespace BarbarianCall.Types
{
    public class VehicleColor : IEquatable<VehicleColor>
    {
        /// <summary>
        /// The primary color paint index
        /// </summary>
        public VehiclePaint PrimaryColor { get; private set; }
        /// <summary>
        /// The secondary color paint index
        /// </summary>
        public VehiclePaint SecondaryColor { get; private set; }
        /// <summary>
        /// The primary color using RGBA <see cref="Color"/>
        /// </summary>
        public Color PrimaryColorRGBA { get; private set; }
        /// <summary>
        /// The secondary color using RGBA <see cref="Color"/>
        /// </summary>
        public Color SecondaryColorRGBA { get; private set; }
        /// <summary>
        /// Gets the primary color name
        /// </summary>
        public string PrimaryColorName { get; private set; }
        /// <summary>
        /// Gets the secondary color name
        /// </summary>
        public string SecondaryColorName { get; private set; }
        public VehicleColor(VehiclePaint primary, VehiclePaint secondary)
        {
            PrimaryColor = primary;
            SecondaryColor = secondary;
            PrimaryColorRGBA = primary.GetColor();
            SecondaryColorRGBA = secondary.GetColor();
            PrimaryColorName = primary.GetName();
            SecondaryColorName = secondary.GetName();
        }
        public VehicleColor(VehiclePaint primary)
        {
            VehiclePaint secondary = primary;
            PrimaryColor = primary;
            SecondaryColor = secondary;
            PrimaryColorRGBA = primary.GetColor();
            SecondaryColorRGBA = secondary.GetColor();
            PrimaryColorName = primary.GetName();
            SecondaryColorName = secondary.GetName();
        }

        public bool Equals(VehicleColor other)
        {
            return other is not null && other.PrimaryColor == PrimaryColor && other.SecondaryColor == SecondaryColor;
        }
        public static implicit operator VehicleColor(VehiclePaint paint) => new(paint, paint);
    }
}
