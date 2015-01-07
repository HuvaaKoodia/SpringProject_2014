using UnityEngine;
using System.Collections;

public class OutOfMoneyMenu : MonoBehaviour 
{
	public GameObject AOKPanel,WarningPanel;
	MissionMenuHud _menu;

	void Awake () 
	{
		CloseMenu();
	}

	public void OpenMenu(MissionMenuHud menu)
	{
		_menu=menu;
		gameObject.SetActive(true);

		var GDB=SharedSystemsMain.I.GDB;

		if (GDB.GameData.PlayerData.Money>=0){
			//change to aok mode
			AOKPanel.SetActive(true);
			WarningPanel.SetActive(false);
		}
		else{
			//business as usual.
			AOKPanel.SetActive(false);
			WarningPanel.SetActive(true);
		}
	}

	void CloseMenu()
	{
		gameObject.SetActive(false);
	}

	public void ContinueGamePressed(){
		_menu.CheckMoneyAmount();
		_menu.OpenMissionSelect();
		CloseMenu();
	}
	
	public void CloseWarningPressed(){
		_menu.OpenVendor();
		CloseMenu();
	}

	public void EndGamePressed()
	{
		_menu.OpenGameoverMenu();
	}
}
