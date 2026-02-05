
//#define DEBUG_TILE_BATCH_GAMEOBJECT
using System.Collections.Generic;
using UnityEngine;

namespace hexaChess.worldGen
{
    public class ChunkGameObject : MonoBehaviour
    {
        private MeshRenderer m_MeshRenderer = null;
        private MeshFilter m_MeshFilter = null;
        private MeshCollider m_MeshCollider = null;

        public Chunk m_Chunk { get; private set; }

        public void Init(Material material, Chunk tileBatch)
        {
            m_Chunk = tileBatch;
            Mesh mesh = new Mesh();
            // Setup components
            m_MeshRenderer = gameObject.AddComponent<MeshRenderer>();
            m_MeshRenderer.material = material;
            m_MeshFilter = gameObject.AddComponent<MeshFilter>();
            m_MeshFilter.mesh = mesh;
            m_MeshCollider = gameObject.AddComponent<MeshCollider>();
            m_MeshCollider.sharedMesh = mesh;
        }

        public void RefreshMesh()
        {
            if (m_Chunk == null)
                return;

            // Accumulate all tile points and triangles
            List<Vector3> accumulatedPolygonPoints = new List<Vector3>();
            List<int> accumulatedTriangles = new List<int>();

            var tiles = m_Chunk.GetTiles();
            foreach (Tile tile in tiles)
            {
                Vector3[] polygonPoints = tile.GetPoints(true, -gameObject.transform.localPosition);

                if (polygonPoints == null || polygonPoints.Length == 0)
                    continue;

                int[] triangles = tile.DrawTriangles(polygonPoints, accumulatedPolygonPoints.Count);
                accumulatedPolygonPoints.AddRange(polygonPoints);
                accumulatedTriangles.AddRange(triangles);
            }

            // Generate mesh
            m_MeshFilter.mesh.Clear();
            m_MeshFilter.mesh.vertices = accumulatedPolygonPoints.ToArray();
            m_MeshFilter.mesh.triangles = accumulatedTriangles.ToArray();

            m_MeshFilter.mesh.RecalculateNormals();
            m_MeshCollider.sharedMesh = m_MeshFilter.mesh;

#if DEBUG_TILE_BATCH_GAMEOBJECT
            Debug.Log($"<color=blue>IslandGenerator></color> Generated batch mesh with {accumulatedPolygonPoints.Count} vertices and {accumulatedTriangles.Count / 3} triangles");
            Debug.Log($"<color=blue>IslandGenerator</color> Polygons list: {string.Join(", ", accumulatedPolygonPoints)}");
            Debug.Log($"<color=blue>IslandGenerator</color> Triangles list: {string.Join(", ", accumulatedTriangles)}");
#endif
        }
    }
}
