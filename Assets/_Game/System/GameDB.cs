using UnityEngine;
using System.Collections;

public class GameDB : MonoBehaviour {

    public PlayerObjData PlayerData{get;private set;}

	// Use this for initialization
	void Awake () {
        StartNewGame();
	}

    public void StartNewGame(){
        PlayerData=new PlayerObjData();
    }
}
