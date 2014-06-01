using UnityEngine;
using System.Collections;

public class EscHudMain : MonoBehaviour {

	public GameDB GDB;
	public UILabel GameSavedLabel;
	public GameObject SaveButton;

	public bool Active{get{return gameObject.activeSelf;}}
		
	public void ShowGameSavedLabel(){
		var savetype=GDB.GameData.IronManMode?"Ironman":"Normal";
		ShowLabel("GAME SAVED - "+savetype);
	}

	public void ShowGameLoadedLabel(){
		var savetype=GDB.GameData.IronManMode?"Ironman":"Normal";
		ShowLabel("GAME LOADED - "+savetype);
	}

	public void ShowLabel(string text){
		GameSavedLabel.text=text;
		GameSavedLabel.gameObject.SetActive(true);
		Invoke("HideGameSavedLabel",3f);
	}
	
	public void HideGameSavedLabel(){
		GameSavedLabel.gameObject.SetActive(false);
	}

	public void ActivateSaveButton (bool show)
	{
		SaveButton.SetActive(show);
	}

	public void Toggle ()
	{
		Activate(!Active);
	}

	public void Activate(bool on){
		gameObject.SetActive(on);

		Time.timeScale=on?0:1f;
	}

	//button functions

	public void Close(){
		Activate(false);
	}
	
	public void MainMenuPressed(){
		Activate(false);
		GDB.LoadMainMenu();
	}
	
	public void SaveGamePressed(){
		GDB.SaveGame();
		ShowGameSavedLabel();
	}
}
