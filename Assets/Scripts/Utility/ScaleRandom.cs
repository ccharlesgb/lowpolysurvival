using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class ScaleRandom : MonoBehaviour {

	public float xMin = 0.5f;
	public float xMax = 1.5f;
	public float yMin = 0.5f;
	public float yMax = 1.5f;
	public float zMin = 0.5f;
	public float zMax = 1.5f;

	// Use this for initialization
	void Start () 
	{
		Vector3 scale = new Vector3(Random.Range (xMin,xMax), Random.Range (yMin,yMax), Random.Range (zMin,zMax));
		transform.localScale = scale;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
