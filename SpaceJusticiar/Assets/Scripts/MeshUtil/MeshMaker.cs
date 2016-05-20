using UnityEngine;
using System.Collections;
using UnityEditor;
using LibNoise.Generator;
using LibNoise.Operator;

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

    public static Mesh MakePlanetSurface(int verticesCount, int seed = 0, float scalar = 1, float offset = 0)
    {
        Billow baseFlatTerrainNoise = new Billow();
        baseFlatTerrainNoise.Seed = seed;
        baseFlatTerrainNoise.Frequency = 0.5;

        ScaleBias flatTerrain = new ScaleBias(baseFlatTerrainNoise);
        flatTerrain.Scale = 0.08;
        flatTerrain.Bias = -0.7;

        RidgedMultifractal mountainNoise = new RidgedMultifractal();
        mountainNoise.Seed = seed;
        mountainNoise.Frequency = 0.15f;
        mountainNoise.OctaveCount = 5;

        Perlin terrainType = new Perlin();
        terrainType.Seed = seed;
        terrainType.Frequency = 0.5;
        terrainType.Persistence = 0.5;
        terrainType.OctaveCount = 5;

        Select finalTerrain = new Select(flatTerrain, mountainNoise, terrainType);
        finalTerrain.SetBounds(0.0, 1);
        finalTerrain.FallOff = 0.4;

        float noiseScale = 0.1f;

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

            // Modify the vertex with noise
            float x = vertices3d[i].x / noiseScale;
            float y = vertices3d[i].y / noiseScale;

            float value = scalar * (float)finalTerrain.GetValue(x + offset, y + offset, 0);

            value = (1 + value) / 2f;
            vertices3d[i] *= (1 + value * 0.1f);

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
        mesh.name = "PlanetSurface";

        mesh.vertices = vertices3d;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateBounds();
        mesh.Optimize();

        return mesh;
    }
}
