using UnityEngine;
using System.Collections;

public class MainMenuHud : MonoBehaviour {
	
	public GameObject CreditsPanel,HelpPanel,QuitButton,FullScreenButton,MenuParent,NewGameMenu;
	public GameOptionsMenu OptionsPanel;
	GameDB GDB;
	public UIButton ContinueButton;

    void Start(){

		GDB=SharedSystemsMain.I.GDB;
		GDB.CheckForSaves();

        DisableAll();

#if UNITY_WEBPLAYER
        QuitButton.SetActive(false);
#endif

		if (!SharedSystemsMain.I.GDB.HasSave){
			//ContinueButton.SetActive(false);
			ContinueButton.defaultColor=new Color(1f,1f,1f,0.3f);
			ContinueButton.enabled=false;
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
		if (GDB.HasSave) GDB.LoadGame();
	}

	void OptionsClick(){
		ToggleOne(OptionsPanel.gameObject);
		OptionsPanel.OpenMenu();
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
		OptionsPanel.gameObject.SetActive(false);
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
