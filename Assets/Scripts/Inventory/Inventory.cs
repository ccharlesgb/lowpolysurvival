using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
	public static int NbrSlots = 25;

	// We need an event for communicating with the GUI.
	public delegate void ItemAddedHandler(InventoryItem container, int amount);
	public event ItemAddedHandler OnItemAdded;

	public delegate void ItemRemovedHandler(ItemContainer container, int amount);
	public event ItemRemovedHandler OnItemRemoved;

    public delegate void LootHandler(Inventory lootInv);
    public event LootHandler OnLootBegin;

	public List<ItemContainer> containerList = new List<ItemContainer>();

	public bool canPickup = false; //Does this inventory support picking up items?
	public float pickupDelay = 0.5f;

	public ItemList masterList; //Singleton instance of the main list

    private bool isLooting = false;
    private Inventory lootInventory = null;

    //Start looting a specific inventory
    public void BeginLooting(Inventory inv)
    {
        Debug.Log("BEGIN LOOTING " + inv);
        lootInventory = inv;
        isLooting = true;
        if (OnLootBegin == null) return; //WRONG: Always null?
        OnLootBegin(inv);
    }

    public void StopLooting()
    {
        lootInventory = null;
        isLooting = false;
    }

	void Awake()
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
			bool found = containerList.Any(container => container != null && container.Slot == i);

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
	    return containerList.Any(t => t.Item.itemName == name);
	}

    //Searches for the container with this name
	public ItemContainer GetContainer(string name)
	{
	    return containerList.FirstOrDefault(t => t.Item.itemName == name);
	}

	public void SendNotification(ItemContainer it, int amount)
	{
		if (OnItemAdded == null) return;

		OnItemAdded(it.Item, amount);
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

		if (OnItemRemoved != null)
		{
			OnItemRemoved(container, amount);
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

    //Similar to Drop Item but the item doesnt get spawned it just gets 'used up'
    public void UseItem(string name, int amount)
    {
        ItemContainer container = GetContainer(name);
        if (container == null) return;
        if (container.Amount < amount) return;

        if (OnItemRemoved != null)
        {
            OnItemRemoved(container, amount);
        }

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
	
    //Handles item picking up from the collider trigger
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

}
