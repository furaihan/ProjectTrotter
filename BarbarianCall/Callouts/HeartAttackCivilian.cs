using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Rage;
using LSPDFR = LSPD_First_Response.Mod.API.Functions;
using LSPD_First_Response.Engine.Scripting.Entities;
using BarbarianCall.Extensions;
using BarbarianCall.Types;

namespace BarbarianCall.Callouts
{
    public class HeartAttackCivilian : CalloutBase
    {
        public Ped Civilian;
        public override bool OnBeforeCalloutDisplayed()
        {
            Spawn = SpawnManager.GetPedSpawnPoint(PlayerPed, 350, 950);
            if (Spawn == Spawnpoint.Zero)
            {
                Displayed = false;
                return false;
            }
            var nameRnd = PersonaHelper.GetRandomName(LSPD_First_Response.Gender.Male);
            var model = Extension.GetRandomMaleModel();
            Persona clone = Persona.FromExistingPed(World.GetAllPeds().Where(x => x && x.Model.Hash == model.Hash).GetRandomElement());
            clone.Forename = nameRnd.Item1;
            clone.Surname = nameRnd.Item2;
            Civilian = new Ped(model, Spawn, Spawn);
            CalloutEntities.Add(Civilian);
            Civilian.MakeMissionPed();
            LSPDFR.SetPersonaForPed(Suspect, clone);
            CalloutPosition = Spawn;
            CalloutMessage = "Heart Attack Civilian";
            CalloutAdvisory = $"The Civilian name is {nameRnd.Item1} {nameRnd.Item2}";
            ShowCalloutAreaBlipBeforeAccepting(CalloutPosition, 30f);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            Blip = new Blip(Spawn.Position.Around2D(20f).ToGround(), 25)
            {
                Color = Yellow,
                IsRouteEnabled = true
            };
            Civilian.Tasks.PlayAnimation("random@drunk_driver_1", "drunk_fall_over", 4.0f, AnimationFlags.StayInEndFrame | AnimationFlags.Loop | AnimationFlags.RagdollOnCollision);
            Civilian.IsInvincible = true;
            MainSituation();
            CalloutMainFiber.Start();
            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            base.Process();
        }
        public override void End()
        {
            base.End();
        }
        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }
        private void GetClose()
        {
            while(CalloutRunning)
            {
                GameFiber.Yield();
                var traceStart = PlayerPed.IsInAnyVehicle(false) ? PlayerPed.CurrentVehicle.FrontPosition : PlayerPed.FrontPosition;
                var raycast = World.TraceLine(traceStart, Civilian.Position, (TraceFlags)(1 | 2 | 4 | 8 | 16 | 256));
                if (Civilian && raycast.Hit && raycast.HitEntity && raycast.HitEntity == Civilian)
                {
                    "Raycast hit civilian".ToLog();
                    break;
                }
                else if (Civilian && Civilian.Position.IsOnScreen()) break;
                else if (Civilian && PlayerPed.DistanceTo(Civilian) < 30f) break;
            }
            if (Blip) Blip.Delete();
            Blip = Civilian.AttachBlip();
            Blip.Color = Color.Orange;
            Blip.SetBlipName("Heart Attack Civilian");
        }
        private void MainSituation()
        {
            CalloutMainFiber = new GameFiber(() =>
            {
                try
                {
                    GetClose();
                    if (!CalloutRunning) return;
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        if (Game.IsKeyDownRightNow(System.Windows.Forms.Keys.End))
                            break;
                    }
                    End();
                }
                catch (Exception e)
                {
                    e.ToString().ToLog();
                    NetExtension.SendError(e);
                    End(e.Message);
                }
            });
        }
    }
}
