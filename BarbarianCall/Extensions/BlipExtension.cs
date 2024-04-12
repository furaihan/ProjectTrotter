using System.Drawing;
using Rage;
using Rage.Native;

namespace BarbarianCall.Extensions
{
    public static class BlipExtension
    {
        /// <summary>
        /// Gets whether the blip exists.
        /// </summary>
        /// <param name="blip"></param>
        /// <returns></returns>
        internal static bool DoesBlipExist(Blip blip) => NativeFunction.Natives.DOES_BLIP_EXIST<bool>(blip);
        /// <summary>
        /// Sets the blip display type.
        /// </summary>
        /// <param name="blip"> The blip to set the display type for. </param>
        /// <param name="displayType"> The display type to set. </param>
        internal static void SetBlipDisplayType(Blip blip, BlipDisplayType displayType) => NativeFunction.Natives.SET_BLIP_DISPLAY(blip, (int)displayType);
        public static void SetBlipHighDetail(Blip blip, bool highDetail) => NativeFunction.Natives.SET_BLIP_HIGH_DETAIL(blip, highDetail);
        public static void SetBlipHiddenOnLegend(Blip blip, bool hidden) => NativeFunction.Natives.SET_BLIP_HIDDEN_ON_LEGEND(blip, hidden);
        /// <summary>
        /// Set blip sprite
        /// </summary>
        /// <param name="blip"></param>
        /// <param name="spriteID">spriteID information: https://docs.fivem.net/docs/game-references/blips/</param>
        public static void SetBlipSprite(this Blip blip, int spriteID) => NativeFunction.Natives.SET_BLIP_SPRITE(blip, spriteID);
        public static void SetBlipName(this Blip blip, string name)
        {
            NativeFunction.Natives.BEGIN_TEXT_COMMAND_SET_BLIP_NAME("STRING");
            NativeFunction.Natives.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME(name);
            NativeFunction.Natives.END_TEXT_COMMAND_SET_BLIP_NAME(blip);
        }
        public static void ShowOutlineColor(Blip blip, Color color)
        {
            NativeFunction.Natives.SHOW_OUTLINE_INDICATOR_ON_BLIP(blip, true);
            NativeFunction.Natives.SET_BLIP_SECONDARY_COLOUR(blip, color.R, color.G, color.B);
        }
        public static void HideOutlineColor(Blip blip) => NativeFunction.Natives.SHOW_OUTLINE_INDICATOR_ON_BLIP(blip, false);
        public static void SetBlipShrink(Blip blip, bool shrink) => NativeFunction.Natives.SET_BLIP_AS_MINIMAL_ON_EDGE(blip, shrink);
        public enum BlipDisplayType : int
        {
            Minimap = 5,
            MainmapSelectable = 3,
            BothMapUnselectable = 8,
            BothMapSelectable = 2,
            NoDisplay = 0
        }
        /// <summary>
        /// Gets the token used to display the sprite of <paramref name="blip"/> in formatted scaleform text (i.e. the scaleform uses the game function `SET_FORMATTED_TEXT_WITH_ICONS`).
        /// <para>
        /// Example:
        /// <code>
        /// Blip myBlip = ...;<br />
        /// Game.DisplayHelp($"Go to ~{myBlip.GetIconToken()}~.");<br />
        /// Game.DisplayHelp($"Go to ~{HudColor.Red.GetName()}~~{myBlip.GetIconToken()}~~s~."); // with a different color
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="blip">The blip to get the sprite from.</param>
        /// <returns>The <see cref="string"/> with the icon token for the sprite of the given blip.</returns>
        /// <remarks>Source: <a href="https://github.com/alexguirre/RAGENativeUI/blob/master/Source/BlipExtensions.cs">RAGENativeUI by alexguirre</a></remarks>
        internal static string GetIconToken(this Blip blip, bool withColor)
        {
            if (withColor)
            {
                return $"<font color=\"{ColorTranslator.ToHtml(blip.Color)}\">~BLIP_{(int)blip.Sprite}~</font>";
            }
            return $"~BLIP_{(int)blip.Sprite}";
        }
    }
}
