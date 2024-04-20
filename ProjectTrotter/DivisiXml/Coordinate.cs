using System;
using System.Xml.Serialization;

namespace ProjectTrotter.DivisiXml
{
    [Serializable]
    public class Coordinate
    {
        [XmlAttribute(AttributeName = "X")]
        public float AxisX { get; set; }
        [XmlAttribute(AttributeName = "Y")]
        public float AxisY { get; set; }
        [XmlAttribute(AttributeName = "Z")]
        public float AxisZ { get; set; }
        public float Heading { get; set; }
        public string Zone { get; set; }
        public string NearestStreet { get; set; }
    }
}
