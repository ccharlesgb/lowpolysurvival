using System.Linq;
using System.Runtime.InteropServices;
using LowPolySurvival.Inventory;
using UnityEngine;
using System.Collections;

public class FencePlacer : MonoBehaviour, IHolster
{

    public int currentSpawnRotationIndex = 0;
    public static Vector3[] spawnRotations =
    {
        Vector3.right,
        Vector3.forward,
        -Vector3.right,
        -Vector3.forward
    };
    public Vector3 currentSpawnRotation = spawnRotations[0];

    private float nextPrimaryFire = 0.0f;

    public GameObject fencePrefab;

    private GameObject ghostProp;

    public Color ghostColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);

	// Use this for initialization
	void Start ()
	{
	    nextPrimaryFire = Time.time;
	}
	
    public void OnHolster(Inventory ownerInv)
    {
        CreateGhost();
    }

    public void CreateGhost()
    {
        Vector3 spawnPos = GetSpawnPos();
        ghostProp = Instantiate(fencePrefab, spawnPos, Quaternion.identity) as GameObject;
        UpdateSpawnTransform(ghostProp.transform);
        ghostProp.renderer.material.SetColor("_Color", ghostColor);
    }

    public void OnDeHolster(Inventory ownerInv)
    {
        //Get rid of the ghost
        if (ghostProp != null)
            Destroy(ghostProp);
    }

    public void RotateGhost()
    {
        currentSpawnRotationIndex++;
        if (currentSpawnRotationIndex == spawnRotations.Count())
        {
            currentSpawnRotationIndex = 0;
        }
        currentSpawnRotation = spawnRotations[currentSpawnRotationIndex];
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

    public void UpdateSpawnTransform(Transform placeTransform)
    {
        placeTransform.position = GetSpawnPos();
        Quaternion newRot = new Quaternion();
        newRot.SetLookRotation(currentSpawnRotation);
        placeTransform.rotation = newRot;
        //placeTransform.localEulerAngles = currentSpawnRotation;
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
        UpdateSpawnTransform(fence.transform);

        //Use up a fence in the inventory
        inv.RemoveItem(GetComponent<ItemBehaviour>().slot.ItemDetails, 1);

        nextPrimaryFire = Time.time + 0.5f;
    }

    public void SecondaryFire(Inventory inv)
    {
        

    }

    void Update()
    {
        if (ghostProp != null)
        {
            UpdateSpawnTransform(ghostProp.transform);
            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateGhost();
            }
        }
    }
}
