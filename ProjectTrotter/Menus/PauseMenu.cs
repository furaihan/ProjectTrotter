﻿using System.Collections.Generic;
using System.Windows.Forms;
using Rage;
using RAGENativeUI;
using RAGENativeUI.PauseMenu;
using RAGENativeUI.Elements;

namespace ProjectTrotter.Menus
{
    internal class PauseMenu
    {
        internal static TabView pauseMenu;
        internal static TabSubmenuItem calloutMenu;
        internal static TabInteractiveListItem pluginSettingMenu;

        internal static UIMenuCheckboxItem autoAvailable;
        internal static UIMenuCheckboxItem otherUnitAudio;
        internal static UIMenuCheckboxItem onSceneAudio;
        internal static UIMenuCheckboxItem offStabCB;
        internal static UIMenuCheckboxItem taxiCB;
        internal static UIMenuCheckboxItem susVehCB;
        internal static uint? playerMugshotHandle;

        internal static void CreatePauseMenu()
        {
            "Creating pause menu".ToLog();
            pauseMenu = new TabView("~g~BarbarianCall")
            {
                MoneySubtitle = "~b~Los Santos ~g~Police ~y~Department",
                PauseGame = true,
                Name = LSPD_First_Response.Mod.API.Functions.GetPersonaForPed(Game.LocalPlayer.Character).FullName,
            };          
            pauseMenu.OnMenuClose += (s, e) => playerMugshotHandle.UnregisterPedHeadshot();

            List<TabItem> CalloutList = new();

            TabItem officerStabbedCall = new TabInteractiveListItem("Officer Stabbed", Callouts.CalloutBase.CreateMenu());
            CalloutList.Add(officerStabbedCall);

            TabItem refuseTaxiCall = new TabInteractiveListItem("Taxi Passenger Refuse To Pay", Callouts.CalloutBase.CreateMenu());
            CalloutList.Add(refuseTaxiCall);

            TabItem suspiciousVehCall = new TabInteractiveListItem("Strange Looking Vehicle", Callouts.CalloutBase.CreateMenu());
            CalloutList.Add(suspiciousVehCall);

            calloutMenu = new TabSubmenuItem("Callout Settings", CalloutList);
            pauseMenu.AddTab(calloutMenu);
            "Done creating callouts tab".ToLog();
            
            autoAvailable = new UIMenuCheckboxItem("Set Player Available After Finishing A Callout", true, "If you have GrammarPolice installed, enable this setting will set you " +
                "available for calls after you have finished your callout");
            otherUnitAudio = new UIMenuCheckboxItem("Play Other Unit Respond Audio", true, "If you did not accept a callout from this plugin, a sound from other unit taking a callout will be played");
            onSceneAudio = new UIMenuCheckboxItem("Play Officer On Scene Audio", true, "Play scanner audio when you arrived on scene");
            offStabCB = new UIMenuCheckboxItem("Officer Stabbed", true, "Enable or disable \"Officer Stabbed\" callout");
            taxiCB = new UIMenuCheckboxItem("Taxi Passenger Refuse To Pay", true, "Enable or disable \"Taxi Passenger Refuse To Pay\" callout");
            susVehCB = new UIMenuCheckboxItem("Strange Looking Vehicle", true, "Enable or disable \"Strange Looking Vehicle\" callout");
            UIMenuItem gpSection = new("Grammar Police Integration");
            SetUIMenuAsSection(gpSection);
            List<UIMenuItem> settings = new()
            {
                gpSection, autoAvailable, otherUnitAudio, onSceneAudio, offStabCB, taxiCB, susVehCB
            };

            pluginSettingMenu = new TabInteractiveListItem("Plugin Settings", settings);
            pluginSettingMenu.Items.ForEach(ui =>
            {
                ui.Activated += (m, s) => Game.DisplaySubtitle(s.Description);
            });
            pauseMenu.AddTab(pluginSettingMenu);
            "Creating pause menu is nearly done".ToLog();
            pauseMenu.RefreshIndex();
            "Refreshing pause menu index".ToLog();
            MainMenu.CreateMenu();
            ProcessMenus();
        }
        internal static List<UIMenuItem> sectionItem = new();
        private static void SetUIMenuAsSection(UIMenuItem menuItem)
        {
            menuItem.TextStyle = new TextStyle(TextFont.HouseScript, menuItem.ForeColor, 0.45f);
            menuItem.Enabled = false;
            menuItem.BackColor = HudColor.NetPlayer10Dark.GetColor();
            menuItem.HighlightedBackColor = HudColor.NetPlayer10.GetColor();
            menuItem.RightBadge = UIMenuItem.BadgeStyle.Heart;
            menuItem.LeftBadge = UIMenuItem.BadgeStyle.Heart;
            sectionItem.Add(menuItem);
        }
        internal static void ProcessMenus()
        {
            "Drawing texture with game raw framerender".ToLog();
            Game.RawFrameRender += (s, e) => pauseMenu.DrawTextures(e.Graphics);
            "Starting menu loop".ToLog();
            while (true)
            {
                GameFiber.Yield();
                pauseMenu.Update();
                MainMenu.Pool.ProcessMenus();
                if (!UIMenu.IsAnyMenuVisible && GenericUtils.CheckKey(Keys.RControlKey, Keys.D0))
                {
                    MainMenu.BarbarianCallMenu.Visible = !MainMenu.BarbarianCallMenu.Visible;
                }
                if (MainMenu.BarbarianCallMenu.Visible && MainMenu.cargobobServices.Selected && MainMenu.cargobobServices.SelectedItem)
                {
                    Vehicle x = MainMenu.cargobobServices.SelectedItem;
                    Types.Marker.DrawMarker(MarkerType.UpsideDownCone, x.Position + new Vector3(0f, 0f, x.Height / 1.5f), HudColor.Orange.GetColor());
                }
            }
        }
    }
}
