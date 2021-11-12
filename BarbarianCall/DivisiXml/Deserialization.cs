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
                Peralatan.ToLog(string.Format("Deserialized {0} locations from {1}", ret.Count, Path.GetFileName(filename)));
                Peralatan.ToLog(string.Format("Reading this XML file took {0} ms", sw.ElapsedMilliseconds));
                return ret;
            }
            catch (Exception e)
            {
                Peralatan.ToLog(string.Format("Failed to deserialize {0} : {1}", Path.GetFullPath(filename), e.Message));
                e.ToString().ToLog();
            }
            return new List<Spawnpoint>();
        }
        private const string PedDecalXml = @"Plugins\LSPDFR\BarbarianCall\PedDecalBadgeTorso.xml";
        /// <summary>
        /// Valid <paramref name="gender"/> are (male, female, and any)
        /// </summary>
        internal static List<Tuple<string, string>> GetBadgeFromXml(string gender = "any")
        {
            try
            {
                Peralatan.ToLog(string.Format("Reading XML File {0}", Path.GetFullPath(PedDecalXml)));
                Stopwatch sw = Stopwatch.StartNew();
                List<Tuple<string, string>> ret = new();
                XmlDocument xmlDocument = new();
                xmlDocument.Load(PedDecalXml);
                foreach (XmlNode node in xmlDocument.DocumentElement.SelectNodes("/BarbarianCall/DecalCollection"))
                {
                    string collectionName = node.Attributes["name"].Value;
                    foreach (XmlNode node2 in node.ChildNodes)
                    {
                        if (node2.Attributes["gender"].Value == gender.ToLower())
                        {
                            string decalName = node2.InnerText;
                            ret.Add(new Tuple<string, string>(collectionName, decalName));
                        }
                    }
                }
                Peralatan.ToLog(string.Format("Deserialized {0} decal from {1}", ret.Count, Path.GetFileName(PedDecalXml)));
                Peralatan.ToLog(string.Format("Reading this XML file took {0} ms", sw.ElapsedMilliseconds));
                return ret;
            }
            catch (Exception e)
            {
                e.ToString().ToLog();
            }
            return new List<Tuple<string, string>>();
        }

    }
}
