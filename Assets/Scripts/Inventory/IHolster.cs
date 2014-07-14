using System;
using UnityEngine;
using System.Collections;

/// <summary>
///		Interface for holsterable items.
/// </summary>
public interface IHolster
{
    void OnHolster(Inventory ownerInv);
    Void OnDeHolster(Inventory ownerInv);
	void PrimaryFire(Inventory ownerInv);
	void SecondaryFire(Inventory ownerInv);
}
