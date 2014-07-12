
using UnityEngine;
using System.Collections;

public class Axe : MonoBehaviour, IHolster
{
	public float primaryFireDelay = 1.0f;
	float nextPrimaryFire = 0.0f;

	public float hitRange = 2.0f;

	// Use this for initialization
	void Start () 
	{
		nextPrimaryFire = 0.0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	public void PrimaryFire(Inventory ownerInv)
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
				float distance = (obj.transform.position - transform.position).magnitude;
				if (distance > hitRange)
					return;
				Inventory inv = obj.GetComponent<Inventory>();
				if (inv != null) //It has an inventory we can harvest!
				{
					ItemContainer itemHarvest = inv.GetContainer ("Wood");
					if (itemHarvest != null)
					{
						if (itemHarvest.Amount > 1)
						{
							ownerInv.TransferItem (itemHarvest, 1);
						}
					}
				}
			}
		}
	}

	public void SecondaryFire(Inventory ownerInv)
	{

	}
}
