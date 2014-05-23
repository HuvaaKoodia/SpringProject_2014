using UnityEngine;
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

	public List<PlayerBloodArrowSub> bloodArrows;

	public Animation buttonAnimation;

	void Awake()
	{
		
		buttonAnimation.Play("Take 001");
		StartCoroutine(stopButtonAnim());
	}

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

		PlayButton3Anim();
		MasterHud.ToggleMovementHUD();
	}
	
	public void ToggleTargetingHUD()
	{
		if (player.inputSub.NotUsable()) return;

		PlayButton5Anim();
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

		PlayButton2Anim();
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
		if (player.inputSub.NotUsable()) return;
		PlayButton6Anim();
		player.inputSub.EndTurnInput();
	}

	void InteractButtonPressed()
	{
		if (player.inputSub.NotUsable()) return;
		PlayButton1Anim();
		player.inputSub.InteractInput(false);
	}

	void EngageCombatPressed()
	{
		if (player.inputSub.NotUsable()) return;
		PlayButton6Anim();
		player.inputSub.EngageCombatInput();
	}

	void DisperseHeatPressed()
	{
		if (player.inputSub.NotUsable()) return;
		PlayButton4Anim();
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
		PlayButton5Anim();
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

	public void ShowBloodArrow(int dir)
	{
		bloodArrows[dir].ShowArrow();
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

	
	IEnumerator stopButtonAnim()
	{
		yield return new WaitForEndOfFrame();
		buttonAnimation.Stop();
		yield return new WaitForEndOfFrame();
		buttonAnimation.Stop();
		yield return new WaitForEndOfFrame();
		buttonAnimation.Stop();
	}

	void PlayButton1Anim()
	{
		buttonAnimation.Play("Button1");
	}

	void PlayButton2Anim()
	{
		buttonAnimation.Play("Button2");
	}

	void PlayButton3Anim()
	{
		buttonAnimation.Play("Button3");
	}

	void PlayButton4Anim()
	{
		buttonAnimation.Play("Button4");
	}

	void PlayButton5Anim()
	{
		buttonAnimation.Play("Button5");
	}

	void PlayButton6Anim()
	{
		buttonAnimation.Play("Button6");
	}

}