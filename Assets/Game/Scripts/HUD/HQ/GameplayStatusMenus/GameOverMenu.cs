using UnityEngine;
using System.Collections;

public class GameOverMenu : MonoBehaviour {

	public GameObject Warning_finances;

	void Start(){
		var SS=SharedSystemsMain.I;

		Warning_finances.SetActive(!SS.GDB.GameData.UsedFinanceManager);
	}

	public void OnBackToMenuPressed()
	{
		SharedSystemsMain.I.GDB.EndGame();
	}
}
