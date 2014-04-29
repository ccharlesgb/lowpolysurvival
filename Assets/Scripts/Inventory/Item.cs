using UnityEngine;
using System.Collections;

[System.Serializable]
public class Item
{
	public string name = "ITEM";
	public bool stackable = true;

	public Item()
	{
		name = "BLANK";
		stackable = true;
	}

	public Item(string nm, bool stack)
	{
		name = nm; 
		stackable = stack;
	}
}

[System.Serializable]
public class ItemHandle
{
	public Item item;
	public int amount; //If the item is NOT stackable can only be 0/1
}