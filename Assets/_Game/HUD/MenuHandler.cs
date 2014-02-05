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

	public UISprite turnText;
	
	public UISprite engageButton;

    public UILabel healthText;

	public UIPanel targetMarkPanel;

	public GunInfoDisplay gunInfoDisplay;

	public MenuState currentMenuState;

	public GameObject EndHud;
	public GameObject MovementHud;
	public GameObject TargetingHud;
	public GameObject InteractHud;
	public InventoryMain InventoryHud;

	// Use this for initialization
	void Start()
	{
		currentMenuState = MenuState.NothingSelected;
		MovementHud.SetActive(false);
		TargetingHud.SetActive(false);
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

	void InteractButtonPressed()
	{
		player.inputSub.InteractInput(false);
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

	public void SetInteractVisibility(bool visible)
	{
		if (currentMenuState == MenuState.InventoryHUD ||
		    currentMenuState == MenuState.TargetingHUD)
			visible = false;

		InteractHud.SetActive(visible && player.interactSub.HasInteractable);
	}

	void ChangeMenuState(MenuState newState)
	{
		if (newState == currentMenuState)
			currentMenuState = MenuState.NothingSelected;
		else
			currentMenuState = newState;

		switch (currentMenuState)
		{
		case MenuState.NothingSelected:
			EndHud.SetActive(true);
			MovementHud.SetActive(false);
			TargetingHud.SetActive(false);
			InventoryHud.DeactivateInventory();
			GC.Player.EndTargetingMode();
			SetInteractVisibility(true);
			break;

		case MenuState.MovementHUD:
			EndHud.SetActive(false);
			MovementHud.SetActive(true);
			TargetingHud.SetActive(false);
			InventoryHud.DeactivateInventory();
			GC.Player.EndTargetingMode();
			break;

		case MenuState.TargetingHUD:
			EndHud.SetActive(false);
			MovementHud.SetActive(false);
			TargetingHud.SetActive(true);
			InventoryHud.DeactivateInventory();
			SetInteractVisibility(false);
			break;

		case MenuState.InventoryHUD:
			EndHud.SetActive(false);
			MovementHud.SetActive(false);
			TargetingHud.SetActive(false);
			GC.Player.EndTargetingMode();
			SetInteractVisibility(false);
			break;
		}
	}

	public void CheckTargetingModePanel()
	{
		engageButton.gameObject.SetActive(player.targetingSub.HasAnyTargets());
	}

	public void UpdateHealthText (int health)
	{
		healthText.text = ""+health;
	}
}
