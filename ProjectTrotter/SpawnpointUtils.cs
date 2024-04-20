using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using ProjectTrotter.Extensions;
using ProjectTrotter.Types;
using Rage;
using static Rage.Native.NativeFunction;

namespace ProjectTrotter
{
    internal static class SpawnpointUtils
    {
        internal static void LogNodePropertiesAndFlags(this Vector3 position)
        {
            if (Natives.GetVehicleNodeProperties<bool>(position, out uint density, out uint flag))
            {
                LogNodeFlags(position, flag);
                Logger.Log($"Density: {density}. Flag: {flag}");
            }
            else
            {
                "Unsuccessful Getting node flag value".ToLog();
            }
        }
        internal static float GetRoadHeading(Vector3 pos)
        {
            Natives.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING<bool>(pos.X, pos.Y, pos.Z, out Vector3 _, out float heading, 12, 0x40400000, 0);
            return heading;
        }
        internal static Spawnpoint GetVehicleSpawnPoint(Vector3 pos, float minimalDistance, float maximumDistance, bool considerDirection = false)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var minimalDistanceSquared = (float)Math.Pow(minimalDistance, 2);
            var maximumDistanceSquared = (float)Math.Pow(maximumDistance, 2);
            HashSet<int> flags = new();
            int distanceCount, propCount, flagCount, screenNode, dirCount;
            distanceCount = propCount = flagCount = screenNode = dirCount = 0;
            NodeFlags[] excludedNodeFlags = { NodeFlags.Junction, NodeFlags.TunnelOrUndergroundParking, NodeFlags.StopNode, NodeFlags.SpecialStopNode, NodeFlags.MinorRoad };

            Vector3[] vehicleNodes = NodeUtils.GetVehicleNodesAroundPosition(pos, minimalDistance, maximumDistance);

            for (int i = 0; i < vehicleNodes.Length; i++)
            {
                Vector3 nodePosition = vehicleNodes[i];
                if (Natives.GET_VEHICLE_NODE_PROPERTIES<bool>(nodePosition.X, nodePosition.Y, nodePosition.Z, out int _, out int flag))
                {
                    NodeFlags nodeFlags = (NodeFlags)flag;
                    flags.Add(flag);
                    if (excludedNodeFlags.Any(x => nodeFlags.HasFlag(x)))
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

                if (nodePosition.DistanceToSquared(pos) < minimalDistanceSquared || nodePosition.DistanceToSquared(pos) > maximumDistanceSquared + 25f)
                {
                    distanceCount++;
                    continue;
                }

                if (NodeUtils.IsNodeSafe(nodePosition))
                {
                    if (!considerDirection || Game.LocalPlayer.Character.GetHeadingTowards(nodePosition).HeadingDiff(Game.LocalPlayer.Character) < 90)
                    {
                        float nodeHeading = GetRoadHeading(nodePosition);
                        Spawnpoint spawnpoint = new(nodePosition, nodeHeading);
                        LogSpawnpointFound(spawnpoint, pos, sw.ElapsedMilliseconds);
                        flags.Clear();
                        return spawnpoint;
                    }
                    else
                    {
                        dirCount++;
                    }
                }
                else
                {
                    screenNode++;
                }
            }

            LogStatistics(distanceCount, propCount, flagCount, screenNode, dirCount, flags);
            "Vehicle spawn point is not found".ToLog();
            flags.Clear();
            return Spawnpoint.Zero;
        }
        internal static Spawnpoint GetPedSpawnPoint(Vector3 pos, float minimalDistance, float maximumDistance)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            float minimalDistanceSquared = (float)Math.Pow(minimalDistance, 2);
            float maximumDistanceSquared = (float)Math.Pow(maximumDistance, 2);
            SortedSet<float> distanceSquaredValues = new();

            int nodeCount, flagCount, distanceCount, safeCount, propCount;
            nodeCount = flagCount = distanceCount = safeCount = propCount = 0;

            pos.LogNodePropertiesAndFlags();

            int maxAttempts = 2000;
            int attempt = 0;

