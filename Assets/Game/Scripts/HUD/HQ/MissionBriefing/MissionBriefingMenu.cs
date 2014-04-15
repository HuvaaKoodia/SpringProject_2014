using UnityEngine;
using System.Collections;

public class MissionBriefingMenu : MonoBehaviour {

    public UILabel BriefingLabel,ObjectivesLabel;

    public void SetMission(MissionObjData mission){
        BriefingLabel.text=mission.Briefing;
        ObjectivesLabel.text=mission.Objectives;
    }
}
