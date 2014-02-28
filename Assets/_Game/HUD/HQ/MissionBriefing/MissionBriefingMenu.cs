using UnityEngine;
using System.Collections;

public class MissionBriefingMenu : MonoBehaviour {

    public UILabel BriefingLabel,ObjectivesLabel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {}

    public void SetMission(MissionObjData mission){
        BriefingLabel.text=mission.Briefing;
        ObjectivesLabel.text=mission.Objectives;
    }
}
