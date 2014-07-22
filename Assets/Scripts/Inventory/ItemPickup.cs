using UnityEngine;

namespace LowPolySurvival.Inventory
{
	/// <summary>
	/// Handles picking up items the player sphere with.
	/// </summary>
	class ItemPickup : MonoBehaviour
	{

		public Inventory Inventory;
		public float PickupDelay = 0.5f;

		void Awake()
		{
			// If Inventory is null, attempt to load it from components.
			if (Inventory == null)
			{
				Inventory = GetComponent<Inventory>();
			}
		}

		//Handles ItemDetails picking up from the collider trigger
		private void OnTriggerStay(Collider other)
		{
			// Ensure the item has the component ItemBehaviour.
			var behav = other.gameObject.GetComponent<ItemBehaviour>();
			if (behav != null)
			{
				PickupItem(behav);
			}
		}

		private bool CanPickup(ItemBehaviour itemBehave)
		{
			if (itemBehave.spawnTime + PickupDelay > Time.time)
			{
				return false;
			}
			return true;
		}

		public void PickupItem(ItemBehaviour itemBehave)
		{
			if (!CanPickup(itemBehave)) return;

			Inventory.AddItem(itemBehave.slot.ItemDetails, itemBehave.slot.Amount);
			Destroy(itemBehave.gameObject);
		}

	}
}
