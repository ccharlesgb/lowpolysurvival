using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class GUIDragHandler
{

	// Item currently being dragged.
	public static ItemSlot Item;

	// Item is being dragged?
	public static bool IsActive = false;
	public static Inventory Inventory;

	public static void SetItem(Inventory inventory, ItemSlot item)
	{
		Inventory = inventory;
		Item = item;
		IsActive = true;
	}

	public static void ResetItem()
	{
		Inventory = null;
		Item = null;
		IsActive = false;
	}

}
