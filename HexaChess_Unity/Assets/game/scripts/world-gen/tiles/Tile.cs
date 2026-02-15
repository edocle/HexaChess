
//#define DEBUG_TILE
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace hexaChess.worldGen
{
    public class Tile
    {
        // tile ids (x, y)
        public int m_CoordX { get; private set; }
        public int m_CoordY { get; private set; }
        public int AdjacentValue => m_CoordX + m_CoordY;

        // tile radius (physical size)
        public float m_Radius { get; private set; }

        // tile coordinate position (calculated)
        // converted tile coords to tile position
        // Because it's an hexagon tile, there is a x & z position offset per y coord
        public Vector2 m_CoordPos { get; private set; }

        // tile world position (calculated)
        // converted tile position to world position (using radius)
        public Vector2 m_WorldPos { get; private set; }
        public Vector3 WorldPos3D => new Vector3(m_WorldPos.x, m_WorldPosZ, m_WorldPos.y);

        // tile distance to origin (calculated)
        // converted tile coord pos to: tile distance to origin
        public float m_CoordPosDistanceToOrigin { get; private set; }

        // tile height (calculated)
        public float m_CoordPosZ { get; private set; }
        public float m_WorldPosZ { get; private set; }

        public Tile(int coordX, int coordY, float radius, float initialHeight = 0f)
        {
            m_CoordX = coordX;
            m_CoordY = coordY;
            m_Radius = radius;

            ComputeDatas(initialHeight);
        }

        public void ComputeDatas(float initialHeight)
        {
            // coord pos
            float coordPosX, coordPosY;
            coordPosX = m_CoordX + ((float)m_CoordY / 2);
            float distanceY = Mathf.Abs(m_CoordY);
            coordPosY = Mathf.Sqrt(Mathf.Pow(distanceY, 2) - Mathf.Pow(distanceY / 2, 2)) * (m_CoordY > 0 ? 1 : -1);
            m_CoordPos = new Vector2(coordPosX, coordPosY);

            // world pos
            m_WorldPos = new Vector2(m_Radius * coordPosX, m_Radius * coordPosY);

            // coord distance to origin
            m_CoordPosDistanceToOrigin = Mathf.Sqrt(Mathf.Pow(coordPosY, 2) + Mathf.Pow(coordPosX, 2));

            // Height
            RefreshHeight(initialHeight);
        }

        #region Relief

        public void RefreshHeight(float posZ)
        {
            m_CoordPosZ = posZ;
            m_WorldPosZ = m_Radius * m_CoordPosZ;
        }

        #endregion Relief


        #region Adjacents

        public Tile[] m_AdjacentTiles { get; private set; }

        public void TryRegisterAdjacents(Tile[] allTiles)
        {
            var adjacents = new List<Tile>();
            foreach (var tile in allTiles)
            {
                if (IsAdjacent(tile))
                {
                    adjacents.Add(tile);
                }
            }

#if DEBUG_TILE
            Debug.Log($"[{m_CoordX};{m_CoordY}] Add adjacents: {adjacents.Count} ({string.Join(", ", adjacents.Select(n => n.m_CoordX + ";" + n.m_CoordY))})");
#endif
            m_AdjacentTiles = adjacents.ToArray();
        }

        bool IsAdjacent(Tile other)
        {
            if (Mathf.Abs(AdjacentValue - other.AdjacentValue) > 1)
                return false; // distance too far

            if (Mathf.Abs(m_CoordX - other.m_CoordX) > 1)
                return false; // x too far

            if (Mathf.Abs(m_CoordY - other.m_CoordY) > 1)
                return false; // y too far

            if (m_CoordX == other.m_CoordX && m_CoordY == other.m_CoordY)
                return false; // same tile

            return true;
        }

        #endregion Adjacents

        #region Meshpoints

        public Vector3[] GetPoints(bool includeTilePosition = false, Vector3 offsetPos = new Vector3())
        {
            // Identify all adjacents
            // May need to register them for other usages, later
            Tile xForward_yBackward = System.Array.Find(m_AdjacentTiles, t => t.m_CoordX == m_CoordX + 1 && t.m_CoordY == m_CoordY - 1);
            Tile xForward_yNeutral = System.Array.Find(m_AdjacentTiles, t => t.m_CoordX == m_CoordX + 1 && t.m_CoordY == m_CoordY);
            Tile xNeutral_yBackward = System.Array.Find(m_AdjacentTiles, t => t.m_CoordX == m_CoordX && t.m_CoordY == m_CoordY - 1);
            Tile xNeutral_yForward = System.Array.Find(m_AdjacentTiles, t => t.m_CoordX == m_CoordX && t.m_CoordY == m_CoordY + 1);
            Tile xBackward_yNeutral = System.Array.Find(m_AdjacentTiles, t => t.m_CoordX == m_CoordX - 1 && t.m_CoordY == m_CoordY);
            Tile xBackward_yForward = System.Array.Find(m_AdjacentTiles, t => t.m_CoordX == m_CoordX - 1 && t.m_CoordY == m_CoordY + 1);

            // if one adjacent tile is null, it's a border tile, no need to compute mesh points
            if (xForward_yBackward == null ||
                xForward_yNeutral == null ||
                xNeutral_yBackward == null ||
                xNeutral_yForward == null ||
                xBackward_yNeutral == null ||
                xBackward_yForward == null)
                return null;

            Vector3 worldPos3D = WorldPos3D;
            // Compute vertices
            // points to obtain: center between target tile & 2 adjacent tiles (to triangulate its position)
            Vector3 localPosition0 = (worldPos3D + xForward_yNeutral.WorldPos3D + xNeutral_yForward.WorldPos3D) / 3 - (includeTilePosition ? new Vector3() : worldPos3D);
            Vector3 localPosition1 = (worldPos3D + xNeutral_yForward.WorldPos3D + xBackward_yForward.WorldPos3D) / 3 - (includeTilePosition ? new Vector3() : worldPos3D);
            Vector3 localPosition2 = (worldPos3D + xBackward_yForward.WorldPos3D + xBackward_yNeutral.WorldPos3D) / 3 - (includeTilePosition ? new Vector3() : worldPos3D);
            Vector3 localPosition3 = (worldPos3D + xBackward_yNeutral.WorldPos3D + xNeutral_yBackward.WorldPos3D) / 3 - (includeTilePosition ? new Vector3() : worldPos3D);
            Vector3 localPosition4 = (worldPos3D + xNeutral_yBackward.WorldPos3D + xForward_yBackward.WorldPos3D) / 3 - (includeTilePosition ? new Vector3() : worldPos3D);
            Vector3 localPosition5 = (worldPos3D + xForward_yBackward.WorldPos3D + xForward_yNeutral.WorldPos3D) / 3 - (includeTilePosition ? new Vector3() : worldPos3D);

            Vector3[] vertices = new Vector3[]
            {
                localPosition0 + offsetPos,
                localPosition1 + offsetPos,
                localPosition2 + offsetPos,
                localPosition3 + offsetPos,
                localPosition4 + offsetPos,
                localPosition5 + offsetPos,
            };

            return vertices;
        }

        public int[] DrawTriangles(Vector3[] points, int offset = 0)
        {
            int[] newTriangles = new int[]
            {
                // Triangle bordering xNeutral_yForward && xBackward_yForward
                0 + offset,
                2 + offset,
                1 + offset,
                
                // Triangle bordering xBackward_yNeutral
                0 + offset,
                3 + offset,
                2 + offset,

                // Triangle bordering xForward_yNeutral
                0 + offset,
                5 + offset,
                3 + offset,

                // Triangle bordering xForward_yBackward && xNeutral_yBackward
                5 + offset,
                4 + offset,
                3 + offset,
            };

            return newTriangles;
        }

        #endregion Meshpoints

        #region Operators

        public override bool Equals(object obj)
        {
            var other = obj as Tile;

            if (other == null)
            {
#if DEBUG_TILE
            Debug.Log($"[{m_CoordX};{m_CoordY}] false: not same type");
#endif
                return false;
            }

            if (m_CoordX == other.m_CoordX && m_CoordY == other.m_CoordY)
            {
#if DEBUG_TILE
            Debug.Log($"[{m_CoordX};{m_CoordY}] true: coords are the same ({other.m_CoordX};{other.m_CoordY})");
#endif
                return true;
            }
            else
            {
#if DEBUG_TILE
            Debug.Log($"[{m_CoordX};{m_CoordY}] false: coords are not the same ({other.m_CoordX};{other.m_CoordY})");
#endif
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion Operators
    }
}