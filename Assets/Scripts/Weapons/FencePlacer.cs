using System.Linq;
using System.Runtime.InteropServices;
using LowPolySurvival.Inventory;
using UnityEditor.ProjectWindowCallback;
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
    private bool validSpawn = false;

    public Color ghostColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);

    public float BuildingRange = 20.0f;
    public Building CurrentBuilding = null;

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
        Vector3 spawnPos = Vector3.zero;
        ghostProp = Instantiate(fencePrefab, spawnPos, Quaternion.identity) as GameObject;
        GetSpawnTransform(ghostProp.transform);
        if (ghostProp.renderer != null)
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

    public void GetSpawnTransform(Transform tran)
    {
        tran.position = Vector3.zero;
        //We need a building
        //SHOULDNT BE DOING THIS ALL THE TIME
        if (CurrentBuilding == null)
        {
            CurrentBuilding = Building.FindNearestBuilding(transform.position + transform.forward * 2.0f, BuildingRange);
        }
        if (CurrentBuilding == null) return; //Couldnt find a building so we cant spawn (TODO making new buildings)
        
        Ray lookRay = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(lookRay, out hit))
        {
            Quaternion newRot = new Quaternion();
            newRot.SetLookRotation(currentSpawnRotation);
            tran.rotation = newRot;
            AttachmentSnap bestAttachment = CurrentBuilding.PreviewSnapPosition(hit.point, ghostProp.GetComponent<Structure>());
            if (bestAttachment != null && bestAttachment.IsValid)
            {
                //Debug.Log(bestAttachment.first.point + "   " + bestAttachment.second.point);
                tran.position = bestAttachment.first.GetGlobalPoint() -
                                bestAttachment.second.structure.transform.TransformPoint(bestAttachment.second.point);
            }
            else
            {
                tran.position = hit.point;
            }
        }
    }

    public void PrimaryFire(Inventory inv)
    {
        if (nextPrimaryFire > Time.time)
        {
            return; //Cant fire yet
        }

        //Spaw the fence prefab
        GetSpawnTransform(ghostProp.transform);
        GameObject fence = Instantiate(fencePrefab, ghostProp.transform.position, ghostProp.transform.rotation) as GameObject;

        //TODO Having to do this twice is bad
        AttachmentSnap bestAttachment = CurrentBuilding.PreviewSnapPosition(transform.position, fence.GetComponent<Structure>());
        CurrentBuilding.AddStructure(fence.GetComponent<Structure>(), bestAttachment);

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
            GetSpawnTransform(ghostProp.transform);
            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateGhost();
            }
        }
    }
}
