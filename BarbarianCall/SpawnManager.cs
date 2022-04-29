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
        internal static void GetFlags(this Vector3 position)
        {
            var valid = Natives.GetVehicleNodeProperties<bool>(position, out uint density, out uint flag);
            if (valid)
            {
                NodeFlags nodeFlags = (NodeFlags)flag;
                string flagN = "";
                foreach (NodeFlags node in Enum.GetValues(typeof(NodeFlags)))
                {
                    if (nodeFlags.HasFlag(node))
                    {
                        flagN += node.ToString() + " ";
                    }
                }
                position.ToString().ToLog();
                Peralatan.ToLog($"Density: {density}. Flag: {flag}");
                Peralatan.ToLog(flagN);
            }
            else
            {
                "Unsuccessfull Getting node flag value".ToLog();
            }
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
            List<int> flags = new();
            int distanceCount, nodeCount, propCount, flagCount, screenNode, dirCount;
            distanceCount = nodeCount = propCount = flagCount = screenNode = dirCount = 0;
            NodeFlags[] bl = { NodeFlags.Junction, NodeFlags.TunnelOrUndergroundParking, NodeFlags.StopNode, NodeFlags.SpecialStopNode, NodeFlags.MinorRoad };
            for (int i = 1; i <= 2000; i++)
            {
                Vector3 v = pos.AroundPosition(Peralatan.Random.Next((int)minimalDistance, (int)maximumDistance));
                if (i % 80 == 0)
                {
                    GameFiber.Yield();
                }

                if (Natives.xFF071FB798B803B0<bool>(v.X, v.Y, v.Z, out Vector3 nodeP, out float nodeH, 12, 3.0f, 0))
                {
                    if (Natives.GET_VEHICLE_NODE_PROPERTIES<bool>(nodeP.X, nodeP.Y, nodeP.Z, out int _, out int flag))
                    {
                        NodeFlags nodeFlags = (NodeFlags)flag;
                        flags.Add(flag);
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
                    if (nodeP.DistanceToSquared(pos) < minimalDistanceSquared || nodeP.DistanceToSquared(pos) > maximumDistanceSquared + 25f)
                    {
                        distanceCount++;
                        continue;
                    }

                    if (IsNodeSafe(nodeP))
                    {
                        if (!considerDirection || Game.LocalPlayer.Character.GetHeadingTowards(nodeP).HeadingDiff(Game.LocalPlayer.Character) < 90)
                        {
                            Spawnpoint ret = new(nodeP, nodeH);
                            $"Vehicle Spawn found {ret}. Distance: {ret.Position.DistanceTo(pos):0.00}".ToLog();
                            $"{i} Process took {sw.ElapsedMilliseconds} ms".ToLog();
                            ret.Position.GetFlags();
                            flags.Clear();
                            return ret;
                        }
                        else
                        {
                            dirCount++;
                            continue;
                        }
                    }
                    else
                    {
                        screenNode++;
                        continue;
                    }
                }
                else
                {
                    nodeCount++;
                    continue;
                }
            }
            var groups = flags.GroupBy(v => v).ToList();
            groups.ForEach(g => Peralatan.ToLog($"({(NodeFlags)g.Key}) has {g.Count()} items"));
            $"Distance: {distanceCount}, Node: {nodeCount} Flag: {flagCount}, Prop: {propCount}, SafeNode: {screenNode}, Direction: {dirCount}".ToLog();
            "Vehicle spawn point is not found".ToLog();
            flags.Clear();
            groups.Clear();
            return Spawnpoint.Zero;
        }
        internal static Spawnpoint GetVehicleSpawnPoint2(Vector3 pos, float minimalDistance, float maximumDistance, bool considerDirection = false)
        {
            pos.GetFlags();
            Stopwatch sw = Stopwatch.StartNew();
            var minimalDistanceSquared = (float)Math.Pow(minimalDistance, 2);
            var maximumDistanceSquared = (float)Math.Pow(maximumDistance, 2);
            List<int> flags = new();
            int distanceCount, nodeCount, propCount, flagCount, screenNode, dirCount;
            distanceCount = nodeCount = propCount = flagCount = screenNode = dirCount = 0;
            NodeFlags[] bl = { NodeFlags.Junction, NodeFlags.TunnelOrUndergroundParking, NodeFlags.StopNode, NodeFlags.SpecialStopNode, NodeFlags.MinorRoad };
            for (int i = 1; i <= 2000; i++)
            {
                Vector3 v = pos.AroundPosition(Peralatan.Random.Next((int)minimalDistance, (int)maximumDistance));
                if (i % 80 == 0)
                {
                    GameFiber.Yield();
                }

                if (Natives.GET_NTH_CLOSEST_VEHICLE_NODE_WITH_HEADING<bool>(v.X, v.Y, v.Z,i % 5 + 1, out Vector3 nodeP, out float nodeH, out uint _, 9, 3.0f, 2.5f))
                {
                    if (Natives.GET_VEHICLE_NODE_PROPERTIES<bool>(nodeP.X, nodeP.Y, nodeP.Z, out int _, out int flag))
                    {
                        NodeFlags nodeFlags = (NodeFlags)flag;
                        flags.Add(flag);
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
                    if (nodeP.DistanceToSquared(pos) < minimalDistanceSquared || nodeP.DistanceToSquared(pos) > maximumDistanceSquared + 25f)
                    {
                        distanceCount++;
                        continue;
                    }

                    if (IsNodeSafe(nodeP))
                    {
                        if (!considerDirection || Game.LocalPlayer.Character.GetHeadingTowards(nodeP).HeadingDiff(Game.LocalPlayer.Character) < 90)
                        {
                            Spawnpoint ret = new(nodeP, nodeH);
                            $"Vehicle Spawn found {ret}. Distance: {ret.Position.DistanceTo(pos):0.00}".ToLog();
                            $"{i} Process took {sw.ElapsedMilliseconds} ms".ToLog();
                            ret.Position.GetFlags();
                            flags.Clear();
                            return ret;
                        }
                        else
                        {
                            dirCount++;
                            continue;
                        }
                    }
                    else
                    {
                        screenNode++;
                        continue;
                    }
                }
                else
                {
                    nodeCount++;
                    continue;
                }
            }
            var groups = flags.GroupBy(v => v).ToList();
            groups.ForEach(g => Peralatan.ToLog($"({(NodeFlags)g.Key}) has {g.Count()} items"));
            $"Distance: {distanceCount}, Node: {nodeCount} Flag: {flagCount}, Prop: {propCount}, SafeNode: {screenNode}, Direction: {dirCount}".ToLog();
            "Vehicle spawn point is not found".ToLog();
            flags.Clear();
            groups.Clear();
            return Spawnpoint.Zero;
        }
        internal static Spawnpoint GetVehicleSpawnPoint3(Vector3 pos, float minimalDistance, float maximumDistance, bool considerDirection = false)
        {
            pos.GetFlags();
            Stopwatch sw = Stopwatch.StartNew();
            var minimalDistanceSquared = (float)Math.Pow(minimalDistance, 2);
            var maximumDistanceSquared = (float)Math.Pow(maximumDistance, 2);
            List<int> flags = new();
            int distanceCount, nodeCount, propCount, flagCount, screenNode, dirCount;
            distanceCount = nodeCount = propCount = flagCount = screenNode = dirCount = 0;
            NodeFlags[] bl = { NodeFlags.Junction, NodeFlags.TunnelOrUndergroundParking, NodeFlags.StopNode, NodeFlags.SpecialStopNode, NodeFlags.MinorRoad };
            for (int i = 1; i <= 2000; i++)
            {
                Vector3 v = pos.AroundPosition(Peralatan.Random.Next((int)minimalDistance, (int)maximumDistance));
                if (i % 80 == 0)
                {
                    GameFiber.Yield();
                }

                if (Natives.GET_CLOSEST_ROAD<bool>(v.X, v.Y, v.Z, 1f, 1, out Vector3 road1, out Vector3 road2, out int _, out int _, out float _, Peralatan.Random.Next(2) == 0))
                {
                    Vector3 nodeP = Peralatan.Random.Next(2) == 1 ? road1 : road2;
                    if (Natives.GET_VEHICLE_NODE_PROPERTIES<bool>(nodeP.X, nodeP.Y, nodeP.Z, out int _, out int flag))
                    {
                        NodeFlags nodeFlags = (NodeFlags)flag;
                        flags.Add(flag);
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
                    if (nodeP.DistanceToSquared(pos) < minimalDistanceSquared || nodeP.DistanceToSquared(pos) > maximumDistanceSquared + 25f)
                    {
                        distanceCount++;
                        continue;
                    }

                    if (IsNodeSafe(nodeP))
                    {
                        if (!considerDirection || Game.LocalPlayer.Character.GetHeadingTowards(nodeP).HeadingDiff(Game.LocalPlayer.Character) < 90)
                        {
                            float nodeH = GetRoadHeading(nodeP);
                            Spawnpoint ret = new(nodeP, nodeH);
                            $"Vehicle Spawn found {ret}. Distance: {ret.Position.DistanceTo(pos):0.00}".ToLog();
                            $"{i} Process took {sw.ElapsedMilliseconds} ms".ToLog();
                            ret.Position.GetFlags();
                            flags.Clear();
                            return ret;
                        }
                        else
                        {
                            dirCount++;
                            continue;
                        }
                    }
                    else
                    {
                        screenNode++;
                        continue;
                    }
                }
                else
                {
                    nodeCount++;
                    continue;
                }
            }
            var groups = flags.GroupBy(v => v).ToList();
            groups.ForEach(g => Peralatan.ToLog($"({(NodeFlags)g.Key}) has {g.Count()} items"));
            $"Distance: {distanceCount}, Node: {nodeCount} Flag: {flagCount}, Prop: {propCount}, SafeNode: {screenNode}, Direction: {dirCount}".ToLog();
            "Vehicle spawn point is not found".ToLog();
            flags.Clear();
            groups.Clear();
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
                Vector3 v = pos.AroundPosition(Peralatan.Random.Next((int)minimalDistance, (int)maximumDistance));
                if (i % 90 == 0)
                {
                    GameFiber.Yield();
                }

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
                        if (nodeP.DistanceToSquared(pos) < minimalDistanceSquared || nodeP.DistanceToSquared(pos) > maximumDistanceSquared)
                        {
                            distanceCount++;
                            continue;
                        }

                        if (!IsOnScreen(nodeP))
                        {
                            Spawnpoint ret = new(nodeP, MathExtension.GetRandomFloatInRange(0, 360));
                            $"Ped sidewalk spawn found {ret} Distance: {ret.Position.DistanceTo(pos)}".ToLog();
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
                else
                {
                    nodeCount++;
                }
            }
            $"Safe coord not found".ToLog();
            $"2000 process took {sw.ElapsedMilliseconds} ms".ToLog();
            $"Node: {nodeCount}, Flag: {flagCount}, Distance: {distanceCount}, Safe: {safeCount}, Prop: {propCount}".ToLog();
            $"Distance: Max: {Math.Sqrt(distanceList.Max())}, Mix: {Math.Sqrt(distanceList.Min())}, Avg: {Math.Sqrt(distanceList.Average())}".ToLog();
            return Spawnpoint.Zero;
        }
        internal static Spawnpoint GetVehicleSpawnPoint4(Vector3 pos, float minimalDistance, float maximumDistance, bool considerDirection = false)
        {
            pos.GetFlags();
            Stopwatch sw = Stopwatch.StartNew();
            var minimalDistanceSquared = (float)Math.Pow(minimalDistance, 2);
            var maximumDistanceSquared = (float)Math.Pow(maximumDistance, 2);
            List<int> flags = new();
            int distanceCount, nodeCount, propCount, flagCount, screenNode, dirCount;
            distanceCount = nodeCount = propCount = flagCount = screenNode = dirCount = 0;
            NodeFlags[] bl = { NodeFlags.Junction, NodeFlags.TunnelOrUndergroundParking, NodeFlags.StopNode, NodeFlags.SpecialStopNode, NodeFlags.MinorRoad };
            for (int i = 1; i <= 2000; i++)
            {
                Vector3 v = pos.AroundPosition(Peralatan.Random.Next((int)minimalDistance, (int)maximumDistance)).ToGround();
                if (i % 80 == 0)
                {
                    GameFiber.Yield();
                }

                if (Natives.GET_RANDOM_VEHICLE_NODE<bool>(v.X, v.Y, v.Z, 50f, true, true, true, out Vector3 nodeP, out int _))
                {
                    if (Natives.GET_VEHICLE_NODE_PROPERTIES<bool>(nodeP.X, nodeP.Y, nodeP.Z, out int _, out int flag))
                    {
                        NodeFlags nodeFlags = (NodeFlags)flag;
                        flags.Add(flag);
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
                    if (nodeP.DistanceToSquared(pos) < minimalDistanceSquared || nodeP.DistanceToSquared(pos) > maximumDistanceSquared + 25f)
                    {
                        distanceCount++;
                        continue;
                    }

                    if (IsNodeSafe(nodeP))
                    {
                        if (!considerDirection || Game.LocalPlayer.Character.GetHeadingTowards(nodeP).HeadingDiff(Game.LocalPlayer.Character) < 90)
                        {
                            float nodeH = GetRoadHeading(nodeP);
                            Spawnpoint ret = new(nodeP, nodeH);
                            $"Vehicle Spawn found {ret}. Distance: {ret.Position.DistanceTo(pos):0.00}".ToLog();
                            $"{i} Process took {sw.ElapsedMilliseconds} ms".ToLog();
                            ret.Position.GetFlags();
                            flags.Clear();
                            return ret;
                        }
                        else
                        {
                            dirCount++;
                            continue;
                        }
                    }
                    else
                    {
                        screenNode++;
                        continue;
                    }
                }
                else
                {
                    nodeCount++;
                    continue;
                }
            }
            var groups = flags.GroupBy(v => v).ToList();
            groups.ForEach(g => Peralatan.ToLog($"({(NodeFlags)g.Key}) has {g.Count()} items"));
            $"Distance: {distanceCount}, Node: {nodeCount} Flag: {flagCount}, Prop: {propCount}, SafeNode: {screenNode}, Direction: {dirCount}".ToLog();
            "Vehicle spawn point is not found".ToLog();
            flags.Clear();
            groups.Clear();
            return Spawnpoint.Zero;
        }
        internal static Spawnpoint GetVehicleSpawnPoint5(Vector3 pos, float minimalDistance, float maximumDistance, bool considerDirection = false)
        {
            pos.GetFlags();
            Stopwatch sw = Stopwatch.StartNew();
            var minimalDistanceSquared = (float)Math.Pow(minimalDistance, 2);
            var maximumDistanceSquared = (float)Math.Pow(maximumDistance, 2);
            List<int> flags = new();
            int distanceCount, nodeCount, propCount, flagCount, screenNode, dirCount;
            distanceCount = nodeCount = propCount = flagCount = screenNode = dirCount = 0;
            NodeFlags[] bl = { NodeFlags.Junction, NodeFlags.TunnelOrUndergroundParking, NodeFlags.StopNode, NodeFlags.SpecialStopNode, NodeFlags.MinorRoad };
            for (int i = 1; i <= 1000; i++)
            {
                Vector3 v = pos.AroundPosition(Peralatan.Random.Next((int)minimalDistance, (int)maximumDistance)).ToGround();
                if (i % 40 == 0)
                {
                    GameFiber.Yield();
                }
                Vector3 output = Vector3.Zero;
                Vector3 desiredPos = v + new Vector3(MathExtension.GetRandomFloatInRange(1.0f, 25.0f) * Peralatan.Random.Next(2) == 1 ? 1f : -1f, MathExtension.GetRandomFloatInRange(1.0f, 25.0f) * Peralatan.Random.Next(2) == 1 ? 1f : -1f, 0f);
                bool closestNode = Natives.GET_​CLOSEST_​VEHICLE_​NODE<bool>(v, out Vector3 closestNodeVector, 1, 3.0f, 0.0f);
                bool majorNode = Natives.GET_​CLOSEST_​MAJOR_​VEHICLE_​NODE<bool>(v, out Vector3 majorNodeVector, 3.0f, 0.0f);
                bool closestNodeHeading = Natives.GET_​CLOSEST_​VEHICLE_​NODE_​WITH_​HEADING<bool>(v, out Vector3 closestNodeHeadingVector, out float _, 1, 3.0f, 0.0f);
                bool nthClosestNode = Natives.GET_​NTH_​CLOSEST_​VEHICLE_​NODE<bool>(v, 1 % 5, out Vector3 nthClosestNodeVector, 1, 3.0f, 0.0f);
                bool nthClosestNodeHeading = Natives.GET_​NTH_​CLOSEST_​VEHICLE_​NODE_​WITH_​HEADING<bool>(v, i % 5, out Vector3 nthClosestNodeHeadingVector, out float _, out int _, 1, 3.0f, 2.5f);
                bool nthClosectNodeFavourDirection = Natives.GET_NTH_CLOSEST_VEHICLE_NODE_FAVOUR_DIRECTION<bool>(v, desiredPos, i % 5, out Vector3 nthClosectNodeFavourDirectionVector, out float _, 1, 3.0f, 0);
                Vector3[] vectors = { closestNodeVector, majorNodeVector, closestNodeHeadingVector, nthClosestNodeVector, nthClosestNodeHeadingVector, nthClosectNodeFavourDirectionVector };
                output = vectors.Where(x => x != Vector3.Zero).GetRandomElement();
                if (Natives.GET_VEHICLE_NODE_PROPERTIES<bool>(output.X, output.Y, output.Z, out int _, out int flag))
                    {
                    NodeFlags nodeFlags = (NodeFlags)flag;
                    flags.Add(flag);
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
                if (output.DistanceToSquared(pos) < minimalDistanceSquared || output.DistanceToSquared(pos) > maximumDistanceSquared + 25f)
                {
                    distanceCount++;
                    continue;
                }

                if (IsNodeSafe(output))
                {
                    if (!considerDirection || Game.LocalPlayer.Character.GetHeadingTowards(output).HeadingDiff(Game.LocalPlayer.Character) < 90)
                    {
                        float nodeH = GetRoadHeading(output);
                        Spawnpoint ret = new(output, nodeH);
                        $"Vehicle Spawn found {ret}. Distance: {ret.Position.DistanceTo(pos):0.00}".ToLog();
                        $"{i} Process took {sw.ElapsedMilliseconds} ms".ToLog();
                        ret.Position.GetFlags();
                        flags.Clear();
                        return ret;
                    }
                    else
                    {
                        dirCount++;
                        continue;
                    }
                }
                else
                {
                    screenNode++;
                    continue;
                }
            }
            var groups = flags.GroupBy(v => v).ToList();
            groups.ForEach(g => Peralatan.ToLog($"({(NodeFlags)g.Key}) has {g.Count()} items"));
            $"Distance: {distanceCount}, Node: {nodeCount} Flag: {flagCount}, Prop: {propCount}, SafeNode: {screenNode}, Direction: {dirCount}".ToLog();
            "Vehicle spawn point is not found".ToLog();
            flags.Clear();
            return Spawnpoint.Zero;
        }
        internal static Spawnpoint GetRoadSideSpawnPointFavored(Entity entity, float favoredDistance)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Vector3 fav = entity.GetOffsetPositionFront(favoredDistance - 5);
            for (int i = 1; i < 600; i++)
            {
                Vector3 pos = entity.GetOffsetPositionFront(Peralatan.Random.Next(1, 6) + favoredDistance - 10);
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
                if (i % 30 == 0)
                {
                    GameFiber.Yield();
                }
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
                    Vector3 v = pos.AroundPosition(Peralatan.Random.Next(1, 6));
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
                    if (i % 30 == 0)
                    {
                        GameFiber.Yield();
                    }
                }
            }
            else
            {
                for (int i = 1; i < 600; i++)
                {
                    Vector3 v = pos.AroundPosition(Peralatan.Random.Next(5, 15), Peralatan.Random.Next(20, 35));
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
                    if (i % 35 == 0)
                    {
                        GameFiber.Yield();
                    }
                }
            }
            for (int i = 1; i < 600; i++)
            {
                Vector3 v = pos.AroundPosition(Peralatan.Random.Next(25));
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
                if (i % 40 == 0)
                {
                    GameFiber.Yield();
                }
            }
            $"RoadSide position not found".ToLog();
            $"1200 process took {sw.ElapsedMilliseconds} ms".ToLog();
            return Spawnpoint.Zero;
        }
        internal static Vector3 GetRoadSidePointWithHeading(Entity entity)
        {
            var pos = entity.FrontPosition;
            if (Natives.xA0F8A7517A273C05<bool>(pos.X, pos.Y, pos.Z, entity.Heading, out Vector3 result))
            {
                return result;
            }
            return Vector3.Zero;
        }
        internal static Spawnpoint GetSlowRoadSpawnPoint(Vector3 pos, float minimumDistance, float maximumDistance)
        {
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < 0b1110000100; i++)
            {
                if (i % 40 == 0)
                {
                    GameFiber.Yield();
                }

                Vector3 v = pos.AroundPosition(Peralatan.Random.Next((int)minimumDistance, (int)(maximumDistance + 1)));
                if (Natives.xE50E52416CCF948B<bool>(v.X, v.Y, v.Z, i % 5, out Vector3 randomNode, true, 0f, 0f))
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
            int trys = 1000;
            int shouldYieldAt = 60;
            int distanceCount, safeCount, nodeCount, pedDisCount, propCount, flagCount, flag2Count, densityCount;
            bool log = false;
            distanceCount = safeCount = nodeCount = pedDisCount = propCount = flagCount = flag2Count = densityCount = 0;
            List<int> flags = new();
            try
            {
                ulong totalMem = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / 1048576;
                $"Total Memory: {totalMem} Mb".ToLog();
                if (totalMem > 16000)
                {
                    trys = 2000;
                    shouldYieldAt = 110;
                }
            }
            catch (System.ComponentModel.Win32Exception e) { (e.ToString() + " " + e.NativeErrorCode.ToString()).ToLog(); }

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
                if (i % shouldYieldAt == 0)
                {
                    GameFiber.Yield();
                }

                Vector3 around = playerPos.Around2D(Peralatan.Random.Next(250, 500));
                if (Natives.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING<bool>(around.X, around.Y, around.Z, out Vector3 nodePos, out float nodeHeading, 0, 3, 0, 0))
                {
                    if (nodePos.DistanceToSquared(playerPos) > 250000f || nodePos.TravelDistanceTo(playerPos) > 1250)
                    {
                        distanceCount++;
                        continue;
                    }

                    if (Natives.GET_VEHICLE_NODE_PROPERTIES<bool>(nodePos.X, nodePos.Y, nodePos.Z, out int density, out int flag))
                    {
                        NodeFlags nodeFlags = (NodeFlags)flag;
                        flags.Add(flag);
                        if (blacklist.Any(flag => nodeFlags.HasFlag(flag)))
                        {
                            flagCount++;
                            continue;
                        }
                        if (density > 10f)
                        {
                            densityCount++;
                            continue;
                        }
                        if (!log && nodeFlags == NodeFlags.None)
                        {
                            $"Node 0: {nodePos}".ToLog();
                            log = true;
                        }
                        if (nodeFlags.HasFlag(NodeFlags.SlowNormalRoad) || nodeFlags.HasFlag(NodeFlags.UnknownBit2) || nodeFlags == NodeFlags.None)
                        {
                            if (Natives.GET_SAFE_COORD_FOR_PED<bool>(nodePos.X, nodePos.Y, nodePos.Z, true, out Vector3 pedNodePos, 17))
                            {
                                if (pedNodePos.DistanceToSquared(nodePos) > 2500f || pedNodePos.DistanceToSquared(playerPos) < 90000f)
                                {
                                    pedDisCount++;
                                    continue;
                                }
                                bool success = Natives.x16F46FB18C8009E4<bool>(pedNodePos.X, pedNodePos.Y, pedNodePos.Z, -1, out Vector3 roadSidePos);
                                if (!success)
                                {
                                    propCount++;
                                    continue;
                                }
                                //if (roadSidePos.DistanceTo(pedNodePos) < 8f) continue;
                                Spawnpoint ret = new(pedNodePos, pedNodePos.GetHeadingTowards(nodePos));
                                $"Get solicitation spawnpoint is successfull {ret}, {i + 1} process took {sw.ElapsedMilliseconds} ms".ToLog();
                                $"Solicitation Spawnpoint => Distance {ret.Position.DistanceTo(playerPos)}, Travel Distance: {ret.Position.TravelDistanceTo(playerPos)}".ToLog();
                                pedNodePos.GetFlags();
                                nodePos.GetFlags();
                                roadSidePosition = new(roadSidePos, 0f);
                                nodePosition = new(nodePos, nodeHeading);
                                return ret;
                            }
                            else
                            {
                                safeCount++;
                            }
                        }
                        else
                        {
                            flag2Count++;
                        }
                    }
                }
                else
                {
                    nodeCount++;
                }
            }
            $"Solicitation spawnpoint was not successfull, {sw.ElapsedMilliseconds} ms".ToLog();
            $"Distance: {distanceCount}, Node: {nodeCount} Flag1: {flagCount}, Flag2: {flag2Count}, Safe: {safeCount}, Prop: {propCount}, Ped Node Distance: {pedDisCount}, Density: {densityCount}".ToLog();
            var groups = flags.GroupBy(v => v).ToList();
            groups.ForEach(g => Peralatan.ToLog($"Value {g.Key} has {g.Count()} items"));
            return Spawnpoint.Zero;
        }
        internal static Spawnpoint GetRandomFleeingPoint(Ped fleeingPed)
        {
            Vector3 pos = fleeingPed.Position;
            short dirCount, nodeCount;
            dirCount = nodeCount = 0;
            for (int i = 0; i < 2000; i++)
            {
                if (i % 50 == 1) GameFiber.Yield();
                Vector3 v = pos.Around(Peralatan.Random.Next(800, 1250));
                if (Natives.GET_NTH_CLOSEST_VEHICLE_NODE_WITH_HEADING<bool>(v, i % 5 + 1, out Vector3 nodeP, out float heading, out uint _, 9, 3.0f, 2.5f))
                {
                    if (MathExtension.FloatDiff(pos.GetHeadingTowards(nodeP), pos.GetHeadingTowards(Game.LocalPlayer.Character)) > 90f)
                    {
                        Spawnpoint ret = new(nodeP, heading);
                        Peralatan.ToLog($"Fleeing point is found {ret}");
                        return ret;
                    }
                    else dirCount++;
                }
                else nodeCount++;
            }
            Peralatan.ToLog($"Fleeing point is not found");
            Peralatan.ToLog($"Node: {nodeCount}, Direction: {dirCount}");
            return Spawnpoint.Zero;
        }
        public static bool GetSafeCoordForPed(Vector3 pos, bool onFootpath, out Vector3 result, int flag)
        {
            bool ret = Natives.GET_SAFE_COORD_FOR_PED<bool>(pos.X, pos.Y, pos.Z, onFootpath, out result, flag);
            return ret;
        }
    }
}