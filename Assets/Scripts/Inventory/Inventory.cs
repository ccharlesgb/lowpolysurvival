using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public List<ItemSlot> containerList = new List<ItemSlot>();

    public bool IsPickup = false; //Does this inventory support picking up items?
    public float PickupDelay = 0.5f;

    private ItemList _masterList; //Singleton instance of the main list

    private bool _isLooting = false;
    private Inventory _lootInventory = null;

    //public List<ItemSlot> containerList = new List<ItemSlot>();
    public ItemSlot[] Items;


    #region Events

    public delegate void ItemAddedHandler(ItemSlot item, int amount);

    public event ItemAddedHandler OnItemAdded;

    public delegate void ItemRemovedHandler(ItemSlot item, int amount);

    public event ItemRemovedHandler OnItemRemoved;

    public delegate void ItemTransferHandler(ItemSlot item, int amount);

    public event ItemTransferHandler OnTransferItem;

    public delegate void LootHandler(Inventory lootInv);

    public event LootHandler OnLootBegin;

    #endregion

    public int InventoryMaxSize; //MAX number of slots this inventory can hold

    [Flags]
    public enum InventoryOptions
    {

    }

    //Begin Implementation

    private void Awake()
    {
        Items = new ItemSlot[InventoryMaxSize];
        _masterList = MasterList.Instance.itemList;
    }

    //Returns the correct item amount (Stops stacksize getting too high
    public int ClampItemAmount(int amount, ItemDetails item)
    {
        if (!item.isStackable && amount > 1)
            amount = 1;

        if (item.isStackable && amount > item.stackSize)
            amount = item.stackSize;

        return amount;
    }

    //Get the total amount of space left in the inventory
    public int GetSpaceForItem(ItemDetails item)
    {
        int totalSpace = 0;
        for (int i = 0; i < InventoryMaxSize; i++)
        {
            ItemSlot curSlot = Items[i];

            if (curSlot == null) //The slot is empty
            {
                if (item.isStackable)
                    totalSpace += item.stackSize; //We can add a whole stack!
                else
                    totalSpace += 1;
            }
            else if (item.isStackable) //Its not empty but we are stackable
            {
                if (curSlot.ItemDetails == item && curSlot.Amount < item.stackSize)
                    //We found a slot that has a stack of our item in it!
                {
                    totalSpace += item.stackSize - curSlot.Amount;
                }
            }
            else //Its not empty and we are NOT stackable
            {
                totalSpace += 1; //Can only add one
            }
        }

        return totalSpace;
    }

    public int FindFirstSlotWithSpace(ItemDetails item)
    {
        for (int i = 0; i < InventoryMaxSize; i++)
        {
            ItemSlot curSlot = Items[i];
            if (curSlot == null) //The slot is empty
            {
                return i; //This is an empty slot
            }
            else if (item.isStackable) //Its not empty but we are stackable
            {
                if (curSlot.ItemDetails == item && curSlot.Amount < item.stackSize)
                    //We found a slot that has a stack of our item in it!
                {
                    return i;
                }
            }
            else //Its not empty and we are NOT stackable
            {
                return i;
            }
        }
        return -1; //Couldnt find a slot with space for this item
    }

    //Adds an ItemDetails from "Thin air" (Doesnt take from anything else)
    public void AddItem(ItemDetails item, int amount, int slot = -1)
    {
        if (amount <= 0) return; //Use Remove item for this!

        if (slot == -1)
        {
            if (GetSpaceForItem(item) < amount) //Inventory doesnt have enough space!
                return; //We cant add this much 'item'

            int amountLeftAdd = amount;
            Debug.Log("ADDING " + amount + " " + item.itemName);
            //Loop through adding to slots with space until we've added enough
            while (amountLeftAdd > 0)
            {
                int slotSpace = FindFirstSlotWithSpace(item);

                if (Items[slotSpace] == null) //Empty slot
                {
                    var newSlot = ScriptableObject.CreateInstance<ItemSlot>();
                    newSlot.ItemDetails = item;
                    newSlot.Amount = ClampItemAmount(amount, item);
                    amountLeftAdd -= newSlot.Amount;
                    Items[slotSpace] = newSlot;
                }
                else
                {
                    int amountCanAdd = item.stackSize - Items[slotSpace].Amount;
                    Items[slotSpace].Amount += amountCanAdd;
                    amountLeftAdd -= amountCanAdd;
                }
            }
        }
        else //Dont need to check we have enough space as we are just doing a flat out slot replace
        {
            if (Items[slot] != null)
            {
                Debug.LogWarning("Overriding existing slot. Are you sure you meant to do this?");
                Destroy(Items[slot]); //Dont leak memory
            }
            //Do I actually need to make a new instance
            var newSlot = ScriptableObject.CreateInstance<ItemSlot>();
            newSlot.ItemDetails = item;
            newSlot.Amount = ClampItemAmount(amount, item);
            //TODO: should this add N slots worth to ensure we always add 'amount' of things?
            Items[slot] = newSlot;
        }
    }

    //Overload for adding via item name (SLOWER)
    public void AddItem(string name, int amount, int slot = -1)
    {
        ItemDetails itemDetails = _masterList.FindByName(name);
        if (itemDetails == null)
        {
            Debug.Log("ERROR: Tried to add null item " + name);
            return;
        }
        AddItem(itemDetails, amount, slot);
    }

    //Takes an ItemDetails from another inventory. IS THIS THE BEST NAME?
    //Takes an 'amount' of an 'item' from the 'other' inventory
    public void TransferItem(ItemDetails item, int amount, Inventory other)
    {
        if (item == null) return;

        if (other.GetTotalAmount(item) < amount)
        {
            Debug.LogWarning("Other inventory has insusfficient " + item.itemName + " to transfer");
            return;
        }
        //Take from the other inventory!
        other.RemoveItem(item, amount);

        //Add to ours!
        AddItem(item, amount);

        //TODO: THE EVENTS!
    }

    public int GetTotalAmount(ItemDetails item)
    {
        int total = 0;
        for (int i = 0; i < InventoryMaxSize; i++)
        {
            if (Items[i] != null && Items[i].ItemDetails == item)
            {
                total += Items[i].Amount;
            }
        }
        return total;
    }

    public void RemoveItem(int slot)
    {
        Items[slot] = null;
    }

    // Remove amount from a specific slot.
    public void RemoveItem(int slot, int amount)
    {
        ItemSlot item = Items[slot];
        item.Amount -= amount;

        if (item.Amount <= 0)
        {
            RemoveItem(slot);
        }
    }

    public void RemoveItem(string name, int amount)
    {
        ItemDetails itemDetails = _masterList.FindByName(name);
        RemoveItem(itemDetails, amount);
    }

    // Remove amount of a specific ItemDetails type.
    public void RemoveItem(ItemDetails item, int amount)
    {
        if (GetTotalAmount(item) < amount) return; //NOT ENOUGH

        List<ItemSlot> itemSlots = GetSlots(item);
        foreach (ItemSlot i in itemSlots)
        {
            // We can remove all from this stack.
            if (i.Amount > amount)
            {
                RemoveItem(i.SlotID, amount);
                break;
            }

            // Remove what we can from this stack, and continue to next.
            amount -= i.Amount;
            RemoveItem(i.SlotID, i.Amount);

        }
    }

    //Searches through each slot and removes ALL of this type (might be useful?)
    public void RemoveAll(ItemDetails item)
    {
        List<ItemSlot> itemSlots = GetSlots(item);
        foreach (ItemSlot i in itemSlots)
        {
            Items[i.SlotID] = null;
        }
    }

    //USE LINQ? (Its meant to be slower but is this important?)
    public ItemSlot GetSlot(int slot)
    {
        return Items[slot];
    }

    public ItemSlot GetSlot(string name) //Return first (unity convention)
    {
        ItemDetails itemDetails = _masterList.FindByName(name);
        return GetSlot(itemDetails);
    }

    public ItemSlot GetSlot(ItemDetails item)
    {
        return Items.FirstOrDefault(i => item.Equals(i.ItemDetails));
    }

    public List<ItemSlot> GetSlots(string name) //Multiple slots? Return first? al
    {
        ItemDetails itemDetails = _masterList.FindByName(name);
        return GetSlots(itemDetails);
    }

    public List<ItemSlot> GetSlots(ItemDetails item)
    {
        return Items.Where(i => item.Equals(i.ItemDetails)).ToList();
    }

    public ItemSlot[] GetAllSlots() //Dump all slots to an array
    {
        return Items;
    }

    //Returns -1 if nothing found
    private int FindFirstEmptySlot() //You can get the 'int' from the slotID
    {
        return Array.IndexOf(Items, null);
    }

    //Spawns ItemDetails in the world
    public ItemSlot DropItem(int slot, int amount)
    {
        return null;
    }

    public void DropAllItems()
    {
        foreach (ItemSlot slot in Items)
        {
            DropItem(slot.SlotID, slot.Amount);
        }
    }

    //MIGHT HAVE TO MOVE THIS
    //Handles ItemDetails picking up from the collider trigger
    private void OnTriggerStay(Collider other)
    {
        if (!IsPickup) return;
        var behav = other.gameObject.GetComponent<ItemBehaviour>();
        if (behav != null)
        {
            PickupItem(behav);
        }
    }

    private bool CanPickup(ItemBehaviour itemBehave)
    {
        if (itemBehave.spawnTime + PickupDelay > Time.time)
        {
            return false;
        }
        return true;
    }

    public void PickupItem(ItemBehaviour itemBehave)
    {
        if (!CanPickup(itemBehave)) return;
        Debug.Log("PICKUP " + itemBehave.slot.ItemDetails.itemName + " " + itemBehave.slot.Amount);
        AddItem(itemBehave.slot.ItemDetails, itemBehave.slot.Amount);
        Destroy(itemBehave.gameObject);
    }

}

