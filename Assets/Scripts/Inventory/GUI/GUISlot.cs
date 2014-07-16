using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class GUISlot : IGUIElement
{
	private Inventory _inventory;
	private int _slotID;
	private IGUIElement _parent;

	private ItemSlot _itemSlot;

	private Rect _rect;

	private GUIPosition _position; // Position relative to parent element.

	public GUISlot(IGUIElement parent, GUIPosition position, Inventory inventory, int slot)
	{
		_parent = parent;
		_inventory = inventory;
		_slotID = slot;
		_position = position;

		_rect = new Rect(0, 0, 50, 50);
	}

	public void Update()
	{
		_itemSlot = _inventory.GetSlot(_slotID);

		// Update position.
		_rect.x = _parent.GetSize().x + _position.x;
		_rect.y = _parent.GetSize().y + _position.y;
	}

	public void Draw()
	{
		if (_itemSlot != null)
		{
			Event e = Event.current;

			if (_rect.Contains(e.mousePosition))
			{
				DrawToolTip(_rect, _itemSlot.ItemDetails.itemName);
			}

			GUI.Button(_rect, _itemSlot.ItemDetails.itemIcon);

			// Draw the stack count.
			if (_itemSlot.ItemDetails.isStackable)
			{
				GUI.Label(_rect, "" + _itemSlot.Amount, "Stacks");
			}
		}
		else
		{
			GUI.Box(_rect, ""+_slotID);
		}
		
	}

	private void DrawToolTip(Rect itemRect, string toolTipText)
	{
		float x = itemRect.x;
		float y = itemRect.y - 15;

		Rect rect = new Rect(_rect.x, _rect.y - 25, _rect.width, 25);

		GUI.Box(rect, toolTipText);
		//GUI.Label(rect, toolTipText, "ToolTip");
	}

	public Rect GetSize()
	{
		return _rect;
	}
}