
//#define DEBUG_TILE_BATCH_GAMEOBJECT
using System.Collections.Generic;
using UnityEngine;

namespace hexaChess.worldGen
{
    public class ChunkGameObject : MonoBehaviour
    {
        private MeshRenderer m_MeshRenderer = null;
        private MeshFilter m_MeshFilter = null;

        public Chunk m_Chunk { get; private set; }

        public void Init(Material material, Chunk tileBatch)
        {
            m_Chunk = tileBatch;
            // Setup components
            m_MeshRenderer = gameObject.AddComponent<MeshRenderer>();
            m_MeshFilter = gameObject.AddComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            m_MeshFilter.mesh = mesh;
            m_MeshRenderer.material = material;
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

#if DEBUG_TILE_BATCH_GAMEOBJECT
            Debug.Log($"<color=blue>IslandGenerator></color> Generated batch mesh with {accumulatedPolygonPoints.Count} vertices and {accumulatedTriangles.Count / 3} triangles");
            Debug.Log($"<color=blue>IslandGenerator</color> Polygons list: {string.Join(", ", accumulatedPolygonPoints)}");
            Debug.Log($"<color=blue>IslandGenerator</color> Triangles list: {string.Join(", ", accumulatedTriangles)}");
#endif
        }
    }
}
