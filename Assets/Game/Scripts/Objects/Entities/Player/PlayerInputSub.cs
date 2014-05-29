using UnityEngine;
using System.Collections;

public class PlayerInputSub : MonoBehaviour {

	public bool DISABLE_INPUT=false;

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
		if (DISABLE_INPUT) return;


		//always usable hotkeys

		if (Input.GetButtonDown("Toggle Flashlight"))
		{
			player.ToggleFlashlight();
		}
		if (Input.GetButtonDown("Toggle Freelook"))
		{
			FreeLookToggleInput();
		}

        if (playerMovement.currentMovement == MovementState.NotMoving && !player.interactSub.WaitingInteractToFinish && !player.Shooting)
        {
            HotkeyInput();

			if (!UICamera.MenuButtonPressed)
            	MouseInput();
        }

		UICamera.MenuButtonPressed = false;
	}

	bool pressed_once_moving=false,pressed_once_turning=false;

    void HotkeyInput()
    {
		if (!player.targetingMode && player.GC.HUD.currentMenuState != MenuState.InventoryHUD)
		{
	        float verticalAxis = Input.GetAxisRaw("Vertical");
			float horizontalAxis = Input.GetAxisRaw("Horizontal");
			bool instant=player.movement.InstantMovement;
			instant=false;//Dev.temp un used

	        if (verticalAxis > 0)
	        {
				if (!instant||instant&&!pressed_once_moving) MoveForwardInput();
				pressed_once_moving=true;
				return ;
	        }
	        else if (verticalAxis < 0)
	        {
				if (!instant||instant&&!pressed_once_moving) MoveBackwardInput();
				pressed_once_moving=true;

				return ;
	        }
	        else if (horizontalAxis > 0)
	        {
				if (!instant||instant&&!pressed_once_moving) MoveRightInput();
				pressed_once_moving=true;

	            return ;
	        }
	        else if (horizontalAxis < 0)
	        {
				if (!instant||instant&&!pressed_once_moving) MoveLeftInput();
				pressed_once_moving=true;

	            return;
	        }
			else{
				pressed_once_moving=false;
			}
			
			if (Input.GetButton("TurnLeft"))
			{
				if (!instant||instant&&!pressed_once_turning) TurnLeftInput();
				pressed_once_turning=true;
			}
			else if (Input.GetButton("TurnRight"))
			{
				if (!instant||instant&&!pressed_once_turning) TurnRightInput();
				pressed_once_turning=true;
			}
			else pressed_once_turning=false;
		}

	 	if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			player.HUD.WeaponPressed(WeaponID.LeftHand);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			player.HUD.WeaponPressed(WeaponID.LeftShoulder);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			player.HUD.WeaponPressed(WeaponID.RightShoulder);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			player.HUD.WeaponPressed(WeaponID.RightHand);
		}
		else if (Input.GetButtonDown("Engage Combat"))
		{
			EngageCombatInput();
		}

		if (Input.GetButtonDown("Targeting mode"))
		{
			TargetingModeInput();
		}

		if (Input.GetButtonDown("Interact"))
		{
			InteractInput(false);
		}

		if (Input.GetButtonDown("Toggle Inventory"))
		{
			player.HUD.ToggleInventory();
		}
		if (Input.GetButtonDown("Toggle Map"))
		{
			player.HUD.MasterHud.ToggleMap();
		}
		if (Input.GetButtonDown("Toggle Logs"))
		{
			player.HUD.MasterHud.ToggleLogs();
		}
		if (Input.GetButtonDown("Toggle Status"))
		{
			player.HUD.MasterHud.ToggleStatus();
		}

		if (Input.GetButtonDown("Disperse Heat"))
		{
			DisperseHeatInput();
		}
		if (Input.GetButtonDown("End Turn"))
		{
			EndTurnInput();
		}

#if UNITY_EDITOR
        //DEV.DEBUG damage
        var x=playerMovement.currentGridX;
        var y=playerMovement.currentGridY;
        if (Input.GetKeyDown(KeyCode.Keypad6)){
            player.TakeDamage(10,x+1,y+0);
        }
        if (Input.GetKeyDown(KeyCode.Keypad8)){
            player.TakeDamage(10,x+0,y+1);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4)){
            player.TakeDamage(10,x-1,y+0);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2)){
            player.TakeDamage(10,x+0,y-1);
        }
		if (Input.GetKeyDown(KeyCode.Keypad5)){
			player.ObjData.Equipment.UpperTorso.ObjData.AddHEAT(25);
		}

        if (Input.GetKeyDown(KeyCode.L)){
            player.ap=200;
            player.movement.movementSpeed=10;
            player.movement.turnSpeed=360;
			player.AnimationsOn = false;
        }
