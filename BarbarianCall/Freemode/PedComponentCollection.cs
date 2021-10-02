using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using BarbarianCall.Extensions;

namespace BarbarianCall.Freemode
{
    public class PedComponentCollection
    {
        private readonly FreemodePed _owner;
        internal PedComponentCollection(FreemodePed owner)
        {
            _owner = owner;
        }
        public PedComponent Torso
        {
            get
            {
                return PedComponent.GetPedComponent(_owner, PedComponent.EComponentID.Torso);
            }
            set
            {
                if (value.ComponentID != PedComponent.EComponentID.Torso) Peralatan.ToLog("Wrong componentID");
                else
                {
                    PedComponent.SetPedComponent(_owner, value);
                }
            }
        }
        public PedComponent Head
        {
            get
            {
                return PedComponent.GetPedComponent(_owner, PedComponent.EComponentID.Head);
            }
            set
            {
                if (value.ComponentID != PedComponent.EComponentID.Head) Peralatan.ToLog("Wrong componentID");
                else
                {
                    PedComponent.SetPedComponent(_owner, value);
                }
            }
        }
        public PedComponent Mask
        {
            get
            {
                return PedComponent.GetPedComponent(_owner, PedComponent.EComponentID.Mask);
            }
            set
            {
                if (value.ComponentID != PedComponent.EComponentID.Mask) Peralatan.ToLog("Wrong componentID");
                else
                {
                    PedComponent.SetPedComponent(_owner, value);
                }
            }
        }
        public PedComponent Shoes
        {
            get
            {
                return PedComponent.GetPedComponent(_owner, PedComponent.EComponentID.Shoes);
            }
            set
            {
                if (value.ComponentID != PedComponent.EComponentID.Shoes) Peralatan.ToLog("Wrong componentID");
                else
                {
                    PedComponent.SetPedComponent(_owner, value);
                }
            }
        }
        public PedComponent Leg
        {
            get
            {
                return PedComponent.GetPedComponent(_owner, PedComponent.EComponentID.Leg);
            }
            set
            {
                if (value.ComponentID != PedComponent.EComponentID.Leg) Peralatan.ToLog("Wrong componentID");
                else
                {
                    PedComponent.SetPedComponent(_owner, value);
                }
            }
        }
        public PedComponent Parachute
        {
            get
            {
                return PedComponent.GetPedComponent(_owner, PedComponent.EComponentID.Parachute);
            }
            set
            {
                if (value.ComponentID != PedComponent.EComponentID.Parachute) Peralatan.ToLog("Wrong componentID");
                else
                {
                    PedComponent.SetPedComponent(_owner, value);
                }
            }
        }
        public PedComponent Decal
        {
            get
            {
                return PedComponent.GetPedComponent(_owner, PedComponent.EComponentID.Decal);
            }
            set
            {
                if (value.ComponentID != PedComponent.EComponentID.Decal) Peralatan.ToLog("Wrong componentID");
                else
                {
                    PedComponent.SetPedComponent(_owner, value);
                }
            }
        }
        public PedComponent UnderShirt
        {
            get
            {
                return PedComponent.GetPedComponent(_owner, PedComponent.EComponentID.UnderShirt);
            }
            set
            {
                if (value.ComponentID != PedComponent.EComponentID.UnderShirt) Peralatan.ToLog("Wrong componentID");
                else
                {
                    PedComponent.SetPedComponent(_owner, value);
                }
            }
        }
        public PedComponent HairStyle
        {
            get
            {
                return PedComponent.GetPedComponent(_owner, PedComponent.EComponentID.HairStyle);
            }
            set
            {
                if (value.ComponentID != PedComponent.EComponentID.HairStyle) Peralatan.ToLog("Wrong componentID");
                else
                {
                    PedComponent.SetPedComponent(_owner, value);
                }
            }
        }
        public PedComponent BodyArmor
        {
            get
            {
                return PedComponent.GetPedComponent(_owner, PedComponent.EComponentID.BodyArmor);
            }
            set
            {
                if (value.ComponentID != PedComponent.EComponentID.BodyArmor) Peralatan.ToLog("Wrong componentID");
                else
                {
                    PedComponent.SetPedComponent(_owner, value);
                }
            }
        }
        public PedComponent Tops
        {
            get
            {
                return PedComponent.GetPedComponent(_owner, PedComponent.EComponentID.Tops);
            }
            set
            {
                if (value.ComponentID != PedComponent.EComponentID.Tops) Peralatan.ToLog("Wrong componentID");
                else
                {
                    PedComponent.SetPedComponent(_owner, value);
                }
            }
        }
        public PedComponent Accessories
        {
            get
            {
                return PedComponent.GetPedComponent(_owner, PedComponent.EComponentID.Accessories);
            }
            set
            {
                if (value.ComponentID != PedComponent.EComponentID.Accessories) Peralatan.ToLog("Wrong componentID");
                else
                {
                    PedComponent.SetPedComponent(_owner, value);
                }
            }
        }
        public void ApplyToAnotherPed(FreemodePed target, bool includingHairStyle = false)
        {
            target.Wardrobe.Accessories = Accessories;
            target.Wardrobe.BodyArmor = BodyArmor;
            target.Wardrobe.Decal = Decal;
            if (includingHairStyle) target.Wardrobe.HairStyle = HairStyle;
            target.Wardrobe.Leg = Leg;
            target.Wardrobe.Mask = Mask;
            target.Wardrobe.Parachute = Parachute;
            target.Wardrobe.Shoes = Shoes;
            target.Wardrobe.Tops = Tops;
            target.Wardrobe.Torso = Torso;
            target.Wardrobe.UnderShirt = UnderShirt;
        }
        public void CopyFromPed(FreemodePed ped, bool includeHairstyle = false)
        {
            $"Aks = {Accessories} Aks = {ped.Wardrobe.Accessories} Ped exist: {ped.Exists()}".ToLog();
            Accessories = ped.Wardrobe.Accessories;
            BodyArmor = ped.Wardrobe.BodyArmor;
            Decal = ped.Wardrobe.Decal;
            if (includeHairstyle) HairStyle = ped.Wardrobe.HairStyle;
            Leg = ped.Wardrobe.Leg;
            Mask = ped.Wardrobe.Mask;
            Parachute = ped.Wardrobe.Parachute;
            Shoes = ped.Wardrobe.Shoes;
            Tops = ped.Wardrobe.Tops;
            Torso = ped.Wardrobe.Torso;
            UnderShirt = ped.Wardrobe.UnderShirt;
        }
    }
}
