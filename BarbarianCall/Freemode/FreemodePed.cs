using System;
using System.Linq;
using Rage;
using LSPD_First_Response;
using HB = BarbarianCall.Freemode.HeadBlend;

namespace BarbarianCall.Freemode
{
    public class FreemodePed : Ped
    {
        public HeadBlendData HeadBlendData
        {
            get
            {
                return HB.GetDataFromPed(this);
            }
            set
            {
                HB.SetPedHeadBlendData(this, value);
            }
        }
        public EyeColor EyeColor 
        { 
            get
            {
                return HB.GetPedEyeColor(this);
            } 
            set
            {
                HB.SetPedEyeColor(this, value);
            }
        }
        public Gender Gender 
        { 
            get 
            {
                return Model.Hash == 0x705E61F2 ? Gender.Male : Gender.Female;
            } 
        }
        #region COMPONENT
        public PedComponent Torso
        {
            get
            {
                return PedComponent.GetPedComponent(this, PedComponent.EComponentID.Torso);
            }
            set
            {
                if (value.ComponentID != PedComponent.EComponentID.Torso) Peralatan.ToLog("Wrong componentID");
                else
                {
                    PedComponent.SetPedComponent(this, value);
                }
            }
        }
        public PedComponent Head
        {
            get
            {
                return PedComponent.GetPedComponent(this, PedComponent.EComponentID.Head);
            }
            set
            {
                if (value.ComponentID != PedComponent.EComponentID.Head) Peralatan.ToLog("Wrong componentID");
                else
                {
                    PedComponent.SetPedComponent(this, value);
                }
            }
        }
        public PedComponent Mask
        {
            get
            {
                return PedComponent.GetPedComponent(this, PedComponent.EComponentID.Mask);
            }
            set
            {
                if (value.ComponentID != PedComponent.EComponentID.Mask) Peralatan.ToLog("Wrong componentID");
                else
                {
                    PedComponent.SetPedComponent(this, value);
                }
            }
        }
        public PedComponent Shoes
        {
            get
            {
                return PedComponent.GetPedComponent(this, PedComponent.EComponentID.Shoes);
            }
            set
            {
                if (value.ComponentID != PedComponent.EComponentID.Shoes) Peralatan.ToLog("Wrong componentID");
                else
                {
                    PedComponent.SetPedComponent(this, value);
                }
            }
        }
        public PedComponent Leg
        {
            get
            {
                return PedComponent.GetPedComponent(this, PedComponent.EComponentID.Leg);
            }
            set
            {
                if (value.ComponentID != PedComponent.EComponentID.Leg) Peralatan.ToLog("Wrong componentID");
                else
                {
                    PedComponent.SetPedComponent(this, value);
                }
            }
        }
        public PedComponent Parachute
        {
            get
            {
                return PedComponent.GetPedComponent(this, PedComponent.EComponentID.Parachute);
            }
            set
            {
                if (value.ComponentID != PedComponent.EComponentID.Parachute) Peralatan.ToLog("Wrong componentID");
                else
                {
                    PedComponent.SetPedComponent(this, value);
                }
            }
        }
        public PedComponent Decal
        {
            get
            {
                return PedComponent.GetPedComponent(this, PedComponent.EComponentID.Decal);
            }
            set
            {
                if (value.ComponentID != PedComponent.EComponentID.Decal) Peralatan.ToLog("Wrong componentID");
                else
                {
                    PedComponent.SetPedComponent(this, value);
                }
            }
        }
        public PedComponent UnderShirt
        {
            get
            {
                return PedComponent.GetPedComponent(this, PedComponent.EComponentID.UnderShirt);
            }
            set
            {
                if (value.ComponentID != PedComponent.EComponentID.UnderShirt) Peralatan.ToLog("Wrong componentID");
                else
                {
                    PedComponent.SetPedComponent(this, value);
                }
            }
        }
        public PedComponent HairStyle
        {
            get
            {
                return PedComponent.GetPedComponent(this, PedComponent.EComponentID.HairStyle);
            }
            set
            {
                if (value.ComponentID != PedComponent.EComponentID.HairStyle) Peralatan.ToLog("Wrong componentID");
                else
                {
                    PedComponent.SetPedComponent(this, value);
                }
            }
        }
        public PedComponent BodyArmor
        {
            get
            {
                return PedComponent.GetPedComponent(this, PedComponent.EComponentID.BodyArmor);
            }
            set
            {
                if (value.ComponentID != PedComponent.EComponentID.BodyArmor) Peralatan.ToLog("Wrong componentID");
                else
                {
                    PedComponent.SetPedComponent(this, value);
                }
            }
        }
        public PedComponent Tops
        {
            get
            {
                return PedComponent.GetPedComponent(this, PedComponent.EComponentID.Tops);
            }
            set
            {
                if (value.ComponentID != PedComponent.EComponentID.Tops) Peralatan.ToLog("Wrong componentID");
                else
                {
                    PedComponent.SetPedComponent(this, value);
                }
            }
        }
        public PedComponent Accessories
        {
            get
            {
                return PedComponent.GetPedComponent(this, PedComponent.EComponentID.Accessories);
            }
            set
            {
                if (value.ComponentID != PedComponent.EComponentID.Accessories) Peralatan.ToLog("Wrong componentID");
                else
                {
                    PedComponent.SetPedComponent(this, value);
                }
            }
        }
        #endregion
        public FreemodePed(Vector3 position, float heading, Gender gender) : base(gender == Gender.Male ? 0x705E61F2/*mp_m_freemode_01*/ : 0x9C9EFFD8/*mp_f_freemode_01*/, position, heading)
        {
            MakePersistent();
            GameFiber.Wait(25);
            RandomizeFaceShape();
            Metadata.BAR_FreemodePed = true;
            Metadata.BAR_Entity = true;
        }
        public FreemodePed(Vector3 position, Gender gender) : base(gender == Gender.Male ? 0x705E61F2/*mp_m_freemode_01*/ : 0x9C9EFFD8/*mp_f_freemode_01*/, position, 0f)
        {
            MakePersistent();
            GameFiber.Wait(25);
            RandomizeFaceShape();
            Metadata.BAR_FreemodePed = true;
            Metadata.BAR_Entity = true;
        }
        public void RandomizeFaceShape()
        {
            Random random = new Random((int)Game.GetHashKey(Guid.NewGuid().ToString("X")));
            int[] maleSkinSecondID = { 0, 1, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 2, 20, 3, 4, 42, 43, 44, 5, 6, 7, 8, 9 };
            int[] femaleSecondID = { 0, 1, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 2, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 3, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 4, 40, 41, 45, 5, 6, 7, 8, 9 };
            int[] maleHairModel = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 24, 30, 31, 35,
                36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,65,66,68,70,71,72,73,74 };
            int[] femaleHairModel = {1, 2,3,4,5,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,28,30,31,32,36,37,38,39,40,
                41,42,45,46,47,48,49,50,52,53,54,55,56,57,58,59,60,61,65,73,78,74,77,76 };
            HeadBlendData = Gender == Gender.Male ?
                new HeadBlendData(random.Next(44), random.Next(44), 0, random.Next(44), maleSkinSecondID.GetRandomElement(), 0, (float)Math.Round(random.NextDouble(), 5), (float)Math.Round(random.NextDouble(), 5), 0.0f, false) :
                new HeadBlendData(random.Next(21, 45), femaleSecondID.GetRandomElement(), 0, random.Next(21, 45), femaleSecondID.GetRandomElement(), 0, (float)Math.Round(random.NextDouble(), 5), (float)Math.Round(random.NextDouble(), 5), 0.0f, false);
            OverlayId[] oIds = (OverlayId[])Enum.GetValues(typeof(OverlayId));
            var randomizedOIds = oIds.GetRandomNumberOfElements(random.Next(2, oIds.Length));
            FaceFeature[] faces = (FaceFeature[])Enum.GetValues(typeof(FaceFeature));
            var randomizedFaces = faces.GetRandomNumberOfElements(random.Next(3, faces.Length));
            if (Gender == Gender.Male)
            {
                HairStyle = new PedComponent(PedComponent.EComponentID.HairStyle, maleHairModel.GetRandomElement(true), 0, 0);
                int hairColor = random.Next() % 2 == 0 ? 0 : random.Next(1, 68);
                HB.SetPedHairColor(this, hairColor, hairColor);
                foreach (var olay in randomizedOIds)
                {
                    int index = olay switch
                    {
                        OverlayId.Blemishes => random.Next(HB.GetNumHeadOverlayValues(OverlayId.Blemishes) + 1),
                        OverlayId.FacialHair => random.Next(HB.GetNumHeadOverlayValues(OverlayId.FacialHair) + 1),
                        OverlayId.Eyebrows => random.Next(HB.GetNumHeadOverlayValues(OverlayId.Eyebrows) + 1),
                        OverlayId.Ageing => random.Next(HB.GetNumHeadOverlayValues(OverlayId.Ageing) + 1),
                        //OverlayId.Makeup => 255,
                        //OverlayId.Blush => 255,
                        OverlayId.Complexion => random.Next(HB.GetNumHeadOverlayValues(OverlayId.Complexion) + 1),
                        OverlayId.SunDamage => random.Next(HB.GetNumHeadOverlayValues(OverlayId.SunDamage) + 1),
                        //OverlayId.Lipstick => 255,
                        OverlayId.Freckles => random.Next(HB.GetNumHeadOverlayValues(OverlayId.Freckles) + 1),
                        OverlayId.ChestHair => random.Next(HB.GetNumHeadOverlayValues(OverlayId.ChestHair) + 1),
                        OverlayId.BodyBlemishes => random.Next(HB.GetNumHeadOverlayValues(OverlayId.BodyBlemishes) + 1),
                        OverlayId.AddBodyBlemishes => random.Next(HB.GetNumHeadOverlayValues(OverlayId.AddBodyBlemishes) + 1),
                        _ => 255
                    };
                    index = random.NextDouble() > 0.958475 ? 255 : index;
                    HB.SetPedHeadOverlay(this, olay, index, (float)Math.Round(random.NextDouble(), 1));
                    switch (olay)
                    {
                        case OverlayId.FacialHair:
                        case OverlayId.Eyebrows:
                        case OverlayId.ChestHair:
                            HB.SetPedHeadOverlayColor(this, olay, ColorType.EyebrowBeardChestHair, 0, 0);
                            break;
                        default: break;
                    }
                }
                randomizedFaces.ToList().ForEach(f => HB.SetPedFaceFeature(this, f, (float)Math.Round(Peralatan.Random.Next() % 2 == 0 ? random.NextDouble() * -1 : random.NextDouble(), 1)));
            }
            else
            {
                HairStyle = new PedComponent(PedComponent.EComponentID.HairStyle, femaleHairModel.GetRandomElement(true), 0, 0);
                int hairColor = random.Next() % 2 == 0 ? 0 : random.Next(1, 68);
                HB.SetPedHairColor(this, hairColor, hairColor);
                foreach (var olay in randomizedOIds)
                {
                    int index = olay switch
                    {
                        OverlayId.Blemishes => random.Next(HB.GetNumHeadOverlayValues(OverlayId.Blemishes) + 1),
                        //OverlayId.FacialHair => random.Next(28),
                        OverlayId.Eyebrows => random.Next(HB.GetNumHeadOverlayValues(OverlayId.Eyebrows) + 1),
                        OverlayId.Ageing => random.Next(HB.GetNumHeadOverlayValues(OverlayId.Ageing) + 1),
                        //OverlayId.Makeup => random.Next(74),
                        OverlayId.Blush => random.Next(HB.GetNumHeadOverlayValues(OverlayId.Blush) + 1),
                        OverlayId.Complexion => random.Next(HB.GetNumHeadOverlayValues(OverlayId.Complexion) + 1),
                        OverlayId.SunDamage => random.Next() % 2 == 0 ? 255 : random.Next(HB.GetNumHeadOverlayValues(OverlayId.SunDamage) + 1),
                        OverlayId.Lipstick => random.Next(HB.GetNumHeadOverlayValues(OverlayId.Lipstick) + 1),
                        OverlayId.Freckles => random.Next(HB.GetNumHeadOverlayValues(OverlayId.Freckles) + 1),
                        //OverlayId.ChestHair => 255,
                        OverlayId.BodyBlemishes => random.Next(HB.GetNumHeadOverlayValues(OverlayId.BodyBlemishes) + 1),
                        OverlayId.AddBodyBlemishes => random.Next(HB.GetNumHeadOverlayValues(OverlayId.AddBodyBlemishes) + 1),
                        _ => 255
                    };
                    HB.SetPedHeadOverlay(this, olay, index, (float)Math.Round(random.NextDouble(), 1));
                    switch (olay)
                    {
                        case OverlayId.Blush:
                        case OverlayId.Lipstick:
                            HB.SetPedHeadOverlayColor(this, olay, ColorType.BlushLipstick, index % 2 == 0 ? 0 : random.Next(1, 68), 0);
                            break;
                        default: break;
                    }
                }
                randomizedFaces.ToList().ForEach(f => HB.SetPedFaceFeature(this, f, (float)Math.Round(Peralatan.Random.Next() % 2 == 0 ? random.NextDouble() * -1 : random.NextDouble(), 1)));
            }
            HeadBlendData.ToString().ToLog();
        }
        public void RandomizeTextureFromCurrentDrawable()
        {

        }
        public void SetRobberComponent()
        {
            if (Gender == Gender.Female) return;
            Torso = new PedComponent(PedComponent.EComponentID.Torso, 0, 0, 0);
            Leg = new PedComponent(PedComponent.EComponentID.Leg, 34, 0, 0);
            Parachute = new PedComponent(PedComponent.EComponentID.Parachute, 45, 0, 0);
            Shoes = new PedComponent(PedComponent.EComponentID.Shoes, 24, 0, 0);
            Accessories = new PedComponent(PedComponent.EComponentID.Accessories, 40, 0);
            UnderShirt = new PedComponent(PedComponent.EComponentID.UnderShirt, -1, 0);
            Tops = new PedComponent(PedComponent.EComponentID.Tops, 241, Peralatan.Random.Next(5), 0);
        }
        public void SetMechanicComponent()
        {
            if (Gender == Gender.Female) return;
            Torso = new PedComponent(PedComponent.EComponentID.Torso, 194, Peralatan.Random.Next(1, 8));
            Leg = new PedComponent(PedComponent.EComponentID.Leg, 15, Peralatan.Random.Next(1, 16));
            Tops = new PedComponent(PedComponent.EComponentID.Tops, 69, Peralatan.Random.Next(1, 5));
            Shoes = new PedComponent(PedComponent.EComponentID.Shoes, 25, 0, 0);
            UnderShirt = new PedComponent(PedComponent.EComponentID.UnderShirt, 136, Peralatan.Random.Next(21));
            //Parachute = new PedComponent(PedComponent.EComponentID.Parachute, 45, 0, 0);
        }
    }
}
