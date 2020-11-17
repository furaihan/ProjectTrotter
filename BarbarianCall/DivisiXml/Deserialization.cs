using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using System.IO;
using System.Xml.Serialization;

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
        public static void GetDataFromXml(string filename, out List<Vector3> locations, out List<float> headings)
        {
            Peralatan.ToLog($"Reading XML file {filename}");
            locations = new List<Vector3>();
            headings = new List<float>();
            var outVar = Deserialize(filename);
            foreach (Coordinate c in outVar)
            {
                Vector3 location = new Vector3(c.AxisX, c.AxisY, c.AxisZ);
                float heading = c.Heading;                
                locations.Add(location);
                headings.Add(heading);
            }
        }
    }
}
