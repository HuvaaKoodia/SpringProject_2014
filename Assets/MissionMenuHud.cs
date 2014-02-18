using UnityEngine;
using System.Collections;

public class MissionMenuHud : MonoBehaviour {

    public GameObject MissionMenu;
    public MissionBriefMain MissionBrief;
    public Transform MissionButtonsParent;
    SharedSystemsMain SS;
    public MissionButtonMain MButtonPrefab;

	// Use this for initialization
	void Start () {
	    
        SS=GameObject.FindGameObjectWithTag("SharedSystems").GetComponent<SharedSystemsMain>();

        //Dev.temp
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
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void SelectMission(MissionObjData mission)
    {
        MissionMenu.SetActive(false);
        MissionBrief.gameObject.SetActive(true);
        MissionBrief.SetMission(mission);
    }
}
