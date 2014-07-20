using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine;

namespace LowPolySurvival.Inventory
{ //The initial items in the inventory
	[System.Serializable]
	public class InventoryLoadout
	{
		[SerializeField]
		public List<ItemSlot> initalSlots;
	}

	public class Inventory : MonoBehaviour
	{
		//[HideInInspector]
		[SerializeField]
		public InventoryLoadout InitialLoadout = new InventoryLoadout();

        public enum InventoryFlags
        {
            //None = 0x00,
            IsPlayer = 0x01,
            IsMineable = 0x02,
            //All = 0x0F,
        }
        [BitMask(typeof(InventoryFlags))]
	    public InventoryFlags Flags;

		public int InventoryMaxSize; //MAX number of slots this inventory can hold
		//[HideInInspector]
		public ItemSlot[] Items;

		private bool _isLooting = false;
		private Inventory _lootInventory = null;
		private ItemList _masterList; //Singleton instance of the main list
	

		#region Events

		public delegate void ItemAddedHandler(ItemSlot item, int amount);
		public delegate void ItemRemovedHandler(ItemSlot item, int amount);
		public delegate void ItemTransferHandler(ItemSlot item, int amount);
		public delegate void LootHandler(Inventory otherInventory);

		public event ItemAddedHandler OnItemAdded;
		public event ItemRemovedHandler OnItemRemoved;
		public event ItemTransferHandler OnTransferItem;
		public event LootHandler OnLootBegin;

		#endregion

		/// <summary>
		/// Initialize the inventory, creates the Items array and setups the initial layout.
		/// </summary>
		private void Awake()
		{
			Items = new ItemSlot[InventoryMaxSize];
			foreach (ItemSlot slot in InitialLoadout.initalSlots)
			{
				AddItem(slot.ItemDetails, slot.Amount);
			}
			_masterList = MasterList.Instance.itemList;
		}

