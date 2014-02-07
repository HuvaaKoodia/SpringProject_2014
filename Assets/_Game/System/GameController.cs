using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TurnState
{
    StartPlayerTurn, PlayerTurn, StartAITurn, AIMovement, AIAttack, WaitingAIToFinish
}

public class GameController : MonoBehaviour {

	public string TestLoadShipName;
	public bool UseTestMap;
	public EngineController EngCont;
	
	public SharedSystemsMain SS {get;private set;}
	
	public TileObjData[,] TileObjectMap;
	public TileMain[,] TileMainMap;

	public AIcontroller aiController;
	
	public TurnState currentTurn = TurnState.StartPlayerTurn;

	public MenuHandler menuHandler;
	public InventoryMain Inventory;

	public List<LootCrateMain> LootCrates {get;private set;}

	public UICamera NGUICamera;
	
    PlayerMain player;

    public PlayerMain Player{
        get{return player;}
        set{
            player=value;
            player.SetObjData(SS.GDB.PlayerData);
        }
    }

	// Use this for initialization
	void Start()
    {
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

		//init menuhandler stuff
		menuHandler.player = player;
		menuHandler.CheckTargetingModePanel();
		menuHandler.radar.Init();

		//link player to hud
		Inventory.SetPlayer(player);

        var ec=GetComponent<EngineController>();
        ec.AfterRestart+=SS.GDB.StartNewGame;

        //DEV.DEBUG random equipment
        for (int i=0;i<7;i++){
            InvEquipmentStorage.EquipRandomItem(player.ObjData.Equipment,SS.XDB);
        }
	}

	// Update is called once per frame
	void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.M)){
            var mission=MissionGenerator.GenerateMission();
            Debug.Log("mission: "+mission.Info);
        }
#endif

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
