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
    public bool foundMove;
	public bool waitedForOthersToMoveThisTurn;

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
        parent = gameObject.GetComponent<EnemyMain>();
		movement = parent.movement;

		tilemap = parent.GC.TileMainMap;
		player = parent.GC.Player;

		HasUsedTurn = false;
        foundMove = false;
		waitedForOthersToMoveThisTurn = false;

		ap = apMax;
		
		CheckPath();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void PlayMovementPhase()
	{
		if (hasTarget)
		{
            MoveToTarget();
		}
		else
		{
            //parent.FinishedMoving(true);
			//HasUsedTurn = true;
            RandomMovement();
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
		player.TakeDamage(damage,movement.currentGridX,movement.currentGridY);
	}

	private void MoveToTarget()
    {
        if (path == null || path.next == null)
		{
            HasUsedTurn = true;
			parent.FinishedMoving(true);
            return;
		}

		MovementState movedSuccessfully = movement.MoveToTile(path.next.position, true);

        if (movedSuccessfully == MovementState.Moving)
		{
            HasUsedTurn = true;
            foundMove = true;
            ap -= movementCost;
            path = path.next;
		}
        else if (movedSuccessfully == MovementState.Turning)
        {
            HasUsedTurn = true;
            foundMove = true;
            ap -= movementCost;
        }
        else
        {
            waitedForOthersToMoveThisTurn = true;
            EntityMain entityBlocking = tilemap[path.next.position.X, path.next.position.Y].entityOnTile;
            //Debug.Log(entityBlocking.tag + " is blocking " + path.next.position);
            if (entityBlocking != null)
            {
				//vähän rikki niin kommenteissa, korjataan kun tärkeämpää asiaa saatu alta pois
                if (!waitedForOthersToMoveThisTurn)
                {
                    if (entityBlocking.tag == "Player")
                    {
                        HasUsedTurn = true;
                        parent.FinishedMoving(true);
                    }
                }
                else
                {
                    HasUsedTurn = true;
                }
			}
		}
    }

	private void CheckPath()
	{	
		Point3D currentPos = new Point3D(movement.currentGridX, movement.currentGridY);
		Point3D targetPos = new Point3D(player.movement.currentGridX, player.movement.currentGridY);

		path = PathFinder.FindPath(tilemap, currentPos, targetPos, 20);

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
		int rand = Subs.GetRandom(3);
		switch (rand)
		{
		case 0:
            if (movement.MoveForward())
            {
                //Debug.Log("forward");
                ap--;
                HasUsedTurn = true;
                foundMove = true;
            }
			break;
		case 1:
            //Debug.Log("right");
            ap--;
            HasUsedTurn = true;
            foundMove = true;
			movement.TurnRight();
			break;
		case 2:
            //Debug.Log("left");
            ap--;
            HasUsedTurn = true;
            foundMove = true;
			movement.TurnLeft();
			break;
		}
	}
}
