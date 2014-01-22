using UnityEngine;
using System.Collections;

public class PlayerMain : MonoBehaviour
{
	public GameController GC;
    PlayerInputSub inputSub;
	public EntityMovementSub movement { get; private set; }

	// Use this for initialization
	void Start()
    {
        GC = GameObject.Find("GameSystems").GetComponent<GameController>();
        inputSub = GetComponent<PlayerInputSub>();
		movement = GetComponent<EntityMovementSub>();
	}
	
	// Update is called once per frame
	void Update()
    {
	
	}

    public void StartTurn()
    {
        inputSub.enabled = true;
    }

    void EndTurn()
    {
		GC.ChangeTurn(TurnState.StartAITurn);
    }

	public void FinishedMoving()
	{
		EndTurn();
	}

	public void StartedMoving()
	{
		inputSub.enabled = false;
	}
}
