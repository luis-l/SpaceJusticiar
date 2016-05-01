using UnityEngine;
using System.Collections;
using UnityEditor;

public class MeshMaker
{
    /// <summary>
    /// Creates a mesh of a unit circle.
    /// </summary>
    /// <param name="segmentCount"></param>
    /// <returns></returns>
    public static Mesh MakeCircle(int verticesCount)
    {
        Vector2[] vertices2d = new Vector2[verticesCount];
        Vector3[] vertices3d = new Vector3[verticesCount];
        Vector2[] uvs = new Vector2[verticesCount];

        int[] tris = new int[verticesCount * 3];

        vertices3d[0] = Vector3.zero;
        vertices2d[0] = Vector2.zero;
        uvs[0] = new Vector2(0.5f, 0.5f);

        float angle = 360f / (verticesCount - 1);

        // Create vertices and UVs
        for (int i = 1; i < verticesCount; i++) {
            vertices3d[i] = Quaternion.AngleAxis(angle * (i - 1), Vector3.back) * Vector3.up;

            // There is really no point in having normals for a flat circle, but this is the calculation.
            //normals[i] = vertices3d[i].normalized;

            float texX = (vertices3d[i].x + 1.0f) * 0.5f;
            float texY = (vertices3d[i].y + 1.0f) * 0.5f;
            uvs[i] = new Vector2(texX, texY);

            vertices2d[i] = vertices3d[i];
        }

        for (int i = 0; i + 2 < verticesCount; ++i) {
            int index = i * 3;
            tris[index + 0] = 0;
            tris[index + 1] = i + 1;
            tris[index + 2] = i + 2;
        }

        // The last triangle has to wrap around to the first vert so we do this last and outside the lop  
        int lastTriangleIndex = tris.Length - 3;
        tris[lastTriangleIndex + 0] = 0;
        tris[lastTriangleIndex + 1] = verticesCount - 1;
        tris[lastTriangleIndex + 2] = 1;

        Mesh mesh = new Mesh();
        mesh.name = "Circle";

        mesh.vertices = vertices3d;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateBounds();
        mesh.Optimize();

        return mesh;
    }
    /*
    public static Mesh MakePlanetSurface(int verticesCount)
    {
        Vector2[] vertices2d = new Vector2[verticesCount];
        Vector3[] vertices3d = new Vector3[verticesCount];
        Vector2[] uvs = new Vector2[verticesCount];

        int[] tris = new int[verticesCount * 3];

        vertices3d[0] = Vector3.zero;
        vertices2d[0] = Vector2.zero;
        uvs[0] = new Vector2(0.5f, 0.5f);

        float angle = 360f / (verticesCount - 1);

        // Create vertices and UVs
        for (int i = 1; i < verticesCount; i++) {
            vertices3d[i] = Quaternion.AngleAxis(angle * (i - 1), Vector3.back) * Vector3.up;

            // There is really no point in having normals for a flat circle, but this is the calculation.
            //normals[i] = vertices3d[i].normalized;

            float texX = (vertices3d[i].x + 1.0f) * 0.5f;
            float texY = (vertices3d[i].y + 1.0f) * 0.5f;
            uvs[i] = new Vector2(texX, texY);

            vertices2d[i] = vertices3d[i];
        }

        for (int i = 0; i + 2 < verticesCount; ++i) {
            int index = i * 3;
            tris[index + 0] = 0;
            tris[index + 1] = i + 1;
            tris[index + 2] = i + 2;
        }

        // The last triangle has to wrap around to the first vert so we do this last and outside the lop  
        int lastTriangleIndex = tris.Length - 3;
        tris[lastTriangleIndex + 0] = 0;
        tris[lastTriangleIndex + 1] = verticesCount - 1;
        tris[lastTriangleIndex + 2] = 1;

        Mesh mesh = new Mesh();
        mesh.name = "Circle";

        mesh.vertices = vertices3d;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateBounds();
        mesh.Optimize();

        return mesh;
    } */
}
