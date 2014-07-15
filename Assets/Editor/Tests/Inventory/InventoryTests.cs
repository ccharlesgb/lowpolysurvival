using NSubstitute;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
[Category("Inventory")]
public class InventoryTests : UnityUnitTest
{

	private GameObject _gameObject;
	private Inventory _inventory;

	[SetUp]
	public void Init() {
		_gameObject = CreateGameObject();
		_gameObject.AddComponent<Inventory>();

		_inventory = _gameObject.GetComponent<Inventory>();
		_inventory.InventoryMaxSize = 5;
		_inventory.Items = new ItemSlot[5];
	}

	#region ClampItemAmount
	[Test]
	[Category("ClampItemAmount")]
	public void it_should_clamp_when_item_is_not_stackable()
	{
		var itemDetails = new ItemDetails() { isStackable = false };

		int amount = _inventory.ClampItemAmount(5, itemDetails);
		Assert.AreEqual(1, amount, "Amount should be 1 when item is not stackable.");
	}

	[Test]
	[Category("ClampItemAmount")]
	public void it_should_clamp_when_amount_is_less_than_stackSize()
	{
		var itemDetails = new ItemDetails() { isStackable = true, stackSize = 5 };

		int amount = _inventory.ClampItemAmount(2, itemDetails);
		Assert.AreEqual(2, amount, "Amount should be 'amount' when amount is less than stacksize.");
	}

	[Test]
	[Category("ClampItemAmount")]
	public void it_should_clamp_when_amount_is_more_than_stackSize()
	{
		var itemDetails = new ItemDetails() { isStackable = true, stackSize = 5 };

		int amount = _inventory.ClampItemAmount(10, itemDetails);
		Assert.AreEqual(5, amount, "Amount should be 'stackSize' when amount is more than stacksize.");
	}
	#endregion

	#region GetSpaceForItem
	[Test]
	[Category("GetSpaceForItem")]
	public void GetSpaceForItem_item_is_not_stackable()
	{
		var itemDetails = new ItemDetails() { isStackable = false, stackSize = 1 };

		Assert.AreEqual(5, _inventory.GetSpaceForItem(itemDetails));

		_inventory.Items[0] = new ItemSlot() { ItemDetails = itemDetails, SlotID = 0, Amount = 1 };
		_inventory.Items[3] = new ItemSlot() { ItemDetails = itemDetails, SlotID = 3, Amount = 1 };

		Assert.AreEqual(3, _inventory.GetSpaceForItem(itemDetails));
	}

	[Test]
	[Category("GetSpaceForItem")]
	public void GetSpaceForItem_item_is_stackable()
	{
		var itemDetails = new ItemDetails() { isStackable = true, stackSize = 5 };

		Assert.AreEqual(5 * 5, _inventory.GetSpaceForItem(itemDetails));

		_inventory.Items[0] = new ItemSlot() { ItemDetails = itemDetails, SlotID = 0, Amount = 5 };
		_inventory.Items[3] = new ItemSlot() { ItemDetails = itemDetails, SlotID = 3, Amount = 3 };

		Assert.AreEqual(3 * 5 + 2, _inventory.GetSpaceForItem(itemDetails));
	}
	#endregion


}