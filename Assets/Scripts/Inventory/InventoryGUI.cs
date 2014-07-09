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
	public Inventory inv;
	//List of notifications when a new item is added
	List<InventoryNotification> notifications = new List<InventoryNotification>();
	public bool showInv = false;
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
		Rect itemRect = new Rect(0,20, boxSize, boxSize);
		for (int i = 0; i < inv.containerList.Count; i++)
		{
			ItemContainer it = inv.containerList[i];
			if (it != null)
			{
				itemRect.x = i * (boxSize + boxPadding) + boxAreaPadding;
				if (GUI.Button(itemRect, it.item.itemName + "\n" + it.amount))
				{
					if (Event.current.button == 0) //Left mouse
					{
						//
					}
					else if (Event.current.button == 1) //Right mouse
					{
						//inv.DropItem(it);
					}
				}
			}
		}
		// Close button
		if (GUI.Button(new Rect(10, this.windowSize.height - 40, this.windowSize.width - 20, 30), "Close"))
		{
			this.renderGUI = false;
		}
	}
}
