using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	private Map map;
	public float rayDist;
	
	public bool blocked;
	
	private Vector3 velocity;
	private Vector3 oldVelLocal;
	
	private Transform _transform;
	
	public float frictionCoeff;
	public float bounceCoeff;
	
	public float jumpSpeed = 5.0f;
	
	private Ray _collisionRay;
	
	public float gravity = 0.05f;
	
	private float currentMoveSpeed = 0.0f;
	private float targetMoveSpeed = 0.0f;
	public float moveSpeedSharpness = 0.8f;
	
	public float walkSpeed = 1.5f;
	public float sprintSpeed = 5.31f;
	
	private Vector3 topColliderPos;
	private Vector3 bottomColliderPos;
	public float colliderRadius;
	
	private Vector3[] debugContacts;
	private int contactCount;
	
	private Vector3 groundNormal;
	
	public Vector3 GetColliderPos(int collideID)
	{
		if (collideID == 0)
			return topColliderPos;
		if (collideID == 1)
			return bottomColliderPos;
		
		return Vector3.zero;
	}
	
	public float GetColliderRadius()
	{
		return colliderRadius;	
	}
	
	void Awake()
	{
		//Setup character collider pos parameters;
		topColliderPos = new Vector3(0,1,0);
		bottomColliderPos = new Vector3(0,-1,0);
		
		groundNormal = Vector3.zero;
		debugContacts = new Vector3[20];
		contactCount = 0;
		_collisionRay = new Ray();
		
		_transform = transform;
		
		map = GameObject.Find ("Map").GetComponent<Map>();	
		blocked = false;
	}
	
	// Use this for initialization
	void Start ()
	{
		
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		Vector3 oldVel = velocity;
		Vector3 oldVelLocal = _transform.InverseTransformDirection(oldVel);
		
		float moveX = Input.GetAxis ("Horizontal");
		float moveY = Input.GetAxis ("Vertical");
		float jump = Input.GetAxis ("Jump");
		
		targetMoveSpeed = moveY * walkSpeed;
		
		if (targetMoveSpeed > currentMoveSpeed)
		{
			currentMoveSpeed = currentMoveSpeed + moveSpeedSharpness;
			if (currentMoveSpeed > targetMoveSpeed)
				currentMoveSpeed = targetMoveSpeed;
				
		}
		else if (targetMoveSpeed < currentMoveSpeed)
		{
			currentMoveSpeed = currentMoveSpeed - moveSpeedSharpness;
			if (currentMoveSpeed < targetMoveSpeed)
				currentMoveSpeed = targetMoveSpeed;
		}
		
		//Walking
		Vector3 velChange = new Vector3();
		velChange.x = oldVelLocal.x * -0.8f;
		velChange.z = currentMoveSpeed - oldVelLocal.z;
		
		
		Vector3 newVelLocal = oldVelLocal + velChange;
		velocity = _transform.TransformDirection (newVelLocal);
		
		//Jumping
		if (jump > 0 && IsOnGround ())
		{
			velocity.y += jumpSpeed;
		}
		
		//Gravity
		velocity.y -= gravity;
	
		
		//DO ALL VELOCITY ADJUSTMENTS BEFORE THIS
		contactCount = 0;
		groundNormal = Vector3.zero;
		CharacterCollision.GetContactPoint(this, map);
		Vector3 oldPos = _transform.position;
		Vector3 newPos = oldPos;
		newPos += velocity * Time.fixedDeltaTime;
		_transform.position = newPos;
	}
	
	public void OnMapCollide(HitInfo hit)
	{
		if (hit.hit)
		{
			_transform.position += hit.minTranslation;
			
			float frictionFactor = Vector3.Dot (hit.normal, Vector3.up) * frictionCoeff;
			
			Vector3 frictionChange = -frictionFactor * velocity;
			float bounceBack = -Vector3.Dot (velocity, hit.normal) * bounceCoeff;
			velocity += hit.normal * bounceBack;
			velocity += frictionChange;
			
			debugContacts[contactCount] = hit.point;
			debugContacts[contactCount + 1] = hit.point + (hit.minTranslation * 10.0f);
			contactCount += 2;
			
			if (groundNormal.y < hit.normal.y)
			{
				groundNormal = hit.normal;
			}	
		}
	}
	
	public bool IsOnGround()
	{
		return groundNormal == Vector3.up;
	}
	
	void OnDrawGizmos()
	{
		Vector3 end = transform.position + (Vector3.down * rayDist);
		Gizmos.DrawLine (transform.position, end);	
		
		Gizmos.DrawRay (_collisionRay);
		
		
		for (int i=0; i < contactCount; i=i+2)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine (debugContacts[i], debugContacts[i+1]);	
		}
	}

}
