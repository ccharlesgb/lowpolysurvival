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

	private int ActiveSlot = 0;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnGUI()
	{
		GUI.skin = GUISkin;

		GUI.Box(_boxSize, "");

		ItemContainer[] items = Inv.GetInventoryAsArray();

		for (int x = 0; x < 5; x++)
		{
			var rect = new Rect(_boxSize.x + x * (BoxSize), _boxSize.y, BoxSize, _boxSize.height);

			if (x == ActiveSlot)
			{
				GUI.Box(rect, "" + (x + 1));
			}
			else
			{
				GUI.Box(rect, "" + (x + 1), "NotActiveBox");
			}

			ItemContainer it = items[x];

			if (it != null)
			{
				GUI.DrawTexture(new Rect(rect.x + 5, rect.y + 20, BoxSize - 10, BoxSize - 10), it.item.itemIcon);
			}

		}

	}
}