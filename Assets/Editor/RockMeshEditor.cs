using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RockMesh))]
public class RockMeshEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		//Target of the inspector
		RockMesh tgt = (RockMesh)target;
		if (GUILayout.Button("Build Mesh"))
		{
			tgt.CreateMesh();
		}
	}
}