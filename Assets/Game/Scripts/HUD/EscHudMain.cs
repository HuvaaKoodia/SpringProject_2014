using UnityEngine;
using System.Collections;

public class EscHudMain : MonoBehaviour {

	public static EscHudMain I;

	public GameDB GDB;
	public UILabel GameSavedLabel;
	public GameObject SaveButton;

	public GameObject Anchor;

	public bool Active{get{return Anchor.activeSelf;}}
	[SerializeField] UISprite FadePanel;
	[SerializeField] GameoverPanel _GameoverPanel;
		
	void Awake()
	{
		I=this;

		_GameoverPanel.gameObject.SetActive(false);
	}

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
		Anchor.SetActive(on);
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

	//fader

	
	public void FadeIn(){
		FadeIn(1);
	}
	
	public void FadeOut(){
		FadeOut(1);
	}
	
	public void FadeIn(float fade_speed){
		StopCoroutine("Fader");
		StartCoroutine(Fader(Time.fixedDeltaTime*fade_speed));
	}
	
	public void FadeOut(float fade_speed){
		StopCoroutine("Fader");
		StartCoroutine(Fader(-Time.fixedDeltaTime*fade_speed));
	}
	
	public void SetAlpha(float alpha){
		FadePanel.alpha=alpha;
	}
	
	public bool FadeInProgress{get;private set;}
	
	IEnumerator Fader(float amount){

		if (!FadeInProgress)
		{
			if (amount<0) FadePanel.alpha=1;
			if (amount>0) FadePanel.alpha=0;
		}

		FadeInProgress=true;
		while (true){
			FadePanel.alpha+=amount;
			if (FadePanel.alpha<=0){
				FadePanel.alpha=0;
				break;
			}
			else if (FadePanel.alpha>=1){
				FadePanel.alpha=1;
				break;
			}
			else yield return null;
		}
		FadeInProgress=false;
	}

	
	
	public void ShowGameoverPanel()
	{
		_GameoverPanel.Activate(!SharedSystemsMain.I.GDB.GameData.IronManMode);
	}
}
