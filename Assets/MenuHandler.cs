using UnityEngine;
using System.Collections;

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

	// Use this for initialization
	void Start () {
       	healthText.text = player.Health.ToString();
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
		player.ChangeWeapon(WeaponID.LeftHand);
	}

	void LeftShoulderWeaponPressed()
	{
		player.ChangeWeapon(WeaponID.LeftShoulder);
	}

	void RightHandWeaponPressed()
	{
		player.ChangeWeapon(WeaponID.RightHand);
	}

	void RightShoulderWeaponPressed()
	{
		player.ChangeWeapon(WeaponID.RightShoulder);
	}

	public void CheckTargetingModePanel()
	{
		targetingText.gameObject.SetActive(player.targetingMode);
		engageButton.gameObject.SetActive(player.targetingSub.HasAnyTargets());
		//don't show end turn button when targeting, as it's kinda confusing
		//as it won't shoot
		endButton.gameObject.SetActive(!player.targetingMode);
	}
}
