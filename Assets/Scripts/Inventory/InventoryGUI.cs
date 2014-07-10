using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class InventoryNotification
{
	public float timeCreated;
	public InventoryItem item;
	public int amount;
	static float lifeTime = 3.0f;

	private bool deleteMe;
	public bool DeleteMe
	{
		get
		{
			return deleteMe;
		}
		private set
		{
			deleteMe = value;
		}
		
	}

	public InventoryNotification(InventoryItem it, int amt)
	{
		item = it;
		amount = amt;
		timeCreated = Time.time;
		DeleteMe = false;
	}

	public void DrawGUI()
	{
		float w = Screen.width;
		float h = Screen.height;
		float sizeX = w * 0.2f;
		float sizeY = h * 0.1f;
		float progress = (Time.time - timeCreated) / lifeTime;
		float moveAmount = h / 0.5f;
		Rect position = new Rect(w - sizeX, h - sizeY - (progress * moveAmount), sizeX, sizeY);
		string message = "Added " + item.itemName + " x" + amount.ToString();

		Color boxCol = GUI.color;
		boxCol.a = 1.0f - progress;

		GUI.color = boxCol;
		GUI.Box (position, message);

		if (progress >= 1.0f)
		{
			DeleteMe = true;
		}
	}
}

public class InventoryGUI : MonoBehaviour
{
	// Size of the slot container
	private const int SlotsX = 5;
	private const int SlotsY = 5;
	private readonly List<InventoryNotification> _notifications = new List<InventoryNotification>();
	
	public float BoxAreaPadding = 5;
	public float BoxPadding = 5;
	public float BoxSize = 50;

	// GUI skin to use for inventory.
	public GUISkin GUISkin;
	public Inventory Inv;

	public bool RenderGUI = false;
	public Rect WindowSize;

	private ItemContainer _draggedItem;
	private bool _isDraggingItem;

	private void Awake()
	{
		Inv = GetComponent<Inventory>();
		if (Inv == null)
		{
			Debug.Log("Need inventory object to have GUI");
		}
		RenderGUI = false;
	}

	// Use this for initialization
	private void Start()
	{
		WindowSize = new Rect(0, 0, 10 + (BoxSize + BoxPadding)*5, 50 + (BoxSize + BoxPadding)*5);
	}

	// Update is called once per frame
	private void Update()
	{
		var v = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
		if (_isDraggingItem && Input.GetMouseButtonUp(0) && !WindowSize.Contains(v))
		{
			Inv.DropItem(_draggedItem.item.itemName, _draggedItem.amount);
			ResetDragging();
		}
	}

	private void OnGUI()
	{
		GUI.skin = GUISkin;

		if (RenderGUI)
		{
			GUI.Window(0, WindowSize, MyWindow, "Inventory");
		}
		else if (_isDraggingItem)
		{
			ResetDragging();
		}

		for (int i = 0; i < _notifications.Count; i++)
		{
			_notifications[i].DrawGUI();
			if (_notifications[i].DeleteMe)
			{
				_notifications.RemoveAt(i);
				i--;
			}
		}
	}

	public void AddNotification(InventoryItem it, int amount)
	{
		var not = new InventoryNotification(it, amount);
		_notifications.Add(not);
	}

	private void MyWindow(int id)
	{
		DrawItemList();

		if (_isDraggingItem)
		{
			Graphics.DrawTexture(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, BoxSize, BoxSize),
				_draggedItem.item.itemIcon);
		}

		/*
		// Close button
		if (GUI.Button(new Rect(10, this.windowSize.height - 40, this.windowSize.width - 20, 30), "Close"))
		{
			this.renderGUI = false;
		}
		*/
	}

	private void DrawItemList()
	{
		var itemRect = new Rect(0, 65, BoxSize, BoxSize);

		ItemContainer[] items = InventoryListToArray();

		// Loop all slots, by x and y.
		for (int y = 0; y < SlotsY; y++)
		{
			for (int x = 0; x < SlotsX; x++)
			{
				int slot = x + y*SlotsX;
				ItemContainer it = items[slot];

				var rect = new Rect(5 + BoxPadding/2 + x*(BoxSize + BoxPadding), 45 + y*(BoxSize + BoxPadding), BoxSize, BoxSize);

				GUI.Box(rect, "" + x + y);

				// Draw the Item.
				if (it != null)
				{
					DrawItem(rect, x, y, it);
				}

				// Drop handling.
				if (_isDraggingItem && Event.current.type == EventType.mouseUp && rect.Contains(Event.current.mousePosition))
				{
					StopDragging(slot, it);
				}
			}
		}
	}

	private void DrawItem(Rect rect, int x, int y, ItemContainer it)
	{
		Event e = Event.current;

		if (rect.Contains(e.mousePosition))
		{
			DrawToolTip(rect, it.item.itemName);

			// Left mouse button.
			if (!_isDraggingItem && e.button == 0 && e.type == EventType.mouseDrag)
			{
				StartDragging(it);
			}
		}

		// Prevent the icon from being drawn if dragging it.
		if (it == _draggedItem)
		{
			return;
		}

		if (GUI.Button(rect, it.item.itemIcon))
		{
			if (e.button == 0) //Left mouse
			{
				Inv.HolsterItem(it);
			}
			else if (Event.current.button == 1) //Right mouse
			{
				//inv.DropItem(it.item.itemName, 1);
			}
		}

		// Draw the stack count.
		if (it.item.isStackable)
		{
			GUI.Label(rect, "" + it.amount, "Stacks");
		}
	}

	/// <summary>
	///     Set the currently dragged item.
	/// </summary>
	/// <param name="it">ItemContainer to drag.</param>
	private void StartDragging(ItemContainer it)
	{
		_isDraggingItem = true;
		_draggedItem = it;
	}

	/// <summary>
	///     Drop handling.
	/// </summary>
	/// <param name="slot">Slot to drop the item into.</param>
	/// <param name="it">Item already in the slot, null if empty.</param>
	private void StopDragging(int slot, ItemContainer it)
	{
		if (it != null)
		{
			// TODO: merge stacks?

			// Switch item position.
			int newSlot = it.slot;
			it.slot = _draggedItem.slot;
			_draggedItem.slot = newSlot;
		}
		else
		{
			_draggedItem.slot = slot;
		}
		ResetDragging();
	}

	/// <summary>
	///     Reset the dragging status.
	/// </summary>
	private void ResetDragging()
	{
		_isDraggingItem = false;
		_draggedItem = null;
	}

	private ItemContainer[] InventoryListToArray()
	{
		// TODO: Break out the array size.
		var array = new ItemContainer[25];
		foreach (ItemContainer container in Inv.containerList)
		{
			array[container.slot] = container;
		}
		return array;
	}

	private void DrawToolTip(Rect itemRect, string toolTipText)
	{
		float x = itemRect.x;
		float y = itemRect.y - 15;

		GUI.Label(new Rect(x, y, BoxSize + BoxPadding, 60), toolTipText, "ToolTip");
	}
}