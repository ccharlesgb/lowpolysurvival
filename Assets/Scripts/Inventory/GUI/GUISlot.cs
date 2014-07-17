using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class GUISlot : GUIElement
{
	private Inventory _inventory;
	private int _slotID;

	private ItemSlot _itemSlot;

	public GUISlot(IGUIElement parent, GUIPosition position, Inventory inventory, int slot)
		: base(parent, position)
	{
		_inventory = inventory;
		_slotID = slot;

		WindowRect = new Rect(0, 0, 50, 50);
	}

	public override void Update()
	{
		base.Update();
		_itemSlot = _inventory.GetSlot(_slotID);
	}

	public override void Draw()
	{
		base.Draw();

		if (_itemSlot != null)
		{
			Event e = Event.current;

			if (WindowRect.Contains(e.mousePosition))
			{
				DrawToolTip(WindowRect, _itemSlot.ItemDetails.itemName);
			}

			GUI.Button(WindowRect, _itemSlot.ItemDetails.itemIcon);

			// Draw the stack count.
			if (_itemSlot.ItemDetails.isStackable)
			{
				GUI.Label(WindowRect, "" + _itemSlot.Amount, "Stacks");
			}
		}
		else
		{
			GUI.Box(WindowRect, "" + _slotID);
		}
		
	}

	private void DrawToolTip(Rect itemRect, string toolTipText)
	{
		float x = itemRect.x;
		float y = itemRect.y - 15;

		Rect rect = new Rect(WindowRect.x, WindowRect.y - 25, WindowRect.width, 25);

		GUI.Box(rect, toolTipText);
		//GUI.Label(rect, toolTipText, "ToolTip");
	}
}