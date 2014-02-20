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
        SellSlot.OnItemReplaceEvent=SellItem;
	}
	
	// Update is called once per frame
	void Update () {
        MoneyLabel.text="Money: "+_player.Money+" BC";
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

    public void SellAll(){
        for(int i=0;i<PlayerStorage.ItemStorage.maxItemCount;i++){
            var item=PlayerStorage.ItemStorage.GetItem(i);
            if (item==null) continue;
            SellItem(item);
            PlayerStorage.ItemStorage.Replace(i,null);
        }
    }

    void SellItem(InvGameItem item){
        var cost=item.Stats.Find(s=>s.type==InvStat.Type.Value);
        if (cost==null) return;
        _player.Money+=cost._amount;
    }
}
