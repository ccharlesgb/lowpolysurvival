using UnityEngine;
using System.Collections;

public class Crate : MonoBehaviour, IUseable
{
    void Start()
    {
        GetComponent<Inventory>().AddItem("Wood", 45);

    }

    public void OnHoverStart()
    {
        GetComponent<MeshRenderer>().material.color = Color.red;
    }

    public void OnHoverEnd()
    {
        GetComponent<MeshRenderer>().material.color = Color.white;
    }

    public void OnUse(GameObject user)
    {
        Inventory useInv = user.GetComponent<Inventory>();
        Inventory mInv = GetComponent<Inventory>();
        useInv.BeginLooting(mInv);
    }
}
