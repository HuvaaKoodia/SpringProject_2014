using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum TurnState
{
    PlayerTurn, AiTurn, WaitingAIToFinish
}

public class GameController : MonoBehaviour {
	
	//public List<TileObjData> TileObjects{get;private set;}
	//public List<TileMain> Tiles{get;private set;}
	public bool UseTestMap;
	public EngineController EngCont;
	
	public MapGenerator MapGen;
	public ShipGenerator ShipGen;
	
	public TileObjData[,] TileObjectMap;
	public TileMain[,] TileMainMap;

    int currentEnemy = 0;
    int finishedEnemies = 0;
    public List<EnemyMain> enemies;

    TurnState currentTurn = TurnState.PlayerTurn;

	// Use this for initialization
	void Start()
    {
		//TileObjects=new List<TileObjData>();
		//Tiles=new List<TileMain>();
       	enemies = new List<EnemyMain>();

		if (UseTestMap)
		{
			var testfloor = MapGen.XmlMapRead.Rooms["pathfindingtest"][0];
			MapGen.GenerateObjectDataMap(this,testfloor);
			MapGen.GenerateSceneMap(this);
		}
		else{
			var ship_objdata=ShipGen.GenerateShipObjectData();
			MapGen.GenerateObjectDataMap(this,ship_objdata.Floors[0]);
			MapGen.GenerateShipItems(this,ship_objdata);
			MapGen.GenerateSceneMap(this);
		}
	}

	// Update is called once per frame
	void Update()
    {
		if (!UseTestMap) return;

        //AI:n pyörittely, pois täältä jossain vaiheessa?
        if (currentTurn == TurnState.AiTurn && enemies.Count > 0)
        {
            enemies[currentEnemy].SendMessage("RandomMovement");
            currentEnemy++;
        }
        else if (currentTurn == TurnState.WaitingAIToFinish && enemies.Count == finishedEnemies)
        {
            finishedEnemies = 0;
            ChangeTurn();
        }

		if (currentEnemy == enemies.Count)
		{
			currentEnemy = 0;
			ChangeTurn();
		}
	}

	/// <summary>
	/// Resets the tile object map.
	/// Doesn't create objects.
	/// </summary>
	public void ResetTileObjectMap (int w, int h)
	{
		TileObjectMap=new TileObjData[w,h];
	}
	
	/// <summary>
	/// Resets the tile main map.
	/// Doesn't create objects.
	/// </summary>
	public void ResetTileMainMap (int w, int h)
	{
		TileMainMap=new TileMain[w,h];
	}

    public void ChangeTurn()
    {
        if (currentTurn == TurnState.PlayerTurn)
        {
            currentTurn = TurnState.AiTurn;
        }
        else if (currentTurn == TurnState.AiTurn)
        {
            currentTurn = TurnState.WaitingAIToFinish;
        }
        else
        {
            currentTurn = TurnState.PlayerTurn;
            GameObject.Find("Player").SendMessage("StartTurn");
        }
    }

    public void EnemyFinishedTurn()
    {
        finishedEnemies++;
    }
}
