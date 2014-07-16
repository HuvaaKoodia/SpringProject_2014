using UnityEngine;
using System.Collections;

public class GameoverPanel : MonoBehaviour {

	public GameObject LoadGameButton;

	public void Activate(bool allow_load){
		gameObject.SetActive(true);
		if (!allow_load){
			LoadGameButton.SetActive(false);
		}
	}

	public void MainMenuPressed(){
		SharedSystemsMain.I.GDB.LoadMainMenu();
		gameObject.SetActive(false);
	}

	public void LoadGamePressed(){
		SharedSystemsMain.I.GDB.LoadGame();
		gameObject.SetActive(false);
	}
}
