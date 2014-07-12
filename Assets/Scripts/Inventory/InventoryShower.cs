using UnityEngine;
using System.Collections;

public class InventoryShower : MonoBehaviour {

	InventoryGUI invGUI;
	public KeyCode showKey;

	// Use this for initialization
	void Start () 
	{
		invGUI = GetComponent<InventoryGUI>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (invGUI != null)
		{
			if (Input.GetKeyDown(showKey))
			{
			    Screen.lockCursor = true;
                Screen.lockCursor = false;
				invGUI.RenderGUI = true;
			}
			else if (Input.GetKeyUp (showKey))
			{
			    Screen.lockCursor = true;
				invGUI.RenderGUI = false;
			}
		}
	}
}
