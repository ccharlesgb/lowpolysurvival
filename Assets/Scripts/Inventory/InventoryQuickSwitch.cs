using UnityEngine;
using System.Collections;

public class InventoryQuickSwitch : MonoBehaviour
{

	// Inventory to connect to.
	public Inventory Inv;

	// GUI skin to use for inventory.
	public GUISkin GUISkin;

	private Rect _boxSize = new Rect(Screen.width / 2 - 125, Screen.height - 70, 250, 65);

	public float BoxAreaPadding = 5;
	public float BoxPadding = 5;
	public float BoxSize = 50;

	private ItemContainer[] _items;
	private int _activeSlot = 1;
	private ItemContainer _activeItem;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		// Update the box location.
		_boxSize.x = (Screen.width - _boxSize.width)/2;
		_boxSize.y = Screen.height - (_boxSize.height + 5);

		_items = Inv.GetInventoryAsArray();

		for (int i = 1; i <= 5; i++)
		{
			if (_activeSlot != i && Input.GetKeyDown(""+i))
			{
				SetActiveSlot(i);
			}
		}

		// Check if the item in the active slot was changed.
		if (_activeItem != _items[_activeSlot - 1])
		{
			SetActiveSlot(_activeSlot);
		}

	}

	// Note, slot is 1 higher than it should be, (starts at 1 rather than 0).
	private void SetActiveSlot(int slot)
	{
		_activeSlot = slot;

		// Remove the existing item
		Inv.HolsterItem(null);

		// Set the new item.
		_activeItem = _items[slot - 1];

		if (_activeItem != null)
		{
			Inv.HolsterItem(_activeItem);
		}
	}

	void OnGUI()
	{
		GUI.skin = GUISkin;

		GUI.Box(_boxSize, "");

		for (int x = 0; x < 5; x++)
		{
			var rect = new Rect(_boxSize.x + x * (BoxSize), _boxSize.y, BoxSize, _boxSize.height);

			if (x == _activeSlot-1)
			{
				GUI.Box(rect, "" + (x + 1));
			}
			else
			{
				GUI.Box(rect, "" + (x + 1), "NotActiveBox");
			}

			ItemContainer it = _items[x];

			if (it != null)
			{
				GUI.DrawTexture(new Rect(rect.x + 5, rect.y + 20, BoxSize - 10, BoxSize - 10), it.item.itemIcon);
			}

		}

	}
}