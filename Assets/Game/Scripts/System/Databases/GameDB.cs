﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameDB : MonoBehaviour {

    public SharedSystemsMain SS;

	public GameObjData GameData;

	public bool GOTO_DEBRIEF=false;
	public bool GameStarted=false;

    public string HQScene="MissionSelectScene",GameScene="GameScene";

    public void StartNewGame(){
        GameStarted=true;
		GameData=new GameObjData();
        GenerateNewMissions();
    }

#if UNITY_EDITOR 
	public void Update(){
		if (Input.GetKeyDown(KeyCode.F5)){
			SaveLoadSys.SaveGame("Save",GameData);
		}

		if (Input.GetKeyDown(KeyCode.F9)){
			LoadGame();
		}
	}
#endif
	public void LoadGame(){
		GameStarted=true;

		GameData=SaveLoadSys.LoadGame("Save");

		//init GameData
		
		foreach(var e in GameData.VendorStore.items){
			if (e!=null) e.InitBaseItem();
		}
		foreach(var e in GameData.PlayerData.Items.items){
			if (e!=null) e.InitBaseItem();
		}

		foreach(var e in GameData.PlayerData.Equipment.EquipmentSlots){
			if (e.Item!=null) e.Item.InitBaseItem();
		}

		Application.LoadLevel(HQScene);
	}

    public void SetCurrentMission(MissionObjData mission)
    {
        GameData.CurrentMission=mission;
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

    public int CalculateQuestReward()
    {
		var mission =GameData.CurrentMission;
		int max_reward=mission.XmlData.Reward;
		int max=mission.PrimaryObjectives.Count;
		float completed=mission.PrimaryObjectives.Count(obj=>obj.status==1);
		float reward=max_reward*(completed/max);
		return (int)reward;
    }

    public void RemoveQuestItems()
    {
		for(int i=0;i<GameData.PlayerData.Items.maxItemCount;++i){
			var item=GameData.PlayerData.Items.GetItem(i);
            if (item==null) continue;
            if (item.baseItem.type==InvBaseItem.Type.QuestItem){
				GameData.PlayerData.Items.Replace(i,null);
                --i;
            }
        }
    }

    void GenerateNewMissions()
    {
		GameData.AvailableMissions.Clear();
        for (int i=0;i<4;++i){
			GameData.AvailableMissions.Add(MissionGenerator.GenerateMission());
        }
    }
}

public class GameObjData{
	public PlayerObjData PlayerData{get; set;}
	public List<MissionObjData> AvailableMissions{get; set;}
	
	public MissionObjData CurrentMission{get;set;}
	public InvItemStorage VendorStore{get;set;}
	
	public GameObjData(){
		AvailableMissions=new List<MissionObjData>();
		PlayerData=new PlayerObjData();
		VendorStore=new InvItemStorage(8,4,2);

		PlayerData.Money=1000;
		
		//DEV.DEBUG random equipment
		for (int i=0;i<4;i++){
			InvEquipmentStorage.EquipRandomItem(PlayerData.Equipment);
		}
		
		//DEV.DEBUG random vendor items
		for (int i=0;i<6;i++){
			InvEquipmentStorage.EquipRandomItem(PlayerData.Equipment);
			VendorStore.Add(InvGameItem.GetRandomItem());
		}

	}
}