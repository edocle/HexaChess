
using System.Collections.Generic;
using UnityEngine;

namespace edocle.tools
{
    public class MeshGeneration
    {
        public static void GenerateMesh(ref Mesh mesh, MeshGenData data)
        {
            List<MeshTriangle> triangles = new List<MeshTriangle>();
            foreach(MeshSurface surface in data.Surfaces)
            {
                triangles.AddRange(surface.GetTriangles());
            }
            GenerateMesh(ref mesh, data.Vertices, triangles);
        }

        public static void GenerateMesh(ref Mesh mesh, List<Vector3> vertices, List<MeshTriangle> triangles)
        {
            mesh.vertices = vertices.ToArray();
            int[] triangleIndices = new int[triangles.Count * 3];
            for (int i = 0; i < triangles.Count; i++)
            {
                triangleIndices[i * 3] = triangles[i].t1;
                triangleIndices[i * 3 + 1] = triangles[i].t2;
                triangleIndices[i * 3 + 2] = triangles[i].t3;
                Debug.Log($"{i}> Triangle: {i * 3} {i * 3 + 1} {i * 3 + 2} > {triangles[i].t1} {triangles[i].t2} {triangles[i].t3}");
            }
            mesh.triangles = triangleIndices;
        }
    }
}