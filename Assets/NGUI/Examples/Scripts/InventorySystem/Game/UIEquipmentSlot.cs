using UnityEngine;
using System.Collections.Generic;

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
        UpperTorso,
        LowerTorso,
		RecycleBin,
		_Amount
	}

	public InvEquipmentStorage equipment;
    public InventoryMain Inventory;
	public Slot slot;

	public void UpdateSlotColors(InvGameItem item){
		foreach (var slot in Inventory.EquipmentSlots){
            if (slot.slot!=Slot.RecycleBin)
                slot.SetSlotColor(item);
		}
	}

	public void SetSlotColor(InvGameItem item){
		if (item==null){
			background.color=Color.white;
			return;
		}
		if (equipment.GetSlot(slot).HasType(item.baseItem.type)){
			background.color=Color.green;
		}
		else{
			background.color=Color.red;
		}
	}

	void Start(){
        OnDragStart+=UpdateSlotColors;
	}

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
		if (slot==Slot.RecycleBin){
			return null;
		}
		return (equipment != null) ? equipment.Replace(slot, item) : item;
	}
}