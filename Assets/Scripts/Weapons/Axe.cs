
using System;
using LowPolySurvival.Inventory;
using UnityEngine;
using System.Collections;

public class Axe : MonoBehaviour, IHolster
{
    public float primaryFireDelay = 1.0f;
    private float nextPrimaryFire = 0.0f;

    public float hitRange = 2.0f;

    private Animation _animation;

    void OnEnable()
    {
        _animation = GetComponent<Animation>();
    }

    // Use this for initialization
    private void Start()
    {
        nextPrimaryFire = 0.0f;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void OnHolster(Inventory ownerInv)
    {
        _animation.Play();
        _animation.Stop();
    }

    public void OnDeHolster(Inventory ownerInv)
    {
        
    }

    //Called when left click is held down
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
	    Ray ray = new Ray();
	    ray.origin = ownerInv.transform.position;
	    ray.direction = ownerInv.transform.forward;

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
                if (inv != null && (inv.Flags & Inventory.InventoryFlags.IsMineable) == Inventory.InventoryFlags.IsMineable) //It has an inventory we can harvest!
				{
					ItemSlot itemHarvest = inv.GetSlot("Wood");
					if (itemHarvest != null)
                    {
 
						if (itemHarvest.Amount > 1)
						{
						    Debug.Log("TRANSFER " + itemHarvest.ItemDetails.itemName + " " + itemHarvest.Amount);
							ownerInv.TransferItem (itemHarvest.ItemDetails, 1, inv);
						}
					}
				}
			}
		}
	}

    //Called when right click is held down
	public void SecondaryFire(Inventory ownerInv)
	{

	}

    void OnDrawGizmos()
    {
        //Ray ray = new Ray();
        //ray.origin = ownerInv.transform.position;
        //ray.direction = ownerInv.transform.forward;
        //Gizmos.DrawRay(ray);
    }
}
