namespace BarbarianCall.Types
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using Rage;
    using Rage.Native;

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
            NativeFunction.Natives.x2746BD9D88C5C5D0<bool>(ped, out HeadBlendData result);

            return result;
        }

        public static void SetPedHeadBlendData(Ped ped, HeadBlendData headBlendData)
        {
            NativeFunction.Natives.SetPedHeadBlendData(ped, headBlendData.shapeFirstID, headBlendData.shapeSecondID,
                headBlendData.shapeThirdID, headBlendData.skinFirstID, headBlendData.skinSecondID,
                headBlendData.skinThirdID, headBlendData.shapeMix, headBlendData.skinMix, headBlendData.thirdMix,
                headBlendData.isParent);
        }

        public static void UpdatePedHeadBlendData(Ped ped, float shapeMix, float skinMix, float thirdMix)
        {
            NativeFunction.Natives.UpdatePedHeadBlendData(ped, shapeMix, skinMix, thirdMix);
        }

        public static void SetPedEyeColor(Ped ped, int index)
        {
            NativeFunction.Natives.x50B56988B170AFDF(ped, index);
        }

        public static void SetPedHeadOverlay(Ped ped, OverlayId overlayId, int index, float opacity)
        {
            NativeFunction.Natives.SetPedHeadOverlay(ped, (int)overlayId, index, opacity);
        }

        public static int GetPedHeadOverlayValue(Ped ped, OverlayId overlayId)
        {
            return NativeFunction.Natives.xA60EF3B6461A4D43<int>(ped, (int)overlayId);
        }

        public static int GetNumHeadOverlayValues(OverlayId overlayId)
        {
            return NativeFunction.Natives.xCF1CE768BB43480E<int>((int)overlayId);
        }

        public static void SetPedHeadOverlayColor(Ped ped, OverlayId overlayId, ColorType colorType, int colorId,
            int secondColorId)
        {
            NativeFunction.Natives.x497BF74A7B9CB952(ped, (int)overlayId, (int)colorType, colorId, secondColorId);
        }

        public static void SetPedHairColor(Ped ped, int colorId, int highlightColorId)
        {
            NativeFunction.Natives.x4CFFC65454C93A49(ped, colorId, highlightColorId);
        }

        public static int GetNumberOfPedHairColors()
        {
            return NativeFunction.Natives.xE5C0CF872C2AD150<int>();
        }

        public static int GetNumberOfMakeupColors()
        {
            return NativeFunction.Natives.xD1F7CA1535D22818<int>();
        }

        public static Color GetHairColor(int colorId)
        {
            NativeFunction.Natives.x4852FC386E2E1BB5(colorId, out int r, out int g, out int b);
            var color = Color.FromArgb(r, g, b);

            return color;
        }

        public static Color GetLipstickColor(int colorId)
        {
            NativeFunction.Natives.x013E5CFC38CD5387(colorId, out int r, out int g, out int b);
            var color = Color.FromArgb(r, g, b);

            return color;
        }

        public static bool IsPedHairColorValid(int colorId)
        {
            return NativeFunction.Natives.xE0D36E5D9E99CC21<bool>(colorId);
        }

        public static bool IsPedLipstickColorValid(int colorId)
        {
            return NativeFunction.Natives.x0525A2C2562F3CD4<bool>(colorId);
        }

        public static bool IsPedBlushColorValid(int colorId)
        {
            return NativeFunction.Natives.x604E810189EE3A59<bool>(colorId);
        }

        public static void SetPedFaceFeature(Ped ped, FaceFeature faceFeature, float scale)
        {
            NativeFunction.Natives.x71A5C1DBA060049E(ped, (int)faceFeature, scale);
        }

        public static bool HasPedHeadBlendFinished(Ped ped)
        {
            return NativeFunction.Natives.HasPedHeadBlendFinished<bool>(ped);
        }

        public static int GetFirstParentIdForPedType(PedType pedType)
        {
            return NativeFunction.Natives.x68D353AB88B97E0C<int>((int)pedType);
        }

        public static int GetNumParentPedsOfType(PedType pedType)
        {
            return NativeFunction.Natives.x5EF37013A6539C9D<int>((int)pedType);
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
}
