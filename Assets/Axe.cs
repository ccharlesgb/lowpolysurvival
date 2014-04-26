using UnityEngine;
using System.Collections;

public class Axe : MonoBehaviour 
{

	public float primaryFireDelay = 1.0f;
	float nextPrimaryFire = 0.0f;

	public GameObject player;

	// Use this for initialization
	void Start () 
	{
		player = gameObject;
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

		RaycastHit hit;
		
		// Generate a ray from the cursor position
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		if (Physics.Raycast(ray, out hit))
		{
			//Does this object have a resource?
			GameObject obj = hit.collider.gameObject;
			if (obj != null)
			{
				Resource res = obj.GetComponent<Resource>();
				if (res != null)
				{
					Item itemHarvest = res.FindItems("wood");
					if (itemHarvest != null)
					{
						if (itemHarvest.amount > 1)
						{
							itemHarvest.amount--;

							if (player)
							{
								player.GetComponent<Inventory>().AddItem(itemHarvest, 1.0f);
							}
							nextPrimaryFire += primaryFireDelay;
						}
					}
				}
			}
		}
	}
				
}
