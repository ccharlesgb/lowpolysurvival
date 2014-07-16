using System;
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    private bool paintingTexture = false;
    private bool paintingHeight = false;

    private Map.BrushSettings brushSettings = new Map.BrushSettings();
    private Map.BrushSettings brushSettingsHeight = new Map.BrushSettings();

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Map map = (Map) target;
        if (GUILayout.Button("Create Blank Heightmap"))
        {
            map.ResetHeightMap();
        }
        if (GUILayout.Button("Load Heightmap Texture"))
        {
            string absPath = EditorUtility.OpenFilePanel("Select Heightmap", "", "");
            if (absPath.StartsWith(Application.dataPath))
            {
                string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
                map.LoadHeightMap(relPath);
            }
        }

        if (GUILayout.Button("Recalculate Splats"))
        {
            map.CalculateSplatsFromHeights();
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
}