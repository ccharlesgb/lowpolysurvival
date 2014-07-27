using System.Collections.Generic;
using UnityEngine;
using System.Collections;

//Store information on how to snap two attachment points
public class AttachmentSnap
{
    public bool IsValid()
    {
        return (first != null && second != null);
    }
    public AttachmentSnap(AttachmentPoint f, AttachmentPoint s)
    {
        first = f;
        second = s;
    }
    public AttachmentPoint first;
    public AttachmentPoint second;
}

public class Structure : MonoBehaviour
{
    public enum StructureType
    {
        Foundation,
        Wall
    }

    public AttachmentSnap lastSnap;

    public StructureType type;

    public List<AttachmentPoint> Attachments = new List<AttachmentPoint>();

    public float YawOffset = 0.0f;

	void Awake() 
    {
        //Cache all the attachment points in this object
	    foreach (AttachmentPoint attach in GetComponentsInChildren<AttachmentPoint>())
	    {
	        attach.structure = this;
	        Attachments.Add(attach);
	    }
	    ActivateAttachments();
    }

    public void ActivateAttachments()
    {
        foreach (AttachmentPoint curPoint in Attachments)
        {
            curPoint.Activate();
        }
    }

    public void DeActivateAttachments()
    {
        foreach (AttachmentPoint curPoint in Attachments)
        {
            curPoint.DeActivate();
        }
    }

    public AttachmentSnap GetSnapPosition(AttachmentPoint rayHit, List<AttachmentPoint> structToSnap)
    {
        AttachmentPoint bestStructurePoint = null;
        foreach (AttachmentPoint curPoint in structToSnap)
        {
            float dot = Vector3.Dot(curPoint.GetGlobalNormal(), rayHit.GetGlobalNormal());
            if (dot < -0.9f)
            {
                bestStructurePoint = curPoint;
            }
        }
        lastSnap = new AttachmentSnap(rayHit, bestStructurePoint);
        return lastSnap;
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

    /*
     *         AttachmentPoint bestStructurePoint = null;
        foreach (AttachmentPoint curPoint in toAdd.Attachments)
        {
            float dot = Vector3.Dot(curPoint.GetGlobalNormal(), closestPoint.GetGlobalNormal());
            if (dot < -0.9f)
            {
                bestStructurePoint = curPoint;
            }
        }*/

    void OnDrawGizmos()
    {
        if (lastSnap != null && lastSnap.IsValid())
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
