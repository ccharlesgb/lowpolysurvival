using System.Collections.Generic;

namespace LowPolySurvival.Inventory
{
	[System.Serializable]
	public class Blueprint
	{
		public string printName;
		public List<ItemSlot> requiredItems;
		public List<ItemSlot> outputItems;
	}
}
