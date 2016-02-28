using UnityEngine;
using System.Collections;
using UnityEditor;

public class MeshMaker
{
    /// <summary>
    /// Creates a mesh of a unit circle.
    /// </summary>
    /// <param name="verticesCount"></param>
    /// <returns></returns>
    public static Mesh MakeCircle(int verticesCount)
    {
        Vector2[] vertices2d = new Vector2[verticesCount];
        Vector3[] vertices3d = new Vector3[verticesCount];
        Vector3[] uvs = new Vector3[verticesCount];

        vertices3d[0] = Vector3.zero;
        uvs[0] = new Vector2(0.5f, 0.5f);

        float angle = 360f / (verticesCount - 1);

        // Create vertices and UVs
        for (int i = 0; i < verticesCount; i++) {
            vertices3d[i] = Quaternion.AngleAxis(angle * (i - 1), Vector3.back) * Vector3.up;
            uvs[i] = vertices3d[i].normalized;
            vertices2d[i] = vertices3d[i];
        }

        Triangulator tri = new Triangulator(vertices2d);
        int[] indices = tri.Triangulate();

        Mesh mesh = new Mesh();
        mesh.name = "Circle";

        mesh.vertices = vertices3d;
        mesh.normals = uvs;
        mesh.triangles = indices;
        mesh.RecalculateBounds();
        mesh.Optimize();

        return mesh;
    }
}
