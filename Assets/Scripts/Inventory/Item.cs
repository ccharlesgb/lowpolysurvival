using UnityEngine;
using System.Collections;

[System.Serializable]
public class Item
{
	public string name = "ITEM";
	public bool stackable = true;

	[SerializeField]
	public Color worldColor;

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
public struct ItemHandle
{
	public Item item;
	public int amount; //If the item is NOT stackable can only be 0/1

	public ItemHandle(Item it, int amt)
	{
		item = it;
		amount = amt;
	}

	public bool IsValid() 
	{
		return item != null;
	}

	public static ItemHandle Empty() 
	{
		return new ItemHandle(null, 0);
	}
}