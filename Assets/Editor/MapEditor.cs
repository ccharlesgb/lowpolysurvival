using System;
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    private Texture2D heightMap;
    private Texture2D splatMap;

    private bool paintingTexture = false;
    private bool paintingHeight = false;

    private Map.BrushSettings brushSettings = new Map.BrushSettings();
    private Map.BrushSettings brushSettingsHeight = new Map.BrushSettings();

    private Map map;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        map = (Map) target;

        if (GUILayout.Button("Build Terrain Mesh"))
        {
            BuildTerrainMesh();
        }

        if (GUILayout.Button("Create Blank Heightmap"))
        {
            //
        }
        if (GUILayout.Button("Load Heightmap Texture"))
        {
            string absPath = EditorUtility.OpenFilePanel("Select Heightmap", "", "");
            if (absPath.StartsWith(Application.dataPath))
            {
                string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
                //map.LoadHeightMap(relPath);
            }
        }

        //Paint texture GUI
        EditorGUILayout.LabelField("Paint Texture Splats", EditorStyles.boldLabel);
        if (!paintingTexture)
        {
            if (GUILayout.Button("Paint Texture"))
            {
                paintingTexture = true;
            }
        }
        else
        {
            if (GUILayout.Button("Stop Painting"))
            {
                paintingTexture = false;
            }
        }

        GUI.enabled = paintingTexture;
        brushSettings.paintChannel = EditorGUILayout.IntSlider("Splat Channel", brushSettings.paintChannel, 1, 3);
        brushSettings.opacity = EditorGUILayout.Slider("Brush Opacity", brushSettings.opacity, 0.0f, 1.0f);
        brushSettings.size = EditorGUILayout.Slider("Brush Size", brushSettings.size, 1.0f, 100.0f);
        GUI.enabled = true;

        EditorGUILayout.LabelField("Paint Terrain Height", EditorStyles.boldLabel);
        //Height painting
        if (!paintingHeight)
        {
            if (GUILayout.Button("Paint Height"))
            {
                paintingHeight = true;
            }
        }
        else
        {
            if (GUILayout.Button("Stop Painting"))
            {
                paintingHeight = false;
            }
        }

        GUI.enabled = paintingHeight;
        brushSettingsHeight.paintChannel = EditorGUILayout.IntSlider("Splat Channel", brushSettingsHeight.paintChannel, 1, 3);
        brushSettingsHeight.opacity = EditorGUILayout.Slider("Brush Opacity", brushSettingsHeight.opacity, -0.1f, 0.1f);
        brushSettingsHeight.size = EditorGUILayout.Slider("Brush Size", brushSettingsHeight.size, 1.0f, 100.0f);
        GUI.enabled = true;
    }

    void OnSceneGUI()
    {
        Map map = (Map) target;
        if (!paintingTexture && !paintingHeight) return;
        if (Event.current.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(0);
        }
        if (Event.current.type == EventType.mouseDrag && Event.current.button == 0)
        {
            if (Camera.current != null)
            {
                if (paintingTexture)
                    PaintSplats(Event.current);
                if (paintingHeight)
                    PaintHeight(Event.current);
                Event.current.Use();
            }
            else
            {
                Debug.Log("camera is null");
            }
        }
    }

    void PaintSplats(Event mouseEvent)
    {
        Map map = (Map)target;
        //Hack. Mouse is flipped for some reason
        Vector2 mousePos = mouseEvent.mousePosition;
        mousePos.y *= -1.0f;
        mousePos.y += (float)Camera.current.GetScreenHeight();

        Ray ray = Camera.current.ScreenPointToRay(mousePos);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 terrainPos = hit.point;
            terrainPos.y = 0.0f;
            map.PaintSplat(terrainPos, brushSettings);
        }
    }

    void PaintHeight(Event mouseEvent)
    {
        Map map = (Map)target;
        //Hack. Mouse is flipped for some reason
        Vector2 mousePos = mouseEvent.mousePosition;
        mousePos.y *= -1.0f;
        mousePos.y += (float)Camera.current.GetScreenHeight();

        Ray ray = Camera.current.ScreenPointToRay(mousePos);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 terrainPos = hit.point;
            terrainPos.y = 0.0f;
            map.PaintHeight(terrainPos, brushSettingsHeight);
        }
    }

    public void BuildTerrainMesh()
    {
        TerrainSettings terrSettings = map.terrainSettings;
        SplatSettings splatSettings = map.splatSettings;

        splatSettings.control = GenerateSplatTexture();

        //Spawn the tiles
        int terrainSize = Map.Instance().terrainSettings.tileArraySideLength;
        for (int x = 0; x < terrainSize; x++)
        {
            for (int z = 0; z < terrainSize; z++)
            {
                //Spawn a new tile object
                GameObject tile = new GameObject {name = "Tile"};
                tile.AddComponent<MeshFilter>();
                tile.AddComponent<MeshRenderer>();
                tile.AddComponent<MeshCollider>();

                Vector3 tilePos = MathTools.ScalarMultiply(new Vector3(x, 0, z), TileRender.GetTileBounds());
                tile.transform.position = tilePos;
                tile.transform.parent = map.transform;

                tile.GetComponent<MeshFilter>().sharedMesh = TileMeshBuilder.GenerateMesh(tilePos);
                tile.GetComponent<MeshRenderer>().sharedMaterial = TileMeshBuilder.CreateMaterial(tilePos, splatSettings);
                tile.GetComponent<MeshCollider>().sharedMesh = tile.GetComponent<MeshFilter>().sharedMesh;

                Undo.RegisterCreatedObjectUndo(tile, "Created Tiles");

                TileMeshBuilder.ClearMesh(); //IMPORTANT TO DELETE STATIC VARIABLES
            }
        }
    }

    public Vector2 GetSlopeAt(int x, int y, int gap)
    {
        float valAtPos = map.heightTexture.GetPixel(x, y).grayscale;
        Vector2 gradient = new Vector2();

        if (x >= map.heightTexture.width - gap - 2 || y >= map.heightTexture.height - gap - 2)
        {
            return gradient;
        }

        gradient.x = map.heightTexture.GetPixel(x + gap, y).grayscale - valAtPos;
        gradient.y = map.heightTexture.GetPixel(x, y + gap).grayscale - valAtPos;
        return gradient;
    }

    private void CalculateSlopes(FloatField output, int subSamples)
    {
        int pixelCount = map.heightTexture.width*map.heightTexture.width;

        Vector2 gradient = new Vector2();
        output.Create(map.heightTexture.height, map.heightTexture.width);
        float maxGrad = 0.0f;
        for (int x = 0; x < map.heightTexture.width; x++)
        {
            for (int y = 0; y < map.heightTexture.width; y++)
            {
                gradient = GetSlopeAt(x, y, subSamples);
                float grad = gradient.magnitude;

                if (grad > maxGrad)
                    maxGrad = grad;

                output.SetValue(x, y, grad);
            }
        }
        //Normalize the gradient map
        if (maxGrad != 0.0f)
        {
            for (int i = 0; i < pixelCount; i++)
            {
                output.SetValue(i, output.GetValue(i) / maxGrad);
            }
        }
    }

    public Texture2D GenerateSplatTexture()
    {
        var gradientMag = ScriptableObject.CreateInstance<FloatField>();
        CalculateSlopes(gradientMag, map.splatSettings.splatSubSamples);

        Texture2D splat = new Texture2D(map.heightTexture.width, map.heightTexture.height);
        for (int x = 0; x < map.heightTexture.width; x++)
        {
            for (int y = 0; y < map.heightTexture.width; y++)
            {
                Color col = map.splatSettings.GetSplatChannelValue(gradientMag.GetValue(x,y));
                col.a = 1.0f;
                splat.SetPixel(x, y, col);
            }
        }
        splat.Apply();
        DestroyImmediate(gradientMag);
        return splat;
    }
}