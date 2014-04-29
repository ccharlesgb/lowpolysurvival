using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(Inventory))]
public class InventoryEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		Inventory inv = (Inventory)target;
		ItemList list = ItemList.Instance;

		for (int i = 0; i < list.items.Count; i++)
		{
			int amount = 0;
			ItemHandle test = inv.FindItem(list.items[i].name);
			if (test != null && test.amount > 0)
				amount = test.amount;

			amount = EditorGUILayout.IntField (list.items[i].name, amount);

			if (test == null && amount > 0)
			{
				ItemHandle it = new ItemHandle();
				it.item = list.items[i];
				it.amount = amount;
				inv.AddItem (it);
			}

			if (test != null)
			{
				test.amount = amount;
			}
		}
	}
}