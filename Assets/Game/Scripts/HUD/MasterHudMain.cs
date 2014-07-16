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

	[SerializeField] GameObject ManualMenuGO;
	[SerializeField] GameObject InfoMenuGO;
	
	public UILabel FPS;
	public bool ShowFPS=true;

	public DataTerminalHudController TerminalHud;

	// Use this for initialization
	void Start()
	{
		currentMenuState = MenuState.NothingSelected;

		ShowFPS=SharedSystemsMain.I.GOps.Data.ShowFPS;

		if (!ShowFPS){
			FPS.gameObject.SetActive(false);
		}
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
		InfoHud.DataTerminalPanel.CloseDataTerminal();
		ChangeMenuState(MenuState.NothingSelected);
	}

	public void ActivateDataTerminalHUD (TileObjData.Obj terminalType)
	{
		TerminalHud.OpenDataTerminal(terminalType);
		ChangeMenuState(MenuState.InventoryHUD);
	}

	private bool DeactivateInventoryHUDIfOpen(){
		if (InfoHud.InfoPanelOpen)
		{
			DeactivateInventoryHUD();
			return true;
		}
		return false;
	}
	
	public void ToggleInventory()
	{
		if (DeactivateInventoryHUDIfOpen()) return;
		InfoHud.OpenTab_Inventory();
		ActivateInventoryHUD();
	}

	public void ToggleStatus()
	{
		if (DeactivateInventoryHUDIfOpen()) return;
		OpenStatusInfopanel();
	}

	public void ToggleMap()
	{
		if (DeactivateInventoryHUDIfOpen()) return;
		OpenMapInfopanel();
	}

	public void ToggleLogs()
	{
		if (DeactivateInventoryHUDIfOpen()) return;
		OpenLogsInfoPanel();
	}

	public void OpenMapInfopanel()
	{
		InfoHud.OpenTab_Map();
		ActivateInventoryHUD();
	}

	public void OpenStatusInfopanel()
	{
		InfoHud.OpenTab_Status();
		ActivateInventoryHUD();
	}

	public void OpenLogsInfoPanel()
	{
		InfoHud.OpenTab_Logs();
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
			player.HUD.ShowTargetingHud(false);
			player.HUD.disperseHeatButton.SetActive(true);
			player.EndTargetingMode();

			//player.SetMouseLook(true);

			DeactivateInventory();

			SetInteractVisibility(true);
			break;

		case MenuState.MovementHUD:
			player.HUD.EndHud.SetActive(false);
			player.HUD.MovementHud.SetActive(true);
			player.HUD.ShowTargetingHud(false);
			player.HUD.disperseHeatButton.SetActive(false);
			player.EndTargetingMode();

			//player.SetMouseLook(true);

			DeactivateInventory();
			break;

		case MenuState.TargetingHUD:
			player.HUD.EndHud.SetActive(false);
			player.HUD.MovementHud.SetActive(false);
			player.HUD.ShowTargetingHud(true);
			player.HUD.disperseHeatButton.SetActive(false);
			//player.SetMouseLook(true);

			DeactivateInventory();

			SetInteractVisibility(false);
			break;

		case MenuState.InventoryHUD:
			player.HUD.EndHud.SetActive(false);
			player.HUD.MovementHud.SetActive(false);
			player.HUD.ShowTargetingHud(false);
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
		Time.timeScale=0;
		GC.Player.inputSub.DISABLE_INPUT=true;
    }

    public void CloseEndMissionPanel(){
        EndMissionPanel.SetActive(false);
		Time.timeScale=1;
		GC.Player.inputSub.DISABLE_INPUT=false;
    }

	public void ToggleManual()
	{
		bool manualOn=ManualMenuGO.activeSelf;

		ManualMenuGO.SetActive(!manualOn);
		InfoMenuGO.SetActive(manualOn);

		if (manualOn)
		{

		}
		else
		{

		}
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
			string format = System.String.Format("{0:F2}",fps);
			
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

}