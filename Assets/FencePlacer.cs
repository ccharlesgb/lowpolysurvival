using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class FencePlacer : MonoBehaviour, IHolster
{

    public GameObject fencePrefab;
	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void OnHolster(Inventory ownerInv)
    {


    }

    public void OnDeHolster(Inventory ownerInv)
    {

    }

    public void PrimaryFire(Inventory inv)
    {
        Vector3 spawnPos = inv.transform.position + inv.transform.forward*10.0f;
        GameObject fence = Instantiate(fencePrefab, spawnPos, Quaternion.identity) as GameObject;
        
        inv.UseItem(GetComponent<ItemBehaviour>().container.Item.itemName, 1);
    }

    public void SecondaryFire(Inventory inv)
    {
        

    }
}
