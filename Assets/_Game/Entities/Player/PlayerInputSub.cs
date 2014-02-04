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

			player.GC.menuHandler.gunInfoDisplay.UpdateGunInfo();
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
				MoveRightInput();
	            return ;
	        }
	        else if (horizontalAxis < 0)
	        {
				MoveLeftInput();
	            return;
	        }
		}

		if (Input.GetButtonDown("TurnLeft"))
		{
			TurnLeftInput();
		}
		else if (Input.GetButtonDown("TurnRight"))
		{
			TurnRightInput();
		}
		else if (Input.GetButtonDown("Targeting mode"))
		{
			TargetingModeInput();
		}
		else if (Input.GetButtonDown("Engage Combat"))
		{
			EngageCombatInput();
		}
		else if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			ChangeWeaponInput(WeaponID.LeftShoulder);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			ChangeWeaponInput(WeaponID.LeftHand);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			ChangeWeaponInput(WeaponID.RightShoulder);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			ChangeWeaponInput(WeaponID.RightHand);
		}
    }

	void MouseInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (player.targetingMode)
			{
				player.targetingSub.TargetAtMousePosition(true);
			}
			else
			{
				Component target;

                //DEV.TEMP DOOR CHECK
                var door=playerMovement.CheckForDoor();
                if (door!=null){
                    door.Toggle();
                }
                else
				if (Subs.GetObjectMousePos(out target, MapGenerator.TileSize.magnitude - 2, "Loot"))
	            {
	                player.PickupLoot(target.GetComponent<LootCrateMain>());
	            }
				
				
			}
			
			player.GC.menuHandler.CheckTargetingModePanel();
		}
		else if (Input.GetMouseButtonDown(1))
		{
			if (player.targetingMode)
			{
				player.targetingSub.TargetAtMousePosition(false);
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

	public void MoveLeftInput()
	{
		if (this.enabled == false || playerMovement.currentMovement != MovementState.NotMoving || player.targetingMode)
			return;
		
		if (playerMovement.MoveLeft())
			player.StartedMoving();
	}

	public void MoveRightInput()
	{
		if (this.enabled == false || playerMovement.currentMovement != MovementState.NotMoving || player.targetingMode)
			return;
		
		if (playerMovement.MoveRight())
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

		player.GC.menuHandler.ToggleTargetingHUD();
	}

	public void EngageCombatInput()
	{
		if (this.enabled == false || !player.targetingMode)
			return;

		player.Attack();
	}

	public void ChangeWeaponInput(WeaponID id)
	{
		if (this.enabled == false || playerMovement.currentMovement != MovementState.NotMoving)
			return;

		player.ChangeWeapon(id);
	}

	public void EndTurnInput()
	{
		if (this.enabled == false || playerMovement.currentMovement != MovementState.NotMoving)
			return;

		player.EndPlayerPhase();
	}
}
