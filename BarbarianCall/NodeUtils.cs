using BarbarianCall.Extensions;
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Rage.Native.NativeFunction;

namespace BarbarianCall
{
    internal static class NodeUtils
    {
        internal static Vector3[] GetVehicleNodesAroundPosition(Vector3 pos, float minimalDistance, float maximumDistance)
        {
            List<Vector3> vehicleNodes = new();
            for (int i = 1; i <= 1000; i++)
            {
                Vector3 randomPosition = pos.AroundPosition(MyRandom.Next((int)minimalDistance, (int)maximumDistance)).ToGround();
                if (i % 40 == 0)
                {
                    GameFiber.Yield();
                }

                Vector3 desiredPosition = randomPosition + new Vector3(MathExtension.GetRandomFloatInRange(1.0f, 25.0f) * MyRandom.Next(2) == 1 ? 1f : -1f, MathExtension.GetRandomFloatInRange(1.0f, 25.0f) * MyRandom.Next(2) == 1 ? 1f : -1f, 0f);

                vehicleNodes.AddRange(GetVehicleNodesAtPosition(randomPosition, desiredPosition));
            }

            return vehicleNodes.ToArray();
        }
        internal static IEnumerable<Vector3> GetVehicleNodesAtPosition(Vector3 position, Vector3 desiredPosition)
        {
            Natives.GET_CLOSEST_VEHICLE_NODE<bool>(position, out Vector3 closestNodeVector, 1, 3.0f, 0.0f);
            Natives.GET_CLOSEST_MAJOR_VEHICLE_NODE<bool>(position, out Vector3 majorNodeVector, 3.0f, 0.0f);
            Natives.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING<bool>(position, out Vector3 closestNodeHeadingVector, out float _, 1, 3.0f, 0.0f);
            Natives.GET_NTH_CLOSEST_VEHICLE_NODE<bool>(position, 1 % 5, out Vector3 nthClosestNodeVector, 1, 3.0f, 0.0f);
            Natives.GET_NTH_CLOSEST_VEHICLE_NODE_WITH_HEADING<bool>(position, 1 % 5, out Vector3 nthClosestNodeHeadingVector, out float _, out int _, 1, 3.0f, 2.5f);
            Natives.GET_NTH_CLOSEST_VEHICLE_NODE_FAVOUR_DIRECTION<bool>(position, desiredPosition, 1 % 5, out Vector3 nthClosectNodeFavourDirectionVector, out float _, 1, 3.0f, 0);

            return new[] { closestNodeVector, majorNodeVector, closestNodeHeadingVector, nthClosestNodeVector, nthClosestNodeHeadingVector, nthClosectNodeFavourDirectionVector }.Where(x => x != Vector3.Zero);
        }
        internal static bool IsNodeIdValid(int nodeId)
        {
            return Natives.IS_VEHICLE_NODE_ID_VALID<bool>(nodeId);
        }

        internal static bool IsNodeSwitchedOff(int nodeId)
        {
            return Natives.GET_VEHICLE_NODE_IS_SWITCHED_OFF<bool>(nodeId);
        }

        internal static bool IsNodeSafe(Vector3 position)
        {
            int closestNodeId = Natives.GET_NTH_CLOSEST_VEHICLE_NODE_ID<int>(position.X, position.Y, position.Z, 1, 0, 3f, 0f);

            if (!IsNodeIdValid(closestNodeId))
                return false;

            if (IsNodeSwitchedOff(closestNodeId))
                return false;

            return IsGpsAllowedOnNode(closestNodeId);
        }

        internal static bool IsGpsAllowedOnNode(int nodeId)
        {
            return Natives.GET_VEHICLE_NODE_IS_GPS_ALLOWED<bool>(nodeId);
        }
        internal static bool IsOnScreen(this Vector3 pos)
        {
            return Natives.GET_HUD_SCREEN_POSITION_FROM_WORLD_POSITION<bool>(pos.X, pos.Y, pos.Z, out float _, out float _);
        }

        internal static bool IsSlowRoad(Vector3 position)
        {
            return Natives.GET_NTH_CLOSEST_VEHICLE_NODE_ID<bool>(position.X, position.Y, position.Z, 1, 5, 1077936128, 0f);
        }
    }
}
