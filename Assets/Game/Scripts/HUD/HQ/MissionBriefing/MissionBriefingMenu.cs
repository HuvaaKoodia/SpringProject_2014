using UnityEngine;
using System.Collections;

public class MissionBriefingMenu : MonoBehaviour {

    public UILabel BriefingLabel,ObjectivesLabel;

	public GameObject LoadingLabel;

	void Awake(){
		LoadingLabel.SetActive(false);
	}

	public void SetMission(MissionObjData mission){
		BriefingLabel.text=mission.Briefing;
        ObjectivesLabel.text=mission.Objectives;
    }

	public void ShowLoadingLabel(){
		LoadingLabel.SetActive(true);
	}
}
