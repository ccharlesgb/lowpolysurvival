using System.ComponentModel;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class CopterController : MonoBehaviour
{

    public Vector3 targetPosition; //Where we are tring to move towards
    public float engineForce = 1.0f;

    public Transform targetEntity;
    public float followAltitude = 10.0f;
    public float torquePower = 2.0f;
    public float bufferSize = 2.0f;

	// Use this for initialization
	void Start ()
	{
	    targetPosition = transform.position;

	    targetEntity = GameObject.Find("Player").transform;
	}
	
	// Update is called once per frame
    private void FixedUpdate()
    {
        //Follow position
        if (targetEntity != null)
        {
            targetPosition = targetEntity.position;
            targetPosition.y += followAltitude;
        }


        Vector3 positionDiff = transform.position - targetPosition;

        if (positionDiff.magnitude > bufferSize)
        {
            Vector3 force = positionDiff*-engineForce;
            rigidbody.AddForce(force);

            //Rotation
            float turnTorque = Vector3.Dot(positionDiff.normalized, -transform.forward);
            Vector3 torque = new Vector3(0.0f, turnTorque, 0.0f);
            rigidbody.AddTorque(torque*torquePower);
        }
    }
}
