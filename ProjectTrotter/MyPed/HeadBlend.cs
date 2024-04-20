namespace ProjectTrotter.MyPed
{
    using System.Drawing;
    using System.Runtime.InteropServices;
    using Rage.Native;
    using Rage;

    [StructLayout(LayoutKind.Explicit, Size = 80)]
    public struct HeadBlendData
    {
        [FieldOffset(0)]
        public int shapeFirstID;
        [FieldOffset(8)]
        public int shapeSecondID;
        [FieldOffset(16)]
        public int shapeThirdID;
        [FieldOffset(24)]
        public int skinFirstID;
        [FieldOffset(32)]
        public int skinSecondID;
        [FieldOffset(40)]
        public int skinThirdID;
        [FieldOffset(48)]
        public float shapeMix;
        [FieldOffset(56)]
        public float skinMix;
        [FieldOffset(64)]
        public float thirdMix;
        [FieldOffset(75)]
        public bool isParent;

        public HeadBlendData(int shapeFirstID, int shapeSecondID, int shapeThirdID, int skinFirstID, int skinSecondID, int skinThirdID, float shapeMix, float skinMix, float thirdMix, bool isParent)
        {
            this.shapeFirstID = shapeFirstID;
            this.shapeSecondID = shapeSecondID;
            this.shapeThirdID = shapeThirdID;
            this.skinFirstID = skinFirstID;
            this.skinSecondID = skinSecondID;
            this.skinThirdID = skinThirdID;
            this.shapeMix = shapeMix;
            this.skinMix = skinMix;
            this.thirdMix = thirdMix;
            this.isParent = isParent;
        }

        public override string ToString()
        {
            return $"Shape : ({shapeFirstID}, {shapeSecondID}, {shapeThirdID}). Skin : ({skinFirstID}, {skinSecondID}, {skinThirdID}). Mix : (Shape: {shapeMix}, Skin: {skinMix}, Third: {thirdMix}) Parent: {isParent}";
        }
    }
    public static class HeadBlend
    {
        public static HeadBlendData GetDataFromPed(Ped ped)
        {
            NativeFunction.Natives.GET_PED_HEAD_BLEND_DATA<bool>(ped, out HeadBlendData result);

            return result;
        }

        public static void SetPedHeadBlendData(Ped ped, HeadBlendData headBlendData)
        {
            NativeFunction.Natives.SET_PED_HEAD_BLEND_DATA(ped, headBlendData.shapeFirstID, headBlendData.shapeSecondID,
                headBlendData.shapeThirdID, headBlendData.skinFirstID, headBlendData.skinSecondID,
                headBlendData.skinThirdID, headBlendData.shapeMix, headBlendData.skinMix, headBlendData.thirdMix,
                headBlendData.isParent);
        }

        public static void UpdatePedHeadBlendData(Ped ped, float shapeMix, float skinMix, float thirdMix)
        {
            NativeFunction.Natives.UPDATE_PED_HEAD_BLEND_DATA(ped, shapeMix, skinMix, thirdMix);
        }

        public static void SetPedEyeColor(Ped ped, EyeColor eyeColor) => SetPedEyeColor(ped, (int)eyeColor);
        public static void SetPedEyeColor(Ped ped, int index)
        {
            NativeFunction.Natives.SET_HEAD_BLEND_EYE_COLOR(ped, index);
        }
        public static EyeColor GetPedEyeColor(Ped ped)
        {
            int eyeColor = NativeFunction.Natives.GET_HEAD_BLEND_EYE_COLOR<int>(ped);
            return (EyeColor)eyeColor;

        }
        public static void SetPedHeadOverlay(Ped ped, OverlayId overlayId, int index, float opacity)
        {
            NativeFunction.Natives.SET_PED_HEAD_OVERLAY(ped, (int)overlayId, index, opacity);
        }

        public static int GetPedHeadOverlayValue(Ped ped, OverlayId overlayId)
        {
            return NativeFunction.Natives.GET_PED_HEAD_OVERLAY<int>(ped, (int)overlayId);
        }

        public static int GetNumHeadOverlayValues(OverlayId overlayId)
        {
            return NativeFunction.Natives.GET_PED_HEAD_OVERLAY_NUM<int>((int)overlayId);
        }

        public static void SetPedHeadOverlayColor(Ped ped, OverlayId overlayId, ColorType colorType, int colorId,
            int secondColorId)
        {
            NativeFunction.Natives.SET_PED_HEAD_OVERLAY_TINT(ped, (int)overlayId, (int)colorType, colorId, secondColorId);
        }

        public static void SetPedHairColor(Ped ped, int colorId, int highlightColorId)
        {
            NativeFunction.Natives.SET_PED_HAIR_TINT(ped, colorId, highlightColorId);
        }

        public static int GetNumberOfPedHairColors()
        {
            return NativeFunction.Natives.GET_NUM_PED_HAIR_TINTS<int>();
        }

        public static int GetNumberOfMakeupColors()
        {
            return NativeFunction.Natives.GET_NUM_PED_MAKEUP_TINTS<int>();
        }

        public static Color GetHairColor(int colorId)
        {
            NativeFunction.Natives.GET_PED_HAIR_TINT_COLOR(colorId, out int r, out int g, out int b);
            Color color = Color.FromArgb(r, g, b);

            return color;
        }

        public static Color GetLipstickColor(int colorId)
        {
            NativeFunction.Natives.GET_PED_MAKEUP_TINT_COLOR(colorId, out int r, out int g, out int b);
            Color color = Color.FromArgb(r, g, b);

            return color;
        }

        public static bool IsPedHairColorValid(int colorId)
        {
            return NativeFunction.Natives.IS_PED_HAIR_TINT_FOR_BARBER<bool>(colorId);
        }

        public static bool IsPedLipstickColorValid(int colorId)
        {
            return NativeFunction.Natives.IS_PED_LIPSTICK_TINT_FOR_BARBER<bool>(colorId);
        }

        public static bool IsPedBlushColorValid(int colorId)
        {
            return NativeFunction.Natives.IS_PED_BLUSH_TINT_FOR_BARBER<bool>(colorId);
        }

        public static void SetPedFaceFeature(Ped ped, FaceFeature faceFeature, float scale)
        {
            NativeFunction.Natives.SET_PED_MICRO_MORPH(ped, (int)faceFeature, scale);
        }

        public static bool HasPedHeadBlendFinished(Ped ped)
        {
            return NativeFunction.Natives.HAS_PED_HEAD_BLEND_FINISHED<bool>(ped);
        }

        public static int GetFirstParentIdForPedType(PedType pedType)
        {
            return NativeFunction.Natives.GET_PED_HEAD_BLEND_FIRST_INDEX<int>((int)pedType);
        }

        public static int GetNumParentPedsOfType(PedType pedType)
        {
            return NativeFunction.Natives.GET_PED_HEAD_BLEND_NUM_HEADS<int>((int)pedType);
        }
    }
    public enum PedType
    {
        NonDlcMale,
        NonDlcFemale,
        DlcMale,
        DlcFemale
    }

    public enum OverlayId
    {
        Blemishes,
        FacialHair,
        Eyebrows,
        Ageing,
        Makeup,
        Blush,
        Complexion,
        SunDamage,
        Lipstick,
        Freckles,
        ChestHair,
        BodyBlemishes,
        AddBodyBlemishes
    }

    public enum ColorType
    {
        Other,
        EyebrowBeardChestHair,
        BlushLipstick
    }

    public enum FaceFeature
    {
        Nose_Width,
        Nose_Peak_Hight,
        Nose_Peak_Lenght,
        Nose_Bone_High,
        Nose_Peak_Lowering,
        Nose_Bone_Twist,
        EyeBrown_High,
        EyeBrown_Forward,
        Cheeks_Bone_High,
        Cheeks_Bone_Width,
        Cheeks_Width,
        Eyes_Openning,
        Lips_Thickness,
        Jaw_Bone_Width,
        Jaw_Bone_Lenght,
        Chimp_Bone_Lowering,
        Chimp_Bone_Lenght,
        Chimp_Bone_Width,
        Chimp_Hole,
        Neck_Thikness,
    }
    public enum EyeColor
    {
        Black,
        VeryLightBlueORGreen,
        DarkBlue,
        Brown,
        DarkerBrown,
        LightBrown,
        Blue,
        LightBlue,
        Pink,
        Yellow,
        Purple,
        Black2,
        DarkGreen,
        LightBrown2,
        YellowORBlackPattern,
        LightColoredSpiralPattern,
        ShinyRed,
        ShinyHalfBlueORHalfRed,
        HalfBlackORHalfLightBlue,
        WhiteORRedPerimter,
        GreenSnake,
        RedSnake,
        DarkBlueSnake,
        DarkYellow,
        BrightYellow,
        AllBlack,
        RedSmallPupil,
        DevilBlueORBlack,
        WhiteSmallPupil,
        GlossedOver,
    }
}
