using UnityEngine;
using System.Collections;

public class ChangeScene : MonoBehaviour {
	
	public string changeToScene;
	// Use this for initialization
	void Start () {
		Application.LoadLevel(changeToScene);
	}
}
