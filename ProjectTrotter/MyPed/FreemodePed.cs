using System;
using System.Linq;
using Rage;
using LSPD_First_Response;
using HB = ProjectTrotter.MyPed.HeadBlend;
using N = Rage.Native.NativeFunction;
using System.Collections.Generic;
using System.Diagnostics;

namespace ProjectTrotter.MyPed
{
    internal class FreemodePed : MyPed
    {
        private int? currentHairColor;
        internal HeadBlendData HeadBlendData
        {
            get => HB.GetDataFromPed(this);
            set => HB.SetPedHeadBlendData(this, value);
        }
        internal EyeColor EyeColor
        {
            get => HB.GetPedEyeColor(this);
            set => HB.SetPedEyeColor(this, value);
        }
        internal Gender Gender => Model.Hash == 0x705E61F2 ? Gender.Male : Gender.Female;
        /// <summary>
        /// Gets a value that indicates whether this <see cref="FreemodePed"/> is male
        /// </summary>
        /// <value><c>true</c> if this <see cref="FreemodePed"/> is male, otherwise <c>false</c></value>
        internal new bool IsMale => Model == 0x705E61F2;
        /// <summary>
        /// Gets a value that indicates whether this <see cref="FreemodePed"/> is female
        /// </summary>
        /// <value><c>true</c> if this <see cref="FreemodePed"/> is female, otherwise <c>false</c></value>
        internal new bool IsFemale => Model == 0x9C9EFFD8;
        internal FreemodePed(Vector3 position, float heading, bool isMale) : base(isMale ? 0x705E61F2/*mp_m_freemode_01*/ : 0x9C9EFFD8/*mp_f_freemode_01*/, position, heading)
        {
            MakePersistent();
            RandomizeAppearance();
            Metadata.BAR_FreemodePed = true;
            Metadata.BAR_Entity = true;
        }
        internal FreemodePed(Vector3 position, bool isMale) : base(isMale ? 0x705E61F2/*mp_m_freemode_01*/ : 0x9C9EFFD8/*mp_f_freemode_01*/, position, 0f)
        {
            MakePersistent();
            RandomizeAppearance();
            Metadata.BAR_FreemodePed = true;
            Metadata.BAR_Entity = true;
        }
        private FreemodePed(PoolHandle handle) : base(handle)
        {
        }
        internal static new FreemodePed FromRegularPed(Ped ped)
        {
            return new FreemodePed(ped.Handle);
        }
        internal void RandomizeAppearance()
        {
            SetHeadBlendData();
            SetHair();
            SetFaceFeatures();
            SetEyeColor();
            SetHeadOverlays();
            SetMechanicComponent();
        }
        private void SetHeadBlendData()
        {
            HashSet<int> mothers = new(Enumerable.Range(21, 21).Concat(new[] { 45 }));
            HashSet<int> fathers = new(Enumerable.Range(0, 21).Concat(new[] { 42, 43, 44 }));
            int firstID = mothers.GetRandomElement(true);
            int secondID = fathers.GetRandomElement(true);
            int thirdID = MyRandom.Next(10) == 0 ? IsMale ? fathers.GetRandomElement(true) : mothers.GetRandomElement(true) : 0;
            float thirdMix = (float)(thirdID == 0 ? 0.0f : MyRandom.NextDouble());
            float resemblance = (float)(IsFemale ? MyRandom.NextDouble() * 2 * 0.077 : MyRandom.NextDouble() * 2 * 0.785);
            resemblance = MathHelper.Clamp(resemblance, 0.0f, 1.0f);
            float skinTone = (float)MyRandom.NextDouble();
            HeadBlendData = new HeadBlendData(firstID, secondID, thirdID, firstID, secondID, thirdID, 
                (float)Math.Round(resemblance, 5), (float)Math.Round(skinTone, 5), (float)Math.Round(thirdMix, 5), false);
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (true)
            {
                GameFiber.Yield();
                if (HB.HasPedHeadBlendFinished(this))
                {
                    break;
                }
                if (stopwatch.ElapsedMilliseconds > 1000)
                {
                    "FreemodePed | Headblend: Timeout".ToLog();
                    break;
                }
            }
            N.Natives.FINALIZE_HEAD_BLEND(this);
        }
        private void SetHair()
        {
            HashSet<int> maleHairModel = new(Enumerable.Range(0, 59).Concat(new[] { 65, 66, 67, 68, 70, 71, 73 }));
            HashSet<int> femaleHairModel = new()
            {
                1, 2, 3, 4, 5, 7, 9, 10, 11, 14, 15, 17, 18, 20, 21, 22, 38, 39, 40, 41, 45, 47, 48, 49, 52, 53,
                54, 55, 56, 58, 59, 60, 65, 74, 75, 76
            };
            HashSet<int> normalHairColor = new(Enumerable.Range(0, 17)
                .Concat(new[] { 28, 29, 55, 56, 57, 58, 59, 60, 61, 62, 63 }));
            HashSet<int> hairHighlightColor = new(Enumerable.Range(0, 3)
                .Concat(new[] { 11, 12, 20, 21, 22, 33, 34, 29, 36, 35, 40, 41, 53, 52, 51, 47, 45, 62, 63 }));
            int hairColor = normalHairColor.GetRandomElement(true);
            HB.SetPedHairColor(this, hairColor, MyRandom.Next(10) == 0 ? hairHighlightColor.GetRandomElement(true) : hairColor);
            int hairStyle = IsMale ? maleHairModel.GetRandomElement(true) : femaleHairModel.GetRandomElement(true);
            currentHairColor = hairColor;
            Wardrobe.SetPedComponentVariation(PedComponentType.HairStyle, new PedComponentVariation(hairStyle, 0, 0));
        }
        private void SetFaceFeatures()
        {
            HashSet<FaceFeature> faceFeatures = new(Enumerable.Range(0, 20).Cast<FaceFeature>());
            FaceFeature[] selectedFaceFeatures = faceFeatures.OrderBy(x => MyRandom.Next(25)).Take(MyRandom.Next(5, 20)).ToArray();
            foreach (FaceFeature faceFeature in selectedFaceFeatures)
            {
                float scale = (float)Math.Round(MyRandom.Next(2) == 1 ? MyRandom.NextDouble() : MyRandom.NextDouble() * -1, 3, MidpointRounding.ToEven);
                HB.SetPedFaceFeature(this, faceFeature, scale);
            }
        }
        private void SetEyeColor()
        {
            EyeColor[] normalEyeColors = Enumerable.Range(0, 8).Cast<EyeColor>().ToArray();
            EyeColor = normalEyeColors.GetRandomElement(true);
        }
        private void SetHeadOverlays()
        {
            OverlayId[] headOverlays = Enum.GetValues(typeof(OverlayId)).Cast<OverlayId>().ToArray();
            OverlayId[] selectedOverlayIds = headOverlays.OrderBy(x => MyRandom.Next(25)).Take(MyRandom.Next(3, headOverlays.Length)).ToArray();
            OverlayId[] forbidden = IsMale ? new[] { OverlayId.Lipstick, OverlayId.Makeup, OverlayId.Blush } :
                new[] { OverlayId.FacialHair, OverlayId.ChestHair, OverlayId.SunDamage, OverlayId.Ageing, OverlayId.Freckles };
            Dictionary<OverlayId, float> opacityMultiplier = new()
            {
                { OverlayId.Blemishes, 0.1f },
                { OverlayId.FacialHair, 1f },
                { OverlayId.Eyebrows, 1f },
                { OverlayId.Ageing, 0.5f },
                { OverlayId.Makeup, 1f },
                { OverlayId.Blush, 0.6f },
                { OverlayId.Complexion, 0.6f },
                { OverlayId.SunDamage, 0.4f },
                { OverlayId.Lipstick, 1f },
                { OverlayId.Freckles, 1f },
                { OverlayId.ChestHair, 1f },
            };
            foreach (OverlayId headOverlay in selectedOverlayIds)
            {
                int index = headOverlay switch
                {
                    OverlayId.Blush => MyRandom.Next(10) == 0 ? MyRandom.Next(1, 6) : 255,
                    OverlayId.Makeup => MyRandom.Next(4) == 1 ? MyRandom.Next(1, 16) : 255,
                    _ when forbidden.Contains(headOverlay) => 255,
                    _ => MyRandom.Next(HB.GetNumHeadOverlayValues(headOverlay))
                };
                float opacity = headOverlay switch
                {
                    _ when forbidden.Contains(headOverlay) => 0.0f,
                    _ when opacityMultiplier.ContainsKey(headOverlay) => (float)(MyRandom.NextDouble() * 2 * opacityMultiplier[headOverlay]),
                    _ => (float)Math.Round(MyRandom.NextDouble(), 5, MidpointRounding.ToEven),
                };
                HB.SetPedHeadOverlay(this, headOverlay, index, opacity);
                int hairColor = currentHairColor ?? 0;
                switch (headOverlay)
                {
                    case OverlayId.FacialHair:
                    case OverlayId.ChestHair:
                    case OverlayId.Eyebrows:
                        HB.SetPedHeadOverlayColor(this, headOverlay, ColorType.EyebrowBeardChestHair, hairColor, 0);
                        break;
                    case OverlayId.Blush:
                    case OverlayId.Lipstick:
                        HB.SetPedHeadOverlayColor(this, headOverlay, ColorType.BlushLipstick, MyRandom.Next(26), 0);
                        break;
                    default: break;
                }

            }
        }
        internal void SetMechanicComponent()
        {
            if (Gender == Gender.Female) return;
            Wardrobe[PedComponentType.Torso] = new(194, MyRandom.Next(1, 8), 0);
            Wardrobe[PedComponentType.Leg] = new(15, MyRandom.Next(1, 16), 0);
            Wardrobe[PedComponentType.Tops] = new(69, MyRandom.Next(1, 5), 0);
            Wardrobe[PedComponentType.Shoes] = new(25, 0, 0);
            Wardrobe[PedComponentType.UnderShirt] = new(136, MyRandom.Next(21), 0);
            //Parachute = new PedComponent(PedComponent.EComponentID.Parachute, 45, 0, 0);
        }
    }
}
