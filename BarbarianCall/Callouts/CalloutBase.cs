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
        public static List<Blip> CalloutBlips { get; protected set; } = new List<Blip>();
        public static List<Entity> CalloutEntities { get; protected set; } = new List<Entity>();
        public ECalloutStates CalloutStates;
        public Ped Suspect;
        public Vehicle SuspectCar;
        public Types.Manusia Manusia;
        public Model CarModel;
        public Blip Blip;
        public bool CalloutRunning = false;
        public Vector3 SpawnPoint = Vector3.Zero;
        public float SpawnHeading = 0f;
        public SpawnPoint Spawn = BarbarianCall.SpawnPoint.Zero;
        public long Timer;
        public DateTime Time;
        public TimeSpan TimeSpan = new TimeSpan(0, 0, 15);
        public LHandle Pursuit;
        public LHandle PullOver;
        public bool PursuitCreated = false;
        public bool GrammarPoliceRunning  = false;
        public bool StopThePedRunning = false;
        public bool UltimateBackupRunning = false;
        public Persona SuspectPersona;
        public Ped PlayerPed = Game.LocalPlayer.Character;
        public GameFiber CalloutMainFiber;
        public string FilePath;
        public static readonly uint[] WeaponHashes = { 0x1B06D571, 0xBFE256D4, 0x5EF9FEC4, 0x22D8FE39, 0x99AEEB3B, 0x2B5EF5EC, 0x78A97CD0, 0x1D073A89, 0x555AF99A, 0xBD248B55, 0x13532244, 0x624FE830 };
        #endregion

        public override void OnCalloutNotAccepted()
        {
            CalloutRunning = false;
            PursuitCreated = false;
            if (Suspect) Suspect.Delete();
            if (SuspectCar) SuspectCar.Delete();
            if (Blip) Blip.Delete();
            if (GrammarPoliceRunning) API.GrammarPoliceFunc.SetStatus(API.GrammarPoliceFunc.EGrammarPoliceStatusType.Available, true, false);
            Types.Manusia.CurrentManusia = null;
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
            $"Callout Created From {CreationSource}".ToLog();
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            CalloutStates = ECalloutStates.EnRoute;
            HandleEnd();
            GameFiber.Sleep(75);
            return base.OnCalloutAccepted();
        }
        public override void OnCalloutDisplayed()
        {
            base.OnCalloutDisplayed();
        }
        public override void End()
        {
            CalloutRunning = false;
            Peralatan.Speaking = false;
            if (Suspect && !Functions.IsPedArrested(Suspect)) Suspect.Dismiss();
            if (SuspectCar) SuspectCar.Dismiss();
            if (Blip) Blip.Delete();
            if (GrammarPoliceRunning && Menus.PauseMenu.autoAvailable.Checked) API.GrammarPoliceFunc.SetStatus(API.GrammarPoliceFunc.EGrammarPoliceStatusType.Available);
            if (Pursuit != null && Functions.IsPursuitStillRunning(Pursuit)) Functions.ForceEndPursuit(Pursuit);
            Types.Manusia.CurrentManusia = null;
            CalloutBlips.ForEach(b => { if (b) b.Delete(); });
            CalloutEntities.ForEach(e =>
            {
                if (e)
                {
                    if (e.Model.IsPed && e.IsAlive && !Functions.IsPedArrested((Ped)e)) e.Dismiss();
                    else if (e.Model.IsPed && e.IsAlive && Functions.IsPedArrested((Ped)e)) e.IsInvincible = false;
                    else if (e.Model.IsVehicle) e.Dismiss();
                    else if (e.Model.IsObject) e.Delete();
                    else e.Dismiss();
                }
            });
            base.End();              
        }
        protected void HandleEnd()
        {
            "Starting EndHandler loop".ToLog();
            GameFiber.StartNew(() =>
            {
                DateTime dateTime = DateTime.Now;
                while (CalloutRunning)
                {
                    GameFiber.Yield();
                    if (Peralatan.CheckKey(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.End))
                    {
                        GameFiber.Sleep(300);
                        if (Game.IsKeyDownRightNow(System.Windows.Forms.Keys.End))
                        {
                            GameFiber.Sleep(1850);
                            if (Game.IsKeyDownRightNow(System.Windows.Forms.Keys.End)) End();
                        }
                        else Game.DisplayHelp($"~y~To force end the callout, press and hold down {Peralatan.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.End)}~y~ for 2 second");
                    }
                }
                $"Callout ended successfully, that callout took {(DateTime.Now - dateTime).TotalSeconds:0.00} seconds".ToLog();
            }, "[BarbarianCall] Callout End Handler Fiber");
        }
        protected void DisplayGPNotif()
        {
            if (Initialization.IsLSPDFRPluginRunning("GrammarPolice"))
            {
                "If you have GrammarPolice installed, you can ask dispatch to display the suspect detail ~y~e.g. ~b~\"Dispatch requesting suspect details\"".DisplayNotifWithLogo("~y~" + GetType().Name + "~s~");
            }
        }
        protected void PlayScannerWithCallsign(string audio)
        {
            if (Initialization.IsLSPDFRPluginRunning("GrammarPolice", new Version(1, 4, 2, 2))) Functions.PlayScannerAudio("DISP_ATTENTION_UNIT " + API.GrammarPoliceFunc.GetCallsignAudio() + " " + audio);
            else Functions.PlayScannerAudio($"ATTENTION_ALL_UNITS {audio}");
        }
        protected void PlayScannerWithCallsign(string audio, Vector3 position)
        {
            if (Initialization.IsLSPDFRPluginRunning("GrammarPolice", new Version(1, 4, 2, 2))) 
                Functions.PlayScannerAudioUsingPosition("DISP_ATTENTION_UNIT " + API.GrammarPoliceFunc.GetCallsignAudio() + " " + audio, position);
            else Functions.PlayScannerAudioUsingPosition($"ATTENTION_ALL_UNITS {audio}", position);
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
