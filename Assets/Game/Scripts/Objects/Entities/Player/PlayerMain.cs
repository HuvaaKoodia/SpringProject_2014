using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class PlayerMain : EntityMain
{
    public PlayerObjData ObjData{get;private set;}

	public PlayerHud HUD;

	public PlayerInputSub inputSub;
	public PlayerTargetingSub targetingSub;
	public PlayerInteractSub interactSub;

	public List<WeaponMain> weaponList;
	public WeaponID currentWeaponID;
	
	public bool targetingMode { get; private set; }

	public bool ShotLastTurn = false;
	public bool Finished = false;

	public bool INVINCIBLE=false;

	public int ap;
	public const int apMax = 2;
	public const int movementCost = 1;
	public const int attackCost = 2;
	public const int disperseHeatCost = 1;

	public Camera GameCamera;
	public Camera HudCamera;
	public Camera TargetingCamera;
	public Camera PlayerCamera;

	public GameObject Flashlight;

	public Animation playerAnimation;
	public bool AnimationsOn;

	public bool WeaponMouseLookOn = false;
	public bool Shooting = false;
	
	int gunsFinishedShooting;

	public List<ParticleSystem> DisperseHeatParkikkels;

	public AudioClip TakeDamageFX;

	public Animation buttonAnimation;

	public bool SystemOverheat {
		get{return ObjData.UpperTorso.OVERHEAT;}
	}
	
	public GameObject ParticleTempParent;

    public void SetObjData(PlayerObjData data){
        ObjData=data;
    }

	// Use this for initialization
	public override void Awake()
	{
		base.Awake();
		
		GC = GameObject.Find("GameSystems").GetComponent<GameController>();

		inputSub = GetComponent<PlayerInputSub>();

		targetingMode = false;
	}

	public void InitPlayer()
    {
		ap = apMax;
		HUD.ShowApBlips(ap);

		ShotLastTurn = false;
		Finished = false;
		movement.UpdateFloor();
		interactSub.CheckForInteractables();

        for (int i=0;i<4;++i){
            weaponList[i].weaponID=(WeaponID)i;
        }

		GameCamera.GetComponent<MouseLook>().SetOriginalRot(GameCamera.transform.localRotation);
		
		HUD.Init(this,GC);
		
		TargetingCamera.aspect = 16.0f / 9.0f;
	}
	
	void Update()
    {
        foreach (WeaponMain weapon in weaponList)
		{
			//Mouse look was taken off by default when 2 pivot point rotation was implemented
			//weapon graphics caused clipping and 2 pivot points weird vibration when moving mouse fast
			//or around floor/ceiling

			if (WeaponMouseLookOn && targetingMode && !weapon.HasTargets && weapon == GetCurrentWeapon())
				weapon.LookAtMouse(targetingSub.TargetingArea);
			else
				weapon.RotateGraphics();
		}
	}

    public void StartPlayerPhase()
    {
		ap = apMax;
		HUD.ShowApBlips(ap);

		Finished = false;
		DecreaseWeaponHeat();

		MovedLastPhase = false;

		HUD.UpdateHudPanels();
        StartTurn();
    }

    void StartTurn()
    {
        inputSub.enabled = true;
        interactSub.CheckForInteractables();
    }

    public void EndPlayerPhase()
    {
		Finished = true;
		inputSub.enabled = false;

		HUD.ShowApBlips(0);
	
		EndTargetingMode();

		if (!interactSub.WaitingInteractToFinish)
			GC.ChangeTurn(TurnState.StartAITurn);
    }

    public override void FinishedMoving(bool wontMoveAnymore)
	{
		interactSub.CheckForInteractables();

        if (ap <= 0)
            EndPlayerPhase();
        else
            StartTurn();
	}

	public void StartedMoving()
	{
		if (transform.position!=movement.targetPosition){
			GC.CullWorldBasedOnPlayer(true);
		}

		inputSub.enabled = false;
		ap -= movementCost;
		HUD.ShowApBlips(ap);

		if (AnimationsOn && movement.currentMovement == MovementState.Moving)
		{	
			playerAnimation.Play("Walk");
		}
	}

    public IEnumerator Attack()
    {
		if (Shooting) yield break;

		ap = 0;
		HUD.ShowApBlips(ap);

		gunsFinishedShooting = 0;
		Shooting = true;

        foreach(WeaponMain weapon in weaponList)
		{
            if (weapon.HasTargets)
			{
				StartCoroutine(weapon.Shoot());
			}
			else
			{
				GunFinishedShooting();
			}
		}

		ShotLastTurn = true;

		HUD.UpdateHudPanels();

		while (gunsFinishedShooting < weaponList.Count)
		{
			yield return null;
		}

		Shooting = false;
		
		HUD.DeactivateTargetingHUD();
		HUD.UpdateHudPanels();

		EndPlayerPhase();
	}

    /// <summary>
    /// Inflicts damage from grid position x,y
    /// </summary>
	public void TakeDamage(int damage,int x,int y)
	{
		if (INVINCIBLE||Dead) return;

		//calculate correct attack direction
        int dir=0;
        int xo=x-movement.currentGridX,yo=y-movement.currentGridY;

        if (yo==0){ dir=xo>0?dir=0:dir=2;}
        else{ dir=yo>0?dir=1:dir=3;}
        //rotate according to player rotation
        var rot=Mathf.FloorToInt(transform.rotation.eulerAngles.y/90f);
        dir=(dir+rot)%4;

        //Debug.Log("Damage from dir: "+dir+" rot "+rot);
        ObjData.TakeDMG(damage,dir);

        Health = ObjData.UpperTorso.HP;

		switch(dir)
		{
		case 0:
			playerAnimation["Damage_Right"].normalizedTime = 0;
			playerAnimation["Damage_Right"].speed = 1;
			playerAnimation.Blend("Damage_Right");
			break;
		case 1:
			playerAnimation["Damage_Front"].normalizedTime = 0;
			playerAnimation["Damage_Front"].speed = 1;
			playerAnimation.Blend("Damage_Front");
			break;
		case 2:
			playerAnimation["Damage_Left"].normalizedTime = 0;
			playerAnimation["Damage_Left"].speed = 1;
			playerAnimation.Blend("Damage_Left");
			break;
		case 3:
			playerAnimation["Damage_Back"].normalizedTime = 0;
			playerAnimation["Damage_Back"].speed = 1;
			playerAnimation.Blend("Damage_Back");
			break;
		}

        if (Health <= 0)
		{
			Dead=true;
			inputSub.DISABLE_INPUT=true;
			GC.EndGame();

		}

		if (TakeDamageFX != null)
		{
			audio.PlayOneShot(TakeDamageFX);
		}

		HUD.UpdateHudPanels();
	}
	bool Dead=false;

	public bool StartTargetingMode()
	{
        //check for usable weapons
        if (!GetCurrentWeapon().Usable()){
			bool foundWeapon = false;
            for(int i=0;i<4;i++){
                var w=weaponList[i];
                if (w.Usable()){
                    ChangeWeapon((WeaponID)i);
					foundWeapon = true;
                    break;
                }
            }

			if (!foundWeapon)
				return false;
        }

		targetingMode = true;
		targetingSub.CheckTargetableEnemies();
		targetingSub.CheckGunSightToEnemies(GetCurrentWeapon().transform.position);
		HUD.CheckTargetingModePanel();
		HUD.gunInfoDisplay.ChangeCurrentHighlight(currentWeaponID);
		return true;
	}

	public void EndTargetingMode()
	{
		targetingMode = false;
		targetingSub.UnsightAllEnemies();
		HUD.CheckTargetingModePanel();
	}

	public void DisperseWeaponHeat()
	{
		for (int i = 0; i < DisperseHeatParkikkels.Count; i++)
		{
			DisperseHeatParkikkels[i].Play();
		}

		for (int i = 0; i < weaponList.Count; i++)
		{
			weaponList[i].DisperseHeat();
		}
		
		ObjData.UpperTorso.AddHEAT(-(XmlDatabase.HullHeatDisperseConstant+ObjData.UpperTorso.HEAT*XmlDatabase.HullHeatDisperseHeatMultiplier));
		HUD.gunInfoDisplay.UpdateAllDisplays();
	}

	public void DecreaseWeaponHeat()
	{
		foreach(WeaponMain gun in weaponList)
		{
			gun.ReduceHeat();
		}
		
		ObjData.UpperTorso.AddHEAT(-XmlDatabase.HullHeatDisperseConstant);
		HUD.gunInfoDisplay.UpdateAllDisplays();
	}

	public void ChangeWeapon(WeaponID id)
	{
        var weapon=weaponList[(int)id];
        if (!weapon.Usable() || currentWeaponID == id) 
			return;

		GetCurrentWeapon().Unselected();
		currentWeaponID = id;
		targetingSub.CheckGunTargets();
		targetingSub.CheckGunSightToEnemies(GetCurrentWeapon().transform.position);
		HUD.CheckTargetingModePanel();
		HUD.gunInfoDisplay.ChangeCurrentHighlight(currentWeaponID);
		targetingSub.ShowTargetMarks(GetCurrentWeapon().Weapon != null);
	}

	public WeaponMain GetWeapon (WeaponID id)
	{
        return weaponList[(int)id];
	}
	
	public WeaponMain GetCurrentWeapon()
	{
        return weaponList[(int)currentWeaponID];
	}

    /// <summary>
    /// Sets the equipped GameItem weapon to the corresponding WeaponMain for game usage.
    /// </summary>
    public void ActivateEquipment(WeaponID id,UIEquipmentSlot.Slot slot){
        var weapon=GetWeapon(id);
        var Slot=ObjData.Equipment.GetSlot(slot);
        weapon.SetWeapon(Slot);

		HUD.gunInfoDisplay.SetWeaponToDisplay(id, weapon);
    }

	public bool HasRadar{get;private set;}
	public bool HasMap{get;private set;}
	public int RadarRange{get;private set;}
	public float RadarRangeMax{get;private set;}

    public void ActivateEquippedItems()
    {
        //activate weapons
        ActivateEquipment(WeaponID.LeftHand,UIEquipmentSlot.Slot.WeaponLeftHand);
        ActivateEquipment(WeaponID.LeftShoulder,UIEquipmentSlot.Slot.WeaponLeftShoulder);
        ActivateEquipment(WeaponID.RightHand,UIEquipmentSlot.Slot.WeaponRightHand);
        ActivateEquipment(WeaponID.RightShoulder,UIEquipmentSlot.Slot.WeaponRightShoulder);
        
		if (!GetCurrentWeapon().Usable())
		{
			for (int i = 0; i < 4; i++)
			{
				if (GetWeapon((WeaponID)i).Usable())
					ChangeWeapon((WeaponID)i);
			}
		}

		HUD.gunInfoDisplay.ChangeCurrentHighlight(currentWeaponID);

		foreach(WeaponMain weapon in weaponList)
		{
			if (weapon.graphics != null)
				weapon.graphics.SetActive(weapon.Weapon != null);
		}
        //activate utilities
		HasRadar=false;
		HUD.radar.radarViewSprite.enabled = false;
		HasMap=false;
		RadarRange=0;
		int overheat_limit=0;
		float accu_multi=0,def_multi=0,melee_multi=0,cooling_multi=0;
		foreach (var s in ObjData.Equipment.EquipmentSlots){
			if (s.Item==null||!s.ObjData.USABLE) continue;
			if (s.Item.baseItem.type==InvBaseItem.Type.Utility||
			    s.Item.baseItem.type==InvBaseItem.Type.Navigator||
			    s.Item.baseItem.type==InvBaseItem.Type.Radar){

				int value;
				if (GetStatValue(s.Item,InvStat.Type.AccuracyBoost,out value)){
					accu_multi+=value;
				}
				else if (GetStatValue(s.Item,InvStat.Type.HullArmor,out value)){
					def_multi+=value;
				}
				else if (GetStatValue(s.Item,InvStat.Type.HullOverheatLimit,out value)){
					overheat_limit+=value;
				}
				else if (GetStatValue(s.Item,InvStat.Type.MeleeDamage,out value)){
					melee_multi+=value;
				}
				else if (GetStatValue(s.Item,InvStat.Type.RadarRange,out value)){
					if (value>RadarRange) RadarRange=value;
				}
				else if (GetStatValue(s.Item,InvStat.Type.SystemCooling,out value)){
					cooling_multi+=value;
				}
				if (s.Item.baseItem.type==InvBaseItem.Type.Navigator){
					HasMap=true;
				}
				else if (s.Item.baseItem.type==InvBaseItem.Type.Radar){
					HasRadar=true;
					//RadarRangeMax=s.Item.baseItem.GetStat(InvStat.Type.RadarRange).max_amount;
				}
			}
		}

		if (HasRadar && !HasMap)
		{
			HUD.radar.radarViewSprite.enabled = true;
		}

		ObjData.UpperTorso.armor_multi=def_multi*0.01f;
		ObjData.UpperTorso.Overheat_limit_bonus=overheat_limit;

		foreach(var w in ObjData.MechParts){
			if (w.IsWeapon){
				w.cooling_multi=cooling_multi*0.01f;
				w.accuracy_multi=accu_multi;

				if (w.Equipment.Item!=null){
					if (w.Equipment.Item.baseItem.type==InvBaseItem.Type.MeleeWeapon) w.attack_multi=melee_multi*0.01f;
				}
			}
		}
    }

	private bool GetStatValue(InvGameItem item,InvStat.Type type,out int value){
		var stat=item.GetStat(type);
		value=0;
		if (stat==null) return false;
		value=stat._amount;
		return true;
	}

	protected override void UpdateFloor(){
		base.UpdateFloor();
	}

	public void SetMouseLook(bool look)
	{
		GameCamera.GetComponent<MouseLook>().MouseLookOn = look;
	}

	public void ToggleMouseLook()
	{
		GameCamera.GetComponent<MouseLook>().ToggleMouseLookOn();
	}

	public void ActivateHaxMapNRadar ()
	{
		HasMap = true;
		HasRadar = true;
	}

	public void GunFinishedShooting()
	{
		gunsFinishedShooting++;
	}

	public void ToggleFlashlight()
	{
		Flashlight.SetActive(!Flashlight.activeSelf);
	}
}