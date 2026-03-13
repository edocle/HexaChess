
//#define DEBUG_TILE
using edocle.tools;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

        #region Vectors

        Vector3[] m_Edges;

        public Vector3[] Edges
        {
            get
            {
                if (m_Edges == null)
                    GenerateEdges();

                return m_Edges;
            }
        }

        public Vector3 Center
        {
            get; private set;
        }

        /// <summary>
        /// Generate position of tile edges
        /// I need them to be specifically named, because I want to know the order and direction of each point
        /// First, need neighbour tiles
        /// </summary>
        void GenerateEdges()
        {
            // Identify all adjacents
            // May need to register them for other usages, later
            Tile xForward_yBackward = System.Array.Find(m_AdjacentTiles, t => t.m_CoordX == m_CoordX + 1 && t.m_CoordY == m_CoordY - 1);
            Tile xForward_yNeutral = System.Array.Find(m_AdjacentTiles, t => t.m_CoordX == m_CoordX + 1 && t.m_CoordY == m_CoordY);
            Tile xNeutral_yBackward = System.Array.Find(m_AdjacentTiles, t => t.m_CoordX == m_CoordX && t.m_CoordY == m_CoordY - 1);
            Tile xNeutral_yForward = System.Array.Find(m_AdjacentTiles, t => t.m_CoordX == m_CoordX && t.m_CoordY == m_CoordY + 1);
            Tile xBackward_yNeutral = System.Array.Find(m_AdjacentTiles, t => t.m_CoordX == m_CoordX - 1 && t.m_CoordY == m_CoordY);
            Tile xBackward_yForward = System.Array.Find(m_AdjacentTiles, t => t.m_CoordX == m_CoordX - 1 && t.m_CoordY == m_CoordY + 1);

            // if one adjacent tile is null, it's a border tile, replacing it with self
            if (xForward_yBackward == null)
                xForward_yBackward = this;

            if (xForward_yNeutral == null)
                xForward_yNeutral = this;

            if (xNeutral_yBackward == null)
                xNeutral_yBackward = this;

            if (xNeutral_yForward == null)
                xNeutral_yForward = this;

            if (xBackward_yNeutral == null)
                xBackward_yNeutral = this;

            if (xBackward_yForward == null)
                xBackward_yForward = this;

            Vector3 worldPos3D = WorldPos3D;
            // Compute vertices
            // points to obtain: center between target tile & 2 adjacent tiles (to triangulate its position)
            Vector3 localPosition0 = (worldPos3D + xForward_yNeutral.WorldPos3D + xNeutral_yForward.WorldPos3D) / 3;
            Vector3 localPosition1 = (worldPos3D + xNeutral_yForward.WorldPos3D + xBackward_yForward.WorldPos3D) / 3;
            Vector3 localPosition2 = (worldPos3D + xBackward_yForward.WorldPos3D + xBackward_yNeutral.WorldPos3D) / 3;
            Vector3 localPosition3 = (worldPos3D + xBackward_yNeutral.WorldPos3D + xNeutral_yBackward.WorldPos3D) / 3;
            Vector3 localPosition4 = (worldPos3D + xNeutral_yBackward.WorldPos3D + xForward_yBackward.WorldPos3D) / 3;
            Vector3 localPosition5 = (worldPos3D + xForward_yBackward.WorldPos3D + xForward_yNeutral.WorldPos3D) / 3;

            m_Edges = new Vector3[]
            {
                (worldPos3D + xForward_yNeutral.WorldPos3D + xNeutral_yForward.WorldPos3D) / 3,
                (worldPos3D + xNeutral_yForward.WorldPos3D + xBackward_yForward.WorldPos3D) / 3,
                (worldPos3D + xBackward_yForward.WorldPos3D + xBackward_yNeutral.WorldPos3D) / 3,
                (worldPos3D + xBackward_yNeutral.WorldPos3D + xNeutral_yBackward.WorldPos3D) / 3,
                (worldPos3D + xNeutral_yBackward.WorldPos3D + xForward_yBackward.WorldPos3D) / 3,
                (worldPos3D + xForward_yBackward.WorldPos3D + xForward_yNeutral.WorldPos3D) / 3,
            };

            var realCenter = new Vector3();
            foreach (var edge in m_Edges)
            {
                realCenter += edge;
            }
            Center = realCenter / m_Edges.Length;
        }

        #region Tilt direction

        private Vector3 m_TiltDirection;
        bool m_TiltDirectionGenerated = false;
        public Vector3 TiltDirection
        {
            get
            {
                if (!m_TiltDirectionGenerated)
                    GenerateTiltDirection();

                return m_TiltDirection;
            }
        }

        void GenerateTiltDirection()
        {
            var edges = Edges;
            var center = Center;
            m_TiltDirectionGenerated = true;

            // @todo update tilt here so that it's average of all edges (may need at least 3 cross)
            // Order here is important, so that the tilt is upward and not backward
            Vector3 a = (edges[1] - center);
            Vector3 b = (edges[0] - center);
            var ab = Vector3.Cross(a, b);
            Vector3 c = (edges[3] - center);
            Vector3 d = (edges[2] - center);
            var cd = Vector3.Cross(c, d);
            Vector3 e = (edges[5] - center);
            Vector3 f = (edges[4] - center);
            var ef = Vector3.Cross(e, f);
            m_TiltDirection = (ab + cd + ef) / 3f;

            m_TiltDirection.Normalize();
        }

        #endregion Tilt direction

        #endregion Vectors

        #region Meshpoints

        public Vector3[] GetPoints(bool includeTilePosition = false, Vector3 offsetPos = new Vector3())
        {
            Vector3[] edges = Edges;
            Vector3 worldPos3D = WorldPos3D;
            return new Vector3[]
            {
                edges[0] - (includeTilePosition ? new Vector3() : worldPos3D) + offsetPos,
                edges[1] - (includeTilePosition ? new Vector3() : worldPos3D) + offsetPos,
                edges[2] - (includeTilePosition ? new Vector3() : worldPos3D) + offsetPos,
                edges[3] - (includeTilePosition ? new Vector3() : worldPos3D) + offsetPos,
                edges[4] - (includeTilePosition ? new Vector3() : worldPos3D) + offsetPos,
                edges[5] - (includeTilePosition ? new Vector3() : worldPos3D) + offsetPos,
            };
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

        /// <summary>
        /// Slots are sockets where we can place objects
        /// Every slot has a position, so that if we allow an object onto a slot, it knows its position
        /// We can have up to x side slots (3 to 6, not sure), so an entity can spawn into an empty side slot if there is any
        /// We can only have one main slot, that takes all the place and prevent any entity to spawn in a side slot
        /// Micro slots are only here to put additional small entities like grass or other things
        /// </summary>
        #region Slots

        private TileSlot m_MainSlot = null;

        private TileSlot[] m_SideSlots = null;

        // private TileSlot[] m_MicroSlots = null;

        public TileSlot MainSlot
        {
            get
            {
                if (m_MainSlot == null)
                    GenerateMainSlot();

                return m_MainSlot;
            }
        }

        void GenerateMainSlot()
        {
            m_MainSlot = new TileSlot(WorldPos3D, SlotType.main);
        }

        public TileSlot[] SideSlots
        {
            get
            {
                if (m_SideSlots == null)
                    GenerateSideSlots();

                return m_SideSlots;
            }
        }

        void GenerateSideSlots()
        {
            int sideSlots = 3;
            float degreesBetweenEachSide = 360f / sideSlots;
            float sideSlotsDistanceToCenter = m_Radius * 0.4f;
            Vector2 newDirection = Random.insideUnitCircle.normalized * sideSlotsDistanceToCenter;
            m_SideSlots = new TileSlot[sideSlots];

            for (int i = 0; i < sideSlots; i++)
            {
                m_SideSlots[i] = GenerateSideSlot(ref newDirection, degreesBetweenEachSide);
            }
        }

        TileSlot GenerateSideSlot(ref Vector2 previousDirection, float degrees)
        {
            // Everytime, we update previous direction, si that it can turn
            previousDirection = previousDirection.RotateUsingDegrees(degrees);

            // Vector from center of tile to side slot pos if tile was flat
            Vector3 slotFlatDirection = new Vector3(previousDirection.x, 0, previousDirection.y);

            // Vector from center taking into account the tilt
            // point is to "rotate" flat vector
            // smart way of doing it: cross using "up" to get perpendicular axis,
            // then use (-)perpendicular axis + tilted "up" direction to get rotated vector
            Vector3 firstCross = Vector3.Cross(slotFlatDirection, Vector3.up);
            Vector3 tiltedSlotVector = Vector3.Cross(-firstCross, TiltDirection);

            // Get position using world pos
            // may need local position AND world position one day, both are here
            Vector3 position3D = WorldPos3D + tiltedSlotVector;

            return new TileSlot(position3D, SlotType.side);
            // Debug.Log($"New side slot> Direction: {tiltedSlotVector} /Position: {position3D}");
        }

        #endregion Slots
    }
}