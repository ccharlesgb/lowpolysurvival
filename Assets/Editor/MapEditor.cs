using System;
using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    private bool paintingTexture = false;

    private Map.BrushSettings brushSettings = new Map.BrushSettings();

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Map map = (Map) target;
        if (GUILayout.Button("Update Float Fields"))
        {
            map.UpdateFloatFields();
        }

        if (!paintingTexture)
        {
            if (GUILayout.Button("Paint Texture"))
            {
                map.BeginTexturePaint();
                paintingTexture = true;
            }
        }
        else
        {
            if (GUILayout.Button("Stop Painting"))
            {
                map.EndTexturePaint();
                paintingTexture = false;
            }
        }


        GUI.enabled = paintingTexture;

        brushSettings.paintChannel = EditorGUILayout.IntSlider("Splat Channel", brushSettings.paintChannel, 1, 3);
        brushSettings.opacity = EditorGUILayout.Slider("Brush Opacity", brushSettings.opacity, 0.0f, 1.0f);
        brushSettings.size = EditorGUILayout.Slider("Brush Size", brushSettings.size, 1.0f, 100.0f);

        GUI.enabled = true;
    }

    void OnSceneGUI()
    {
        Map map = (Map) target;
        if (!paintingTexture) return;
        if (Event.current.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(0);
        }
        if (Event.current.type == EventType.mouseDrag && Event.current.button == 0)
        {
            if (Camera.current != null)
            {
                Vector2 mousePos = Event.current.mousePosition;
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
                else
                {
                    Debug.Log("miss");
                }

                Event.current.Use();
            }
            else
            {
                Debug.Log("camera is null");
            }
        }
    }
}