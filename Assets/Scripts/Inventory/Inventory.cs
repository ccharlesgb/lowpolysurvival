using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {

	public List<Item> items;

	void Awake()
	{
		items = new List<Item>();
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public Item FindItems(string id)
	{
		for (int i =0; i < items.Count; i++)
		{
			if (items[i] != null && items[i].ID == id)
			{
				return items[i];
			}
		}
		return null;
	}
	
	public void AddItem(Item it, float amount)
	{
		Item testItem = FindItems(it.ID);
		if (testItem != null)
		{
			testItem.amount += amount;
		}
		else
		{
			Item newItem = new Item();
			newItem.name = it.name;
			newItem.ID = it.ID;
			newItem.amount = amount;
			items.Add (newItem);
		}
		Debug.Log ("Added " + it.amount + " " + it.name + " to inventory.");
	}
}
