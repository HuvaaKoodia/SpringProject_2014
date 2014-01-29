using UnityEngine;
using System.Collections;

public class PlayerInputSub : MonoBehaviour {

    PlayerMain player;
    EntityMovementSub playerMovement;

	// Use this for initialization
	void Start()
    {
        player = gameObject.GetComponent<PlayerMain>();
		playerMovement = player.movement;
	}
	
	// Update is called once per frame
	void Update()
    {
        if (playerMovement.currentMovement == MovementState.NotMoving)
        {
            HotkeyInput();

            MouseInput();
        }
	}

    void HotkeyInput()
    {
		if (!player.targetingMode)
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
		else if (Input.GetButtonDown("Engage Combat"))
		{
			EngageCombatInput();
		}
    }

	void MouseInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Component target;
			if (player.targetingMode && Subs.GetObjectMousePos(out target, 20, "Enemy"))
			{
                EnemyMain enemy = target.GetComponent<EnemyMain>();
				player.gun.ToggleTarget(enemy);

                return;
			}

			if (!player.targetingMode && Subs.GetObjectMousePos(out target, MapGenerator.TileSize.magnitude - 2, "Loot"))
            {
                player.PickupLoot(target.gameObject);
            }
		}
	}
	
	public void MoveForwardInput()
	{
		if (this.enabled == false || playerMovement.currentMovement != MovementState.NotMoving || player.targetingMode)
			return;

		if (playerMovement.MoveForward())
			player.StartedMoving();
	}

	public void MoveBackwardInput()
	{
		if (this.enabled == false || playerMovement.currentMovement != MovementState.NotMoving || player.targetingMode)
			return;
		
		if (playerMovement.MoveBackward())
			player.StartedMoving();
	}

	public void TurnLeftInput()
	{
		if (this.enabled == false || playerMovement.currentMovement != MovementState.NotMoving || player.targetingMode)
			return;
		
		playerMovement.TurnLeft();
		player.StartedMoving();
	}

	public void TurnRightInput()
	{
		if (this.enabled == false || playerMovement.currentMovement != MovementState.NotMoving || player.targetingMode)
			return;
		
		playerMovement.TurnRight();
		player.StartedMoving();
	}

	public void TargetingModeInput()
	{
		if (this.enabled == false || playerMovement.currentMovement != MovementState.NotMoving)
			return;

		if (player.targetingMode)
			player.EndTargetingMode();
		else
			player.StartTargetingMode();
	}

	public void EngageCombatInput()
	{
		if (this.enabled == false || !player.targetingMode)
			return;

		player.Attack();
	}

	public void EndTurnInput()
	{
		if (this.enabled == false || playerMovement.currentMovement != MovementState.NotMoving)
			return;

		player.EndPlayerPhase();
	}
}
