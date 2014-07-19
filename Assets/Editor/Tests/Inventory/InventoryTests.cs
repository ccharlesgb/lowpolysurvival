using LowPolySurvival.Inventory;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
[Category("Inventory")]
public class InventoryTests : UnityUnitTest
{

	private GameObject _gameObject;
	private Inventory _inventory;

	private ItemDetails _stackableItemDetails;
	private ItemDetails _notStackableItemDetails;

	[SetUp]
	public void Init() {
		_gameObject = CreateGameObject();
		_gameObject.AddComponent<Inventory>();

		_inventory = _gameObject.GetComponent<Inventory>();
		_inventory.InventoryMaxSize = 5;
		_inventory.Items = new ItemSlot[5];

		_stackableItemDetails = new ItemDetails() { isStackable = true, stackSize = 5 };
		_notStackableItemDetails = new ItemDetails() { isStackable = false, stackSize = 1 };
	}

	#region ClampItemAmount
	[Test]
	[Category("ClampItemAmount")]
	public void it_should_clamp_when_item_is_not_stackable()
	{
		int amount = _inventory.ClampItemAmount(5, _notStackableItemDetails);
		Assert.AreEqual(1, amount, "Amount should be 1 when item is not stackable.");
	}

	[Test]
	[Category("ClampItemAmount")]
	public void it_should_clamp_when_amount_is_less_than_stackSize()
	{
		int amount = _inventory.ClampItemAmount(2, _stackableItemDetails);
		Assert.AreEqual(2, amount, "Amount should be 'amount' when amount is less than stacksize.");
	}

	[Test]
	[Category("ClampItemAmount")]
	public void it_should_clamp_when_amount_is_more_than_stackSize()
	{
		int amount = _inventory.ClampItemAmount(10, _stackableItemDetails);
		Assert.AreEqual(5, amount, "Amount should be 'stackSize' when amount is more than stacksize.");
	}
	#endregion

	#region GetSpaceForItem
	[Test]
	[Category("GetSpaceForItem")]
	public void GetSpaceForItem_item_is_not_stackable()
	{
		Assert.AreEqual(5, _inventory.GetSpaceForItem(_notStackableItemDetails));

		_inventory.Items[0] = new ItemSlot() { ItemDetails = _notStackableItemDetails, SlotID = 0, Amount = 1 };
		_inventory.Items[3] = new ItemSlot() { ItemDetails = _notStackableItemDetails, SlotID = 3, Amount = 1 };

		Assert.AreEqual(3, _inventory.GetSpaceForItem(_notStackableItemDetails));
	}

	[Test]
	[Category("GetSpaceForItem")]
	public void GetSpaceForItem_item_is_stackable()
	{
		Assert.AreEqual(5 * 5, _inventory.GetSpaceForItem(_stackableItemDetails));

		_inventory.Items[0] = new ItemSlot() { ItemDetails = _stackableItemDetails, SlotID = 0, Amount = 5 };
		_inventory.Items[3] = new ItemSlot() { ItemDetails = _stackableItemDetails, SlotID = 3, Amount = 3 };

		Assert.AreEqual(3 * 5 + 2, _inventory.GetSpaceForItem(_stackableItemDetails));
	}
	#endregion

	#region FindForstSlotWithSpace
	[Test]
	public void FindFirstSlotWithSpace_item_is_not_stackable()
	{
		Assert.AreEqual(0, _inventory.FindFirstSlotWithSpace(_notStackableItemDetails), "Should return 0 when inventory is empty");

		_inventory.Items[0] = new ItemSlot() { ItemDetails = _notStackableItemDetails, SlotID = 0, Amount = 1 };
		_inventory.Items[3] = new ItemSlot() { ItemDetails = _notStackableItemDetails, SlotID = 3, Amount = 1 };

		Assert.AreEqual(1, _inventory.FindFirstSlotWithSpace(_notStackableItemDetails), "Should return 1 when slot 0 and 3 is taken");

		// Full
		_inventory.Items[1] = new ItemSlot() { ItemDetails = _notStackableItemDetails, SlotID = 1, Amount = 1 };
		_inventory.Items[2] = new ItemSlot() { ItemDetails = _notStackableItemDetails, SlotID = 3, Amount = 1 };
		_inventory.Items[4] = new ItemSlot() { ItemDetails = _notStackableItemDetails, SlotID = 4, Amount = 1 };

		Assert.AreEqual(-1, _inventory.FindFirstSlotWithSpace(_notStackableItemDetails), "Should return -1 when inventory has no free space");
	}

