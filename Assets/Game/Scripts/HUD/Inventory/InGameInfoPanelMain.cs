using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InGameInfoPanelMain : MonoBehaviour {

	public bool InfoPanelOpen{get{return InfoPanel.activeSelf;}}

	public DataTerminalHudController DataTerminalPanel;
	public MasterHudMain HUD;
	public GameObject InfoPanel,LootParent;
	public UIItemStorage InventoryStorage,LootStorage;
	public List<UIEquipmentSlot> EquipmentSlots;
	public List<UIAmmoSlot> AmmoSlots;
	public EquipRandomItem temp_random_item_button;

    public MenuTabController Tabs;
    public MechanicalMenu Mechanic;

	public InfoMenuMap MenuMap;
	PlayerMain Player;

	// Use this for initialization
	void Start (){
		InfoPanel.SetActive(false);

		MenuMap.Init(HUD.GC);
		MenuMap.gameObject.SetActive(false);

	#if !UNITY_EDITOR
		temp_random_item_button.gameObject.SetActive(false);	
	#endif
	}
    
    public void OpenTab_Inventory(){
        Tabs.OpenTab1();
    }

	public void OpenTab_Map(){
		StartCoroutine(CallBecomeVisibleAfterOneUpdateStepBecauseUnitySetActiveShenanigans());
		Tabs.OpenTab4();
	}

	IEnumerator CallBecomeVisibleAfterOneUpdateStepBecauseUnitySetActiveShenanigans(){
		yield return null;
		MenuMap.BecomeVisible();
	}

	public void OpenTab_Status(){
		Mechanic.OpenPanel();
		Tabs.OpenTab2();
	}

	public void ActivateInventory()
	{ 
		InfoPanel.SetActive(true);
    }

	public void DeactivateInventory()
	{
		InfoPanel.SetActive(false);
		MenuMap.gameObject.SetActive(false);

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

		int i=0;
		foreach (var a in player.ObjData.Ammo.Keys){
			if(i>AmmoSlots.Count-1) break;
			var s = AmmoSlots[i];
			s.Player=player.ObjData;
			s.SetAmmoType(a);
			++i;
		}

		temp_random_item_button.equipment=player.ObjData.Equipment;
        Mechanic.SetPlayer(player.ObjData);
	}

	public void SetLoot(LootCrateMain loot){
		LootParent.SetActive(true);
		LootStorage.ChangeItemStorage(loot.Items);

		OpenTab_Inventory();
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
