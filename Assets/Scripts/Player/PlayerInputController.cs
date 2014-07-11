using UnityEngine;
using System.Collections;

// Require a character controller to be attached to the same game object
[RequireComponent(typeof(CharacterMotorC))]

//RequireComponent (CharacterMotor)
[AddComponentMenu("Character/Player Input Controller C")]
//@script AddComponentMenu ("Character/FPS Input Controller")

public class PlayerInputController : MonoBehaviour {

    public float lookSensitivity;

	private CharacterMotorC cmotor;
	// Use this for initialization
	void Awake()
	{
		cmotor = GetComponent<CharacterMotorC>();
	}

	// Update is called once per frame
	void Update()
	{
		// Get the input vector from keyboard or analog stick
		Vector3 directionVector;
		directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		if (directionVector != Vector3.zero)
		{
			// Get the length of the directon vector and then normalize it
			// Dividing by the length is cheaper than normalizing when we already have the length anyway
			float directionLength = directionVector.magnitude;
			directionVector = directionVector / directionLength;

			// Make sure the length is no bigger than 1
			directionLength = Mathf.Min(1, directionLength);

			// Make the input vector more sensitive towards the extremes and less sensitive in the middle
			// This makes it easier to control slow speeds when using analog sticks
			directionLength = directionLength * directionLength;

			// Multiply the normalized direction vector by the modified length
			directionVector = directionVector * directionLength;
		}

		// Apply the direction to the CharacterMotor
		cmotor.inputMoveDirection = transform.rotation * directionVector;
		cmotor.inputJump = Input.GetButton("Jump");

        float xIn = Input.GetAxis("Mouse X");
        float yIn = Input.GetAxis("Mouse Y");

	    Vector3 newEuler = transform.rotation.eulerAngles;
	    newEuler.y += xIn * lookSensitivity;
	    transform.eulerAngles = newEuler;

	    /*
		// Generate a ray from the cursor position
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;

		if (Physics.Raycast(ray, out hit))
		{
			// Create a target position using the X and Y position from our raycast, but keep the current y.
			Vector3 targetPostition = new Vector3(hit.point.x, this.transform.position.y, hit.point.z);

			this.transform.LookAt(targetPostition);
		}*/
	}
}
