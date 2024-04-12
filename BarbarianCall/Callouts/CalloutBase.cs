using System;
using System.Linq;
using System.Collections.Generic;
using LSPD_First_Response.Mod.API;
using Rage;
using LSPD_First_Response.Engine.Scripting.Entities;
using RAGENativeUI.Elements;
using BarbarianCall.Types;

namespace BarbarianCall.Callouts
{
    public abstract class CalloutBase : LSPD_First_Response.Mod.Callouts.Callout
    {
        #region Fields
        public enum ESuspectStates { InAction, Arrested, Dead, Escaped };
        public enum ECalloutStates { UnAccepted, EnRoute, OnScene, InPursuit, Finish };
        public static List<Blip> CalloutBlips { get; protected set; } = new List<Blip>();
        public static List<Entity> CalloutEntities { get; protected set; } = new List<Entity>();
        public static List<Checkpoint> CalloutCheckpoints { get; protected set; } = new List<Checkpoint>();
        public ECalloutStates CalloutStates;
        public Ped Suspect;
        public Vehicle SuspectCar;
        public Manusia Manusia;
        public Model CarModel;
        public List<Model> GangModels = new List<Model>();
        public Blip Blip;
        public bool CalloutRunning = false;
        public Vector3 Position = Vector3.Zero;
        public float SpawnHeading = 0f;
        public Spawnpoint Spawn = Spawnpoint.Zero;
        public long Timer;
        public DateTime Time;
        public TimeSpan TimeSpan = new(0, 0, 15);
        public System.Diagnostics.Stopwatch StopWatch;
        public LHandle Pursuit;
        public LHandle PullOver;
        public bool PursuitCreated = false;
        protected bool GrammarPoliceRunning => Initialization.IsLSPDFRPluginRunning("GrammarPolice", new Version(1, 4, 2, 2));
        protected bool StopThePedRunning => Initialization.IsLSPDFRPluginRunning("StopThePed", new Version(4, 9, 4, 4));
        protected bool UltimateBackupRunning => Initialization.IsLSPDFRPluginRunning("UltimateBackup", new Version(1, 8, 5, 4));
        protected bool CalloutInterfaceRunning => Initialization.IsLSPDFRPluginRunning("CalloutInterface", new Version(1, 2, 0, 0));
        public Persona SuspectPersona;
        public Ped PlayerPed => Game.LocalPlayer.Character;
        public GameFiber CalloutMainFiber;
        public StaticFinalizer Finalizer { get; private set; }
        public readonly System.Drawing.Color Yellow = RAGENativeUI.HudColorExtensions.GetColor(RAGENativeUI.HudColor.Yellow);
        public readonly System.Drawing.Color Red = RAGENativeUI.HudColorExtensions.GetColor(RAGENativeUI.HudColor.Red);
        public readonly System.Drawing.Color Blue = RAGENativeUI.HudColorExtensions.GetColor(RAGENativeUI.HudColor.Blue);
        public readonly System.Drawing.Color Green = RAGENativeUI.HudColorExtensions.GetColor(RAGENativeUI.HudColor.Green);
        public readonly System.Drawing.Color Orange = RAGENativeUI.HudColorExtensions.GetColor(RAGENativeUI.HudColor.Orange);
        public readonly System.Drawing.Color Purple = RAGENativeUI.HudColorExtensions.GetColor(RAGENativeUI.HudColor.Purple);
        public string FilePath;
        public static readonly uint[] WeaponHashes = { 0x1B06D571, 0xBFE256D4, 0x5EF9FEC4, 0x22D8FE39, 0x99AEEB3B, 0x2B5EF5EC, 0x78A97CD0, 0x1D073A89, 0x555AF99A, 0xBD248B55, 0x13532244, 0x624FE830 };
        protected bool Displayed = true;
        protected bool Accepted = true;
        #endregion
        private GameFiber EndHandlerFiber;

