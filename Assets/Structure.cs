using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Structure : MonoBehaviour
{
    public List<AttachmentPoint> Attachments = new List<AttachmentPoint>();

    public Building mBuilding = null;

	// Use this for initialization
	void Start () 
    {
        //Cache all the attachment points in this object
	    foreach (Transform attach in GetComponentsInChildren<Transform>())
	    {
	        if (attach.gameObject.name == "Attach")
	        {
	            AttachmentPoint point = new AttachmentPoint();
	            point.point = attach.position;
	            point.structure = this;
	            Attachments.Add(point);
	            Destroy(attach.gameObject);
	        }
	    }
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
}
