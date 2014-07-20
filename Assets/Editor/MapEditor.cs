using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using System.Collections;
using UnityEditor;

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


                Texture2D heightCopy = TextureTools.DeepCopy(loadHeights);
                map.heightTexture = heightCopy;
                AssetDatabase.CreateAsset(map.heightTexture, "Assets/Terrain/terrainHeights.asset");
                AssetDatabase.SaveAssets();
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
            brushSettings.opacity = EditorGUILayout.Slider("Brush Opacity", brushSettings.opacity, 0.0f, 0.1f);
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
                //Hack. Mouse is flipped for some reason
                Vector2 mousePos = Event.current.mousePosition;
                mousePos.y *= -1.0f;
                mousePos.y += (float)Screen.height;

                Ray ray = Camera.current.ScreenPointToRay(mousePos);
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Terrain")) return;
                    if (paintingTexture)
                        TerrainPainter.PaintSplats(map, hit, brushSettings);
                    if (paintingHeight)
                        TerrainPainter.PaintHeight(map, hit, brushSettingsHeight);
                    Event.current.Use();
                }
            }
            else
            {
                Debug.Log("camera is null");
            }
        }
    }


    public void ClearTerrainMesh()
    {
        map.ClearTerrain();
    }

    public void BuildTerrainMesh()
    {
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
        GameObject tile = Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Tile.prefab", typeof(object))) as GameObject;
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
        Texture2D gradMags = TextureTools.GetDerivativeMap(map.heightTexture, map.splatSettings.splatSubSamples);
        map.gradTexture = gradMags;
        AssetDatabase.AddObjectToAsset(gradMags, "Assets/Terrain/terrainHeights.asset");
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
    }
}