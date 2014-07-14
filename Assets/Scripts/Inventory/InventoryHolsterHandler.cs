using UnityEngine;

[RequireComponent(typeof(Inventory))]
class InventoryHolsterHandler : MonoBehaviour
{
	private Inventory _inventory;
	public GameObject ActiveItem;

	private void Start()
	{
		_inventory = GetComponent<Inventory>();
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
		Inventory.OnItemRemoved += RemoveCheck;
	}

	private void OnDisable()
	{
		Inventory.OnItemRemoved -= RemoveCheck;
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

		if (!it.Item.isEquipable) return;

		if (ActiveItem != null) //Do we already have a gameobject that is present?
		{
			Destroy(ActiveItem);
			ActiveItem = null;
		}
		//Spawn the new gameobject
		GameObject itemFab = Instantiate(it.Item.itemObject, transform.position, Quaternion.identity) as GameObject;
		itemFab.GetComponent<ItemBehaviour>().Init(it, 1);
		itemFab.GetComponent<ItemBehaviour>().Holster(_inventory);
		ActiveItem = itemFab;
	}

	private void RemoveCheck(ItemContainer it, int amount)
	{
		if (ActiveItem == null) return;

		if (ActiveItem.GetComponent<ItemBehaviour>().container.Item == it.Item)
		{
			Destroy(ActiveItem);
			ActiveItem = null;
		}
	}

}