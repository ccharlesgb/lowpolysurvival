﻿using UnityEngine;

namespace LowPolySurvival.Inventory
{
	/// <summary>
	///		Behaviour for Items.
	/// </summary>
	public class ItemBehaviour : MonoBehaviour 
	{
		public ItemSlot slot;
		public float spawnTime;

		public bool isHolstering;

		//Hack to make it so you can add items in editor
		public string itemName;
		public int startAmount;

		void Start()
		{
			if (slot == null)
			{
				ItemDetails itemDetails = MasterList.Instance.itemList.FindByName(itemName);
				if (itemDetails != null)
				{
					slot = ScriptableObject.CreateInstance<ItemSlot>();
					slot.ItemDetails = itemDetails;
					if (!itemDetails.isStackable) startAmount = 1;
					if (itemDetails.isStackable && startAmount > itemDetails.stackSize)
						startAmount = itemDetails.stackSize;

					slot.Amount = startAmount;
				}
			}
		}

        //Set up the item slot details
		public void Init(ItemSlot ct, int amount)
		{
			if (ct == null) return;
			slot = ItemSlot.CreateInstance<ItemSlot>();
			slot.ItemDetails = ct.ItemDetails;
			slot.Amount = amount;

			spawnTime = Time.time;
		}

		//Set this ItemDetails to be the active weapon/consumable etc	
		public bool Holster(Inventory inv)
		{
			if (!slot.ItemDetails.isEquipable) return false;

			isHolstering = true;
		    IHolster[] holsterables = gameObject.GetInterfaces<IHolster>();
            foreach (IHolster holster in holsterables)
			    holster.OnHolster(inv);

			return true;
		}

		public void UnHolster(Inventory inv)
		{
            IHolster[] holsterables = gameObject.GetInterfaces<IHolster>();
            foreach (IHolster holster in holsterables)
                holster.OnDeHolster(inv);
			isHolstering = false;
			transform.parent = null;
		}

		public void LeftClick(Inventory owner)
		{
			IHolster[] holsters = gameObject.GetInterfaces<IHolster>();

			foreach(IHolster holster in holsters)
			{
				holster.PrimaryFire (owner);
			}
		}

		public void RightClick(Inventory owner)
		{
			IHolster[] holsters = gameObject.GetInterfaces<IHolster>();
		
			foreach(IHolster holster in holsters)
			{
				holster.SecondaryFire(owner);
			}
		}
	}
}
