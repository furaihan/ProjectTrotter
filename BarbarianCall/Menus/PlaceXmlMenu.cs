using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using BarbarianCall.Types;

namespace BarbarianCall.Menus
{
    internal class PlaceXmlMenu
    {
        internal static UIMenu XmlMenu;
        internal static UIMenuItem BeginExport;
        internal static Dictionary<UIMenuCheckboxItem, Place> PlaceList = new();
        internal static void CreateMenu()
        {
            XmlMenu = new UIMenu("Export Xml", "Export your created place(s) to an xml file")
            {
                AllowCameraMovement = true,
                MouseControlsEnabled = false,
                WidthOffset = 200,
                TitleStyle = new TextStyle(TextFont.Monospace, Color.Aquamarine, 1.05f, TextJustification.Center)
            };
            XmlMenu.SetBannerType(new Sprite("vbblockgroup7+hi", "_ml_bvpfloor01", Point.Empty, Size.Empty));
            MainMenu.Pool.Add(XmlMenu);
            BeginExport = new UIMenuItem("Begin Export", "Start export your selected place to an xml file")
            {
                BackColor = HudColor.BlueDark.GetColor(),
                ForeColor = Color.White,
                HighlightedBackColor = HudColor.BlueLight.GetColor(),
                HighlightedForeColor = Color.Black,
                LeftBadge = UIMenuItem.BadgeStyle.Star
            };
            BeginExport.Activated += BeginExport_Activated;
            XmlMenu.OnMenuOpen += XmlMenu_OnMenuOpen;
            var dump = new UIMenuItem("---XML EXPORT---")
            {
                Enabled = false,
                TextStyle = TextStyle.Default.With(scale: 0.40f, font: TextFont.ChaletLondon, justification: TextJustification.Center),
                RightBadge = UIMenuItem.BadgeStyle.GoldMedal,
                LeftBadge = UIMenuItem.BadgeStyle.GoldMedal
            };
            XmlMenu.AddItem(dump);
            InstructionalButtonGroup selectAll = new("Select All", Keys.LControlKey.GetInstructionalId(), Keys.A.GetInstructionalId());
            InstructionalButtonGroup deselectAll = new("Select All", Keys.LShiftKey.GetInstructionalId(), Keys.A.GetInstructionalId());
            XmlMenu.AddInstructionalButton(selectAll);
            XmlMenu.AddInstructionalButton(deselectAll);
            Peralatan.ToLog("Bind the xml menu item to xml menu");
            PlaceEditor.PlaceEditorMenu.BindMenuToItem(XmlMenu, PlaceEditor.ExportXml);
            XmlMenu.RefreshIndex();
        }

        private static void XmlMenu_OnMenuOpen(UIMenu sender)
        {
            if (PlaceEditor.Places.Any())
            {
                foreach (Place place in PlaceEditor.Places)
                {
                    if (XmlMenu.MenuItems.Any(x => x.Text == place.Name)) continue;
                    UIMenuCheckboxItem item = new(place.Name, true);
                    PlaceList.Add(item, place);
                    XmlMenu.AddItem(item);
                }
                if (!XmlMenu.MenuItems.Contains(BeginExport)) XmlMenu.AddItem(BeginExport);
                XmlMenu.RefreshIndex();
            }
            else sender.Close();
        }

        private static void BeginExport_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            string filename = MenuUtil.GetKeyboardInput("Enter a filename (max 50 charaters, must ended with '.xml' extension)", "", 50);
            while (!filename.ToLower().EndsWith(".xml"))
            {
                GameFiber.Yield();
                filename = MenuUtil.GetKeyboardInput("Invalid filename (must end with '.xml' extension)", filename, 50);
            }
            string rootXml = MenuUtil.GetKeyboardInput("Enter xml root name", "PlaceList", 25);
            string path = Path.Combine("Plugins", "LSPDFR", "BarbarianCall", filename);
            if (File.Exists(path))
            {

            }
            List<Place> places = PlaceList.Where(x => x.Key.Checked).Select(x => x.Value).ToList();
            XDocument xml = new(new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(rootXml, from p in places select p.ToXmlElement()));
            xml.Save(path);
        }
    }
}
