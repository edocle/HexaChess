
using hexaChess.worldGen;
using System.Collections.Generic;

namespace hexaChess.tool
{
    public class PathFindManager
    {
        /// <summary>
        /// Basic find path method
        /// </summary>
        /// <param name="startTile"></param>
        /// <param name="endTile"></param>
        /// <returns></returns>
        public List<Tile> FindPath(Tile startTile, Tile endTile)
        {
            // Safeguard
            if (startTile == null || endTile == null)
                return new List<Tile>();

            // @todo Benchtest if ReferenceEquals is faster than Equals, as Tile.Equals is overriden therefore may be faster
            if (ReferenceEquals(startTile, endTile) || startTile == endTile)
                return new List<Tile> { startTile };

            // Queued tiles we need to check; As they are visited in the right order, that's always the shortest path
            // All the 1-length paths then all the 2-length paths, etc...
            var queue = new Queue<Tile>();
            // all tiles already visited, so we don't check them twice
            var visited = new HashSet<Tile>();
            // Every node generated with the shortest path, used to reconstruct the path once we found the end tile
            var cameFrom = new Dictionary<Tile, Tile>();

            queue.Enqueue(startTile);
            visited.Add(startTile);

            // Runs until queue is depleted
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                // @todo If we want to handle other movement rules or blocked tiles, need to develop here
                // Move like a knight ? use a "knight" class to calculate tiles
                // Rocks and walls ? Include rules that knows which tiles are unavailable
                var neighbors = current.m_AdjacentTiles;
                if (neighbors == null)
                {
                    continue;
                }

                // For each queued tile, check all neighbours that have not been visited yet
                foreach (var neighbor in neighbors)
                {
                    if (neighbor == null)
                        continue;

                    if (visited.Contains(neighbor))
                        continue;

                    cameFrom[neighbor] = current;

                    // @todo Benchtest if ReferenceEquals is faster than Equals, as Tile.Equals is overriden therefore may be faster
                    if (ReferenceEquals(neighbor, endTile) || neighbor == endTile)
                    {
                        var path = new List<Tile>();
                        var node = endTile;
                        // Go through "cameFrom" to retrieve path from end to start, then reverse it to have the right order
                        while (true)
                        {
                            path.Add(node);
                            if (!cameFrom.TryGetValue(node, out node))
                                break;
                        }
                        path.Reverse();
                        return path;
                    }

                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }

            return new List<Tile>();
        }
    }
}