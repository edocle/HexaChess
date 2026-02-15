
using System;
using System.Collections.Generic;
using UnityEngine;

namespace edocle.tools
{
    [CreateAssetMenu(fileName = "MeshGenData", menuName = "edocle/tools/MeshGenData")]
    public class MeshGenData : ScriptableObject
    {
        [SerializeField] List<Vector3> m_Vertices;
        public List<Vector3> Vertices => m_Vertices;

        [SerializeField] List<MeshSurface> m_Surfaces;
        public List<MeshSurface> Surfaces => m_Surfaces;

        [SerializeField] List<MeshTriangle> m_Triangles;
        public List<MeshTriangle> Triangles => m_Triangles;
    }

    [Serializable]
    public class MeshSurface
    {
        public string m_Name;
        public List<int> m_Vertices;
        public List<MeshTriangle> m_Triangles;

        public List<MeshTriangle> GetTriangles()
        {
            List<MeshTriangle> triangles = new List<MeshTriangle>();

            int currentForwardIndex = 0;
            int currentBackwardIndex = m_Vertices.Count - 1;
            bool goingForward = true;

            while (currentForwardIndex + 1 < currentBackwardIndex)
            {
                MeshTriangle triangle = null;
                int[] points = new int[3];
                if (goingForward)
                {
                    points[0] = m_Vertices[currentForwardIndex];
                    points[1] = m_Vertices[currentForwardIndex + 1];
                    points[2] = m_Vertices[currentBackwardIndex];
                    currentBackwardIndex--;
                }
                else
                {
                    points[0] = m_Vertices[currentBackwardIndex + 1];
                    points[1] = m_Vertices[currentForwardIndex + 1];
                    points[2] = m_Vertices[currentBackwardIndex];
                    currentForwardIndex++;
                }

                triangle = new MeshTriangle
                {
                    name = String.Concat(m_Name, "_", currentForwardIndex, currentBackwardIndex),
                    t1 = points[0],
                    t2 = points[1],
                    t3 = points[2]
                };

                triangles.Add(triangle);
                goingForward = !goingForward;
            }

            m_Triangles = triangles;
            return triangles;
        }
    }

    [Serializable]
    public class MeshTriangle
    {
        public string name;
        public int t1;
        public int t2;
        public int t3;
    }
}