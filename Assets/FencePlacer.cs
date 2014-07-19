using System.Runtime.InteropServices;
using LowPolySurvival.Inventory;
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
        Ray ray = new Ray(transform.position + transform.forward * 5.0f, Vector3.down);

        RaycastHit hit;
        Physics.Raycast(ray, out hit, 10.0f, 1 << 8);
        if (hit.collider != null)
        {
            return hit.point;
        }
        return Vector3.zero;
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
        inv.RemoveItem(GetComponent<ItemBehaviour>().slot.ItemDetails, 1);

        nextPrimaryFire = Time.time + 0.5f;
    }

    public void SecondaryFire(Inventory inv)
    {
        

    }

    void Update()
    {
        if (ghostPrefab != null)
        {
            ghostPrefab.transform.position = GetSpawnPos();
            Quaternion newRot = new Quaternion();
            newRot.SetLookRotation(transform.forward);
            ghostPrefab.transform.rotation = newRot;
        }
    }
}
