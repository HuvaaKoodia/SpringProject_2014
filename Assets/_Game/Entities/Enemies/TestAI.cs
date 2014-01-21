using UnityEngine;
using System.Collections;

public class TestAI : MonoBehaviour {

    GameObject parent;
    EntityMovementSub movement;

    TileMain[,] tilemap;
    AStar pathfinder;

	// Use this for initialization
	void Start () {
        parent = transform.root.gameObject;
        movement = parent.gameObject.GetComponent<EntityMovementSub>();

        tilemap = GameObject.Find("SharedSystems").GetComponentInChildren<GameController>().TileMainMap;
        /*pathfinder = new AStar();

		AStarGridNode goal = new AStarGridNode(null, null, tilemap[movement.currentGridX, movement.currentGridY]);
        AStarGridNode start = new AStarGridNode(null, goal, tilemap[movement.currentGridX, movement.currentGridY]);

        pathfinder.FindPath(start, goal);*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void RandomMovement()
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
