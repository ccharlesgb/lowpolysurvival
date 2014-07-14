using UnityEngine;
using System.Collections;

/// <summary>
///		Behaviour for Items.
/// </summary>
public class ItemBehaviour : MonoBehaviour 
{
	public ItemContainer container;
	public float spawnTime;

	public bool isHolstering;

	//Hack to make it so you can add items in editor
	public string itemName;
	public int startAmount;

	void Start()
	{
		if (container == null)
		{
			InventoryItem item = MasterList.Instance.itemList.FindByName(itemName);
			if (item != null)
			{
				container = ScriptableObject.CreateInstance<ItemContainer>();
				container.Item = item;
                if (!item.isStackable) startAmount = 1;
                if (item.isStackable && startAmount > item.stackSize)
                    startAmount = item.stackSize;

				container.Amount = startAmount;
			}
		}
	}

	public void Init(ItemContainer ct, int amount)
	{
		if (ct == null) return;
		container = ItemContainer.CreateInstance<ItemContainer>();
		container.Item = ct.Item;
		container.Amount = amount;

		spawnTime = Time.time;
	}

	//Set this item to be the active weapon/consumable etc	
	public bool Holster(Inventory inv)
	{
		if (!container.Item.isEquipable) return false;

		isHolstering = true;
		transform.parent = inv.transform;
	    transform.rotation = Quaternion.identity; //NOT WORKING?
        gameObject.GetInterface<IHolster>().OnHolster(inv);

		return true;
	}

	public void UnHolster(Inventory inv)
	{
        gameObject.GetInterface<IHolster>().OnDeHolster(inv);
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
