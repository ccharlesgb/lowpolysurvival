using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ThirdPerson : MonoBehaviour
{

    private Transform _transform;

    //Transform target
    public Transform targetTransform;

    // Default zoom.
    public float distance = 20f;

    // Distance to zoom towards.
    private float distanceWanted;

    public Vector3 targetPosition;
    public Vector3 targetDirection;

    public float xLookSensitivity = 1.0f;
    public float yLookSensitivity = 0.5f;

    public float dirLookInfluence;
    public float yawLookFactor = 0.1f;

    public float positionLerpSpeed = 1.0f;
    public float directionLerpSpeed = 1.0f;

    [Range(-1, 1)] public float minVerticalLook = 0.0f;
    [Range(-1, 1)] public float maxVerticalLook = 0.0f;

	// Use this for initialization
	void Start ()
	{
	    _transform = transform;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
        float xIn = Input.GetAxis("Mouse X") * xLookSensitivity;
        float yIn = Input.GetAxis("Mouse Y") * yLookSensitivity;

	    Vector3 currentPos = _transform.position;
	    Vector3 targetPos = targetTransform.position;

        Vector3 playerForward = targetTransform.forward;
        Vector3 playerUp = targetTransform.up;

	    Quaternion currentDir = _transform.rotation;

	    targetDirection.x = playerForward.x;
	    targetDirection.z = playerForward.z;
	    targetDirection.y = Mathf.Clamp(targetDirection.y + yIn, minVerticalLook, maxVerticalLook);

	    targetDirection += targetTransform.right * xIn * yawLookFactor;

       // Vector3 posOffset = -playerForward * behindDistance + playerUp * heightDistance;

	    Vector3 targetDirectionPosOffset = targetDirection;
	    targetDirectionPosOffset.x = 0.0f;
	    targetDirectionPosOffset.z = 0.0f;
        targetPos += targetDirectionPosOffset * -dirLookInfluence;

	    Vector3 newPos = Vector3.Lerp(currentPos, targetPos, positionLerpSpeed);

	    Quaternion newRot = _transform.rotation;
        newRot.SetLookRotation(targetDirection);

	    _transform.position = newPos;
	    _transform.rotation = Quaternion.Slerp(currentDir, newRot, directionLerpSpeed);
	}
}
