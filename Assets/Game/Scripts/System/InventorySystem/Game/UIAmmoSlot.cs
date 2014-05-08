using UnityEngine;
using System.Collections.Generic;

public class UIAmmoSlot : UIItemSlot
{
	public PlayerObjData Player;
	public UILabel amount_label,cost_label;

	public UIButton BuyButton;

	string AmmoType;
	public bool AllowBuying=true;

	//ammo code

	private void SetAmmoType(string type){
		AmmoType=type;
		label.text=Player.GetAmmoData(type).Name;
		icon.spriteName=Player.GetAmmoData(AmmoType).sprite;
	}
	
	private void SetAmount(int min,int max){
		amount_label.text=min+"/"+max;
	}

	void SetCost (int cost)
	{
		Cost=cost;
		cost_label.text=""+cost+" "+XmlDatabase.MoneyUnit;
	}

	public void SetPlayer(PlayerObjData player,string type){
		Player=player;
		SetAmmoType(type);
		
		UpdateStats();
	}

	int Cost;
	
	void UpdateStats(){
		int amount=Player.GetAmmoAmount(AmmoType);
		var data=Player.GetAmmoData(AmmoType);
		int cost=(int)((data.MaxAmount-amount)*data.Cost);

		if (!AllowBuying||cost==0){
			BuyButton.gameObject.SetActive(false);
			cost_label.gameObject.SetActive(false);
		}

		SetAmount(Player.GetAmmoAmount(AmmoType),Player.GetAmmoData(AmmoType).MaxAmount);
		SetCost(cost);
	}
	
	public void FillAmmo(){
		if (Player.Money>=Cost){
			Player.Money-=Cost;
			
			Player.FillAmmo(AmmoType);
			
			UpdateStats();
		}
	}

	//ui slot code

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
			UpdateStats();
			return item;
		}
		//all ammo fits
		Player.AddAmmoAmount(AmmoType,add);
		UpdateStats();
		return null;
	}
}