using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Rage;
using BarbarianCall.Extensions;

namespace BarbarianCall.Types
{
    public class Place : ISpatial, IEquatable<Place>
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
                 new XAttribute("Name", Name),
                 new XAttribute("ScannerAudio", PoliceScannerAudio),
                 Position.ToXmlElement("Position"),
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

        public float DistanceTo(Vector3 position) => Position.DistanceTo(position);

        public float DistanceTo(ISpatial spatialObject) => Position.DistanceTo(spatialObject.Position);

        public float DistanceTo2D(Vector3 position) => Position.DistanceTo2D(position);

        public float DistanceTo2D(ISpatial spatialObject) => Position.DistanceTo2D(spatialObject.Position);

        public float TravelDistanceTo(Vector3 position) => Position.TravelDistanceTo(position);

        public float TravelDistanceTo(ISpatial spatialObject) => Position.TravelDistanceTo(spatialObject.Position);
        public bool Equals(Place other)
        {
            return other is not null && Name == other.Name;
        }
    }
}
