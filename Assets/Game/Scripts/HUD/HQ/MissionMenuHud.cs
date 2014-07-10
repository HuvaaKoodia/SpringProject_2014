using UnityEngine;
using System.Collections;

public class MissionMenuHud : MonoBehaviour {

    public GameObject MissionMenu;
    public MissionDebriefingMenu MissionDebrief;
    public MissionBriefingMenu MissionBrief;
    public VendorMenu _VendorMenu;
	public Instantiate_MechanicalMenu mm_init;
	MechanicalMenu _MechanicMenu;
	public FinanceMenu _FinanceMenu;
    public Transform MissionButtonsParent;
    public MissionButtonMain MButtonPrefab;
    public MenuTabController Tabs;
	public UILabel MoneyLabel,Daylabel;

	public VictoryMenu victoryMenu;
	public OutOfMoneyMenu outofmoney;

	public int MissionButtonGap=8;

	[SerializeField] GameObject InputBlocker;
	[SerializeField] GameObject GameStartText,TabButtons;

    SharedSystemsMain SS;
    MissionObjData Mission;
	PlayerObjData _player;
	
	public AudioSource RandomSoundPlayer;
	public bool GotoMoneyWarningMenu;

	// Use this for initialization
	void Start () {
        SS=SharedSystemsMain.I;

		SS.MusicSystem.StopCurrent();

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

		_MechanicMenu=mm_init.Instance;
		_MechanicMenu.SetPlayer(_player);
		_MechanicMenu.gameObject.SetActive(false);
	
		_FinanceMenu.SetFinanceManager(SS.GDB.GameData.FinanceManager);

        //open correct menus
		CheckMoneyAmount();

		
		if (SS.GDB.GameData.FirstTimeInMissionMenu)
		{
			GameStartText.SetActive(true);
			TabButtons.SetActive(false);
			SS.GDB.GameData.FirstTimeInMissionMenu=false;
		}
		else
		{
			if (SS.GDB.GOTO_DEBRIEF){
	            SS.GDB.GOTO_DEBRIEF=false;
				MissionDebrief.SetMission(SS.GDB);
	            OpenMissionDebrief();
	        }
	        else{
				SetStartUpMenus();
	        }
		}

		//save load label
		if (SS.GDB.GameLoaded){
			SS.GDB.GameLoaded=false;
			SS.EscHud.ShowGameLoadedLabel();
		}
		else{
			SS.EscHud.ShowGameSavedLabel();

			//autosave
			SS.GDB.SaveGame();
		}

		Daylabel.text="Day: "+SS.GDB.GameData.CurrentTime;

		_FinanceMenu.OnDebtShorten+=OnFinancePayment;
		SS.GDB.EscHudShowEnabled=true;
		SS.GDB.EscHudShowSaveButton=true;
	}

	//lazy public
	public void CheckMoneyAmount(){
		GotoMoneyWarningMenu=SS.GDB.GameData.PlayerData.Money<0;
	}
	
	void Update (){
		MoneyLabel.text="Money: "+_player.Money+" "+XmlDatabase.MoneyUnit;

		if (Input.GetButtonDown("Toggle Inventory"))
		{
			OpenVendor();
		}
		if (Input.GetButtonDown("Toggle Logs"))
		{
			OpenMissionSelect();
		}
		if (Input.GetButtonDown("Toggle Status"))
		{
			OpenMechanic();
		}
		
		if (Input.GetButtonDown("Toggle Finances"))
		{
			OpenFinance();
		}
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
		if (lock_input) return;
		if (GotoMoneyWarningMenu){
			Tabs.ActivateMenu(outofmoney.gameObject);
			outofmoney.OpenMenu(this);
		}
		else{
        	Tabs.ActivateMenu(MissionMenu);
		}
	}

    public void OpenVendor(){
		if (lock_input) return;
        Tabs.ActivateMenu(_VendorMenu.gameObject);
    }

    public void OpenMechanic(){
		if (lock_input) return;
		Tabs.ActivateMenu(mm_init.gameObject);
		_MechanicMenu.OpenPanel();
    }

	public void OpenFinance()
	{
		if (lock_input) return;
		Tabs.ActivateMenu(_FinanceMenu.gameObject);
		_FinanceMenu.OpenMenu();
	}

    public void PlayMission()
	{
		InputBlocker.SetActive(true);
		EscHudMain.I.FadeIn();
		StartCoroutine(StartAfterFade());
    }

	IEnumerator StartAfterFade()
	{
		while(EscHudMain.I.FadeInProgress) yield return null;

		SS.GDB.SetCurrentMission(Mission);
		SS.GDB.PlayMission();
		MissionBrief.ShowLoadingLabel();
	}

	//victory menu imp
	void OnFinancePayment()
	{
		if (!_FinanceMenu._FinanceManager.HasActiveDebts())
		{
			//victory
			lock_input=true;
			//play sound
			RandomSoundPlayer.Play();
			//wait for a bit
			Invoke("OpenVictoryMenu",3f);
		}
	}

	public bool lock_input=false;

	void OpenVictoryMenu(){
		Tabs.ActivateMenu(victoryMenu.gameObject);
		victoryMenu.OpenMenu(this);
	}

	//start up

	public void OnStartButtonPressed(){
		SetStartUpMenus();
	}

	void SetStartUpMenus()
	{
		GameStartText.SetActive(false);
		TabButtons.SetActive(true);

		OpenMissionSelect();
	}

}
