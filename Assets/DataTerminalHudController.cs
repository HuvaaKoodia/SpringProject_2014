using UnityEngine;
using System.Collections;

public class DataTerminalHudController : MonoBehaviour {

	public MenuHandler Menu;
	
	public GameObject DataTerminal_power;
	public GameObject DataTerminal_command;
	public GameObject DataTerminal_armory;
	
	public void OpenDataTerminal (DataTerminalMain.Type terminalType)
	{
		switch(terminalType){
		case DataTerminalMain.Type.Power:
			DataTerminal_power.SetActive(true);
			break;
		case DataTerminalMain.Type.Armory:
			DataTerminal_command.SetActive(true);
			break;
		case DataTerminalMain.Type.Command:
			DataTerminal_armory.SetActive(true);
			break;
		}
	}
}
