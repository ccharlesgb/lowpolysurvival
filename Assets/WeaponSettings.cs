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

    public string PrimaryAmmoName = "";
    public int PrimaryAmmoMax = -1; //-1 means disabled (No Max)
    public int PrimaryAmmoClipSize = -1; //-1 means disabled (No clips/infinite clips)
    public float PrimaryFireDelay = 0.5f;
    public float PrimaryFireReloadTime = 0.5f;
    public WeaponFireType PrimaryFireType = WeaponFireType.Automatic;

    private float _nextPrimaryFire = 0.0f;
    private float _reloadEndTime = 0.0f;

    public bool CanFire()
    {
        return true;
    }

    void Update()
    {
        
    }

    void OnHolster()
    {
        
    }

}
