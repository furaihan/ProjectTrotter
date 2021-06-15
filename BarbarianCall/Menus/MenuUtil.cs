using System.Windows.Forms;
using Rage;
using Rage.Native;
using RAGENativeUI;

namespace BarbarianCall.Menus
{
    internal class MenuUtil
    {
        public static string GetKeyboardInput(string title, string boxText, int textLength)
        {
            Localization.SetText("BAR_KEYB_TT", title);
            NativeFunction.Natives.DISPLAY_ONSCREEN_KEYBOARD(true, "BAR_KEYB_TT", 0, boxText, 0, 0, 0, textLength);
            Game.DisplayHelp($"Press {FormatKeyBinding(Keys.None, Keys.Enter)} to commit changes\nPress {FormatKeyBinding(Keys.None, Keys.Escape)} to back", true);
            Game.DisplaySubtitle(title, 900000);
            while (NativeFunction.Natives.x0CF2B696BBF945AE<int>() == 0)
            {
                GameFiber.Yield();
            }
            NativeFunction.Natives.ENABLE_ALL_CONTROL_ACTIONS(2);
            Game.DisplaySubtitle("");
            Game.HideHelp();

            return NativeFunction.Natives.GET_ONSCREEN_KEYBOARD_RESULT<string>();
        }
        public static string FormatKeyBinding(Keys modifierKey, Keys key)
            => modifierKey == Keys.None ? $"~{key.GetInstructionalId()}~" :
                                          $"~{modifierKey.GetInstructionalId()}~ ~+~ ~{key.GetInstructionalId()}~";
    }
}
