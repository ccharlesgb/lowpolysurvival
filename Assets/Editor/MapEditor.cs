using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    private bool paintingTexture = false;
    private int paintChannel = 0;

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

        paintChannel = EditorGUILayout.IntSlider("Splat Channel", paintChannel, 1, 3);
    }

    void OnSceneGUI()
    {
        Map map = (Map) target;
        if (!paintingTexture) return;
        if (Event.current.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(0);
        }
        if (Event.current.type == EventType.mouseDrag)
        {
            if (Camera.current != null)
            {
                Ray ray = Camera.current.ScreenPointToRay(Event.current.mousePosition);
                
                RaycastHit hit = new RaycastHit();
                Debug.DrawLine(ray.origin, ray.origin + ray.direction * 1000);

                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log(hit.collider.gameObject);
                    Vector3 terrainPos = hit.point;
                    terrainPos.y = 0.0f;
                    map.PaintSplat(terrainPos, paintChannel);
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