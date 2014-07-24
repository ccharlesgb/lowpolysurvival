using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Structure : MonoBehaviour
{
    public List<AttachmentPoint> Attachments = new List<AttachmentPoint>();

    public Building mBuilding = null;

    public float YawOffset = 0.0f;

	void Awake() 
    {
        //Cache all the attachment points in this object
	    foreach (Transform attach in GetComponentsInChildren<Transform>())
	    {
	        if (attach.gameObject.name == "Attach")
	        {
	            AttachmentPoint point = new AttachmentPoint();
	            point.point = attach.localPosition;
	            point.structure = this;
	            point.normal = transform.InverseTransformDirection(attach.forward);
	            Attachments.Add(point);
	            Destroy(attach.gameObject);
	        }
	    }
        if (mBuilding != null)
            mBuilding.AddStructure(this);
	}

    public Vector3 GetClosestAttachmentPoint(Vector3 pos)
    {
        for (int i = 0; i < Attachments.Count; i++)
        {
            
        }
        return Vector3.zero;
    }
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnDrawGizmos()
    {
        foreach (AttachmentPoint p in Attachments)
        {
            Vector3 start = transform.TransformPoint(p.point);
            Vector3 end = transform.TransformPoint(p.point + p.normal * 0.5f);
            Gizmos.DrawLine(start, end);

            Gizmos.DrawSphere(start, 0.25f);
        }
    }
}
