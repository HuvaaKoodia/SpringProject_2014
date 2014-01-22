using UnityEngine;
using System.Collections;

public class PlayerInputSub : MonoBehaviour {

    PlayerMain player;
    EntityMovementSub playerMovement;

	// Use this for initialization
	void Start()
    {
        player = transform.root.gameObject.GetComponent<PlayerMain>();
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
				player.StartedMoving();
			}
        }
	}

    bool MovementInput()
    {
        float verticalAxis = Input.GetAxis("Vertical");
        float horizontalAxis = Input.GetAxis("Horizontal");

        if (verticalAxis > 0)
        {
            return playerMovement.MoveForward();
        }
        else if (verticalAxis < 0)
        {
            return playerMovement.MoveBackward();
        }
        else if (horizontalAxis > 0)
        {
            playerMovement.TurnRight();
            return true;
        }
        else if (horizontalAxis < 0)
        {
            playerMovement.TurnLeft();
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
