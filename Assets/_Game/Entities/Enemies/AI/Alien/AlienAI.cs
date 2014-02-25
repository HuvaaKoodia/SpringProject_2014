using UnityEngine;
using System.Collections;

using TreeSharp;

public class AlienAI : AIBase {

	EnemyMain owner;
	EntityMovementSub movement;

	public AlienLookupTable blackboard;

	public bool HasUsedTurn;
	public bool foundMove;
	public bool waitedForOthersToMoveThisTurn;

	public const int APmax = 3;
	public const int MovementCost = 1;
	public const int AttackCost = 2;
	public int AP;

	public int Damage = 20;

	// Use this for initialization
	void Start()
	{
		parent = gameObject.GetComponent<EnemyMain>();
		movement = parent.movement;

		blackboard.owner = this;
		player = parent.GC.Player;

		HasUsedTurn = false;
		foundMove = false;
		waitedForOthersToMoveThisTurn = false;
		
		AP = APmax;

		CreateBehaviourTree();
	}
	
	// Update is called once per frame
	void Update()
	{
		behaviourTree.Tick(blackboard);

		if (behaviourTree.LastStatus != RunStatus.Running)
		{
			behaviourTree.Stop(blackboard);
			behaviourTree.Start(blackboard);
		}
	}

	private void MoveToNextTile()
	{
		if (blackboard.Path == null || blackboard.Path.next == null)
		{
			HasUsedTurn = true;
			parent.FinishedMoving(true);
			return;
		}
		
		MovementState movedSuccessfully = movement.MoveToTile(blackboard.Path.next.position, true);
		
		if (movedSuccessfully == MovementState.Moving)
		{
			HasUsedTurn = true;
			foundMove = true;
			AP -= MovementCost;
			blackboard.Path = blackboard.Path.next;
		}
		else if (movedSuccessfully == MovementState.Turning)
		{
			HasUsedTurn = true;
			foundMove = true;
			AP -= MovementCost;
		}
		else
		{
			//waitedForOthersToMoveThisTurn = true;
			EntityMain entityBlocking = tilemap[blackboard.Path.next.position.X, blackboard.Path.next.position.Y].entityOnTile;
			//Debug.Log(entityBlocking.tag + " is blocking " + path.next.position);
			if (entityBlocking != null)
			{
				//vähän rikki niin kommenteissa, korjataan kun tärkeämpää asiaa saatu alta pois
				//if (!waitedForOthersToMoveThisTurn)
				//{
				if (entityBlocking.tag == "Player")
				{
					HasUsedTurn = true;
					parent.FinishedMoving(true);
				}
				/*}
                else
                {
                    HasUsedTurn = true;
                }*/
			}
		}
	}

	private RunStatus FindPathToLastKnownPlayerPosition()
	{
		blackboard.LatestPathType = AlienAiState.Chase;

		Point3D currentPos = new Point3D(movement.currentGridX, movement.currentGridY);
		Point3D playerPos = new Point3D(player.movement.currentGridX, player.movement.currentGridY);
		
		if (playerPos != blackboard.Destination)
			blackboard.Path = PathFinder.FindPath(tilemap, currentPos, playerPos, -1);
		
		if (blackboard.Path != null)
		{
			blackboard.Destination = playerPos;
			return RunStatus.Success;
		}
		else
		{
			blackboard.Destination = null;
			return RunStatus.Failure;
		}
	}

	protected override void CreateBehaviourTree()
	{
	}
}
