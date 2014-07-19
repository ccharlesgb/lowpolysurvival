using LowPolySurvival.Inventory;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class BluePrintEditor : EditorWindow
{
    public BlueprintList blueprintList;
    private int viewIndex = 1;

    [MenuItem("Window/Blueprint Editor")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(BluePrintEditor));
    }

    void OnEnable()
    {
        if (EditorPrefs.HasKey("ObjectPathBlueprint"))
        {
            string objectPath = EditorPrefs.GetString("ObjectPathBlueprint");
            blueprintList = AssetDatabase.LoadAssetAtPath(objectPath, typeof(BlueprintList)) as BlueprintList;
        }
    }

    void OnGUI()
    {
        GUILayout.Label("Blueprint Editor", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        if (blueprintList != null)
        {
            if (GUILayout.Button("Show List"))
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = blueprintList;
            }
        }
        if (GUILayout.Button("Open List"))
        {
            OpenItemList();
        }
        if (GUILayout.Button("New List"))
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = blueprintList;
        }
        GUILayout.EndHorizontal();

        if (blueprintList == null)
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

        if (blueprintList != null && blueprintList.blueprintList != null)
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
                if (viewIndex < blueprintList.blueprintList.Count)
                    viewIndex++;
            }
            GUILayout.Space(60);

            if (GUILayout.Button("Add ItemDetails", GUILayout.ExpandWidth(false)))
            {
                AddItem();
            }
            if (GUILayout.Button("Delete ItemDetails", GUILayout.ExpandWidth(false)))
            {
                DeleteItem(viewIndex - 1);
            }

            GUILayout.EndHorizontal();

            //Grab all the ItemDetails names so we can create a drop down
            List<string> itemNames = MasterList.Instance.itemList.GetItemNames();

            if (blueprintList.blueprintList.Count > 0)
            {
                Blueprint curPrint = blueprintList.blueprintList[viewIndex - 1];

                GUILayout.BeginHorizontal();

                viewIndex = Mathf.Clamp(EditorGUILayout.IntField("Current Blueprint", viewIndex, GUILayout.ExpandWidth(false)), 1, blueprintList.blueprintList.Count);
                EditorGUILayout.LabelField("of    " + blueprintList.blueprintList.Count);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                //Adds a new required part to the current blueprint
                if (GUILayout.Button("Add Input Part", GUILayout.ExpandWidth(false)))
                {
                    ItemSlot newPart = ScriptableObject.CreateInstance<ItemSlot>();
                    newPart.ItemDetails = null;
                    newPart.Amount = 1;
                    curPrint.requiredItems.Add(newPart);
                }
                if (GUILayout.Button("Add Output Part", GUILayout.ExpandWidth(false)))
                {
                    ItemSlot newPart = ScriptableObject.CreateInstance<ItemSlot>();
                    newPart.ItemDetails = null;
                    newPart.Amount = 1;
                    curPrint.outputItems.Add(newPart);
                }
                GUILayout.EndHorizontal();
                curPrint.printName = EditorGUILayout.TextField("Name", curPrint.printName as string);

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

        if (GUI.changed)
        {
            EditorUtility.SetDirty(blueprintList);
        }
    }

    void AddItem()
    {
        Blueprint newBlueprint = new Blueprint();
        newBlueprint.printName = "New Blueprint";
        blueprintList.blueprintList.Add(newBlueprint);
        viewIndex = blueprintList.blueprintList.Count;
    }

    void DeleteItem(int index)
    {
        blueprintList.blueprintList.RemoveAt(index);
    }

    void CreateNewItemList()
    {
        viewIndex = 1;
        blueprintList = CreateBlueprintList.Create();
        if (blueprintList)
        {
            string path = AssetDatabase.GetAssetPath(blueprintList);
            EditorPrefs.SetString("ObjectPathBlueprint", path);
        }
    }

    void OpenItemList()
    {
        string absPath = EditorUtility.OpenFilePanel("Selecting Blueprint List", "", "");
        if (absPath.StartsWith(Application.dataPath))
        {
            string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
            blueprintList = AssetDatabase.LoadAssetAtPath(relPath, typeof(BlueprintList)) as BlueprintList;

            if (blueprintList)
            {
                EditorPrefs.SetString("ObjectPathBlueprint", relPath);
            }
        }
    }
}
