using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///		List of InventoryItems.
/// </summary>
public class ItemList : ScriptableObject
{
	public List<InventoryItem> itemList;

    public List<string> GetItemNames()
    {
        List<string> nameList = new List<string>();
        for (int i = 0; i < itemList.Count; i++)
        {
            nameList.Add(itemList[i].itemName);
        }
        return nameList;
    }

    public InventoryItem FindByName(string name)
	{
		for (int i=0; i < itemList.Count; i++)
		{
			if (itemList[i].itemName == name)
			{
				return itemList[i];
			}
		}
		return null;
	}
}
