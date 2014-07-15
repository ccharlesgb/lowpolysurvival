using UnityEngine;

/// <summary>
/// Sets the InventoryGUI to render when ShowKey is pressed.
/// </summary>
public class InventoryShower : MonoBehaviour
{
	public KeyCode ShowKey;
	private InventoryGUI _invGUI;

	// Use this for initialization
	private void Start()
	{
		_invGUI = GetComponent<InventoryGUI>();
	}

	// Update is called once per frame
	private void Update()
	{
		// Terminate if we have no InventoryGUI.
		if (_invGUI == null) return;

		if (Input.GetKeyDown(ShowKey) && _invGUI.RenderGUI == false)
		{
		    InputState.AddMenuLevel(); //Tell the input state that theres a menu open (and cursor is visible)
			Screen.lockCursor = true;
			Screen.lockCursor = false;
			_invGUI.RenderGUI = true;
		}
		else if (Input.GetKeyDown(ShowKey) && _invGUI.RenderGUI == true)
		{
            InputState.LowerMenuLevel(); //Tell the input state that we closed a menu.
			Screen.lockCursor = true;
			_invGUI.RenderGUI = false;
		}
	}
}