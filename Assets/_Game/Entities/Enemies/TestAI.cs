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

		CheckPath();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void PlayMovementPhase()
	{
		waitedForOthersToMoveThisTurn = false;
		HasUsedTurn = true;

		if (!hasTarget)
		{
			parent.FinishedMoving();
		}
		else
		{
			MoveToTarget();
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

		bool DidMove = movement.MoveToTile(path.next.position, true);

		ap--;

       	if (DidMove)
		{
			if (movement.currentMovement == MovementState.Moving)
			{
            	path = path.next;
			}
			else
			{
				Debug.Log("Something is blocking " + path.next.position);
				EntityMain entityBlocking = tilemap[path.next.position.X, path.next.position.Y].entityOnTile;

				if (entityBlocking != null) 
				{
					/*if (entityBlocking.tag == "AI" && !waitedForOthersToMoveThisTurn)
					{
						Debug.Log("Telling other guy to move");
						waitedForOthersToMoveThisTurn = true;
						entityBlocking.GetComponent<EnemyMain>().PlayMovementPhase();
						this.MoveToTarget();
					}*/
					if (entityBlocking.tag == "Player")
					{
						//meleetä tänne
						Debug.Log("Enemy at melee range");
					}
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
