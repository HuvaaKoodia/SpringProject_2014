using UnityEngine;
using System.Collections;

public class GatlingAI : AIBase {

	public const int APmax = 3;
	public const int MovementCost = 1;
	public const int AttackCost = 2;
	
	public int Damage = 20;
	
	public const int PlayerHearRadiusUnaware = 10;
	public const int PlayerHearRadiusAware = 18;
	
	public const int PlayerSeeRadiusUnaware = 20;
	public const int PlayerSeeRadiusAware = 30;
	
	public const int FleeHealth = 30;
	
	public int numPhasesWaited = 0;
	public bool NeedsPathfinding = true;
	public bool test = false;
	
	public float communicationRange = 3;
	
	bool readyToAttack = false;
	
	Point3D MyPosition;

	// Use this for initialization
	void Start()
	{
		parent = gameObject.GetComponent<EnemyMain>();
		tilemap = parent.GC.TileMainMap;
		
		//movement = parent.movement;
		
		blackboard.owner = this;
		player = parent.GC.Player;
		
		HasUsedTurn = false;
		foundMove = false;
		
		AP = APmax;
		
		CreateBehaviourTree();
		//behaviourTree.Start (null);
	}

	public override void PlayAiTurn(){}
	
	public override void Reset(){}

	protected override void CreateBehaviourTree()
	{
	}
}
