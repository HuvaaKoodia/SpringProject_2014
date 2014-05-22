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

	public System.Action OnPlayerTurnStart;

	public string TestLoadShipName;
	public bool UseTestMap,OverrideMissionShip=false,randomizeDoorStates=true;
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

	public MasterHudMain HUD;
	public InGameInfoPanelMain Inventory;

    public PlayerMain Player;
	public bool do_culling=false;

	public HaxKnifeCulling culling_system;

	public List<GameObject> FloorContainers{get;private set;}

	void Awake(){
		
		SS=SharedSystemsMain.I;

		Floors=new List<FloorObjData>();
		FloorContainers=new List<GameObject>();
	}

	ShipObjData current_ship_data;

	// Use this for initialization
	void Start()
    {
		//Dev.debug
		if (!SS.GDB.GameStarted) SS.GDB.CreateNewGame();

		#if !UNITY_EDITOR
		OverrideMissionShip=false;
		UseTestMap=false;
		#endif
		aiController = new AIcontroller(this);
		ShipObjData ship_objdata=null;
        //DEV.DEBUG generate mission
        if (SS.GDB.GameData.CurrentMission==null){
			SS.GDB.GameData.CurrentMission=MissionGenerator.GenerateMission(Subs.GetRandom(XmlDatabase.MissionPool.Pools.Keys));
        }
		HUD.MissionBriefing.SetMission(SS.GDB.GameData.CurrentMission);

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

		Debug.Log("Loading ship "+ship_objdata.XmlData.Name);

		for (int i=0;i<ship_objdata.Floors.Count;++i){
			var floor=new FloorObjData();
			floor.FloorIndex=i;
			Floors.Add(floor);
			SS.MGen.GenerateObjectDataMap(floor,ship_objdata.Floors[i]);
			SS.SDGen.GenerateShipItems(floor,ship_objdata,SS.GDB.GameData.CurrentMission);
			SS.MGen.GenerateSceneMap(this,floor);
			var cmis=SS.GDB.GameData.CurrentMission;
			var xml=cmis.XmlData;
			SS.SDGen.GenerateLoot(floor,xml.LootPool,cmis.LootQuality);
			Debug.Log("Floor: "+i+" loaded");
		}

		SS.SDGen.GenerateMissionObjectives(this,SS.GDB.GameData.CurrentMission,ship_objdata);

		//minimap data
		MiniMapData = new MiniMapData();
		MiniMapData.Init(this);

		//init culling
		culling_system.Init(this);

		//create player
		var legit_floors=new List<FloorObjData>();
		foreach(var f in Floors){
			if (f.AirlockPositions.Count>0){
				legit_floors.Add(f);
			}
		}

		if (legit_floors.Count==0){
			Debug.LogError("The ship "+ship_objdata.XmlData.Name+" has no floors with airlocks.");
		}

		//init player

		var start_floor=Subs.GetRandom(legit_floors);
		//DEv.temp
		//start_floor=Floors[0];
		SS.MGen.InitPlayer(Player, start_floor);
		Player.SetObjData(SS.GDB.GameData.PlayerData);
		Player.CurrentFloorIndex=start_floor.FloorIndex;
		Player.GC=this;

		Debug.Log("StartFloor: "+start_floor.FloorIndex);

		Player.InitPlayer();
		Player.HUD.CheckTargetingModePanel();

		Inventory.SetPlayer(Player);

        Player.ActivateEquippedItems();

		//init hud
		HUD.player = Player;
		HUD.SetGC(this);

		RandomizeLightsInAllFloors(XmlDatabase.LightsBrokenPercent,XmlDatabase.LightsFlickerPercent);

		//set ship status to mission status
		var c_mis=SS.GDB.GameData.CurrentMission;
		var power=c_mis.MissionShipPower==MissionObjData.ShipPower.On;

		if (c_mis.MissionShipPower==MissionObjData.ShipPower.Broken); //DEV.todo break generator

		if (randomizeDoorStates)SS.SDGen.RandomizeDoorStates(this,ship_objdata);

		Debug.Log("Power: "+power);

		SetFloorsPowerState(power);

		//misc
		var ec=GetComponent<EngineController>();
		ec.AfterRestart+=SS.GDB.CreateNewGame;

		//debug
		current_ship_data=ship_objdata;

		//floor stats
		SetFloor(CurrentFloorIndex);

		HUD.SetAlpha(1f);
		HUD.FadeOut(0.5f);

		SS.GDB.AllowEscHud=true;

		SS.MusicSystem.StartGameTrack();
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
		if (Input.GetKeyDown(KeyCode.B)){
			SS.SDGen.RandomizeDoorStates(this,current_ship_data);
		}

		if (Input.GetKeyDown(KeyCode.C)){
			do_culling=!do_culling;
			if (do_culling)
				CullWorldBasedOnPlayer(true);
			else
				culling_system.ResetCulling(this);
		}

		if (Input.GetKeyDown(KeyCode.V)){
			CullWorldBasedOnPlayer(true);
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
		Player.HUD.turnText.gameObject.SetActive(currentTurn != TurnState.PlayerTurn);
    }

	void StartPlayerTurn()
	{
		if (OnPlayerTurnStart!=null) OnPlayerTurnStart();
		Player.StartPlayerPhase();
	}
	
	public void CullWorldBasedOnPlayer(bool StopAtDoors){
		if (do_culling) culling_system.CullBasedOnPositions(Player.transform.position,Player.movement.targetPosition,Player.GameCamera.farClipPlane,this, StopAtDoors,Player.HasMap);
	}

	public FloorObjData GetFloor (int index)
	{
		return Floors[index];
	}

	public void GotoFloor(int index){
		if (index<0||index>Floors.Count-1) return;
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
		aiController.SetFloor(CurrentFloorData);
		Player.interactSub.CheckForInteractables();
		Player.HUD.ChangeFloor(index);

		StartCoroutine (CallAfterAFewUpdateSteps());
	}

	IEnumerator CallAfterAFewUpdateSteps(){
		yield return null;
		yield return null;
		yield return null;

		culling_system.DisableOtherFloors(CurrentFloorIndex,this);
		CullWorldBasedOnPlayer (true);
	}

	private IEnumerator GotoFloorTimer(int index){
		Player.inputSub.DISABLE_INPUT=true;
		HUD.FadeIn();
		while(HUD.FadeInProgress){
			yield return null;
		}
		yield return new WaitForSeconds(2f);
		//dev. elevator sound
		HUD.FadeOut();
		SetFloor(index);
		//DEV. elevator sound here + some delay

		yield return null;//DEV. haxy hax Thanks to having to call UpdateFloorStats one step after setActive

		UpdateFloorStats(index);

		while(HUD.FadeInProgress){
			yield return null;
		}

		Player.inputSub.DISABLE_INPUT=false;
		Player.movement.Init();
	}

	void UpdateFloorStats (int index)
	{
		//update tiles based on floor stats
		SetFloorPowerState(index,CurrentFloorData.PowerOn);//DEV. lazy
		CullWorldBasedOnPlayer(true);
	}

	public void UseElevator ()
	{
		//Dev.Mega HAX!
		if (CurrentFloorIndex==0)
			GotoFloor(CurrentFloorIndex+1);
		else
			GotoFloor(CurrentFloorIndex-1);
	}

	//FUNCTIONS TO SET STATE OF THE WHITE LIGHTS

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

	public void EndMission ()
	{
		HUD.FadeIn(0.5f);
		StartCoroutine(EndMissionAfterFade());
	}

	IEnumerator EndMissionAfterFade(){
		while(HUD.FadeInProgress) yield return null;
		SS.GDB.EndMission(this);
	}

	public void EndGame ()
	{
		SS.GDB.RemoveSavesIfIronman();
		HUD.FadeIn(0.3f);
		StartCoroutine(EndGameAfterFade());
	}

	IEnumerator EndGameAfterFade(){
		while(HUD.FadeInProgress) yield return null;
		HUD.ShowGameoverPanel();
	}
}
