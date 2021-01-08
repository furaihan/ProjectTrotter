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
        public static void GetDataFromXml(string filename, out List<Vector3> locations, out List<float> headings)
        {
            Peralatan.ToLog($"Reading XML file {filename}");
            locations = new List<Vector3>();
            headings = new List<float>();
            var outVar = Deserialize(filename);
            int count = 0;
            foreach (Coordinate c in outVar)
            {
                count++;
                Vector3 location = new Vector3(c.AxisX, c.AxisY, c.AxisZ);
                float heading = c.Heading;                
                locations.Add(location);
                headings.Add(heading);
                if (count % 45 == 0) GameFiber.Yield();
            }
        }
        public static List<SpawnPoint> GetSpawnPointFromXml(string filename)
        {
            GetDataFromXml(filename, out var vector3s, out var hs);
            List<SpawnPoint> spawnPoints = vector3s.Select(s => new SpawnPoint(s, hs[vector3s.IndexOf(s)])).ToList();
            return spawnPoints;
        }
    }
}
