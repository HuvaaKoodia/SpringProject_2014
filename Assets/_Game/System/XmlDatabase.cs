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

	//item specific stuff
	public InvDatabase ItemDB;
	public UIAtlas ItemAtlas;

    //Game Constants
    public static int
        OverheatDissipateThreshold,
        WeaponChangeableHeatThreshold
        ;

	// Use this for initialization
	void Awake()
    {
        players = new List<PlayerXmlData>();
	    //weapons = new List<WeaponXmlData>();
        enemies = new List<EnemyXmlData>();
        obstacles = new List<ObstacleXmlData>();

		items=new List<InvBaseItem>();

        XMLDataLoader.Read(this);

		foreach (var i in items){
			i.iconAtlas=ItemAtlas;
			//add to visual database for debugging
			if (ItemDB!=null) ItemDB.items.Add(i);
		}

        XMLDataLoader.ReadConstants();
    }
}
