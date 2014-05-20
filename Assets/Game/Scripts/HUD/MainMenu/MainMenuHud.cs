using UnityEngine;
using System.Collections;

public class MainMenuHud : MonoBehaviour {
	
	public GameObject CreditsPanel,HelpPanel,OptionsPanel,QuitButton,MenuParent,ContinueButton,NewGameMenu;
	GameDB GDB;

    void Start(){

		GDB=SharedSystemsMain.I.GDB;
		GDB.CheckForSaves();

        DisableAll();
#if UNITY_WEBPLAYER
        QuitButton.SetActive(false);
#endif

		if (!SharedSystemsMain.I.GDB.HasSave){
			ContinueButton.SetActive(false);
		}

		GDB.AllowEscHud=false;
    }

	void Update(){
		if (Input.GetKeyDown(KeyCode.Escape)){
			DisableAll();
		}
	}

	void NewGameClick(){
		ToggleOne(NewGameMenu);
	}
    
    void HelpClick(){
        ToggleOne(HelpPanel);
    }

	void BackClick(){
		DisableAll();
	}

	void ContinueClick(){
		SharedSystemsMain.I.GDB.LoadGame();
	}

	void OptionsClick(){
		ToggleOne(OptionsPanel);
	}

    void CreditsClick(){
        ToggleOne(CreditsPanel);
    }

    void QuitClick(){
        Application.Quit();
    }
	
    void DisableAll(){
        CreditsPanel.SetActive(false);
        HelpPanel.SetActive(false);
		OptionsPanel.SetActive(false);
		NewGameMenu.SetActive(false);

		MenuParent.SetActive(false);
    }

    void ToggleOne(GameObject target){
        bool active=target.activeSelf;
        DisableAll();
        target.SetActive(!active);
		MenuParent.SetActive(true);
    }

    void ToggleFullscreen(){
        Screen.fullScreen=!Screen.fullScreen;
    }
}
