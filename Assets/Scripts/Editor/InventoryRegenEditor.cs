using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(InventoryRegen))]
public class InventoryRegenEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		InventoryRegen invRegen = (InventoryRegen)target;
		ItemList list = ItemList.Instance;

		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField ("Item", GUILayout.Width (100));
		EditorGUILayout.LabelField ("Recharge/Sec", GUILayout.Width (220));
		EditorGUILayout.LabelField ("Max Val", GUILayout.Width (100));
		GUILayout.EndHorizontal();

		for (int i = 0; i < list.items.Count; i++)
		{
			Item item = list.items[i];
			RegenInfo regen = invRegen.GetRegen (item.name);
			float recharge = 0;
			float maxval = 0;

			if (regen != null)
			{
				recharge = regen.regen;
				maxval = regen.max;
			}

			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField (item.name, GUILayout.Width (100));
			recharge = EditorGUILayout.FloatField (recharge);
			maxval = EditorGUILayout.FloatField (maxval);

			GUILayout.EndHorizontal();
			if (recharge != 0)
				invRegen.SetRegen(item.name, recharge, maxval);
			else if (regen != null)
				invRegen.DeleteRegen(item.name);
		}
	}
}