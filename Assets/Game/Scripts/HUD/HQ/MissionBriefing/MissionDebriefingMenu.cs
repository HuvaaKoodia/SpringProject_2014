using UnityEngine;
using System.Collections;

public class MissionDebriefingMenu : MonoBehaviour {

    public UILabel text;

    public void SetMission(GameDB GDB){
		text.text=MissionGenerator.MissionDebriefText(GDB.GameData.CurrentMission,GDB.GameData.PlayerData);

		int reward=GDB.CalculateQuestReward();
		text.text+="\n\nReward: "+reward+" "+XmlDatabase.MoneyUnit;
    }
}
