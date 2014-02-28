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
    public MechanicalMenu Mechaninc;

	PlayerMain Player;

	// Use this for initialization
	void Start (){
        InventoryPanel.SetActive(false);
		temp_random_item_button.XDB=GameObject.FindGameObjectWithTag("SharedSystems").GetComponent<SharedSystemsMain>().XDB;


	}
    
    public void OpenInventory(){
        //ActivateInventory();
        Tabs.OpenTab1();
    }

    public void OpenStatus(){
        //ActivateInventory();
        Tabs.OpenTab2();
    }

    public void OpenLog(){
        //ActivateInventory();
        Tabs.OpenTab3();
    }

    public void ToggleInventory()
    {
        if (InventoryOpen)
        {
            DeactivateInventory();
            HUD.DeactivateInventoryHUD();
        }
        else
        {
            ActivateInventory();
            HUD.ActivateInventoryHUD();
        }
    }

	public void ActivateInventory()
	{ 
        InventoryPanel.SetActive(true);
        OpenInventory();
    }

	public void DeactivateInventory()
	{ 
        InventoryPanel.SetActive(false);
        Player.ActivateEquippedItems();

		if (LootParent.activeSelf) LootParent.SetActive(false);
	}

	public void SetPlayer(PlayerMain player){
		Player=player;
        InventoryStorage.SetItemStorage(player.ObjData.Items);
		foreach(var s in EquipmentSlots){
            s.equipment=player.ObjData.Equipment;
		}

		temp_random_item_button.equipment=player.ObjData.Equipment;

        Mechaninc.SetPlayer(player.ObjData);
	}

	public void SetLoot(LootCrateMain loot){
		LootParent.SetActive(true);
		LootStorage.ChangeItemStorage(loot.Items);

		ActivateInventory();
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
