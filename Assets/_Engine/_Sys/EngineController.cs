using UnityEngine;
using System.Collections;
public class EngineController : MonoBehaviour {
	
	public bool enable_Restart=true,enable_Quit=true;
	
	//Update is called once per frame
	void Update (){

		if (enable_Restart&&Input.GetKeyDown(KeyCode.R)){
			Application.LoadLevel(Application.loadedLevel);
		}
		
		if (enable_Quit&&Input.GetKeyDown(KeyCode.Escape)){
			Application.Quit();
		}
	}
}
