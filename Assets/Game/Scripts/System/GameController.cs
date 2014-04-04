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

	public bool PowerOn=true;

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
			return Player.CurrentFloorIndex;
		}
	}
	public FloorObjData CurrentFloorData{get{
			return Floors[CurrentFloorIndex];
		}
	}

	public List<FloorObjData> Floors{get;set;}

	public MiniMapData MiniMapData { get; private set; }

	public AIcontroller aiController;
	public TurnState currentTurn = TurnState.StartPlayerTurn;

	public MenuHandler menuHandler;
	public InventoryMain Inventory;

    public PlayerMain Player;
	public bool do_culling=false;

	public HaxKnifeCulling culling_system;
	public MeshCombiner MeshCombi;

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
			SS.GDB.GameData.CurrentMission=MissionGenerator.GenerateMission(Subs.GetRandom(XmlDatabase.MissionPool.Pools.Keys));
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
			var xml=SS.GDB.GameData.CurrentMission.XmlData;
			SS.SDGen.GenerateLoot(floor,xml.LootPool,xml.LootQuality);
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

		//minimap data
		MiniMapData = new MiniMapData();
		MiniMapData.Init(this);

		//init player

		var start_floor=Subs.GetRandom(legit_floors);
		SS.MGen.InitPlayer(Player, start_floor);
		Player.SetObjData(SS.GDB.GameData.PlayerData);
		Player.CurrentFloorIndex=start_floor.FloorIndex;
		Player.GC=this;

		Player.InitPlayer();
		Player.HUD.CheckTargetingModePanel();

		Inventory.SetPlayer(Player);

        Player.ActivateEquippedItems();


		//init hud
		menuHandler.player = Player;
		menuHandler.SetGC(this);

		//floor stats
		SetFloor(CurrentFloorIndex);

		RandomizeLightsInAllFloors(10,20);

		//set ship status to mission status
		var c_mis=SS.GDB.GameData.CurrentMission;
		var power=c_mis.MissionShipPower==MissionObjData.ShipPower.On;

		if (c_mis.MissionShipPower==MissionObjData.ShipPower.Broken); //DEV.todo break generator

		Debug.Log("Power: "+power);

		SetFloorsPowerState(power);

		//misc
		var ec=GetComponent<EngineController>();
		ec.AfterRestart+=SS.GDB.StartNewGame;
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
		Player.HUD.turnText.gameObject.SetActive(currentTurn == TurnState.PlayerTurn);
    }

	void StartPlayerTurn()
	{
		Player.StartPlayerPhase();
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
	/// <summary>
	/// Sets the floor.
	/// </summary>
	/// <param name="index">Index.</param>
	public void SetFloor(int index){                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
		index=Mathf.Max(0,Mathf.Min(Floors.Count-1,index));

		SetFloorPowerState(index,CurrentFloorData.PowerOn);//DEV. lazy

		Player.CurrentFloorIndex=index;
		culling_system.DisableOtherFloors(index,this);
		aiController.SetFloor(CurrentFloorData);
		Player.interactSub.CheckForInteractables();
		Player.HUD.ChangeFloor(index);
	}

	private IEnumerator GotoFloorTimer(int index){
		while(menuHandler.FadeInProgress){
			yield return null;
		}
		menuHandler.FadeOut();
		SetFloor(index);
		//DEV. elevator sound here + some delay
		yield return null;

		UpdateFloorStats(index);//DEV. haxy hax Thanks to having to be called one step after setActive 
	}

	void UpdateFloorStats (int index)
	{
		//update tiles based on floor stats
		SetFloorPowerState(index,CurrentFloorData.PowerOn);//DEV. lazy
	}

	public void UseElevator ()
	{
		//Dev.Mega HAX!
		if (CurrentFloorIndex==0)
			GotoFloor(CurrentFloorIndex+1);
		else
			GotoFloor(CurrentFloorIndex-1);
	}

	//FUNCTIONS TO ENABLE LIGHTS

	//function to enable the white lights in a specific TileMain in the environment for a specific floor
	//intensity of the white lights is passed in here
	public void EnableLights_FloorNum(int floor_num, int tilemain_X, int tilemain_Y,bool on)
	{
		var tilegraphics = GetFloor(floor_num).GetTileMain(tilemain_X, tilemain_Y).TileGraphics;
		
		//as long as there are TileGraphics and TileLights
		if(tilegraphics != null)
		{
			if(tilegraphics.TileLights != null)
			{
				//set electricity to flow for the white light in particular TileMainMap in particular floor
				//tilegraphics.TileLights.SetPowerOn(on);
				tilegraphics.TileLights.PowerOn = on;
			}
		}
	}

	//function to enable all white lights in the environment for a specific floor
	//intensity of the white lights is passed in here
	public void EnableLights_FloorNum(int floor_num, float power, bool on)
	{
		//as long as there are TileMainMaps
		if(GetFloor(floor_num).TileMainMap != null)
		{
			//traverse through all the TileMainMaps
			for(int x = 0; x < GetFloor(floor_num).TileMainMap.GetLength(0); x++)
			{
				for(int y = 0; y < GetFloor(floor_num).TileMainMap.GetLength(1); y++)
				{
					//enable lights in the particular TieMainMap in particular floor
					EnableLights_FloorNum(floor_num, x, y, on);
				}
			}
		}
	}

	//function to enable all white lights in the environment for all floors
	//intensity of the white light is passed in here
	public void EnableLights_AllFloors(float power, bool on)
	{
		//as long as there are Floors
		if(Floors != null)
		{
			//traverse through all the floors
			for(int floornum = 0; floornum < Floors.Count; floornum++)
			{
				//enable lights in the particular floor
				EnableLights_FloorNum(floornum, power, on);
			}
		}
	}
	//FUNCTIONS TOP SET STATE OF THE WHITE LIGHTS

	//function to set the state of the white lights in a specific TileMain in aspecific floor
	public void SetState_FloorNum(int floor_num, int tilemain_X, int tilemain_Y, Lighting_State LS)
	{
		SetTileLightState(GetFloor(floor_num).GetTileMain(tilemain_X, tilemain_Y),LS,true);
	}

	private void SetTileLightState(TileMain tile, Lighting_State LS,bool power){
		var tilegraphics = tile.TileGraphics;
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
		if(tilegraphics != null&&tilegraphics.TileLights != null)
		{
			tilegraphics.TileLights.PowerOn=power;
			tilegraphics.TileLights.LightingState = LS;
		}
	}

	private void SetTileLightState(TileMain tile, bool power){
		var tilegraphics = tile.TileGraphics;
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
		if(tilegraphics != null&&tilegraphics.TileLights != null)
		{
			tilegraphics.TileLights.PowerOn=power;
		}
	}

	public void SetFloorPowerState(int i,bool on){
		var f=GetFloor(i);
		f.PowerOn=on;
		foreach(var t in f.TileMainMap){
			SetTileLightState(t,on);
		}
	}

	public void SetFloorsPowerState(bool on){
		for(var f=0;f<Floors.Count;++f){
			SetFloorPowerState(f,on);
		}
	}
	//function to randomize the state of the white lights in the environment
	//percentages passed in represents the chances of getting each state
	//like a dice roll
	public Lighting_State RandomizeStartState(int broken_percent, int flickering_percent)
	{
		//as long as the percentages do not add up to 100, display error log and set state to normal
		int totalpercent = broken_percent + flickering_percent;
		
		if(totalpercent >= 100)
		{
			Debug.LogError("Percentages passed are over 100%");
			return Lighting_State.Normal;
		}
		
		int value = Subs.RandomPercent();
		
		if(value < broken_percent)
		{
			return Lighting_State.Broken;
		}
		else if(value < broken_percent+flickering_percent)
		{
			return Lighting_State.Flickering;
		}
		else
		{
			return Lighting_State.Normal;
		}
	}

	void RandomizeLightsInAllFloors(int broken_percent, int flickering_percent)
	{
		for (int i=0;i<Floors.Count;++i){
			var floor=GetFloor(i);
			foreach(var tile in floor.TileMainMap){
				SetTileLightState(tile,RandomizeStartState(broken_percent,flickering_percent),floor.PowerOn);
			}
		}
	}
}
