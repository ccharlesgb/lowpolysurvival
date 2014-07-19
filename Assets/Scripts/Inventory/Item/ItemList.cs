using System.Collections.Generic;
using UnityEngine;

namespace LowPolySurvival.Inventory
{
	/// <summary>
	///		List of InventoryItems.
	/// </summary>
	public class ItemList : ScriptableObject
	{
		public List<ItemDetails> itemList;

		public List<string> GetItemNames()
		{
			List<string> nameList = new List<string>();
			for (int i = 0; i < itemList.Count; i++)
			{
				nameList.Add(itemList[i].itemName);
			}
			return nameList;
		}

		public ItemDetails FindByName(string name)
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
}
