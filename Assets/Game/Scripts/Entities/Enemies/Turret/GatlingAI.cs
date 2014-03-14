using UnityEngine;
using TreeSharp;
using System.Collections;

public class GatlingAI : AIBase {

	EntityMovementSub movement;

	public const int APmax = 2;
	public const int AppearCost = 1;
	public const int AttackCost = 2;
	
	public int Damage = 20;

	public const int PlayerSeeRadius = 16;
	public LayerMask PlayerSeeMask;

	Point3D MyPosition;

	bool open;

	public Animation baseAnimation;
	public Animation turretAnimation;

	public string appearAnimation;
	public string hideAnimation;

	public string shootAnimation;

	// Use this for initialization
	void Start()
	{
		parent = gameObject.GetComponent<EnemyMain>();
		tilemap = parent.GC.TileMainMap;
		
		movement = parent.movement;
		
		blackboard.owner = this;
		player = parent.GC.Player;
		
		HasUsedTurn = false;
		foundMove = false;

		AP = APmax;

		open = false;

		MyPosition = new Point3D(movement.currentGridX, movement.currentGridY);
		
		CreateBehaviourTree();
		//behaviourTree.Start (null);
	}

	public override void PlayAiTurn()
	{
		/*
		behaviourTree.Tick(blackboard);
		
		if (behaviourTree.LastStatus != RunStatus.Running)
		{
			behaviourTree.Stop(blackboard);
			behaviourTree.Start(blackboard);
		}
*/
		if (open)
		{
			Quaternion lookToPlayerRot = Quaternion.LookRotation(player.transform.position - parent.transform.position);
			movement.Turn((int)lookToPlayerRot.eulerAngles.y - 90);
		}
	}
	
	public override void Reset()
	{
		AP = APmax;
		HasUsedTurn = false;
		foundMove = false;
	}

	private RunStatus CheckForPlayerPresence()
	{
		if (PathFinder.CanSeeFromTileToTile(player, parent, MyPosition, PlayerSeeRadius, PlayerSeeMask))
		{
			blackboard.AwareOfPlayer = true;
			blackboard.TurnsWithoutPlayerAwareness = 0;
			blackboard.LastKnownPlayerPosition = new Point3D(player.movement.currentGridX, player.movement.currentGridY);

			return RunStatus.Success;
		}

		blackboard.AwareOfPlayer = false;
		blackboard.TurnsWithoutPlayerAwareness++;
		blackboard.Berserk = false;
		return RunStatus.Failure;
	}

	void Open()
	{
		if (!open)
		{
			open = true;
			
			baseAnimation[appearAnimation].normalizedTime = 0;
			baseAnimation[appearAnimation].speed = 1;
			baseAnimation.Play(appearAnimation);
			
			turretAnimation[appearAnimation].normalizedTime = 0;
			turretAnimation[appearAnimation].speed = 1;
			turretAnimation.Play(appearAnimation);

			AP -= AppearCost;
		}
	}

	void Close()
	{
		if (open)
		{
			open = false;
			
			baseAnimation[appearAnimation].normalizedTime = 1;
			baseAnimation[appearAnimation].speed = -1;
			baseAnimation.Play(appearAnimation);
			
			turretAnimation[appearAnimation].normalizedTime = 1;
			turretAnimation[appearAnimation].speed = -1;
			turretAnimation.Play(appearAnimation);
		}
		
		AP = 0;
	}

	void Shoot()
	{

	}

	protected override void CreateBehaviourTree()
	{/*
		Action CheckPlayerPresenceAction = new Action(action => CheckForPlayerPresence());

		Action CheckIfOpenAction = new Action(action => open == true);
		Action ShootAction = new Action(action => Shoot());

		Action OpenAction = new Action(action => Open);

		Sequence PlayerPresentSequence = new Sequence(CheckIfOpenAction, ShootAction);
		PrioritySelector PlayerPresentSelector = new PrioritySelector(PlayerPresentSequence, OpenAction);

		Sequence PlayerPresentCheck = new Sequence(CheckPlayerPresenceAction, PlayerPresentSelector);


		//Sequence playerPresentSequence = new Sequence
		behaviourTree = new Action(action => CheckForPlayerPresence());*/
	}

	RunStatus SuccessReturner()
	{
		return RunStatus.Success;
	}
}
