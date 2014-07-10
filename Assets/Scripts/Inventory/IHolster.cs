using UnityEngine;
using System.Collections;

public interface IHolster
{
	void PrimaryFire(Inventory ownerInv);
	void SecondaryFire(Inventory ownerInv);
}
