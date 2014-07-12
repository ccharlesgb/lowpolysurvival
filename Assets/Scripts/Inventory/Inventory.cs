using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
	public static int NbrSlots = 25;

	public List<ItemContainer> containerList = new List<ItemContainer>();

	public InventoryGUI inventoryGUI;

	public bool canPickup = false;
	public float pickupDelay = 0.5f;

	public GameObject holstered = null;

	public ItemList masterList;
	// Use this for initialization
	void Start ()
	{
		masterList = MasterList.Instance.itemList;
	}

	public ItemContainer[] GetInventoryAsArray()
	{
		var array = new ItemContainer[NbrSlots];
		foreach (ItemContainer container in containerList)
		{
			array[container.Slot] = container;
		}
		return array;
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
			container.Item = item;
			container.Amount = amount;
			container.Slot = slot;
			containerList.Add (container);
		}
		else
		{
			container.Amount += amount;
		}
		SendNotification(container, amount);
	}

	private int FindFirstEmptySlot()
	{
		int slot = 0;
		for (int i = 0; i < NbrSlots; i++)
		{
			bool found = false;
			
			foreach (ItemContainer container in containerList)
			{
				if (container.Slot == i)
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
			if (containerList[i].Item.itemName == name)
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
			if (containerList[i].Item.itemName == name)
			{
				return containerList[i];
			}
		}
		return null;
	}

	public void SendNotification(ItemContainer it, int amount)
	{
		if (inventoryGUI == null) return;

		inventoryGUI.AddNotification (it.Item, amount);
	}

	public void TransferItem(ItemContainer other, int amount)
	{
		if (other == null) return;

		//Does the other inventory have enough of this item to give it to us?
		if (other.Amount < amount) return; //More validation needed here

		AddItem (other.Item.itemName, amount);
		other.Amount -= amount;
	}

	public void DropItem(string name, int amount)
	{
		ItemContainer container = GetContainer (name);
		if (container == null) return;
		if (container.Amount < amount) return;

		if (holstered != null && holstered.GetComponent<ItemBehaviour>().container.Item == container.Item)
		{
			Destroy (holstered);
			holstered = null;
		}

		GameObject itemFab = Instantiate (container.Item.itemObject, transform.position, Quaternion.identity) as GameObject;

		itemFab.GetComponent<ItemBehaviour>().Init(container, amount);

		container.Amount -= amount;

		//We've run out of this item
		if (container.Amount <= 0)
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
		if (!canPickup) return;
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

		AddItem (itemBehave.container.Item.itemName, itemBehave.container.Amount);
		Destroy(itemBehave.gameObject);
	}

	public void HolsterItem(ItemContainer it)
	{
		if (it == null && holstered != null) //Unholster
		{
			Destroy (holstered);
			return;
		}
		else if (it == null)
		{
			return;
		}

		if (!it.Item.isEquipable) return;

		if (holstered != null) //Do we already have a gameobject that is present?
		{
			Destroy (holstered);
			holstered = null;
		}
		//Spawn the new gameobject
		GameObject itemFab = Instantiate (it.Item.itemObject, transform.position, Quaternion.identity) as GameObject;
		itemFab.GetComponent<ItemBehaviour>().Init(it, 1);
		itemFab.GetComponent<ItemBehaviour>().Holster (this);
		holstered = itemFab;
	}

	void Update()
	{
		if (holstered != null)
		{
			if (Input.GetAxis ("Fire1") == 1.0f)
			{
				holstered.GetComponent<ItemBehaviour>().LeftClick(this);
			}
			if (Input.GetAxis ("Fire2") == 1.0f)
			{
				holstered.GetComponent<ItemBehaviour>().RightClick(this);
			}
		}
	}
}
