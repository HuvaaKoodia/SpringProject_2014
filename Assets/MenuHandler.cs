using UnityEngine;
using System.Collections;

public class MenuHandler : MonoBehaviour {

	public GameController GC;
	public PlayerMain player;

	// Use this for initialization
	void Start () {
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
}
