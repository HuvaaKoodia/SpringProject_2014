using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XmlDatabase
{
	//data
	public static PlayerXmlData Player;
	public static Dictionary<string,InvBaseItem> Items{get;private set;}
	public static List<EnemyXmlData> enemies { get; private set; }
	public static List<ObstacleXmlData> obstacles { get; private set; }
	public static Dictionary<MissionObjData.Type,MissionXmlData> Missions{ get; private set; }
	public static Dictionary<string,InvBaseItem> QuestItems=new Dictionary<string,InvBaseItem>();
	public static Dictionary<string,AmmoXmlData> AmmoTypes=new Dictionary<string,AmmoXmlData>();
	public static Dictionary<MissionObjData.Objective,ObjectiveXmlData> Objectives=new Dictionary<MissionObjData.Objective,ObjectiveXmlData>();
	public static Dictionary<string,RewardClassXmlData> RewardClasses=new Dictionary<string,RewardClassXmlData>();
	public static PoolContainerXmlData LootPool=new PoolContainerXmlData("LootPool");
	public static PoolContainerXmlData MissionPool=new PoolContainerXmlData("MissionPool");
	public static PoolContainerXmlData LootQualityPool=new PoolContainerXmlData("LootQuality");
	
	//Constants
	public static string MoneyUnit="BTC";
	
    public static int
		WeaponOverheatEndThreshold,
        WeaponChangeableHeatThreshold,
		HullOverheatEndThreshold,
        MissionInfoSuccessRating,
        MissionInfoFailRating,
		HullHeatDisperseConstant,
		MissionCatastrophicIntelFailureChance,
		AttackFrontBackPlayerTorsoHitChance,
		AttackLeftRightPlayerTorsoHitChance,
		AttackTorsoUtilityDamageChance,
		MissionRewardRounding
        ;
    public static float
        HullHeatAddMultiplier,
		WeaponHeatDisperseCoolingMultiplier,
		WeaponHeatDisperseHeatMultiplier,
		DisperseHeatMultiplier,
		HullHeatDisperseHeatMultiplier,
		ConditionFairThreshold,
		ConditionBadThreshold
    ;

	//Logic
	public static void LoadData(UIAtlas ItemAtlas)
    {
        Missions =new Dictionary<MissionObjData.Type, MissionXmlData>();
        enemies = new List<EnemyXmlData>();
        obstacles = new List<ObstacleXmlData>();

		Items=new Dictionary<string,InvBaseItem>();

        XMLDataLoader.Read();

        foreach (var i in QuestItems){
            i.Value.iconAtlas=ItemAtlas;
        }

		foreach (var i in Items.Values){
			i.iconAtlas=ItemAtlas;
            if(i.ammotype!=""){
                i.AmmoData=AmmoTypes[i.ammotype];
            }
		}

        XMLDataLoader.ReadDataFiles();
    }

	//getters
	public static MissionXmlData GetMission (MissionObjData.Type type)
	{
		return Missions[type];
	}

	public static InvBaseItem GetBaseItem(string name){
		if (Items.ContainsKey(name)){
			return Items[name];
		}
		Debug.Log("Item not found: "+name);
		return null;
	}
	
	public static InvBaseItem GetQuestItem(string name){
		if (QuestItems.ContainsKey(name)){
			return QuestItems[name];
		}
		Debug.Log("Quest Item not found: "+name);
		return null;
	}
	
	//Defensive add data functions
	public static void AddRewardClass(string name,RewardClassXmlData data){
		if (RewardClasses.ContainsKey(name)){
			Debug.LogWarning(data.GetType().ToString()+" redefinition: "+name);
			return;
		}
		RewardClasses.Add(name,data);
	}
	public static void AddObjective(string name,ObjectiveXmlData data){
		var type=(MissionObjData.Objective)System.Enum.Parse(typeof(MissionObjData.Objective),name,true);
		if (Objectives.ContainsKey(type)){
			Debug.LogWarning(data.GetType().ToString()+" redefinition: "+name);
			return;
		}
		Objectives.Add(type,data);
	}
	
	public static void AddAmmoType(string name,AmmoXmlData data){
		if (AmmoTypes.ContainsKey(name)){
			Debug.LogWarning(data.GetType().ToString()+" redefinition: "+name);
			return;
		}
		AmmoTypes.Add(name,data);
	}

	public static void AddItem(InvBaseItem data){
		var name=data.name;
		if (Items.ContainsKey(name)){
			Debug.LogWarning(data.GetType().ToString()+" redefinition: "+name);
			return;
		}
		Items.Add(name,data);
	}

	public static void AddQuestItem(InvBaseItem data){
		var name=data.name;
		if (QuestItems.ContainsKey(name)){
			Debug.LogWarning(data.GetType().ToString()+" redefinition: "+name);
			return;
		}
		QuestItems.Add(name,data);
	}

	public static void AddMission(string name,MissionXmlData data){
		var type=(MissionObjData.Type)System.Enum.Parse(typeof(MissionObjData.Type),name,true);
		if (Missions.ContainsKey(type)){
			Debug.LogWarning(data.GetType().ToString()+" redefinition: "+data);
			return;
		}
		Missions.Add(type,data);
	}
}
