using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using System.IO;
using System.Xml.Serialization;
using BarbarianCall.Types;

namespace BarbarianCall.DivisiXml
{
    class Deserialization
    {
        public static List<Coordinate> Deserialize(string filename)
        {
            XmlSerializer xml = new XmlSerializer(typeof(List<Coordinate>));
            TextReader reader = new StreamReader(filename);
            object obj = xml.Deserialize(reader);
            List<Coordinate> XmlData = (List<Coordinate>)obj;
            reader.Close();
            return XmlData;
        }
        public static List<Spawnpoint> GetSpawnPointFromXml(string filename)
        {
            Peralatan.ToLog("Reading XML file " + filename);
            List<Coordinate> coordinates = Deserialize(filename);
            List<Spawnpoint> ret = coordinates.Select(c => new Spawnpoint(c.AxisX, c.AxisY, c.AxisZ, c.Heading)).ToList();
            return ret;
        }
    }
}
