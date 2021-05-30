using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            NativeFunction.Natives.DISABLE_ALL_CONTROL_ACTIONS(2);
            NativeFunction.Natives.DISPLAY_ONSCREEN_KEYBOARD(true, "STRING", 0, boxText, 0, 0, 0, textLength);
            NativeFunction.Natives.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME(title);
            NativeFunction.Natives.ADD_TEXT_COMPONENT_SUBSTRING_KEYBOARD_DISPLAY(title);
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
