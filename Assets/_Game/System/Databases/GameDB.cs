using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameDB : MonoBehaviour {

    public SharedSystemsMain SS;

    public PlayerObjData PlayerData{get;private set;}
    public List<MissionObjData> AvailableMissions;

    public MissionObjData CurrentMission{get;set;}
    public InvItemStorage VendorStore{get;set;}

    public bool GOTO_DEBRIEF{get;set;}
    public bool GameStarted{get;set;}

    public string HQScene="MissionSelectScene",GameScene="TestScene_Ilkka";

    public void Awake(){
        GOTO_DEBRIEF=false;
        GameStarted=false;
    }

    public void StartNewGame(){
        GameStarted=true;

        AvailableMissions=new List<MissionObjData>();
        PlayerData=new PlayerObjData(SS.XDB);
        VendorStore=new InvItemStorage(8,4,2);

        PlayerData.Money=1000;

        //DEV.DEBUG random equipment
        for (int i=0;i<4;i++){
            InvEquipmentStorage.EquipRandomItem(PlayerData.Equipment,SS.XDB);
        }

        //DEV.DEBUG random vendor items
        for (int i=0;i<6;i++){
            InvEquipmentStorage.EquipRandomItem(PlayerData.Equipment,SS.XDB);
            VendorStore.Add(InvGameItem.GetRandomItem(SS.XDB));
        }

        GenerateNewMissions();
    }

    public void SetCurrentMission(MissionObjData mission)
    {
        CurrentMission=mission;
    }

    public void PlayMission()
    {
        Application.LoadLevel(GameScene);
    }

    public void EndMission()
    {
        GOTO_DEBRIEF=true;
        GenerateNewMissions();
        Application.LoadLevel(HQScene);
    }

    public void RemoveQuestItems()
    {
        for(int i=0;i<PlayerData.Items.maxItemCount;++i){
            var item=PlayerData.Items.GetItem(i);
            if (item==null) continue;
            if (item.baseItem.type==InvBaseItem.Type.QuestItem){
                PlayerData.Items.Replace(i,null);
                --i;
            }
        }
    }

    void GenerateNewMissions()
    {
        AvailableMissions.Clear();
        for (int i=0;i<3;++i){
            AvailableMissions.Add(MissionGenerator.GenerateMission(SS.XDB));
        }
    }
}
