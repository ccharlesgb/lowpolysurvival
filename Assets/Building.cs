using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[System.Serializable]
public class AttachmentPoint
{
    public Vector3 point;
    public Structure structure;
}

public class Building : MonoBehaviour 
{
    //VERY SLOW CACHE!
    public static Building FindNearestBuilding(Vector3 point, float range)
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
        float minDist = range * range;
        Building closest = null;
        foreach (GameObject b in buildings)
        {
            Building current = b.GetComponent<Building>();
            if (current != null) //It has a building
            {
                if ((current.transform.position - point).sqrMagnitude < minDist)
                    closest = current;
            }
        }

        return closest;
    }

    public List<Structure> Structures = new List<Structure>();

    public List<AttachmentPoint> AvailableAttachments = new List<AttachmentPoint>(); 

    public void AddStructure(Structure struc)
    {
        Structures.Add(struc);
        foreach (AttachmentPoint point in struc.Attachments)
        {
            AvailableAttachments.Add(point);
        }
    }

    public AttachmentPoint FindBestAttachment(Vector3 pos)
    {
        AttachmentPoint best = null;
        float minDist = 10000000000000000000000.0f;
        foreach (AttachmentPoint attach in AvailableAttachments)
        {
            float curDist = Vector3.Distance(pos, attach.point);
            if (curDist < minDist)
            {
                best = attach;
                minDist = curDist;
            }
        }
        return best;
    }

	void Start () 
    {
	
	}

	void Update () 
    {
	
	}
}
