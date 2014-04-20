using UnityEngine;
using System.Collections;

namespace Energy
{

	public class Generator : MonoBehaviour
	{

		public bool Active = false;

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}

		// Player click on the object
		void OnMouseDown()
		{
			// Flip the boolean
			this.Active = !this.Active;

			if (this.Active)
			{
				this.Activate();
			}
			else
			{
				this.Deactivate();
			}
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