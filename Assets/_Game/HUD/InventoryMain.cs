using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryMain : MonoBehaviour {

	public bool InventoryOpen{get{return InventoryParent.activeSelf;}}

	public MenuHandler HUD;
	public GameObject InventoryParent,LootParent;
	public UIItemStorage InventoryStorage,LootStorage;
	public List<UIEquipmentSlot> EquipmentSlots;
	public EquipRandomItem temp_random_item_button;

	PlayerMain Player;

	// Use this for initialization
	void Start () {
		InventoryParent.SetActive(false);

		temp_random_item_button.XDB=GameObject.FindGameObjectWithTag("SharedSystems").GetComponent<SharedSystemsMain>().XDB;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ToggleInventory()
	{
		if (InventoryParent.activeSelf)
		{
			DeactivateInventory();
		}
		else
		{
			ActivateInventory();
		}

		HUD.ToggleInventoryHUD();
	}

	public void ActivateInventory()
	{ 
		InventoryParent.SetActive(true);
	}

	public void DeactivateInventory()
	{ 
		InventoryParent.SetActive(false);

		if (!InventoryParent.activeSelf){
			//activate weapons
            Player.ActivateWeapon(WeaponID.LeftHand,UIEquipmentSlot.Slot.WeaponLeftHand);
            Player.ActivateWeapon(WeaponID.LeftShoulder,UIEquipmentSlot.Slot.WeaponLeftShoulder);
            Player.ActivateWeapon(WeaponID.RightHand,UIEquipmentSlot.Slot.WeaponRightHand);
            Player.ActivateWeapon(WeaponID.RightShoulder,UIEquipmentSlot.Slot.WeaponRightShoulder);
		

			//activate utilities
			//DEV.TODO

		if (LootParent.activeSelf) LootParent.SetActive(false);
	}
			if (LootParent.activeSelf) LootParent.SetActive(false);
		}
	}

	public void SetPlayer(PlayerMain player){
		Player=player;
        InventoryStorage.SetItemStorage(player.ObjData.Items);
		foreach(var s in EquipmentSlots){
            s.equipment=player.ObjData.Equipment;
            s.Inventory=this;
		}

		temp_random_item_button.equipment=player.ObjData.Equipment;
	}

	//DEV.TODO
	public void SetLoot(LootCrateMain loot){
		LootParent.SetActive(true);

		LootStorage.ChangeItemStorage(loot.Items);
	}
}
