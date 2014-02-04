using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MenuState
{
	NothingSelected, MovementHUD, TargetingHUD, InventoryHUD
}

public class MenuHandler : MonoBehaviour {

	public GameController GC;
	public PlayerMain player;

	public Camera NGUICamera;

    public UISprite targetingText;
	public UISprite turnText;

	public UISprite endButton;
	public UISprite engageButton;

    public UILabel healthText;

	public UIPanel targetMarkPanel;

	public GunInfoDisplay gunInfoDisplay;

	public MenuState currentMenuState;

	// Use this for initialization
	void Start()
	{
		currentMenuState = MenuState.NothingSelected;
	}
	
	// Update is called once per frame
	void Update () {
	}

	void MoveBackwardButtonPressed()
	{
		player.inputSub.MoveBackwardInput();
	}

	void MoveForwardButtonPressed()
	{
		player.inputSub.MoveForwardInput();
	}

	void MoveLeftButtonPressed()
	{
		player.inputSub.MoveLeftInput();
	}

	void MoveRightButtonPressed()
	{
		player.inputSub.MoveRightInput();
	}

	void TurnLeftButtonPressed()
	{
		player.inputSub.TurnLeftInput();
	}

	void TurnRightButtonPressed()
	{
		player.inputSub.TurnRightInput();
	}

	void TargetingModeButtonPressed()
	{
		player.inputSub.TargetingModeInput();
	}

	void EndTurnButtonPressed()
	{
		player.inputSub.EndTurnInput();
	}

	void EngageCombatPressed()
	{
		player.inputSub.EngageCombatInput();
	}

	void LeftHandWeaponPressed()
	{
		player.inputSub.ChangeWeaponInput(WeaponID.LeftHand);
	}

	void LeftShoulderWeaponPressed()
	{
		player.inputSub.ChangeWeaponInput(WeaponID.LeftShoulder);
	}

	void RightHandWeaponPressed()
	{
		player.inputSub.ChangeWeaponInput(WeaponID.RightHand);
	}

	void RightShoulderWeaponPressed()
	{
		player.inputSub.ChangeWeaponInput(WeaponID.RightShoulder);
	}

	public void ToggleMovementHUD()
	{
		ChangeMenuState(MenuState.MovementHUD);
	}

	public void ToggleTargetingHUD()
	{
		ChangeMenuState(MenuState.TargetingHUD);
	}

	public void ToggleInventoryHUD()
	{
		ChangeMenuState(MenuState.InventoryHUD);
	}

	void ChangeMenuState(MenuState newState)
	{
		if (newState == currentMenuState)
			currentMenuState = MenuState.NothingSelected;

		switch (currentMenuState)
		{
		case MenuState.NothingSelected:
			break;

		case MenuState.MovementHUD:
			break;

		case MenuState.TargetingHUD:
			break;

		case MenuState.InventoryHUD:
			break;
		}
	}

	public void CheckTargetingModePanel()
	{
		targetingText.gameObject.SetActive(player.targetingMode);
		engageButton.gameObject.SetActive(player.targetingSub.HasAnyTargets());
		//don't show end turn button when targeting, as it's kinda confusing
		//as it won't shoot
		endButton.gameObject.SetActive(!player.targetingMode);
	}

	public void UpdateHealthText (int health)
	{
		healthText.text = ""+health;
	}
}
