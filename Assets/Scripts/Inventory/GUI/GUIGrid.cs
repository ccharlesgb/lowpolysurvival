using System;
using UnityEngine;

class GUIGrid : IGUIElement
{
	private GUIPosition _position;

	private Inventory _inventory;
	private InventoryGUI _inventoryGUI;

	private int _boxSize = 50;
	private int _boxPadding = 5;

	public int Columns = 5;
	public int Rows;
	private Rect _rect;

	private GUISlot[] _slots;

	public GUIGrid(InventoryGUI inventoryGUI, GUIPosition position, Inventory inventory)
	{
		_inventoryGUI = inventoryGUI;
		_position = position;
		_inventory = inventory;

		var size = _inventory.InventoryMaxSize;

		// Check how many rows we have.
		Rows = (int)Math.Ceiling((decimal)(size / Columns));

		GUIPosition lastPosition = GetSlotPosition(Rows*Columns + Columns-1);
		lastPosition.x += OuterBoxSize();
		lastPosition.y += OuterBoxSize();

		_rect = new Rect(0, 0, lastPosition.x, lastPosition.y);

		// Create the slots.
		_slots = new GUISlot[size];
		for (int i = 0; i < size; i++)
		{
			_slots[i] = new GUISlot(this, GetSlotPosition(i),  inventory, i);
		}
	}

	private GUIPosition GetSlotPosition(int i)
	{
		int row = i/Columns;
		int column = i%Columns;

		return new GUIPosition(OuterBoxSize() * column, OuterBoxSize() * row);
	}

	/// <summary>
	/// Get the box size including the padding.
	/// </summary>
	private int OuterBoxSize()
	{
		return _boxSize + _boxPadding;
	}

	public void Update()
	{
		// Update position.
		_rect.x = _inventoryGUI.GetWindowSize().x + _position.x;
		_rect.y = _inventoryGUI.GetWindowSize().y + _position.y;

		foreach (GUISlot slot in _slots)
		{
			slot.Update();
		}
	}

	public void Draw()
	{
		foreach (GUISlot slot in _slots)
		{
			slot.Draw();
		}
	}

	public Rect GetSize()
	{
		return _rect;
	}
}
