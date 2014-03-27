using UnityEngine;
using System.Collections;

public class MissionButtonMain : MonoBehaviour {

    public UISprite BG;
    public UILabel Header,Info;

    public MissionMenuHud Menu;

    public MissionObjData Mission{get;private set;}

    public void SetMission(MissionObjData mission){
        Mission=mission;

		Header.text=""+MissionGenerator.MissionName(mission);
        Info.text=MissionGenerator.MissionShortDescription(mission);
    }

    public int getWidth(){
        return BG.width;
    }

    public void OnClick(){
        Menu.SelectMission(Mission);
    }
}
