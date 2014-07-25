using System.Collections.Generic;
using UnityEngine;

namespace LowPolySurvival.Inventory.Blueprints
{
	[System.Serializable]
	public class Blueprint
	{
		public string PrintName;
        [SerializeField]
		public List<BlueprintIngredient> RequiredItems;
        [SerializeField]
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
