using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameDB : MonoBehaviour {

    public SharedSystemsMain SS;

	public GameObjData  GameData;

	public bool GOTO_DEBRIEF=false;
	public bool GameStarted=false;

    public string HQScene="MissionSelectScene",GameScene="GameScene";

	void OnLevelWasLoaded(int i){
		ResetStuff();
	}

    public void StartNewGame(){
        GameStarted=true;
		GameData=new GameObjData();
        GenerateNewMissions();
		
		GameData.PlayerData.Money=1000;

		GameData.FinanceManager.AddDebt(0,XmlDatabase.StartingDept);

		//starting player equipment
		for (int i=0;i<XmlDatabase.Player.StartingWeaponAmount;i++){
			InvEquipmentStorage.EquipRandomItem(GameData.PlayerData.Equipment,"starting_weapons","starting_quality");
		}
		
		for (int i=0;i<XmlDatabase.Player.StartingUtilityAmount;i++){
			InvEquipmentStorage.EquipRandomItem(GameData.PlayerData.Equipment,"starting_utilities","starting_quality");
		}
		
		RestockVendor();
    }

#if UNITY_EDITOR && !UNITY_WEBPLAYER
	public void Update(){
		if (Input.GetKeyDown(KeyCode.F5)){
			SaveGame();
		}

		if (Input.GetKeyDown(KeyCode.F9)){
			LoadGame();
		}
	}
	#endif

	public void SaveGame(){
		SaveLoadSys.SaveGame("Save",GameData);
	}
	
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
        LoadLevel(GameScene);
    }

	void ResetStuff(){
		//haxy reset for haxy static lists!
		if (UIEquipmentSlot.EquipmentSlots!=null) UIEquipmentSlot.EquipmentSlots.Clear();
		if (UIAmmoSlot.EquipmentSlots!=null) UIAmmoSlot.EquipmentSlots.Clear();
	}

	void LoadLevel(string level){

		Application.LoadLevel(level);
	}

    public void EndMission(GameController GC)
    {
        GOTO_DEBRIEF=true;

		//mission status
		MissionGenerator.UpdateMissionObjectiveStatus(GameData.CurrentMission,GameData.PlayerData,GC);
		int reward=CalculateQuestReward();
		GameData.PlayerData.Money+=reward;
		RemoveQuestItems();
		GameData.PlayerData.ClearDownloadData();

		//world simulation
		GameData.CurrentTime+=GameData.CurrentMission.TravelTime;

		GameData.CurrentMission.ExpirationTime=0;
		UpdateMissions(GameData.CurrentMission.TravelTime);
		RestockVendor();

		UpdateFinanceManager(GameData.CurrentMission.TravelTime);

        LoadLevel(HQScene);
    }

    public int CalculateQuestReward()
    {
		var mission =GameData.CurrentMission;
		int max_reward=mission.Reward;
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
		//get all possible pools
		var availablemissionpools=new List<string>();
		
		foreach (var p in XmlDatabase.MissionPool.Pools.Keys){
			availablemissionpools.Add(p);
		}
		//generate new missions if any are needed
		while (GameData.AvailableMissions.Count<4){
			var index=Subs.GetRandom(availablemissionpools);
			availablemissionpools.Remove(index);
				
			GameData.AvailableMissions.Add(MissionGenerator.GenerateMission(index));
		}
	}

    void UpdateMissions(int time_increase)
    {
		//Remove old missions and find all available mission pools.
		var availablemissionpools=new List<string>();

		foreach (var p in XmlDatabase.MissionPool.Pools.Keys){
			availablemissionpools.Add(p);
		}

		for (int i=0;i<GameData.AvailableMissions.Count;++i){
			var m=GameData.AvailableMissions[i];
			m.ExpirationTime-=time_increase;
			if (m.ExpirationTime<0){
				continue;
			}
			//mission stays. Pool not available
			availablemissionpools.Remove(m.MissionPoolIndex);
		}

		//generate new missions if any are needed
		
		for (int i=0;i<GameData.AvailableMissions.Count;++i){
			var m=GameData.AvailableMissions[i];
			if (m.ExpirationTime<0){
				var index=Subs.GetRandom(availablemissionpools);
				availablemissionpools.Remove(index);

				GameData.AvailableMissions[i]=MissionGenerator.GenerateMission(index);
			}
		}
    }

	//update number of days until update
	void UpdateFinanceManager(int amt_of_days)
	{
		var GDFM = GameData.FinanceManager;
		GDFM.days_till_update -= amt_of_days;
		GDFM.UpdateDays();
	}
	
	public void RestockVendor(){
		GameData.VendorStore.Clear();
		
		for (int i=0;i<Subs.GetRandom(6,8);i++){
			InvItemStorage.EquipRandomItem(GameData.VendorStore,"vendor_items","vendor_quality");
		}
	}
}

public class GameObjData{
	public PlayerObjData PlayerData{get; set;}
	public List<MissionObjData> AvailableMissions{get; set;}
	
	public MissionObjData CurrentMission{get;set;}
	public InvItemStorage VendorStore{get;set;}

	public FinanceManager FinanceManager{get;set;}

	public int CurrentTime{get;set;}

	public int[] TPhighScores{get;set;}

	public GameObjData(){
		CurrentTime=1;
		AvailableMissions=new List<MissionObjData>();
		PlayerData=new PlayerObjData();
		VendorStore=new InvItemStorage(8,4,2);
		FinanceManager = new FinanceManager(PlayerData);

		TPhighScores=new int[5];
	}
}