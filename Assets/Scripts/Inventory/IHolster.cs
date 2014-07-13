using UnityEngine;
using System.Collections;

/// <summary>
///		Interface for holsterable items.
/// </summary>
public interface IHolster
{
	void PrimaryFire(Inventory ownerInv);
	void SecondaryFire(Inventory ownerInv);
}
