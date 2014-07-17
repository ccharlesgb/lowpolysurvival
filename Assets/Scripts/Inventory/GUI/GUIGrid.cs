using System;
using UnityEngine;

class GUIGrid : GUIElement
{
	private Inventory _inventory;

	private int _boxSize = 50;
	private int _boxPadding = 5;

	public int Columns = 5;
	public int Rows;
	private Rect _rect;

	private GUISlot[] _slots;

	public GUIGrid(IGUIElement parentElement, GUIPosition position, Inventory inventory)
		: base(parentElement, position)
	{
		_inventory = inventory;

		var size = _inventory.InventoryMaxSize;

		// Check how many rows we have.
		Rows = (int)Math.Ceiling((decimal)(size / Columns));

		// Get the position of 
		GUIPosition lastPosition = GetSlotPosition(Rows*Columns - 1);
		lastPosition.x += OuterBoxSize();
		lastPosition.y += OuterBoxSize();

		WindowRect = new Rect(0, 0, lastPosition.x, lastPosition.y);

		// Create the slots.
		_slots = new GUISlot[size];
		for (int i = 0; i < size; i++)
		{
			_slots[i] = new GUISlot(this, GetSlotPosition(i),  inventory, i);
			Elements.Add(_slots[i]);
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

	public override void Update()
	{
		base.Update();
	}

	public override void Draw()
	{
		base.Draw();
	}

}
