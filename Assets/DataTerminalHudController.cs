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
}
