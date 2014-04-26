using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Resource : MonoBehaviour {

	public List<Item> items = new List<Item>();

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

	public void Gather(GameObject gatherer)
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
}
