using UnityEngine;
using System.Collections;

public class PlayerInputSub : MonoBehaviour {

    GameObject player;
    EntityMovementSub playerMovement;

	// Use this for initialization
	void Start()
    {
        player = transform.root.gameObject;
        playerMovement = player.gameObject.GetComponent<EntityMovementSub>();
	}
	
	// Update is called once per frame
	void Update()
    {
        if (playerMovement.currentMovement == MovementState.NotMoving)
        {
            bool didMove = MovementInput();
            if (didMove)
            {
                SendMessageUpwards("EndTurn");
            }
        }
	}

    bool MovementInput()
    {
        float verticalAxis = Input.GetAxis("Vertical");
        float horizontalAxis = Input.GetAxis("Horizontal");

        if (verticalAxis > 0)
        {
            playerMovement.SendMessage("MoveForward");
            return true;
        }
        else if (verticalAxis < 0)
        {
            playerMovement.SendMessage("MoveBackward");
            return true;
        }
        else if (horizontalAxis > 0)
        {
            playerMovement.SendMessage("TurnRight");
            return true;
        }
        else if (horizontalAxis < 0)
        {
            playerMovement.SendMessage("TurnLeft");
            return true;
        }

        return false;
    }

    void MouseInput()
    {
        //TODO
    }

    void HotkeyInput()
    {
        //TODO
    }
}
