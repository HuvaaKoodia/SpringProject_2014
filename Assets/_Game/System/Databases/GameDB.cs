using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameDB : MonoBehaviour {

    public SharedSystemsMain SS;

    public PlayerObjData PlayerData{get;private set;}
    public List<MissionObjData> AvailableMissions;

    public void StartNewGame(){
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
}
