using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class RandomColour : MonoBehaviour {
	public List<Color> colors = new List<Color>();

	// Use this for initialization
	void OnEnable()
	{
		Material mat = new Material(GetComponent<MeshRenderer>().sharedMaterial);
		Color col = colors[Random.Range (0, colors.Count)];
		mat.color = col;
		GetComponent<MeshRenderer>().sharedMaterial = mat;
	}
	void OnDisable()
	{
		//DestroyImmediate(GetComponent<MeshRenderer>().material);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
