namespace ProjectTrotter.MyPed
{
    using Rage.Native;
    /// <summary>
    /// Class to represent a Ped Component Variation, each <see cref="Rage.Ped"/> has a different set of components and variations for each body part.
    /// </summary>
    public class PedComponentVariation
    {
        /// <summary>
        /// Gets or sets the component drawable.
        /// </summary>
        /// <remarks>
        /// An integer representing the clothing model for specific <see cref="PedPropType"/>
        /// </remarks>
        public int Drawable { get; set; }
        /// <summary>
        /// Gets or sets the component texture.
        /// </summary>
        /// <remarks>
        /// An integer representing the texture for the clothing model for specific <see cref="Drawable"/>
        /// </remarks>
        public int Texture { get; set; }
        public int Pallete { get; set; }
        public PedComponentVariation(int drawable, int texture, int pallete = 0)
        {
            Drawable = drawable;
            Texture = texture;
            Pallete = pallete;
        }
    }
}
