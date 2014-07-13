using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class InventoryItemEditor : EditorWindow
{
	public ItemList itemList;
	private int viewIndex = 1;

	[MenuItem("Window/Inventory Editor")]
	private static void Init()
	{
		EditorWindow.GetWindow(typeof (InventoryItemEditor));
	}

	private void OnEnable()
	{
		if (EditorPrefs.HasKey("ObjectPath"))
		{
			string objectPath = EditorPrefs.GetString("ObjectPath");
			itemList = AssetDatabase.LoadAssetAtPath(objectPath, typeof (ItemList)) as ItemList;
		}
	}

	private void OnGUI()
	{
		GUILayout.Label("Inventory Item Editor", EditorStyles.boldLabel);
		GUILayout.BeginHorizontal();
		if (itemList != null)
		{
			if (GUILayout.Button("Show List"))
			{
				EditorUtility.FocusProjectWindow();
				Selection.activeObject = itemList;
			}
		}
		if (GUILayout.Button("Open List"))
		{
			OpenItemList();
		}
		if (GUILayout.Button("New List"))
		{
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = itemList;
		}
		GUILayout.EndHorizontal();

		if (itemList == null)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(10);
			if (GUILayout.Button("Create New List", GUILayout.ExpandWidth(false)))
				CreateNewItemList();
			if (GUILayout.Button("Open Existing List", GUILayout.ExpandWidth(false)))
				OpenItemList();
			GUILayout.EndHorizontal();
		}
		GUILayout.Space(20);

		if (itemList != null && itemList.itemList != null)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(10);

			if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
			{
				if (viewIndex > 1)
					viewIndex--;
			}
			if (GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
			{
				if (viewIndex < itemList.itemList.Count)
					viewIndex++;
			}
			GUILayout.Space(60);

			if (GUILayout.Button("Add Item", GUILayout.ExpandWidth(false)))
			{
				AddItem();
			}
			if (GUILayout.Button("Delete Item", GUILayout.ExpandWidth(false)))
			{
				DeleteItem(viewIndex - 1);
			}

			GUILayout.EndHorizontal();

			if (itemList.itemList.Count > 0)
			{
				GUILayout.BeginHorizontal();

				viewIndex = Mathf.Clamp(EditorGUILayout.IntField("Current Item", viewIndex, GUILayout.ExpandWidth(false)), 1,
					itemList.itemList.Count);
				EditorGUILayout.LabelField("of    " + itemList.itemList.Count);
				GUILayout.EndHorizontal();

				itemList.itemList[viewIndex - 1].itemName = EditorGUILayout.TextField("Item Name",
					itemList.itemList[viewIndex - 1].itemName as string);
				itemList.itemList[viewIndex - 1].itemIcon =
					EditorGUILayout.ObjectField("Item Icon", itemList.itemList[viewIndex - 1].itemIcon, typeof (Texture2D), false) as
						Texture2D;
				itemList.itemList[viewIndex - 1].itemObject =
					EditorGUILayout.ObjectField("Item Prefab", itemList.itemList[viewIndex - 1].itemObject, typeof (GameObject), false)
						as GameObject;

				itemList.itemList[viewIndex - 1].isStackable =
					(bool)
						EditorGUILayout.Toggle("Stackable", itemList.itemList[viewIndex - 1].isStackable, GUILayout.ExpandWidth(false));
				GUI.enabled = itemList.itemList[viewIndex - 1].isStackable;
				itemList.itemList[viewIndex - 1].stackSize =
					(int) EditorGUILayout.IntField("Stack Size", itemList.itemList[viewIndex - 1].stackSize);
				GUI.enabled = true;

				itemList.itemList[viewIndex - 1].isEquipable =
					(bool)
						EditorGUILayout.Toggle("Equipable", itemList.itemList[viewIndex - 1].isEquipable, GUILayout.ExpandWidth(false));

			}
			else
			{
				GUILayout.Label("This list is empty");
			}
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty(itemList);
		}
	}

	private void AddItem()
	{
		InventoryItem newItem = new InventoryItem();
		newItem.itemName = "New Item";
		itemList.itemList.Add(newItem);
		viewIndex = itemList.itemList.Count;
	}

	private void DeleteItem(int index)
	{
		itemList.itemList.RemoveAt(index);
	}

	private void CreateNewItemList()
	{
		viewIndex = 1;
		itemList = CreateItemList.Create();
		if (itemList)
		{
			string path = AssetDatabase.GetAssetPath(itemList);
			EditorPrefs.SetString("ObjectPath", path);
		}
	}

	private void OpenItemList()
	{
		string absPath = EditorUtility.OpenFilePanel("Selecting Inventory List", "", "");
		if (absPath.StartsWith(Application.dataPath))
		{
			string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
			itemList = AssetDatabase.LoadAssetAtPath(relPath, typeof (ItemList)) as ItemList;

			if (itemList)
			{
				EditorPrefs.SetString("ObjectPath", relPath);
			}
		}
	}
}