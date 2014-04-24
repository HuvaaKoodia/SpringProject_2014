using UnityEngine;
using System.Collections;

public class DataTerminalHudController : MonoBehaviour {

	public enum StatType{Generator,Lights,Elevator,EngineData,NavigationData,CargoData,ArmoryData};

	public MasterHudMain menu;
	public GameObject DataTerminal_button;
	public MenuTabController Tabs;

	public GameObject[] Terminals;

	private TileObjData.Obj CurrentType;

	public void Awake(){
		foreach(var t in Terminals){
			Tabs.TabMenus.Add(t);
		}

		CloseDataTerminal();
	}

	//menu activation

	public void OpenDataTerminal (TileObjData.Obj terminalType)
	{
		DataTerminal_button.SetActive(true);
		CurrentType=terminalType;

		Tabs.ActivateMenu(Terminals[DataTerminalMain.TypeToIndex(terminalType)]);
	}

	public void CloseDataTerminal ()
	{
		DataTerminal_button.SetActive(false);
	}

	public void OpenCurrentTerminal(){
		Tabs.ActivateMenu(Terminals[(int)CurrentType]);
	}

	//game control functions

	public void ToggleStatus(DataTerminalButton button){
		switch(button.type){
			case StatType.Generator:
				ToggleGeneratorStatus(button);
			break;
		}

		button.SetStatus(GetStatus(button));
	}

	void ToggleGeneratorStatus(DataTerminalButton button){
		var power=menu.GC.GetFloor(0).PowerOn;
		menu.GC.SetFloorsPowerState(!power);
	}

	public bool GetStatus(DataTerminalButton button){
		switch(button.type){
		case StatType.Generator: return menu.GC.GetFloor(0).PowerOn;
		}
		return false;
	}

	void ToggleLightStatus(){
		
	}

	void ToggleWeaponStatus(){
		
	}
}
