using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour 
{
	public List<ItemContainer> containerList = new List<ItemContainer>();

	public ItemList masterList;
	// Use this for initialization
	void Start () 
	{
		AddItem ("Wood", 10);
	}

	void AddItem(string name, int amount)
	{
		InventoryItem item = masterList.FindByName (name);
		if (item == null || item.itemObject == null) return; //Not a valid item
		GameObject itemFab = Instantiate (item.itemObject, Vector3.zero, Quaternion.identity) as GameObject;
		itemFab.transform.parent = transform;

		ItemContainer container = itemFab.GetComponent<ItemContainer>();
		container.item = item;
		container.amount = amount;
		containerList.Add (container);
	}

	public bool HasItem(string name)
	{
		for (int i=0; i < containerList.Count; i++)
		{
			if (containerList[i].item.itemName == name)
			{
				return true;
			}
		}
		return false;
	}

	public ItemContainer GetContainer(string name)
	{
		for (int i=0; i < containerList.Count; i++)
		{
			if (containerList[i].item.itemName == name)
			{
				return containerList[i];
			}
		}
		return null;
	}

	public void TransferItem(ItemContainer other, int amount)
	{
		if (other == null) return;

		//Does the other inventory have enough of this item to give it to us?
		if (other.amount < amount) return; //More validation needed here

		ItemContainer container = GetContainer (other.item.itemName);

		if (container == null)
		{
			AddItem (other.item.itemName, amount);
		}
		other.amount -= amount;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
