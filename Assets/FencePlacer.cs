using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class FencePlacer : MonoBehaviour, IHolster
{

    private float nextPrimaryFire = 0.0f;

    public GameObject fencePrefab;

    private GameObject ghostPrefab;

    public Color ghostColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);

	// Use this for initialization
	void Start ()
	{
	    nextPrimaryFire = Time.time;
	}
	
    public void OnHolster(Inventory ownerInv)
    {
        Vector3 spawnPos = ownerInv.transform.position + ownerInv.transform.forward * 10.0f;
        ghostPrefab = Instantiate(fencePrefab, spawnPos, Quaternion.identity) as GameObject;
        ghostPrefab.renderer.material.SetColor("_Color", ghostColor);

    }

    public void OnDeHolster(Inventory ownerInv)
    {
        //Get rid of the ghost
        if (ghostPrefab != null)
            Destroy(ghostPrefab);
    }

    public Vector3 GetSpawnPos()
    {
        return transform.position + transform.forward * 10.0f;
    }

    public void PrimaryFire(Inventory inv)
    {
        if (nextPrimaryFire > Time.time)
        {
            return; //Cant fire yet
        }

        //Spaw the fence prefab
        Vector3 spawnPos = GetSpawnPos();
        GameObject fence = Instantiate(fencePrefab, spawnPos, Quaternion.identity) as GameObject;
        Quaternion newRot = new Quaternion();
        newRot.SetLookRotation(transform.forward);
        fence.transform.rotation = newRot;

        //Use up a fence in the inventory
        inv.UseItem(GetComponent<ItemBehaviour>().container.Item.itemName, 1);

        nextPrimaryFire = Time.time + 0.5f;
    }

    public void SecondaryFire(Inventory inv)
    {
        

    }

    void Update()
    {
        ghostPrefab.transform.position = GetSpawnPos();
        Quaternion newRot = new Quaternion();
        newRot.SetLookRotation(transform.forward);
        ghostPrefab.transform.rotation = newRot;
    }
}
