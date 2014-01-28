using UnityEngine;
using System.Collections;

public class PlayerInputSub : MonoBehaviour {

    PlayerMain player;
    EntityMovementSub playerMovement;

	public bool targetingMode;

	// Use this for initialization
	void Start()
    {
        player = gameObject.GetComponent<PlayerMain>();
		playerMovement = player.movement;

		targetingMode = false;
	}
	
	// Update is called once per frame
	void Update()
    {
        if (playerMovement.currentMovement == MovementState.NotMoving)
        {
            HotkeyInput();
        }

		if (targetingMode)
		{
			MouseInput();
		}
	}

    void HotkeyInput()
    {
		if (!targetingMode)
		{
	        float verticalAxis = Input.GetAxis("Vertical");
	        float horizontalAxis = Input.GetAxis("Horizontal");

	        if (verticalAxis > 0)
	        {
				MoveForwardInput();
				return ;
	        }
	        else if (verticalAxis < 0)
	        {
				MoveBackwardInput();
				return ;
	        }
	        else if (horizontalAxis > 0)
	        {
				TurnRightInput();
	            return ;
	        }
	        else if (horizontalAxis < 0)
	        {
				TurnLeftInput();
	            return;
	        }
		}

		if (Input.GetButtonDown("Targeting mode"))
		{
			TargetingModeInput();
		}
    }

	void MouseInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Component target;
			if (Subs.GetObjectMousePos(out target, 50, "Enemy"))
			{
				if (target.tag == "AI");
			}
		}
	}
	
	public void MoveForwardInput()
	{
		if (playerMovement.currentMovement != MovementState.NotMoving || targetingMode)
			return;

		if (playerMovement.MoveForward())
			player.StartedMoving();
	}

	public void MoveBackwardInput()
	{
		if (playerMovement.currentMovement != MovementState.NotMoving || targetingMode)
			return;
		
		if (playerMovement.MoveBackward())
			player.StartedMoving();
	}

	public void TurnLeftInput()
	{
		if (playerMovement.currentMovement != MovementState.NotMoving || targetingMode)
			return;
		
		playerMovement.TurnLeft();
		player.StartedMoving();
	}

	public void TurnRightInput()
	{
		if (playerMovement.currentMovement != MovementState.NotMoving || targetingMode)
			return;
		
		playerMovement.TurnRight();
		player.StartedMoving();
	}

	public void TargetingModeInput()
	{
		targetingMode = !targetingMode;
	}
}
