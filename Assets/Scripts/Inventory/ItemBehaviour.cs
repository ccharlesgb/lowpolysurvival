using UnityEngine;
using System.Collections;

public class ItemBehaviour : MonoBehaviour 
{
	public ItemContainer container;


	public void Init(ItemContainer ct)
	{
		if (ct == null) return;
		container = ScriptableObject.CreateInstance <ItemContainer>();
		container.item = ct.item;
		container.amount = ct.amount;
	}
}
