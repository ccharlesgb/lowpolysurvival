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

		private GameObject sphere;

		// Use this for initialization
		void Start()
		{
			// Remove fuel every second.
			// TODO: Implement a better fuel system depending on the energy usage?
			InvokeRepeating("RemoveFuel", 0f, 1.0f);

			//GameObject sphere = CreateSphere(5);
			GameObject sphere_inv = CreateSphere(5);

			//get a reference to the mesh
			Mesh mesh = sphere_inv.GetComponent<MeshFilter>().mesh;
			
			// Reverse triangle winding.
			int[] triangles = mesh.triangles;
			int numpolies = triangles.Length / 3;

			for (var t = 0; t < numpolies; t++)
			{
				int tribuffer = triangles[t * 3];
				triangles[t * 3] = triangles[(t * 3) + 2];
				triangles[(t * 3) + 2] = tribuffer;
			}

			mesh.triangles = triangles;
			
		}

		// Create a sphere with radius size.
		GameObject CreateSphere(float size)
		{
			sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sphere.collider.enabled = false;
			sphere.transform.localPosition = this.transform.localPosition;
			sphere.transform.localScale = new Vector3(size, size, size);
			sphere.layer = 2;

			Renderer r = sphere.GetComponent<Renderer>();
			print(Shader.Find("Transparent/Diffuse"));
			r.material = new Material(Shader.Find("Transparent/Diffuse"));
			r.material.color = new Color(1, 1, 1, 0.2f);

			return sphere;
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
			// Render the GUI.
			this.renderGUI = true;
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