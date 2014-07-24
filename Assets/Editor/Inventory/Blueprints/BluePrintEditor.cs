using LowPolySurvival.Inventory.Blueprints;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class BluePrintEditor : EditorWindow
{
	public BlueprintList BlueprintList;
	private int _viewIndex = 1;

	[MenuItem("Window/Blueprint Editor")]
	private static void Init()
	{
		GetWindow(typeof (BluePrintEditor));
	}

	private void OnEnable()
	{
		// Attempt to load the previously loaded list.
		if (EditorPrefs.HasKey("ObjectPathBlueprint"))
		{
			string objectPath = EditorPrefs.GetString("ObjectPathBlueprint");
			BlueprintList = AssetDatabase.LoadAssetAtPath(objectPath, typeof (BlueprintList)) as BlueprintList;
		}
	}

	private void OnGUI()
	{
		GUILayout.Label("Blueprint Editor", EditorStyles.boldLabel);

		GUILayout.BeginHorizontal();

		// Show the active blueprint list in Inspector.
		if (BlueprintList != null)
		{
			if (GUILayout.Button("Show List"))
			{
				EditorUtility.FocusProjectWindow();
				Selection.activeObject = BlueprintList;
			}
		}

		// Show the FileBrowser.
		if (GUILayout.Button("Open List"))
		{
			OpenItemList();
		}

		// Create a new Blueprint list.
		if (GUILayout.Button("New List"))
		{
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = BlueprintList;
		}

		GUILayout.EndHorizontal();

		if (BlueprintList == null)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(10);
			if (GUILayout.Button("Create New List", GUILayout.ExpandWidth(false)))
				CreateNewBlueprintList();
			if (GUILayout.Button("Open Existing List", GUILayout.ExpandWidth(false)))
				OpenItemList();
			GUILayout.EndHorizontal();
		}
		GUILayout.Space(20);

		if (BlueprintList != null && BlueprintList.List != null)
		{
			ShowBlueprintList();
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty(BlueprintList);
		}
	}

	private void ShowBlueprintList()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(10);

		if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
		{
			if (_viewIndex > 1)
				_viewIndex--;
		}
		if (GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
		{
			if (_viewIndex < BlueprintList.List.Count)
				_viewIndex++;
		}
		GUILayout.Space(60);

		if (GUILayout.Button("Add ItemDetails", GUILayout.ExpandWidth(false)))
		{
			AddItem();
		}
		if (GUILayout.Button("Delete ItemDetails", GUILayout.ExpandWidth(false)))
		{
			DeleteItem(_viewIndex - 1);
		}

		GUILayout.EndHorizontal();

		//Grab all the ItemDetails names so we can create a drop down
		List<string> itemNames = LowPolySurvival.Inventory.MasterList.Instance.itemList.GetItemNames();

		if (BlueprintList.List.Count > 0)
		{
			Blueprint curPrint = BlueprintList.List[_viewIndex - 1];

			GUILayout.BeginHorizontal();

			_viewIndex = Mathf.Clamp(EditorGUILayout.IntField("Current Blueprint", _viewIndex, GUILayout.ExpandWidth(false)), 1,
				BlueprintList.List.Count);
			EditorGUILayout.LabelField("of    " + BlueprintList.List.Count);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			//Adds a new required part to the current blueprint

			/*

                if (GUILayout.Button("Add Input Part", GUILayout.ExpandWidth(false)))
                {
                    ItemSlot newPart = ScriptableObject.CreateInstance<ItemSlot>();
                    newPart.ItemDetails = null;
                    newPart.Amount = 1;
                    curPrint.RequiredItems.Add(newPart);
                }
                if (GUILayout.Button("Add Output Part", GUILayout.ExpandWidth(false)))
                {
                    ItemSlot newPart = ScriptableObject.CreateInstance<ItemSlot>();
                    newPart.ItemDetails = null;
                    newPart.Amount = 1;
                    curPrint.OutputItems.Add(newPart);
                }
				*/

			GUILayout.EndHorizontal();
			curPrint.PrintName = EditorGUILayout.TextField("Name", curPrint.PrintName as string);

			GUILayout.Label("Required Items");

			/*for (int i2 = 0; i2 < curPrint.requiredItems.Count; i2++)
                {
                    string itemName = EditorGUILayout.TextField("Part ItemDetails", curPrint.requiredItems[i2].ItemDetails.itemName as string);
                    ItemDetails testItem = MasterList.Instance.itemList.FindByName(itemName);
                    if (testItem != null)
                    {
                        curPrint.requiredItems[i2].ItemDetails = testItem;
                    }
                }*/

			/*
                itemList.itemList[viewIndex - 1].itemName = EditorGUILayout.TextField("ItemDetails Name", itemList.itemList[viewIndex - 1].itemName as string);
                itemList.itemList[viewIndex - 1].itemIcon = EditorGUILayout.ObjectField("ItemDetails Icon", itemList.itemList[viewIndex - 1].itemIcon, typeof(Texture2D), false) as Texture2D;
                itemList.itemList[viewIndex - 1].itemObject = EditorGUILayout.ObjectField("ItemDetails Prefab", itemList.itemList[viewIndex - 1].itemObject, typeof(GameObject), false) as GameObject;

                itemList.itemList[viewIndex - 1].isStackable = (bool)EditorGUILayout.Toggle("Stackable", itemList.itemList[viewIndex - 1].isStackable, GUILayout.ExpandWidth(false));
                GUI.enabled = itemList.itemList[viewIndex - 1].isStackable;
                itemList.itemList[viewIndex - 1].stackSize = (int)EditorGUILayout.IntField("Stack Size", itemList.itemList[viewIndex - 1].stackSize);
                GUI.enabled = true;

                itemList.itemList[viewIndex - 1].isEquipable = (bool)EditorGUILayout.Toggle("Equipable", itemList.itemList[viewIndex - 1].isEquipable, GUILayout.ExpandWidth(false));
                */
		}
		else
		{
			GUILayout.Label("This list is empty");
		}
	}

	private void AddItem()
	{
		Blueprint newBlueprint = new Blueprint();
		newBlueprint.PrintName = "New Blueprint";
		BlueprintList.List.Add(newBlueprint);
		_viewIndex = BlueprintList.List.Count;
	}

	private void DeleteItem(int index)
	{
		BlueprintList.List.RemoveAt(index);
	}

	private void CreateNewBlueprintList()
	{
		_viewIndex = 1;
		BlueprintList = CreateBlueprintList.Create();
		if (BlueprintList)
		{
			string path = AssetDatabase.GetAssetPath(BlueprintList);
			EditorPrefs.SetString("ObjectPathBlueprint", path);
		}
	}

	private void OpenItemList()
	{
		string absPath = EditorUtility.OpenFilePanel("Selecting Blueprint List", "", "");
		if (absPath.StartsWith(Application.dataPath))
		{
			string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
			BlueprintList = AssetDatabase.LoadAssetAtPath(relPath, typeof (BlueprintList)) as BlueprintList;

			if (BlueprintList)
			{
				EditorPrefs.SetString("ObjectPathBlueprint", relPath);
			}
		}
	}
}