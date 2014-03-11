using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XmlDatabase : MonoBehaviour
{
    public List<PlayerXmlData> players { get; private set; }
   // public List<WeaponXmlData> weapons { get; private set; }
	public List<InvBaseItem> items{get;private set;}
    public List<EnemyXmlData> enemies { get; private set; }
    public List<ObstacleXmlData> obstacles { get; private set; }
    public Dictionary<MissionObjData.Type,MissionXmlData> Missions{ get; private set; }
    public Dictionary<string,InvBaseItem> QuestItems=new Dictionary<string,InvBaseItem>();
    public Dictionary<string,AmmoXmlData> AmmoTypes=new Dictionary<string,AmmoXmlData>();
    public Dictionary<MissionObjData.Objective,ObjectiveXmlData> Objectives=new Dictionary<MissionObjData.Objective,ObjectiveXmlData>();

	//item specific stuff
	public InvDatabase ItemDB;
	public UIAtlas ItemAtlas;

    //Game Constants
    public static int
        WeaponOverheatDisperseThreshold,
        WeaponChangeableHeatThreshold,
        HullOverheatDisperseThreshold,
        MissionInfoSuccessRating,
        MissionInfoFailRating
        ;
    public static float
        HullHeatMultiplier
    ;

	// Use this for initialization
	public void LoadData()
    {
        Missions =new Dictionary<MissionObjData.Type, MissionXmlData>();
        players = new List<PlayerXmlData>();
	    //weapons = new List<WeaponXmlData>();
        enemies = new List<EnemyXmlData>();
        obstacles = new List<ObstacleXmlData>();

		items=new List<InvBaseItem>();

        XMLDataLoader.Read(this);

        foreach (var i in QuestItems){
            i.Value.iconAtlas=ItemAtlas;
        }

		foreach (var i in items){
			i.iconAtlas=ItemAtlas;
            if(i.ammotype!=""){
                i.AmmoData=AmmoTypes[i.ammotype];
            }
			//add to visual database for debugging
			if (ItemDB!=null) ItemDB.items.Add(i);
		}

        XMLDataLoader.ReadDataFiles();
    }
}
