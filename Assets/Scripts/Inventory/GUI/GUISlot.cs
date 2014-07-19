using UnityEngine;

namespace LowPolySurvival.Inventory
{
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

			Event e = Event.current;

			if (GUIDragHandler.IsActive && e.type == EventType.mouseUp && WindowRect.Contains(e.mousePosition))
			{
				//_inventory.TransferItem(GUIDragHandler.Item.ItemDetails, GUIDragHandler.Item.Amount, GUIDragHandler.Inventory);
				_inventory.TransferItem(GUIDragHandler.Item, GUIDragHandler.Inventory, _slotID);
				GUIDragHandler.ResetItem();
			}

			// Don't draw if the item is being dragged. TODO: Draw a shadowed icon?
			if (GUIDragHandler.IsActive && GUIDragHandler.Item.Equals(_itemSlot))
			{
				GUI.Box(WindowRect, "" + _slotID);
			} 
			else if (_itemSlot != null)
			{
				if (WindowRect.Contains(e.mousePosition))
				{
					DrawToolTip(WindowRect, _itemSlot.ItemDetails.itemName);

					if (!GUIDragHandler.IsActive && e.button == 0 && e.type == EventType.mouseDrag)
					{
						GUIDragHandler.SetItem(_inventory, _itemSlot);
					}
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
}