//public SendNotification() // is this needed? should prob belong to AddItem


    //Looting


    //OLD INVENTORY
    /*
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

	public ItemSlot[] GetInventoryAsArray()
	{
		var array = new ItemSlot[NbrSlots];
		foreach (ItemSlot container in containerList)
		{
			array[container.SlotID] = container;
		}
		return array;
	}


	public void AddItem(string name, int amount, int slotID = -1)
	{
		ItemDetails ItemDetails = masterList.FindByName (name);
		if (ItemDetails == null || ItemDetails.itemObject == null) return; //Not a valid ItemDetails

		ItemSlot slot = GetContainer (name);

		if (slot == null)
		{
			if (slotID == -1)
			{
				slotID = FindFirstEmptySlot();
			}

			slot = ScriptableObject.CreateInstance<ItemSlot>();
			slot.ItemDetails = ItemDetails;
			slot.Amount = amount;
			slot.SlotID = slotID;
			containerList.Add (slot);
		}
		else
		{
			slot.Amount += amount;
		}
		SendNotification(slot, amount);
	}

	private int FindFirstEmptySlot()
	{
		int slot = 0;
		for (int i = 0; i < NbrSlots; i++)
		{
			bool found = containerList.Any(container => container != null && container.SlotID == i);

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
	    return containerList.Any(t => t.ItemDetails.itemName == name);
	}

    //Searches for the slot with this name
	public ItemSlot GetContainer(string name)
	{
	    return containerList.FirstOrDefault(t => t.ItemDetails.itemName == name);
	}

	public void SendNotification(ItemSlot it, int amount)
	{
		if (OnItemAdded == null) return;

		OnItemAdded(it.ItemDetails, amount);
	}

    //Give to our inventory and take from the other
	public void TransferItem(ItemSlot other, int amount, Inventory otherInv)
	{
		if (other == null) return;

		//Does the other inventory have enough of this ItemDetails to give it to us?
		if (other.Amount < amount) return; //More validation needed here

		AddItem (other.ItemDetails.itemName, amount);
		otherInv.UseItem(other.ItemDetails.itemName, amount);
	}

	public void DropItem(string name, int amount)
	{
		ItemSlot slot = GetContainer (name);
		if (slot == null) return;
		if (slot.Amount < amount) return;

		if (OnItemRemoved != null)
		{
			OnItemRemoved(slot, amount);
		}

		GameObject itemFab = Instantiate (slot.ItemDetails.itemObject, transform.position, Quaternion.identity) as GameObject;

		itemFab.GetComponent<ItemBehaviour>().Init(slot, amount);

		slot.Amount -= amount;

		//We've run out of this ItemDetails
		if (slot.Amount <= 0)
		{
			RemoveItem(slot);
		}
	}

    //Similar to Drop ItemDetails but the ItemDetails doesnt get spawned it just gets 'used up'
    public void UseItem(string name, int amount)
    {
        Debug.Log("USing up " + name + amount);
        ItemSlot slot = GetContainer(name);
        if (slot == null) return;
        Debug.Log("Not null");
        if (slot.Amount < amount) return;
        Debug.Log("Not enough");
        if (OnItemRemoved != null)
        {
            OnItemRemoved(slot, amount);
        }

        slot.Amount -= amount;

        //We've run out of this ItemDetails
        if (slot.Amount <= 0)
        {
            RemoveItem(slot);
        }

    }
	public void RemoveItem(ItemSlot ct)
	{
		containerList.Remove (ct);
	}
	
    //Handles ItemDetails picking up from the collider trigger
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

		AddItem (itemBehave.slot.ItemDetails.itemName, itemBehave.slot.Amount);
		Destroy(itemBehave.gameObject);
	}
}
*/

