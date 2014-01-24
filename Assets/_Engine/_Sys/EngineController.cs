using UnityEngine;
using System.Collections;
public class EngineController : MonoBehaviour {
	
	public bool enable_Restart=true,enable_Quit=true;
	public System.Action AfterRestart,BeforeQuit;
	
	
	//Update is called once per frame
	void Update (){

		if (enable_Restart&&Input.GetKeyDown(KeyCode.R)){
			
			Application.LoadLevel(Application.loadedLevel);
			if (AfterRestart!=null)
				AfterRestart();
		}
		
		if (enable_Quit&&Input.GetKeyDown(KeyCode.Escape)){
			if (BeforeQuit!=null)
				BeforeQuit();
			Application.Quit();
		}
	}
}
