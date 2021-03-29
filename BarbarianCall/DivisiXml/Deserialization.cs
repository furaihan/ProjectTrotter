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
        internal static List<Spawnpoint> LoadPoliceStationLocations()
        {
            int nodeCount = 0;
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                if (File.Exists(@"lspdfr\data\stations.xml"))
                {
                    Peralatan.ToLog(string.Format("Attempting to read stations.xml file located in {0}", Path.GetFullPath(@"lspdfr\data\stations.xml")));
                    List<Spawnpoint> stations = new List<Spawnpoint>();
                    XmlDocument xml = new XmlDocument();
                    xml.Load(@"lspdfr\data\stations.xml");
                    foreach (XmlNode station in xml.SelectNodes("/PoliceStations/Station"))
                    {
                        string vectorString = station.ChildNodes[3].InnerText.Replace('f', ' ');
                        var v = vectorString.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(s => Convert.ToSingle(s)).ToList();
                        float heading = Convert.ToSingle(station.ChildNodes[4].InnerText);
                        Spawnpoint sp = new Spawnpoint(new Rage.Vector3(v[0], v[1], v[2]), heading);
                        Peralatan.ToLog($"Found a station location {sp} with name {station.ChildNodes[0].InnerText}");
                        stations.Add(sp);
                        nodeCount++;
                    }
                }
                else Peralatan.ToLog("File stations.xml doesn't exist, make sure you have installed LSPDFR correctly");
            }
            catch (Exception e)
            {
                Peralatan.ToLog(string.Format("Cannot read stations.xml file {0}. Num: {1}", e.Message, nodeCount));
                e.ToString().ToLog();
            }           
            return new List<Spawnpoint>();
        }
    }
}
