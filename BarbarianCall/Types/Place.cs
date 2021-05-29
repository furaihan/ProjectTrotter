using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Rage;

namespace BarbarianCall.Types
{
    public class Place : ISpatial, IFormattable
    {
        public string Name { get; set; }
        public string PoliceScannerAudio { get; set; } = string.Empty;
        public Vector3 Position { get; set; }
        public List<Spawnpoint> VehicleEntrance { get; set; } = new List<Spawnpoint>();
        public List<Spawnpoint> PedEntrance { get; set; } = new List<Spawnpoint>();
        public List<Spawnpoint> VehicleExits { get; set; } = new List<Spawnpoint>();
        public List<Spawnpoint> PedExits { get; set; } = new List<Spawnpoint>();
        public Place(string name, Vector3 position)
        {
            Name = name;
            Position = position;
        }
        public XElement ToXmlElement()
            => new("Place",
                 new XElement("Name", Name),
                 new XElement("ScannerAudio", PoliceScannerAudio),
                 new XElement("Position",
                     new XAttribute("X", Position.X),
                     new XAttribute("Y", Position.Y),
                     new XAttribute("Z", Position.Z)),
                 new XElement("VehicleEntrance",
                     from sp in VehicleEntrance
                     select sp.ToXmlElement("Point")),
                 new XElement("PedEntrance",
                     from sp in PedEntrance
                     select sp.ToXmlElement("Point")),
                 new XElement("VehicleExits",
                     from sp in VehicleExits
                     select sp.ToXmlElement("Point")),
                  new XElement("PedExits",
                     from sp in PedExits
                     select sp.ToXmlElement("Point")));
        public static Place ReadFromXml(XmlNode node)
        {

            return null;
        }

        public float DistanceTo(Vector3 position)
        {
            throw new NotImplementedException();
        }

        public float DistanceTo(ISpatial spatialObject)
        {
            throw new NotImplementedException();
        }

        public float DistanceTo2D(Vector3 position)
        {
            throw new NotImplementedException();
        }

        public float DistanceTo2D(ISpatial spatialObject)
        {
            throw new NotImplementedException();
        }

        public float TravelDistanceTo(Vector3 position)
        {
            throw new NotImplementedException();
        }

        public float TravelDistanceTo(ISpatial spatialObject)
        {
            throw new NotImplementedException();
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }
    }
}
