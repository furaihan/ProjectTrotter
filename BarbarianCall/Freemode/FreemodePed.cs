using System;
using System.Linq;
using Rage;
using LSPD_First_Response;
using BarbarianCall.Extensions;
using HB = BarbarianCall.Freemode.HeadBlend;
using N = Rage.Native.NativeFunction;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Diagnostics;

namespace BarbarianCall.Freemode
{
    public class FreemodePed : Ped
    {
        private PedComponentCollection _wardrobe;
        private PedCombatProperty _combatProperty;
        public HeadBlendData HeadBlendData
        {
            get => HB.GetDataFromPed(this);
            set => HB.SetPedHeadBlendData(this, value);
        }
        public EyeColor EyeColor
        {
            get => HB.GetPedEyeColor(this);
            set => HB.SetPedEyeColor(this, value);
        }
        public Gender Gender => Model.Hash == 0x705E61F2 ? Gender.Male : Gender.Female;
        /// <summary>
        /// Gets a value that indicates whether this <see cref="FreemodePed"/> is male
        /// </summary>
        /// <value><c>true</c> if this <see cref="FreemodePed"/> is male, otherwise <c>false</c></value>
        public new bool IsMale => Model == 0x705E61F2;
        /// <summary>
        /// Gets a value that indicates whether this <see cref="FreemodePed"/> is female
        /// </summary>
        /// <value><c>true</c> if this <see cref="FreemodePed"/> is female, otherwise <c>false</c></value>
        public new bool IsFemale => Model == 0x9C9EFFD8;
        public PedComponentCollection Wardrobe => _wardrobe ??= new PedComponentCollection(this);
        public PedCombatProperty CombatProperty => _combatProperty ??= new PedCombatProperty(this);
        #region COMPONENT
        public PedComponent Torso
        {
            get => PedComponent.GetPedComponent(this, PedComponent.EComponentID.Torso);
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
            get => PedComponent.GetPedComponent(this, PedComponent.EComponentID.Head);
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
            get => PedComponent.GetPedComponent(this, PedComponent.EComponentID.Mask);
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
            get => PedComponent.GetPedComponent(this, PedComponent.EComponentID.Shoes);
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
            get => PedComponent.GetPedComponent(this, PedComponent.EComponentID.Leg);
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
            get => PedComponent.GetPedComponent(this, PedComponent.EComponentID.Parachute);
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
            get => PedComponent.GetPedComponent(this, PedComponent.EComponentID.Decal);
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
            get => PedComponent.GetPedComponent(this, PedComponent.EComponentID.UnderShirt);
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
            get => PedComponent.GetPedComponent(this, PedComponent.EComponentID.HairStyle);
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
            get => PedComponent.GetPedComponent(this, PedComponent.EComponentID.BodyArmor);
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
            get => PedComponent.GetPedComponent(this, PedComponent.EComponentID.Tops);
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
            get => PedComponent.GetPedComponent(this, PedComponent.EComponentID.Accessories);
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
        public FreemodePed(Vector3 position, float heading, bool isMale) : base(isMale ? 0x705E61F2/*mp_m_freemode_01*/ : 0x9C9EFFD8/*mp_f_freemode_01*/, position, heading)
        {
            MakePersistent();
            RandomizeAppearance();
            Metadata.BAR_FreemodePed = true;
            Metadata.BAR_Entity = true;
        }
        public FreemodePed(Vector3 position, bool isMale) : base(isMale ? 0x705E61F2/*mp_m_freemode_01*/ : 0x9C9EFFD8/*mp_f_freemode_01*/, position, 0f)
        {
            MakePersistent();
            RandomizeAppearance();
            Metadata.BAR_FreemodePed = true;
            Metadata.BAR_Entity = true;
        }
        private FreemodePed(PoolHandle handle) : base(handle)
        {
        }
        /// <summary>
        /// Get freemode ped from regular ped, only work if the ped model is equal to <c>mp_m_freemode_01</c> or <c>mp_f_freemode_01</c>
        /// </summary>
        /// <param name="ped"></param>
        /// <returns>return the ped instance as freemode ped, if it failed, then will return <c>null</c></returns>
        public static FreemodePed FromRegularPed(Ped ped) => new(ped.Handle);
        public void RandomizeAppearance()
        {
            byte[] box = new byte[4];
            RNGCryptoServiceProvider provider = new();
            provider.GetNonZeroBytes(box);
            SHA512CryptoServiceProvider sHA512 = new();
            //https://s.id/BkZuh
            #region local variable
            int[] mothers = { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 45 };
            int[] fathers = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 42, 43, 44 };
            int[] maleHairModel = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 24, 30, 31, 32, 33,
                35, 36, 37, 38, 39, 40, 41, 42, 43, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 65, 66, 67, 68, 70, 71, 73 };
            int[] femaleHairModel = { 1, 2, 3, 4, 5, 7, 9, 10, 11, 14, 15, 17, 18, 20, 21, 22, 38, 39, 40, 41, 45, 47, 48, 49, 52, 53,
                54, 55, 56, 58, 59, 60, 65, 74, 75, 76 };
            int[] normalHairColor = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 28, 29, 55, 56, 57, 58, 59, 60, 61, 62, 63 };
            int[] hairHighlightColor = { 0, 1, 2, 11, 12, 20, 21, 22, 33, 34, 29, 36, 35, 40, 41, 53, 52, 51, 47, 45, 62, 63 };
            int firstID = mothers.GetRandomElement(true);
            int secondID = fathers.GetRandomElement(true);
            int thirdID = MyRandom.Next(10) == 0 ? IsMale ? fathers.GetRandomElement(true) : mothers.GetRandomElement(true) : 0;
            float thirdMix = (float)(thirdID == 0 ? 0.0f : MyRandom.NextDouble());
            float resemblance = (float)(IsFemale ? MyRandom.NextDouble() * 2 * 0.077 : MyRandom.NextDouble() * 2 * 0.785);
            resemblance = MathHelper.Clamp(resemblance, 0.0f, 1.0f);
            float skinTone = (float)MyRandom.NextDouble();
            int hairColor = normalHairColor.GetRandomElement(true);
            OverlayId[] headOverlays = Enum.GetValues(typeof(OverlayId)).Cast<OverlayId>().ToArray();
            OverlayId[] selectedOverlayIds = headOverlays.OrderBy(x => MyRandom.Next(25)).Take(MyRandom.Next(3, headOverlays.Length)).ToArray();
            OverlayId[] forbiddenForFemale = { OverlayId.FacialHair, OverlayId.ChestHair, OverlayId.SunDamage, OverlayId.Ageing, OverlayId.Freckles };
            OverlayId[] forbiddenForMale = { OverlayId.Lipstick, OverlayId.Makeup, OverlayId.Blush, };
            FaceFeature[] faceFeatures = Enum.GetValues(typeof(FaceFeature)).Cast<FaceFeature>().ToArray();
            FaceFeature[] selectedFaceFeatures = faceFeatures.OrderBy(x => MyRandom.Next(25)).Take(MyRandom.Next(5, headOverlays.Length)).ToArray();
            EyeColor[] normalEyeColors = Enumerable.Range(0, 8).Cast<EyeColor>().ToArray();
            //https://s.id/Bx6sU
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
            #endregion
            GameFiber.Yield();
            HeadBlendData = new HeadBlendData(firstID, secondID, thirdID, firstID, secondID, thirdID, (float)Math.Round(resemblance, 5), (float)Math.Round(skinTone, 5), (float)Math.Round(thirdMix, 5), false);
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
            EyeColor = normalEyeColors.GetRandomElement(true);
            HB.SetPedHairColor(this, hairColor, MyRandom.Next(10) == 0 ? hairHighlightColor.GetRandomElement(true) : hairColor);
            N.Natives.FinalizeHeadBlend(this);
            foreach (FaceFeature faceFeature in selectedFaceFeatures)
            {
                float scale = (float)Math.Round(MyRandom.Next(2) == 1 ? MyRandom.NextDouble() : MyRandom.NextDouble() * -1, 3, MidpointRounding.ToEven);
                HB.SetPedFaceFeature(this, faceFeature, scale);
            }
            if (IsMale)
            {
                Voice = Globals.MaleVoiceName.GetRandomElement(true);
                HairStyle = new PedComponent(PedComponent.EComponentID.HairStyle, maleHairModel.GetRandomElement(true), 0, 0);
                foreach (OverlayId headOverlay in selectedOverlayIds)
                {
                    int index = headOverlay switch
                    {
                        _ when forbiddenForMale.Contains(headOverlay) => 255,
                        _ => MyRandom.Next(HB.GetNumHeadOverlayValues(headOverlay))
                    };
                    float opacity = headOverlay switch
                    {
                        _ when forbiddenForMale.Contains(headOverlay) => 0.0f,
                        _ when opacityMultiplier.ContainsKey(headOverlay) => (float)(MyRandom.NextDouble() * 2 * opacityMultiplier[headOverlay]),
                        _ => (float)Math.Round(MyRandom.NextDouble(), 5, MidpointRounding.ToEven),
                    };
                    HB.SetPedHeadOverlay(this, headOverlay, index, opacity);
                    switch (headOverlay)
                    {
                        case OverlayId.FacialHair:
                        case OverlayId.ChestHair:
                        case OverlayId.Eyebrows:
                            HB.SetPedHeadOverlayColor(this, headOverlay, ColorType.EyebrowBeardChestHair, hairColor, 0);
                            break;
                        default: break;
                    }
                }
            }
            else if (IsFemale)
            {
                Voice = Globals.FemaleVoiceName.GetRandomElement();
                HairStyle = new PedComponent(PedComponent.EComponentID.HairStyle, femaleHairModel.GetRandomElement(true), 0, 0);
                foreach (OverlayId headOverlay in selectedOverlayIds)
                {
                    int index = headOverlay switch
                    {
                        OverlayId.Blush => MyRandom.Next(10) == 0 ? MyRandom.Next(1, 6) : 255,
                        OverlayId.Makeup => MyRandom.Next(4) == 1 ? MyRandom.Next(1, 16) : 255,
                        _ when forbiddenForFemale.Contains(headOverlay) => 255,
                        _ => MyRandom.Next(HB.GetNumHeadOverlayValues(headOverlay)),
                    };
                    float opacity = headOverlay switch
                    {
                        _ when forbiddenForFemale.Contains(headOverlay) => 0.0f,
                        _ when opacityMultiplier.ContainsKey(headOverlay) => (float)(MyRandom.NextDouble() * 2 * opacityMultiplier[headOverlay]),
                        _ => (float)Math.Round(MyRandom.NextDouble(), 5, MidpointRounding.ToEven),
                    };
                    HB.SetPedHeadOverlay(this, headOverlay, index, opacity);
                    switch (headOverlay)
                    {
                        case OverlayId.Eyebrows:
                            HB.SetPedHeadOverlayColor(this, headOverlay, ColorType.EyebrowBeardChestHair , hairColor, 0);
                            break;
                        case OverlayId.Blush:
                        case OverlayId.Lipstick:
                            HB.SetPedHeadOverlayColor(this, headOverlay, ColorType.BlushLipstick , MyRandom.Next(26), 0);
                            break;
                        default: break;
                    }
                }
            }
            HeadBlendData.ToString().ToLog();
        }
        public void RandomizeOutfit()
        {
            bool jaketan = MyRandom.Next() % 2 == 0;
            if (IsMale)
            {
                var selectedTops = Globals.AtasanCowokPolos.GetRandomElement();
                var selectedBottoms = Globals.BawahanCowok.GetRandomElement();
                var selectedShoes = Globals.AlasKaki.GetRandomElement();
                var topTex = selectedTops.Value.GetRandomElement();
                var botTex = selectedBottoms.Value.GetRandomElement();
                var shoTex = selectedShoes.Value.GetRandomElement();
                if (jaketan)
                {
                    $"Jaketan".ToLog();
                    selectedTops = Globals.JaketCowok.GetRandomElement();
                    botTex = selectedTops.Value.GetRandomElement();
                    var selectedUndershirt = Globals.UndershirtMale.GetRandomElement();
                    var usTex = selectedUndershirt.Value.GetRandomElement();
                    $"UNDERSHIRT | Draw: {selectedUndershirt.Key}. Tex: {usTex}".ToLog();
                    Wardrobe.UnderShirt = new PedComponent(PedComponent.EComponentID.UnderShirt, selectedUndershirt.Key, usTex);
                    if (MyRandom.Next() % 5 != 0) Wardrobe.Torso = new PedComponent(PedComponent.EComponentID.Torso, 184, 0, 0);
                    else Wardrobe.Torso = new PedComponent(PedComponent.EComponentID.Torso, 180, MyRandom.Next(7));
                }
                $"TOP | Draw: {selectedTops.Key}. Tex: {topTex}".ToLog();
                $"BOTTOM | Draw: {selectedBottoms.Key}. Tex: {botTex}".ToLog();
                $"SHOES | Draw: {selectedShoes.Key}. Tex: {shoTex}".ToLog();
                Wardrobe.Tops = new PedComponent(PedComponent.EComponentID.Tops, selectedTops.Key, topTex);
                Wardrobe.Leg = new PedComponent(PedComponent.EComponentID.Leg, selectedBottoms.Key, botTex);
                Wardrobe.Shoes = new PedComponent(PedComponent.EComponentID.Shoes, selectedShoes.Key, shoTex);
                Wardrobe.UnderShirt = new PedComponent(PedComponent.EComponentID.UnderShirt, -1, 0);
                if (selectedTops.Key == 238 && !jaketan) Wardrobe.Torso = new PedComponent(PedComponent.EComponentID.Torso, 2, 0);
                if (MyRandom.Next() % 2 == 0)
                {
                    var kcmt = Globals.KacamataCowok.GetRandomElement();
                    N.Natives.SET_PED_PROP_INDEX(this, 1, kcmt.Key, kcmt.Value.GetRandomElement(), true);
                }
                if (MyRandom.Next() % 2 == 0)
                {
                    var topi = Globals.TopiCowok.GetRandomElement();
                    N.Natives.SET_PED_PROP_INDEX(this, 0, topi.Key, topi.Value.GetRandomElement(), true);
                }
                if (MyRandom.Next() % 2 == 0)
                {
                    int maxWatch = N.Natives.GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS<int>(this, 6);
                    int selected = MyRandom.Next(maxWatch);
                    int maxTex = N.Natives.GET_NUMBER_OF_PED_PROP_TEXTURE_VARIATIONS<int>(this, 6, selected);
                    N.Natives.SET_PED_PROP_INDEX(this, 6, selected, MyRandom.Next(maxTex), true);
                }
            }
            var decal = Globals.DecalBadge.GetRandomElement();
            $"DECAL | Collection: {decal.Item1}, Decal: {decal.Item2}".ToLog();
            N.Natives.ADD_​PED_​DECORATION_​FROM_​HASHES(this, Game.GetHashKey(decal.Item1), Game.GetHashKey(decal.Item2));
        }               
        internal void SetMechanicComponent()
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
