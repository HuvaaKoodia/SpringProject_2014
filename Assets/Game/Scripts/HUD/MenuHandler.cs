using UnityEngine;
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
    public MissionBriefingMenu MissionBriefing;
    public GameObject EndMissionPanel;
    public MechStatisticsMain MechStats;
	public UISprite FadePanel;

	public UILabel FPS;
	public bool ShowFPSonAndroid=true;

	// Use this for initialization
	void Start()
	{
		currentMenuState = MenuState.NothingSelected;
		MovementHud.SetActive(false);
		TargetingHud.SetActive(false);

        //SetGC(GC);
		MechStats.SetPlayer(player.ObjData);
		
		radar.Init();
		map.Init();
		
		SetHudToPlayerStats();
	}

    public void SetGC(GameController gc){
        GC=gc;
        gunInfoDisplay.GC=GC;
        radar.GC=GC;
        map.GC = GC;
        
    }

	public  float updateInterval = 0.5F;
	private float accum   = 0; // FPS accumulated over the interval
	private int   frames  = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval

	// Update is called once per frame
	void Update () {
		#if UNITY_ANDROID
		if (ShowFPSonAndroid) {
			DrawFPS();
		}
	#endif
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

        if (currentMenuState == MenuState.InventoryHUD) return;

        if (currentMenuState == MenuState.TargetingHUD){
            if (player.currentWeaponID==id){
                player.targetingSub.UnsightWeapon(player.GetWeapon(id));
                return;
            }
            player.inputSub.ChangeWeaponInput(id);
        }
        else{
            player.inputSub.TargetingModeInput();
            player.inputSub.ChangeWeaponInput(id);
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

	void DeactivateInventory ()
	{
		if (InventoryHud.InventoryOpen) return;
		
		InventoryHud.DeactivateInventory();
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

			DeactivateInventory();

			GC.Player.EndTargetingMode();
			SetInteractVisibility(true);
			break;

		case MenuState.MovementHUD:
			EndHud.SetActive(false);
			MovementHud.SetActive(true);
			TargetingHud.SetActive(false);

			DeactivateInventory();

			GC.Player.EndTargetingMode();
			break;

		case MenuState.TargetingHUD:
			EndHud.SetActive(false);
			MovementHud.SetActive(false);
			TargetingHud.SetActive(true);

			DeactivateInventory();

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

	public void SetHudToPlayerStats(){
		radar.SetDisabled(!player.HasRadar);
		map.SetDisabled(!player.HasMap);
		radar.radarZoom=2f+1f*(1f-(player.RadarRange/player.RadarRangeMax));
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
        GC.SS.GDB.EndMission();
    }

	void DrawFPS(){
		timeleft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		++frames;
		
		// Interval ended - update GUI text and start new interval
		if( timeleft <= 0.0 )
		{
			// display two fractional digits (f2 format)
			float fps = accum/frames;
			string format = System.String.Format("{0:F2} FPS",fps);
			
			var color="[008009]";//green
			if(fps < 30)
				color="[e5ff00]";//yellow
			else if(fps < 10)
				color="[ff0000]";//red
			//	DebugConsole.Log(format,level);
			
			FPS.text = color+format;
			
			timeleft = updateInterval;
			accum = 0.0F;
			frames = 0;
		}
	}

	public void FadeIn(){
		StartCoroutine(Fader(Time.deltaTime));
	}

	public void FadeOut(){
		StartCoroutine(Fader(-Time.deltaTime));
	}

	public bool FadeInProgress{get;private set;}

	IEnumerator Fader(float amount){
		FadeInProgress=true;
		while (true){
			FadePanel.alpha+=amount;
			if (FadePanel.alpha<=0){
				FadePanel.alpha=0;
				break;
			}
			else if (FadePanel.alpha>=1){
				FadePanel.alpha=1;
				break;
			}
			else yield return null;
		}
		FadeInProgress=false;
	}

}