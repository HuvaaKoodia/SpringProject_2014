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
	public Slot slot;

    //DEv. puukkoa!
    public static List<UIEquipmentSlot> EquipmentSlots;
   
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
        if (EquipmentSlots==null)
            EquipmentSlots=new List<UIEquipmentSlot>();
        EquipmentSlots.Add(this);
	}

    void OnSceneLoad(){
        if (EquipmentSlots!=null)
            EquipmentSlots.Clear();
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
		if (slot==Slot.RecycleBin){
            if (item.baseItem.type==InvBaseItem.Type.QuestItem)
                return item;
			return null;
		}
		return (equipment != null) ? equipment.Replace(slot, item) : item;
	}
}