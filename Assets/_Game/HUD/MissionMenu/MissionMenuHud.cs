using UnityEngine;
using System.Collections;

public class MissionMenuHud : MonoBehaviour {

    public GameObject MissionMenu;
    public MissionDebriefingMenu MissionDebrief;
    public MissionBriefingMenu MissionBrief;
    public Transform MissionButtonsParent;
    SharedSystemsMain SS;
    public MissionButtonMain MButtonPrefab;

    GameObject[] menus;

    MissionObjData Mission;

	// Use this for initialization
	void Start () {
	    
        menus=new GameObject[]{MissionMenu,MissionDebrief.gameObject,MissionBrief.gameObject};
        SS=GameObject.FindGameObjectWithTag("SharedSystems").GetComponent<SharedSystemsMain>();

        //Dev.temp
        if (!SS.GDB.GameStarted)
            SS.GDB.StartNewGame();

        int i=0;
        foreach(var m in SS.GDB.AvailableMissions){
            var button=Instantiate(MButtonPrefab) as MissionButtonMain;
            button.transform.parent=MissionButtonsParent;

            button.transform.localScale=Vector3.one;
            button.transform.localPosition=Vector3.zero;
            button.transform.localPosition+=Vector3.right*((button.getWidth()+10)*i);
            ++i;

            button.Menu=this;
            button.SetMission(m);
        }

        if (SS.GDB.GOTO_DEBRIEF){
            SS.GDB.GOTO_DEBRIEF=false;
            MissionDebrief.SetMission(SS.GDB,SS.XDB);
            OpenMissionDebrief();
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
        ActivateMenu(MissionBrief.gameObject);
    }

    public void OpenMissionDebrief(){
        ActivateMenu(MissionDebrief.gameObject);
    }

    public void OpenMissionSelect(){
        ActivateMenu(MissionMenu);
    }

    public void PlayMission(){
        SS.GDB.SetCurrentMission(Mission);
        SS.GDB.PlayMission();
    }

    void ActivateMenu(GameObject menu){
        foreach (var m in menus){
            m.SetActive(m==menu?true:false);
        }
    }
}
