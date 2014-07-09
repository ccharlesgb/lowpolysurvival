using UnityEngine;
using System.Collections;

public class ItemBehaviour : MonoBehaviour 
{
	public ItemContainer container;
	public float spawnTime;

	public void Init(ItemContainer ct, int amount)
	{
		if (ct == null) return;
		container = ScriptableObject.CreateInstance <ItemContainer>();
		container.item = ct.item;
		container.amount = amount;

		spawnTime = Time.time;
	}
}
