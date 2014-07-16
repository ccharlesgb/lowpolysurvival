using System;
using System.Collections.Generic;
using NUnit.Framework;
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

    private List<GameObject> tileList = new List<GameObject>();

    private Map map;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        map = (Map) target;

        if (GUILayout.Button("Build Terrain Mesh"))
        {
            BuildTerrainMesh();
        }

        if (GUILayout.Button("Clear Terrain"))
        {
            ClearTerrainMesh();
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
        brushSettings.size = EditorGUILayout.IntSlider("Brush Size", brushSettings.size, 1, 100);
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
        brushSettingsHeight.size = EditorGUILayout.IntSlider("Brush Size", brushSettingsHeight.size, 1, 100);
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
        if ((Event.current.type == EventType.mouseDown || Event.current.type == EventType.mouseDrag) && Event.current.button == 0)
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
        mousePos.y += (float) Screen.height;

        Ray ray = Camera.current.ScreenPointToRay(mousePos);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 terrainPos = hit.point;
            terrainPos.y = 0.0f;

            Point splatCoord = map.WorldToTextureCoords(terrainPos, map.splatSettings.control.width);

            float brushWidth = brushSettings.size / 2.0f;
            for (int x = -brushSettings.size; x < brushSettings.size; x++)
            {
                for (int y = -brushSettings.size; y < brushSettings.size; y++)
                {
                    Color originalVal = map.splatSettings.control.GetPixel(splatCoord.x + x, splatCoord.y + y);

                    float brushStrength = MathTools.Gaussian2D(new Vector2(x, y), brushSettings.opacity, Vector2.zero,
                        new Vector2(brushWidth, brushWidth));

                    Color addVal = new Color(-brushStrength, -brushStrength, -brushStrength);
                    //Debug.Log("Brush Strength " + brushStrength);
                    addVal[brushSettings.paintChannel - 1] += brushStrength * 2;
                    originalVal += addVal;
                    originalVal.r = Mathf.Clamp(originalVal.r, 0.0f, 1.0f);
                    originalVal.g = Mathf.Clamp(originalVal.g, 0.0f, 1.0f);
                    originalVal.b = Mathf.Clamp(originalVal.b, 0.0f, 1.0f);

                    map.splatSettings.control.SetPixel(splatCoord.x + x, splatCoord.y + y, originalVal);
                }
            }

            map.splatSettings.control.Apply();
        }
    }

    void PaintHeight(Event mouseEvent)
    {
        Map map = (Map)target;
        //Hack. Mouse is flipped for some reason
        Vector2 mousePos = mouseEvent.mousePosition;
        mousePos.y *= -1.0f;
        mousePos.y += (float) Screen.height;

        Ray ray = Camera.current.ScreenPointToRay(mousePos);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 terrainPos = hit.point;
            terrainPos.y = 0.0f;
            map.PaintHeight(terrainPos, brushSettingsHeight);
        }
    }

    public void ClearTerrainMesh()
    {
        foreach (GameObject tile in tileList)
            DestroyImmediate(tile);
    }

    public void BuildTerrainMesh()
    {
        AssetDatabase.CreateAsset(TextureTools.DeepCopy(map.heightTexture),"Assets/Terrain/terrainHeights.asset");
        AssetDatabase.SaveAssets();

        ClearTerrainMesh();
        TerrainSettings terrSettings = map.terrainSettings;
        SplatSettings splatSettings = map.splatSettings;

        splatSettings.control = GenerateSplatTexture();
        AssetDatabase.AddObjectToAsset(splatSettings.control, "Assets/Terrain/terrainHeights.asset");
        AssetDatabase.SaveAssets();

        //Spawn the tiles
        int terrainSize = Map.Instance().terrainSettings.tileArraySideLength;
        for (int x = 0; x < terrainSize; x++)
        {
            for (int z = 0; z < terrainSize; z++)
            {
                //Spawn a new tile object
                GameObject tile = Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Empty.prefab", typeof(object))) as GameObject;
                tile.AddComponent<MeshFilter>();
                tile.AddComponent<MeshRenderer>();
                tile.AddComponent<MeshCollider>();

                Vector3 tilePos = MathTools.ScalarMultiply(new Vector3(x, 0, z), TileRender.GetTileBounds());
                tile.transform.position = tilePos;
                tile.transform.parent = map.transform;

                Mesh tileMesh = TileMeshBuilder.GenerateMesh(tilePos);
                tile.GetComponent<MeshFilter>().sharedMesh = tileMesh;
                tile.GetComponent<MeshRenderer>().sharedMaterial = TileMeshBuilder.CreateMaterial(tilePos, splatSettings);
                tile.GetComponent<MeshCollider>().sharedMesh = tile.GetComponent<MeshFilter>().sharedMesh;
                //tile.hideFlags = HideFlags.HideAndDontSave;
                AssetDatabase.AddObjectToAsset(tileMesh, "Assets/Terrain/terrainHeights.asset");

                Undo.RegisterCreatedObjectUndo(tile, "Created Tiles");

                TileMeshBuilder.ClearMesh(); //IMPORTANT TO DELETE STATIC VARIABLES

                //tileList.Add(tile);
            }
        }
        AssetDatabase.SaveAssets();
    }

    public Texture2D GenerateSplatTexture()
    {
        var gradientMag = ScriptableObject.CreateInstance<FloatField>();
        Texture2D gradMags = TextureTools.GetDerivativeMap(map.heightTexture, map.splatSettings.splatSubSamples);

        Texture2D splat = new Texture2D(map.heightTexture.width, map.heightTexture.height);
        for (int x = 0; x < map.heightTexture.width; x++)
        {
            for (int y = 0; y < map.heightTexture.width; y++)
            {
                Color col = map.splatSettings.GetSplatChannelValue(gradMags.GetPixel(x,y).r);
                col.a = 1.0f;
                splat.SetPixel(x, y, col);
            }
        }
        splat.Apply();
        DestroyImmediate(gradientMag);
        return splat;
    }
}