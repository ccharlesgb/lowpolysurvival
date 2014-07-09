using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemList : ScriptableObject
{
	public List<InventoryItem> itemList;

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
