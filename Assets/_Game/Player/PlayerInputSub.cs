using UnityEngine;
using System.Collections;

public class PlayerInputSub : MonoBehaviour {

    GameObject player;
    PlayerMovementSub playerMovement;

	// Use this for initialization
	void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.gameObject.GetComponent<PlayerMovementSub>();
	}
	
	// Update is called once per frame
	void Update()
    {
        MovementInput();
	}

    void MovementInput()
    {
        float verticalAxis = Input.GetAxis("Vertical");
        float horizontalAxis = Input.GetAxis("Horizontal");

        if (verticalAxis > 0)
        {
            playerMovement.SendMessage("MoveForward");
        }
        else if (verticalAxis < 0)
        {
            playerMovement.SendMessage("MoveBackward");
        }
        else if (horizontalAxis > 0)
        {
            playerMovement.SendMessage("TurnRight");
        }
        else if (horizontalAxis < 0)
        {
            playerMovement.SendMessage("TurnLeft");
        }
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
