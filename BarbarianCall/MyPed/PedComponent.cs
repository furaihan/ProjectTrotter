namespace BarbarianCall.MyPed
{
    using Rage.Native;
    public class PedComponent
    {
        public int DrawableID { get; set; }
        public int TextureID { get; set; }
        public int PalleteID { get; set; }
        public PedComponent(int drawable, int texture, int pallete = 0)
        {
            DrawableID = drawable;
            TextureID = texture;
            PalleteID = pallete;
        }
    }
}
