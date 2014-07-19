using UnityEngine;

namespace LowPolySurvival.Inventory
{
	/// <summary>
	///		Container for items.
	/// </summary>
	[System.Serializable]
	public class ItemDetails
	{
		public string itemName = "ItemName"; //Name of the ItemDetails
		public GameObject itemObject = null; //Prefab pointing 
		public Texture2D itemIcon = null;
		public bool isStackable = false;
		public int stackSize = 1;

		public bool isEquipable = true;

		public override bool Equals(object obj)
		{
			ItemDetails other = (ItemDetails) obj;
			return itemName == other.itemName;
		}
	}
}
