using System.Security.Policy;
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

    public string AmmoName = "";
    public int AmmoMax = -1; //-1 means disabled (No Max)
    public int AmmoClipSize = -1; //-1 means disabled (No clips/infinite clips)
    public float FireDelay = 0.8f;
    public float FireReloadTime = 2.0f;
    public WeaponFireType FireType = WeaponFireType.Automatic;

    public float _nextFire = 0.0f;
    private float _reloadEndTime = 0.0f;
    private int _currentClip = 0;

    private bool _canFire = false;

    private bool _hasReleased = true; //Used by manual firemode

    public AudioClip FireSound;
    public AudioClip ReloadSound;
    public AudioClip OutOfAmmoSound;
    private AudioSource _AudioSource;

    void Awake()
    {
        _AudioSource = GetComponent<AudioSource>();
    }

    public bool CanFire()
    {
        return _canFire;
    }

    public void PrimaryFire(Inventory ownerInv)
    {
        _hasReleased = false;
    }

    public void SecondaryFire(Inventory ownerInv)
    {
        _hasReleased = false;
    }

    public void Reload()
    {
        //Play audio
        if (FireSound != null)
        {
            _AudioSource.clip = ReloadSound;
            _AudioSource.Play();
        }
        _currentClip = AmmoClipSize;
        _nextFire = Time.time + FireReloadTime;
    }

    public void OnFire()
    {
        //Play audio
        if (FireSound != null)
        {
            _AudioSource.clip = FireSound;
            _AudioSource.Play();
        }

        _nextFire = Time.time + FireDelay;

        _currentClip--;
        if (_currentClip <= 0)
        {
            Reload();
        }
    }

    public int AmmoCount()
    {
        if (_owner == null) return -1;

        return _owner.GetTotalAmount(AmmoName);
    }

    public bool UpdateCanFire()
    {
        //Done in order of time to execute (roughly)
        if (Time.time < _nextFire)
            return false;
        if (FireType == WeaponFireType.Manual && !_hasReleased)
            return false;
        if (AmmoName != "" && AmmoCount() <= 0)
            return false;
        if (_currentClip <= 0)
        {
            Reload();
            return false;
        }

        return true;
    }

    void Update()
    {
        if (Input.GetAxis("Fire1") < 0.5f) //We arent holding fire
            _hasReleased = true;

       _canFire = UpdateCanFire();
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
