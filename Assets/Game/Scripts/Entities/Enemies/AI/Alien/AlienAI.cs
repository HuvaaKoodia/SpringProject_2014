﻿using UnityEngine;
using System.Collections;

using TreeSharp;

public class AlienAI : AIBase {
	EntityMovementSub movement;

	public AlienLookupTable blackboard;
	
	public const int APmax = 3;
	public const int MovementCost = 1;
	public const int AttackCost = 2;

	public int Damage = 20;

	public LayerMask PlayerHearMask;
	public LayerMask PlayerSeeMask;

	public const int PlayerHearRadiusUnaware = 10;
	public const int PlayerHearRadiusAware = 18;

	public const int PlayerSeeRadiusUnaware = 20;
	public const int PlayerSeeRadiusAware = 30;

	public const int FleeHealth = 30;

	public int numPhasesWaited = 0;
	public bool NeedsPathfinding = true;
	public bool test = false;

	bool readyToAttack = false;

	Point3D MyPosition;

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

		CreateBehaviourTree();
		behaviourTree.Start (null);
	}

	public override void Reset()
	{
		AP = APmax;
		HasUsedTurn = false;
		foundMove = false;
		NeedsPathfinding = true;
		readyToAttack = false;
		numPhasesWaited = 0;
	}

	public override void PlayAiTurn()
	{
		MyPosition = new Point3D(movement.currentGridX, movement.currentGridY);

		if ((AP <= AttackCost || readyToAttack) && CanAttack())
		{
			Attack();
		}
		else
		{
			behaviourTree.Tick(blackboard);
			
			if (behaviourTree.LastStatus != RunStatus.Running)
			{
				behaviourTree.Stop(blackboard);
				behaviourTree.Start(blackboard);
			}
		}
	}
	/*
	public override void PlayAttackPhase ()
	{
		if (AP > AttackCost && movement.GetTileInFront().entityOnTile == player)
		{
			AP -= AttackCost;
			player.TakeDamage(Damage, movement.currentGridX, movement.currentGridY);
		}

		//parent.FinishedAttacking();
	}
	*/

	private void MoveToNextTile()
	{
		if (test)
		{
			int i = 0;
			i++;
		}

		if (blackboard.Path == null || blackboard.Path.next == null)
		{
			HasUsedTurn = true;
			AP = 0;
			//parent.FinishedMoving(true);
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

			if (numPhasesWaited > 2)
			{
				HasUsedTurn = true;
				AP = 0;

				if (blackboard.LatestPathType == AlienAiState.Flee)
					blackboard.Berserk = true;

				return;
			}
			else if (numPhasesWaited > 1)
         	{
				NeedsPathfinding = true;
			}

			numPhasesWaited++;
			readyToAttack = true;

			EntityMain entityBlocking = tilemap[blackboard.Path.next.position.X, blackboard.Path.next.position.Y].entityOnTile;
			//Debug.Log(entityBlocking.tag + " is blocking " + path.next.position);
			if (entityBlocking != null)
			{
				//vähän rikki niin kommenteissa, korjataan kun tärkeämpää asiaa saatu alta pois
				//if (!waitedForOthersToMoveThisTurn)
				//{
				if (entityBlocking.tag == "Player")
				{
					readyToAttack = true;
				}
			}
                /*else
                {
                    HasUsedTurn = true;
                }*/
			//}
		}
	}

	private RunStatus FindPathToLastKnownPlayerPosition()
	{
		if (!NeedsPathfinding && blackboard.Path != null)
			return RunStatus.Success;

		NeedsPathfinding = false;
		blackboard.LatestPathType = AlienAiState.Chase;

		Point3D currentPos = MyPosition;
		Point3D playerPos = new Point3D(player.movement.currentGridX, player.movement.currentGridY);
		
		if (playerPos != blackboard.Destination || blackboard.Path.pathCost > 300)
			blackboard.Path = PathFinder.FindPath(tilemap, currentPos, playerPos, -1);
		
		if (blackboard.Path != null)
		{
			blackboard.Destination = playerPos;
			return RunStatus.Success;
		}
		else
		{
			//DEV NÄMÄ POIS, JOSSEI PELAAJAA LÖYDY NIIN TEE JOTAIN MAHD JÄRKEVÄÄ
			AP = 0;
			HasUsedTurn = true;
			//parent.FinishedMoving(true);
			//DEV


			blackboard.Destination = null;
			return RunStatus.Failure;
		}
	}

	private void FindRandomPath()
	{
		if (!NeedsPathfinding && blackboard.Path != null)
			return;

		NeedsPathfinding = false;
		blackboard.LatestPathType = AlienAiState.Rand;

		int nextX = Random.Range(-3, 3) + movement.currentGridX;
		nextX = (int)Mathf.Clamp(nextX, 0, tilemap.GetLength(0)-1);
		
		int nextY = Random.Range(-3, 3) + movement.currentGridY;
		nextY = (int)Mathf.Clamp(nextY, 0, tilemap.GetLength(1)-1);
		
		Point3D destination = new Point3D(nextX, nextY);
		
		blackboard.Path = PathFinder.FindPath(tilemap, MyPosition, destination, 10);
		
		if (blackboard.Path != null)
			blackboard.Destination = destination;
		else
			blackboard.Destination = null;
	}

	private RunStatus CheckForPlayerPresence()
	{
		//Hear
		int checkRadius = blackboard.AwareOfPlayer ? PlayerHearRadiusAware : PlayerHearRadiusUnaware;

		if (PathFinder.CanHearFromTileToTile(player, MyPosition, checkRadius,
		                                     5, PlayerHearMask))
	    {
			blackboard.AwareOfPlayer = true;
			blackboard.TurnsWithoutPlayerAwareness = 0;
			blackboard.LastKnownPlayerPosition = new Point3D(player.movement.currentGridX, player.movement.currentGridY);
			
			return RunStatus.Success;
		}

		//See
		Quaternion myRot = transform.rotation;
		Vector3 playerInRelationToMe =
			player.transform.position - parent.transform.position;
		
		Quaternion playerRelationRotation = Quaternion.LookRotation(playerInRelationToMe);
		
		float angle = Quaternion.Angle(myRot, playerRelationRotation);
		
		if (angle < parent.rangedAngleMax)
		{
			//PathFinder.CanSeeFromTileToTile(player, 
		}
		/*Debug.DrawLine(ray.origin, ray.origin + ray.direction*checkRadius, Color.red, 5.0f);
		if (Physics.Raycast(ray, out hitInfo, checkRadius, PlayerCheckMask))
		{
			if (hitInfo.transform == player.transform)
			{
				blackboard.AwareOfPlayer = true;
				blackboard.TurnsWithoutPlayerAwareness = 0;
				blackboard.LastKnownPlayerPosition = new Point3D(player.movement.currentGridX, player.movement.currentGridY);

				return RunStatus.Success;
			}
		}*/

		blackboard.AwareOfPlayer = false;
		blackboard.TurnsWithoutPlayerAwareness++;
		blackboard.Berserk = false;
		return RunStatus.Failure;
	}

	private RunStatus SearchPlayer()
	{
		if (blackboard.Path != null && blackboard.LastKnownPlayerPosition != null &&
		   	blackboard.Path.position != blackboard.LastKnownPlayerPosition)
		{
			MoveToNextTile();

			if (blackboard.Path.position == blackboard.LastKnownPlayerPosition)
			{
				blackboard.AwareOfPlayer = false;
				blackboard.LastKnownPlayerPosition = null;
			}

			return RunStatus.Success;
		}

		blackboard.AwareOfPlayer = false;
		blackboard.LastKnownPlayerPosition = null;

		return RunStatus.Failure;
	}

	private RunStatus FindPathAwayFromPlayer()
	{
		if (!NeedsPathfinding && blackboard.Path != null)
			return RunStatus.Success;

		Point3D currentPos = MyPosition;;
		blackboard.LatestPathType = AlienAiState.Flee;

		if (blackboard.LastKnownPlayerPosition == null)//how do you run away from something when you don't pos of that something?
			return RunStatus.Failure;

		//if player can't see us, we're OK already. Hearmask because we don't want to hide behind other enemies
		if (!PathFinder.CanSeeFromTileToTile(player, parent, currentPos, PlayerSeeRadiusAware, 
             	PlayerHearMask))
			return RunStatus.Success;

		Point3D playerPos = new Point3D(player.movement.currentGridX, player.movement.currentGridY);

		blackboard.Path = 
			PathFinder.FindPathToSafety(tilemap, currentPos, 
                    	playerPos, parent, player, PlayerSeeRadiusAware, PlayerHearMask);

		if (blackboard.Path != null)
			blackboard.Destination = PathFinder.LatestDestination;
		else
			blackboard.Destination = null;

		//couldn't find path away, go berserk!
		if (blackboard.Path == null && currentPos.GetDistance(playerPos) < 5)
		{
			blackboard.Berserk = true;
			return RunStatus.Failure;
		}

		return RunStatus.Success;
	}

	private void MoveToRandomizedPosition()
	{
		MoveToNextTile();
	}

	bool CanPlayerSee(Point3D position)
	{
		Quaternion playerRot = player.transform.rotation;
		Vector3 posInRelationToPlayer = new Vector3(position.X * MapGenerator.TileSize.x, 
		position.Z * MapGenerator.TileSize.z, 
		position.Y * MapGenerator.TileSize.y)
			- player.transform.position;
	
		Quaternion playerToEnemyRot = Quaternion.LookRotation(posInRelationToPlayer);

		//check that player is facing this general direction
		float angleBetween = Quaternion.Angle(playerRot, playerToEnemyRot);
		if (angleBetween >  50)
			return false;
	
		if (PathFinder.CanSeeFromTileToTile(player, parent, position, PlayerSeeRadiusAware, 
                      PlayerSeeMask))
		{
			return true;
		}

		return false;
	}

	private bool IsHealthLow()
	{
		if (parent.Health < FleeHealth)
			return true;
		else
			return false;
	}

	RunStatus CheckFleeConditions()
	{
		if (IsHealthLow())
			return RunStatus.Success;
		else
			return RunStatus.Failure;
	}

	bool CheckIfBerserk(object context)
	{
		AlienLookupTable bb = context as AlienLookupTable;
		
		if (bb.Berserk)
			return true;
		else
			return false;
	}

	/// <summary>
	/// </summary>
	/// <returns>Success if player sees over 2 tiles on the path, Failure otherwise</returns>
	RunStatus CheckIfPathIsHeadon()
	{
		int tilesPlayerCanSee = 0;
		
		SearchNode current = blackboard.Path;
		int pathLength = 0;

		while (current != null && current.position != blackboard.LastKnownPlayerPosition)
		{
			if (CanPlayerSee(current.position))
				tilesPlayerCanSee++;

			pathLength++;
			current = current.next;
		}
		
		if (tilesPlayerCanSee >= 2)
		{
			if (pathLength < 3 && tilesPlayerCanSee == 2)
				return RunStatus.Failure;
	
				return RunStatus.Success;
		}
	
		return RunStatus.Failure;
	}

	RunStatus CheckIfAlreadyBehindCover()
	{
		if (!CanPlayerSee(MyPosition))
			return RunStatus.Success;
		
		return RunStatus.Failure;
	}

	RunStatus MoveInCover()
	{
		int nextX = blackboard.Path.next.position.X;
		int nextY = blackboard.Path.next.position.Y;

		int twoStepsX = blackboard.Path.next.next.position.X;
		int twoStepsY = blackboard.Path.next.next.position.Y;

		//move if turning needed or player can't see
		if (movement.GetTileInFront() != tilemap[nextX, nextY] || 
		    (!CanPlayerSee(new Point3D(nextX, nextY))))// && !CanPlayerSee(new Point3D(twoStepsX, twoStepsY)))
		{
			MoveToNextTile();
		}

		return RunStatus.Success;
	}

	RunStatus CanHide()
	{
		Point3D currentPos = MyPosition;
		Point3D playerPos = blackboard.LastKnownPlayerPosition;
		
		SearchNode pathToCover = 
			PathFinder.FindPathToSafety(tilemap, currentPos, playerPos, parent, player, PlayerHearRadiusAware, PlayerHearMask);
		
		if (pathToCover != null)
		{
			SearchNode temp = pathToCover;
			
			if (temp == null)
				return RunStatus.Failure;

			//path to safety isn't actually in cover but just further away
			//again hearmask because we don't want to hide behind others
			if (PathFinder.CanSeeFromTileToTile(player, parent, PathFinder.LatestDestination,
			                                    PlayerHearRadiusAware*3,
			                                    PlayerHearMask))
			{
				return RunStatus.Failure;    
			}
			
			blackboard.Path = pathToCover;
			blackboard.Destination = PathFinder.LatestDestination;
			
			return RunStatus.Success;
		}
		
		return RunStatus.Failure;
	}

	bool CanAttack()
	{
		if (AP < AttackCost)
			return false;

		//check if directly looking at player
		Quaternion myRot = transform.rotation;
		Vector3 playerInRelationToMe =
			player.transform.position - parent.transform.position;

		Quaternion playerRelationRotation = Quaternion.LookRotation(playerInRelationToMe);

		float angle = Quaternion.Angle(myRot, playerRelationRotation);

		if (angle < parent.rangedAngleMax)
		{
			float distance = playerInRelationToMe.magnitude / MapGenerator.TileSize.x;

			//check both next tile and ranged if ranged == 0 (which means no ranged <'',) )
			if ((distance < 1.5f || distance <= parent.rangedRange) &&
			    PathFinder.CanSeeFromTileToTile(player, parent, MyPosition, 
                	Mathf.Max(1.5f, parent.rangedRange), PlayerSeeMask))
				return true;

		}

		return false;
	}

	void Attack()
	{
		if (movement.GetTileInFront().entityOnTile == player)
			AttackMelee();
		else
			AttackRanged();

		AP -= AttackCost;
	}

	void AttackRanged()
	{
		player.TakeDamage(Damage / 2, MyPosition.X, MyPosition.Y);
	}

	void AttackMelee()
	{
		player.TakeDamage(Damage, MyPosition.X, MyPosition.Y);
	}

	protected override void CreateBehaviourTree()
	{
		Action moveAction = new Action(action => MoveToNextTile());
		
		#region damaged
		//run away if health <25%
		Action fleeCheckAction = new Action(action => CheckFleeConditions());
		
		Action findPathAwayFromPlayerAction = new Action(action => FindPathAwayFromPlayer());
		//CounterDecorator fleePathfindTreshold = new CounterDecorator(3, AlienAiState.Flee, findPathAwayFromPlayerAction);
		
		Sequence fleeSequence = new Sequence(fleeCheckAction, findPathAwayFromPlayerAction, moveAction);
		DecoratorCondition deadendBerserkFuse = new DecoratorCondition(runFunc => CheckIfBerserk(blackboard), fleeSequence);
		
		//attack headon otherwise
		Action findPathToPlayerAction = new Action(action => FindPathToLastKnownPlayerPosition());
		//CounterDecorator playerPathfindTreshold = new CounterDecorator(3, AlienAiState.Chase, findPathToPlayerAction);
		
		Sequence berserkSequence = new Sequence(fleeCheckAction, findPathToPlayerAction, moveAction);
		
		PrioritySelector damagedSelector = new PrioritySelector(deadendBerserkFuse, berserkSequence);
		
		#endregion
		
		#region Attack player
		
		//go here always
		findPathToPlayerAction = new Action(action => FindPathToLastKnownPlayerPosition());
		//playerPathfindTreshold = new CounterDecorator(3, AlienAiState.Chase, findPathToPlayerAction);
		//end
		
		//flanking/hiding if headon
		Action checkIfHeadonAction = new Action(action => CheckIfPathIsHeadon());
		
		//stay hidden
		Action checkIfAlreadyInCover = new Action(action => CheckIfAlreadyBehindCover());
		Action moveInCoverAction = new Action(action => MoveInCover());
		Sequence moveWhileInCoverSequence = new Sequence(checkIfAlreadyInCover, moveInCoverAction);
		
		//get into cover
		Action checkForNearbyCoverAction = new Action(action => CanHide());
		Sequence moveToCoverSequence = new Sequence(checkForNearbyCoverAction, moveAction);
		
		PrioritySelector getIntoCoverSelector = new PrioritySelector(moveWhileInCoverSequence, moveToCoverSequence);
		
		Sequence flankHideSequence = new Sequence(checkIfHeadonAction, getIntoCoverSelector);
		//end
		
		
		//selector for flanking or charging
		PrioritySelector tactiqueSelector = new PrioritySelector(flankHideSequence, moveAction);
		
		//the main sequence
		Sequence attackSequence = new Sequence(findPathToPlayerAction, tactiqueSelector);
		
		#endregion

		#region Search player

		Action SearchPlayerAction = new Action(action => SearchPlayer());

		#endregion
		
		#region Random movement
		
		//randomness
		Action findRandomPathAction = new Action(action => FindRandomPath());
		//CounterDecorator randomPathfindTreshold = new CounterDecorator(3, AlienAiState.Rand, findRandomPathAction);
		Sequence randomMovementSequence = new Sequence(findRandomPathAction, moveAction);//new Action(action =>RandomMovement()));
		
		#endregion
		
		//root
		Action checkPlayerPresenceAction = new Action(action => CheckForPlayerPresence());

		//player present
		//return success to make sure that randomness isn't triggered when player is present
		Action returnSuccessAction = new Action(action => SuccessReturner());
		
		PrioritySelector playerFoundSelector = new PrioritySelector(damagedSelector, attackSequence/*, ei löydetty */, returnSuccessAction);
		Sequence playerPresentSequence = new Sequence(checkPlayerPresenceAction, playerFoundSelector);

		//player not present
		PrioritySelector playerNotFoundSelector = new PrioritySelector(SearchPlayerAction, randomMovementSequence);
		
		PrioritySelector root = new PrioritySelector(playerPresentSequence, playerNotFoundSelector);
		
		behaviourTree = root;
	}
	
	public RunStatus SuccessReturner()
	{
		return RunStatus.Success;
	}

	private void RandomMovement()
	{/*
		int rand = Subs.GetRandom(100);
		if (rand >= 66)
		{
			if (movement.MoveForward())
			{
				//Debug.Log("forward");
				AP--;
				HasUsedTurn = true;
				foundMove = true;
			}
		}
		else if (rand > 33 && rand < 66)
		{
			//Debug.Log("right");
			AP--;
			HasUsedTurn = true;
			foundMove = true;
			movement.TurnRight();
		}
		else
		{*/
			//Debug.Log("left");
			AP--;
			HasUsedTurn = true;
			foundMove = true;
			movement.TurnLeft();
		//}
	}
}
