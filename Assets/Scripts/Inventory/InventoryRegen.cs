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

//Component to slowly regenerate an items inventory until it reaches a max value
public class InventoryRegen : MonoBehaviour {

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
		foreach (KeyValuePair<string, RegenInfo> pair in itemDic)
		{
			ItemHandle hand = inventory.FindItem (pair.Key);
			if (hand == null || hand.amount < pair.Value.max)
			{
				if (pair.Value.lastRegen + (1/pair.Value.regen) < Time.time)
				{
					ItemHandle it = new ItemHandle();
					it.item = ItemList.Instance.GetItem (pair.Key);
					it.amount = 1;
					inventory.AddItem(it);

					pair.Value.lastRegen = Time.time;
				}
			}
		}
	}
}
