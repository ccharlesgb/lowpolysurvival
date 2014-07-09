
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ItemList : Singleton<ItemList>
{
	public List<Item> items = new List<Item>();

	void Awake()
	{
	}

	void LoadItems()
	{
		//Add default items for now
		/*Item it = new Item();
		it.name = "Wood";
		it.stackable = true;
		AddItem (it);

		it = new Item();
		it.name = "Stone";
		it.stackable = true;
		
		AddItem (it);*/
	}

	void AddItem(Item item)
	{
		Debug.Log ("TRY ITM");
		if (item != null)
		{
			Debug.Log ("Added " + item.name);
			items.Add (item);
		}
	}

	public Item GetItem(string name)
	{
		for (int i=0; i<items.Count; i++)
		{
			if (name == items[i].name)
				return items[i];
		}
		return null;
	}

	void ClearItems()
	{
		for (int i = 0; i < items.Count; i++)
		{
			items[i] = null;
		}
		items.Clear();
	}
}