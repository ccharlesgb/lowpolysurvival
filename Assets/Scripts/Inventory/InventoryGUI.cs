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
	// Inventory to connect to.
	public Inventory inv;

	// GUI skin to use for inventory.
	public GUISkin guiSkin;

	//List of notifications when a new item is added
	List<InventoryNotification> notifications = new List<InventoryNotification>();
	//public bool showInv = false;
	public bool renderGUI = false;
	public Rect windowSize;

	public float boxSize = 50;
	public float boxPadding = 5;
	public float boxAreaPadding = 5;

	public GameObject holstered = null;

	void Awake()
	{
		inv = GetComponent<Inventory>();
		if (inv == null)
		{
			Debug.Log ("Need inventory object to have GUI");
		}
		renderGUI = false;
	}

	// Use this for initialization
	void Start () 
	{
		this.windowSize = new Rect(0, 0, 10 + (boxSize + boxPadding) * 5, 50 + (boxSize + boxPadding) * 5);
	
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	void OnGUI()
	{
		GUI.skin = guiSkin;

		if (this.renderGUI)
		{
			GUI.Window(0, windowSize, MyWindow, "Inventory");
		}
		for (int i = 0; i < notifications.Count; i++)
		{
			notifications[i].DrawGUI ();
			if (notifications[i].DeleteMe)
			{
				notifications.RemoveAt (i);
				i--;
			}
		}
	}

	public void AddNotification(InventoryItem it, int amount)
	{
		InventoryNotification not = new InventoryNotification(it, amount);
		notifications.Add (not);

	}
	
	void MyWindow(int id)
	{
		

		DrawItemList();

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
		Rect itemRect = new Rect(0, 65, boxSize, boxSize);

		string toolTipText = null;
		int toolTipItem = 0;

		int slotsX = 5;
		int slotsY = 5;

		ItemContainer[] items = InventoryListToArray();

		// Loop all slots, by x and y.
		for (int y = 0; y < slotsY; y++)
		{
			for (int x = 0; x < slotsX; x++)
			{
				int slot = x + y * slotsX;
				
				GUI.Box(new Rect(5 + boxPadding / 2 + x * (boxSize + boxPadding), 45 + y * (boxSize + boxPadding), boxSize, boxSize), "" + x + y);
				
				// If slot is empty, continue to next slot.
				if (items[slot] == null)
				{
					continue;
				}

				DrawItemNew(x, y, items[slot]);
			}
		}

	}

	private void DrawItemNew(int x, int y, ItemContainer it)
	{
		Rect rect = new Rect(5 + boxPadding / 2 + x * (boxSize + boxPadding), 45 + y * (boxSize + boxPadding), boxSize, boxSize);

		if (rect.Contains(Event.current.mousePosition))
		{
			DrawToolTip(rect, it.item.itemName);
		}
		
		if (GUI.Button(rect, it.item.itemIcon))
		{
			if (Event.current.button == 0) //Left mouse
			{
				if (holstered != null)
				{
					Destroy (holstered);
				}
				GameObject itemFab = Instantiate (it.item.itemObject, transform.position, Quaternion.identity) as GameObject;
				itemFab.GetComponent<ItemBehaviour>().Init(it, 1);
				itemFab.GetComponent<ItemBehaviour>().Holster (inv);
				holstered = itemFab;

			}
			else if (Event.current.button == 1) //Right mouse
			{
				inv.DropItem(it.item.itemName, 1);
			}
		}

		// Draw the stack count.
		if (it.item.isStackable)
		{
			GUI.Label(rect, "" + it.amount, "Stacks");
		}
	}

	private ItemContainer[] InventoryListToArray()
	{
		// TODO: Break out the array size.
		ItemContainer[] array = new ItemContainer[25];
		foreach (ItemContainer container in inv.containerList)
		{
			array[container.slot] = container;
		}
		return array;
	}

	private void DrawToolTip(Rect itemRect, string toolTipText)
	{
		var x = itemRect.x;
		var y = itemRect.y - 15;

		GUI.Label(new Rect(x, y, boxSize + boxPadding, 60), toolTipText, "ToolTip");
	}
	
}
