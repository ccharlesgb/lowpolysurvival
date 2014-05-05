using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {

	public List<ItemHandle> items = new List<ItemHandle>();

	void Awake()
	{

	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public ItemHandle FindItem(string name)
	{
		for (int i =0; i < items.Count; i++)
		{
			if (items[i] != null && name == items[i].item.name)
			{
				return items[i];
			}
		}
		return null;
	}
	
	public void AddItem(ItemHandle it)
	{
		ItemHandle testItem = FindItem(it.item.name);
		if (testItem != null)
		{
			testItem.amount += it.amount;
		}
		else
		{
			items.Add (it);
		}
		//Debug.Log ("Added " + it.amount + " " + it.item.name + " to inventory.");
	}
}
