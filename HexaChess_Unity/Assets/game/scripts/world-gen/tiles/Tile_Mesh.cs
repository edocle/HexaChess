
using UnityEngine;

namespace hexaChess.worldGen
{
    public class Tile_Mesh : TileGameObject
    {
        [SerializeField] MeshFilter m_MeshFilter = null;

        [SerializeField] Vector3 m_NormalSample;
        [SerializeField] Vector2 m_UvSample;

        Mesh m_Mesh;
        public Vector3[] m_PolygonPoints;
        public int[] m_PolygonTriangles;

        public override void GenerateMesh()
        {
            m_Mesh = new Mesh();
            m_MeshFilter.mesh = m_Mesh;

            m_PolygonPoints = m_Data.GetPoints();

            if (m_PolygonPoints == null)
                return;

            m_PolygonTriangles = m_Data.DrawTriangles(m_PolygonPoints);

            m_Mesh.Clear();
            m_Mesh.vertices = m_PolygonPoints;
            m_Mesh.triangles = m_PolygonTriangles;

            m_Mesh.RecalculateNormals();
        }
    }
}