            while (attempt < maxAttempts)
            {
                attempt++;
                Vector3 randomPosition = pos.AroundPosition(MyRandom.Next((int)minimalDistance, (int)maximumDistance));

                if (TryGetValidVehicleNode(randomPosition, out Vector3 vehicleNode, out bool isValidNode))
                {
                    if (isValidNode)
                    {
                        if (TryGetSafeCoordForPed(vehicleNode, out Vector3 safePosition))
                        {
                            float distanceToPlayerSquared = safePosition.DistanceToSquared(Game.LocalPlayer.Character);
                            if (distanceToPlayerSquared >= minimalDistanceSquared && distanceToPlayerSquared <= maximumDistanceSquared && !NodeUtils.IsOnScreen(safePosition))
                            {
                                Spawnpoint spawnPoint = new(safePosition, MathExtension.GetRandomFloatInRange(0, 360));
                                LogSpawnPointFound(spawnPoint, pos, attempt, stopwatch.ElapsedMilliseconds);
                                return spawnPoint;
                            }
                            else
                            {
                                distanceCount++;
                                distanceSquaredValues.Add(distanceToPlayerSquared);
                            }
                        }
                        else
                        {
                            safeCount++;
                        }
                    }
                    else
                    {
                        flagCount++;
                    }
                }
                else
                {
                    nodeCount++;
                    propCount++;
                }

                if (attempt % 90 == 0)
                {
                    GameFiber.Yield();
                }
            }

            LogStatistics(nodeCount, flagCount, distanceCount, safeCount, propCount, distanceSquaredValues);
            return Spawnpoint.Zero;
        }
        internal static Spawnpoint GetSpawnPointFavoringDirection(Entity entity, float favoredDirectionDistance)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Vector3 favoredPosition = entity.GetOffsetPositionFront(favoredDirectionDistance - 5);

            int maxAttempts = 600;
            int attempt = 0;

            while (attempt < maxAttempts)
            {
                attempt++;
                Vector3 candidatePosition = entity.GetOffsetPositionFront(MyRandom.Next(1, 6) + favoredDirectionDistance - 10);

                if (TryGetFavoredVehicleNode(candidatePosition, favoredPosition, attempt, out Vector3 nodePosition, out float nodeHeading) &&
                    TryGetRoadBoundaryFromNode(nodePosition, nodeHeading, out Vector3 roadSidePosition) &&
                    IsValidSpawnPoint(roadSidePosition, entity, favoredDirectionDistance))
                {
                    Spawnpoint spawnPoint = new(roadSidePosition, nodeHeading);
                    LogSpawnPointFound(spawnPoint, attempt, stopwatch.ElapsedMilliseconds);
                    return spawnPoint;
                }

                if (attempt % 30 == 0)
                {
                    GameFiber.Yield();
                }
            }

