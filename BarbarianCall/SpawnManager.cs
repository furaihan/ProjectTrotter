using BarbarianCall.Types;
using Rage;

namespace BarbarianCall
{
    internal static class SpawnManager
    {
        /// <summary>
        /// Gets a vehicle spawnpoint based on the provided spatial object, minimal distance, maximum distance, and direction consideration.
        /// </summary>
        /// <param name="spatial">The spatial object.</param>
        /// <param name="minimalDistance">The minimal distance from the spatial object.</param>
        /// <param name="maximumDistance">The maximum distance from the spatial object.</param>
        /// <param name="considerDirection">Whether to consider the direction of the spatial object.</param>
        /// <returns>The vehicle spawnpoint.</returns>
        internal static Spawnpoint GetVehicleSpawnPoint(ISpatial spatial, float minimalDistance, float maximumDistance, bool considerDirection = false)
        {
            return SpawnpointUtils.GetVehicleSpawnPoint(spatial.Position, minimalDistance, maximumDistance, considerDirection);
        }
        internal static Spawnpoint GetVehicleSpawnPoint(Vector3 position, float minimalDistance, float maximumDistance, bool considerDirection = false)
        {
            return SpawnpointUtils.GetVehicleSpawnPoint(position, minimalDistance, maximumDistance, considerDirection);
        }

        /// <summary>
        /// Gets a pedestrian spawnpoint based on the provided position, minimal distance, and maximum distance.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="minimalDistance">The minimal distance from the position.</param>
        /// <param name="maximumDistance">The maximum distance from the position.</param>
        /// <returns>The pedestrian spawnpoint.</returns>
        internal static Spawnpoint GetPedSpawnPoint(Vector3 pos, float minimalDistance, float maximumDistance)
        {
            return SpawnpointUtils.GetPedSpawnPoint(pos, minimalDistance, maximumDistance);
        }
        internal static Spawnpoint GetPedSpawnPoint(ISpatial spatial, float minimalDistance, float maximumDistance)
        {
            return SpawnpointUtils.GetPedSpawnPoint(spatial.Position, minimalDistance, maximumDistance);
        }

        /// <summary>
        /// Gets a spawnpoint favoring a specific direction relative to an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="favoredDirectionDistance">The favored direction distance.</param>
        /// <returns>The spawnpoint favoring the specified direction.</returns>
        internal static Spawnpoint GetSpawnPointFavoredDirection(Entity entity, float favoredDirectionDistance)
        {
            return SpawnpointUtils.GetSpawnPointFavoringDirection(entity, favoredDirectionDistance);
        }

        /// <summary>
        /// Gets a roadside spawnpoint based on the provided entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The roadside spawnpoint.</returns>
        internal static Spawnpoint GetRoadSideSpawnPoint(Entity entity)
        {
            return SpawnpointUtils.GetRoadSideSpawnPoint(entity);
        }

        /// <summary>
        /// Gets a roadside spawnpoint based on the provided spatial object and an optional heading.
        /// </summary>
        /// <param name="spatial">The spatial object.</param>
        /// <param name="heading">The optional heading.</param>
        /// <returns>The roadside spawnpoint.</returns>
        internal static Spawnpoint GetRoadSideSpawnPoint(ISpatial spatial, float? heading = null)
        {
            return SpawnpointUtils.GetRoadSideSpawnPoint(spatial.Position, heading);
        }
        internal static Spawnpoint GetRoadSideSpawnPoint(Vector3 position, float? heading = null)
        {
            return SpawnpointUtils.GetRoadSideSpawnPoint(position, heading);
        }

        /// <summary>
        /// Gets a spawnpoint on a slow road based on the provided position, minimum distance, and maximum distance.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="minimumDistance">The minimum distance from the position.</param>
        /// <param name="maximumDistance">The maximum distance from the position.</param>
        /// <returns>The slow road spawnpoint.</returns>
        internal static Spawnpoint GetSlowRoadSpawnPoint(Vector3 pos, float minimumDistance, float maximumDistance)
        {
            return SpawnpointUtils.GetSlowRoadSpawnPoint(pos, minimumDistance, maximumDistance);
        }

        /// <summary>
        /// Gets a solicitation spawnpoint, along with a node position and a roadside position.
        /// </summary>
        /// <param name="playerPos">The player's position.</param>
        /// <param name="nodePosition">The node position.</param>
        /// <param name="roadSidePosition">The roadside position.</param>
        /// <returns>The solicitation spawnpoint.</returns>
        internal static Spawnpoint GetSolicitationSpawnpoint(Vector3 playerPos, out Spawnpoint nodePosition, out Spawnpoint roadSidePosition)
        {
            return SpawnpointUtils.GetSolicitationSpawnpoint(playerPos, out nodePosition, out roadSidePosition);
        }

        /// <summary>
        /// Gets a random fleeing point based on a fleeing pedestrian.
        /// </summary>
        /// <param name="fleeingPed">The fleeing pedestrian.</param>
        /// <returns>The random fleeing point.</returns>
        internal static Spawnpoint GetRandomFleeingPoint(Ped fleeingPed)
        {
            return SpawnpointUtils.GetRandomFleeingPoint(fleeingPed);
        }
    }
}