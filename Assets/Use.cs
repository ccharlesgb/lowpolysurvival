using UnityEngine;
using System.Collections;

//Handles using of objects that have a useable component
public class Use : MonoBehaviour
{

    public KeyCode useKey = KeyCode.E; //Which key activates use

    public IUseable currentHover; //Which object we are currently hovering over

    public float useDistance = 10.0f;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    UpdateHovering();

	    if (currentHover != null && Input.GetKeyDown(useKey))
	    {
	        currentHover.OnUse();
	    }
	}

    void UpdateHovering()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        RaycastHit hit;
        Physics.Raycast(ray, out hit);

        if (hit.collider == null || hit.distance > useDistance) //Not hovering over anything
        {
            if (currentHover != null)
            {
                currentHover.OnHoverEnd();
                currentHover = null;
            }
            return;
        }

        IUseable useable = hit.collider.gameObject.GetInterface<IUseable>();
        if (useable != null)
        {
            if (currentHover != null && useable != currentHover)
                currentHover.OnHoverEnd();

            useable.OnHoverStart();
            currentHover = useable;
        }
    }
}
