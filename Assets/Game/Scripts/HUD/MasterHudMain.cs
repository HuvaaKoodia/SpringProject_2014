using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MenuState
{
	NothingSelected, MovementHUD, TargetingHUD, InventoryHUD
}

public class MasterHudMain : MonoBehaviour {

	public GameController GC;
	public PlayerMain player;
	
	public MenuState currentMenuState;
	public MenuState CurrentMenuState{get{return currentMenuState;}}

	public InGameInfoPanelMain InfoHud;
    public MissionBriefingMenu MissionBriefing;
	public GameObject EndMissionPanel;
	public GameoverPanel _GameoverPanel;
	public UISprite FadePanel;

	public UILabel FPS;
	public bool ShowFPS=true;

	public DataTerminalHudController TerminalHud;

	// Use this for initialization
	void Start()
	{
		currentMenuState = MenuState.NothingSelected;

		if (!ShowFPS){
			FPS.gameObject.SetActive(false);
		}

		_GameoverPanel.gameObject.SetActive(false);
	}

    public void SetGC(GameController gc){
        GC=gc;
    }

	public  float updateInterval = 0.5F;
	private float accum   = 0; // FPS accumulated over the interval
	private int   frames  = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval

	// Update is called once per frame
	void Update () {

		if (ShowFPS) {
			DrawFPS();
		}
	}
	
    public void SetNothingSelected()
    {
        ChangeMenuState(MenuState.NothingSelected);
    }

	void DeactivateInventory ()
	{
		if (!InfoHud.InfoPanelOpen) return;
		
		InfoHud.DeactivateInventory();
	}

	public void ToggleMovementHUD()
	{
		ChangeMenuState(MenuState.MovementHUD);
	}
	
	public void ToggleTargetingHUD()
	{
		ChangeMenuState(MenuState.TargetingHUD);
	}

    public void DeactivateTargetingHUD()
    {
        ChangeMenuState(MenuState.NothingSelected);
    }

	public void ActivateInventoryHUD()
	{
		ChangeMenuState(MenuState.InventoryHUD);
	}
	
	public void DeactivateInventoryHUD()
	{
		ChangeMenuState(MenuState.NothingSelected);
	}

	public void ActivateDataTerminalHUD (TileObjData.Obj terminalType)
	{
		TerminalHud.OpenDataTerminal(terminalType);
		ChangeMenuState(MenuState.InventoryHUD);
	}
	
	public void ToggleInventory()
	{
		if (InfoHud.InfoPanelOpen)
		{
			InfoHud.DataTerminalPanel.CloseDataTerminal();
			DeactivateInventoryHUD();
		}
		else
		{
			InfoHud.OpenTab_Inventory();
			ActivateInventoryHUD();
		}
	}

	public void OpenMapInfopanel()
	{
		InfoHud.OpenTab_Map();
		ActivateInventoryHUD();
	}

	public void SetInteractVisibility(bool visible)
	{
		if (currentMenuState == MenuState.InventoryHUD ||
		    currentMenuState == MenuState.TargetingHUD)
			visible = false;

		player.HUD.InteractHud.SetActive(visible && player.interactSub.HasInteractable);
	}

	public void ChangeMenuState(MenuState newState)
	{
		if (newState == currentMenuState)
			currentMenuState = MenuState.NothingSelected;
		else
			currentMenuState = newState;

		switch (currentMenuState)
		{
		case MenuState.NothingSelected:
			player.HUD.EndHud.SetActive(true);
			player.HUD.MovementHud.SetActive(false);
			player.HUD.TargetingHud.SetActive(false);
			player.HUD.disperseHeatButton.SetActive(true);
			player.EndTargetingMode();

			//player.SetMouseLook(true);

			DeactivateInventory();

			SetInteractVisibility(true);
			break;

		case MenuState.MovementHUD:
			player.HUD.EndHud.SetActive(false);
			player.HUD.MovementHud.SetActive(true);
			player.HUD.TargetingHud.SetActive(false);
			player.HUD.disperseHeatButton.SetActive(false);
			player.EndTargetingMode();

			//player.SetMouseLook(true);

			DeactivateInventory();
			break;

		case MenuState.TargetingHUD:
			player.HUD.EndHud.SetActive(false);
			player.HUD.MovementHud.SetActive(false);
			player.HUD.TargetingHud.SetActive(true);
			player.HUD.disperseHeatButton.SetActive(false);
			//player.SetMouseLook(true);

			DeactivateInventory();

			SetInteractVisibility(false);
			break;

		case MenuState.InventoryHUD:
			player.HUD.EndHud.SetActive(false);
			player.HUD.MovementHud.SetActive(false);
			player.HUD.TargetingHud.SetActive(false);
			player.HUD.disperseHeatButton.SetActive(false);
			player.EndTargetingMode();

			player.SetMouseLook(false);

			InfoHud.ActivateInventory();

			SetInteractVisibility(false);
			break;
		}
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
        GC.EndMission();
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
		FadeIn(1);
	}
	
	public void FadeOut(){
		FadeOut(1);
	}

	public void FadeIn(float fade_speed){
		StartCoroutine(Fader(Time.deltaTime*fade_speed));
	}

	public void FadeOut(float fade_speed){
		StartCoroutine(Fader(-Time.deltaTime*fade_speed));
	}

	public void SetAlpha(float alpha){
		FadePanel.alpha=alpha;
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

	public void ShowGameoverPanel()
	{
		_GameoverPanel.Activate(!GC.SS.GDB.GameData.IronManMode);
	}
}