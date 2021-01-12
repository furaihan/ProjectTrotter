using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Rage;
using System.Drawing;
using BarbarianCall.SupportUnit;

namespace BarbarianCall.Menus
{
    internal class MenuHandler
    {
        internal static void ItemSelectHandler(UIMenu sender, UIMenuItem selected, int index)
        {
            if (sender == MainMenu.BarbarianCallMenu)
            {
                if (selected == MainMenu.setting)
                {
                    sender.Close(false);
                    "Opening pause menu".ToLog();
                    DateTime start = DateTime.Now;
                    string headshot = Game.LocalPlayer.Character.GetPedHeadshotTexture(out uint? pmh);
                    "Requesting ped headshot for pause menu".ToLog();
                    PauseMenu.playerMugshotHandle = pmh;
                    "Setting pause menu photo with player mugshot".ToLog();
                    PauseMenu.pauseMenu.Photo = new Sprite(headshot, headshot, Point.Empty, Size.Empty);
                    $"Set photo took {(DateTime.Now - start).TotalMilliseconds} ms".Print();
                    "Set pause menu to visible".ToLog();
                    PauseMenu.pauseMenu.Visible = true;
                }
                else if (selected == MainMenu.mechanic)
                {
                    if ((selected as UIMenuListScrollerItem<string>).SelectedItem == "My Vehicle")
                    {
                        if (Game.LocalPlayer.Character.IsInAnyVehicle(false))
                        {
                            Game.DisplayNotification("~b~Please leave any vehicle first");
                            return;
                        }
                        if (Game.LocalPlayer.Character.LastVehicle)
                        {
                            Mechanic mechanic = new Mechanic(Game.LocalPlayer.Character.LastVehicle);
                            mechanic.RespondToLocation();
                        }
                        else "Your last vehicle is not found, please make sure you has been in any vehicle before".DisplayNotifWithLogo("~y~Mechanic Service");
                    }
                    else
                    {
                        Vehicle brokenVeh = Game.LocalPlayer.Character.GetNearbyVehicles(8).Where(v => v && !v.HasOccupants && v.IsOnScreen && (v.IsCar || v.IsBike) && !v.IsBig && 
                        !Mechanic.VehicleQueue.Contains(v) && !v.IsInAir)
                            .OrderBy(v => v.DistanceTo(Game.LocalPlayer.Character)).FirstOrDefault();
                        if (brokenVeh)
                        {
                            Mechanic mechanic = new Mechanic(brokenVeh);
                            mechanic.AlwaysWork = true;
                            mechanic.RespondToLocation();
                        }
                        else Game.DisplayHelp("No nearby vehicle found to repair");
                    }
                }
            }
        }
    }
}
