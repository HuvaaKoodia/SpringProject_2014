using UnityEngine;
using System.Collections;

public class VictoryMenu : MonoBehaviour {
	
	MissionMenuHud _menu;

	void Awake () 
	{
		CloseMenu();
	}

	public void OpenMenu(MissionMenuHud menu)
	{
		_menu=menu;
		gameObject.SetActive(true);
	}

	void CloseMenu()
	{
		gameObject.SetActive(false);
	}

	public void ContinueGamePressed(){
		_menu.OpenFinance();
		CloseMenu();
	}

	public void EndGamePressed(){
		SharedSystemsMain.I.GDB.EndGame();
	}
}
