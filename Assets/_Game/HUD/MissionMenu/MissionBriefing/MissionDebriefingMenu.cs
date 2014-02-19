using UnityEngine;
using System.Collections;

public class MissionDebriefingMenu : MonoBehaviour {

    public UILabel text;

    public void SetMission(GameDB GDB,XmlDatabase XDB){
        text.text=MissionGenerator.MissionDebriefText(GDB.CurrentMission,GDB.PlayerData,XDB);
    }
}
