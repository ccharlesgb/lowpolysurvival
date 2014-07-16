using System;
using Castle.Core.Internal;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Inventory))]
public class InventoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Inventory inv = (Inventory) target;

        GUILayout.Label("Initial Items", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();

        //Add and delete item containers
        if (GUILayout.Button("Add Item"))
        {
            ItemSlot newSlot = ScriptableObject.CreateInstance<ItemSlot>();
            newSlot.ItemDetails = null;
            newSlot.Amount = 0;
            inv.InitialLoadout.initalSlots.Add(newSlot);
        }
        GUI.enabled = inv.InitialLoadout.initalSlots.Count > 0; //Disable if no items
        if (GUILayout.Button("Delete Item"))
        {
            inv.InitialLoadout.initalSlots.RemoveAt(inv.InitialLoadout.initalSlots.Count -1);
        }
        GUI.enabled = true;
        GUILayout.EndHorizontal();

        string[] list = MasterList.Instance.itemList.GetItemNames().ToArray();
        for (int i = 0; i < inv.InitialLoadout.initalSlots.Count; i++)
        {
            GUILayout.BeginHorizontal();
            //Item name dropdown
            ItemSlot curSlot = inv.InitialLoadout.initalSlots[i];
            int selectedItem = 0;
            if (curSlot.ItemDetails != null) //This already has an item
            {
                //Find out which one it is and select it
                selectedItem = Array.FindIndex(list, row => curSlot.ItemDetails.itemName == row);
            }
            selectedItem = EditorGUILayout.Popup("", selectedItem, list, GUILayout.MaxWidth(80.0f));
            curSlot.ItemDetails = MasterList.Instance.itemList.FindByName(list[selectedItem]);

            curSlot.Amount = EditorGUILayout.IntSlider(curSlot.Amount, 0, curSlot.ItemDetails.stackSize);
            GUILayout.EndHorizontal();
        }



        DrawDefaultInspector();

    }
}
