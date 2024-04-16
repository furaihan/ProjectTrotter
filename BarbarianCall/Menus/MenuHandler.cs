using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Rage;
using System.Drawing;
using BarbarianCall.SupportUnit;
using BarbarianCall.Types;
using BarbarianCall.Extensions;
using BarbarianCall.MyPed;

namespace BarbarianCall.Menus
{
    internal class MenuHandler
    {
        private static Ped PlayerPed => Game.LocalPlayer.Character;
        internal static void ItemSelectHandler(UIMenu sender, UIMenuItem selected, int index)
        {
            $"Menu Item Pressed: {sender.TitleText} - {selected.Text}, {index}".ToLog();
            if (sender == MainMenu.BarbarianCallMenu)
            {
                if (selected == MainMenu.setting)
                {
                    sender.Close(false);
                    "Opening pause menu".ToLog();
                    DateTime start = DateTime.Now;
                    string headshot = PlayerPed.GetPedHeadshotTexture(out uint? pmh);
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
                        if (PlayerPed.IsInAnyVehicle(false))
                        {
                            Game.DisplayNotification("~b~Please leave any vehicle first");
                            return;
                        }
                        if (PlayerPed.LastVehicle)
                        {
                            Mechanic mechanic = new(PlayerPed.LastVehicle);
                            mechanic.DismissFixedVehicle = false;
                            mechanic.SuccessProbability = 1f;
                            mechanic.RespondToLocation();
                        }
                        else "Your last vehicle is not found, please make sure you has been in any vehicle before".DisplayNotifWithLogo("~y~Mechanic Service");
                    }
                    else
                    {
                        Vehicle brokenVeh = PlayerPed.GetNearbyVehicles(8).Where(v => v && !v.HasOccupants && !v.HasDriver && v.IsOnScreen && (v.IsCar || v.IsBike) && !v.IsBig && 
                        !Mechanic.VehicleQueue.Contains(v) && !v.IsInAir && v.DistanceTo(Game.LocalPlayer.Character) < 15f)
                            .OrderBy(v => v.DistanceToSquared(Game.LocalPlayer.Character)).FirstOrDefault();
                        if (brokenVeh)
                        {
                            Mechanic mechanic = new(brokenVeh);
                            mechanic.RespondToLocation();
                        }
                        else Game.DisplayHelp("No nearby vehicle found to repair");
                    }
                }
                else if (selected == MainMenu.cargobobServices)
                {
                    sender.Close(false);
                    if (MainMenu.cargobobServices.SelectedItem)
                    {
                        CargobobServices cargobobServices = new(MainMenu.cargobobServices.SelectedItem);
                    }
                }
#if DEBUG
                else if (selected == MainMenu.spawnFreemode)
                {
                    Ped player = Game.LocalPlayer.Character;
                    Vector3 pos = player.Position + player.ForwardVector * 8f;
                    float heading = player.Heading + 180f;
                    FreemodePed ped = new(pos, heading, (selected as UIMenuListScrollerItem<string>).SelectedItem.ToLower() == "male");
                    ped.Metadata.BAR_Entity = true;
                    if (ped) ped.Dismiss();
                }
                else if (selected.Text.ToLower().Contains("raycast"))
                {
                    var rcast = selected as UIMenuCheckboxItem;
                    GameFiber.StartNew(() =>
                    {
                        if (rcast.Checked)
                        {
                            Stopwatch sw = new();
                            sw.Start();
                            Vector2 center = new(0, 0);
                            var fpos = MathExtension.RaycastGameplayCamForCoord(center, Game.LocalPlayer.Character);
                            Checkpoint checkpoint = new(CheckpointIcon.Cylinder3, fpos, 2, 250, Color.HotPink, Color.Wheat, true);
                            while (rcast.Checked)
                            {
                                GameFiber.Yield();
                                checkpoint.Position = MathExtension.RaycastGameplayCamForCoord(center, Game.LocalPlayer.Character);
                                if (sw.ElapsedMilliseconds > 300000 || Game.IsScreenFadingOut || PlayerPed.IsInAnyVehicle(false))
                                {
                                    Game.LogTrivial("[BARBARIANCALL]: Breaking loop" + $" {sw.ElapsedMilliseconds > 300000} {Game.IsScreenFadingOut} {PlayerPed.IsInAnyVehicle(false)}");
                                    break;
                                }
                            }
                            Game.LogTrivial($"Last Position: {MathExtension.RaycastGameplayCamForCoord(center, Game.LocalPlayer.Character)}");
                            if (checkpoint) checkpoint.Delete();
                            rcast.Checked = false;
                        }
                    });
                }
                else if (selected.Text.ToLower().Contains("solicitation"))
                {
                    GameFiber.StartNew(() =>
                    {
                        selected.Enabled = false;
                        var pos = SpawnManager.GetSolicitationSpawnpoint(PlayerPed.Position, out Spawnpoint nodePos, out Spawnpoint roadSidePos);
                        if (pos != Spawnpoint.Zero)
                        {
                            Checkpoint checkpoint1 = new(CheckpointIcon.CylinderTripleArrow5, pos, 2f, 250, Color.Red, Color.HotPink, true);
                            Checkpoint checkpoint2 = new(CheckpointIcon.CylinderTripleArrow5, nodePos, 2f, 250, Color.Green, Color.HotPink, true);
                            Checkpoint checkpoint3 = new(CheckpointIcon.CylinderTripleArrow5, roadSidePos, 2f, 250, Color.Blue, Color.HotPink, true);
                            Blip blip = new(pos)
                            {
                                Color = Color.Yellow,
                            };
                            blip.EnableRoute(Color.Yellow);
                            bool arrive = false;
                            while (true)
                            {
                                GameFiber.Yield();
                                if (PlayerPed.DistanceTo(pos.Position) > 1500f || GenericUtils.CheckKey(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.Tab))
                                {
                                    break;
                                }
                                if (!arrive && PlayerPed.DistanceTo(pos.Position) < 10f)
                                {
                                    arrive = true;
                                    Game.DisplaySubtitle("~r~Position~s~, ~g~Node Position~s~, ~b~Roadside Position~s~");
                                }
                            }
                            if (blip) blip.Delete();
                            if (checkpoint1) checkpoint1.Delete();
                            if (checkpoint2) checkpoint2.Delete();
                            if (checkpoint3) checkpoint3.Delete();
                        }
                        else Game.DisplaySubtitle("~r~Solicitation spawn is not found");
                        selected.Enabled = true;
                    });
                }
#endif
            }
        }
        internal static void MenuOpenHandler(UIMenu sender)
        {
            if (sender == MainMenu.BarbarianCallMenu)
            {
                var vehicles = CargobobServices.GetVehicles();
                if (!vehicles.Any() || vehicles.All(x=> !x))
                {
                    MainMenu.cargobobServices.Enabled = false;
                    MainMenu.cargobobServices.Items = new List<Vehicle>() { null };
                    MainMenu.cargobobServices.Description = "No suitable vehicle found";
                    return;
                }
                MainMenu.cargobobServices.Items = vehicles;
                MainMenu.cargobobServices.Enabled = true;
                MainMenu.cargobobServices.Index = 0;
                var x = MainMenu.cargobobServices.SelectedItem;
                MainMenu.cargobobServices.Description = $"Call a cargobob to tow the selected vehicle. Selected vehicle is {x.GetMakeName() + " " + x.GetDisplayName()}. " + $"({x.DistanceTo(Game.LocalPlayer.Character):0.00} meters from local player)";
            }
        }
        internal static void MenuItemIndexChangeHandler(UIMenuScrollerItem sender, int oldIndex, int newIndex)
        {
            if (sender == MainMenu.cargobobServices)
            {
                var x = sender as UIMenuListScrollerItem<Vehicle>;
                sender.Description = 
                    x.SelectedItem ?
                    $"Call a cargobob to tow the selected vehicle. Selected vehicle is {x.SelectedItem.GetMakeName() + " " + x.SelectedItem.GetDisplayName()}. " + $"({x.SelectedItem.DistanceTo(Game.LocalPlayer.Character):0.00} meters from local player)" :
                    "Selected vehicle does not exist";
            }
            else if (sender == MainMenu.mechanic)
            {
                MainMenu.mechanic.Description = $"Call mechanic to repair ~y~{MainMenu.mechanic.SelectedItem}~s~";
            }
        }
    }
}
