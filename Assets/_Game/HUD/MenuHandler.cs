﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MenuState
{
	NothingSelected, MovementHUD, TargetingHUD, InventoryHUD
}

public class MenuHandler : MonoBehaviour {

	public GameController GC;
	public PlayerMain player;

	public Camera NGUICamera;

	public UISprite turnText;
	
	public UISprite engageButton;

	public UIPanel targetMarkPanel;

	public GunDisplayMain gunInfoDisplay;
	public RadarMain radar;
	public HudMapMain map;

	public MenuState currentMenuState;

	public GameObject EndHud;
	public GameObject MovementHud;
	public GameObject TargetingHud;
	public GameObject InteractHud;
	public InventoryMain InventoryHud;
    public MissionBriefMain MissionBriefing;
    public GameObject EndMissionPanel;

	// Use this for initialization
	void Start()
	{
		currentMenuState = MenuState.NothingSelected;
		MovementHud.SetActive(false);
		TargetingHud.SetActive(false);

        SetGC(GC);
        radar.Init();
		map.Init();
	}

    public void SetGC(GameController gc){
        GC=gc;
        gunInfoDisplay.GC=GC;
        radar.GC=GC;
        map.GC = GC;
    }
	
	// Update is called once per frame
	void Update () {
	}

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

    void WeaponPressed(WeaponID id){
        if (player.currentGunID==id){
            player.inputSub.TargetingModeInput();
        }
        else{
            player.inputSub.ChangeWeaponInput(id);
            
            if (currentMenuState != MenuState.InventoryHUD 
                && currentMenuState != MenuState.TargetingHUD)
                player.inputSub.TargetingModeInput();
        }
    }

    void TargetingModeButtonPressed()
    {
        player.inputSub.TargetingModeInput();
    }

	public void ToggleMovementHUD()
	{
		ChangeMenuState(MenuState.MovementHUD);
	}

	public void ToggleTargetingHUD()
	{
		ChangeMenuState(MenuState.TargetingHUD);
	}

	public void DeactivateInventoryHUD()
	{
		ChangeMenuState(MenuState.NothingSelected);
	}

	public void ActivateInventoryHUD()
	{
		ChangeMenuState(MenuState.InventoryHUD);
	}

    public void SetNothingSelected()
    {
        ChangeMenuState(MenuState.NothingSelected);
    }

	public void SetInteractVisibility(bool visible)
	{
		if (currentMenuState == MenuState.InventoryHUD ||
		    currentMenuState == MenuState.TargetingHUD)
			visible = false;

		InteractHud.SetActive(visible && player.interactSub.HasInteractable);
	}

	void ChangeMenuState(MenuState newState)
	{
		if (newState == currentMenuState)
			currentMenuState = MenuState.NothingSelected;
		else
			currentMenuState = newState;

		switch (currentMenuState)
		{
		case MenuState.NothingSelected:
			EndHud.SetActive(true);
			MovementHud.SetActive(false);
			TargetingHud.SetActive(false);

			if (InventoryHud.InventoryParent.activeSelf)
				InventoryHud.DeactivateInventory();

			GC.Player.EndTargetingMode();
			SetInteractVisibility(true);
			break;

		case MenuState.MovementHUD:
			EndHud.SetActive(false);
			MovementHud.SetActive(true);
			TargetingHud.SetActive(false);

			if (InventoryHud.InventoryParent.activeSelf)
				InventoryHud.DeactivateInventory();

			GC.Player.EndTargetingMode();
			break;

		case MenuState.TargetingHUD:
			EndHud.SetActive(false);
			MovementHud.SetActive(false);
			TargetingHud.SetActive(true);

			if (InventoryHud.InventoryParent.activeSelf)
				InventoryHud.DeactivateInventory();

			SetInteractVisibility(false);
			break;

		case MenuState.InventoryHUD:
			EndHud.SetActive(false);
			MovementHud.SetActive(false);
			TargetingHud.SetActive(false);
			GC.Player.EndTargetingMode();
			SetInteractVisibility(false);
			break;
		}
	}
	
	public void CheckTargetingModePanel()
	{
		engageButton.gameObject.SetActive(player.targetingSub.HasAnyTargets());
	}

    void MissionLogPressed(){
        MissionBriefing.gameObject.SetActive(!MissionBriefing.gameObject.activeSelf);
    }

    public void OpenEndMissionPanel(){
        EndMissionPanel.SetActive(true);
    }

    public void CloseEndMissionPanel(){
        EndMissionPanel.SetActive(false);
    }

    public void EndMission(){
        //DEV.TODO!
    }
}
