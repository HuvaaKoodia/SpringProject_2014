using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameDatabase : MonoBehaviour
{

    public List<PlayerXmlData> players { get; private set; }
    public List<WeaponXmlData> weapons { get; private set; }
    public List<EnemyXmlData> enemies { get; private set; }
    public List<ObstacleXmlData> obstacles { get; private set; }

	// Use this for initialization
	void Start()
    {
        players = new List<PlayerXmlData>();
	    weapons = new List<WeaponXmlData>();
        enemies = new List<EnemyXmlData>();
        obstacles = new List<ObstacleXmlData>();

        XMLDataLoader.Read(this);
	}
	
	// Update is called once per frame
	void Update()
    {
		
	}
}
