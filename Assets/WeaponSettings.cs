using LowPolySurvival.Inventory;
using UnityEngine;
using System.Collections;

public class WeaponSettings : MonoBehaviour, IHolster
{
    public enum WeaponFireType
    {
        Automatic,
        Manual
    }

    public Inventory _owner;

    public string PrimaryAmmoName = "";
    public int PrimaryAmmoMax = -1; //-1 means disabled (No Max)
    public int PrimaryAmmoClipSize = -1; //-1 means disabled (No clips/infinite clips)
    public float PrimaryFireDelay = 0.5f;
    public float PrimaryFireReloadTime = 0.5f;
    public WeaponFireType PrimaryFireType = WeaponFireType.Automatic;

    public string SecondaryAmmoName = "";

    private float _nextPrimaryFire = 0.0f;
    private float _reloadEndTime = 0.0f;

    private bool _canPrimaryFire = false;

    private bool _hasReleasedPrimary = true; //Used by manual firemode
    private bool _hasReleasedSecondary = true;

    public bool CanPrimaryFire()
    {
        return _canPrimaryFire;
    }

    public void PrimaryFire(Inventory ownerInv)
    {
        _hasReleasedPrimary = false;
    }

    public void SecondaryFire(Inventory ownerInv)
    {

    }

    public int PrimaryAmmoCount()
    {
        if (_owner == null) return -1;

        return _owner.GetTotalAmount(PrimaryAmmoName);
    }

    public int SecondaryAmmoCount()
    {
        if (_owner == null) return -1;

        return _owner.GetTotalAmount(SecondaryAmmoName);
    }

    public bool UpdateCanPrimaryFire()
    {
        if (PrimaryAmmoName != "" && PrimaryAmmoCount() <= 0)
            return false;
        if (Time.time + PrimaryFireDelay < _nextPrimaryFire)
            return false;

        if (PrimaryFireType == WeaponFireType.Manual && !_hasReleasedPrimary)
            return false;

        return true;
    }

    void Update()
    {
        if (Input.GetAxis("Fire1") < 0.5f) //We arent holding fire
            _hasReleasedPrimary = true;
        if (Input.GetAxis("Fire2") < 0.5f)
            _hasReleasedSecondary = true;

       _canPrimaryFire = UpdateCanPrimaryFire();
    }
    
    //Just as we get the game object out
    public void OnHolster(Inventory ownerInv)
    {
        _owner = ownerInv;
    }

    //Called just before the gameobject is detroyed (being put away)
    public void OnDeHolster(Inventory ownerInv)
    {
        _owner = null;
    }

}
