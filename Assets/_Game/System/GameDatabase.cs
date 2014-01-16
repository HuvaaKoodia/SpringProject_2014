using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameDatabase : MonoBehaviour
{

    public List<PlayerData> players { get; private set; }
    public List<WeaponData> weapons { get; private set; }
    public List<EnemyData> enemies { get; private set; }
    public List<ObstacleData> obstacles { get; private set; }

	// Use this for initialization
	void Start()
    {
        players = new List<PlayerData>();
	    weapons = new List<WeaponData>();
        enemies = new List<EnemyData>();
        obstacles = new List<ObstacleData>();

        XMLDataLoader.Read(this);
	}
	
	// Update is called once per frame
	void Update()
    {
		
	}
}
