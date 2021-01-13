using System;
using BarbarianCall.Extensions;
using BarbarianCall.Types;
using Rage;
using Rage.Native;

namespace BarbarianCall
{
    internal static class SpawnManager
    {
        internal static SpawnPoint GetVehicleSpawnPoint(Vector3 pos, float minimalDistance, float maximumDistance)
        {
            DateTime start = DateTime.UtcNow;
            for (int i = 0; i < 900; i++)
            {
                Vector3 v = pos.Around(minimalDistance, maximumDistance + Extension.GetRandomAbsoluteSingle(1, 5));
                if (i % 35 == 0) GameFiber.Yield();
                if (NativeFunction.Natives.xFF071FB798B803B0<bool>(v.X, v.Y, v.Z, out Vector3 nodeP, out float nodeH, 5, 3.0f, 0))
                {
                    if (nodeP.DistanceTo(pos) < minimalDistance || nodeP.DistanceTo(pos) > maximumDistance) continue;
                    if (nodeP.TravelDistanceTo(pos) < maximumDistance * 3f)
                    {
                        SpawnPoint ret = new SpawnPoint(nodeP, nodeH);
                        $"Vehicle Spawn found {ret}".ToLog();
                        $"{i} Process took {(DateTime.UtcNow - start).TotalMilliseconds} ms".ToLog();
                        return ret;
                    }
                }
            }
            "Vehicle spawn point is not found".ToLog();
            return SpawnPoint.Zero;
        }
        internal static SpawnPoint GetPedSpawnPoint(Vector3 pos, float minimalDistance, float maximumDistance)
        {
            DateTime start = DateTime.UtcNow;
            for (int i = 0; i < 600; i++)
            {
                Vector3 v = pos.Around(minimalDistance, maximumDistance);
                if (i % 15 == 0) GameFiber.Yield();
                if (NativeFunction.Natives.xB61C8E878A4199CA<bool>(v.X, v.Y, v.Z, true, out Vector3 nodeP, 0))
                {
                    if (nodeP.DistanceTo(pos) < minimalDistance || nodeP.DistanceTo(pos) > maximumDistance) continue;
                    if (nodeP.TravelDistanceTo(pos) < maximumDistance * 3f)
                    {
                        SpawnPoint ret = new SpawnPoint(nodeP, Extension.GetRandomAbsoluteSingle(1, 359));
                        $"Ped sidewalk spawn found {ret} Distance: {ret.DistanceTo(pos)}".ToLog();
                        $"{i} Process took {(DateTime.UtcNow - start).TotalMilliseconds} ms".ToLog();
                        return ret;
                    }
                }
            }
            for (int i = 0; i < 600; i++)
            {
                Vector3 v = pos.Around(minimalDistance, maximumDistance + Extension.GetRandomAbsoluteSingle(1f, 5f));
                if (i % 30 == 0) GameFiber.Yield();
                if (NativeFunction.Natives.xB61C8E878A4199CA<bool>(v.X, v.Y, v.Z, false, out Vector3 nodeP, 0))
                {
                    if (nodeP.DistanceTo(pos) < minimalDistance || nodeP.DistanceTo(pos) > maximumDistance) continue;
                    if (nodeP.TravelDistanceTo(pos) < maximumDistance * 3f)
                    {
                        SpawnPoint ret = new SpawnPoint(nodeP, Extension.GetRandomAbsoluteSingle(1, 359));
                        $"Ped safe spawn found {ret} Distance: {ret.DistanceTo(pos)}".ToLog();
                        $"{i + 600} Process took {(DateTime.UtcNow - start).TotalMilliseconds} ms".ToLog();
                        return ret;
                    }
                }
            }
            $"Safe coord not found".ToLog();
            $"1200 process took {(DateTime.UtcNow - start).TotalMilliseconds} ms".ToLog();
            return SpawnPoint.Zero;
        }
        internal static SpawnPoint GetRoadSideSpawnPoint(Vector3 pos, float? heading = null)
        {
            DateTime start = DateTime.UtcNow;
            if (heading.HasValue)
            {
                for (int i = 0; i < 600; i++)
                {
                    Vector3 v = pos.Around(Peralatan.Random.Next(5));
                    if (NativeFunction.Natives.xA0F8A7517A273C05<bool>(v.X, v.Y, v.Z, heading.Value, out Vector3 rsPos)) //_GET_ROAD_SIDE_POINT_WITH_HEADING
                    {
                        if (NativeFunction.Natives.xFF071FB798B803B0<bool>(rsPos.X, rsPos.Y, rsPos.Z, out Vector3 _, out float nodeHeading, 5, 3.0f, 0))
                        {
                            if (rsPos.DistanceTo(pos) < 100f && !rsPos.IsOccupied())
                            {
                                SpawnPoint ret = new SpawnPoint(rsPos, nodeHeading);
                                $"RoadSide with heading found {ret}".ToLog();
                                $"{i} process took {(DateTime.UtcNow - start).TotalMilliseconds:0.00} ms".ToLog();
                                return ret;
                            }
                        }
                    }
                    if (i % 30 == 0) GameFiber.Yield();
                }
            }
            else
            {
                for (int i = 0; i < 600; i++)
                {
                    Vector3 v = pos.Around(Peralatan.Random.Next(5, 15), Peralatan.Random.Next(20, 35));
                    if (NativeFunction.Natives.xFF071FB798B803B0<bool>(v.X, v.Y, v.Z, out Vector3 nodePos, out float nodeHeading, 5, 3.0f, 0))
                    {
                        if (NativeFunction.Natives.xA0F8A7517A273C05<bool>(nodePos.X, nodePos.Y, nodePos.Z, nodeHeading, out Vector3 rsPos)) //_GET_ROAD_SIDE_POINT_WITH_HEADING
                        {
                            if (rsPos.DistanceTo(pos) < 100f && !rsPos.IsOccupied())
                            {
                                SpawnPoint ret = new SpawnPoint(rsPos, nodeHeading);
                                $"RoadSide with heading found {ret}".ToLog();
                                $"{i} process took {(DateTime.UtcNow - start).TotalMilliseconds:0.00} ms".ToLog();
                                return ret;
                            }
                        }
                    }
                    if (i % 35 == 0) GameFiber.Yield();
                }
            }
            for (int i = 0; i < 600; i++)
            {
                Vector3 v = pos.Around(Peralatan.Random.Next(25));
                if (NativeFunction.Natives.x16F46FB18C8009E4<bool>(v.X, v.Y, v.Z, 0, out Vector3 roadSide)) //_GET_POINT_ON_ROAD_SIDE
                {
                    if (NativeFunction.Natives.xFF071FB798B803B0<bool>(roadSide.X, roadSide.Y, roadSide.Z, out Vector3 nodePos, out float nodeHeading, 12, 0x40400000, 0))
                    {
                        if (nodePos.DistanceTo(roadSide) < 50f && roadSide.DistanceTo(pos) < 100f && !roadSide.IsOccupied())
                        {
                            SpawnPoint ret = new SpawnPoint(roadSide, nodeHeading);
                            $"RoadSide found without heading {ret}".ToLog();
                            $"{i} process took {(DateTime.UtcNow - start).TotalMilliseconds:0.00} ms".ToLog();
                            return ret;
                        }
                    }
                }
                if (i % 40 == 0) GameFiber.Yield();
            }
            $"RoadSide position not found".ToLog();
            $"1200 process took {(DateTime.UtcNow - start).TotalMilliseconds} ms".ToLog();
            return SpawnPoint.Zero;
        }
    }
}