using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using BarbarianCall.Extensions;

namespace BarbarianCall.Callouts
{
    class MassStreetFighting : CalloutBase
    {
        private List<Entity> Participant;
        private Ped Boss1;
        private Ped Boss2;
        private bool CanEnd = false;
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutRunning = false;
            PursuitCreated = false;
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            if (CalloutRunning)
            {
                if (!Participant.All(e=> CalloutEntities.Contains(e)))
                {
                    Participant.ForEach(e =>
                    {
                        if (e)
                        {
                            if (!CalloutEntities.Contains(e)) CalloutEntities.Add(e);
                        }
                    });
                }              
            }
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
    }
}
