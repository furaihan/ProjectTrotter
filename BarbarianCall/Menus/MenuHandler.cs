using System;
using System.Collections.Generic;
using System.Linq;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Rage;
using System.Drawing;
using BarbarianCall.SupportUnit;
using BarbarianCall.Types;
using BarbarianCall.Extensions;

namespace BarbarianCall.Menus
{
    internal class MenuHandler
    {
        private static Ped PlayerPed => Game.LocalPlayer.Character;
        internal static void ItemSelectHandler(UIMenu sender, UIMenuItem selected, int index)
        {
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
                            .OrderBy(v => v.DistanceTo(Game.LocalPlayer.Character)).FirstOrDefault();
                        if (brokenVeh)
                        {
                            Mechanic mechanic = new(brokenVeh);
                            mechanic.RespondToLocation();
                        }
                        else Game.DisplayHelp("No nearby vehicle found to repair");
                    }
                }
#if DEBUG
                else if (selected == MainMenu.spawnFreemode)
                {
                    Ped player = Game.LocalPlayer.Character;
                    Vector3 pos = player.Position + player.ForwardVector * 8f;
                    float heading = player.Heading + 180f;
                    Freemode.FreemodePed ped = new(pos, heading, (selected as UIMenuListScrollerItem<string>).SelectedItem.ToLower() == "male" ? LSPD_First_Response.Gender.Male : LSPD_First_Response.Gender.Female);
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
                            System.Diagnostics.Stopwatch sw = new();
                            sw.Start();
                            Vector2 center = new(0, 0);
                            var fpos = MathExtension.RaycastGameplayCamForCoord(center, Game.LocalPlayer.Character);
                            Checkpoint checkpoint = new(Checkpoint.CheckpointIcon.Cyclinder3, fpos, 2, 250, Color.HotPink, Color.Wheat, true);
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
                            Checkpoint checkpoint1 = new(Checkpoint.CheckpointIcon.CylinderTripleArrow5, pos, 2f, 250, Color.Red, Color.HotPink, true);
                            Checkpoint checkpoint2 = new(Checkpoint.CheckpointIcon.CylinderTripleArrow5, nodePos, 2f, 250, Color.Green, Color.HotPink, true);
                            Checkpoint checkpoint3 = new(Checkpoint.CheckpointIcon.CylinderTripleArrow5, roadSidePos, 2f, 250, Color.Blue, Color.HotPink, true);
                            Blip blip = new(pos)
                            {
                                Color = Color.Yellow,
                            };
                            blip.EnableRoute(Color.Yellow);
                            bool arrive = false;
                            while (true)
                            {
                                GameFiber.Yield();
                                if (PlayerPed.DistanceTo(pos.Position) > 1500f || Peralatan.CheckKey(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.Tab))
                                {
                                    break;
                                }
                                if (!arrive && PlayerPed.DistanceTo(pos.Position) < 10f)
                                {
                                    arrive = true;
                                    Game.DisplaySubtitle("~r~Position~s~, ~g~Node Position~s~, ~b~Roadside Position~s~");
                                    List<string> log = new()
                                    {
                                        $"Ahead: {pos.IsAheadPosition(PlayerPed, MathHelper.ConvertHeadingToDirection(PlayerPed.Heading))} Behind: {pos.IsBehindPosition(PlayerPed, MathHelper.ConvertHeadingToDirection(PlayerPed.Heading))} => Position",
                                        $"Ahead: {nodePos.IsAheadPosition(PlayerPed, MathHelper.ConvertHeadingToDirection(PlayerPed.Heading))} Behind: {nodePos.IsBehindPosition(PlayerPed, MathHelper.ConvertHeadingToDirection(PlayerPed.Heading))} => Node Position",
                                        $"Ahead: {roadSidePos.IsAheadPosition(PlayerPed, MathHelper.ConvertHeadingToDirection(PlayerPed.Heading))} Behind: {roadSidePos.IsBehindPosition(PlayerPed, MathHelper.ConvertHeadingToDirection(PlayerPed.Heading))} => Roadside Position",
                                    };
                                    log.ForEach(Peralatan.ToLog);
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
    }
}
