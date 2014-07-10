using UnityEngine;
using System.Collections;

public class ItemBehaviour : MonoBehaviour 
{
	public ItemContainer container;
	public float spawnTime;

	public bool isHolstering;

	//Hack to make it so you can add items in editor
	public string itemName;
	public int amount;

	void Start()
	{
		if (container == null)
		{
			InventoryItem item = MasterList.Instance.itemList.FindByName(itemName);
			if (item != null)
			{
				container = ScriptableObject.CreateInstance<ItemContainer>();
				container.item = item;
				if (!item.isStackable) amount = 1;
				if (item.isStackable && amount > item.stackSize)
					amount = item.stackSize;

				container.amount = amount;
			}
		}
	}

	public void Init(ItemContainer ct, int amount)
	{
		if (ct == null) return;
		container = new ItemContainer();
		container.item = ct.item;
		container.amount = amount;

		spawnTime = Time.time;
	}

	//Set this item to be the active weapon/consumable etc	
	public bool Holster(Inventory inv)
	{
		if (!container.item.isEquipable) return false;

		isHolstering = true;
		transform.parent = inv.transform;

		return true;
	}

	public void UnHolster()
	{
		isHolstering = false;
		transform.parent = null;
	}
}
