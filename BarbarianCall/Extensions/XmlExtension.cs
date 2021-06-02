using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;
using BarbarianCall.Types;

namespace BarbarianCall.Extensions
{
    public static class XmlExtension
    {
        public static Vector3 ReadVector3(this XmlNode node)
        {
            float.TryParse(node.Attributes["X"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out float x);
            float.TryParse(node.Attributes["Y"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out float y);
            float.TryParse(node.Attributes["Z"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out float z);
            return new Vector3(x, y, z);
        }
        public static XElement ToXmlElement(this Vector3 vector3, string elementName)
            => new(elementName,
                new XAttribute("X", vector3.X.ToString(CultureInfo.InvariantCulture)),
                new XAttribute("Y", vector3.X.ToString(CultureInfo.InvariantCulture)),
                new XAttribute("Z", vector3.X.ToString(CultureInfo.InvariantCulture)));
        public static XElement ToXmlElement(this Spawnpoint sp,string elementName) => new(elementName,
                                                                                new XAttribute("X", sp.Position.X),
                                                                                new XAttribute("Y", sp.Position.Y),
                                                                                new XAttribute("Z", sp.Position.Z),
                                                                                new XAttribute("Heading", sp.Heading));
        public static Spawnpoint ReadSpawnPoint(this XmlNode node)
        {
            float.TryParse(node.Attributes["X"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out float x);
            float.TryParse(node.Attributes["Y"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out float y);
            float.TryParse(node.Attributes["Z"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out float z);
            float.TryParse(node.Attributes["Heading"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out float h);
            return new Spawnpoint(x, y, z, h);
        }
    }
}
