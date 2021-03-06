﻿using UnityEngine;

namespace LowPolySurvival.Inventory
{
	[RequireComponent(typeof(Inventory))]
	class HolsterHandler : MonoBehaviour
	{
		private Inventory _inventory;
		public GameObject ActiveItem;

		private void Start()
		{
		
		}

		private void Update()
		{
			if (ActiveItem != null)
			{
				if (!Screen.lockCursor) return; //Dont fire when opening menu
				if (Input.GetAxis("Fire1") == 1.0f)
				{
					ActiveItem.GetComponent<ItemBehaviour>().LeftClick(_inventory);
				}
				if (Input.GetAxis("Fire2") == 1.0f)
				{
					ActiveItem.GetComponent<ItemBehaviour>().RightClick(_inventory);
				}
			}
		}

	    private void FixedUpdate()
	    {
	        Quaternion newRot = new Quaternion();
	        newRot.SetLookRotation(Camera.main.transform.forward);
	        transform.rotation = newRot;
	    }

		private void OnEnable()
		{
			_inventory = GetComponent<Inventory>();
			_inventory.OnItemRemoved += RemoveCheck;
		}

		private void OnDisable()
		{
			_inventory.OnItemRemoved -= RemoveCheck;
		}

		public void SetActiveItem(ItemSlot it)
		{
			if (ActiveItem != null) //Do we already have a gameobject that is present?
			{
				ActiveItem.GetComponent<ItemBehaviour>().UnHolster(_inventory);
				Destroy(ActiveItem);
				ActiveItem = null;
			}

            if (it == null)
                return;

			if (!it.ItemDetails.isEquipable) return;

			//Spawn the new gameobject
			GameObject itemFab = Instantiate(it.ItemDetails.itemObject, transform.position, Quaternion.identity) as GameObject;
            //Make it face the same way we are
            itemFab.transform.parent = transform;
            itemFab.transform.localRotation = Quaternion.identity;
			itemFab.GetComponent<ItemBehaviour>().Init(it, 1);
			itemFab.GetComponent<ItemBehaviour>().Holster(_inventory);
			ActiveItem = itemFab;
		}

		private void RemoveCheck(ItemSlot it, int amount)
		{
			if (ActiveItem == null) return;
            
			if (ActiveItem.GetComponent<ItemBehaviour>().slot.ItemDetails == it.ItemDetails && it.Amount == 0)
			{
				ActiveItem.GetComponent<ItemBehaviour>().UnHolster(_inventory);
				Destroy(ActiveItem);
				ActiveItem = null;
			}
		}

	}
}