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
	public MeshCombiner MeshCombi;

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
			SS.GDB.GameData.CurrentMission=MissionGenerator.GenerateMission(Subs.GetRandom(XmlDatabase.MissionPool.Pool.Keys));
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
			SS.SDGen.GenerateLoot(floor,SS.GDB.GameData.CurrentMission.MissionPoolIndex);
			Debug.Log("Floor: "+i+" loaded");
		}

        if (!OverrideMissionShip)
			SS.SDGen.GenerateMissionObjectives(this,SS.GDB.GameData.CurrentMission,ship_objdata);

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

		//Dev. temp.
		if (Input.GetKeyDown(KeyCode.B)){
			MeshCombi.Combine(this,0);
		}

		if (Input.GetKeyDown(KeyCode.Alpha8)){
			GotoFloor(CurrentFloorIndex-1);
		}
		if (Input.GetKeyDown(KeyCode.Alpha9)){
			GotoFloor(CurrentFloorIndex+1);
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

	public void GotoFloor(int index){
		if (index<0||index>Floors.Count-1) return;
		
		menuHandler.FadeIn();
		StartCoroutine(GotoFloorTimer(index));
	}

	public void SetFloor(int index){                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
		index=Mathf.Max(0,Mathf.Min(Floors.Count-1,index));

		player.CurrentFloorIndex=index;
		culling_system.DisableOtherFloors(index,this);
		aiController.SetFloor(CurrentFloorData);
		player.interactSub.CheckForInteractables();
	}

	private IEnumerator GotoFloorTimer(int index){
		while(menuHandler.FadeInProgress){
			yield return null;
		}
		menuHandler.FadeOut();
		SetFloor(index);
		//DEV. elevator sound here + some delay
	}

	public void UseElevator ()
	{
		//Dev.Mega HAX!
		if (CurrentFloorIndex==0)
			GotoFloor(CurrentFloorIndex+1);
		else
			GotoFloor(CurrentFloorIndex-1);
	}





	//function to enable a specific light of a specific type or all lights in a specific TileMain in the environment for a specific floor
	//switch case handles the types of lights to enable
	public void EnableLights_FloorNum(int floor_num, int tilemain_X, int tilemain_Y, float power, Lighting_State LS)
	{
		var tilegraphics = GetFloor(floor_num).GetTileMain(tilemain_X, tilemain_Y).TileGraphics;
		
		//as long as there are TileGraphics and TileLights
		if(tilegraphics != null)
		{
			if(tilegraphics.TileLights != null)
			{
				tilegraphics.TileLights.EnableLights(power);
			}
		}
	}

	//function to enable a specific light of a specific type or all lights in the environment for a specific floor
	public void EnableLights_FloorNum(int floor_num, float power, Lighting_State LS)
	{
		//as long as there are TileMainMaps
		if(GetFloor(floor_num).TileMainMap != null)
		{
			//traverse through all the TileMainMaps
			for(int x = 0; x < GetFloor(floor_num).TileMainMap.GetLength(0); x++)
			{
				for(int y = 0; y < GetFloor(floor_num).TileMainMap.GetLength(1); y++)
				{
					EnableLights_FloorNum(floor_num, x, y, power, LS);
				}
			}
		}
	}

	//function to enable a specific light of a specific type or all lights in the environment for all floors
	public void EnableLights_AllFloors(float power, Lighting_State LS)
	{
		//as long as there are Floors
		if(Floors != null)
		{
			//traverse through all the floors
			for(int floornum = 0; floornum < Floors.Count; floornum++)
			{
				EnableLights_FloorNum(floornum, power, LS);
			}
		}
	}





	public Lighting_State RandomizeStartState(int broken_percent, int flickering_percent, int normal_percent)
	{
		int totalpercent = broken_percent + flickering_percent + normal_percent;

		if(totalpercent != 100)
		{
			Debug.Log("Percentages passed in do not add up to 100%");
			return Lighting_State.Normal;
		}

		int value = Subs.RandomPercent();

		if(value < broken_percent)
		{
			return Lighting_State.Broken;
		}
		else if(value >= broken_percent && value < flickering_percent)
		{
			return Lighting_State.Flickering;
		}
		else if(value >= flickering_percent && value < normal_percent)
		{
			return Lighting_State.Normal;
		}
		else
		{
			return Lighting_State.Broken;
		}
	}





//	//function to enable all lights of a specific type or all lights in a specific TileMain in the environment for a specific floor
//	//switch case handles the types of lights to enable
//	public void EnableLights_FloorNum(int floor_num, int tilemain_X, int tilemain_Y, bool on, Environment_Light EL)
//	{
//		var tilegraphics = GetFloor(floor_num).GetTileMain(tilemain_X, tilemain_Y).TileGraphics;
//
//		//as long as there are TileGraphics and TileLights
//		if(tilegraphics != null)
//		{
//			if(tilegraphics.TileLights != null)
//			{
//				switch(EL)
//				{
//				case Environment_Light.WhiteLight:
//					tilegraphics.TileLights.EnableLights(on, Environment_Light.WhiteLight);
//					break;
//				case Environment_Light.OrangeLight:
//					tilegraphics.TileLights.EnableLights(on, Environment_Light.OrangeLight);
//					break;
//				case Environment_Light.AllLight:
//					tilegraphics.TileLights.EnableLights(on, Environment_Light.AllLight);
//					break;
//				default:
//					Debug.Log("PASS IN WhiteLight, OrangeLight OR AllLight ONLY");
//					break;
//				}
//			}
//		}
//	}
//
//	//function to enable all lights of a specific type or all lights in the environment for a specific floor
//	public void EnableLights_FloorNum(int floor_num, bool on, Environment_Light EL)
//	{
//		//as long as there are TileMainMaps
//		if(GetFloor(floor_num).TileMainMap != null)
//		{
//			//traverse through all the TileMainMaps
//			for(int x = 0; x < GetFloor(floor_num).TileMainMap.GetLength(0); x++)
//			{
//				for(int y = 0; y < GetFloor(floor_num).TileMainMap.GetLength(1); y++)
//				{
//					EnableLights_FloorNum(floor_num, x, y, on, EL);
//				}
//			}
//		}
//	}
//
//	//function to enable all lights of a specific type or all lights in the environment for all floors
//	public void EnableLights_AllFloors(bool on, Environment_Light EL)
//	{
//		//as long as there are Floors
//		if(Floors != null)
//		{
//			//traverse through all the floors
//			for(int floornum = 0; floornum < Floors.Count; floornum++)
//			{
//				EnableLights_FloorNum(floornum, on, EL);
//			}
//		}
//	}
	
	
	
	
	
//	//function to enable a specific light of a specific type or all lights in a specific TileMain in the environment for a specific floor
//	//switch case handles the types of lights to enable
//	public void EnableLights_FloorNum(int floor_num, int tilemain_X, int tilemain_Y, bool on, Environment_Light EL, int light_num)
//	{
//		var tilegraphics = GetFloor(floor_num).GetTileMain(tilemain_X, tilemain_Y).TileGraphics;
//
//		//as long as there are TileGraphics and TileLights
//		if(tilegraphics != null)
//		{
//			if(tilegraphics.TileLights != null)
//			{
//				switch(EL)
//				{
//				case Environment_Light.WhiteLight:
//					tilegraphics.TileLights.EnableLights(on, Environment_Light.WhiteLight, light_num);
//					break;
//				case Environment_Light.OrangeLight:
//					tilegraphics.TileLights.EnableLights(on, Environment_Light.OrangeLight, light_num);
//					break;
//				default:
//					Debug.Log("PASS IN WhiteLight OR OrangeLight ONLY");
//					Debug.Break();
//					break;
//				}
//			}
//		}
//	}

//	//function to enable a specific light of a specific type or all lights in the environment for a specific floor
//	public void EnableLights_FloorNum(int floor_num, bool on, Environment_Light EL, int light_num)
//	{
//		//as long as there are TileMainMaps
//		if(GetFloor(floor_num).TileMainMap != null)
//		{
//			//traverse through all the TileMainMaps
//			for(int x = 0; x < GetFloor(floor_num).TileMainMap.GetLength(0); x++)
//			{
//				for(int y = 0; y < GetFloor(floor_num).TileMainMap.GetLength(1); y++)
//				{
//					EnableLights_FloorNum(floor_num, x, y, on, EL, light_num);
//				}
//			}
//		}
//	}
	
//	//function to enable a specific light of a specific type or all lights in the environment for all floors
//	public void EnableLights_AllFloors(bool on, Environment_Light EL, int light_num)
//	{
//		//as long as there are Floors
//		if(Floors != null)
//		{
//			//traverse through all the floors
//			for(int floornum = 0; floornum < Floors.Count; floornum++)
//			{
//				EnableLights_FloorNum(floornum, on, EL, light_num);
//			}
//		}
//	}



	
	
//	//function to enable a specific light of a specific type or all lights in a specific TileMain in the environment for a specific floor
//	//switch case handles the types of lights to enable
//	public void EnableLights_FloorNum(int floor_num, int tilemain_X, int tilemain_Y, float power, Environment_Light EL, int light_num)
//	{
//		var tilegraphics = GetFloor(floor_num).GetTileMain(tilemain_X, tilemain_Y).TileGraphics;
//		
//		//as long as there are TileGraphics and TileLights
//		if(tilegraphics != null)
//		{
//			if(tilegraphics.TileLights != null)
//			{
//				switch(EL)
//				{
//				case Environment_Light.WhiteLight:
//					tilegraphics.TileLights.white_lights[light_num].intensity = power;
//					break;
//				case Environment_Light.OrangeLight:
//					tilegraphics.TileLights.orange_lights[light_num].intensity = power;
//					break;
//				default:
//					Debug.Log("PASS IN WhiteLight OR OrangeLight ONLY");
//					Debug.Break();
//					break;
//				}
//			}
//		}
//	}

//	//function to enable a specific light of a specific type or all lights in the environment for a specific floor
//	public void EnableLights_FloorNum(int floor_num, float power, Environment_Light EL, int light_num)
//	{
//		//as long as there are TileMainMaps
//		if(GetFloor(floor_num).TileMainMap != null)
//		{
//			//traverse through all the TileMainMaps
//			for(int x = 0; x < GetFloor(floor_num).TileMainMap.GetLength(0); x++)
//			{
//				for(int y = 0; y < GetFloor(floor_num).TileMainMap.GetLength(1); y++)
//				{
//					EnableLights_FloorNum(floor_num, x, y, power, EL, light_num);
//				}
//			}
//		}
//	}
	
//	//function to enable a specific light of a specific type or all lights in the environment for all floors
//	public void EnableLights_AllFloors(float power, Environment_Light EL, int light_num)
//	{
//		//as long as there are Floors
//		if(Floors != null)
//		{
//			//traverse through all the floors
//			for(int floornum = 0; floornum < Floors.Count; floornum++)
//			{
//				EnableLights_FloorNum(floornum, power, EL, light_num);
//			}
//		}
//	}





//	//function to flicker all lights of a specific type or all lights in a specific TileMain in the environment for a specific floor
//	public void Flicker_FloorNum(int floor_num, int tilemain_X, int tilemain_Y, float delay, bool on, Environment_Light EL)
//	{
//		//as long as there are TileGraphs, TileLights and TileLights' light_flicker has been enabled
//		if(GetFloor(floor_num).GetTileMain(tilemain_X, tilemain_Y).TileGraphics != null)
//		{
//			if(GetFloor(floor_num).GetTileMain(tilemain_X, tilemain_Y).TileGraphics.TileLights != null)
//			{
//				if(GetFloor(floor_num).GetTileMain(tilemain_X, tilemain_Y).TileGraphics.TileLights.GetFlicker())
//				{
//					GetFloor(floor_num).GetTileMain(tilemain_X, tilemain_Y).TileGraphics.TileLights.Flicker(delay, on, EL);
//				}
//			}
//		}
//	}
//
//	//function to flicker a specific light of a specific type or all lights in a specific TileMain in the environment for a specific floor
//	public void Flicker_FloorNum(int floor_num, int tilemain_X, int tilemain_Y, float delay, bool on, Environment_Light EL, int light_num)
//	{
//		//as long as there are TileGraphs, TileLights and TileLights' light_flicker has been enabled
//		if(GetFloor(floor_num).GetTileMain(tilemain_X, tilemain_Y).TileGraphics != null)
//		{
//			if(GetFloor(floor_num).GetTileMain(tilemain_X, tilemain_Y).TileGraphics.TileLights != null)
//			{
//				if(GetFloor(floor_num).GetTileMain(tilemain_X, tilemain_Y).TileGraphics.TileLights.GetFlicker())
//				{
//					GetFloor(floor_num).GetTileMain(tilemain_X, tilemain_Y).TileGraphics.TileLights.Flicker(delay, on, EL, light_num);
//				}
//			}
//		}
//	}
//
//	//function to flicker all lights of a specific type or all lights in the environment for a specific floor
//	public void Flicker_FloorNum(int floor_num, float delay, bool on, Environment_Light EL)
//	{
//		//as long as there are TileMainMaps
//		if(GetFloor(floor_num).TileMainMap != null)
//		{
//			//traverse through all the TileMainMaps
//			for(int x = 0; x < GetFloor(floor_num).TileMainMap.GetLength(0); x++)
//			{
//				for(int y = 0; y < GetFloor(floor_num).TileMainMap.GetLength(1); y++)
//				{
//					Flicker_FloorNum(floor_num, x, y, delay, on, EL);
//				}
//			}
//		}
//	}
//
//	//function to flicker a specific light of a specific type or all lights in the environment for a specific floor
//	public void Flicker_FloorNum(int floor_num, float delay, bool on, Environment_Light EL, int light_num)
//	{
//		//as long as there are TileMainMaps
//		if(GetFloor(floor_num).TileMainMap != null)
//		{
//			//traverse through all the TileMainMaps
//			for(int x = 0; x < GetFloor(floor_num).TileMainMap.GetLength(0); x++)
//			{
//				for(int y = 0; y < GetFloor(floor_num).TileMainMap.GetLength(1); y++)
//				{
//					Flicker_FloorNum(floor_num, x, y, delay, on, EL, light_num);
//				}
//			}
//		}
//	}
//
//	//function to flicker all lights of a specific type or all lights in the environment for all floors
//	public void Flicker_AllFloors(float delay, bool on, Environment_Light EL)
//	{
//		//as long as there are Floors
//		if(Floors != null)
//		{
//			//traverse through all the floors
//			for(int floornum = 0; floornum < Floors.Count; floornum++)
//			{
//				Flicker_FloorNum(floornum, delay, on, EL);
//			}
//		}
//	}
//
//	//function to flicker a specific light of a specific type or all lights in the environment for all floors
//	public void Flicker_AllFloors(float delay, bool on, Environment_Light EL, int light_num)
//	{
//		//as long as there are Floors
//		if(Floors != null)
//		{
//			//traverse through all the floors
//			for(int floornum = 0; floornum < Floors.Count; floornum++)
//			{
//				Flicker_FloorNum(floornum, delay, on, EL, light_num);
//			}
//		}
//	}
//
//	//function to set flicker for a specific TileMain on a specific floor
//	public void SetFlicker_FloorNum(int floor_num, int tilemain_X, int tilemain_Y, bool flicker)
//	{
//		//as long as there are TileGraphs, TileLights
//		if(GetFloor(floor_num).GetTileMain(tilemain_X, tilemain_Y).TileGraphics != null)
//		{
//			if(GetFloor(floor_num).GetTileMain(tilemain_X, tilemain_Y).TileGraphics.TileLights != null)
//			{
//				GetFloor(floor_num).GetTileMain(tilemain_X, tilemain_Y).TileGraphics.TileLights.SetFlicker(flicker);
//			}
//		}
//	}
//
//	//function to set flicker for all TileMains on a specific floor
//	public void SetFlicker_FloorNum(int floor_num, bool flicker)
//	{
//		//as long as there are TileMainMaps
//		if(GetFloor(floor_num).TileMainMap != null)
//		{
//			//traverse through all the TileMainMaps
//			for(int x = 0; x < GetFloor(floor_num).TileMainMap.GetLength(0); x++)
//			{
//				for(int y = 0; y < GetFloor(floor_num).TileMainMap.GetLength(1); y++)
//				{
//					SetFlicker_FloorNum(floor_num, x, y, flicker);
//				}
//			}
//		}
//	}
//
//	//function to set flicker for all TileMains on all floors
//	public void SetFlicker_AllFloors(bool flicker)
//	{
//		//as long as there are Floors
//		if(Floors != null)
//		{
//			//traverse through all the floors
//			for(int floornum = 0; floornum < Floors.Count; floornum++)
//			{
//				SetFlicker_FloorNum(floornum, flicker);
//			}
//		}
//	}
//
//	//function to set delay for a specific TileMain on a specific floor
//	public void SetDelay_FloorNum(int floor_num, int tilemain_X, int tilemain_Y, float delay)
//	{
//		//as long as there are TileGraphs, TileLights
//		if(GetFloor(floor_num).GetTileMain(tilemain_X, tilemain_Y).TileGraphics != null)
//		{
//			if(GetFloor(floor_num).GetTileMain(tilemain_X, tilemain_Y).TileGraphics.TileLights != null)
//			{
//				GetFloor(floor_num).GetTileMain(tilemain_X, tilemain_Y).TileGraphics.TileLights.SetDelay(delay);
//			}
//		}
//	}
//
//	//function to set delay for all TileMains on a specific floor
//	public void SetDelay_FloorNum(int floor_num, float delay)
//	{
//		//as long as there are TileMainMaps
//		if(GetFloor(floor_num).TileMainMap != null)
//		{
//			//traverse through all the TileMainMaps
//			for(int x = 0; x < GetFloor(floor_num).TileMainMap.GetLength(0); x++)
//			{
//				for(int y = 0; y < GetFloor(floor_num).TileMainMap.GetLength(1); y++)
//				{
//					SetDelay_FloorNum(floor_num, x, y, delay);
//				}
//			}
//		}
//	}
//
//	//function to set delay for all TileMains on all floors
//	public void SetDelay_AllFloors(float delay)
//	{
//		//as long as there are Floors
//		if(Floors != null)
//		{
//			//traverse through all the floors
//			for(int floornum = 0; floornum < Floors.Count; floornum++)
//			{
//				SetDelay_FloorNum(floornum, delay);
//			}
//		}
//	}
//
//	//function to get the enabled state of a specific light of a specific type of a specific TileMain on a specific floor
//	public bool GetEnabledLightsState_FloorNum(int floor_num, int tilemain_X, int tilemain_Y, Environment_Light EL,  int light_num)
//	{
//		//as long as there are TileGraphs, TileLights
//		if(GetFloor(floor_num).GetTileMain(tilemain_X, tilemain_Y).TileGraphics != null)
//		{
//			if(GetFloor(floor_num).GetTileMain(tilemain_X, tilemain_Y).TileGraphics.TileLights != null)
//			{
//				return GetFloor(floor_num).GetTileMain(tilemain_X, tilemain_Y).TileGraphics.TileLights.GetEnableLightsState(EL, light_num);
//			}
//			else
//			{
//				return false;
//			}
//		}
//		else
//		{
//			return false;
//		}
//	}
}
