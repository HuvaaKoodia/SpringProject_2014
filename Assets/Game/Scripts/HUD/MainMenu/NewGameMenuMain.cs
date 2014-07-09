using UnityEngine;
using System.Collections;

public class NewGameMenuMain : MonoBehaviour {

	public UILabel Description;
	public GameObject Warning_overwrite;
	public GameObject InputBlocker;

	GameDB GDB;

	bool ironman_mode;

	void Start () {
		NormalModeSelected();

		GDB=SharedSystemsMain.I.GDB;
		Warning_overwrite.SetActive(GDB.HasSave); 
	}

	void PlayClick(){
		EscHudMain.I.FadeIn();
		InputBlocker.SetActive(true);

		StartCoroutine(PlayAfterFade());
	}

	IEnumerator PlayAfterFade()
	{
		while(EscHudMain.I.FadeInProgress) yield return null;

		GDB.CreateNewGame();
		GDB.GameData.IronManMode=ironman_mode;
		GDB.PlayGame();
	}

	public void NormalModeSelected(){
		ironman_mode=false;

		Description.text="Loading a saved game after death is permitted.";
	}
	
	public void IronmanModeSelected(){
		ironman_mode=true;

		Description.text="Death is permanent.\n\nThe save file will be removed after defeat.";
	}
}
