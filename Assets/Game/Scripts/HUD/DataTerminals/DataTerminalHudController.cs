using UnityEngine;
using System.Collections;

public class DataTerminalHudController : MonoBehaviour {

	public MenuHandler menu;
	public GameObject DataTerminal_button;
	public MenuTabController Tabs;

	public GameObject[] Terminals;

	private DataTerminalMain.Type CurrentType;

	public void Awake(){
		foreach(var t in Terminals){
			Tabs.TabMenus.Add(t);
		}

		CloseDataTerminal();
	}

	//menu activation

	public void OpenDataTerminal (DataTerminalMain.Type terminalType)
	{
		DataTerminal_button.SetActive(true);
		CurrentType=terminalType;

		Tabs.ActivateMenu(Terminals[(int)terminalType]);
	}

	public void CloseDataTerminal ()
	{
		DataTerminal_button.SetActive(false);
	}

	public void OpenCurrentTerminal(){
		Tabs.ActivateMenu(Terminals[(int)CurrentType]);
	}

	//game control functions

	void ToggleGeneratorStatus(){
		var power=menu.GC.GetFloor(0).PowerOn;
		menu.GC.SetFloorsPowerState(!power);
	}

	void ToggleLightStatus(){
		
	}

	void ToggleWeaponStatus(){
		
	}

	void ToggleElevatorStatus(){

	}
}
