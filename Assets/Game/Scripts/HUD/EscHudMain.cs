using UnityEngine;
using System.Collections;

public class EscHudMain : MonoBehaviour {

	public GameDB GDB;

	public bool Active{get{return gameObject.activeSelf;}}

	public void MainMenuPressed(){
		Activate(false);
		GDB.LoadMainMenu();
	}

	public void Toggle ()
	{
		Activate(!Active);
	}

	public void Activate(bool on){
		gameObject.SetActive(on);
	}

	//button functions

	public void Close(){
		Activate(false);
	}
}
