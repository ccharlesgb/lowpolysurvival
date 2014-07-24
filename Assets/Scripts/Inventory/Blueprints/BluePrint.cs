using System.Collections.Generic;

namespace LowPolySurvival.Inventory.Blueprints
{
	[System.Serializable]
	public class Blueprint
	{
		public string PrintName;
		public List<ItemSlot> RequiredItems;
		public List<ItemSlot> OutputItems;
	}
}
