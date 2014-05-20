using UnityEngine;
using System.Collections;

public class MissionMenuHud : MonoBehaviour {

    public GameObject MissionMenu;
    public MissionDebriefingMenu MissionDebrief;
    public MissionBriefingMenu MissionBrief;
    public VendorMenu _VendorMenu;
    public MechanicalMenu _MechanicMenu;
	public FinanceMenu _FinanceMenu;
    public Transform MissionButtonsParent;
    public MissionButtonMain MButtonPrefab;
    public MenuTabController Tabs;
	public UILabel MoneyLabel,GameSavedLabel,Daylabel;

	public int MissionButtonGap=8;

    SharedSystemsMain SS;
    MissionObjData Mission;
	PlayerObjData _player;

	// Use this for initialization
	void Start () {
        SS=SharedSystemsMain.I;

        //Dev.debug
        if (!SS.GDB.GameStarted)
            SS.GDB.CreateNewGame();

        //create mission buttons
        int i=0;
        foreach(var m in SS.GDB.GameData.AvailableMissions){
            var button=Instantiate(MButtonPrefab) as MissionButtonMain;
            button.transform.parent=MissionButtonsParent;

            button.transform.localScale=Vector3.one;
            button.transform.localPosition=Vector3.zero;
            button.transform.localPosition+=Vector3.right*((button.getWidth()+MissionButtonGap)*i);
            ++i;

            button.Menu=this;
            button.SetMission(m);
        }

		_player=SS.GDB.GameData.PlayerData;

        //set references
        _VendorMenu.SetPlayer(_player);
		_VendorMenu.SetVendor(SS.GDB.GameData.VendorStore);
        _VendorMenu.Init();

		_MechanicMenu.SetPlayer(_player);
	
		_FinanceMenu.SetFinanceManager(SS.GDB.GameData.FinanceManager);

        //open correct menu
        if (SS.GDB.GOTO_DEBRIEF){
            SS.GDB.GOTO_DEBRIEF=false;
			MissionDebrief.SetMission(SS.GDB);
            OpenMissionDebrief();
        }
        else{
            OpenMissionSelect();
        }

		if (SS.GDB.GameLoaded){
			ShowSaveGameLabel("GAME LOADED");
			SS.GDB.GameLoaded=true;
		}
		else{
			SS.GDB.SaveGame();
			ShowSaveGameLabel("GAME SAVED");
		}

		Daylabel.text="Day: "+SS.GDB.GameData.CurrentTime;

		SS.GDB.AllowEscHud=true;
	}
	
	void Update (){
		MoneyLabel.text="Money: "+_player.Money+" "+XmlDatabase.MoneyUnit;
	}

    public void SelectMission(MissionObjData mission)
    {
        Mission=mission;
        MissionBrief.SetMission(mission);

        OpenMissionBrief();
    }

    public void OpenMissionBrief(){
        Tabs.ActivateMenu(MissionBrief.gameObject);
    }

    public void OpenMissionDebrief(){
        Tabs. ActivateMenu(MissionDebrief.gameObject);
    }

    public void OpenMissionSelect(){
        Tabs.ActivateMenu(MissionMenu);
    }

    public void OpenVendor(){
        Tabs.ActivateMenu(_VendorMenu.gameObject);
    }

    public void OpenMechanic(){
		StartCoroutine(CallUpdateAfterOneUpdateStepBecauseUnitySetActiveShenanigans());
		Tabs.ActivateMenu(_MechanicMenu.gameObject);
    }

	IEnumerator CallUpdateAfterOneUpdateStepBecauseUnitySetActiveShenanigans(){
		yield return null;
		_MechanicMenu.OpenPanel();
	}

	public void OpenFinance()
	{
		Tabs.ActivateMenu(_FinanceMenu.gameObject);
	}

    public void PlayMission(){
        SS.GDB.SetCurrentMission(Mission);
        SS.GDB.PlayMission();
    }

	
	void ShowSaveGameLabel(string text){
		GameSavedLabel.text=text;
		GameSavedLabel.gameObject.SetActive(true);
		Invoke("HideGameSavedLabel",3f);
	}
	
	void HideGameSavedLabel(){
		GameSavedLabel.gameObject.SetActive(false);
	}
}
