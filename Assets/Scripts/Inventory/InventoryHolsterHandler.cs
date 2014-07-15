using UnityEngine;

[RequireComponent(typeof(Inventory))]
class InventoryHolsterHandler : MonoBehaviour
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

	private void OnEnable()
	{
		_inventory = GetComponent<Inventory>();
		_inventory.OnItemRemoved += RemoveCheck;
	}

	private void OnDisable()
	{
		_inventory.OnItemRemoved -= RemoveCheck;
	}

	public void SetActiveItem(ItemContainer it)
	{
		if (it == null && ActiveItem != null) //Unholster
		{
			Destroy(ActiveItem);
			return;
		}
		if (it == null)
		{
			return;
		}


		if (ActiveItem != null) //Do we already have a gameobject that is present?
		{
            ActiveItem.GetComponent<ItemBehaviour>().UnHolster(_inventory);
			Destroy(ActiveItem);
			ActiveItem = null;
		}

        if (!it.Item.isEquipable) return;

		//Spawn the new gameobject
		GameObject itemFab = Instantiate(it.Item.itemObject, transform.position, Quaternion.identity) as GameObject;
		itemFab.GetComponent<ItemBehaviour>().Init(it, 1);
		itemFab.GetComponent<ItemBehaviour>().Holster(_inventory);
		ActiveItem = itemFab;
	}

	private void RemoveCheck(ItemContainer it, int amount)
	{
		if (ActiveItem == null) return;
            
		if (ActiveItem.GetComponent<ItemBehaviour>().container.Item == it.Item && it.Amount == 0)
		{
            ActiveItem.GetComponent<ItemBehaviour>().UnHolster(_inventory);
			Destroy(ActiveItem);
			ActiveItem = null;
		}
	}

}