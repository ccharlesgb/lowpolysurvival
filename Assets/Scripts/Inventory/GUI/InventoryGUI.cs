using System.Collections.Generic;
using UnityEngine;

internal class InventoryGUI : MonoBehaviour
{
	public GUISkin GUISkin;

	private List<IGUIElement> _elements;
	private Inventory _inventory;

	private bool _renderGUI;
	private GUIGrid _inventoryGrid;

	private void Awake()
	{
		_elements = new List<IGUIElement>();
		_inventory = GetComponent<Inventory>();

		_inventoryGrid = new GUIGrid(this, new GUIPosition(5, 30), _inventory);
		_elements.Add(_inventoryGrid);
	}

	private void Update()
	{
		foreach (IGUIElement element in _elements)
		{
			element.Update();
		}
	}

	private void OnGUI()
	{
		if (_renderGUI == false) return;

		GUI.skin = GUISkin;
		GUI.Window(0, GetWindowSize(), MyWindow, "Inventory");
	}

	private void MyWindow(int id)
	{
		foreach (IGUIElement element in _elements)
		{
			element.Draw();
		}
	}

	public bool RenderGUI()
	{
		return _renderGUI;
	}

	public void Popup()
	{
		InputState.AddMenuLevel(); //Tell the input state that theres a menu open (and cursor is visible)
		Screen.lockCursor = true;
		Screen.lockCursor = false;
		_renderGUI = true;
	}

	public void Hide()
	{
		InputState.LowerMenuLevel(); //Tell the input state that we closed a menu.
		Screen.lockCursor = true;
		_renderGUI = false;
	}

	public Rect GetWindowSize()
	{
		var size = _inventoryGrid.GetSize();

		return new Rect(5, 5, size.width + 15, size.height);
	}
}