	[Test]
	public void FindFirstSlotWithSpace_item_is_stackable()
	{
		Assert.AreEqual(0, _inventory.FindFirstSlotWithSpace(_stackableItemDetails), "Should return 0 when inventory is empty");

		_inventory.Items[0] = new ItemSlot() { ItemDetails = _stackableItemDetails, SlotID = 0, Amount = 1 };
		_inventory.Items[3] = new ItemSlot() { ItemDetails = _stackableItemDetails, SlotID = 3, Amount = 5 };
		Assert.AreEqual(0, _inventory.FindFirstSlotWithSpace(_stackableItemDetails), "Should return 0 when slot 0 has amount 1/5");

		_inventory.Items[0] = new ItemSlot() { ItemDetails = _stackableItemDetails, SlotID = 0, Amount = 5 };
		Assert.AreEqual(1, _inventory.FindFirstSlotWithSpace(_stackableItemDetails), "Should return 1 when slot 0 is full.");

		ItemDetails otherItemDetails = new ItemDetails() { isStackable = true, stackSize = 5 };

		// Full
		_inventory.Items[1] = new ItemSlot() { ItemDetails = otherItemDetails, SlotID = 1, Amount = 1 };
		_inventory.Items[2] = new ItemSlot() { ItemDetails = _stackableItemDetails, SlotID = 3, Amount = 5 };
		_inventory.Items[4] = new ItemSlot() { ItemDetails = _stackableItemDetails, SlotID = 4, Amount = 5 };

		Assert.AreEqual(-1, _inventory.FindFirstSlotWithSpace(_stackableItemDetails), "Should return -1 when inventory has no free space");
	}
	#endregion

	// public void AddItem();

	[Test]
	public void MoveToSlot()
	{
		// MoveToSlot(ItemSlot slot, int newSlotID)
		ItemSlot itemSlot = new ItemSlot() {ItemDetails = _stackableItemDetails, Amount = 2, SlotID = 3};
		_inventory.Items[3] = itemSlot;

		_inventory.MoveToSlot(itemSlot, 1);

		Assert.AreEqual(itemSlot, _inventory.Items[1]);
		Assert.AreEqual(1, itemSlot.SlotID);
	}

	[Test]
	public void SwapSlots()
	{
		// MoveToSlot(ItemSlot slot, int newSlotID)
		ItemSlot itemSlot1 = new ItemSlot() { ItemDetails = _stackableItemDetails, Amount = 2, SlotID = 2 };
		ItemSlot itemSlot2 = new ItemSlot() { ItemDetails = _stackableItemDetails, Amount = 2, SlotID = 4 };
		_inventory.Items[2] = itemSlot1;
		_inventory.Items[4] = itemSlot2;

		_inventory.SwapSlots(itemSlot1, itemSlot2);

		Assert.AreEqual(itemSlot1, _inventory.Items[4]);
		Assert.AreEqual(4, itemSlot1.SlotID);

		Assert.AreEqual(itemSlot2, _inventory.Items[2]);
		Assert.AreEqual(2, itemSlot2.SlotID);
	}

	// public void TransferItem();
	
	[Test]
	public void GetTotalAmount()
	{
		Assert.AreEqual(0, _inventory.GetTotalAmount(_stackableItemDetails), "Should return 0 if inventory is empty.");
		_inventory.Items[0] = new ItemSlot() { ItemDetails = _stackableItemDetails, SlotID = 0, Amount = 5 };
		_inventory.Items[1] = new ItemSlot() { ItemDetails = _stackableItemDetails, SlotID = 1, Amount = 2 };
		_inventory.Items[3] = new ItemSlot() { ItemDetails = _stackableItemDetails, SlotID = 3, Amount = 3 };

		Assert.AreEqual(10, _inventory.GetTotalAmount(_stackableItemDetails), "Should return 10, if we have items with stackSize, 5, 2, 3");
	}

	[Test]
	public void RemoveItem()
	{
		Assert.AreEqual(null, _inventory.Items[0]);

		_inventory.Items[0] = new ItemSlot() { ItemDetails = _stackableItemDetails, SlotID = 0, Amount = 5 };
		_inventory.RemoveItem(0);

		Assert.AreEqual(null, _inventory.Items[0]);
	}







}