using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {

	public List<ItemHandle> items = new List<ItemHandle>();
	public InventoryGUI gui;
	public GameObject worldItemFab;
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
			if (items[i].IsValid () && name == items[i].item.name)
			{
				return items[i];
			}
		}
		return ItemHandle.Empty ();
	}
	
	public void AddItem(ItemHandle it)
	{
		ItemHandle testItem = FindItem(it.item.name);
		if (testItem.IsValid ())
		{
			testItem.amount += it.amount;
		}
		else
		{
			items.Add (it);
		}
		//Display a notification telling the player he has a new item
		if (gui != null)
			gui.AddNotification (it);
	}

	public void DropItem(ItemHandle it)
	{
		ItemHandle testItem = FindItem (it.item.name);
		if (testItem.IsValid())
			return; //Can't drop an item we dont have

		if (testItem.amount <= 0)
			return;

		testItem.amount--;

		CreateWorldItem(testItem);
	}

	public void CreateWorldItem(ItemHandle it)
	{
		Vector3 spawnPos = transform.position;
		GameObject cube = Instantiate(worldItemFab, spawnPos, Quaternion.identity) as GameObject;
		cube.GetComponent<WorldItem>().OnDropped (it);
	}
}
