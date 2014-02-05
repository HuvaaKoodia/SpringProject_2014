﻿using UnityEngine;
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
	
	public SharedSystemsMain SS {get;private set;}
	
	public TileObjData[,] TileObjectMap;
	public TileMain[,] TileMainMap;

	public AIcontroller aiController;
	public PlayerMain player;

	public TurnState currentTurn = TurnState.StartPlayerTurn;

	public MenuHandler menuHandler;
	public InventoryMain Inventory;

	public List<LootCrateMain> LootCrates {get;private set;}

	public UICamera NGUICamera;
	
	// Use this for initialization
	void Start()
    {
		//TileObjects=new List<TileObjData>();
		//Tiles=new List<TileMain>();

		SS=GameObject.FindGameObjectWithTag("SharedSystems").GetComponent<SharedSystemsMain>();

		LootCrates=new List<LootCrateMain>();
		aiController = new AIcontroller(this);
		ShipObjData ship_objdata=null;
		if (UseTestMap)
		{
			ship_objdata=SS.SGen.GenerateShipObjectData("testship");
		}
		else
		{
			ship_objdata=SS.SGen.GenerateShipObjectData(TestLoadShipName);
		}

		SS.MGen.GenerateObjectDataMap(this,ship_objdata.Floors[0]);
		SS.MGen.GenerateShipItems(this,ship_objdata);
		SS.MGen.GenerateSceneMap(this);
		SS.MGen.GenerateLoot(this,ship_objdata);

		menuHandler.player = player;
		menuHandler.CheckTargetingModePanel();


		//link player to hud
		Inventory.SetPlayer(player);
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
			ChangeTurn(TurnState.PlayerTurn);
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
		menuHandler.turnText.gameObject.SetActive(currentTurn == TurnState.PlayerTurn);
    }

	void StartPlayerTurn()
	{
		player.StartPlayerPhase();
	}
}
