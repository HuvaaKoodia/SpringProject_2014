using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameDB : MonoBehaviour {

    public SharedSystemsMain SS;

    public PlayerObjData PlayerData{get;private set;}
    public List<MissionObjData> AvailableMissions;

    public MissionObjData CurrentMission{get;set;}

    public bool GOTO_DEBRIEF{get;set;}
    public bool GameStarted{get;set;}

    public void Awake(){
        GOTO_DEBRIEF=false;
        GameStarted=false;
    }

    public void StartNewGame(){
        GameStarted=true;

        AvailableMissions=new List<MissionObjData>();
        PlayerData=new PlayerObjData();

        //DEV.DEBUG random equipment
        for (int i=0;i<7;i++){
            InvEquipmentStorage.EquipRandomItem(PlayerData.Equipment,SS.XDB);
        }

        //generate 3 new missions
        AvailableMissions.Clear();
        for (int i=0;i<3;++i){
            AvailableMissions.Add(MissionGenerator.GenerateMission(SS.XDB));
        }
    }

    public void SetCurrentMission(MissionObjData mission)
    {
        CurrentMission=mission;
    }

    public void PlayMission()
    {
        Application.LoadLevel("TestScene_Ilkka");
    }

    public void EndMission()
    {
        GOTO_DEBRIEF=true;
        Application.LoadLevel("MissionSelectScene");
    }
}
