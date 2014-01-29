using UnityEngine;
using System.Collections;

public class MenuHandler : MonoBehaviour {

	public GameController GC;
	public PlayerMain player;

    public UISprite targetingText;
	public UISprite turnText;
    public UILabel healthText;
	// Use this for initialization
	void Start () {
       targetingText.gameObject.SetActive(player.inputSub.targetingMode);
       healthText.text = player.Health.ToString();
	}
	
	// Update is called once per frame
	void Update () {
	    healthText.text = player.Health.ToString();
		turnText.gameObject.SetActive(GC.currentTurn == TurnState.PlayerTurn);
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
        targetingText.gameObject.SetActive(player.inputSub.targetingMode);
	}

	void EndTurnButtonPressed()
	{
		player.inputSub.EndTurnInput();
	}
}
