using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XmlDatabase
{
    public static List<PlayerXmlData> players { get; private set; }
   // public List<WeaponXmlData> weapons { get; private set; }
	public static Dictionary<string,InvBaseItem> items{get;private set;}
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

	public static string MoneyUnit="BTC";

    //Game Constants
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
		HullHeatDisperseHeatMultiplier
    ;

	// Use this for initialization
	public static void LoadData(UIAtlas ItemAtlas)
    {
        Missions =new Dictionary<MissionObjData.Type, MissionXmlData>();
        players = new List<PlayerXmlData>();
	    //weapons = new List<WeaponXmlData>();
        enemies = new List<EnemyXmlData>();
        obstacles = new List<ObstacleXmlData>();

		items=new Dictionary<string,InvBaseItem>();

        XMLDataLoader.Read();

        foreach (var i in QuestItems){
            i.Value.iconAtlas=ItemAtlas;
        }

		foreach (var i in items.Values){
			i.iconAtlas=ItemAtlas;
            if(i.ammotype!=""){
                i.AmmoData=AmmoTypes[i.ammotype];
            }
		}

        XMLDataLoader.ReadDataFiles();
    }

	public static InvBaseItem GetBaseItem(string name){
		if (items.ContainsKey(name)){
			return items[name];
		}
		Debug.Log("Item not found: "+name);
		return null;
	}


}
