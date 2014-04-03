using UnityEngine;
using System.Collections;

public class MainMenuHud : MonoBehaviour {
	
    public GameObject CreditsPanel,HelpPanel,PlayPanel,QuitButton;

    void Start(){

        DisableAll();
#if UNITY_WEBPLAYER
        QuitButton.SetActive(false);
#endif

    }

    void PlayClick(){
        ToggleOne(PlayPanel);
    }

    void StartClick(){
        Application.LoadLevel("GameScene");
    }
    
    void HelpClick(){
        ToggleOne(HelpPanel);
    }

    void CreditsClick(){
        ToggleOne(CreditsPanel);
    }

    void QuitClick(){
        Application.Quit();
    }
	
    void DisableAll(){
        CreditsPanel.gameObject.SetActive(false);
        HelpPanel.gameObject.SetActive(false);
        PlayPanel.gameObject.SetActive(false);
    }

    void ToggleOne(GameObject target){
        bool active=target.activeSelf;
        DisableAll();
        target.SetActive(!active);
    }
    void ToggleFullscreen(){
        Screen.fullScreen=!Screen.fullScreen;
    }
}
