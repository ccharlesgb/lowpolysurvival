using UnityEngine;
using System.Collections;

public class Axe : MonoBehaviour 
{

	public float primaryFireDelay = 1.0f;
	float nextPrimaryFire = 0.0f;

	public float hitRange = 2.0f;

	public GameObject player;

	// Use this for initialization
	void Start () 
	{
		nextPrimaryFire = 0.0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetAxis ("Fire1") == 1.0f)
		{
			PrimaryFire();
		}
	}

	public void PrimaryFire()
	{
		if (nextPrimaryFire > Time.time)
		{
			return; //Cant fire yet
		}
		nextPrimaryFire = Time.time + primaryFireDelay;
		GetComponent<Animation>().Play ();

		RaycastHit hit;
		
		// Generate a ray from the cursor position
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		if (Physics.Raycast(ray, out hit))
		{
			//Does this object have a resource?
			GameObject obj = hit.collider.gameObject;
			if (obj != null)
			{
				float distance = (obj.transform.position - player.transform.position).magnitude;
				if (distance > hitRange)
					return;
				Inventory inv = obj.GetComponent<Inventory>();
				if (inv != null) //It has an inventory we can harvest!
				{
					ItemHandle itemHarvest = inv.FindItem ("Wood"); //Does it have any wood?
					if (itemHarvest != null)
					{
						if (itemHarvest.amount > 1)
						{
							itemHarvest.amount--;

							if (player)
							{
								ItemHandle toAdd = new ItemHandle();
								toAdd.item = itemHarvest.item;
								toAdd.amount = 1;
								player.GetComponent<Inventory>().AddItem(toAdd);
							}
						}
					}
				}
			}
		}
	}
				
}
