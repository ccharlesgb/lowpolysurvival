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
			GUI.Button(_rect, _itemSlot.ItemDetails.itemIcon);
		}
		else
		{
			GUI.Box(_rect, ""+_slotID);
		}
		
	}

	public Rect GetSize()
	{
		return _rect;
	}
}