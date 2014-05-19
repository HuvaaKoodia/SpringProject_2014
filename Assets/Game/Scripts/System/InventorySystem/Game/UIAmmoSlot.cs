using UnityEngine;
using System.Collections.Generic;

public class UIAmmoSlot : UIItemSlot
{
	public PlayerObjData Player;
	public UILabel amount_label,cost_label;

	public GameObject Buttons;

	string AmmoType;
	public bool AllowBuying=true;
	
	int Cost,SingleCost,FillCost,FillAmount;
	bool on_hover_single=false,on_hover_all=false;

	//ammo code

	private void SetAmmoType(string type){
		AmmoType=type;
		label.text=Player.GetAmmoData(type).Name;
		icon.spriteName=Player.GetAmmoData(AmmoType).sprite;
	}
	
	private void SetAmount(int min,int max){
		amount_label.text=min+"/"+max;
	}

	public void SetPlayer(PlayerObjData player,string type){
		Player=player;
		SetAmmoType(type);
		
		UpdateStats();
	}

	public void UpdateStats(){
		int amount=Player.GetAmmoAmount(AmmoType);
		var data=Player.GetAmmoData(AmmoType);

		SingleCost=data.Cost;
		FillAmount=(int)Mathf.Floor(Player.Money/SingleCost);
		FillCost=FillAmount*SingleCost;
		Cost=(int)((data.MaxAmount-amount)*SingleCost);

		cost_label.text="";

		SetAmount(Player.GetAmmoAmount(AmmoType),Player.GetAmmoData(AmmoType).MaxAmount);

		if (!AllowBuying||Cost==0){
			Buttons.gameObject.SetActive(false);
			return;
		}

		if (on_hover_single) cost_label.text=""+SingleCost;
		if (on_hover_all) cost_label.text=""+Cost;
	}
	
	public void FillAmmo(){
		if (Player.Money>=Cost){
			Player.Money-=Cost;
			
			Player.FillAmmo(AmmoType);
			UpdateStats();
		}
		else if (Player.Money>=FillCost){
			Player.Money-=FillCost;
			
			Player.AddAmmoAmount(AmmoType,FillAmount);
			UpdateStats();
		}
	}

	public void BuySingleAmmo(){
		if (Player.Money>=SingleCost){
			Player.Money-=SingleCost;
			
			Player.AddAmmoAmount(AmmoType,1);
			UpdateStats();
		}
	}

	public void OnEnterSingle(){
		on_hover_single=true;
		UpdateStats();
	}

	public void OnEnterAll(){
		on_hover_all=true;
		UpdateStats();
	}

	public void OnLeaveSingle(){
		on_hover_single=false;
		UpdateStats();
	}

	public void OnLeaveAll(){
		on_hover_all=false;
		UpdateStats();
	}

	//ui slot code

    //DEv. puukkoa! Color all slots
    public static List<UIAmmoSlot> EquipmentSlots;
	
    public void SetSlotColor(InvGameItem item){

		var scale=GetComponent<UIButtonScale>();//DEV.lazy but works
		
        if (item==null||item.baseItem.type!=InvBaseItem.Type.AmmoBox){
            background.color=Color.white;
			scale.SendMessage("OnPermaHover",false);
            return;
        }

		if (item.baseItem.ammotype==AmmoType){
            background.color=Color.green;

			scale.SendMessage("OnPermaHover",true);
        }
        else{
            background.color=Color.red;
			scale.DisableTween=true;
        }
    }
    //DEV. puukko end

	void Start(){
        if (EquipmentSlots==null)
            EquipmentSlots=new List<UIAmmoSlot>();
        EquipmentSlots.Add(this);
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