using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		Map map = (Map)target;
		if(GUILayout.Button("Update Float Fields"))
		{
			map.UpdateFloatFields();
		}
	}
}