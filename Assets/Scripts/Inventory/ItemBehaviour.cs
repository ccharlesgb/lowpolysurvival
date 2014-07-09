using UnityEngine;
using System.Collections;

public class ItemBehaviour : MonoBehaviour 
{
	public ItemContainer container;
	public float spawnTime;

	public bool isHolstering;

	public void Init(ItemContainer ct, int amount)
	{
		if (ct == null) return;
		container = ScriptableObject.CreateInstance <ItemContainer>();
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
