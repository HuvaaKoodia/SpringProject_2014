using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class GameDB : MonoBehaviour {
    public SharedSystemsMain SS;

	public GameObjData  GameData;
	public GameOptionsObjData GameOptionsData;

	public bool GOTO_DEBRIEF=false,GameStarted=false,HasSave,GameLoaded,AllowEscHud=true;

    public string HQScene="MissionSelectScene",GameScene="GameScene",MainMenuScene="MainMenuScene";

	public EscHudMain EscHud;

	void Start(){
		EscHud.Activate(false);
	}

	public void CheckForSaves()
	{
		HasSave=false;
#if UNITY_WEBPLAYER
		//Playerprefs
		if (PlayerPrefs.HasKey("Save")){
			//check file integrity DEV.Lazy as hell.
			if (SaveLoadSys.LoadGamePlayerPrefs("Save")!=null){
				HasSave=true;
			}
		}
#else
		//files
		if (File.Exists("Saves/Save.sav")){
			//check file integrity DEV.Lazy as hell.
			if (SaveLoadSys.LoadGame("Save")!=null){
				HasSave=true;
			}
		}
#endif
	}

	public void CheckForGameOptions(){
		if (SaveLoadSys.HasOptions()){
			GameOptionsData=SaveLoadSys.LoadOptions();
		}
		if (GameOptionsData==null){
			GameOptionsData=new GameOptionsObjData();
			GameOptionsData.quality_level=QualitySettings.GetQualityLevel();
		}

		SS.GOps.SetQualitySettingsToData(GameOptionsData);

	}

	void OnApplicationQuit(){
		SaveLoadSys.SaveOptions(GameOptionsData);
	}
	
    public void CreateNewGame(){
        GameStarted=true;
		GameData=new GameObjData();
		GameData.NewGame();

        GenerateNewMissions();		
		GameData.PlayerData.Money=XmlDatabase.StartingMoney;
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


	public void Update(){

		if (AllowEscHud&&Input.GetKeyDown(KeyCode.Escape)){
			EscHud.Toggle();
		}
	}
	
	public void SaveGame(){
#if UNITY_WEBPLAYER
		SaveLoadSys.SaveGamePlayerPrefs("Save",GameData);
#else
		SaveLoadSys.SaveGame("Save",GameData);
#endif
	}
	
	public void LoadGame(){

		GameData=null;

		#if UNITY_WEBPLAYER
		GameData=SaveLoadSys.LoadGamePlayerPrefs("Save");
		#else
		GameData=SaveLoadSys.LoadGame("Save");
		#endif
		if (GameData==null) return;

		GameStarted=true;
		GameLoaded=true;
		
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

	/// <summary>
	/// Goes to the start game scene.
	/// Does not call CreateNewGame.
	/// </summary>
	public void PlayGame(){
		LoadLevel(HQScene);
	}

    public void PlayMission()
    {
        LoadLevel(GameScene);
    }

	public void LoadMainMenu ()
	{
		LoadLevel(MainMenuScene);
	}
	/// <summary>
	/// Ends the game. Removes saves if ironman and goes back to main menu.
	/// </summary>
	public void EndGame ()
	{
		RemoveSavesIfIronman();
		LoadMainMenu();
	}

	void LoadLevel(string level){
		
		Application.LoadLevel(level);
	}

	void ResetStuff(){
		//haxy reset for haxy static lists!
		if (UIEquipmentSlot.EquipmentSlots!=null) UIEquipmentSlot.EquipmentSlots.Clear();
		if (UIAmmoSlot.EquipmentSlots!=null) UIAmmoSlot.EquipmentSlots.Clear();
	}
	
	void OnLevelWasLoaded(int i){
		ResetStuff();
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

	public void RemoveSavesIfIronman(){
		if (GameData.IronManMode){
			SaveLoadSys.ClearSaves("Save");
		}
	}
}

public class GameObjData{
	public bool IronManMode {get;set;}
	public bool UsedFinanceManager {get;set;}

	public PlayerObjData PlayerData{get; set;}
	public List<MissionObjData> AvailableMissions{get; set;}
	
	public MissionObjData CurrentMission{get;set;}
	public InvItemStorage VendorStore{get;set;}

	public FinanceManager FinanceManager{get;set;}

	public int CurrentTime{get;set;}

	public int[] TPhighScores{get;set;}
	//serializer constructor
	public GameObjData(){}

	public void NewGame(){
		IronManMode=false;
		CurrentTime=1;
		AvailableMissions=new List<MissionObjData>();
		PlayerData=new PlayerObjData();
		VendorStore=new InvItemStorage(8,4,2);
		FinanceManager = new FinanceManager(PlayerData);

		TPhighScores=new int[5];
	}
}