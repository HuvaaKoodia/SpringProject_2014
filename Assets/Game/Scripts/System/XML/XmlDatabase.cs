using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XmlDatabase
{
    public static List<PlayerXmlData> players { get; private set; }
   // public List<WeaponXmlData> weapons { get; private set; }
	public static List<InvBaseItem> items{get;private set;}
	public static List<EnemyXmlData> enemies { get; private set; }
	public static List<ObstacleXmlData> obstacles { get; private set; }
	public static Dictionary<MissionObjData.Type,MissionXmlData> Missions{ get; private set; }
	public static Dictionary<string,InvBaseItem> QuestItems=new Dictionary<string,InvBaseItem>();
	public static Dictionary<string,AmmoXmlData> AmmoTypes=new Dictionary<string,AmmoXmlData>();
	public static Dictionary<MissionObjData.Objective,ObjectiveXmlData> Objectives=new Dictionary<MissionObjData.Objective,ObjectiveXmlData>();

	public static string MoneyUnit="BTC";

    //Game Constants
    public static int
		WeaponOverheatEndThreshold,
        WeaponChangeableHeatThreshold,
		HullOverheatEndThreshold,
        MissionInfoSuccessRating,
        MissionInfoFailRating,
		HullHeatDisperseConstant,
		MissionCatastrophicIntelFailureChance
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

		items=new List<InvBaseItem>();

        XMLDataLoader.Read();

        foreach (var i in QuestItems){
            i.Value.iconAtlas=ItemAtlas;
        }

		foreach (var i in items){
			i.iconAtlas=ItemAtlas;
            if(i.ammotype!=""){
                i.AmmoData=AmmoTypes[i.ammotype];
            }
		}

        XMLDataLoader.ReadDataFiles();
    }

	public static InvBaseItem GetBaseItem(string name){
		foreach (var i in items){
			if (i.name==name) return i;
		}
		return null;
	}
}
