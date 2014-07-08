using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(HeightMap))]
public class HeightMapEditor : Editor
{
	Texture2D preview;
	float discreteCount = 0.0f;
	public override void OnInspectorGUI()
	{
		HeightMap heightmap = (HeightMap)target;
		DrawDefaultInspector();
		discreteCount = EditorGUILayout.Slider("Discrete Count", discreteCount, 0.0f, 100.0f);
		heightmap.discreteCount = discreteCount;

		if (preview == null)
		{
			//preview = new Texture2D(heightmap.GetMapWidth (), heightmap.GetMapHeight());
			
		}

		//EditorGUI.DrawPreviewTexture (new Rect(10,10,100,100), preview);
	}
}