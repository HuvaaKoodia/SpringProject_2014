using UnityEngine;
using System.Collections;

public class TestAI : MonoBehaviour
{

    EnemyMain parent;
    EntityMovementSub movement;

    TileMain[,] tilemap;

	PlayerMain player;

	bool hasTarget;
	public bool HasUsedTurn;
	bool waitedForOthersToMoveThisTurn;

	int turnsSincePathCheck;
	public const int PathCheckInterval = 3;

	public int ap;
	const int apMax = 3;
    const int movementCost = 1;
    const int attackCost = 2;

	public int damage = 20;
	
    public SearchNode path;
	// Use this for initialization
	void Start () {
        parent = transform.root.gameObject.GetComponent<EnemyMain>();
		movement = parent.movement;

        tilemap = GameObject.Find("SharedSystems").GetComponentInChildren<GameController>().TileMainMap;

		player = parent.GC.player;

		HasUsedTurn = false;
		waitedForOthersToMoveThisTurn = false;

		ap = apMax;

		if (parent.parent.GC.UseTestMap)
		CheckPath();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void PlayMovementPhase()
	{
		waitedForOthersToMoveThisTurn = false;
		HasUsedTurn = true;

		if (hasTarget)
		{
            MoveToTarget();
		}
		else
		{
            parent.FinishedMoving(false);
		}

		turnsSincePathCheck++;
		if (turnsSincePathCheck >= PathCheckInterval)
		{
			CheckPath();
		}
	}

	public void PlayAttackPhase()
	{
        if (ap >= attackCost)
        {
			TileMain tileinFront = movement.GetTileInFront();

			if (tileinFront != null && tileinFront.entityOnTile != null && tileinFront.entityOnTile.tag == "Player")
			{
				Attack ();
			}
        }

        parent.FinishedAttacking();
	}

	private void Attack()
	{
		player.TakeDamage(damage);
	}

	private void MoveToTarget()
    {
        if (path == null || path.next == null || ap <= 0)
		{
			parent.FinishedMoving(true);
            return;
		}

		MovementState movedSuccessfully = movement.MoveToTile(path.next.position, true);

        if (movedSuccessfully == MovementState.Moving)
		{
            ap -= movementCost;
            path = path.next;
		}
        else if (movedSuccessfully == MovementState.Turning)
        {
            ap -= movementCost;
        }
        else
        {
            EntityMain entityBlocking = tilemap[path.next.position.X, path.next.position.Y].entityOnTile;
            //Debug.Log(entityBlocking.tag + " is blocking " + path.next.position);
            if (entityBlocking != null)
            {
				//vähän rikki niin kommenteissa, korjataan kun tärkeämpää asiaa saatu alta pois
                if (!waitedForOthersToMoveThisTurn)
                {
					if (entityBlocking.tag == "AI")
					{
	                    waitedForOthersToMoveThisTurn = true;
	                    EnemyMain blocker  = entityBlocking.GetComponent<EnemyMain>();

	                    if (!blocker.waitingForAttackPhase)
	                    {
	                        //Debug.Log("Telling other guy to move");
	                        blocker.PlayMovementPhase();
	                    }
	                    else
	                    {
	                        //Debug.Log("Finding path around stationary enemy");
	                        CheckPath();
	                    }

	                    this.MoveToTarget();
					}
					else if (entityBlocking.tag == "Player")
					{
						parent.FinishedMoving(true);
					}
					return;
				}
				CheckPath();
			}
		}
    }

	private void CheckPath()
	{	
		Point3D currentPos = new Point3D(movement.currentGridX, movement.currentGridY);
		Point3D targetPos = new Point3D(player.movement.currentGridX, player.movement.currentGridY);

		path = PathFinder.FindPath(tilemap, currentPos, targetPos);

		if (path == null)
			hasTarget = false;
		else
			hasTarget = true;

		turnsSincePathCheck = 0;
	}

	public void ResetAP()
	{
		ap = apMax;
	}

	private void RandomMovement()
	{
		int rand = Subs.GetRandom(4);
		switch (rand)
		{
		case 0:
			movement.MoveForward();
			break;
		case 1:
			movement.MoveBackward();
			break;
		case 2:
			movement.TurnLeft();
			break;
		case 3:
			movement.TurnRight();
			break;
		}
	}
}
