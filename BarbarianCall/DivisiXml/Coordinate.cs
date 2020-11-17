using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbarianCall.DivisiXml
{
    [Serializable]
    public class Coordinate
    {
        public float AxisX { get; set; }
        public float AxisY { get; set; }
        public float AxisZ { get; set; }
        public float Heading { get; set; }
        public string Zone { get; set; }
        public string NearestStreet { get; set; }
    }
}
