using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEditor;

public class TileMeshBuilder
{
    //Returns how bix the tile is in world space
    static public Vector3 GetTileBounds()
    {
        float bound = Map.Instance().terrainSettings.tileSideLength * Map.Instance().terrainSettings.tileSquareSize;
        return new Vector3(bound, 0, bound);
    }

    //Mesh Data
    private static List<Vector3> l_vertices = new List<Vector3>();
    private static List<Vector3> l_normals = new List<Vector3>();
    private static List<int> l_triangles = new List<int>();
    private static List<Vector2> l_uvs = new List<Vector2>();

    private static Vector3 currentPos; //The current mesh position we are building
    private static Mesh currentMesh; //The current mesh we are building

    public static Mesh GenerateMesh(Vector3 position)
    {
        currentMesh = new Mesh();
        currentPos = position;
        BuildMesh();

        return currentMesh;
    }

    //Delete all the arrays
    public static void ClearMesh()
    {
        l_vertices.Clear();
        l_normals.Clear();
        l_triangles.Clear();
        l_uvs.Clear();

        currentMesh = null;
        currentPos = Vector3.zero;
    }

    /*
    void OnEnable()
    {
        gameObject.hideFlags = HideFlags.NotEditable | HideFlags.HideInHierarchy;
        gameObject.hideFlags = HideFlags.None;
        _meshRenderer = GetComponent<MeshRenderer>();
    }*/

    //Returns the shader parameter to decide which part of the control texture this tile should use
    private static Vector4 CalculateSplatUV(Vector3 position)
    {
        int splatSize = Map.Instance().heightTexture.width;

        //Pos Param is the chunk of the control UV that this particular tile should use
        Point splatCoords = Map.Instance().WorldToTextureCoords(position, splatSize);
        Point splatCoordsTile = Map.Instance().WorldToTextureCoords(TileRender.GetTileBounds(), splatSize);

        //Adjust the control UV
        Vector4 posParam = new Vector4();
        posParam.w = splatCoords.x/(float) splatSize;
        posParam.x = splatCoords.y/(float) splatSize;
        posParam.y = splatCoordsTile.x / ((float)splatSize); // UV Scale parameters
        posParam.z = splatCoordsTile.y / ((float)splatSize) / Map.Instance().terrainSettings.tileSideLength;
        posParam.z = posParam.z * Map.Instance().terrainSettings.tileTextureScale;

        return posParam;
    }

    public static Material CreateMaterial(Vector3 pos, SplatSettings splatSettings)
    {
        Material mat = new Material(Shader.Find("Custom/TerrainSplat"));
        mat.SetTexture("_Control", splatSettings.control); //Delete?
        mat.SetVector("_Position", CalculateSplatUV(pos));
        mat.SetTexture("_Splat0", splatSettings.mat1);
        mat.SetTexture("_Splat1", splatSettings.mat2);
        mat.SetTexture("_Splat2", splatSettings.mat3);

        return mat;
    }

    private static void BuildMesh()
    {
        for (int x = 0; x < Map.Instance().terrainSettings.tileSideLength; x++)
        {
            for (int z = 0; z < Map.Instance().terrainSettings.tileSideLength; z++)
            {
                Vector3 position = new Vector3(Map.Instance().terrainSettings.tileSquareSize * x, 0, Map.Instance().terrainSettings.tileSquareSize * z);
                CreateFace(position);
            }
        }

        currentMesh.vertices = l_vertices.ToArray();

        currentMesh.uv = l_uvs.ToArray();
        currentMesh.triangles = l_triangles.ToArray();
        currentMesh.normals = l_normals.ToArray();
        currentMesh.RecalculateNormals();
        currentMesh.RecalculateBounds();
    }

    //Should this be here?
    private static float IntNoise(int x)
    {
        x = (x << 13) ^ x;
        return (1.0f - ((x * (x * x * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0f);
    }

    private static Vector3 GetNoiseOffset(Vector3 pos)
    {
        float scale = Map.Instance().terrainSettings.gridNoiseScale;
        int hash1 = (int)(pos.x + pos.z * Map.Instance().terrainSettings.tileSideLength);
        int hash2 = (int)(pos.x - pos.z * Map.Instance().terrainSettings.tileSideLength);
        Vector3 noise = new Vector3(IntNoise(hash1), IntNoise(hash1), IntNoise(hash2));
        noise.x *= scale;
        noise.z *= scale;
        noise.y *= Map.Instance().terrainSettings.heightNoiseScale;
        return noise;
    }

    private static void CreateFace(Vector3 pos)
    {
        Vector3 origin = pos;
        int vertCount = l_vertices.Count;

        float squareSize = Map.Instance().terrainSettings.tileSquareSize;

        Vector3 p0 = origin + new Vector3(0, 0, squareSize);
        p0.y = Map.Instance().GetTerrainHeight(p0 + currentPos);
        p0 = p0 + GetNoiseOffset(p0 + currentPos);
        l_vertices.Add(p0);

        Vector3 p1 = origin + new Vector3(squareSize, 0, squareSize);
        p1.y = Map.Instance().GetTerrainHeight(p1 + currentPos);
        p1 = p1 + GetNoiseOffset(p1 + currentPos);
        l_vertices.Add(p1);

        Vector3 p2 = origin + new Vector3(squareSize, 0, 0);
        p2.y = Map.Instance().GetTerrainHeight(p2 + currentPos);
        p2 = p2 + GetNoiseOffset(p2 + currentPos);
        l_vertices.Add(p2);

        //Build second
        l_vertices.Add(p0);

        l_vertices.Add(p2);

        Vector3 p3 = origin + new Vector3(0, 0, 0);
        p3.y = Map.Instance().GetTerrainHeight(p3 + currentPos);
        p3 = p3 + GetNoiseOffset(p3 + currentPos);
        l_vertices.Add(p3);

        l_triangles.Add(vertCount);
        l_triangles.Add(vertCount + 1);
        l_triangles.Add(vertCount + 2);

        l_triangles.Add(vertCount + 3);
        l_triangles.Add(vertCount + 4);
        l_triangles.Add(vertCount + 5);

        Vector3 norm = Vector3.Cross(p1 - p0, p2 - p0).normalized;
        for (int i = 0; i < 3; i++)
        {
            l_normals.Add(norm * 1.0f);
        }

        Vector3 norm2 = Vector3.Cross(p2 - p0, p3 - p0).normalized;
        for (int i = 0; i < 3; i++)
        {
            l_normals.Add(norm2 * 1.0f);
        }

        for (int i = 0; i < 6; i++)
        {
            l_uvs.Add(GetUV(l_vertices[vertCount + i]));
        }

    }

    private static Vector2 GetUV(Vector3 vertPos)
    {
        Vector2 uv = new Vector2(vertPos.x, vertPos.z);
        TerrainSettings settings = Map.Instance().terrainSettings;
        uv /= (settings.tileTextureScale * settings.tileSquareSize);

        return uv;
    }
}
