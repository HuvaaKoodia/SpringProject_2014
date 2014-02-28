using UnityEngine;
using System.Collections;

public class MissionButtonMain : MonoBehaviour {

    public UISprite BG;
    public UILabel Header,Info;

    public MissionMenuHud Menu;

    public MissionObjData Mission{get;private set;}

    public void SetMission(MissionObjData mission){
        Mission=mission;

        Header.text=""+mission.MissionType;
        Info.text=mission.Briefing;
    }

    public int getWidth(){
        return BG.width;
    }

    public void OnClick(){
        Menu.SelectMission(Mission);
    }
}
