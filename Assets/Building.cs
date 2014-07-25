using System;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using System.Collections;

//Information on an attachment point on a structure
[System.Serializable]
public class AttachmentPoint
{
    public Vector3 point;
    public Vector3 normal;
    public Structure structure;

    public Structure.StructureType type;

    public Vector3 GetGlobalPoint()
    {
        return structure.transform.TransformPoint(point);
    }

    public Vector3 GetGlobalNormal()
    {
        return structure.transform.TransformDirection(normal);
    }

}

//Store information on how to snap two attachment points
public class AttachmentSnap
{
    public bool IsValid = false;
    public AttachmentSnap(AttachmentPoint f, AttachmentPoint s)
    {
        first = f;
        second = s;
        IsValid = (first != null && second != null);
    }
    public AttachmentPoint first;
    public AttachmentPoint second;
}

public class Building : MonoBehaviour 
{

    public AttachmentSnap lastSnap = null; //Debug


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

    public void AddStructure(Structure struc, AttachmentSnap snap)
    {
        if (snap == null || !snap.IsValid)
            return;

        Structures.Add(struc);
        foreach (AttachmentPoint point in struc.Attachments)
        {
            Debug.Log("ADDING");
            AvailableAttachments.Add(point);
        }

        lastSnap = snap;

        AvailableAttachments.Remove(snap.first);
        AvailableAttachments.Remove(snap.second);
    }

    //Get the closest attachment point to this vector out of a list
    public AttachmentPoint GetClosestAttachment(Vector3 point, List<AttachmentPoint> points)
    {
        float minDist2 = float.PositiveInfinity;
        AttachmentPoint closest = null;
        foreach (AttachmentPoint attach in points)
        {
            float dist = (attach.GetGlobalPoint() - point).sqrMagnitude;
            if (dist < minDist2)
            {
                minDist2 = dist;
                closest = attach;
            }
        }
        return closest;
    }
    
    public AttachmentSnap PreviewSnapPosition(Vector3 builder, Structure toAdd)
    {
        float dotThreshold = -1.0f;

        var candidates = new List<AttachmentPoint>();

        Vector3 fromAttachToBuilder;
        Vector3 currentNormal;
        float buildRange = 8.0f;
        foreach (AttachmentPoint curPoint in AvailableAttachments)
        {
            //We want to get the attachment points that are pointing towards the player.
            fromAttachToBuilder = (builder - curPoint.GetGlobalPoint());
            float dist = fromAttachToBuilder.magnitude;
            fromAttachToBuilder.Normalize();
            currentNormal = curPoint.GetGlobalNormal();
            float dot = Vector3.Dot(fromAttachToBuilder, currentNormal); //If +VE then pointing towards
            if (dot > dotThreshold && dist < buildRange) //Its pointing towards us enough add to the candidate list
            {
                candidates.Add(curPoint);
            }
        }

        //Next we need to get EVERY combination of the structures attachment points with the candidates
        //The best attachment point will be the one that has an antinormal with the candidate.
        //And is also the closest
        //Think of it as a plug entering a socket
        float minDot = 1.0f;
        AttachmentPoint bestCandidate = null;
        AttachmentPoint bestStructurePoint = null;

        float minDist = 10000000.0f;

        foreach (AttachmentPoint curCandidate in candidates)
        {
            foreach (AttachmentPoint curAttach in toAdd.Attachments)
            {
                float dot = Vector3.Dot(curCandidate.GetGlobalNormal(), curAttach.GetGlobalNormal()); //If +VE then pointing towards
                float distanceToBuilder = Vector3.Distance(curCandidate.GetGlobalPoint(), builder);
                if (dot < -0.1f && distanceToBuilder < minDist) //More antinormal than before
                {
                    bestCandidate = curCandidate;
                    bestStructurePoint = curAttach;
                    minDist = Vector3.Distance(curCandidate.GetGlobalPoint(), builder);
                }
            }  
        }
        AttachmentSnap snap = new AttachmentSnap(bestCandidate, bestStructurePoint);
        lastSnap = snap;
        return snap;
    }

    void OnDrawGizmos()
    {
        foreach (AttachmentPoint p in AvailableAttachments)
        {
            Vector3 start = p.structure.transform.TransformPoint(p.point);
            Vector3 end = p.structure.transform.TransformPoint(p.point + p.normal * 2.0f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(start, end);
        }

        if (lastSnap != null && lastSnap.IsValid)
        {
            Vector3 start = lastSnap.first.structure.transform.TransformPoint(lastSnap.first.point);
            Vector3 end = lastSnap.first.structure.transform.TransformPoint(lastSnap.first.point + lastSnap.first.normal * 1.0f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(start, end);

            start = lastSnap.second.structure.transform.TransformPoint(lastSnap.second.point);
            end = lastSnap.second.structure.transform.TransformPoint(lastSnap.second.point + lastSnap.second.normal * 2.0f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(start, end);
            Gizmos.DrawSphere(start, 0.25f);
        }
    }
}
