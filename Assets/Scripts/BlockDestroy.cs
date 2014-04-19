using UnityEngine;
using System.Collections;

public class BlockDestroy : MonoBehaviour {
	
	private Map map;
	public float rayDist;
	
	public bool blocked;
	
	private Ray myRay;
	
	void Awake()
	{
		map = GameObject.Find ("Map").GetComponent<Map>();	
		blocked = false;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	void Update()
	{
		if (Input.GetMouseButtonUp(0))
		{
			myRay.origin = Camera.main.transform.position;
			myRay.direction = Camera.main.transform.forward;
			BlockRayResult res = BlockRayTrace.Bresenham(map, myRay.origin, myRay.origin + (myRay.direction * 10.0f), 2);
			rayDist = res.distance;
	
			blocked = false;
			
			if (res.block.IsValid ())
			{
				map.RemoveBlock(res.hitPos);
			}
		}
	}
	
	void OnDrawGizmos()
	{
		Gizmos.DrawLine (myRay.origin,  myRay.origin + (myRay.direction * 10.0f));
	}

}
