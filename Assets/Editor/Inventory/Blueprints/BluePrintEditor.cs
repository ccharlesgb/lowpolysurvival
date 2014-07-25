using System.Collections.Generic;
using LowPolySurvival.Inventory.Blueprints;
using UnityEditor;
using UnityEngine;
using MasterList = LowPolySurvival.Inventory.MasterList;

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

		// Switch between active blueprint.
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

		GUILayout.FlexibleSpace();

		// Add/Remove blueprints.
		if (GUILayout.Button("Add", GUILayout.ExpandWidth(false)))
		{
			AddItem();
		}
		if (GUILayout.Button("Remove", GUILayout.ExpandWidth(false)))
		{
			DeleteItem(_viewIndex - 1);
		}

		GUILayout.EndHorizontal();

		//Grab all the ItemDetails names so we can create a drop down
		List<string> itemNames = MasterList.Instance.itemList.GetItemNames();
		string[] itemNamesArray = itemNames.ToArray();

		if (BlueprintList.List.Count > 0)
		{
			Blueprint blueprint = BlueprintList.List[_viewIndex - 1];

			GUILayout.BeginHorizontal();

			_viewIndex = Mathf.Clamp(EditorGUILayout.IntField("Current Blueprint", _viewIndex, GUILayout.ExpandWidth(false)), 1,
				BlueprintList.List.Count);
			EditorGUILayout.LabelField("of    " + BlueprintList.List.Count);
			GUILayout.EndHorizontal();

			blueprint.PrintName = EditorGUILayout.TextField("Name", blueprint.PrintName);

			GUILayout.Label("Required Items:");
			ShowItemList(blueprint.RequiredItems, itemNames, itemNamesArray);

			GUILayout.Label("Output Items:");
			ShowItemList(blueprint.OutputItems, itemNames, itemNamesArray);
		}
		else
		{
			GUILayout.Label("This list is empty");
		}
	}

	private static void ShowItemList(List<Blueprint.BlueprintIngredient> list, List<string> itemNames, string[] itemNamesArray)
	{
		for (int i = 0; i < list.Count; i++)
		{
			Blueprint.BlueprintIngredient item = list[i];

			GUILayout.BeginHorizontal();

			int oldID = Mathf.Clamp(itemNames.IndexOf(item.ItemDetails.itemName), 0,
				MasterList.Instance.itemList.itemList.Count);

			int itemID = EditorGUILayout.Popup(oldID, itemNamesArray);
			string itemName = itemNames[itemID];

			// ItemDetails was changed.
			if (item.ItemDetails.itemName != itemName)
			{
				item.ItemDetails = MasterList.Instance.itemList.FindByName(itemName);
			}

			item.Amount = EditorGUILayout.IntField(item.Amount, GUILayout.MaxWidth(40));

			if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
			{
				list.RemoveAt(i);
			}
			GUILayout.EndHorizontal();
		}

		if (GUILayout.Button("Add", GUILayout.ExpandWidth(false)))
		{
			var newIngredient = new Blueprint.BlueprintIngredient();
			newIngredient.ItemDetails = MasterList.Instance.itemList.itemList[0];
			newIngredient.Amount = 1;

			list.Add(newIngredient);
		}
	}

	private void AddItem()
	{
		var newBlueprint = new Blueprint();
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