        public override void OnCalloutNotAccepted()
        {
            CalloutRunning = false;
            PursuitCreated = false;
            if (Suspect) Suspect.Delete();
            if (SuspectCar) SuspectCar.Delete();
            if (Blip) Blip.Delete();
            if (GrammarPoliceRunning) API.GrammarPoliceFunc.SetStatus(API.GrammarPoliceFunc.EGrammarPoliceStatusType.Available, true, false);
            CalloutBlips.ForEach(b => { if (b) b.Delete(); });
            CalloutEntities.ForEach(e =>
            {
                if (e)
                {
                    if (e is Ped ped && e.IsAlive && !Functions.IsPedArrested(ped)) e.Dismiss();
                    else if (e is Ped ped1 && e.IsAlive && Functions.IsPedArrested(ped1)) e.IsInvincible = false;
                    else if (e is Vehicle) e.Dismiss();
                    else if (e is Rage.Object) e.Delete();
                    else e.Dismiss();
                }
            });
            //CalloutMainFiber?.Abort();
            Manusia.CurrentManusia = null;
            Functions.PlayScannerAudio("BAR_AI_RESPOND");
            CalloutBlips.Clear();
            CalloutEntities.Clear();
            CalloutCheckpoints.Clear();
            GangModels.Clear();
            if (Finalizer != null) Finalizer.Dispose();
            base.OnCalloutNotAccepted();
        }
        public override bool OnBeforeCalloutDisplayed()
        {
            Finalizer = new StaticFinalizer(CleanUp);
            GangModels = Globals.GangPedModels.Values.GetRandomElement();
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            CalloutStates = ECalloutStates.EnRoute;
            CalloutRunning = true;
            if (EndHandlerFiber == null)
            {
                HandleEnd();
                EndHandlerFiber.Start();
            }
            else if (EndHandlerFiber.IsHibernating) EndHandlerFiber.Wake();
            StopWatch = new System.Diagnostics.Stopwatch();
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
            GenericUtils.Speaking = false;
            if (Displayed && Accepted)"~g~We Are CODE 4".DisplayNotifWithLogo(CalloutMessage);
            //StopWatch = null;
            if (Suspect && !Functions.IsPedArrested(Suspect)) Suspect.Dismiss();
            if (SuspectCar) SuspectCar.Dismiss();
            if (Blip) Blip.Delete();
            if (GrammarPoliceRunning && Menus.PauseMenu.autoAvailable.Checked) API.GrammarPoliceFunc.SetStatus(API.GrammarPoliceFunc.EGrammarPoliceStatusType.Available);
            if (Pursuit != null && Functions.IsPursuitStillRunning(Pursuit)) Functions.ForceEndPursuit(Pursuit);
            Manusia.CurrentManusia = null;
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
            CalloutCheckpoints.ForEach(c =>
            {
                if (c) c.Delete();
            });
            CalloutBlips.Clear();
            CalloutEntities.Clear();
            CalloutCheckpoints.Clear();
            if (CarModel.IsLoaded) CarModel.Dismiss();
            if (Finalizer != null) Finalizer.Dispose();
            //CalloutMainFiber?.Abort();
            base.End();
        }
        protected abstract void CleanUp();
        protected void HandleEnd()
        {
            EndHandlerFiber = new GameFiber(() =>
            {
                "Starting EndHandler Loop".ToLog();
                DateTime dateTime = DateTime.UtcNow;
                while (CalloutRunning)
                {
                    GameFiber.Yield();
                    if (GenericUtils.CheckKey(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.End))
                    {
                        GameFiber.Sleep(300);
                        if (Game.IsKeyDownRightNow(System.Windows.Forms.Keys.End))
                        {
                            GameFiber.Sleep(1850);
                            if (Game.IsKeyDownRightNow(System.Windows.Forms.Keys.End)) End();
                        }
                        else Game.DisplayHelp($"~y~To force end the callout, press and hold down {GenericUtils.FormatKeyBinding(System.Windows.Forms.Keys.None, System.Windows.Forms.Keys.End)}~y~ for 2 second");
                    }
                }
                $"Callout ended successfully, that callout took {(DateTime.UtcNow - dateTime).TotalSeconds:0.00} seconds".ToLog();
                GameFiber.Hibernate();
            }, "[BarbarianCall] Callout End Key Listener");
        }
        protected void DisplayGPNotif()
        {
            if (Initialization.IsLSPDFRPluginRunning("GrammarPolice"))
            {
                "If you have GrammarPolice installed, you can ask dispatch to display the suspect detail ~y~e.g. ~b~\"Dispatch requesting suspect details\"".DisplayNotifWithLogo("~y~"
                    + API.GrammarPoliceFunc.GetCallsign() + "~s~");
            }
            else "This Callout work better if ~y~GrammarPolice~s~ is installed, you can ask dispatch to display suspect details anytime with ~y~GrammarPolice".DisplayNotifWithLogo(GetType().Name);
        }
        protected void PlayScannerWithCallsign(string audio)
        {
            if (Initialization.IsLSPDFRPluginRunning("GrammarPolice", new Version(1, 4, 2, 2))) Functions.PlayScannerAudio("DISPATCH_TO " + API.GrammarPoliceFunc.GetCallsignAudio() + " " + audio);
            else Functions.PlayScannerAudio($"ATTENTION_ALL_UNITS {audio}");
        }
        protected void PlayScannerWithCallsign(string audio, Vector3 position)
        {
            if (Initialization.IsLSPDFRPluginRunning("GrammarPolice", new Version(1, 4, 2, 2)))
                Functions.PlayScannerAudioUsingPosition("DISPATCH_TO " + API.GrammarPoliceFunc.GetCallsignAudio() + " " + audio, position);
            else Functions.PlayScannerAudioUsingPosition($"ATTENTION_ALL_UNITS {audio}", position);
        }
        protected void SendCIMessage(string message)
        {
            if (CalloutInterfaceRunning)
            {
                API.CalloutInterfaceFunc.SendMessage(this, message);
            }
        }
        protected void DeclareVariable()
        {
            CalloutRunning = false;
            PursuitCreated = false;
            CalloutStates = ECalloutStates.UnAccepted;
            $"Callout Created From {CreationSource}".ToLog();
        }
        protected void PlayRadioAnimation(int duration) => Functions.PlayPlayerRadioAction(Functions.GetPlayerRadioAction(), duration);
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
