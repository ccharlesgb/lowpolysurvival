using UnityEngine;
using System.Collections;

namespace Energy
{

	public class Generator : MonoBehaviour
	{

		public int fuel = 100; // Fuel procentage, need a better system later?

		public bool generatorActive = false;
		private bool renderGUI = false;

		private Rect windowSize = new Rect(15, 15, 150, 200);

		// Use this for initialization
		void Start()
		{
			InvokeRepeating("RemoveFuel", 0f, 1.0f);
		}

		// Update is called once per frame
		void Update()
		{

		}

		void RemoveFuel()
		{
			if (this.generatorActive)
			{
				this.fuel = this.fuel -1;
				if (this.fuel <= 0)
				{
					this.Deactivate();
				}
			}
		}

		void OnGUI()
		{
			if (this.renderGUI)
			{
				GUI.Window(0, windowSize, MyWindow, "Generator Settings");
			}
		}

		void MyWindow(int id)
		{
			bool oldActive = this.generatorActive; // Save old value for comparing.
			this.generatorActive = GUI.Toggle(new Rect(10, 30, 100, 30), this.generatorActive, "Active");

			// Value changed?
			if (this.generatorActive != oldActive)
			{
				if (this.generatorActive)
				{
					this.Activate();
				}
				else
				{
					this.Deactivate();
				}
			}

			GUI.Label(new Rect(10, 60, 40, 30), this.fuel + "%");
			GUI.Label(new Rect(50, 60, 100, 30), "fuel remaining");

			// Close button
			if (GUI.Button(new Rect(10, this.windowSize.height - 40, this.windowSize.width - 20, 30), "Close"))
			{
				this.renderGUI = false;
			}
		}

		// Player click on the object
		void OnMouseDown()
		{
			// Flip the boolean
			//this.Active = !this.Active;
			this.renderGUI = true;
			/*
			if (this.Active)
			{
				this.Activate();
			}
			else
			{
				this.Deactivate();
			}
			 */
		}

		void Activate()
		{
			this.renderer.material.color = Color.green;
		}

		void Deactivate()
		{
			this.renderer.material.color = Color.red;
		}
	}

}