using System.Collections.Generic;
using UnityEngine;

namespace LowPolySurvival.Inventory
{
	internal class InventoryGUI : MonoBehaviour, IGUIElement
	{
		public GUISkin GUISkin;

		private List<IGUIElement> _elements;
		private Inventory _inventory;
		private Inventory _lootInv;

		private bool _renderGUI;
		private GUIGrid _inventoryGrid;
		private GUIGrid _inventoryGridLoot;

		private void Awake()
		{
			_elements = new List<IGUIElement>();
			_inventory = GetComponent<Inventory>();

			_inventoryGrid = new GUIGrid(this, new GUIPosition(5, 30), _inventory);
			_elements.Add(_inventoryGrid);
		}

		private void OnEnable()
		{
			_inventory.OnLootBegin += OnLootBegin;
		}

		private void OnDisable()
		{
			_inventory.OnLootBegin -= OnLootBegin;			
		}

		public void OnLootBegin(Inventory other)
		{
			if (_lootInv != null && _lootInv == other) return; //Same inventory!
			_lootInv = other;
			Debug.Log("Start Looting");

			_inventoryGridLoot = new GUIGrid(this, new GUIPosition((int)_inventoryGrid.GetWindowSize().width + 5, 30), other);
			_elements.Add(_inventoryGridLoot);

			Popup();
		}

		public void Update()
		{
			foreach (IGUIElement element in _elements)
			{
				element.Update();
			}

			var v = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

			// Drop outside the window?
			if (GUIDragHandler.IsActive && Input.GetMouseButtonUp(0) && !GetWindowSize().Contains(v))
			{
				GUIDragHandler.Inventory.DropItem(GUIDragHandler.Item.SlotID);
				GUIDragHandler.ResetItem();
			}
		}

		public void Draw()
		{
			throw new System.NotImplementedException();
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

			if (GUIDragHandler.IsActive && Event.current.type == EventType.Repaint)
			{
				Graphics.DrawTexture(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 50, 50),
					GUIDragHandler.Item.ItemDetails.itemIcon);
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
			_lootInv = null; //Clear looting info
			_elements.Remove(_inventoryGridLoot);
			_inventoryGridLoot = null;
			InputState.LowerMenuLevel(); //Tell the input state that we closed a menu.
			Screen.lockCursor = true;
			_renderGUI = false;
		}

		public Rect GetWindowSize()
		{
			var size = _inventoryGrid.GetWindowSize();

			Rect windRect = new Rect(5, 5, size.width + 15, size.height + 45);
			if (_lootInv != null)
			{
				windRect.width *= 2;
			}
			return windRect;
		}

	}
}