#endif
	}
	
	void MouseInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			MouseLeftInput();
		}
		else if (Input.GetMouseButtonDown(1))
		{
			if (player.targetingMode)
			{
				player.targetingSub.ClickTargetAtMousePosition(false);
				player.HUD.CheckTargetingModePanel();
				player.HUD.UpdateHudPanels();
			}
		}
	}
	
	public void MoveForwardInput()
	{
		if (NotUsableWorldInteractions()) return;

		if (playerMovement.MoveForward())
			player.StartedMoving();
	}

	public void MoveBackwardInput()
	{
		if (NotUsableWorldInteractions()) return;
		
		if (playerMovement.MoveBackward())
			player.StartedMoving();
	}

	public void MoveLeftInput()
	{
		if (NotUsableWorldInteractions()) return;
		
		if (playerMovement.MoveLeft())
			player.StartedMoving();
	}

	public void MoveRightInput()
	{
		if (NotUsableWorldInteractions()) return;
		
		if (playerMovement.MoveRight())
			player.StartedMoving();
	}

	public void TurnLeftInput()
	{
		if (NotUsableWorldInteractions()) return;
		
		playerMovement.TurnLeft();
		player.StartedMoving();
	}

	public void TurnRightInput()
	{
		if (NotUsableWorldInteractions()) return;
		
		playerMovement.TurnRight();
		player.StartedMoving();
	}

	public void TargetingModeInput()
	{
		if (NotUsable()||player.SystemOverheat) return;

		if (player.targetingMode){
			player.EndTargetingMode();
			player.HUD.ToggleTargetingHUD();
        }
        else{
            if (player.StartTargetingMode()) player.HUD.ToggleTargetingHUD();
        }
	}

	public void EngageCombatInput()
	{
		if (this.enabled == false || !player.targetingMode) return;

		if (player.targetingSub.HasAnyTargets()) StartCoroutine(player.Attack());
    }

	public void DisperseHeatInput()
	{
        if (NotUsable()) return;

		player.DisperseWeaponHeat();

		player.EndPlayerPhase();
	}
    
    public void ChangeWeaponInput(WeaponID id)
	{
		if (NotUsable()|| player.GC.HUD.currentMenuState == MenuState.InventoryHUD)
			return;

		player.ChangeWeapon(id);
	}

	public void EndTurnInput()
	{
        if (NotUsable()) return;

		player.EndPlayerPhase();
	}

	void MouseLeftInput(){
		if (player.targetingMode)
		{
			player.targetingSub.ClickTargetAtMousePosition(true);
			player.HUD.CheckTargetingModePanel();
			player.HUD.UpdateHudPanels();
		}
		else
		{
			if (player.targetingSub.HasTargetAtMousePosition ())
			{
				if (player.interactSub.HasInteractable)
				{
					DoorMain doorInFront = player.movement.GetTileInFront().GetDoor();

					if (doorInFront != null && doorInFront.IsOpen == false)
					{
						InteractInput(true);
						return;
					}
				}

				TargetingModeInput();
				ChangeWeaponInput(player.GetCurrentWeapon().weaponID);
			}
			else if (player.interactSub.HasInteractable)
			{
				InteractInput(true);
			}
		}
	}

	public void InteractInput(bool screenClick)
	{
        if (NotUsable()) return;

		if (player.GC.HUD.currentMenuState == MenuState.NothingSelected || 
		    player.GC.HUD.currentMenuState == MenuState.MovementHUD)
			player.interactSub.Interact(screenClick);
	}

	public void FreeLookToggleInput()
	{
		player.ToggleMouseLook();
	}

    public bool NotUsable(){
		return this.enabled == false || playerMovement.currentMovement != MovementState.NotMoving  || player.interactSub.WaitingInteractToFinish
			|| player.GC.HUD.currentMenuState == MenuState.InventoryHUD || player.playerAnimation.isPlaying || player.Shooting;
    }

	public bool NotUsableInGameInfoPanel(){
		return this.enabled == false || playerMovement.currentMovement != MovementState.NotMoving  || player.interactSub.WaitingInteractToFinish
			|| player.playerAnimation.isPlaying || player.Shooting;
	}

	bool NotUsableWorldInteractions(){
		return NotUsable()|| player.targetingMode||player.SystemOverheat;
	}
}
