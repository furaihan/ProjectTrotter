using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using Rage;
using LSPD_First_Response.Engine.Scripting.Entities;

namespace BarbarianCall.Callouts
{
    public abstract class CalloutBase : Callout
    {
        public enum ESuspectStates { InPursuit, Arrested, Dead, Escaped };
        public enum ECalloutStates { EnRoute, OnScene, InPursuit, Finish };
        public ECalloutStates CalloutStates;
        public Ped Suspect;
        public Vehicle SuspectCar;
        public Blip Blip;
        public bool CalloutRunning = false;
        public Vector3 SpawnPoint = Vector3.Zero;
        public float SpawnHeading  = float.NaN;
        public LHandle Pursuit; 
        public bool GrammarPoliceRunning  = false;
        public bool StopThePedRunning = false;
        public bool UltimateBackupRunning = false;
        public Persona SuspectPersona;
        public Ped PlayerPed = Game.LocalPlayer.Character;

        public override void OnCalloutNotAccepted()
        {
            CalloutRunning = false;
            if (Suspect.Exists()) Suspect.Delete();
            if (SuspectCar.Exists()) SuspectCar.Delete();
            if (Blip.Exists()) Blip.Delete();
            if (GrammarPoliceRunning) GrammarPolice.API.Functions.Available(false, false);
            Functions.PlayScannerAudio("BAR_AI_RESPOND");
            base.OnCalloutNotAccepted();
        }
        public override bool OnBeforeCalloutDisplayed()
        {
            GrammarPoliceRunning = Initialization.IsLSPDFRPluginRunning("GrammarPolice");
            UltimateBackupRunning = Initialization.IsLSPDFRPluginRunning("UltimateBackup");
            StopThePedRunning = Initialization.IsLSPDFRPluginRunning("StopThePed");
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {           
            return base.OnCalloutAccepted();
        }
        public override void End()
        {
            base.End();
            try
            {
                CalloutRunning = false;
                Peralatan.Speaking = false;
                if (Suspect.Exists()) Suspect.Dismiss();
                if (SuspectCar.Exists()) SuspectCar.Dismiss();
                if (Blip.Exists()) Blip.Delete();
                if (GrammarPoliceRunning) GrammarPolice.API.Functions.Available(false, false);
            }
            catch (Exception e)
            {
                e.ToString().ToLog();
            }
        }
    }
}
