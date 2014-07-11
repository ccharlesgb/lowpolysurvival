using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ThirdPerson : MonoBehaviour
{

    private Transform _transform;
    // Player object to follow.

    public GameObject player;

    // Default zoom.
    public float distance = 20f;

    // Distance to zoom towards.
    private float distanceWanted;

    // Maximum and minimum distance from the player.
    public float minZoom = 10f;
    public float maxZoom = 60f;
    // Distance to zoom per "scroll" step.
    public float zoomStep = 30f;
    // Speed to zoom with
    public float zoomSpeed = 5f;

    public Vector3 targetPosition;
    public Vector3 targetDirection;

    public float xLookSensitivity = 1.0f;
    public float yLookSensitivity = 0.5f;


    public float behindDistance = 5.0f;
    public float heightDistance = 5.0f;

    public float dirLookInfluence;
	// Use this for initialization
	void Start ()
	{
	    _transform = transform;
	}
	
	// Update is called once per frame
	void Update ()
	{
        float xIn = Input.GetAxis("Mouse X") * xLookSensitivity;
        float yIn = Input.GetAxis("Mouse Y") * yLookSensitivity;

	    Vector3 newPos = player.transform.position;

        Vector3 playerForward = player.transform.forward;
	    Vector3 playerUp = player.transform.up;

	    targetDirection.x = player.transform.forward.x;
        targetDirection.z = player.transform.forward.z;
	    targetDirection.y += yIn;

        Vector3 posOffset = -playerForward * behindDistance + playerUp * heightDistance;

	    Vector3 targetDirectionPosOffset = targetDirection;
	    targetDirectionPosOffset.x = 0.0f;
	    targetDirectionPosOffset.z = 0.0f;
        posOffset += targetDirectionPosOffset * -dirLookInfluence;

	    newPos = newPos + posOffset;


	    Quaternion newRot = _transform.rotation;
        newRot.SetLookRotation(targetDirection);

	    _transform.position = newPos;
	    _transform.rotation = newRot;
	}
}
