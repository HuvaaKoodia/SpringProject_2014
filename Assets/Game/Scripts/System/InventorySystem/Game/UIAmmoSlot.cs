using UnityEngine;
using System.Collections.Generic;

public class UIAmmoSlot : UIItemSlot
{
	public PlayerObjData Player;
	public UIEquipmentSlot.Slot slot;

	public UILabel amount_label;

	string AmmoType;
	
	public void SetAmmoType(string type){
		AmmoType=type;
		label.text=Player.GetAmmoData(type).Name;
		icon.spriteName=Player.GetAmmoData(AmmoType).sprite;
	}
	
	public void SetAmount(int min,int max){
		amount_label.text=min+"/"+max;
	}

    //DEv. puukkoa! Color all slots
    public static List<UIAmmoSlot> EquipmentSlots;
	
    public void SetSlotColor(InvGameItem item){
        if (item==null||item.baseItem.type!=InvBaseItem.Type.AmmoBox){
            background.color=Color.white;
            return;
        }

		if (item.baseItem.ammotype==AmmoType){
            background.color=Color.green;
        }
        else{
            background.color=Color.red;
        }
    }
    //DEV. puukko end

	void Start(){
        if (EquipmentSlots==null)
            EquipmentSlots=new List<UIAmmoSlot>();
        EquipmentSlots.Add(this);
	}

    void OnSceneLoad(){
        if (EquipmentSlots!=null) EquipmentSlots.Clear();
    }

	override protected InvGameItem observedItem
	{
		get{ return null;}
	}

	/// <summary>
	/// Replace the observed item with the specified value. Should return the item that was replaced.
	/// </summary>

	override protected InvGameItem Replace (InvGameItem item)
	{
		if (item==null) return null;
		if (item.baseItem.type!=InvBaseItem.Type.AmmoBox||item.baseItem.ammotype!=AmmoType) return item;

		int max=Player.GetAmmoData(AmmoType).MaxAmount,amount=Player.GetAmmoAmount(AmmoType), add=item.GetStat(InvStat.Type.Amount)._amount;
		int dif=max-(amount+add);
		if (dif<0){
			//some ammo left over
			Player.AddAmmoAmount(AmmoType,add);
			item.GetStat(InvStat.Type.Amount)._amount=-dif;
			return item;
		}
		//all ammo fits
		Player.AddAmmoAmount(AmmoType,add);
		return null;
	}
    
    new public void Update(){
        base.Update();


		SetAmount(Player.GetAmmoAmount(AmmoType),Player.GetAmmoData(AmmoType).MaxAmount);
    }
}