		/// <summary>
		/// Helper function, clamps the Amount to the maximum the item allows.
		/// </summary>
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
					// Clamp on item stacksize.
					totalSpace += ClampItemAmount(item.stackSize, item);
				}
				else if (item.isStackable) //Its not empty but we are stackable
				{
					if (curSlot.ItemDetails.Equals(item) && curSlot.Amount < item.stackSize)
						//We found a slot that has a stack of our item in it!
					{
						totalSpace += item.stackSize - curSlot.Amount;
					}
				}
			}

			return totalSpace;
		}

		public int FindFirstSlotWithSpace(ItemDetails item)
		{
			for (int i = 0; i < InventoryMaxSize; i++)
			{
				ItemSlot curSlot = Items[i];

				if (curSlot == null) // The slot is empty
				{
					return i; // This is an empty slot
				}

				if (item.isStackable) // It's not empty but we are stackable
				{
					if (curSlot.ItemDetails.Equals(item) && curSlot.Amount < item.stackSize)
						// We found a slot that has a stack of our item in it!
					{
						return i;
					}
				}
			}
			return -1; // Couldnt find a slot with space for this item
		}

		public int FindBestSlotWithSpace(ItemDetails item)
		{
			int firstEmpty = -1;
			for (int i = 0; i < InventoryMaxSize; i++)
			{
				ItemSlot curSlot = Items[i];

				if (curSlot == null) // The slot is empty
				{
					if (item.isStackable && firstEmpty == -1) //We might find a free stack later on
					{
						firstEmpty = i;
					}
					else if (!item.isStackable)
						return i; // This is an empty slot
				}
				else if (item.isStackable) // It's not empty but we are stackable
				{
					if (curSlot.ItemDetails.Equals(item) && curSlot.Amount < item.stackSize)
						// We found a slot that has a stack of our item in it!
					{
						return i;
					}
				}
			}
			return firstEmpty; // Couldnt find a slot with space for this item
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
				//Debug.Log("ADDING " + amount + " " + item.itemName);
				//Loop through adding to slots with space until we've added enough
				while (amountLeftAdd > 0)
				{
					int slotSpace = FindBestSlotWithSpace(item);
					if (Items[slotSpace] == null) //Empty slot
					{
						var newSlot = ScriptableObject.CreateInstance<ItemSlot>();
						newSlot.ItemDetails = item;
						newSlot.Amount = ClampItemAmount(amount, item);
						newSlot.SlotID = slotSpace;
						amountLeftAdd -= newSlot.Amount;
						Items[slotSpace] = newSlot;
					}
					else
					{
						int amountCanAdd = item.stackSize - Items[slotSpace].Amount;
						Items[slotSpace].Amount += Mathf.Min(amountCanAdd, amount);
						amountLeftAdd -= Mathf.Min(amountCanAdd, amount);
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
				newSlot.SlotID = slot;
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

		public void MoveToSlot(ItemSlot slot, int newSlotID)
		{
			if (Items[slot.SlotID] != slot) //Sanity check (IT SHOULD!)
			{
				Debug.LogWarning("SlotIDS seem to be out of sync!");
				return;
			}
			Items[newSlotID] = slot; //Swap them over
			Items[slot.SlotID] = null; //Remove old reference
			Items[newSlotID].SlotID = newSlotID; //Update ID
		}

		public void SwapSlots(ItemSlot first, ItemSlot second)
		{
			// Store the old positions.
			int oldFirstSlotID = first.SlotID;
			int oldSecoundSlotID = second.SlotID;

			first.SlotID = oldSecoundSlotID;
			second.SlotID = oldFirstSlotID;

			Items[oldFirstSlotID] = second;
			Items[oldSecoundSlotID] = first;
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
			Debug.Log(other + " OTHER");
			Debug.Log(item + " ITEM");
			other.RemoveItem(item, amount);
			//Add to ours!
			AddItem(item, amount);

			//TODO: THE EVENTS!
		}

		/// <summary>
		/// Transfers the itemslot from another inventory to this.
		/// </summary>
		/// <param name="itemSlot">The item to transfer.</param>
		/// <param name="sourceInventory">The destination inventory.</param>
		/// <param name="destinationSlotID">The destination slotID.</param>
		public void TransferItem(ItemSlot itemSlot, Inventory sourceInventory, int destinationSlotID)
		{
			Debug.Log("Inventory: Transfering item: '" + itemSlot + "' from '" + sourceInventory + "' in slot '" + destinationSlotID + "'");

			ItemSlot thisItemSlot = GetSlot(destinationSlotID);
			if (thisItemSlot == null) // No item in the spot we want to move to.
			{
				// Remove the item from the source.
				sourceInventory.Items[itemSlot.SlotID] = null;
			}
			else
			{
				// Move the this inventory item to source.
				sourceInventory.Items[itemSlot.SlotID] = thisItemSlot;
				thisItemSlot.SlotID = itemSlot.SlotID;
			}

			// Add the item in the other inventory.
			Items[destinationSlotID] = itemSlot;
			Items[destinationSlotID].SlotID = destinationSlotID;
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
			for (int i = 0; i < Items.Count(); i++)
			{
				if (Items[i] != null)
					if (Items[i].ItemDetails.Equals(item))
						return Items[i];
			}
			return null;
		}

		public List<ItemSlot> GetSlots(string name) //Multiple slots? Return first? al
		{
			ItemDetails itemDetails = _masterList.FindByName(name);
			return GetSlots(itemDetails);
		}

		public List<ItemSlot> GetSlots(ItemDetails item)
		{
			List<ItemSlot> list = new List<ItemSlot>();
			for (int i = 0; i < Items.Count(); i++)
			{
				if (Items[i] != null && Items[i].ItemDetails == item)
				{
					list.Add(Items[i]);
				}
			}
			return list;
		}

		public ItemSlot[] GetAllSlots() //Dump all slots to an array
		{
			return Items;
		}

		/// <summary>
		/// Drop all of a single item.
		/// </summary>
		/// <param name="slotID">The ID of the slot to drop.</param>
		/// <returns>GameObject</returns>
		public GameObject DropItem(int slotID)
		{
			if (Items[slotID] == null) return null;

			return DropItem(slotID, Items[slotID].Amount);
		}

		//Spawns ItemDetails in the world
		/// <summary>
		/// Drop an amount of a single item as a new GameObject in the world.
		/// </summary>
		/// <param name="slotID">The ID of the slot to drop.</param>
		/// <param name="amount">The amount to drop.</param>
		/// <returns>The GameObject of the item droped.</returns>
		public GameObject DropItem(int slotID, int amount)
		{
			ItemSlot slot = Items[slotID];

			if (slot == null) return null;
			if (slot.Amount < amount) return null;

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
				RemoveItem(slotID);
			}

			return itemFab;
		}

		/// <summary>
		/// Drop all items in the inventory, useful for object death.
		/// </summary>
		public void DropAllItems()
		{
			foreach (ItemSlot slot in Items)
			{
				DropItem(slot.SlotID, slot.Amount);
			}
		}
		
		/// <summary>
		///		Start inventory looting.
		/// </summary>
		/// <param name="other">Inventory to loot.</param>
		public void BeginLooting(Inventory other)
		{
			_lootInventory = other;
			OnLootBegin(_lootInventory);
		}
	}
}