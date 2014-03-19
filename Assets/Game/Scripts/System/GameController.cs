using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TurnState
{
    StartPlayerTurn, PlayerTurn, StartAITurn, AITurn, WaitingAIToFinish
}

public class FloorObjData{
	public int FloorIndex{get;set;}

	public TileObjData[,] TileObjectMap {get;private set;}
	public TileMain[,] TileMainMap {get;private set;}
	public List<EnemyMain> Enemies {get;private set;}
	public List<LootCrateMain> LootCrates {get;private set;}
	public List<Vector2> AirlockPositions{get;private set;}

	public int TileMapW{get{return TileObjectMap.GetLength(0);}}
	public int TileMapH{get{return TileObjectMap.GetLength(1);}}

	public FloorObjData(){
		Enemies=new List<EnemyMain>();
		LootCrates= new List<LootCrateMain>();
		AirlockPositions=new List<Vector2>();
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
}

public class GameController : MonoBehaviour {

	public string TestLoadShipName;
	public bool UseTestMap,OverrideMissionShip=false;
	public EngineController EngCont;
	
	public SharedSystemsMain SS {get;private set;}

	public int CurrentFloorIndex{get{
			return player.CurrentFloorIndex;
		}
	}
	public FloorObjData CurrentFloorData{get{
			return Floors[CurrentFloorIndex];
		}
	}

	public List<FloorObjData> Floors{get;set;}

	public AIcontroller aiController;
	public TurnState currentTurn = TurnState.StartPlayerTurn;

	public MenuHandler menuHandler;
	public InventoryMain Inventory;

    PlayerMain player;
	public bool do_culling=false;

	public HaxKnifeCulling culling_system;

    public PlayerMain Player{
        get{return player;}
        set{
            player=value;
            player.SetObjData(SS.GDB.GameData.PlayerData);
        }
    }

	public List<GameObject> FloorContainers{get;private set;}

	void Awake(){
		Floors=new List<FloorObjData>();
		FloorContainers=new List<GameObject>();
	}

	// Use this for initialization
	void Start()
    {
		SS=GameObject.FindGameObjectWithTag("SharedSystems").GetComponent<SharedSystemsMain>();
	
		aiController = new AIcontroller(this);
		ShipObjData ship_objdata=null;

        //DEV.DEBUG generate mission
        if (SS.GDB.GameData.CurrentMission==null){
			SS.GDB.GameData.CurrentMission=MissionGenerator.GenerateMission(SS.XDB);
        }
		menuHandler.MissionBriefing.SetMission(SS.GDB.GameData.CurrentMission);

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
				var mission_ship=Subs.GetRandom(SS.GDB.GameData.CurrentMission.XmlData.ShipsTypes);
                ship_objdata=SS.SGen.GenerateShipObjectData(mission_ship);
            }
		}

		for (int i=0;i<ship_objdata.Floors.Count;++i){
			var floor=new FloorObjData();
			floor.FloorIndex=i;
			Floors.Add(floor);
			SS.MGen.GenerateObjectDataMap(floor,ship_objdata.Floors[i]);
			SS.SDGen.GenerateShipItems(this,floor,ship_objdata);
			SS.MGen.GenerateSceneMap(this,floor);
			SS.SDGen.GenerateLoot(floor,SS.XDB);
			Debug.Log("Floor: "+i+" loaded");
		}

        if (!OverrideMissionShip)
			SS.SDGen.GenerateMissionObjectives(this,SS.GDB.GameData.CurrentMission,ship_objdata,SS.XDB);

		//create player
		var legit_floors=new List<FloorObjData>();
		foreach(var f in Floors){
			if (f.AirlockPositions.Count>0){
				legit_floors.Add(f);
			}
		}
		var start_floor=Subs.GetRandom(legit_floors);
		Player=SS.MGen.CreatePlayer(start_floor);
		player.CurrentFloorIndex=start_floor.FloorIndex;
		player.GC=this;

		//init hud
		menuHandler.player = player;
		menuHandler.CheckTargetingModePanel();
        menuHandler.SetGC(this);

		Inventory.SetPlayer(player);

        var ec=GetComponent<EngineController>();
        ec.AfterRestart+=SS.GDB.StartNewGame;

        player.ActivateEquippedItems();

		SetFloor(CurrentFloorIndex);
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
			if (do_culling)
				Player.CullWorld();
			else
				culling_system.ResetCulling(this);
		}

		if (Input.GetKeyDown(KeyCode.V)){
			Player.CullWorld();
		}

		if (Input.GetKeyDown(KeyCode.Alpha8)){
			SetFloor(Mathf.Max(0,CurrentFloorIndex-1));
		}
		if (Input.GetKeyDown(KeyCode.Alpha9)){
			SetFloor(Mathf.Min(Floors.Count-1,CurrentFloorIndex+1));
		}
#endif
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

	public void CullWorld (Vector3 position, Vector3 targetPosition,float max_distance)
	{
		if (do_culling) culling_system.CullBasedOnPositions(position,targetPosition,max_distance,this);
	}

	public FloorObjData GetFloor (int index)
	{
		return Floors[index];
	}

	public void SetFloor(int index){
		player.CurrentFloorIndex=index;
		culling_system.DisableOtherFloors(index,this);
		aiController.SetFloor(CurrentFloorData);
		player.interactSub.CheckForInteractables();
	}
}
