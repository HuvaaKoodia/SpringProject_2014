using UnityEngine;
using System.Collections;

public class MissionDebriefingMenu : MonoBehaviour {

    public UILabel text;

    public void SetMission(GameDB GDB,XmlDatabase XDB,int reward){
		text.text=MissionGenerator.MissionDebriefText(GDB.GameData.CurrentMission,GDB.GameData.PlayerData,XDB);

        text.text+="\n\nReward: "+reward+" BC";
    }
}
