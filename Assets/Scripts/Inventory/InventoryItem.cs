using UnityEngine;
using System.Collections;

[System.Serializable]
public class InventoryItem
{
	public string itemName = "ItemName"; //Name of the item
	public GameObject itemObject = null; //Prefab pointing 
	public Texture2D itemIcon = null;
	public bool isStackable = false;
}