            "Failed to find a favored direction spawn point after maximum attempts".ToLog();
            return Spawnpoint.Zero;
        }
        internal static Spawnpoint GetRoadSideSpawnPoint(Vector3 pos, float? heading = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            if (heading.HasValue)
            {
                for (int i = 1; i < 600; i++)
                {
                    Vector3 v = pos.AroundPosition(MyRandom.Next(1, 6));
                    if (Natives.GET_ROAD_BOUNDARY_USING_HEADING<bool>(v.X, v.Y, v.Z, heading.Value, out Vector3 rsPos))
                    {
                        if (Natives.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING<bool>(rsPos.X, rsPos.Y, rsPos.Z, out Vector3 _, out float nodeHeading, 5, 3.0f, 0))
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
                    Vector3 v = pos.AroundPosition(MyRandom.Next(5, 15), MyRandom.Next(20, 35));
                    if (Natives.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING<bool>(v.X, v.Y, v.Z, out Vector3 nodePos, out float nodeHeading, 5, 3.0f, 0))
                    {
                        if (Natives.GET_ROAD_BOUNDARY_USING_HEADING<bool>(nodePos.X, nodePos.Y, nodePos.Z, nodeHeading, out Vector3 rsPos))
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
                Vector3 v = pos.AroundPosition(MyRandom.Next(25));
                if (Natives.GET_POSITION_BY_SIDE_OF_ROAD<bool>(v.X, v.Y, v.Z, 0, out Vector3 roadSide))
                {
                    if (Natives.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING<bool>(roadSide.X, roadSide.Y, roadSide.Z, out Vector3 nodePos, out float nodeHeading, 12, 0x40400000, 0))
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
            if (Natives.GET_ROAD_BOUNDARY_USING_HEADING<bool>(pos.X, pos.Y, pos.Z, entity.Heading, out Vector3 result))
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

                Vector3 v = pos.AroundPosition(MyRandom.Next((int)minimumDistance, (int)(maximumDistance + 1)));
                if (Natives.GET_NTH_CLOSEST_VEHICLE_NODE<bool>(v.X, v.Y, v.Z, i % 5, out Vector3 randomNode, true, 0f, 0f))
                {
                    int nodeId = Natives.GET_NTH_CLOSEST_VEHICLE_NODE_ID<int>(randomNode.X, randomNode.Y, randomNode.Z, 1, 11077936128f, 0f);
                    if (Natives.IS_VEHICLE_NODE_ID_VALID<bool>(nodeId))
                    {
                        if (Natives.GET_VEHICLE_NODE_IS_SWITCHED_OFF<bool>(nodeId))
                        {
                            if (randomNode.DistanceTo(pos) < maximumDistance && randomNode.DistanceTo(pos) > minimumDistance && !NodeUtils.IsOnScreen(randomNode) && pos.HeightDiff(randomNode) < 18f)
                            {
                                if (Natives.GET_NTH_CLOSEST_VEHICLE_NODE_WITH_HEADING<bool>(randomNode.X, randomNode.Y, randomNode.Z, 1, out Vector3 _, out float heading, out int _, 1, 3f, 0f))
                                {
                                    Spawnpoint ret = new(randomNode, heading);
                                    $"Slow road found at {i + 1} tries, that process took {sw.ElapsedMilliseconds} ms".ToLog();
                                    pos.LogNodePropertiesAndFlags();
                                    ret.Position.LogNodePropertiesAndFlags();
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

                Vector3 around = playerPos.Around2D(MyRandom.Next(250, 500));
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
                                bool success = Natives.GET_POSITION_BY_SIDE_OF_ROAD<bool>(pedNodePos.X, pedNodePos.Y, pedNodePos.Z, -1, out Vector3 roadSidePos);
                                if (!success)
                                {
                                    propCount++;
                                    continue;
                                }
                                //if (roadSidePos.DistanceTo(pedNodePos) < 8f) continue;
                                Spawnpoint ret = new(pedNodePos, pedNodePos.GetHeadingTowards(nodePos));
                                $"Get solicitation spawnpoint is successfull {ret}, {i + 1} process took {sw.ElapsedMilliseconds} ms".ToLog();
                                $"Solicitation Spawnpoint => Distance {ret.Position.DistanceTo(playerPos)}, Travel Distance: {ret.Position.TravelDistanceTo(playerPos)}".ToLog();
                                pedNodePos.LogNodePropertiesAndFlags();
                                nodePos.LogNodePropertiesAndFlags();
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
            groups.ForEach(g => Logger.Log($"Value {g.Key} has {g.Count()} items"));
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
                Vector3 v = pos.Around(MyRandom.Next(800, 1250));
                if (Natives.GET_NTH_CLOSEST_VEHICLE_NODE_WITH_HEADING<bool>(v, i % 5 + 1, out Vector3 nodeP, out float heading, out uint _, 9, 3.0f, 2.5f))
                {
                    if (MathExtension.FloatDiff(pos.GetHeadingTowards(nodeP), pos.GetHeadingTowards(Game.LocalPlayer.Character)) > 90f)
                    {
                        Spawnpoint ret = new(nodeP, heading);
                        Logger.Log($"Fleeing point is found {ret}");
                        return ret;
                    }
                    else dirCount++;
                }
                else nodeCount++;
            }
            Logger.Log($"Fleeing point is not found");
            Logger.Log($"Node: {nodeCount}, Direction: {dirCount}");
            return Spawnpoint.Zero;
        }
        internal static bool GetSafeCoordForPed(Vector3 pos, bool onFootpath, out Vector3 result, int flag)
        {
            bool ret = Natives.GET_SAFE_COORD_FOR_PED<bool>(pos.X, pos.Y, pos.Z, onFootpath, out result, flag);
            return ret;
        }
        private static bool TryGetFavoredVehicleNode(Vector3 candidatePosition, Vector3 favoredPosition, int attempt, out Vector3 nodePosition, out float nodeHeading)
        {
            int nodeIndex = attempt % 2 + 1;
            return Natives.GET_NTH_CLOSEST_VEHICLE_NODE_FAVOUR_DIRECTION<bool>(candidatePosition.X, candidatePosition.Y, candidatePosition.Z, favoredPosition.X, favoredPosition.Y, favoredPosition.Z, nodeIndex, out nodePosition, out nodeHeading, 0, 3.0f, 0);
        }
        private static bool TryGetRoadBoundaryFromNode(Vector3 nodePosition, float nodeHeading, out Vector3 roadSidePosition)
        {
            return Natives.GET_ROAD_BOUNDARY_USING_HEADING<bool>(nodePosition.X, nodePosition.Y, nodePosition.Z, nodeHeading, out roadSidePosition);
        }
        private static bool IsValidSpawnPoint(Vector3 spawnPosition, Entity entity, float favoredDirectionDistance)
        {
            float maxDistance = 35 + favoredDirectionDistance;
            return spawnPosition.DistanceTo(entity) < maxDistance && !spawnPosition.IsOccupied() && !NodeUtils.IsOnScreen(spawnPosition);
        }
        private static bool TryGetValidVehicleNode(Vector3 position, out Vector3 vehicleNode, out bool isValidNode)
        {
            isValidNode = false;
            if (Natives.GET_NTH_CLOSEST_VEHICLE_NODE<bool>(position, 1 % 5, out vehicleNode, 1, 0f, 0f))
            {
                if (Natives.GET_VEHICLE_NODE_PROPERTIES<bool>(vehicleNode.X, vehicleNode.Y, vehicleNode.Z, out int _, out int flag))
                {
                    NodeFlags[] excludedNodeFlags = { NodeFlags.Freeway, NodeFlags.Junction, NodeFlags.TunnelOrUndergroundParking, NodeFlags.StopNode, NodeFlags.SpecialStopNode };
                    NodeFlags nodeFlags = (NodeFlags)flag;
                    isValidNode = !excludedNodeFlags.Any(x => nodeFlags.HasFlag(x));
                }
            }
            return isValidNode;
        }
        private static bool TryGetSafeCoordForPed(Vector3 position, out Vector3 safePosition)
        {
            return Natives.GET_SAFE_COORD_FOR_PED<bool>(position.X, position.Y, position.Z, true, out safePosition, (new[] { 17, 1, 16 }).GetRandomElement());
        }
        private static void LogStatistics(int nodeCount, int flagCount, int distanceCount, int safeCount, int propCount, SortedSet<float> distanceSquaredValues)
        {
            $"Safe coord not found".ToLog();
            $"Node: {nodeCount}, Flag: {flagCount}, Distance: {distanceCount}, Safe: {safeCount}, Prop: {propCount}".ToLog();

            if (distanceSquaredValues.Any())
            {
                float maxDistance = (float)Math.Sqrt(distanceSquaredValues.Max);
                float minDistance = (float)Math.Sqrt(distanceSquaredValues.Min);
                float avgDistance = (float)Math.Sqrt(distanceSquaredValues.Sum() / distanceSquaredValues.Count);
                $"Distance: Max: {maxDistance}, Min: {minDistance}, Avg: {avgDistance}".ToLog();
            }
        }
        private static void LogSpawnPointFound(Spawnpoint spawnPoint, Vector3 pos, int attempt, long elapsedMilliseconds)
        {
            $"Ped sidewalk spawn found {spawnPoint} Distance: {spawnPoint.Position.DistanceTo(pos)}".ToLog();
            $"Attempt {attempt} took {elapsedMilliseconds} milliseconds".ToLog();
            spawnPoint.Position.LogNodePropertiesAndFlags();
        }
        private static void LogSpawnPointFound(Spawnpoint spawnPoint, int attempt, long elapsedMilliseconds)
        {
            $"Favored direction spawn point found: {spawnPoint}".ToLog();
            $"Attempt {attempt} took {elapsedMilliseconds} milliseconds".ToLog();
            spawnPoint.Position.LogNodePropertiesAndFlags();
        }
        private static void LogSpawnpointFound(Spawnpoint spawnpoint, Vector3 pos, long elapsedMilliseconds)
        {
            $"Vehicle Spawn found {spawnpoint}. Distance: {spawnpoint.Position.DistanceTo(pos):0.00}".ToLog();
            $"{elapsedMilliseconds} ms".ToLog();
            spawnpoint.Position.LogNodePropertiesAndFlags();
        }
        private static void LogStatistics(int distanceCount, int propCount, int flagCount, int screenNode, int dirCount, HashSet<int> flags)
        {
            var groups = flags.GroupBy(v => v).ToList();
            groups.ForEach(g => Logger.Log($"({(NodeFlags)g.Key}) has {g.Count()} items"));
            $"Distance: {distanceCount}, Prop: {propCount}, Flag: {flagCount}, SafeNode: {screenNode}, Direction: {dirCount}".ToLog();
        }
        private static void LogNodeFlags(Vector3 position, uint flag)
        {
            position.ToString().ToLog();

            var nodeFlags = (NodeFlags)flag;
            var flagNames = nodeFlags.GetFlagNames();

            if (flagNames.Any())
            {
                Logger.Log($"Flags: {string.Join(", ", flagNames)}");
            }
            else
            {
                Logger.Log("No flags set");
            }
        }
        private static IEnumerable<string> GetFlagNames(this NodeFlags flags)
        {
            var flagNames = new List<string>();

            foreach (var flag in Enum.GetValues(typeof(NodeFlags)).Cast<NodeFlags>())
            {
                if (flags.HasFlag(flag))
                {
                    flagNames.Add(flag.ToString());
                }
            }

            return flagNames;
        }
    }
}