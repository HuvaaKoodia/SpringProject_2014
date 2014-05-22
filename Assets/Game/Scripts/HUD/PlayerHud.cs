﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHud : MonoBehaviour {
	
	PlayerMain player;
	public MasterHudMain MasterHud;

	public RadarMain radar;
	public HudMapMain map;

	public UISprite turnText;
	public GunDisplayMain gunInfoDisplay;

	public GameObject EndHud;
	public GameObject MovementHud;
	public GameObject TargetingHud;
	public GameObject InteractHud;
	public GameObject engageButton;
	public GameObject disperseHeatButton;
	public MechStatisticsMain MechStats;

	public UIPanel targetMarkPanel;

	public ComputerSystems.TimanttiPeli TP;

	public bool UpdateComputerSystems=true;

	public List<GameObject> ApBlips;

	// Use this for initialization
	void Start()
	{
#if !UNITY_EDITOR
		UpdateComputerSystems=true;
#endif
		MovementHud.SetActive(false);
		TargetingHud.SetActive(false);
	}

    public void Init(PlayerMain p, GameController gc){
        gunInfoDisplay.GC=gc;
		radar.GC=gc;
		map.GC = gc;
        
		player=p;
	
		MechStats.SetPlayer(player.ObjData);

		radar.Init();
		map.Init();

		player.ActivateEquippedItems();
		SetHudToPlayerStats();

		TP.GameData=gc.SS.GDB.GameData;
    }

	//hud buttons
	public void ToggleMovementHUD()
	{
		if (player.inputSub.NotUsable()) return;
			MasterHud.ToggleMovementHUD();
	}
	
	public void ToggleTargetingHUD()
	{
		if (player.inputSub.NotUsable()) return;
			MasterHud.ToggleTargetingHUD();
	}

    public void DeactivateTargetingHUD()
    {
        MasterHud.DeactivateTargetingHUD();
    }

	public void DeactivateInventoryHUD()
	{
		MasterHud.DeactivateInventoryHUD();
	}
	
	public void ActivateInventoryHUD()
	{
		if (player.inputSub.NotUsable()) return;
			MasterHud.ActivateInventoryHUD();
	}

	public void ToggleInventory()
	{
		if (player.inputSub.NotUsable()) return;
			MasterHud.ToggleInventory();
	}

	//input

	void MoveBackwardButtonPressed()
	{
		player.inputSub.MoveBackwardInput();
	}

	void MoveForwardButtonPressed()
	{
		player.inputSub.MoveForwardInput();
	}

	void MoveLeftButtonPressed()
	{
		player.inputSub.MoveLeftInput();
	}

	void MoveRightButtonPressed()
	{
		player.inputSub.MoveRightInput();
	}

	void TurnLeftButtonPressed()
	{
		player.inputSub.TurnLeftInput();
	}

	void TurnRightButtonPressed()
	{
		player.inputSub.TurnRightInput();
	}

	void EndTurnButtonPressed()
	{
		player.inputSub.EndTurnInput();
	}

	void InteractButtonPressed()
	{
		player.inputSub.InteractInput(false);
	}

	void EngageCombatPressed()
	{
		player.inputSub.EngageCombatInput();
	}

	void DisperseHeatPressed()
	{
		player.inputSub.DisperseHeatInput();
	}

	void LeftHandWeaponPressed()
	{
		WeaponPressed(WeaponID.LeftHand);
	}

	void LeftShoulderWeaponPressed()
	{
		WeaponPressed(WeaponID.LeftShoulder);
	}

	void RightHandWeaponPressed()
	{
		WeaponPressed(WeaponID.RightHand);
	}

	void RightShoulderWeaponPressed()
	{
		WeaponPressed(WeaponID.RightShoulder);
	}

    void TargetingModeButtonPressed()
    {
        player.inputSub.TargetingModeInput();
    }

	public void ShowApBlips(int count)
	{
		if (count > 2)
			count = 2;

		int i = 0;
		for (; i < count; i++)
		{
			ApBlips[i].SetActive(true);
		}

		for (; i < 2; i++)
		{
			ApBlips[i].SetActive(false);
		}
	}

	public void CheckTargetingModePanel()
	{
		engageButton.gameObject.SetActive(player.targetingSub.HasAnyTargets());
	}

	public void SetHudToPlayerStats(){
	
		if (UpdateComputerSystems){
			radar.SetDisabled(!player.HasRadar);
			map.SetDisabled(!player.HasMap);
		}
		else
		{
			player.ActivateHaxMapNRadar();
		}
	}

	public void ChangeFloor(int floorIndex)
	{
		map.ChangeFloor(floorIndex);
		radar.ChangeFloor(floorIndex);
	}

	public void ToggleNaviScreenRotateWithCenter()
	{
		if (MasterHud.CurrentMenuState == MenuState.NothingSelected)
			radar.ToggleRotateWithCenter();

		if (MasterHud.CurrentMenuState == MenuState.NothingSelected)
			map.ToggleRotateWithPlayer();
	}

	public void OpenMap(){
		MasterHud.OpenMapInfopanel();
	}

	public void OpenStatus(){
		UpdateHudPanels();
		MasterHud.OpenStatusInfopanel();
	}
	
	public void WeaponPressed(WeaponID id){
		
		if (MasterHud.CurrentMenuState == MenuState.InventoryHUD) return;
		
		if (MasterHud.CurrentMenuState == MenuState.TargetingHUD){
			if (player.currentWeaponID==id){
				player.targetingSub.UnsightWeapon(player.GetWeapon(id));
				return;
			}
			player.inputSub.ChangeWeaponInput(id);
		}
		else{
			player.inputSub.ChangeWeaponInput(id);
			player.inputSub.TargetingModeInput();
		}
	}

	public void UpdateHudPanels ()
	{
		gunInfoDisplay.UpdateAllDisplays();
		MechStats.UpdateStats();
	}
}