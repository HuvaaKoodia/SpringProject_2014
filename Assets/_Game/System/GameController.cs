using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum TurnState
{
    PlayerTurn, AiTurn
}

public class GameController : MonoBehaviour {
	
	//public List<TileObjData> TileObjects{get;private set;}
	//public List<TileMain> Tiles{get;private set;}
	
	public MapGenerator MapGen;
	
	public TileObjData[,] TileObjectMap;
	public TileMain[,] TileMainMap;

    int currentEnemy = 0;
    public List<EnemyMain> enemies;

    TurnState currentTurn = TurnState.PlayerTurn;

	// Use this for initialization
	void Start()
    {
		//TileObjects=new List<TileObjData>();
		//Tiles=new List<TileMain>();
        enemies = new List<EnemyMain>();

		MapGen.GenerateObjectDataMap(this);
		MapGen.GenerateSceneMap(this);
	}
	
	// Update is called once per frame
	void Update()
    {
        //AI:n pyörittely, pois täältä jossain vaiheessa?
        if (currentTurn == TurnState.AiTurn && enemies.Count > 0)
        {
            enemies[currentEnemy].SendMessage("RandomMovement");
            currentEnemy++;

            if (currentEnemy == enemies.Count)
            {
                currentEnemy = 0;
                ChangeTurn();
            }
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
        else
        {
            currentTurn = TurnState.PlayerTurn;
            GameObject.Find("Player").SendMessage("StartTurn");
        }
    }
}
