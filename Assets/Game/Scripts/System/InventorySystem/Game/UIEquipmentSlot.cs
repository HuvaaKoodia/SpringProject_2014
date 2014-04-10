using UnityEngine;
using System.Collections.Generic;

public class UIEquipmentSlot : UIItemSlot
{
	public enum Slot
	{
		None=-1,
		WeaponRightHand=0,
		WeaponLeftHand=1,
		WeaponRightShoulder=2,
		WeaponLeftShoulder=3,
		Utility1=4,
		Utility2=5,
		Utility3=6,
		Utility4=7,
		UpperTorso=8,
        LowerTorso=9,
		RecycleBin=10,
		_Amount
	}
	public InvEquipmentStorage equipment;
	public Slot slot;

    public GameObject DisabledSprite,HeatSprite;

    //DEv. puukkoa! Color all slots
    public static List<UIEquipmentSlot> EquipmentSlots;
   
    public void SetSlotColor(InvGameItem item){
        if (item==null){
            background.color=Color.white;
            return;
        }
		var s=equipment.GetSlot(slot);
		if (s.HasType(item.baseItem.type)&&s.ObjData.USABLE&&s.ObjData.CHANGEABLE){
            background.color=Color.green;
        }
        else{
            background.color=Color.red;
        }
    }
    //DEV. puukko end

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
        if (equipment!=null&&!equipment.CanReplace(slot, item)) return item;
        if (CheckVendorSlotItemInteractions(item)) return item;

        if (OnItemReplaceEvent!=null){
            if (OnItemReplaceEvent(item)) return item;
        }

		if (slot==Slot.RecycleBin){
            if (item.baseItem.type==InvBaseItem.Type.QuestItem)
                return item;
			return null;
		}

		return (equipment != null) ? equipment.Replace(slot, item) : item;
	}
    
    new public void Update(){
        base.Update();
        UpdateSlotCondition();
    }

    void UpdateSlotCondition(){
        if (equipment==null) return;
        var s=equipment.GetSlot(slot);
        if (s!=null){
			if (HeatSprite!=null&&s.ObjData.USABLE){
				HeatSprite.SetActive(!s.ObjData.CHANGEABLE);
			}
            DisabledSprite.SetActive(!s.ObjData.USABLE);
		}
    }
}