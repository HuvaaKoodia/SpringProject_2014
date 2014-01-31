using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryMain : MonoBehaviour {

	public bool InventoryOpen{get{return InventoryParent.activeSelf;}}

	public MenuHandler HUD;
	public GameObject InventoryParent,LootParent;
	public UIItemStorage InventoryStorage,LootStorage;
	public List<UIEquipmentSlot> EqupmentSlots;
	public EquipRandomItem temp_random_item_button;

	PlayerMain Player;

	// Use this for initialization
	void Start () {
		InventoryParent.SetActive(false);

		temp_random_item_button.DB=GameObject.FindGameObjectWithTag("SharedSystems").GetComponent<SharedSystemsMain>().GDB;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ToggleInventory()
	{


		InventoryParent.SetActive(!InventoryParent.activeSelf);
		HUD.gameObject.SetActive(!InventoryParent.activeSelf);


		if (!InventoryParent.activeSelf){
			//activate weapons
			ActivateWeapon(WeaponID.LeftHand,UIEquipmentSlot.Slot.WeaponLeftHand);
			ActivateWeapon(WeaponID.LeftShoulder,UIEquipmentSlot.Slot.WeaponLeftShoulder);
			ActivateWeapon(WeaponID.RightHand,UIEquipmentSlot.Slot.WeaponRightHand);
			ActivateWeapon(WeaponID.RightShoulder,UIEquipmentSlot.Slot.WeaponRightShoulder);

			//activate utilities
			//DEV.TODO


			if (LootParent.activeSelf) LootParent.SetActive(false);
		}
	}

	void ActivateWeapon(WeaponID id,UIEquipmentSlot.Slot slot){
		var weapon=Player.GetWeapon(id);
		var item=Player.equipment.GetSlot(slot).Item;
		if (weapon.Weapon!=item){
			weapon.SetWeapon(item);
		}
	}

	public void SetPlayer(PlayerMain player){
		Player=player;
		InventoryStorage.ItemStorage=player.items;
		foreach(var s in EqupmentSlots){
			s.equipment=player.equipment;
		}

		temp_random_item_button.equipment=player.equipment;
	}

	//DEV.TODO
	public void SetLoot(LootCrateMain loot){
		LootParent.SetActive(true);

		LootStorage.ChangeItemStorage(loot.Items);
	}
}
