using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(VegetationSpawner))]
public class VegetationSpawnerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		//Target of the inspector
		VegetationSpawner tgt = (VegetationSpawner)target;
		if (GUILayout.Button("Clear Props"))
		{
			tgt.ClearVeg();
		}
		if (GUILayout.Button("Place Props"))
		{
			tgt.SpawnVeg();
		}
	}
}