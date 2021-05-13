using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
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
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
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
    }
}
