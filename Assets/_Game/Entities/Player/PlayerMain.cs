using UnityEngine;
using System.Collections;

public class PlayerMain : MonoBehaviour
{
    GameController GC;
    PlayerInputSub inputSub;

	// Use this for initialization
	void Start()
    {
        GC = GameObject.Find("GameSystems").GetComponent<GameController>();
        inputSub = GetComponent<PlayerInputSub>();
	}
	
	// Update is called once per frame
	void Update()
    {
	
	}

    void StartTurn()
    {
        inputSub.enabled = true;
    }

    void EndTurn()
    {
        inputSub.enabled = false;
        GC.ChangeTurn();
    }
}
