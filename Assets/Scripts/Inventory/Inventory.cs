using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour 
{
	public List<ItemContainer> containerList = new List<ItemContainer>();

	public InventoryGUI inventoryGUI;

	public float pickupDelay = 0.5f;

	public ItemList masterList;
	// Use this for initialization
	void Start () 
	{
		masterList = MasterList.Instance.itemList;

		AddItem("Wood", 5);
		AddItem("Stone", 10);
	}

	public void AddItem(string name, int amount, int slot = -1)
	{
		InventoryItem item = masterList.FindByName (name);
		if (item == null || item.itemObject == null) return; //Not a valid item

		ItemContainer container = GetContainer (name);

		if (container == null)
		{
			if (slot == -1)
			{
				slot = FindFirstEmptySlot();
			}

			container = ScriptableObject.CreateInstance<ItemContainer>();
			container.item = item;
			container.amount = amount;
			container.slot = slot;
			containerList.Add (container);
		}
		else
		{
			container.amount += amount;
		}
	}

	private int FindFirstEmptySlot()
	{
		int slot = 0;
		for (int i = 0; i < 30; i++)
		{
			bool found = false;
			
			foreach (ItemContainer container in containerList)
			{
				if (container.slot == i)
				{
					found = true;
					break;
				}
			}

			if (!found)
			{
				slot = i;
				break;
			}
		}
			
		return slot;
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
		if (containerList.Count == 0) return null;
		for (int i=0; i < containerList.Count; i++)
		{
			if (containerList[i].item.itemName == name)
			{
				return containerList[i];
			}
		}
		return null;
	}

	public void SendNotification(ItemContainer it, int amount)
	{
		if (inventoryGUI == null) return;

		inventoryGUI.AddNotification (it.item, amount);
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

		SendNotification(container, amount);
	}

	public void DropItem(string name, int amount)
	{
		ItemContainer container = GetContainer (name);
		if (container == null) return;
		if (container.amount < amount) return;

		GameObject itemFab = Instantiate (container.item.itemObject, transform.position, Quaternion.identity) as GameObject;

		itemFab.GetComponent<ItemBehaviour>().Init(container, amount);

		container.amount -= amount;

		//We've run out of this item
		if (container.amount <= 0)
		{
			RemoveItem(container);
		}
	}

	public void RemoveItem(ItemContainer ct)
	{
		containerList.Remove (ct);
	}
	
	void OnTriggerStay(Collider other)
	{
		ItemBehaviour behav = other.gameObject.GetComponent<ItemBehaviour>();
		if (behav != null)
		{
			PickupItem(behav);
		}
	}

	bool CanPickup(ItemBehaviour itemBehave)
	{
		if (itemBehave.spawnTime + pickupDelay > Time.time)
		{
			return false;
		}
		return true;
	}

	public void PickupItem(ItemBehaviour itemBehave)
	{
		if (!CanPickup (itemBehave)) return;

		AddItem (itemBehave.container.item.itemName, itemBehave.container.amount);
		SendNotification(itemBehave.container, itemBehave.container.amount);
		Destroy(itemBehave.gameObject);
	}
}
