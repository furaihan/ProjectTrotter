﻿namespace BarbarianCall.FreemodeUtil
{
    using System;
    using System.Linq;
    using Rage;
    using LSPD_First_Response;
    public class FreemodePed : Ped
    {
        public HeadBlendData HeadBlendData
        {
            get
            {
                return HeadBlend.GetDataFromPed(this);
            }
            set
            {
                HeadBlend.SetPedHeadBlendData(this, value);
            }
        }
        public Gender Gender { get; }
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
            Gender = gender;
            this.MakeMissionPed();
            GameFiber.Wait(75);
            RandomizeFaceShape();
        }
        public void RandomizeFaceShape()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int[] maleSkinSecondID = { 0, 1, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 2, 20, 3, 4, 42, 43, 44, 5, 6, 7, 8, 9 };
            int[] femaleSecondID = { 0, 1, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 2, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 3, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 4, 40, 41, 45, 5, 6, 7, 8, 9 };
            int[] maleHairModel = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 24, 30, 31, 35,
                36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,65,66,68,70,71,72,73,74 };
            int[] femaleHairModel = {1, 2,3,4,5,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,28,30,31,32,36,37,38,39,40,
                41,42,45,46,47,48,49,50,52,53,54,55,56,57,58,59,60,61,65,73,78,74,77,76 };
            HeadBlendData = Gender == Gender.Male ?
                new HeadBlendData(random.Next(44), random.Next(44), 0, random.Next(44), maleSkinSecondID.GetRandomElement(), 0, (float)Math.Round(random.NextDouble(), 2), (float)Math.Round(random.NextDouble(), 2), 0.0f, false) :
                new HeadBlendData(random.Next(21, 45), femaleSecondID.GetRandomElement(), 0, random.Next(21, 45), femaleSecondID.GetRandomElement(), 0, (float)Math.Round(random.NextDouble(), 2), (float)Math.Round(random.NextDouble(), 2), 0.0f, false);
            OverlayId[] oIds = (OverlayId[])Enum.GetValues(typeof(OverlayId));
            var randomizedOIds = oIds.GetRandomNumberOfElements(random.Next(2, oIds.Length));
            FaceFeature[] faces = (FaceFeature[])Enum.GetValues(typeof(FaceFeature));
            var randomizedFaces = faces.GetRandomNumberOfElements(random.Next(3, faces.Length));
            if (Gender == Gender.Male)
            {
                HairStyle = new PedComponent(PedComponent.EComponentID.HairStyle, maleHairModel.GetRandomElement(true), 0, 0);
                int hairColor = random.Next() % 2 == 0 ? 0 : random.Next(1, 68);
                HeadBlend.SetPedHairColor(this, hairColor, hairColor);
                foreach (var olay in randomizedOIds)
                {
                    int index = olay switch
                    {
                        OverlayId.Blemishes => random.Next(23),
                        OverlayId.FacialHair => random.Next(28),
                        OverlayId.Eyebrows => random.Next(33),
                        OverlayId.Ageing => random.Next(14),
                        //OverlayId.Makeup => 255,
                        //OverlayId.Blush => 255,
                        OverlayId.Complexion => random.Next(11),
                        OverlayId.SunDamage => random.Next(10),
                        //OverlayId.Lipstick => 255,
                        OverlayId.Freckles => random.Next(17),
                        OverlayId.ChestHair => random.Next(16),
                        OverlayId.BodyBlemishes => random.Next(11),
                        OverlayId.AddBodyBlemishes => random.Next(1),
                        _ => 255
                    };
                    HeadBlend.SetPedHeadOverlay(this, olay, index, (float)Math.Round(random.NextDouble(), 1));
                    switch (olay)
                    {
                        case OverlayId.FacialHair:
                        case OverlayId.Eyebrows:
                        case OverlayId.ChestHair:
                            HeadBlend.SetPedHeadOverlayColor(this, olay, ColorType.EyebrowBeardChestHair, 0, 0);
                            break;
                        default: break;
                    }
                }
                randomizedFaces.ToList().ForEach(f => HeadBlend.SetPedFaceFeature(this, f, (float)Math.Round(random.NextDouble(), 1)));
            }
            else
            {
                HairStyle = new PedComponent(PedComponent.EComponentID.HairStyle, femaleHairModel.GetRandomElement(true), 0, 0);
                int hairColor = random.Next() % 2 == 0 ? 0 : random.Next(1, 68);
                HeadBlend.SetPedHairColor(this, hairColor, hairColor);
                foreach (var olay in randomizedOIds)
                {
                    int index = olay switch
                    {
                        OverlayId.Blemishes => random.Next(23),
                        //OverlayId.FacialHair => random.Next(28),
                        OverlayId.Eyebrows => random.Next(33),
                        OverlayId.Ageing => random.Next(14),
                        //OverlayId.Makeup => random.Next(74),
                        OverlayId.Blush => random.Next(6),
                        OverlayId.Complexion => random.Next(11),
                        OverlayId.SunDamage => random.Next() % 2 == 0 ? 255 : random.Next(10),
                        OverlayId.Lipstick => random.Next(9),
                        OverlayId.Freckles => random.Next(17),
                        //OverlayId.ChestHair => 255,
                        OverlayId.BodyBlemishes => random.Next(11),
                        OverlayId.AddBodyBlemishes => random.Next(1),
                        _ => 255
                    };
                    HeadBlend.SetPedHeadOverlay(this, olay, index, (float)Math.Round(random.NextDouble(), 1));
                    switch (olay)
                    {
                        case OverlayId.Blush:
                        case OverlayId.Lipstick:
                            HeadBlend.SetPedHeadOverlayColor(this, olay, ColorType.BlushLipstick, index % 2 == 0 ? 0 : random.Next(1, 68), 0);
                            break;
                        default: break;
                    }
                }
                randomizedFaces.ToList().ForEach(f => HeadBlend.SetPedFaceFeature(this, f, (float)Math.Round(random.NextDouble(), 1)));
            }
            HeadBlendData.ToString().ToLog();
        }
        public int GetOverlayValue(OverlayId overlayId) => HeadBlend.GetPedHeadOverlayValue(this, overlayId);
        public void SetRobberComponent()
        {
            Torso = new PedComponent(PedComponent.EComponentID.Torso, 0, 0, 0);
            Leg = new PedComponent(PedComponent.EComponentID.Leg, 34, 0, 0);
            Parachute = new PedComponent(PedComponent.EComponentID.Parachute, 45, 0, 0);
            Shoes = new PedComponent(PedComponent.EComponentID.Shoes, 24, 0, 0);
            Accessories = new PedComponent(PedComponent.EComponentID.Accessories, 40, 0);
            UnderShirt = new PedComponent(PedComponent.EComponentID.UnderShirt, -1, 0);
            Tops = new PedComponent(PedComponent.EComponentID.Tops, 241, Peralatan.Random.Next(5), 0);
        }
    }
}