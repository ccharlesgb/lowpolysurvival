using System.Collections.Generic;

namespace LowPolySurvival.Inventory.Blueprints
{
	[System.Serializable]
	public class Blueprint
	{
		public string PrintName;
		public List<BlueprintIngredient> RequiredItems;
		public List<BlueprintIngredient> OutputItems;

		/// <summary>
		/// Component required to craft blueprint or provide as reward.
		/// </summary>
		[System.Serializable]
		public class BlueprintIngredient
		{
			public ItemDetails ItemDetails;
			public int Amount;
		}
	}
}
