using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using BarbarianCall.Extensions;
using BarbarianCall.Types;
using Rage;
using static Rage.Native.NativeFunction;

namespace BarbarianCall
{
    internal static class SpawnManager
    {
        private static bool IsNodeSafe(Vector3 v)
        {
            try
            {
                int nodeType = Natives.x22D7275A79FE8215<int>(v.X, v.Y, v.Z, 1, 0, 3f, 0f);
                if (Natives.x1EAF30FCFBF5AF74<bool>(nodeType)) /*IS_VEHICLE_NODE_ID_VALID*/
                {
                    if (!Natives.x4F5070AA58F69279<bool>(nodeType))/*GET_VEHICLE_NODE_IS_SWITCHED_OFF*/
                    {
                        if (Natives.xA2AE5C478B96E3B6<bool>(nodeType))/*GET_VEHICLE_NODE_IS_GPS_ALLOWED*/
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string.Format("IsNodeSafe Error: {0}", e.Message).ToLog();
                e.ToString().ToLog();
            }
            return false;
        }
        [Flags]
        private enum NodeFlags
        {
            None = 0,
            IsDisabled = 1,
            UnknownBit2 = 2,
            SlowNormalRoad = 4,
            MinorRoad = 8,
            TunnelOrUndergroundParking = 16,
            UnknownBit32 = 32,
            Freeway = 64,
            Junction = 128,
            StopNode = 256,
            SpecialStopNode = 512,
            Unk1024 = 1024,
            Unk2048 = 2048,
            Unk4096 = 4096,
            Unk8192 = 8192,
            Unk6542 = 6542,
            Restricted = 19,
        }
        private static void GetFlags(this Vector3 position)
        {
            var valid = Natives.GetVehicleNodeProperties<bool>(position, out uint density, out uint flag);
            if (valid)
            {
                NodeFlags nodeFlags = (NodeFlags)flag;
                string flagN = "";
                foreach (NodeFlags node in Enum.GetValues(typeof(NodeFlags)))
                {
                    if (nodeFlags.HasFlag(node)) flagN += node.ToString() + " ";
                }
                position.ToString().ToLog();
                Peralatan.ToLog($"Density: {density}. Flag: {flag}");
                Peralatan.ToLog(flagN);
            }
            else "Unsuccessfull Getting node flag value".ToLog();
        }
        public static bool IsOnScreen(this Vector3 pos) => Natives.xF9904D11F1ACBEC3<bool>(pos.X, pos.Y, pos.Z, out float _, out float _); //GET_HUD_SCREEN_POSITION_FROM_WORLD_POSITION
        private static bool IsSlowRoad(Vector3 position) => CallByHash<bool>(0b10_0010_1101_0111_0010_0111_0101_1010_0111_1001_1111_1110_1000_0010_0001_0101, position.X, position.Y, position.Z, 1, 5, 1077936128, 0f);
        internal static float GetRoadHeading(Vector3 pos)
        {
            Natives.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING<bool>(pos.X, pos.Y, pos.Z, out Vector3 _, out float heading, 12, 0x40400000, 0);
            return heading;
        }
        internal static Spawnpoint GetVehicleSpawnPoint(ISpatial spatial, float minimalDistance, float maximumDistance, bool considerDirection = false) => 
            GetVehicleSpawnPoint(spatial.Position, minimalDistance, maximumDistance, considerDirection);
        internal static Spawnpoint GetVehicleSpawnPoint(Vector3 pos, float minimalDistance, float maximumDistance, bool considerDirection = false)
        {
            pos.GetFlags();
            Stopwatch sw = Stopwatch.StartNew();
            var minimalDistanceSquared = (float)Math.Pow(minimalDistance, 2);
            var maximumDistanceSquared = (float)Math.Pow(maximumDistance, 2);
            for (int i = 1; i < 900; i++)
            {
                Vector3 v = pos.Around2D(Peralatan.Random.Next((int)minimalDistance, (int)maximumDistance));
                if (i % 35 == 0) GameFiber.Yield();
                if (Natives.xFF071FB798B803B0<bool>(v.X, v.Y, v.Z, out Vector3 nodeP, out float nodeH, 12, 3.0f, 0))
                {
                    if (Natives.GET_VEHICLE_NODE_PROPERTIES<bool>(nodeP.X, nodeP.Y, nodeP.Z, out int _, out int flag))
                    {
                        NodeFlags[] bl = { NodeFlags.Junction, NodeFlags.TunnelOrUndergroundParking, NodeFlags.StopNode, NodeFlags.SpecialStopNode, NodeFlags.MinorRoad };
                        NodeFlags nodeFlags = (NodeFlags)flag;
                        if (bl.Any(x => nodeFlags.HasFlag(x)))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                    if (nodeP.DistanceSquaredTo(pos) < minimalDistanceSquared || nodeP.DistanceSquaredTo(pos) > maximumDistanceSquared + 25f) continue;
                    if (nodeP.TravelDistanceTo(pos) < maximumDistance * 2f && IsNodeSafe(nodeP) && !IsOnScreen(nodeP))
                    {
                        if (!considerDirection || Game.LocalPlayer.Character.GetHeadingTowards(nodeP).HeadingDiff(Game.LocalPlayer.Character) < 90)
                        {
                            Spawnpoint ret = new(nodeP, nodeH);
                            $"Vehicle Spawn found {ret}. Distance: {ret.DistanceTo(pos):0.00}".ToLog();
                            $"{i} Process took {sw.ElapsedMilliseconds} ms".ToLog();                            
                            ret.Position.GetFlags();
                            return ret;
                        }                        
                    }
                }
            }
            "Vehicle spawn point is not found".ToLog();
            return Spawnpoint.Zero;
        }
        internal static Spawnpoint GetVehicleSpawnPoint2(Vector3 pos, float minimalDistance, float maximumDistance)
        {
            pos.GetFlags();
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 1; i < 600; i++)
            {
                if (i % 40 == 0) GameFiber.Yield();
                Vector3 v = pos.Around2D(Peralatan.Random.Next((int)Math.Abs(minimalDistance), (int)Math.Abs(maximumDistance)));
                if (Natives.x80CA6A8B6C094CC4<bool>(v.X, v.Y, v.Z, (i % 5) + 1, out Vector3 nodeP, out float nodeH, 0, 9, 3.0f, 2.5f))
                {
                    if (nodeP.DistanceTo(pos) > minimalDistance && nodeP.DistanceTo(pos) < maximumDistance && nodeP.TravelDistanceTo(pos) < maximumDistance * 2 && IsNodeSafe(nodeP) && !IsOnScreen(nodeP))
                    {
                        Spawnpoint ret = new(nodeP, nodeH);
                        $"Vehicle Spawn 2 found {ret}. Distance: {ret.DistanceTo(pos):0.00}".ToLog();
                        $"{i} Process took {sw.ElapsedMilliseconds} ms".ToLog();
                        ret.Position.GetFlags();
                        return ret;
                    }
                }
            }
            return Spawnpoint.Zero;
        }
        internal static Spawnpoint GetPedSpawnPoint(ISpatial spatial, float minimalDistance, float maximumDistance) => GetPedSpawnPoint(spatial.Position, minimalDistance, maximumDistance);
        internal static Spawnpoint GetPedSpawnPoint(Vector3 pos, float minimalDistance, float maximumDistance)
        {
            int nodeCount, flagCount, distanceCount, safeCount, propCount;
            nodeCount = flagCount = distanceCount = safeCount = propCount = 0;
            List<float> distanceList = new();
            pos.GetFlags();
            Stopwatch sw = Stopwatch.StartNew();
            var minimalDistanceSquared = (float)Math.Pow(minimalDistance, 2);
            var maximumDistanceSquared = (float)Math.Pow(maximumDistance, 2);
            for (int i = 1; i < 2000; i++)
            {
                Vector3 v = pos.Around2D(Peralatan.Random.Next((int)minimalDistance, (int)maximumDistance));
                if (i % 90 == 0) GameFiber.Yield();
                if (Natives.GET_NTH_CLOSEST_VEHICLE_NODE<bool>(v, i % 5, out Vector3 major, 1, 0f, 0f))
                {
                    if (Natives.GET_VEHICLE_NODE_PROPERTIES<bool>(major.X, major.Y, major.Z, out int _, out int flag))
                    {
                        NodeFlags[] bl = { NodeFlags.Freeway, NodeFlags.Junction, NodeFlags.TunnelOrUndergroundParking, NodeFlags.StopNode, NodeFlags.SpecialStopNode };
                        NodeFlags nodeFlags = (NodeFlags)flag;
                        if (bl.Any(x => nodeFlags.HasFlag(x)))
                        {
                            flagCount++;
                            continue;
                        }
                    }
                    else
                    {
                        propCount++;
                        continue;
                    }

                    if (Natives.xB61C8E878A4199CA<bool>(major.X, major.Y, major.Z, true, out Vector3 nodeP, (new[] { 17, 1, 16 }).GetRandomElement()))
                    {
                        if (nodeP.DistanceSquaredTo(pos) < minimalDistanceSquared || nodeP.DistanceSquaredTo(pos) > maximumDistanceSquared)
                        {
                            distanceCount++;
                            continue;
                        }

                        if (!IsOnScreen(nodeP))
                        {
                            Spawnpoint ret = new(nodeP, MathExtension.GetRandomFloatInRange(0, 360));
                            $"Ped sidewalk spawn found {ret} Distance: {ret.DistanceTo(pos)}".ToLog();
                            $"{i} Process took {sw.ElapsedMilliseconds} ms".ToLog();
                            ret.Position.GetFlags();
                            return ret;
                        }
                    }
                    else
                    {
                        safeCount++;
                        distanceList.Add(Vector3.DistanceSquared(major, Game.LocalPlayer.Character));
                    }
                }
                else nodeCount++;               
            }
            $"Safe coord not found".ToLog();
            $"2000 process took {sw.ElapsedMilliseconds} ms".ToLog();
            $"Node: {nodeCount}, Flag: {flagCount}, Distance: {distanceCount}, Safe: {safeCount}, Prop: {propCount}".ToLog();
            $"Distance: Max: {Math.Sqrt(distanceList.Max())}, Mix: {Math.Sqrt(distanceList.Min())}, Avg: {Math.Sqrt(distanceList.Average())}".ToLog();
            return Spawnpoint.Zero;
        }
        internal static Spawnpoint GetRoadSideSpawnPointFavored(Entity entity, float favoredDistance)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Vector3 fav = entity.GetOffsetPositionFront(favoredDistance - 5);
            for (int i = 1; i < 600; i++)
            {
                Vector3 pos = entity.GetOffsetPositionFront(Peralatan.Random.Next(1, 6)+ favoredDistance - 10);
                if (Natives.x45905BE8654AE067<bool>(pos.X, pos.Y, pos.Z, fav.X, fav.Y, fav.Z, i % 2 + 1, out Vector3 nodeP, out float nodeH, 0, 0x40400000, 0))
                //GET_NTH_CLOSEST_VEHICLE_NODE_FAVOUR_DIRECTION
                {
                    if (Natives.xA0F8A7517A273C05<bool>(nodeP.X, nodeP.Y, nodeP.Z, nodeH, out Vector3 roadSide))
                    {
                        if (roadSide.DistanceTo(entity) < 35 + favoredDistance && !roadSide.IsOccupied() && !IsOnScreen(roadSide))
                        {
                            Spawnpoint ret = new(roadSide, nodeH);
                            string.Format("Favored RoadSide Spawnpoint found {0}", ret).ToLog();
                            string.Format("{0} process took {1}", i, sw.ElapsedMilliseconds).ToLog();
                            pos.GetFlags();
                            ret.Position.GetFlags();
                            return ret;
                        }
                    }
                }
                if (i % 30 == 0) GameFiber.Yield();
            }
            string.Format("RoadSide favored Spawnpoint is not found after 600 process").ToLog();
            return Spawnpoint.Zero;
        }
        internal static Spawnpoint GetRoadSideSpawnPoint(Entity entity) => GetRoadSideSpawnPoint(entity.Position, entity.Heading);
        internal static Spawnpoint GetRoadSideSpawnPoint(ISpatial spatial, float? heading = null) => GetRoadSideSpawnPoint(spatial.Position, heading);
        internal static Spawnpoint GetRoadSideSpawnPoint(Vector3 pos, float? heading = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            if (heading.HasValue)
            {
                for (int i = 1; i < 600; i++)
                {
                    Vector3 v = pos.Around2D(Peralatan.Random.Next(1, 6));
                    if (Natives.xA0F8A7517A273C05<bool>(v.X, v.Y, v.Z, heading.Value, out Vector3 rsPos)) //_GET_ROAD_SIDE_POINT_WITH_HEADING
                    {
                        if (Natives.xFF071FB798B803B0<bool>(rsPos.X, rsPos.Y, rsPos.Z, out Vector3 _, out float nodeHeading, 5, 3.0f, 0))
                        {
                            if (rsPos.DistanceTo(pos) < 100f && !rsPos.IsOccupied())
                            {
                                Spawnpoint ret = new(rsPos, nodeHeading);
                                $"RoadSide with heading found {ret}".ToLog();
                                $"{i} process took {sw.ElapsedMilliseconds} ms".ToLog();
                                return ret;
                            }
                        }
                    }
                    if (i % 30 == 0) GameFiber.Yield();
                }
            }
            else
            {
                for (int i = 1; i < 600; i++)
                {
                    Vector3 v = pos.Around2D(Peralatan.Random.Next(5, 15), Peralatan.Random.Next(20, 35));
                    if (Natives.xFF071FB798B803B0<bool>(v.X, v.Y, v.Z, out Vector3 nodePos, out float nodeHeading, 5, 3.0f, 0))
                    {
                        if (Natives.xA0F8A7517A273C05<bool>(nodePos.X, nodePos.Y, nodePos.Z, nodeHeading, out Vector3 rsPos)) //_GET_ROAD_SIDE_POINT_WITH_HEADING
                        {
                            if (rsPos.DistanceTo(pos) < 100f && !rsPos.IsOccupied())
                            {
                                Spawnpoint ret = new(rsPos, nodeHeading);
                                $"RoadSide with heading found {ret}".ToLog();
                                $"{i} process took {sw.ElapsedMilliseconds} ms".ToLog();
                                return ret;
                            }
                        }
                    }
                    if (i % 35 == 0) GameFiber.Yield();
                }
            }
            for (int i = 1; i < 600; i++)
            {
                Vector3 v = pos.Around2D(Peralatan.Random.Next(25));
                if (Natives.x16F46FB18C8009E4<bool>(v.X, v.Y, v.Z, 0, out Vector3 roadSide)) //_GET_POINT_ON_ROAD_SIDE
                {
                    if (Natives.xFF071FB798B803B0<bool>(roadSide.X, roadSide.Y, roadSide.Z, out Vector3 nodePos, out float nodeHeading, 12, 0x40400000, 0))
                    {
                        if (nodePos.DistanceTo(roadSide) < 50f && roadSide.DistanceTo(pos) < 100f && !roadSide.IsOccupied())
                        {
                            Spawnpoint ret = new(roadSide, nodeHeading);
                            $"RoadSide found without heading {ret}".ToLog();
                            $"{i} process took {sw.ElapsedMilliseconds} ms".ToLog();
                            return ret;
                        }
                    }
                }
                if (i % 40 == 0) GameFiber.Yield();
            }
            $"RoadSide position not found".ToLog();
            $"1200 process took {sw.ElapsedMilliseconds} ms".ToLog();
            return Spawnpoint.Zero;
        }
        internal static Spawnpoint GetSlowRoadSpawnPoint(Vector3 pos, float minimumDistance, float maximumDistance)
        {
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < 0b1110000100; i++)
            {
                if (i % 40 == 0) GameFiber.Yield();
                Vector3 v = pos.Around2D(Peralatan.Random.Next((int)minimumDistance, (int)(maximumDistance + 1)));
                if (Natives.xE50E52416CCF948B<bool>(v.X, v.Y, v.Z, i % 5, out Vector3 randomNode,true, 0f, 0f))
                {
                    int nodeId = Natives.x22D7275A79FE8215<int>(randomNode.X, randomNode.Y, randomNode.Z, 1, 11077936128f, 0f);
                    if (Natives.x1EAF30FCFBF5AF74<bool>(nodeId))
                    {
                        if (Natives.x4F5070AA58F69279<bool>(nodeId))
                        {
                            if (randomNode.DistanceTo(pos) < maximumDistance && randomNode.DistanceTo(pos) > minimumDistance && !IsOnScreen(randomNode) && pos.HeightDiff(randomNode) < 18f)
                            {
                                if (Natives.GET_NTH_CLOSEST_VEHICLE_NODE_WITH_HEADING<bool>(randomNode.X, randomNode.Y, randomNode.Z, 1, out Vector3 _, out float heading, out int _, 1, 3f, 0f))
                                {
                                    Spawnpoint ret = new(randomNode, heading);
                                    $"Slow road found at {i + 1} tries, that process took {sw.ElapsedMilliseconds} ms".ToLog();
                                    pos.GetFlags();
                                    ret.Position.GetFlags();
                                    return ret;
                                }
                            }
                        }
                    }
                }
            }
            $"Slow road is not found".ToLog();
            return Spawnpoint.Zero;
        }
        internal static Spawnpoint GetSolicitationSpawnpoint(Vector3 playerPos, out Spawnpoint nodePosition, out Spawnpoint roadSidePosition)
        {
            int trys = 600;
            int shouldYieldAt = 40;
            try
            {
                ulong totalMem = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / 1048576;
                $"Total Memory: {totalMem} Mb".ToLog();
                if (totalMem > 16000)
                {
                    trys = 1200;
                    shouldYieldAt = 110;
                }
            }
            catch (System.ComponentModel.Win32Exception e) { (e.ToString() + " " +  e.NativeErrorCode.ToString()).ToLog(); }

            nodePosition = Spawnpoint.Zero;
            roadSidePosition = Spawnpoint.Zero;
            Stopwatch sw = Stopwatch.StartNew();
            List<NodeFlags> blacklist = new()
            {
                NodeFlags.Freeway,
                NodeFlags.Junction,
                NodeFlags.StopNode,
                NodeFlags.SpecialStopNode,
                NodeFlags.TunnelOrUndergroundParking,
                NodeFlags.MinorRoad,
            };
            for (int i = 0; i < trys; i++)
            {
                if (i % shouldYieldAt == 0) GameFiber.Yield();
                Vector3 around = playerPos.Around2D(Peralatan.Random.Next(350, 800));
                if (Natives.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING<bool>(around.X, around.Y, around.Z, out Vector3 nodePos, out float _, 0, 3,0,0))
                {
                    if (nodePos.DistanceSquaredTo(playerPos) > 640000f || nodePos.TravelDistanceTo(playerPos) > 1250) continue;
                    if (Natives.GET_VEHICLE_NODE_PROPERTIES<bool>(nodePos.X, nodePos.Y, nodePos.Z, out int heading, out int flag))
                    {
                        NodeFlags nodeFlags = (NodeFlags)flag;
                        if (blacklist.Any(flag=> nodeFlags.HasFlag(flag))) continue;
                        if (nodeFlags.HasFlag(NodeFlags.SlowNormalRoad) || nodeFlags.HasFlag(NodeFlags.UnknownBit2))
                        {
                            if (Natives.GET_SAFE_COORD_FOR_PED<bool>(nodePos.X, nodePos.Y, nodePos.Z, true, out Vector3 pedNodePos, 17))
                            {
                                if (pedNodePos.DistanceSquaredTo(nodePos) > 2500f || pedNodePos.DistanceTo(playerPos) < 90000f) continue;
                                bool success = Natives.x16F46FB18C8009E4<bool>(pedNodePos.X, pedNodePos.Y, pedNodePos.Z, -1, out Vector3 roadSidePos);
                                if (!success) continue;
                                //if (roadSidePos.DistanceTo(pedNodePos) < 8f) continue;
                                Spawnpoint ret = new(pedNodePos, pedNodePos.GetHeadingTowards(nodePos));
                                $"Get solicitation spawnpoint is successfull {ret}, {i + 1} process took {sw.ElapsedMilliseconds} ms".ToLog();
                                $"Solicitation Spawnpoint => Distance {ret.DistanceTo(playerPos)}, Travel Distance: {ret.TravelDistanceTo(playerPos)}".ToLog();
                                pedNodePos.GetFlags();
                                nodePos.GetFlags();
                                roadSidePosition = new(roadSidePos, heading);
                                nodePosition = new(nodePos, heading);
                                return ret;
                            }
                            else $"SafeCoordinate is not found Distance: {nodePos.DistanceTo(Game.LocalPlayer.Character)}".ToLog();
                        }
                    }
                }
            }
            $"Solicitation spawnpoint was not successfull, {sw.ElapsedMilliseconds} ms".ToLog();
            return Spawnpoint.Zero;
        }
    }
}