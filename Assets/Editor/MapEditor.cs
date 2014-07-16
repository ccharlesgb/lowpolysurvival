using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using System.Collections;
using UnityEditor;

[System.Serializable]
public class BrushSettings
{
    public int size;
    public float opacity;
    public int paintChannel;

    public brushMode Mode;
    public enum brushMode
    {
        Raise,
        Smooth
    };
}


[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    //private Texture2D heightMap;
    //private Texture2D splatMap;

    private bool paintingTexture = false;
    private bool paintingHeight = false;

    private BrushSettings brushSettings = new BrushSettings();
    private BrushSettings brushSettingsHeight = new BrushSettings();

    private Map map;

    private bool showSplatPaint = false;
    private bool showHeightPaint = false;

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

        if (GUILayout.Button("Texture From Slopes"))
        {
            GenerateSplatTexture(map.splatSettings.control);
        }
        if (GUILayout.Button("Load Heightmap Texture"))
        {
            string absPath = EditorUtility.OpenFilePanel("Select Heightmap", "", "");
            if (absPath.StartsWith(Application.dataPath))
            {
                string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
                Texture2D loadHeights = AssetDatabase.LoadAssetAtPath(relPath, typeof (Texture2D)) as Texture2D;
                map.heightTexture = TextureTools.DeepCopy(loadHeights);
                BuildTerrainMesh();
            }
        }

        //Paint texture GUI
        showSplatPaint = EditorGUILayout.Foldout(showSplatPaint, "Paint Splats");
        if (showSplatPaint)
        {
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
        }

        showHeightPaint = EditorGUILayout.Foldout(showHeightPaint, "Paint Height");
        if (showHeightPaint)
        {
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
            brushSettingsHeight.Mode = (BrushSettings.brushMode)EditorGUILayout.EnumPopup("Paint Mode", brushSettingsHeight.Mode);
            brushSettingsHeight.opacity = EditorGUILayout.Slider("Brush Opacity", brushSettingsHeight.opacity, -0.1f,
                0.1f);
            brushSettingsHeight.size = EditorGUILayout.IntSlider("Brush Size", brushSettingsHeight.size, 1, 100);
            GUI.enabled = true;
        }
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
                    addVal[brushSettings.paintChannel - 1] += brushStrength*2;
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
            if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Terrain")) return;
            Texture2D heightMap = map.heightTexture;
            Vector3 terrainPos = hit.point;
            terrainPos.y = 0.0f;
            Point heightCoord = map.WorldToTextureCoords(terrainPos, heightMap.width);

            float averageHeight = 0.0f;
            for (int x = -brushSettingsHeight.size; x < brushSettingsHeight.size; x++)
            {
                for (int y = -brushSettingsHeight.size; y < brushSettingsHeight.size; y++)
                {
                    averageHeight += heightMap.GetPixel(heightCoord.x + x, heightCoord.y + y).r;
                }
            }
            averageHeight /= (float)brushSettingsHeight.size*(float)brushSettingsHeight.size*4.0f;

            int brushSize = brushSettingsHeight.size;
            float brushOpacity = brushSettingsHeight.opacity;
            float brushWidth = brushSize / 2.0f;

            for (int x = -brushSize; x < brushSize; x++)
            {
                for (int y = -brushSize; y < brushSize; y++)
                {
                    float originalVal = heightMap.GetPixel(heightCoord.x + x, heightCoord.y + y).r;

                    //2D gaussian can be used to model a 'soft' paint brush
                    float brushStrength = MathTools.Gaussian2D(new Vector2(x, y), brushOpacity, Vector2.zero,
                        new Vector2(brushWidth, brushWidth));
                    if (brushSettingsHeight.Mode == BrushSettings.brushMode.Raise)
                    {
                        originalVal += brushStrength;
                        originalVal = Mathf.Clamp(originalVal, 0.0f, 1.0f);
                    }
                    else
                    {
                        originalVal = originalVal - (originalVal - averageHeight)*brushStrength;
                    }

                    heightMap.SetPixel(heightCoord.x + x, heightCoord.y + y, new Color(originalVal,originalVal,originalVal));
                }
            }

            heightMap.Apply();

            GameObject hitTile = hit.collider.gameObject;
            Mesh newMesh = TileMeshBuilder.GenerateMesh(hitTile.transform.position);
            hitTile.GetComponent<MeshFilter>().sharedMesh = newMesh;
            hitTile.GetComponent<MeshCollider>().sharedMesh = newMesh;

            TileMeshBuilder.ClearMesh();
            /*
            TilePlacer placer = GetComponent<TilePlacer>();

            float brushSizeWorld = terrainSettings.tileSquareSize * 2.0f;

            TileRender tile = placer.GetTileAt(pos + new Vector3(-brushSizeWorld, 0, -brushSizeWorld));
            tile.CreateMesh();

            tile = placer.GetTileAt(pos + new Vector3(+brushSizeWorld, 0, -brushSizeWorld));
            tile.CreateMesh();

            tile = placer.GetTileAt(pos + new Vector3(+brushSizeWorld, 0, +brushSizeWorld));
            tile.CreateMesh();

            tile = placer.GetTileAt(pos + new Vector3(-brushSizeWorld, 0, +brushSizeWorld));
            tile.CreateMesh();*/
        }
    }

    public void ClearTerrainMesh()
    {
        map.ClearTerrain();
    }

    public void BuildTerrainMesh()
    {
        AssetDatabase.CreateAsset(TextureTools.DeepCopy(map.heightTexture),"Assets/Terrain/terrainHeights.asset");
        AssetDatabase.SaveAssets();

        ClearTerrainMesh();
        TerrainSettings terrSettings = map.terrainSettings;
        SplatSettings splatSettings = map.splatSettings;

        splatSettings.control = new Texture2D(map.heightTexture.width, map.heightTexture.width);

        GenerateSplatTexture(splatSettings.control);
        AssetDatabase.AddObjectToAsset(splatSettings.control, "Assets/Terrain/terrainHeights.asset");
        AssetDatabase.SaveAssets();

        //Spawn the tiles
        int terrainSize = Map.Instance().terrainSettings.tileArraySideLength;
        for (int x = 0; x < terrainSize; x++)
        {
            for (int z = 0; z < terrainSize; z++)
            {
                Vector3 tilePos = MathTools.ScalarMultiply(new Vector3(x, 0, z), TileRender.GetTileBounds());

                GameObject tile = CreateTile(tilePos);

                Undo.RegisterCreatedObjectUndo(tile, "Created Tiles");

                TileMeshBuilder.ClearMesh(); //IMPORTANT TO DELETE STATIC VARIABLES

                map.tileList.Add(tile);
            }
        }
        AssetDatabase.SaveAssets();
    }

    public GameObject CreateTile(Vector3 tilePos)
    {
        //Spawn a new tile object
        GameObject tile = Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Empty.prefab", typeof(object))) as GameObject;
        tile.AddComponent<MeshFilter>();
        tile.AddComponent<MeshRenderer>();
        tile.AddComponent<MeshCollider>();
        tile.layer = LayerMask.NameToLayer("Terrain");

        tile.transform.position = tilePos;
        tile.transform.parent = map.transform;

        Mesh tileMesh = TileMeshBuilder.GenerateMesh(tilePos);
        tile.GetComponent<MeshFilter>().sharedMesh = tileMesh;
        tile.GetComponent<MeshRenderer>().sharedMaterial = TileMeshBuilder.CreateMaterial(tilePos, map.splatSettings);
        tile.GetComponent<MeshCollider>().sharedMesh = tile.GetComponent<MeshFilter>().sharedMesh;
        //tile.hideFlags = HideFlags.HideAndDontSave;
        AssetDatabase.AddObjectToAsset(tileMesh, "Assets/Terrain/terrainHeights.asset");

        return tile;
    }

    public void GenerateSplatTexture(Texture2D splat)
    {
        var gradientMag = ScriptableObject.CreateInstance<FloatField>();
        Texture2D gradMags = TextureTools.GetDerivativeMap(map.heightTexture, map.splatSettings.splatSubSamples);

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
    }
}