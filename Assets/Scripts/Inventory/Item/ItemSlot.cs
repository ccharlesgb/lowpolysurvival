using UnityEngine;

/// <summary>
///		Basic slot for InventoryItems, that adds amount and slot.
/// </summary>
public class ItemSlot : ScriptableObject
{
	/// <summary>
	/// ItemDetails the slot refers to.
	/// </summary>
	public ItemDetails ItemDetails;

	/// <summary>
	/// The amount we have of the ItemDetails.
	/// </summary>
	public int Amount;

	/// <summary>
	/// What "inventory" slot the ItemDetails is located in.
	/// </summary>
	public int SlotID;
}
