using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TilePlacer))]
public class TilePlacerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		TilePlacer terr = (TilePlacer)target;
		if(GUILayout.Button("Clear Terrain"))
		{
			terr.ClearTerrain();
		}
		if(GUILayout.Button("Build Terrain"))
		{
			terr.MarkDirty ();
		}
	}
}