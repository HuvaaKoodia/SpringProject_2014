﻿using UnityEngine;
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
		
		OpenCurrentTerminal();
	}
	
	public void CloseDataTerminal ()
	{
		DataTerminal_button.SetActive(false);
	}
	
	public void OpenCurrentTerminal(){
		Tabs.ActivateMenu(Terminals[DataTerminalMain.TypeToIndex(CurrentType)]);
	}
	
	//game control functions
	public void ToggleStatus(DataTerminalButton button){
		switch(button.type){
		case StatType.Generator:
			ToggleGeneratorStatus(button);
			break;
		case StatType.EngineData:
		case StatType.CargoData:
		case StatType.ArmoryData:
		case StatType.NavigationData:
			SetDownloadStatus(button);
		break;
		}
		
		button.SetStatus(GetStatus(button));
	}

	void SetDownloadStatus(DataTerminalButton button){
		var data=""+button.type;
		if (!menu.GC.Player.ObjData.HasDownloadData(data)){
			menu.GC.Player.ObjData.AddDownloadData(data);
		}
	}

	void ToggleGeneratorStatus(DataTerminalButton button){
		var power=menu.GC.GetFloor(0).PowerOn;
		menu.GC.SetFloorsPowerState(!power);
	}
	
	public bool GetStatus(DataTerminalButton button){
		switch(button.type){
			case StatType.Generator: return menu.GC.GetFloor(0).PowerOn;
		case StatType.EngineData:
		case StatType.CargoData:
		case StatType.ArmoryData:
		case StatType.NavigationData:
			var data=""+button.type;
			return menu.GC.Player.ObjData.HasDownloadData(data);
		}
		return false;
	}
	
	void ToggleLightStatus(){
		
	}
	
	void ToggleWeaponStatus(){
		
	}
}
