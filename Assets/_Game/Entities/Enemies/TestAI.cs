using UnityEngine;
using System.Collections;

public class TestAI : MonoBehaviour {

    GameObject parent;
    EntityMovementSub movement;

	// Use this for initialization
	void Start () {
        parent = transform.root.gameObject;
        movement = parent.gameObject.GetComponent<EntityMovementSub>();
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
                movement.SendMessage("MoveForward");
                break;
            case 1:
                movement.SendMessage("MoveBackward");
                break;
            case 2:
                movement.SendMessage("TurnLeft");
                break;
            case 3:
                movement.SendMessage("TurnRight");
                break;
        }
    }
}
