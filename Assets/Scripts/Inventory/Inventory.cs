using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static int NbrSlots = 25;

    public List<ItemSlot> containerList = new List<ItemSlot>();

    public bool canPickup = false; //Does this inventory support picking up items?
    public float pickupDelay = 0.5f;

    private ItemList masterList; //Singleton instance of the main list

    private bool isLooting = false;
    private Inventory lootInventory = null;

    //public List<ItemSlot> containerList = new List<ItemSlot>();
    public ItemSlot[] Items;

      
    #region Events
    public delegate void ItemAddedHandler(ItemSlot item, int amount);
    public event ItemAddedHandler OnItemAdded;
  
    public delegate void ItemRemovedHandler(ItemSlot item, int amount);
    public event ItemRemovedHandler OnRemoveItem;
  
    public delegate void ItemTransferHandler(ItemSlot item, int amount);
    public event ItemTransferHandler OnTransferItem;

    public delegate void LootHandler(Inventory lootInv);
    public event LootHandler OnLootBegin;
    #endregion

    public int InventoryMaxSize; //MAX number of slots this inventory can hold
  
    [Flags]
    public enum InventoryOptions {
    
    }

    //Begin Implementation

    void Awake()
    {
        masterList = MasterList.Instance.itemList;
    }
 
    //Adds an ItemDetails from "Thin air" (Doesnt take from anything else)
    public void AddItem(ItemDetails item, int amount, int slot = -1)
    {
        
    }

    public void AddItem(string name, int amount, int slot = -1)
    {
        ItemDetails itemDetails = masterList.FindByName(name);
        if (itemDetails == null)
        {
            Debug.Log("ERROR: Tried to add null item " + name);
            return;
        }
        AddItem(itemDetails, amount, slot);
    }
  
    //Takes an ItemDetails from another inventory. IS THIS THE BEST NAME?
    public void TransferItem()
    {
        
    }
  
    // Remove amount from a specific slot.
    public void RemoveItem(int slot, int amount)
    {
        
    }
    public void RemoveItem(string name, int amount)
    {
        ItemDetails itemDetails = masterList.FindByName(name);
        RemoveItem(itemDetails, amount);
    }
    // Remove amount of a specific ItemDetails type.
    public void RemoveItem(ItemDetails item, int amount)
    {
        
    }
  
    //Searches through each slot and removes ALL of this type (might be useful?)
    public void RemoveAll(ItemSlot item)
    {
        
    }
  
    //USE LINQ? (Its meant to be slower but is this important?)
    public ItemSlot GetSlot(int slot)
    {
        
    }

    public ItemSlot GetSlot(string name) //Return first (unity convention)
    {
        
    }

    public ItemSlot GetSlot(ItemSlot item)
    {
        
    }

    public ItemSlot[] GetSlots(string name) //Multiple slots? Return first? al
    {
        
    }

    public ItemSlot[] GetSlots(ItemSlot item)
    {
        
    }

    public ItemSlot[] GetAllSlots() //Dump all slots to an array
    {
        
    }
  
    //
    private ItemSlot FindFirstEmptySlot() //You can get the 'int' from the slotID
    {
        
    }

    //Spawns ItemDetails in the world
    public ItemSlot DropItem(int slot, int amount)
    {
        
    }
    public void DropAllItems() 
    {
    // Loop all items and call drop.
    }
 
    //We dont nevessarily need this. Can probably be in a PlayerPickup component
    public void PickupItem(ItemBehaviour itemBehave)
    {
        
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


}
