using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;

namespace BarbarianCall.SupportUnit
{
    public class MilitarySupport
    {
        public Vehicle Vehicle { get; private set; }
        public Ped Ped { get; private set; }
        public MilitarySupportType Type { get; private set; }
        public Blip Blip { get; private set; }
        public Entity TargetEntity { get; private set; }
    }
}
