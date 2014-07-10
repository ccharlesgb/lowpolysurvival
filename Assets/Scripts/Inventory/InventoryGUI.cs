using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class InventoryNotification
{
	public float timeCreated;
	public ItemContainer item;
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

	public InventoryNotification(ItemContainer hand)
	{
		item = hand;
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
		string message = "Added " + item.item.itemName + " x" + item.amount.ToString();

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
		
	
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	void OnGUI()
	{
		GUI.skin = guiSkin;
		var bdr = GUI.skin.window.border;
		Debug.Log("Left: " + bdr.left + " Right: " + bdr.right);
		Debug.Log("Top: " + bdr.top + " Bottom: " + bdr.bottom);

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

	public void AddNotification(ItemContainer hand)
	{
		InventoryNotification not = new InventoryNotification(hand);
		notifications.Add (not);

	}
	
	void MyWindow(int id)
	{
		

		DrawItemList();

		// Close button
		if (GUI.Button(new Rect(10, this.windowSize.height - 40, this.windowSize.width - 20, 30), "Close"))
		{
			this.renderGUI = false;
		}
	}

	private void DrawItemList()
	{
		Rect itemRect = new Rect(0, 35, boxSize, boxSize);

		string toolTipText = null;
		int toolTipItem = 0;

		for (int i = 0; i < inv.containerList.Count; i++)
		{
			ItemContainer it = inv.containerList[i];

			if (it != null)
			{
				itemRect.x = i * (boxSize + boxPadding) + boxAreaPadding;

				if (MouseOverItem(ref itemRect, i, Event.current.mousePosition))
				{
					toolTipText = it.item.itemName;
					toolTipItem = i;
				}

				DrawItem(itemRect, i, it);
			}
		}

		if (toolTipText != null)
		{
			DrawToolTip(itemRect, toolTipText, toolTipItem);
		}

	}

	private void DrawToolTip(Rect itemRect, string toolTipText, int toolTipItem)
	{
		var x = toolTipItem * (boxSize + boxPadding) + boxAreaPadding;
		var y = itemRect.y + 60;

		GUI.Label(new Rect(x, y, boxSize + boxPadding, 60), toolTipText, "ToolTip");
	}

	private bool MouseOverItem(ref Rect itemRect, int i, Vector2 mousePosition)
	{
		return mousePosition.x > itemRect.x && mousePosition.x < (i + 1) * (boxSize + boxPadding) + boxAreaPadding &&
		       mousePosition.y > itemRect.y && mousePosition.y < (i + 1) * (boxSize + boxPadding) + boxAreaPadding;
	}

	private void DrawItem(Rect itemRect, int i, ItemContainer it)
	{
		//if (GUI.Button(itemRect, it.item.itemName + "\n" + it.amount))
		if (GUI.Button(itemRect, it.item.itemIcon))
		{
			if (Event.current.button == 0) //Left mouse
			{
				//
			}
			else if (Event.current.button == 1) //Right mouse
			{
				inv.DropItem(it.item.itemName, 1);
			}
		}

		// Draw the stack count.
		if (it.item.isStackable)
		{
			GUI.Label(new Rect(itemRect.x, itemRect.y, boxSize, boxSize), "" + it.amount, "Stacks");
		}

	}

	
}
