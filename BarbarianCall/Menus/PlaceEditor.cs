using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using BarbarianCall.Types;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace BarbarianCall.Menus
{
    internal class PlaceEditor
    {
        internal static UIMenu PlaceEditorMenu;
        internal static UIMenuItem CreateNew;
        internal static UIMenuItem AddVehicleEntry;
        internal static UIMenuItem AddPedEntry;
        internal static UIMenuItem AddVehicleExit;
        internal static UIMenuItem AddPedExit;
        internal static List<Place> Places = new();
        internal static UIMenuListScrollerItem<Place> PlaceList;
        private static Ped PlayerPed => Game.LocalPlayer.Character;
        public static void CreateMenu()
        {

        }
        public static void OnCreateNew(UIMenu m, UIMenuItem si)
        {
            GameFiber.StartNew(() =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                Game.DisplaySubtitle("You will add your current location to a new place, press ~o~Y~s~ to continue, Auto Cancel in 6 Seconds");
                int sec = 6;
                while (true)
                {
                    GameFiber.Yield();
                    if (Game.IsKeyDown(Keys.Y))
                    {
                        break;
                    }
                    if (stopwatch.ElapsedMilliseconds > 6000) return;
                    if (stopwatch.ElapsedMilliseconds % 1000 == 0)
                    {
                        Game.DisplaySubtitle($"You will add your current location to a new place, press ~o~Y~s~ to continue, Auto Cancel in {sec--} Seconds");
                    }
                }
                Game.DisplaySubtitle("");
                string name = MenuUtil.GetKeyboardInput("Please enter the place name", "", 80);
                if (Places.Any(p=> p.Name == name))
                {
                    Game.DisplayNotification($"{name} Place is already exist, try another name");
                }
                Places.Add(new Place(name, PlayerPed.IsInAnyVehicle(false) ? PlayerPed.CurrentVehicle.Position : PlayerPed.Position));
            });
        }
        public static void OnAddVEnt(UIMenu uIMenu, UIMenuItem si)
        {
            if (PlayerPed.IsInAnyVehicle(false))
            {

            }
            else Game.DisplaySubtitle("To add vehicle entry, you must be inside a vehicle");
        }
    }
}
