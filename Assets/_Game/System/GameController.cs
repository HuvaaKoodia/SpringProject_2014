using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TurnState
{
    StartPlayerTurn, PlayerTurn, StartAITurn, AIMovement, AIAttack, WaitingAIToFinish
}

public class GameController : MonoBehaviour {
	
	//public List<TileObjData> TileObjects{get;private set;}
	//public List<TileMain> Tiles{get;private set;}
	public string TestLoadShipName;
	public bool UseTestMap;
	public EngineController EngCont;
	
	public MapGenerator MapGen;
	public ShipGenerator ShipGen;
	
	public TileObjData[,] TileObjectMap;
	public TileMain[,] TileMainMap;

	public AIcontroller aiController;

	public PlayerMain player;

	public TurnState currentTurn = TurnState.StartPlayerTurn;
	
	// Use this for initialization
	void Start()
    {
		//TileObjects=new List<TileObjData>();
		//Tiles=new List<TileMain>();

		aiController = new AIcontroller(this);

		if (UseTestMap)
		{
			var testfloor = MapGen.XmlMapRead.Rooms["pathfindingtest"][0];
			MapGen.GenerateObjectDataMap(this,testfloor);
			MapGen.GenerateSceneMap(this);
		}
		else
		{
			var ship_objdata=ShipGen.GenerateShipObjectData(TestLoadShipName);
			MapGen.GenerateObjectDataMap(this,ship_objdata.Floors[0]);
			MapGen.GenerateShipItems(this,ship_objdata);
			MapGen.GenerateSceneMap(this);
		}
	}

	// Update is called once per frame
	void Update()
    {

		if (currentTurn != TurnState.PlayerTurn && currentTurn != TurnState.StartPlayerTurn)
		{
			aiController.UpdateAI(currentTurn);
		}
		else if (currentTurn == TurnState.StartPlayerTurn)
		{
			StartPlayerTurn();
			currentTurn = TurnState.PlayerTurn;
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

    public void ChangeTurn(TurnState turn)
    {
		currentTurn = turn;
    }

	void StartPlayerTurn()
	{
		player.StartPlayerPhase();
	}
}
