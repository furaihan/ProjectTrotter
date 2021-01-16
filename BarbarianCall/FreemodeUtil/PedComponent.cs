namespace BarbarianCall.FreemodeUtil
{
    using System;
    using Rage;
    using Rage.Native;
    public class PedComponent
    {
        public EComponentID ComponentID { get; set; }
        public int DrawableID { get; set; }
        public int TextureID { get; set; }
        public int PalleteID { get; set; }
        public PedComponent(EComponentID componentID, int drawable, int texture, int pallete = 0)
        {
            ComponentID = componentID;
            DrawableID = drawable;
            TextureID = texture;
            PalleteID = pallete;
        }
        public static PedComponent GetPedComponent(FreemodePed ped, EComponentID componentID)
        {
            int draw = NativeFunction.Natives.GET_PED_DRAWABLE_VARIATION<int>(ped, componentID);
            int tex = NativeFunction.Natives.GET_PED_TEXTURE_VARIATION<int>(ped, componentID);
            int pal = NativeFunction.Natives.GET_PED_PALETTE_VARIATION<int>(ped, componentID);
            return new PedComponent(componentID, draw, tex, pal);
        }
        public static void SetPedComponent(FreemodePed ped, PedComponent pedComponent)
        {
            NativeFunction.Natives.SET_PED_COMPONENT_VARIATION(ped, (int)pedComponent.ComponentID, pedComponent.DrawableID, pedComponent.TextureID, pedComponent.PalleteID);
        }
        public enum EComponentID
        {
            Head,
            Mask,
            HairStyle,
            Torso,
            Leg,
            Parachute,
            Shoes,
            Accessories,
            UnderShirt,
            BodyArmor,
            Decal,
            Tops
        }
    }
}
