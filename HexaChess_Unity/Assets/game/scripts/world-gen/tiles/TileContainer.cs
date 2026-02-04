
// #define DEBUG_TILE_CONTAINER_GENERATOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace hexaChess.worldGen
{
    public class TileContainer : MonoBehaviour
    {
        private MeshRenderer m_MeshRenderer = null;
        private MeshFilter m_MeshFilter = null;

        private Tile m_OriginTile = null;
        public Tile OriginTile => m_OriginTile;

        private Tile[] m_Tiles = null;

        public void Init(Material material)
        {
            // Setup components
            m_MeshRenderer = gameObject.AddComponent<MeshRenderer>();
            m_MeshFilter = gameObject.AddComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            m_MeshFilter.mesh = mesh;
            m_MeshRenderer.material = material;
        }

        public void SetupOriginTile(Tile tile)
        {
            m_OriginTile = tile;
        }

        public void UpdateMeshIndexFormat(IndexFormat format)
        {
            m_MeshFilter.mesh.indexFormat = format;
        }

        public void SetupTiles(Tile[] tiles)
        {
            m_Tiles = tiles;
        }

        public void RefreshMesh()
        {
            List<Vector3> accumulatedPolygonPoints = new List<Vector3>();
            List<int> accumulatedTriangles = new List<int>();

            foreach (Tile tile in m_Tiles)
            {
                Vector3[] polygonPoints = tile.GetPoints(true, -gameObject.transform.localPosition);

                if (polygonPoints == null || polygonPoints.Length == 0)
                    continue;

                int[] triangles = tile.DrawTriangles(polygonPoints, accumulatedPolygonPoints.Count);

                accumulatedPolygonPoints.AddRange(polygonPoints);
                accumulatedTriangles.AddRange(triangles);
            }

            m_MeshFilter.mesh.Clear();
            m_MeshFilter.mesh.vertices = accumulatedPolygonPoints.ToArray();
            m_MeshFilter.mesh.triangles = accumulatedTriangles.ToArray();

            m_MeshFilter.mesh.RecalculateNormals();

    #if DEBUG_TILE_CONTAINER_GENERATOR
            Debug.Log($"<color=blue>IslandGenerator></color> Generated island mesh with {accumulatedPolygonPoints.Count} vertices and {accumulatedTriangles.Count / 3} triangles");
            Debug.Log($"<color=blue>IslandGenerator</color> Polygons list: {string.Join(", ", accumulatedPolygonPoints)}");
            Debug.Log($"<color=blue>IslandGenerator</color> Triangles list: {string.Join(", ", accumulatedTriangles)}");
    #endif
        }
    }
}