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

    SharedSystemsMain SS;
    MissionObjData Mission;

	// Use this for initialization
	void Start () {
        SS=GameObject.FindGameObjectWithTag("SharedSystems").GetComponent<SharedSystemsMain>();

        //Dev.temp
        if (!SS.GDB.GameStarted)
            SS.GDB.StartNewGame();

        //create mission buttons
        int i=0;
        foreach(var m in SS.GDB.GameData.AvailableMissions){
            var button=Instantiate(MButtonPrefab) as MissionButtonMain;
            button.transform.parent=MissionButtonsParent;

            button.transform.localScale=Vector3.one;
            button.transform.localPosition=Vector3.zero;
            button.transform.localPosition+=Vector3.right*((button.getWidth()+10)*i);
            ++i;

            button.Menu=this;
            button.SetMission(m);
        }

        //set references
        _VendorMenu.SetPlayer(SS.GDB.GameData.PlayerData);
		_VendorMenu.SetVendor(SS.GDB.GameData.VendorStore);
        _VendorMenu.Init();

		_MechanicMenu.SetPlayer(SS.GDB.GameData.PlayerData);
	
		_FinanceMenu.SetFinanceManager(SS.GDB.GameData.FinanceManager);

        //open correct menu

        if (SS.GDB.GOTO_DEBRIEF){
            SS.GDB.GOTO_DEBRIEF=false;
			MissionGenerator.UpdateMissionObjectiveStatus(SS.GDB.GameData.CurrentMission,SS.GDB.GameData.PlayerData);
            int reward=SS.GDB.CalculateQuestReward();
			SS.GDB.GameData.PlayerData.Money+=reward;
			MissionDebrief.SetMission(SS.GDB,reward);
            OpenMissionDebrief();
            SS.GDB.RemoveQuestItems();
        }
        else{
            OpenMissionSelect();
        }
	}
	
	// Update is called once per frame
	void Update () {
	    
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
        Tabs.ActivateMenu(_MechanicMenu.gameObject);
    }

	public void OpenFinance()
	{
		Tabs.ActivateMenu(_FinanceMenu.gameObject);
	}

    public void PlayMission(){
        SS.GDB.SetCurrentMission(Mission);
        SS.GDB.PlayMission();
    }
}
