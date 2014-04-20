using UnityEngine;
using System.Collections;

namespace Energy
{
	public class Lamp : MonoBehaviour
	{
		private Generator myGenerator;
		private Light myLight;

		// Use this for initialization
		void Start()
		{
			this.myLight = this.GetComponent<Light>();

			var Generators = GameObject.FindGameObjectsWithTag("Generator");
			foreach (GameObject g in Generators)
			{
				this.myGenerator = g.GetComponent<Generator>();
			}
		}

		// Update is called once per frame
		void Update()
		{
			myLight.enabled = myGenerator.Active;
		}
	}
}