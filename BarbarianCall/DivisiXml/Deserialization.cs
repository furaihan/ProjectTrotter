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
            XmlSerializer xml = new(typeof(List<Coordinate>));
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
                List<Spawnpoint> ret = new();
                XmlDocument xmlDocument = new();
                xmlDocument.Load(filename);
                foreach (XmlNode coordinate in xmlDocument.DocumentElement.SelectNodes("/BarbarianCall/Coordinate"))
                {
                    Spawnpoint spawnpoint = new(
                        coordinate.Attributes["X"] == null ? Spawnpoint.Zero.Position.X : Convert.ToSingle(coordinate.Attributes["X"].Value),
                        coordinate.Attributes["Y"] == null ? Spawnpoint.Zero.Position.Y : Convert.ToSingle(coordinate.Attributes["Y"].Value),
                        coordinate.Attributes["Z"] == null ? Spawnpoint.Zero.Position.Z : Convert.ToSingle(coordinate.Attributes["Z"].Value),
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
                var stationXmls = Directory.EnumerateFiles(@"lspdfr\data\custom\").Select(Path.GetFullPath).Where(s => s.ToLower().Contains("stations")).ToList();
                stationXmls.Add(@"lspdfr\data\stations.xml");
                stationXmls.ForEach(Peralatan.ToLog);
                if (File.Exists(@"lspdfr\data\stations.xml"))
                {
                    Peralatan.ToLog(string.Format("Attempting to read stations.xml file located in {0}", Path.GetFullPath(@"lspdfr\data\stations.xml")));
                    List<Spawnpoint> stations = new();
                    XmlDocument xml = new();
                    xml.Load(@"lspdfr\data\stations.xml");
                    foreach (XmlNode station in xml.SelectNodes("/PoliceStations/Station"))
                    {
                        string vectorString = station.ChildNodes[3].InnerText.Replace("f", string.Empty);
                        var v = vectorString.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(s => Convert.ToSingle(s)).ToList();
                        float heading = Convert.ToSingle(station.ChildNodes[4].InnerText.Replace("f", string.Empty));
                        Spawnpoint sp = new(v[0], v[1], v[2], heading);
                        Peralatan.ToLog($"Found a station location {sp} with name {station.ChildNodes[0].InnerText}");
                        stations.Add(sp);
                        nodeCount++;
                    }
                    $"Reading stations.xml is took {sw.ElapsedMilliseconds} ms".ToLog();
                    return stations;
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
