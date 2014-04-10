using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHud : MonoBehaviour {
	
	PlayerMain player;
	public MasterHudMain menu;

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

	// Use this for initialization
	void Start()
	{
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

		SetHudToPlayerStats();
    }

	//hud buttons

	public void ToggleMovementHUD()
	{
		menu.ToggleMovementHUD();
	}
	
	public void ToggleTargetingHUD()
	{
		menu.ToggleTargetingHUD();
	}
	
	public void DeactivateInventoryHUD()
	{
		menu.DeactivateInventoryHUD();
	}
	
	public void ActivateInventoryHUD()
	{
		menu.ActivateInventoryHUD();
	}

	public void ToggleInventory()
	{
		menu.ToggleInventory();
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
		menu.WeaponPressed(WeaponID.LeftHand);
	}

	void LeftShoulderWeaponPressed()
	{
		menu.WeaponPressed(WeaponID.LeftShoulder);
	}

	void RightHandWeaponPressed()
	{
		menu.WeaponPressed(WeaponID.RightHand);
	}

	void RightShoulderWeaponPressed()
	{
		menu.WeaponPressed(WeaponID.RightShoulder);
	}

    void TargetingModeButtonPressed()
    {
        player.inputSub.TargetingModeInput();
    }
	
	public void CheckTargetingModePanel()
	{
		engageButton.gameObject.SetActive(player.targetingSub.HasAnyTargets());
	}

	public void SetHudToPlayerStats(){
		//radar.SetDisabled(!player.HasRadar);
		//map.SetDisabled(!player.HasMap);
		//radar.radarZoom=2f+1f*(1f-(player.RadarRange/player.RadarRangeMax));
	}

	public void ChangeFloor(int floorIndex)
	{
		map.ChangeFloor(floorIndex);
		radar.ChangeFloor(floorIndex);
	}
}