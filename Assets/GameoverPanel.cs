using UnityEngine;
using System.Collections;

public class GameoverPanel : MonoBehaviour {

	public MasterHudMain HUD;
	public GameObject LoadGameButton;

	public void Activate(bool allow_load){
		gameObject.SetActive(true);
		if (!allow_load){
			LoadGameButton.SetActive(false);
		}
	}

	public void MainMenuPressed(){
		SharedSystemsMain.I.GDB.LoadMainMenu();
	}

	public void LoadGamePressed(){
		SharedSystemsMain.I.GDB.LoadGame();
	}
}
