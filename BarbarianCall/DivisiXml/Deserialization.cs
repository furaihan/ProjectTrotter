using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
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
        public static List<Spawnpoint> GetSpawnPointFromXml2(string filename)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Peralatan.ToLog("Reading XML file " + filename);
            List<Coordinate> coordinates = Deserialize(filename);
            List<Spawnpoint> ret = coordinates.Select(c => new Spawnpoint(c.AxisX, c.AxisY, c.AxisZ, c.Heading)).ToList();
            Peralatan.ToLog(string.Format("Reading xml file is took {0} ms", sw.ElapsedMilliseconds));
            return ret;
        }
        public static List<Spawnpoint> GetSpawnPointFromXml(string filename)
        {
            try
            {
                Peralatan.ToLog(string.Format("Reading XML File {0}", Path.GetFullPath(filename)));
                Stopwatch sw = Stopwatch.StartNew();
                List<Spawnpoint> ret = new List<Spawnpoint>();
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(filename);
                foreach (XmlNode coordinate in xmlDocument.DocumentElement.SelectNodes("/BarbarianCall/Coordinate"))
                {
                    Spawnpoint spawnpoint = new Spawnpoint(
                        coordinate.Attributes["X"] != null ? Convert.ToSingle(coordinate.Attributes["X"].Value) : Spawnpoint.Zero.Position.X,
                        coordinate.Attributes["Y"] != null ? Convert.ToSingle(coordinate.Attributes["Y"].Value) : Spawnpoint.Zero.Position.Y,
                        coordinate.Attributes["Z"] != null ? Convert.ToSingle(coordinate.Attributes["Z"].Value) : Spawnpoint.Zero.Position.Z,
                        Convert.ToSingle(coordinate.ChildNodes[0].InnerText));
                    ret.Add(spawnpoint);
                }
                Peralatan.ToLog(string.Format("Found total {0} locations from {1}", ret.Count, Path.GetFileName(filename)));
                Peralatan.ToLog(string.Format("Reading this XML file took {0} ms", sw.ElapsedMilliseconds));
                return ret;
            }
            catch (Exception e)
            {
                Peralatan.ToLog(string.Format("Get spawn point data from XML error: {0}", e.Message));
                e.ToString().ToLog();
            }
            return new List<Spawnpoint>();
        }
    }
}
