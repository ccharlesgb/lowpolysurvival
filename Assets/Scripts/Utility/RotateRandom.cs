using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RotateRandom : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		transform.rotation = Quaternion.Euler (new Vector3(0, Random.Range (0,360), 0));
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
