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

	int ap;
	const int apMax = 3;
	
    public SearchNode path;
	// Use this for initialization
	void Start () {
        parent = transform.root.gameObject.GetComponent<EnemyMain>();
        movement = parent.gameObject.GetComponent<EntityMovementSub>();

        tilemap = GameObject.Find("SharedSystems").GetComponentInChildren<GameController>().TileMainMap;

		player = parent.parent.GC.player;

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
            ap--;
            path = path.next;
		}
        else if (movedSuccessfully == MovementState.Turning)
        {
            ap--;
        }
        else
        {
            EntityMain entityBlocking = tilemap[path.next.position.X, path.next.position.Y].entityOnTile;
            Debug.Log(entityBlocking.tag + " is blocking " + path.next.position);
            if (entityBlocking != null)
            {
                if (entityBlocking.tag == "AI" && !waitedForOthersToMoveThisTurn)
                {
                    waitedForOthersToMoveThisTurn = true;
                    EnemyMain blocker  = entityBlocking.GetComponent<EnemyMain>();

                    if (!blocker.waitingForAttackPhase)
                    {
                        Debug.Log("Telling other guy to move");
                        blocker.PlayMovementPhase();
                        this.MoveToTarget();
                    }
                    else
                    {
                        Debug.Log("Finding path around stationary enemy");
                        CheckPath();
                    }

                    this.MoveToTarget();
                }
                else
                {
                    if (entityBlocking.tag == "Player")
                    {
                        //meleetä tänne
                        Debug.Log("Enemy at melee range");
                    }

                    parent.FinishedMoving(true);
                }
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
}
