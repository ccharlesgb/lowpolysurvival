using LowPolySurvival.Inventory;
using UnityEngine;
using System.Collections;

public class Crate : MonoBehaviour, IUseable
{

	private Inventory _inventory ;

    void Start()
    {
	    _inventory = GetComponent<Inventory>();

        _inventory.AddItem("Wood", 20);
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
	    Transform userInventoryTransform = user.transform.FindChild("Inventory");
	    if (userInventoryTransform == null) return;

	    GameObject userInventory = userInventoryTransform.gameObject;
        Inventory userInventoryComponent = userInventory.GetComponent<Inventory>();

	    if (userInventoryComponent == null) return;

        userInventoryComponent.BeginLooting(_inventory);
    }
}
