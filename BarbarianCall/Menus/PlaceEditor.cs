using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        internal static UIMenuItem AddVehicleEntry;
        internal static UIMenuItem AddPedEntry;
        internal static UIMenuItem AddVehicleExit;
        internal static UIMenuItem AddPedExit;
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
                TitleStyle = new TextStyle(TextFont.Monospace, Color.Aquamarine)
            };
            PlaceEditorMenu.SetBannerType(new Sprite("vbblockgroup7+hi", "_ml_bvpfloor01", Point.Empty, Size.Empty));
            MainMenu.Pool.Add(PlaceEditorMenu);
            PlaceList = new UIMenuListScrollerItem<Place>("Place List", "List of places that you have created in this session")
            {
                Formatter = m => m.Name,
                BackColor = Color.FromArgb(120, HudColor.FranklinDark.GetColor()),
                ForeColor = HudColor.NetPlayer27.GetColor(),
                HighlightedBackColor = Color.FromArgb(120, HudColor.Franklin.GetColor()),
                HighlightedForeColor = HudColor.NetPlayer27Dark.GetColor(),
            };
            AddVehicleEntry = new UIMenuItem("Add Vehicle Entrance");
            AddPedEntry = new UIMenuItem("Add Ped Entrance");
            AddVehicleExit = new UIMenuItem("Add Vehicle Exit");
            AddPedExit = new UIMenuItem("Add Ped Exit");
            CreateNew = new UIMenuItem("Create New Place");
            ExportXml = new UIMenuItem("Export Xml", "Export your created place(s) to an xml file");
            PlaceList.Enabled = false;
            AddVehicleEntry.Enabled = false;
            AddPedEntry.Enabled = false;
            AddVehicleExit.Enabled = false;
            AddPedExit.Enabled = false;
            ExportXml.Enabled = false;
            PlaceEditorMenu.AddItems(PlaceList, AddVehicleEntry, AddPedEntry, AddVehicleExit, AddPedExit, CreateNew, ExportXml);
            PlaceEditorMenu.RefreshIndex();
            CreateNew.Activated += OnCreateNew;
            PlaceEditorMenu.OnItemSelect += OnAddPlace;
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
                if (Places.Any() && Places.Any(p=> p.Name == name))
                {
                    $"{name} Place is already ~o~exist~s~, try another name".DisplayNotifWithLogo(name, "CHAR_BLOCKED", "CHAR_BLOCKED", "Place Editor");
                    return;
                }
                Places.Add(new Place(name, PlayerPed.IsInAnyVehicle(false) ? PlayerPed.CurrentVehicle.Position : PlayerPed.Position));
                if (Places.Any())
                {
                    PlaceList.Items = Places;
                    PlaceList.Enabled = true;
                    AddVehicleEntry.Enabled = true;
                    AddPedEntry.Enabled = true;
                    AddVehicleExit.Enabled = true;
                    AddPedExit.Enabled = true;
                    ExportXml.Enabled = true;
                    PlaceEditorMenu.RefreshIndex();
                }
            });
        }
        public static void OnAddPlace(UIMenu uIMenu, UIMenuItem si, int i)
        {
            if (si == AddVehicleEntry)
            {
                if (PlayerPed.IsInAnyVehicle(false))
                {
                    PlaceList.SelectedItem.VehicleEntrance.Add(new Spawnpoint(PlayerPed.CurrentVehicle.Position, PlayerPed.CurrentVehicle.Heading));
                }
                else Game.DisplaySubtitle($"You must be inside vehicle to {si.Text.ToLower()}");
            }
            else if (si == AddPedEntry)
            {
                PlaceList.SelectedItem.PedEntrance.Add(new Spawnpoint(PlayerPed.Position, PlayerPed.Heading));
            }
            else if (si == AddVehicleExit)
            {
                if (PlayerPed.IsInAnyVehicle(false))
                {
                    PlaceList.SelectedItem.VehicleExits.Add(new Spawnpoint(PlayerPed.CurrentVehicle.Position, PlayerPed.CurrentVehicle.Heading));
                }
                else Game.DisplaySubtitle($"You must be inside vehicle to {si.Text.ToLower()}");
            }
            else if (si == AddPedExit)
            {
                PlaceList.SelectedItem.PedExits.Add(new Spawnpoint(PlayerPed.Position, PlayerPed.Heading));
            }
        }
        public static void MenuHandler()
        {
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
    }
}
