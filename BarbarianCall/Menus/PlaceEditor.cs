using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
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
        internal static UIMenuListScrollerItem<string> VehicleEntry;
        internal static UIMenuListScrollerItem<string> PedEntry;
        internal static UIMenuListScrollerItem<string> VehicleExit;
        internal static UIMenuListScrollerItem<string> PedExit;
        internal static List<Place> Places = new();
        internal static UIMenuListScrollerItem<Place> PlaceList;
        internal static UIMenuItem ExportXml;
        private static Ped PlayerPed => Game.LocalPlayer.Character;
        public static void CreateMenu()
        {
            PlaceEditorMenu = new("Place Editor", "Create or edit a place")
            {
                AllowCameraMovement = true,
                MouseControlsEnabled = false,
                WidthOffset = 200,
                TitleStyle = new TextStyle(TextFont.Monospace, Color.Aquamarine, 1.05f, TextJustification.Center)
            };
            PlaceEditorMenu.SetBannerType(new Sprite("vbblockgroup7+hi", "_ml_bvpfloor01", Point.Empty, Size.Empty));
            MainMenu.Pool.Add(PlaceEditorMenu);
            PlaceList = new UIMenuListScrollerItem<Place>("Place List", "List of places that you have created in this session")
            {
                Formatter = m => m.Name,
                BackColor = Color.FromArgb(120, HudColor.FranklinDark.GetColor()),
                ForeColor = HudColor.NetPlayer27.GetColor(),
                HighlightedBackColor = Color.FromArgb(120, HudColor.Franklin.GetColor()),
                HighlightedForeColor = HudColor.NetPlayer30.GetColor(),
            };
            VehicleEntry = new UIMenuListScrollerItem<string>("Vehicle Entrance", "Modify your vehicle entrance", new[] { "Add", "Remove Latest" });
            PedEntry = new UIMenuListScrollerItem<string>("Ped Entrance", "Modify your ped entrance", new[] { "Add", "Remove Latest" });
            VehicleExit = new UIMenuListScrollerItem<string>("Vehicle Exits", "Modify your vehicle exits", new[] { "Add", "Remove Latest" });
            PedExit = new UIMenuListScrollerItem<string>("Ped Exits", "Modify your Ped exits", new[] { "Add", "Remove Latest" });
            CreateNew = new UIMenuItem("Create New Place");
            ExportXml = new UIMenuItem("Export Xml", "Export your created place(s) to an xml file");
            PlaceList.Enabled = false;
            VehicleEntry.Enabled = false;
            PedEntry.Enabled = false;
            VehicleExit.Enabled = false;
            PedExit.Enabled = false;
            ExportXml.Enabled = false;
            PlaceXmlMenu.CreateMenu();
            PlaceEditorMenu.AddItems(PlaceList, VehicleEntry, PedEntry, VehicleExit, PedExit, CreateNew);
            PlaceEditorMenu.AddItem(ExportXml);
            PlaceEditorMenu.RefreshIndex();
            CreateNew.Activated += OnCreateNew;
            PlaceEditorMenu.OnItemSelect += MenuHandler.ItemSelectHandler;
            PlaceEditorMenu.OnMenuClose += PlaceEditorMenu_OnMenuClose;
            LocationCheckpointHandler();
            KeyListener();
        }

        private static void PlaceEditorMenu_OnMenuClose(UIMenu sender) => CpPlace?.ForEach(cp => { if (cp) cp?.Delete(); });

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
                if (Places.Any() && Places.Any(p=> p.Name == name))
                {
                    $"{name} Place is already ~o~exist~s~, try another name".DisplayNotifWithLogo(name, "CHAR_BLOCKED", "CHAR_BLOCKED", "Place Editor");
                    return;
                }
                Place place = new(name, PlayerPed.IsInAnyVehicle(false) ? PlayerPed.CurrentVehicle.Position : PlayerPed.Position);
                Places.Add(place);
                if (Places.Any())
                {
                    PlaceList.Items = Places;
                    PlaceList.SelectedItem = place;
                    PlaceList.Enabled = true;
                    VehicleEntry.Enabled = true;
                    PedEntry.Enabled = true;
                    VehicleExit.Enabled = true;
                    PedExit.Enabled = true;
                    ExportXml.Enabled = true;
                    PlaceEditorMenu.RefreshIndex();
                }
            });
        }
        public static void KeyListener()
        {
            Peralatan.ToLog("Start listening place editor keys");
            while (true)
            {
                GameFiber.Yield();
                if (Peralatan.CheckKey(Keys.None, Keys.PageUp))
                {
                    PlaceEditorMenu.Visible = !PlaceEditorMenu.Visible;
                }
                if (PlaceXmlMenu.XmlMenu.Visible && PlaceXmlMenu.PlaceList.Any())
                {
                    if (Peralatan.CheckKey(Keys.LControlKey, Keys.A))
                    {
                        PlaceXmlMenu.PlaceList.Keys.ToList().ForEach(x => x.Checked = true);
                    }
                    if (Peralatan.CheckKey(Keys.LShiftKey, Keys.A))
                    {
                        PlaceXmlMenu.PlaceList.Keys.ToList().ForEach(x => x.Checked = false);
                    }
                }
            }
        }
        private static readonly List<Checkpoint> CpPlace = new(); 
        private static void LocationCheckpointHandler()
        {
            GameFiber.StartNew(() =>
            {
                while (true)
                {
                    GameFiber.Yield();
                    if (PlaceEditorMenu.Visible)
                    {
                        if (VehicleEntry.Selected)
                        {
                            CpPlace.ForEach(cp => { if (cp) cp?.Delete(); });
                            if (PlaceList.SelectedItem.VehicleEntrance.Any())
                            {
                                PlaceList.SelectedItem.VehicleEntrance.ForEach(sp =>
                                {
                                    Checkpoint checkpoint = new(CheckpointIcon.Cylinder3, sp, 2f, 5f, Color.Crimson, Color.Pink, true);
                                    CpPlace.Add(checkpoint);
                                });
                                while (VehicleEntry.Selected && PlaceEditorMenu.Visible) GameFiber.Yield();
                            }
                        }
                        else if (PedEntry.Selected)
                        {
                            CpPlace.ForEach(cp => { if (cp) cp?.Delete(); });
                            if (PlaceList.SelectedItem.PedEntrance.Any())
                            {
                                PlaceList.SelectedItem.PedEntrance.ForEach(sp =>
                                {
                                    Checkpoint checkpoint = new(CheckpointIcon.Cylinder3, sp, 2f, 5f, Color.Crimson, Color.Pink, true);
                                    CpPlace.Add(checkpoint);
                                });
                                while (PedEntry.Selected && PlaceEditorMenu.Visible) GameFiber.Yield();
                            }
                        }
                        else if (VehicleExit.Selected)
                        {
                            CpPlace.ForEach(cp => { if (cp) cp?.Delete(); });
                            if (PlaceList.SelectedItem.VehicleExits.Any())
                            {
                                PlaceList.SelectedItem.VehicleExits.ForEach(sp =>
                                {
                                    Checkpoint checkpoint = new(CheckpointIcon.Cylinder3, sp, 2f, 5f, Color.Crimson, Color.Pink, true);
                                    CpPlace.Add(checkpoint);
                                });
                                while (VehicleExit.Selected && PlaceEditorMenu.Visible) GameFiber.Yield();
                            }
                        }
                        else if (PedExit.Selected)
                        {
                            CpPlace.ForEach(cp => { if (cp) cp?.Delete(); });
                            if (PlaceList.SelectedItem.PedExits.Any())
                            {
                                PlaceList.SelectedItem.PedExits.ForEach(sp =>
                                {
                                    Checkpoint checkpoint = new(CheckpointIcon.Cylinder3, sp, 2f, 5f, Color.Crimson, Color.Pink, true);
                                    CpPlace.Add(checkpoint);
                                });
                                while (PedExit.Selected && PlaceEditorMenu.Visible) GameFiber.Yield();
                            }
                        }
                    }
                }
            });
        }
    }
}
