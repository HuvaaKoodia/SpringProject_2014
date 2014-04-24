using UnityEngine;
using System.Collections;

public class DataTerminalButton : MonoBehaviour {

	public DataTerminalHudController Hud;
	public DataTerminalHudController.StatType type;

	public bool DownloadButton=false;

	public string ButtonText;
	public string StatusTextTrue="On", StatusTextFalse="Off";
	UILabel label;
	UIButton button;
	
	void Start () {

		if (DownloadButton){
			StatusTextTrue="Downloaded";
			StatusTextFalse="";
		}

		button=transform.FindChild("Button").GetComponent<UIButton>();
		label=transform.FindChild("Label").GetComponent<UILabel>();

		button.transform.FindChild("Label").GetComponent<UILabel>().text=ButtonText;

		SetStatus(Hud.GetStatus(this));
	}

	public void SetStatus(bool on){
		label.text=on?StatusTextTrue:StatusTextFalse;
	}

	void OnButtonPress(){
		Hud.ToggleStatus(this);
	}
}
