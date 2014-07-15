
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class RegenInfo
{
	public float regen;
	public float max;
	public float lastRegen;
	public RegenInfo(float r, float m)
	{
		lastRegen = Time.time;
		regen = r; 
		max = m;
	}
}

/// <summary>
///		Component to slowly regenerate an items inventory until it reaches a max value
/// </summary>
public class InventoryRegen : MonoBehaviour 
{

	public Inventory inventory;

	[System.Serializable]
	public class UniDicRegen : UniDictionary<string, RegenInfo>  { }; //Hack to get it to serialize
	public UniDicRegen itemDic = new UniDicRegen();

	// Use this for initialization
	void Start () 
	{
		
	}

	public void SetRegen(string name, float recharge, float max)
	{
		RegenInfo testVal;
		if (!itemDic.TryGetValue(name, out testVal))
		{
			Debug.Log ("Adding " + name);
			itemDic.Add (name, new RegenInfo(recharge, max));
		}
		else
		{
			itemDic[name].regen = recharge;
			itemDic[name].max = max;
		}
	}

	public RegenInfo GetRegen(string name)
	{
		RegenInfo testVal;
		if (itemDic.TryGetValue(name, out testVal))
			return testVal;
		return null;
	}

	public void DeleteRegen(string name)
	{
		if (GetRegen (name) != null)
		{
			itemDic.Remove(name);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (inventory == null)
			enabled = false;

		foreach (KeyValuePair<string, RegenInfo> pair in itemDic)
		{
			ItemSlot slot = inventory.GetSlot(pair.Key);
			if (slot != null)
			{
				if (slot.Amount >= pair.Value.max)
					continue;
			}
			if (pair.Value.lastRegen + (1/pair.Value.regen) < Time.time)
			{
				inventory.AddItem (pair.Key, 1);
				pair.Value.lastRegen = Time.time;
			}
		}
	}
}
