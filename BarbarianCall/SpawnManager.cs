using System;
using System.Diagnostics;
using BarbarianCall.Extensions;
using BarbarianCall.Types;
using Rage;
using Rage.Native;

namespace BarbarianCall
{
    internal static class SpawnManager
    {
        private static bool IsNodeSafe(Vector3 v)
        {
            try
            {
                int nodeType = NativeFunction.Natives.x22D7275A79FE8215<int>(v.X, v.Y, v.Z, 1, 0, 3f, 0f);
                if (NativeFunction.Natives.x1EAF30FCFBF5AF74<bool>(nodeType)) /*IS_VEHICLE_NODE_ID_VALID*/
                {
                    if (!NativeFunction.Natives.x4F5070AA58F69279<bool>(nodeType))/*GET_VEHICLE_NODE_IS_SWITCHED_OFF*/
                    {
                        if (NativeFunction.Natives.xA2AE5C478B96E3B6<bool>(nodeType))/*GET_VEHICLE_NODE_IS_GPS_ALLOWED*/
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
        private static bool IsCoordOnScreen(Vector3 pos) => NativeFunction.Natives.xF9904D11F1ACBEC3<bool>(pos.X, pos.Y, pos.Z, out float _, out float _); //GET_HUD_SCREEN_POSITION_FROM_WORLD_POSITION
        private static bool IsSlowRoad(Vector3 position) => (bool)NativeFunction.CallByHash<bool>(0b10_0010_1101_0111_0010_0111_0101_1010_0111_1001_1111_1110_1000_0010_0001_0101, position.X, position.Y, position.Z, 1, 5, 1077936128, 0f);
        internal static float GetRoadHeading(Vector3 pos)
        {
            NativeFunction.Natives.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING<bool>(pos.X, pos.Y, pos.Z, out Vector3 _, out float heading, 12, 0x40400000, 0);
            return heading;

        }
        internal static Spawnpoint GetVehicleSpawnPoint(ISpatial spatial, float minimalDistance, float maximumDistance, bool considerDirection = false) => 
            GetVehicleSpawnPoint(spatial.Position, minimalDistance, maximumDistance, considerDirection);
        internal static Spawnpoint GetVehicleSpawnPoint(Vector3 pos, float minimalDistance, float maximumDistance, bool considerDirection = false)
        {
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 1; i < 900; i++)
            {
                Vector3 v = pos.Around2D(Peralatan.Random.Next((int)minimalDistance, (int)maximumDistance));
                if (i % 35 == 0) GameFiber.Yield();
                if (NativeFunction.Natives.xFF071FB798B803B0<bool>(v.X, v.Y, v.Z, out Vector3 nodeP, out float nodeH, 12, 3.0f, 0))
                {
                    if (nodeP.DistanceTo(pos) < minimalDistance || nodeP.DistanceTo(pos) > maximumDistance + 5f) continue;
                    if (nodeP.TravelDistanceTo(pos) < maximumDistance * 2f && IsNodeSafe(nodeP) && !IsCoordOnScreen(nodeP))
                    {
                        if (!considerDirection || Game.LocalPlayer.Character.GetHeadingTowards(nodeP).HeadingDiff(Game.LocalPlayer.Character) < 90)
                        {
                            Spawnpoint ret = new Spawnpoint(nodeP, nodeH);
                            $"Vehicle Spawn found {ret}. Distance: {ret.DistanceTo(pos):0.00}".ToLog();
                            $"{i} Process took {sw.ElapsedMilliseconds} ms".ToLog();
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
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 1; i < 600; i++)
            {
                if (i % 40 == 0) GameFiber.Yield();
                Vector3 v = pos.Around2D(Peralatan.Random.Next((int)Math.Abs(minimalDistance), (int)Math.Abs(maximumDistance)));
                if (NativeFunction.Natives.x80CA6A8B6C094CC4<bool>(v.X, v.Y, v.Z, i % 5 + 1, out Vector3 nodeP, out float nodeH, out dynamic unk1, 1, 0x40400000, 0))
                {
                    if (nodeP.DistanceTo(pos) > minimalDistance && nodeP.DistanceTo(pos) < maximumDistance && nodeP.TravelDistanceTo(pos) < maximumDistance * 2 && IsNodeSafe(nodeP) && !IsCoordOnScreen(nodeP))
                    {
                        Spawnpoint ret = new Spawnpoint(nodeP, nodeH);
                        $"Vehicle Spawn found {ret}. Distance: {ret.DistanceTo(pos):0.00}. {unk1}".ToLog();
                        $"{i} Process took {sw.ElapsedMilliseconds} ms".ToLog();
                        return ret;
                    }
                }
            }
            return Spawnpoint.Zero;
        }
        internal static Spawnpoint GetPedSpawnPoint(ISpatial spatial, float minimalDistance, float maximumDistance) => GetPedSpawnPoint(spatial.Position, minimalDistance, maximumDistance);
        internal static Spawnpoint GetPedSpawnPoint(Vector3 pos, float minimalDistance, float maximumDistance)
        {
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 1; i < 600; i++)
            {
                Vector3 v = pos.Around2D(Peralatan.Random.Next((int)minimalDistance, (int)maximumDistance));
                if (i % 15 == 0) GameFiber.Yield();
                if (NativeFunction.Natives.xB61C8E878A4199CA<bool>(v.X, v.Y, v.Z, true, out Vector3 nodeP, 0))
                {
                    if (nodeP.DistanceTo(pos) < minimalDistance || nodeP.DistanceTo(pos) > maximumDistance) continue;
                    if (nodeP.TravelDistanceTo(pos) < maximumDistance * 2f && !IsCoordOnScreen(nodeP))
                    {
                        Spawnpoint ret = new Spawnpoint(nodeP, Extension.GetRandomAbsoluteSingle(1, 359));
                        $"Ped sidewalk spawn found {ret} Distance: {ret.DistanceTo(pos)}".ToLog();
                        $"{i} Process took {sw.ElapsedMilliseconds} ms".ToLog();
                        return ret;
                    }
                }
            }
            for (int i = 1; i < 600; i++)
            {
                Vector3 v = pos.Around2D(Peralatan.Random.Next((int)minimalDistance, (int)maximumDistance));
                if (i % 30 == 0) GameFiber.Yield();
                if (NativeFunction.Natives.xB61C8E878A4199CA<bool>(v.X, v.Y, v.Z, false, out Vector3 nodeP, 0))
                {
                    if (nodeP.DistanceTo(pos) < minimalDistance || nodeP.DistanceTo(pos) > maximumDistance) continue;
                    if (nodeP.TravelDistanceTo(pos) < maximumDistance * 2f && !IsCoordOnScreen(nodeP))
                    {
                        Spawnpoint ret = new Spawnpoint(nodeP, Extension.GetRandomAbsoluteSingle(1, 359));
                        $"Ped safe spawn found {ret} Distance: {ret.DistanceTo(pos)}".ToLog();
                        $"{i + 600} Process took {sw.ElapsedMilliseconds} ms".ToLog();
                        return ret;
                    }
                }
            }
            $"Safe coord not found".ToLog();
            $"1200 process took {sw.ElapsedMilliseconds} ms".ToLog();
            return Spawnpoint.Zero;
        }
        internal static Spawnpoint GetRoadSideSpawnPointFavored(Entity entity, float favoredDistance)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Vector3 fav = entity.GetOffsetPositionFront(favoredDistance - 5);
            for (int i = 1; i < 600; i++)
            {
                Vector3 pos = entity.GetOffsetPositionFront(Peralatan.Random.Next(1, 6)+ favoredDistance - 10);
                if (NativeFunction.Natives.x45905BE8654AE067<bool>(pos.X, pos.Y, pos.Z, fav.X, fav.Y, fav.Z, i % 2 + 1, out Vector3 nodeP, out float nodeH, 0, 0x40400000, 0))
                //GET_NTH_CLOSEST_VEHICLE_NODE_FAVOUR_DIRECTION
                {
                    if (NativeFunction.Natives.xA0F8A7517A273C05<bool>(nodeP.X, nodeP.Y, nodeP.Z, nodeH, out Vector3 roadSide))
                    {
                        if (roadSide.DistanceTo(entity) < 35 + favoredDistance && !roadSide.IsOccupied() && !IsCoordOnScreen(roadSide))
                        {
                            Spawnpoint ret = new Spawnpoint(roadSide, nodeH);
                            string.Format("Favored RoadSide Spawnpoint found {0}", ret).ToLog();
                            string.Format("{0} process took {1}", i, sw.ElapsedMilliseconds).ToLog();
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
                    if (NativeFunction.Natives.xA0F8A7517A273C05<bool>(v.X, v.Y, v.Z, heading.Value, out Vector3 rsPos)) //_GET_ROAD_SIDE_POINT_WITH_HEADING
                    {
                        if (NativeFunction.Natives.xFF071FB798B803B0<bool>(rsPos.X, rsPos.Y, rsPos.Z, out Vector3 _, out float nodeHeading, 5, 3.0f, 0))
                        {
                            if (rsPos.DistanceTo(pos) < 100f && !rsPos.IsOccupied())
                            {
                                Spawnpoint ret = new Spawnpoint(rsPos, nodeHeading);
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
                    if (NativeFunction.Natives.xFF071FB798B803B0<bool>(v.X, v.Y, v.Z, out Vector3 nodePos, out float nodeHeading, 5, 3.0f, 0))
                    {
                        if (NativeFunction.Natives.xA0F8A7517A273C05<bool>(nodePos.X, nodePos.Y, nodePos.Z, nodeHeading, out Vector3 rsPos)) //_GET_ROAD_SIDE_POINT_WITH_HEADING
                        {
                            if (rsPos.DistanceTo(pos) < 100f && !rsPos.IsOccupied())
                            {
                                Spawnpoint ret = new Spawnpoint(rsPos, nodeHeading);
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
                if (NativeFunction.Natives.x16F46FB18C8009E4<bool>(v.X, v.Y, v.Z, 0, out Vector3 roadSide)) //_GET_POINT_ON_ROAD_SIDE
                {
                    if (NativeFunction.Natives.xFF071FB798B803B0<bool>(roadSide.X, roadSide.Y, roadSide.Z, out Vector3 nodePos, out float nodeHeading, 12, 0x40400000, 0))
                    {
                        if (nodePos.DistanceTo(roadSide) < 50f && roadSide.DistanceTo(pos) < 100f && !roadSide.IsOccupied())
                        {
                            Spawnpoint ret = new Spawnpoint(roadSide, nodeHeading);
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
                if (NativeFunction.Natives.x93E0DB8440B73A7D<bool>(v.X, v.Y, v.Z, 80f, false, false, false, out Vector3 randomNode, out int nodeId))
                {
                    if (NativeFunction.Natives.x1EAF30FCFBF5AF74<bool>(nodeId))
                    {
                        if (NativeFunction.Natives.x4F5070AA58F69279<bool>(nodeId))
                        {
                            if (randomNode.DistanceTo(pos) < maximumDistance && randomNode.DistanceTo(pos) > minimumDistance && !IsCoordOnScreen(randomNode))
                            {
                                if (NativeFunction.Natives.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING<bool>(randomNode.X, randomNode.Y, randomNode.Z, out Vector3 _, out float heading, 0b1001, 3.0f, 0f))
                                {
                                    Spawnpoint ret = new Spawnpoint(randomNode, heading);
                                    $"Slow road found at {i + 1} tries, that process took {sw.ElapsedMilliseconds} ms".ToLog();
                                    return ret;
                                }
                            }
                        }
                    }
                }
            }
            return Spawnpoint.Zero;
        }
    }
}