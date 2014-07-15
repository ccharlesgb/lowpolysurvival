using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class InventoryNotification
{
	public float timeCreated;
	public ItemDetails itemDetails;
	public int amount;
	static float lifeTime = 3.0f; //How long does it last?


    //Flag for the main GUI to check if this notification should go
    //Maybe should use events?
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

	public InventoryNotification(ItemDetails it, int amt)
	{
		itemDetails = it;
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
		string message = "Added " + itemDetails.itemName + " x" + amount.ToString();

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

/// <summary>
///		GUI for the Inventory.
/// </summary>
public class InventoryGUI : MonoBehaviour
{
	// Size of the slot slot
	private int SlotsX = 5;
	private int SlotsY = 5;
	private readonly List<InventoryNotification> _notifications = new List<InventoryNotification>();
	
	public float BoxAreaPadding = 5;
	public float BoxPadding = 5;
	public float BoxSize = 50;

	// GUI skin to use for inventory.
	public GUISkin GUISkin;
	public Inventory Inv;

	public bool RenderGUI = false;

	private ItemSlot _draggedItem;
	private bool _isDraggingItem;

    public bool isLooting = false;
    public Inventory lootInventory = null;

	private void Awake()
	{
		Inv = GetComponent<Inventory>();
        //TODO: CHange size of grid?
		if (Inv == null)
		{
			Debug.Log("Need inventory object to have GUI");
		}
		RenderGUI = false;
	    isLooting = false;
	    lootInventory = null;
	}


    public Rect GetWindowSize()
    {
        if (isLooting)
        {
            return new Rect(0, 0, 10 + (BoxSize + BoxPadding)*5*2, 50 + (BoxSize + BoxPadding)*5);
        }
        else
        {
            return new Rect(0, 0, 10 + (BoxSize + BoxPadding) * 5, 50 + (BoxSize + BoxPadding) * 5);
        }
    }

	// Use this for initialization
	private void Start()
	{

	}

	// Update is called once per frame
	private void Update()
	{
		var v = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

		// Drop outside the window?
		if (_isDraggingItem && Input.GetMouseButtonUp(0) && !GetWindowSize().Contains(v))
		{
			Inv.DropItem(_draggedItem.SlotID, _draggedItem.Amount);
			ResetDragging();
		}
	}

	private void OnGUI()
	{
		GUI.skin = GUISkin;

		if (RenderGUI)
		{
			GUI.Window(0, GetWindowSize(), MyWindow, "Inventory");
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

    public void ShowLootGUI(Inventory inv)
    {
        lootInventory = inv;
        isLooting = true;
	    
        Popup();
        Debug.Log("LOOTING GUI");
    }

    public void Popup()
    {
        InputState.AddMenuLevel(); //Tell the input state that theres a menu open (and cursor is visible)
        Screen.lockCursor = true;
        Screen.lockCursor = false;
        RenderGUI = true;
    }

    public void Hide()
    {
        InputState.LowerMenuLevel(); //Tell the input state that we closed a menu.
        Screen.lockCursor = true;
        RenderGUI = false;
    }

	private void OnEnable()
	{
		Inv.OnItemAdded += AddNotification;
	    Inv.OnLootBegin += ShowLootGUI;
	}

	private void OnDisable()
	{
		Inv.OnItemAdded -= AddNotification;
        Inv.OnLootBegin -= ShowLootGUI;
	}

	public void AddNotification(ItemSlot slot, int amount)
	{
		var not = new InventoryNotification(slot.ItemDetails, amount);
		_notifications.Add(not);
	}

	private void MyWindow(int id)
	{
		DrawItemList();

	    if (isLooting)
	        DrawLootList();

		if (_isDraggingItem)
		{
			Graphics.DrawTexture(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, BoxSize, BoxSize),
				_draggedItem.ItemDetails.itemIcon);
		}
	}

	private void DrawItemList()
	{
		var itemRect = new Rect(0, 65, BoxSize, BoxSize);

		ItemSlot[] items = Inv.GetAllSlots();

		// Loop all slots, by x and y.
		for (int y = 0; y < SlotsY; y++)
		{
			for (int x = 0; x < SlotsX; x++)
			{
				int slot = x + y*SlotsX;
				ItemSlot it = items[slot];

				var rect = new Rect(5 + BoxPadding/2 + x*(BoxSize + BoxPadding), 45 + y*(BoxSize + BoxPadding), BoxSize, BoxSize);

				GUI.Box(rect, "" + x + y);

				// Draw the ItemDetails.
				if (it != null)
				{
					DrawItem(rect, it);
				}

				// Drop handling.
				if (_isDraggingItem && Event.current.type == EventType.mouseUp && rect.Contains(Event.current.mousePosition))
				{
					StopDragging(slot, it);
				}
			}
		}
	}

    private void DrawLootList()
    {
        var itemRect = new Rect(0, 65, BoxSize, BoxSize);

        ItemSlot[] items = lootInventory.GetAllSlots();

        // Loop all slots, by x and y.
        for (int y = 0; y < SlotsY; y++)
        {
            for (int x = 0; x < SlotsX; x++)
            {
                int slot = x + y * SlotsX;
                ItemSlot it = items[slot];

                var rect = new Rect(5 + BoxPadding / 2 + x * (BoxSize + BoxPadding) + (GetWindowSize().width / 2), 45 + y * (BoxSize + BoxPadding), BoxSize, BoxSize);

                GUI.Box(rect, "" + x + y);

                // Draw the ItemDetails.
                if (it != null)
                {
                    DrawItem(rect, it, true); //Tell the function that this is a lootable ItemDetails
                }

                // Drop handling.
                if (_isDraggingItem && Event.current.type == EventType.mouseUp && rect.Contains(Event.current.mousePosition))
                {
                    //StopDragging(slot, it);
                }
            }
        }
    }

	private void DrawItem(Rect rect, ItemSlot it, bool isLootInv = false)
	{
		Event e = Event.current;

		if (rect.Contains(e.mousePosition))
		{
			DrawToolTip(rect, it.ItemDetails.itemName);

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

		if (GUI.Button(rect, it.ItemDetails.itemIcon))
		{
			if (e.button == 0) //Left mouse
			{
				//Inv.TransferItem(it.ItemDetails, it.Amount, lootInventory);
			}
			else if (Event.current.button == 1) //Right mouse
			{
				//inv.DropItem(it.ItemDetails.itemName, 1);
			}
		}

		// Draw the stack count.
		if (it.ItemDetails.isStackable)
		{
			GUI.Label(rect, "" + it.Amount, "Stacks");
		}
	}

	/// <summary>
	///     Set the currently dragged ItemDetails.
	/// </summary>
	/// <param name="it">ItemSlot to drag.</param>
	private void StartDragging(ItemSlot it)
	{
		_isDraggingItem = true;
		_draggedItem = it;
	}

	/// <summary>
	///     Drop handling.
	/// </summary>
	/// <param name="slot">SlotID to drop the ItemDetails into.</param>
	/// <param name="it">ItemDetails already in the slot, null if empty.</param>
	private void StopDragging(int slot, ItemSlot it)
	{
		if (it != null)
		{
			// TODO: merge stacks?
            Inv.SwapSlots(_draggedItem, it);
		}
		else
		{
            Inv.MoveToSlot(_draggedItem, slot);
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

	private void DrawToolTip(Rect itemRect, string toolTipText)
	{
		float x = itemRect.x;
		float y = itemRect.y - 15;

		GUI.Label(new Rect(x, y, BoxSize + BoxPadding, 60), toolTipText, "ToolTip");
	}
}