using UnityEngine;

namespace LowPolySurvival.Inventory
{
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
		
			if (Input.GetKeyDown(ShowKey) && _invGUI.RenderGUI() == false)
			{
				_invGUI.Popup();

			}
			else if (Input.GetKeyDown(ShowKey) && _invGUI.RenderGUI() == true)
			{
				_invGUI.Hide();
			}
		}
	}
}