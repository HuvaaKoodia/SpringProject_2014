using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TurnState
{
    StartPlayerTurn, PlayerTurn, StartAITurn, AITurn, WaitingAIToFinish
}

public class GameController : MonoBehaviour {

	public string TestLoadShipName;
	public bool UseTestMap,OverrideMissionShip=false;
	public EngineController EngCont;
	
	public SharedSystemsMain SS {get;private set;}
	
	public TileObjData[,] TileObjectMap;
	public TileMain[,] TileMainMap;

	public AIcontroller aiController;
	
	public TurnState currentTurn = TurnState.StartPlayerTurn;

	public MenuHandler menuHandler;
	public InventoryMain Inventory;

	public List<LootCrateMain> LootCrates {get;private set;}

    PlayerMain player;
	bool do_culling=true;

	public HaxKnifeCulling culling_system;

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

        //DEV.DEBUG generate mission
        if (SS.GDB.CurrentMission==null){
            SS.GDB.CurrentMission=MissionGenerator.GenerateMission(SS.XDB);
        }
        menuHandler.MissionBriefing.SetMission(SS.GDB.CurrentMission);

		if (UseTestMap)
		{
			ship_objdata=SS.SGen.GenerateShipObjectData("testship");
		}
		else
		{
            if (OverrideMissionShip){
			    ship_objdata=SS.SGen.GenerateShipObjectData(TestLoadShipName);
            }
            else{
                var mission_ship=Subs.GetRandom(SS.GDB.CurrentMission.XmlData.ShipsTypes);
                ship_objdata=SS.SGen.GenerateShipObjectData(mission_ship);
            }
		}

		SS.MGen.GenerateObjectDataMap(this,ship_objdata.Floors[0]);
		SS.SDGen.GenerateShipItems(this,ship_objdata);
		SS.MGen.GenerateSceneMap(this);
        SS.SDGen.GenerateLoot(this,ship_objdata);

        if (!OverrideMissionShip)
            SS.SDGen.GenerateMissionObjectives(this,SS.GDB.CurrentMission,ship_objdata,SS.XDB);

		//init hud
		menuHandler.player = player;
		menuHandler.CheckTargetingModePanel();
        menuHandler.SetGC(this);

		Inventory.SetPlayer(player);

        var ec=GetComponent<EngineController>();
        ec.AfterRestart+=SS.GDB.StartNewGame;

        player.ActivateEquippedItems();
	}

	// Update is called once per frame
	void Update()
    {
		if (currentTurn != TurnState.PlayerTurn && currentTurn != TurnState.StartPlayerTurn)
		{
			//Profiler.BeginSample("AI");
			aiController.UpdateAI(currentTurn);
			//Profiler.EndSample();
		}
		else if (currentTurn == TurnState.StartPlayerTurn)
		{
			StartPlayerTurn();
			ChangeTurn(TurnState.PlayerTurn);
		}

#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.C)){
			do_culling=!do_culling;
			if (!do_culling) culling_system.ResetCulling(this);
		}
#endif
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

    public TileObjData GetTileObj(int x,int y){
        if (Subs.insideArea(x,y,0,0,TileMapW,TileMapH)){
            return TileObjectMap[x,y];
        }
        return null;
    }

    public TileMain GetTileMain(int x,int y){
        if (Subs.insideArea(x,y,0,0,TileMapW,TileMapH)){
            return TileMainMap[x,y];
        }
        return null;
    }

    public int TileMapW{get{return TileObjectMap.GetLength(0);}}
    public int TileMapH{get{return TileObjectMap.GetLength(1);}}

	public void CullWorld (Vector3 position, Vector3 targetPosition,float max_distance)
	{
		if (do_culling) culling_system.CullBasedOnPositions(position,targetPosition,max_distance,this);
	}
}
