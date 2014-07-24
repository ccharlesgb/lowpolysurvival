using LowPolySurvival.Inventory;
using UnityEngine;
using System.Collections;

public class Pistol : MonoBehaviour, IHolster
{
    public WeaponSettings WeaponInfo;

    private Transform _barrelTransform;

    private LineRenderer _lineRenderer;

    void OnEnable()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.SetVertexCount(2);
        _lineRenderer.enabled = false;
        //Get the position of the barrel
        _barrelTransform = transform.FindChild("Barrel");
        if (_barrelTransform == null)
            _barrelTransform = transform;
    }


    public void ShowBulletTracer(Ray ray)
    {
        _lineRenderer.SetPosition(0, ray.origin);
        _lineRenderer.SetPosition(1, ray.origin + ray.direction * 100.0f);
        _lineRenderer.enabled = true;

        Invoke("HideBulletTracer", 0.1f);
    }

    public void HideBulletTracer()
    {
        _lineRenderer.enabled = false;
    }

    public void PrimaryFire(Inventory ownerInv)
    {
        if (!WeaponInfo.CanPrimaryFire()) return;

        WeaponInfo.OnPrimaryFire();

        RaycastHit hit;

        // Generate a ray from the cursor position
        Ray ray = new Ray();
        ray.origin = _barrelTransform.position;
        ray.direction = _barrelTransform.forward;

        ShowBulletTracer(ray);

        ownerInv.RemoveItem(WeaponInfo.PrimaryAmmoName, 1);

        if (Physics.Raycast(ray, out hit))
        {
            GameObject obj = hit.collider.gameObject;
            if (obj != null)
            {

            }
        }
    }

    public void SecondaryFire(Inventory ownerInv)
    {

    }

    public void OnHolster(Inventory ownerInv)
    {

    }

    public void OnDeHolster(Inventory ownerInv)
    {

    }
}
