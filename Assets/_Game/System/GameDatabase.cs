using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameDatabase : MonoBehaviour
{
    public List<PlayerXmlData> players { get; private set; }
   // public List<WeaponXmlData> weapons { get; private set; }
	public List<InvBaseItem> items{get;private set;}
    public List<EnemyXmlData> enemies { get; private set; }
    public List<ObstacleXmlData> obstacles { get; private set; }

	//item specific stuff
	public InvDatabase ItemDB;
	public UIAtlas ItemAtlas;

	// Use this for initialization
	void Start()
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
	}
}
