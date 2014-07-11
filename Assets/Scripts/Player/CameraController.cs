using UnityEngine;
using System.Collections;

namespace Player
{
	[ExecuteInEditMode]
	public class CameraController : MonoBehaviour
	{

		// Player object to follow.
		public GameObject player;

		// Default zoom.
		public float distance = 20f;

		// Distance to zoom towards.
		private float distanceWanted;

		// Maximum and minimum distance from the player.
		public float min = 10f;
		public float max = 60f;

		// Distance to zoom per "scroll" step.
		public float zoomStep = 30f;

		// Speed to zoom with
		public float zoomSpeed = 5f;

		// Use this for initialization
		void Start()
		{
			distanceWanted = distance;
		}

		// Update is called once per frame
		void Update()
		{
			// Ensure the controller is attached to a player.
			if (!this.player)
				return;

			// ScrollWheel input.
			float mouseInput = Input.GetAxis("Mouse ScrollWheel");
			distanceWanted -= zoomStep * mouseInput;
			
			// Clamp to max/min.
			distanceWanted = Mathf.Clamp(distanceWanted, min, max);

			// Smooth the scrolling.
			distance = Mathf.Lerp(distance, distanceWanted, Time.deltaTime * zoomSpeed);

			// Set position to player and move "backwards" in the camera direction (zooming out).
			transform.position = player.transform.position - this.transform.forward * distance;

		}
	}

}
