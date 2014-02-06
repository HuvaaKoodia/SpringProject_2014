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

			if (!player.GC.NGUICamera.MenuButtonPressed)
            	MouseInput();

			//player.GC.menuHandler.gunInfoDisplay.UpdateGunInfo();
        }

		player.GC.NGUICamera.MenuButtonPressed = false;
	}

    void HotkeyInput()
    {
		if (!player.targetingMode && player.GC.menuHandler.currentMenuState != MenuState.InventoryHUD)
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
			
			if (Input.GetButtonDown("TurnLeft"))
			{
				TurnLeftInput();
			}
			else if (Input.GetButtonDown("TurnRight"))
			{
				TurnRightInput();
			}
		}
		else if (player.GC.menuHandler.currentMenuState == MenuState.TargetingHUD)
		{
		 	if (Input.GetKeyDown(KeyCode.Alpha1))
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
			else if (Input.GetButtonDown("Engage Combat"))
			{
				EngageCombatInput();
			}
		}

		if (Input.GetButtonDown("Targeting mode"))
		{
			TargetingModeInput();
		}
#if UNITY_EDITOR
        //DEV.DEBUG damage
        if (Input.GetKeyDown(KeyCode.Keypad6)){
            player.TakeDamage(10,1,0);
        }
        if (Input.GetKeyDown(KeyCode.Keypad8)){
            player.TakeDamage(10,0,1);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4)){
            player.TakeDamage(10,-1,0);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2)){
            player.TakeDamage(10,0,-1);
        }
#endif

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
				InteractInput(true);
			}
			
			player.GC.menuHandler.CheckTargetingModePanel();
		}
		else if (Input.GetMouseButtonDown(1))
		{
			if (player.targetingMode)
			{
				player.targetingSub.TargetAtMousePosition(false);
			}
			else
				player.GC.menuHandler.CheckTargetingModePanel();
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

        player.GC.menuHandler.ToggleTargetingHUD();
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

	public void InteractInput(bool screenClick)
	{
		if (this.enabled == false || playerMovement.currentMovement != MovementState.NotMoving)
			return;

		if (player.GC.menuHandler.currentMenuState == MenuState.NothingSelected || 
		    player.GC.menuHandler.currentMenuState == MenuState.MovementHUD)
			player.interactSub.Interact(screenClick);
	}
}
