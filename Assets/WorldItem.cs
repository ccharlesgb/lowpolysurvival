using UnityEngine;
using System.Collections;

public class WorldItem : MonoBehaviour {

	ItemHandle item;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnDropped(ItemHandle it)
	{
		if (!it.IsValid ()) return; //Validate
		item = it;

		GetComponent<MeshRenderer>().material.color = it.item.worldColor;
	}
}
