using System.CodeDom;
using System.Security.Permissions;
using UnityEngine;
using System.Collections;

//Information on an attachment point on a structure
public class AttachmentPoint : MonoBehaviour
{
    public float attachmentRadius = 0.08f;

    public Vector3 point;
    public Vector3 normal;
    public Structure structure;

    public Structure.StructureType type;

    private Transform _transform;
    private SphereCollider _collider;

    void Awake()
    {
        _transform = transform;
    }

    //We are being asked to be built on so we need to turn on our colliders
    public void Activate()
    {
        point = transform.localPosition;
        _collider = GetComponent<SphereCollider>();
        if (_collider == null) //First time we have been activated
        {
            _collider = gameObject.AddComponent<SphereCollider>();
        }
        _collider.enabled = true;
        _collider.isTrigger = true;
        _collider.radius = attachmentRadius;
        _collider.center = normal * attachmentRadius;
    }

    public void DeActivate()
    {
        if (_collider == null) return;

        _collider.enabled = false;

    }

    public Vector3 GetGlobalPoint()
    {
        return transform.TransformPoint(point);
    }

    public Vector3 GetGlobalNormal()
    {
        return transform.TransformDirection(normal);
    }

    void OnDrawGizmos()
    {
        Vector3 start = transform.position;
        Vector3 end = transform.position + GetGlobalNormal() * 0.5f;
        Gizmos.DrawLine(start, end);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(start + GetGlobalNormal() * attachmentRadius, attachmentRadius);
    }
}