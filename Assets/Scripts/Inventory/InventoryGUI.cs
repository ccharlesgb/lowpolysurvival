using UnityEngine;
using System.Collections;

public class InventoryGUI : MonoBehaviour 
{
	public Inventory inv;
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
	}
	
	void MyWindow(int id)
	{
		Rect itemRect = new Rect(0,20, boxSize, boxSize);
		for (int i = 0; i < inv.items.Count; i++)
		{
			ItemHandle it = inv.items[i];
			if (it != null)
			{
				itemRect.x = i * (boxSize + boxPadding) + boxAreaPadding;
				GUI.Box(itemRect, it.item.name + "\n" + it.amount);
			}
		}
		// Close button
		if (GUI.Button(new Rect(10, this.windowSize.height - 40, this.windowSize.width - 20, 30), "Close"))
		{
			this.renderGUI = false;
		}
	}
}
