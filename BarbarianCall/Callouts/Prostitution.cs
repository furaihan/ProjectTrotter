using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using BarbarianCall.Types;
using BarbarianCall.Extensions;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace BarbarianCall.Callouts
{
    [CalloutInfo("Prostitution", CalloutProbability.High)]
    public class Prostitution : CalloutBase
    {
        private Ped Hooker;
        private Ped Suspect;
        private Vehicle SuspectVehicle;
        private Spawnpoint RoadSide;
        private readonly List<Model> hookerModels = new() { 0x73DEA88B, 0x028ABF95, 0x14C3E407, 0x031640AC, 0x2F4AEC3E, 0xB920CC2B, 0x84A1B11A, 0x9CF26183 };
        private readonly List<string> scenarios = new() { "WORLD_HUMAN_PROSTITUTE_HIGH_CLASS", "WORLD_HUMAN_PROSTITUTE_LOW_CLASS" };
        private Spawnpoint SolicitationSpawn;
        private BarTimerBar AwarenessBar;
        private readonly TimerBarPool pool = new();
        public bool CanEnd = false;
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutRunning = false;
            PursuitCreated = false;
            DeclareVariable();
            SolicitationSpawn = SpawnManager.GetSolicitationSpawnpoint(PlayerPed.Position, out Spawnpoint _, out RoadSide);
            CalloutPosition = SolicitationSpawn;
            CalloutMessage = "Prostitution";
            FriendlyName = "Solicitation BO";
            CalloutAdvisory = "Be careful, your siren may draw an attention";
            if (CalloutPosition == Spawnpoint.Zero)
            {
                Peralatan.ToLog("Solicitation | No suitable location found after 600 tries");
                return false;
            }
            ShowCalloutAreaBlipBeforeAccepting(CalloutPosition, 120);
            PlayScannerWithCallsign("WE_HAVE CRIME_SOLICITATION IN_OR_ON_POSITION", CalloutPosition);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            Model selected = hookerModels.GetRandomElement(true);
            selected.Load();
            Blip = new Blip(CalloutPosition.Around2D(50f).ToGround(), 80f)
            {
                Color = Color.FromArgb(100, HudColor.Franklin.GetColor()),
                IsRouteEnabled = true,
                RouteColor = Color.FromArgb(180, HudColor.Yellow.GetColor()),
            };
            CalloutBlips.Add(Blip);
            Hooker = new Ped(selected, SolicitationSpawn, SolicitationSpawn);
            Hooker.MakeMissionPed();
            CalloutEntities.Add(Hooker);
            Hooker.PlayScenarioAction(scenarios.GetRandomElement(), false);
            ClearUnrelatedEntities();
            CalloutMainLogic();
            CalloutMainFiber.Start();
            return base.OnCalloutAccepted();
        }
        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }
        public override void Process()
        {
            if (CalloutRunning)
            {

            }
            base.Process();
        }
        public override void End()
        {
            base.End();
        }
        private void DisplayHookerAwarenessBar()
        {
            GameFiber.StartNew(() =>
            {
                AwarenessBar = new BarTimerBar("Hooker Awareness")
                {
                    ForegroundColor = HudColor.Pink.GetColor()
                };
                AwarenessBar.BackgroundColor = Color.FromArgb(120, AwarenessBar.ForegroundColor);
                pool.Add(AwarenessBar);
                while (CalloutRunning)
                {
                    GameFiber.Yield();
                    float percentage = (PlayerPed.DistanceTo(Hooker) - 15) / 40;                   
                    HitResult hitResult = World.TraceLine(Hooker.FrontPosition, PlayerPed.Position, TraceFlags.IntersectEverything);
                    if (hitResult.HitEntity && hitResult.HitEntity == Game.LocalPlayer.Character && AwarenessBar.Percentage < 0.75f) percentage += 0.007125f;
                    else if (PlayerPed.IsInCover) percentage -= 0.006f;
                    if (PlayerPed.IsSprinting) percentage += 0.0038545f;
                    else if (PlayerPed.IsRunning) percentage += 0.001225f;
                    AwarenessBar.Percentage = percentage;
                }
            });
            GameFiber.StartNew(() =>
            {
                while (CalloutRunning)
                {
                    GameFiber.Yield();
                    pool.Draw();
                }
            });
        }
        private void GetClose()
        {
            while (CalloutRunning)
            {
                GameFiber.Yield();
                var raycast = World.TraceLine(PlayerPed.IsInAnyVehicle(false) ? PlayerPed.CurrentVehicle.FrontPosition : PlayerPed.FrontPosition, Hooker.Position, TraceFlags.IntersectEverything);
                if (raycast.Hit)
                {
                    if (raycast.HitEntity && raycast.HitEntity == Hooker)
                    {
                        "Raycast Hit Hooker".ToLog();
                        break;
                    }
                }
                if (PlayerPed.DistanceTo(Blip.Position) < 80f || PlayerPed.DistanceTo(Hooker) < 25f) break;
            }
            if (Blip) Blip.Delete();
        }
        private void CalloutMainLogic()
        {
            CalloutMainFiber = new GameFiber(() =>
            {
                try
                {
                    GetClose();
                    if (!CalloutRunning) return;
                    if (Blip) Blip.Delete();
                    World.GetAllEntities().Where(e => e && !CalloutEntities.Contains(e) && !e.CreatedByTheCallingPlugin && e.GetAttachedBlips().Length == 0 &&
                    (e.IsEntityAPed() || e.IsEntityAVehicle()) && !e.Position.IsOnScreen() && e.DistanceTo(CalloutPosition) <= 50f).ToList().ForEach(x => { if (x) x.Delete(); });
                    Blip = new Blip(Hooker.Position, 30f)
                    {
                        Color = Color.FromArgb(120, HudColor.PinkLight.GetColor()),
                    };
                    CalloutBlips.Add(Blip);
                    DisplayHookerAwarenessBar();
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        if (AwarenessBar?.Percentage >= 1 && Game.IsKeyDownRightNow(System.Windows.Forms.Keys.End))
                        {
                            End("Konangan su");
                        }
                    }
                }
                catch (Exception e)
                {
                    e.ToString().ToLog();
                    NetExtension.SendError(e);
                }              
            });
        }
    }
}
