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
	public static Dictionary<string,MissionXmlData> Missions=new Dictionary<string, MissionXmlData>();
	public static Dictionary<string,InvBaseItem> QuestItems=new Dictionary<string,InvBaseItem>();
	public static Dictionary<string,AmmoXmlData> AmmoTypes=new Dictionary<string,AmmoXmlData>();
	public static Dictionary<string,ObjectiveXmlData> Objectives=new Dictionary<string,ObjectiveXmlData>();
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
		MissionRewardRounding,
		StartingDept,
		StartingMoney,
		LightsBrokenPercent,
		LightsFlickerPercent
        ;
    public static float
        HullHeatAddMultiplier,
		WeaponHeatDisperseCoolingMultiplier,
		WeaponHeatDisperseHeatMultiplier,
		HullHeatDisperseHeatMultiplier,
		MechaPartConditionFairThreshold,
		MechaPartConditionBadThreshold,
		MechaPartRepairCostMulti,
		MaxEnemyPercentageOnCorridors,
		OpenDoorChange
    ;

	//Logic
	public static void LoadData(UIAtlas ItemAtlas)
    {
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
	public static MissionXmlData GetMission (string type)
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
	public static void AddObjective(string type,ObjectiveXmlData data){
		if (Objectives.ContainsKey(type)){
			Debug.LogWarning(data.GetType().ToString()+" redefinition: "+type);
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

	public static void AddMission(string type,MissionXmlData data){
		if (Missions.ContainsKey(type)){
			Debug.LogWarning(data.GetType().ToString()+" redefinition: "+data);
			return;
		}
		Missions.Add(type,data);
	}

	public static string GetAttributeName(InvStat.Type a)
	{
		switch (a) 
		{
		case InvStat.Type.AccuracyBoost:
			return "% Accuracy Boost";
		case InvStat.Type.Cooling:
			return "Cooling Rate";
		case InvStat.Type.Firerate:
			return "Rate of Fire";
		case InvStat.Type.HullArmor:
			return "% Hull Damage Reduction";
		case InvStat.Type.HullOverheatLimit:
			return "Hull Overheat Limit";
		case InvStat.Type.MeleeDamage:
			return "% Melee Damage Boost";
		case InvStat.Type.SystemCooling:
			return "% Cooling Boost";
		default:
			return a.ToString();
		}
				
	}
}
