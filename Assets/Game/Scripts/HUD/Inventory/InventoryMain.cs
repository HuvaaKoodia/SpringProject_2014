using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryMain : MonoBehaviour {

	public bool InventoryOpen{get{return InventoryPanel.activeSelf;}}

	public MenuHandler HUD;
	public GameObject InventoryPanel,LootParent;
	public UIItemStorage InventoryStorage,LootStorage;
	public List<UIEquipmentSlot> EquipmentSlots;
	public EquipRandomItem temp_random_item_button;

    public MenuTabController Tabs;
    public MechanicalMenu Mechanic;

	PlayerMain Player;

	// Use this for initialization
	void Start (){
		InventoryPanel.SetActive(false);
	#if !UNITY_EDITOR
		temp_random_item_button.gameObject.SetActive(false);	
	#endif
	}
    
    public void SetTabToInventory(){
        Tabs.OpenTab1();
    }

	public void ActivateInventory()
	{ 
        InventoryPanel.SetActive(true);
    }

	public void DeactivateInventory()
	{ 
        InventoryPanel.SetActive(false);
        Player.ActivateEquippedItems();

		Player.HUD.SetHudToPlayerStats();

		if (LootParent.activeSelf) LootParent.SetActive(false);
	}

	public void SetPlayer(PlayerMain player){
		Player=player;
        InventoryStorage.SetItemStorage(player.ObjData.Items);
		foreach(var s in EquipmentSlots){
            s.equipment=player.ObjData.Equipment;
		}

		temp_random_item_button.equipment=player.ObjData.Equipment;
        Mechanic.SetPlayer(player.ObjData);
	}

	public void SetLoot(LootCrateMain loot){
		LootParent.SetActive(true);
		LootStorage.ChangeItemStorage(loot.Items);

		SetTabToInventory();
		HUD.ActivateInventoryHUD();
	}

    public void LootAll(){
        for(int i=0;i<LootStorage.ItemStorage.maxItemCount;i++){
            var item=LootStorage.ItemStorage.GetItem(i);
            if (item==null) continue;
            if (Player.ObjData.Items.Add(item)){
                LootStorage.ItemStorage.Replace(i,null);
            }
            else{
                break;//player inventory full
            }
        }
    }
}
