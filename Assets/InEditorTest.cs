using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class InEditorTest : MonoBehaviour
{
	void Awake()
	{
		Debug.Log ("Test Awake");
	}

	void OnEnable()
	{
		Debug.Log ("Test Enable");
	}

	// Use this for initialization
	void Start () 
	{
		Debug.Log ("Test Start");
	}

	void OnDisable()
	{
		Debug.Log ("Test Disable");
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
