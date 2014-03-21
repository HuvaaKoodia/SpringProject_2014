using UnityEngine;
using System.Collections;

public class MissionDebriefingMenu : MonoBehaviour {

    public UILabel text;

    public void SetMission(GameDB GDB,int reward){
		text.text=MissionGenerator.MissionDebriefText(GDB.GameData.CurrentMission,GDB.GameData.PlayerData);

		text.text+="\n\nReward: "+reward+" "+XmlDatabase.MoneyUnit;
    }
}
