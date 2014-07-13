using UnityEngine;

/// <summary>
///		Basic container for InventoryItems, that adds amount and slot.
/// </summary>
public class ItemContainer : ScriptableObject
{
	/// <summary>
	/// Item the container refers to.
	/// </summary>
	public InventoryItem Item;

	/// <summary>
	/// The amount we have of the item.
	/// </summary>
	public int Amount;

	/// <summary>
	/// What "inventory" slot the item is located in.
	/// </summary>
	public int Slot;
}
