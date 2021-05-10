using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using BarbarianCall.Types;
using BarbarianCall.Extensions;

namespace BarbarianCall.Callouts
{
    public class Solicitation : CalloutBase
    {
        private Ped Hooker;
        private Ped Suspect;
        private Vehicle SuspectVehicle;
        private Spawnpoint RoadSide;
        private List<Model> hookerModels = new() { 0x73DEA88B, 0x028ABF95, 0x14C3E407, 0x031640AC, 0x2F4AEC3E, 0xB920CC2B, 0x84A1B11A, 0x9CF26183 };
        private List<string> scenarios = new() { "WORLD_HUMAN_PROSTITUTE_HIGH_CLASS", "WORLD_HUMAN_PROSTITUTE_LOW_CLASS" };
        private Spawnpoint SolicitationSpawn;

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutRunning = false;
            PursuitCreated = false;
            DeclareVariable();
            SolicitationSpawn = SpawnManager.GetSolicitationSpawnpoint(PlayerPed.Position, out Spawnpoint _, out RoadSide);
            CalloutPosition = SolicitationSpawn;
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
            Hooker = new Ped(selected, SolicitationSpawn, SolicitationSpawn);
            Hooker.PlayScenarioAction(scenarios.GetRandomElement(), false);
            return base.OnCalloutAccepted();
        }
        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }
        public override void Process()
        {
            base.Process();
        }
        public override void End()
        {
            base.End();
        }
        private void GetClose()
        {
            while (CalloutRunning)
            {
                GameFiber.Yield();
                var raycast = World.TraceLine(PlayerPed.Position, Hooker.Position, TraceFlags.IntersectPedsSimpleCollision, PlayerPed, PlayerPed.CurrentVehicle);
                if (raycast.Hit)
                {
                    "Raycast hit".ToLog();
                    if (raycast.HitEntity && raycast.HitEntity == Hooker) break;
                }
                if (PlayerPed.DistanceTo(CalloutPosition) < 100) break;
            }
        }
        private void CalloutMainLogic()
        {
            CalloutMainFiber = new GameFiber(() =>
            {
                GetClose();
            });
        }
    }
}
