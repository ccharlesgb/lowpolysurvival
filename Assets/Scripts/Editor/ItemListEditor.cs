using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(ItemList))]
public class ItemListEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		ItemList mlist = (ItemList)target;

		for (int i = 0; i < mlist.items.Count; i++)
		{
			EditorGUILayout.Separator();
			mlist.items[i].name = EditorGUILayout.TextField ("Name ", mlist.items[i].name);
			mlist.items[i].stackable = EditorGUILayout.Toggle ("Stackable", mlist.items[i].stackable);
			//EditorGUILayout.

		}
		//Add Remove items
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button ("+"))
		{
			Item newItem = new Item("NAME", true);
			mlist.items.Add (newItem);
		}
		if (GUILayout.Button ("-"))
		{
			mlist.items.RemoveAt (mlist.items.Count-1);
		}
		EditorGUILayout.EndHorizontal ();
	}
}