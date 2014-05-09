using UnityEngine;
using System.Collections;
public class EngineController : MonoBehaviour {
	
	public bool enable_Restart=true,enable_Quit=true,ClearConsoleOnRestart=true;
	public System.Action AfterRestart,BeforeQuit;
	
	
	//Update is called once per frame
	void Update (){
		if (Input.GetKeyDown(KeyCode.R)){
            Restart();
		}
		if (Input.GetKeyDown(KeyCode.Escape)){
            Quit();
		}
	}

    public void Restart()
    {
        if (!enable_Restart)
            return;

		if (ClearConsoleOnRestart)
			ClearConsole();

        Application.LoadLevel(Application.loadedLevel);
        if (AfterRestart != null)
            AfterRestart();
    }

    public void Quit()
    {
        if (!enable_Quit)
            return;

        if (BeforeQuit != null)
            BeforeQuit();
        Application.Quit();
    }

	void ClearConsole()
	{
		StartCoroutine(ClearConsole_E());
	}
	
	IEnumerator ClearConsole_E()
	{
		while(!Debug.developerConsoleVisible)
		{
			yield return null;
		}
		yield return null;
		Debug.ClearDeveloperConsole();
	}
}
