using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Native;
using LSPD_First_Response.Engine.Scripting.Entities;
using RAGENativeUI.Elements;

namespace BarbarianCall.Callouts
{
    public abstract class CalloutBase : Callout
    {
        #region Fields
        public enum ESuspectStates { InAction, Arrested, Dead, Escaped };
        public enum ECalloutStates { UnAccepted, EnRoute, OnScene, InPursuit, Finish };
        public ECalloutStates CalloutStates;
        public Ped Suspect;
        public Vehicle SuspectCar;
        public Blip Blip;
        public bool CalloutRunning = false;
        public Vector3 SpawnPoint = Vector3.Zero;
        public float SpawnHeading = 0f;
        public long Timer;
        public DateTime Time;
        public LHandle Pursuit;
        public LHandle PullOver;
        public bool PursuitCreated = false;
        public bool GrammarPoliceRunning  = false;
        public bool StopThePedRunning = false;
        public bool UltimateBackupRunning = false;
        public Persona SuspectPersona;
        public Ped PlayerPed = Game.LocalPlayer.Character;
        public GameFiber CalloutMainFiber;
        public string Path;
        public static readonly uint[] WeaponHashes = { 0x1B06D571, 0xBFE256D4, 0x5EF9FEC4, 0x22D8FE39, 0x99AEEB3B, 0x2B5EF5EC, 0x78A97CD0, 0x1D073A89, 0x555AF99A, 0xBD248B55, 0x13532244, 0x624FE830 };
        #endregion

        public override void OnCalloutNotAccepted()
        {
            CalloutRunning = false;
            PursuitCreated = false;
            if (Suspect.Exists()) Suspect.Delete();
            if (SuspectCar.Exists()) SuspectCar.Delete();
            if (Blip.Exists()) Blip.Delete();
            if (GrammarPoliceRunning) API.GrammarPoliceFunc.SetStatus(API.GrammarPoliceFunc.EGrammarPoliceStatusType.Available, true, false);
            Functions.PlayScannerAudio("BAR_AI_RESPOND");
            base.OnCalloutNotAccepted();
        }
        public override bool OnBeforeCalloutDisplayed()
        {
            GrammarPoliceRunning = Initialization.IsLSPDFRPluginRunning("GrammarPolice");
            UltimateBackupRunning = Initialization.IsLSPDFRPluginRunning("UltimateBackup");
            StopThePedRunning = Initialization.IsLSPDFRPluginRunning("StopThePed");
            CalloutRunning = false;
            PursuitCreated = false;
            CalloutStates = ECalloutStates.UnAccepted;
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            CalloutStates = ECalloutStates.EnRoute;

            return base.OnCalloutAccepted();
        }
        public override void OnCalloutDisplayed()
        {
            base.OnCalloutDisplayed();
        }
        public override void End()
        {
            base.End();
            try
            {
                CalloutRunning = false;
                Peralatan.Speaking = false;
                if (Suspect.Exists() && !Functions.IsPedArrested(Suspect)) Suspect.Dismiss();
                if (SuspectCar.Exists()) SuspectCar.Dismiss();
                if (Blip.Exists()) Blip.Delete();
                if (GrammarPoliceRunning) GrammarPolice.API.Functions.Available(false, false);
            }
            catch (Exception e)
            {
                e.ToString().ToLog();
            }
        }
        public static UIMenuItem[] CreateMenu()
        {
            "Creating callout menu tab, menu items".ToLog();
            return new UIMenuItem[]
            {
                new UIMenuNumericScrollerItem<float>("Minimum Distance", "Set the callout minimum distance", 100f, 500f, 20f),
                new UIMenuNumericScrollerItem<float>("Maximum Distance", "Set the callout maximum distance", 500f, 2000f, 50f)
            };
        }
    }
}
