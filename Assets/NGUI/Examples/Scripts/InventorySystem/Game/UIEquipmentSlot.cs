using UnityEngine;

public class UIEquipmentSlot : UIItemSlot
{
	public enum Slot
	{
		None=-1,
		WeaponRightHand,
		WeaponLeftHand,
		WeaponRightShoulder,
		WeaponLeftShoulder,
		Utility1,
		Utility2,
		Utility3,
		Utility4,

		_Amount
	}

	public InvEquipmentStorage equipment;
	public Slot slot;
	
	override protected InvGameItem observedItem
	{
		get
		{
			return (equipment != null) ? equipment.GetItem(slot) : null;
		}
	}

	/// <summary>
	/// Replace the observed item with the specified value. Should return the item that was replaced.
	/// </summary>

	override protected InvGameItem Replace (InvGameItem item)
	{
		//quick hax
		return (equipment != null) ? equipment.Replace(slot, item) : item;
	}
}