using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VendorMenu : MonoBehaviour {

    public UIItemStorage PlayerStorage,VendorStorage;
    public UIEquipmentSlot SellSlot;
    public UILabel MoneyLabel;

    public List<UIEquipmentSlot> EquipmentSlots;

    PlayerObjData _player;

	// Use this for initialization
	void Start(){

	}
	
	// Update is called once per frame
	void Update () {
		MoneyLabel.text="Money: "+_player.Money+" "+XmlDatabase.MoneyUnit;
	}

	public void SetPlayer(PlayerObjData player){
        _player=player;
        PlayerStorage.SetItemStorage(player.Items);
		foreach(var s in EquipmentSlots){
            s.equipment=player.Equipment;
		}
	}

	public void SetVendor(InvItemStorage store){
        VendorStorage.ChangeItemStorage(store);
	}
    /// <summary>
    /// Call after SetPlayer and SetVendor
    /// </summary>
    public void Init(){
        SellSlot.OnItemReplaceEvent=SellItem;

        //slots
        foreach(var s in VendorStorage.Slots){
            //s.OnItemReplaceEvent=SellItem;
            s.VendorSlot=true;
        }

        foreach(var s in EquipmentSlots){
            s.OnItemReplaceEvent=BuyItem;
            s.VendorSlot=false;
        }

        foreach(var s in PlayerStorage.Slots){
            s.OnItemReplaceEvent=BuyItem;
            s.VendorSlot=false;
        }

        //items
        
        foreach(var i in VendorStorage.ItemStorage.items){
            if (i!=null) i.VendorItem=true;
        }

        foreach(var s in EquipmentSlots){
            if (s.Item!=null) s.Item.VendorItem=false;
        }

        foreach(var i in PlayerStorage.ItemStorage.items){
            if (i!=null) i.VendorItem=false;
        }
    }

    public void SellAll(){
        for(int i=0;i<PlayerStorage.ItemStorage.maxItemCount;i++){
            var item=PlayerStorage.ItemStorage.GetItem(i);
            if (item==null) continue;
            SellItem(item);
            PlayerStorage.ItemStorage.Replace(i,null);
        }
    }

    bool SellItem(InvGameItem item){
        if (item==null) return false;
        if (item.VendorItem) return true;
        var cost=item.Stats.Find(s=>s.type==InvStat.Type.Value);
        if (cost==null) return true;
        _player.Money+=cost._amount;
        item.VendorItem=true;
        return false;
    }

    bool BuyItem(InvGameItem item){
        if (item==null) return false;
        if (!item.VendorItem) return false;
        var cost=item.Stats.Find(s=>s.type==InvStat.Type.Value);
        if (cost==null||_player.Money<cost._amount) return true;
        _player.Money-=cost._amount;
        item.VendorItem=false;
        return false;
    }

}
