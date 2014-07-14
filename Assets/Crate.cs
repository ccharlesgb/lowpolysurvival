using UnityEngine;
using System.Collections;

public class Crate : MonoBehaviour, IUseable
{
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
        Debug.Log("YOU USED ME!");
    }
}
