
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace hexaChess.worldGen
{
    /// <summary>
    /// Made to batch a bunch of tiles together to reduce drawcalls.
    /// Handles neighbouring adjacent tiles together
    /// Handles neighbouring adjacent chunks together
    /// LOD ? Later
    /// </summary>
    public class Chunk
    {
        int m_Range = 8;
        float m_TileRadius = 1f;

        public Chunk(int coordX, int coordY, int range, float tileRadius)
        {
            m_CoordX = coordX;
            m_CoordY = coordY;
            m_Range = range;
            m_TileRadius = tileRadius;
        }

        #region Coordinates

        public int m_CoordX { get; private set; }
        public int m_CoordY { get; private set; }

        /// <summary>
        /// Simplified "IsEqual"
        /// Cannot generate 2 chunks that have the same coordinate
        /// </summary>
        /// <param name="coordX"></param>
        /// <param name="coordY"></param>
        /// <returns></returns>
        public bool HasSameCoordinates(int coordX, int coordY)
        {
            return m_CoordX == coordX && m_CoordY == coordY;
        }

        #endregion Coordinates

        #region Origin tile

        /// <summary>
        /// the tile on center of the chunk
        /// Is defined by the chunk coordinate itself
        /// In order to get it, need to generate all tiles first
        /// </summary>
        private Tile m_OriginTile = null;
        public Tile GetOriginTile()
        {
            if (m_OriginTile == null)
                m_OriginTile = m_Tiles.FirstOrDefault(f => f.m_CoordX == m_CoordX && f.m_CoordY == m_CoordY);

            return m_OriginTile;
        }

        #endregion Origin tile

        #region Tiles

        /// <summary>
        /// All tiles in chunk
        /// </summary>
        private Tile[] m_Tiles = null;

        public int GetTilesCount()
        {
            if (m_Tiles == null)
                return 0;
            return m_Tiles.Length;
        }

        public Tile[] GetTiles()
        { return m_Tiles; }

        public void GenerateTiles()
        {
            // Counter, if we need to pace the generation
            int count = 0;

            int start = -m_Range + 1;
            int end = m_Range - 1;
            int diameter = (m_Range * 2);
            List<Tile> tiles = new List<Tile>(diameter * diameter);

            // Loop all tiles in a specific range
            for (int i = start; i <= end; i++)
            {
                for (int j = start; j <= end; j++)
                {
                    // by checking max range, we shape the chunk as a big hexagon
                    if (Mathf.Abs(i + j) >= m_Range)
                    {
                        // Debug.Log($"i: {i} / j: {j} / total: {Mathf.Abs(i + j)} vs. radius: {m_Range}");
                        continue;
                    }
                    tiles.Add(GenerateTile(i + m_CoordX, j + m_CoordY));
                    count++;
                }
            }

            m_Tiles = tiles.ToArray();
        }

        Tile GenerateTile(int coordX, int coordY)
        {
            Tile tile = new Tile(coordX, coordY, m_TileRadius);
            return tile;
        }

        /// <summary>
        /// For each tile, we link adjacent tiles
        /// Will be useful for systems like path finding
        /// </summary>
        /// <param name="parameters"></param>
        public void LinkTiles(IslandGeneratorParameters parameters)
        {
            if (m_Tiles == null || m_Tiles.Length == 0)
                return;

            // setup neighbours list
            Tile[] neighboursArray = new List<Tile>(m_Tiles)
                .Concat(m_Neighbours.Where(n => n?.m_Tiles != null)
                .SelectMany(n => n.m_Tiles))
                .ToArray();

            foreach (Tile tile in m_Tiles)
            {
                tile.TryRegisterAdjacents(neighboursArray);
            }
        }

        #endregion Tiles

        #region Neighbours

        /// <summary>
        /// All adjacent chunks
        /// </summary>
        private List<Chunk> m_Neighbours = new List<Chunk>();

        public void TryAddNeighbour(Chunk neighbour)
        {
            if (neighbour == null)
                return;

            if (m_Neighbours.Contains(neighbour))
                return;

            m_Neighbours.Add(neighbour);
        }

        #endregion Neighbours
    }
}