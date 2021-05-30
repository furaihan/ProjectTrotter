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
    public class PlaceXmlMenu
    {
        internal static UIMenu XmlMenu;
        internal static UIMenuItem BeginExport;
        internal static Dictionary<UIMenuCheckboxItem, Place> PlaceList = new Dictionary<UIMenuCheckboxItem, Place>();
        internal static void CreateMenu()
        {
            XmlMenu = new UIMenu("Export Xml", "Export your created place(s) to an xml file")
            {
                AllowCameraMovement = true,
                MouseControlsEnabled = false,
                WidthOffset = 200,
                TitleStyle = new TextStyle(TextFont.Monospace, Color.Aquamarine),
                ParentMenu = PlaceEditor.PlaceEditorMenu,
                ParentItem = PlaceEditor.ExportXml,
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
            InstructionalButtonGroup selectAll = new("Select All", Keys.LControlKey.GetInstructionalId(), Keys.A.GetInstructionalId());
            InstructionalButtonGroup deselectAll = new("Select All", Keys.LShiftKey.GetInstructionalId(), Keys.A.GetInstructionalId());
            XmlMenu.AddInstructionalButton(selectAll);
            XmlMenu.AddInstructionalButton(deselectAll);
            XmlMenu.BindMenuToItem(XmlMenu, PlaceEditor.ExportXml);
        }

        private static void XmlMenu_OnMenuOpen(UIMenu sender)
        {
            if (PlaceEditor.Places.Any())
            {
                foreach (Place place in PlaceEditor.Places)
                {
                    UIMenuCheckboxItem item = new(place.Name, true);
                    PlaceList.Add(item, place);
                    XmlMenu.AddItem(item);
                }
                XmlMenu.AddItem(BeginExport);
                XmlMenu.RefreshIndex();
            }
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
            List<Place> places = PlaceList.Where(x => x.Key.Checked).Select(x => x.Value).ToList();
            XDocument xml = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                                            from p in places
                                            select p.ToXmlElement());
            xml.Save(Path.Combine("Plugins", "LSPDFR", "BarbarianCall", filename));
        }
    }
}
