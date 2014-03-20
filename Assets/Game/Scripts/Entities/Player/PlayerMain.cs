using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class PlayerMain : EntityMain
{
    public PlayerObjData ObjData{get;private set;}

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

	void Start()
    {
		ap = apMax;
		ShotLastTurn = false;
		Finished = false;
		movement.UpdateFloor();
		interactSub.CheckForInteractables();

        ActivateEquippedItems();

        for (int i=0;i<4;++i){
            weaponList[i].weaponID=(WeaponID)i;
        }

		CullWorld();
	}
	
	// Update is called once per frame
	void Update()
    {
        foreach (WeaponMain weapon in weaponList)
		{
			if (targetingMode && !weapon.HasTargets && weapon == GetCurrentWeapon())
				weapon.LookAtMouse(targetingSub.TargetingArea);
			else
				weapon.RotateGraphics();
		}
	}

    public void StartPlayerPhase()
    {
		ap = apMax;
		Finished = false;
		DisperseWeaponHeat(1);

		GC.menuHandler.gunInfoDisplay.UpdateAllDisplays();
        StartTurn();
    }

    void StartTurn()
    {
        inputSub.enabled = true;
    }

    public void EndPlayerPhase()
    {
		Finished = true;
		inputSub.enabled = false;
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
			CullWorld();
		}

		inputSub.enabled = false;
		ap -= movementCost;
	}

	public void CullWorld(){
		GC.CullWorld(transform.position,movement.targetPosition,GameCamera.farClipPlane);
	}

    public void Attack()
    {
		ap = 0;

        foreach(WeaponMain weapon in weaponList)
		{
            if (weapon.HasTargets)
			{
                weapon.Shoot();
			}
		}

		ShotLastTurn = true;
		GC.menuHandler.gunInfoDisplay.UpdateAllDisplays();

		if (ap == 0)
			EndPlayerPhase();
	}
	/*
    public void PickupLoot(LootCrateMain loot)
    {
        if (ap < lootPickupCost)
            return;

		GC.Inventory.SetLoot(loot);
		GC.Inventory.ToggleInventory();
		loot.Looted();
    }
    */

    /// <summary>
    /// Inflicts damage from grid position x,y
    /// </summary>
	public void TakeDamage(int damage,int x,int y)
	{
		if (INVINCIBLE) return;

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
        if (Health <= 0)
		{
			Debug.Log("Kuoli saatana");
			GC.EngCont.Restart();
		}
	}

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
		GC.menuHandler.CheckTargetingModePanel();
		GC.menuHandler.gunInfoDisplay.ChangeCurrentHighlight(currentWeaponID);
		return true;
	}

	public void EndTargetingMode()
	{
		targetingMode = false;
		targetingSub.UnsightAllEnemies();
		GC.menuHandler.CheckTargetingModePanel();
	}

	public void DisperseWeaponHeat(float multiplier)
	{

        foreach(WeaponMain gun in weaponList)
		{
            gun.ReduceHeat(multiplier);
		}
		
		ObjData.UpperTorso.AddHEAT(-(XmlDatabase.HullHeatDisperseConstant+XmlDatabase.HullHeatDisperseHeatMultiplier*ObjData.UpperTorso.HEAT)*multiplier);

		GC.menuHandler.gunInfoDisplay.UpdateAllDisplays();
	}

	public void ChangeWeapon(WeaponID id)
	{
        var weapon=weaponList[(int)id];
        if (!weapon.Usable()) 
			return;

		GetCurrentWeapon().Unselected();
		currentWeaponID = id;
		targetingSub.CheckGunTargets();
		targetingSub.CheckGunSightToEnemies(GetCurrentWeapon().transform.position);
		GC.menuHandler.CheckTargetingModePanel();
		GC.menuHandler.gunInfoDisplay.ChangeCurrentHighlight(currentWeaponID);
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

		GC.menuHandler.gunInfoDisplay.SetWeaponToDisplay(id, weapon);
    }

	public bool HasRadar{get;private set;}
	public bool HasMap{get;private set;}
	public int RadarRange{get;private set;}

    public void ActivateEquippedItems()
    {
        //activate weapons
        ActivateEquipment(WeaponID.LeftHand,UIEquipmentSlot.Slot.WeaponLeftHand);
        ActivateEquipment(WeaponID.LeftShoulder,UIEquipmentSlot.Slot.WeaponLeftShoulder);
        ActivateEquipment(WeaponID.RightHand,UIEquipmentSlot.Slot.WeaponRightHand);
        ActivateEquipment(WeaponID.RightShoulder,UIEquipmentSlot.Slot.WeaponRightShoulder);
        
		if (GetCurrentWeapon().Usable())
			GC.menuHandler.gunInfoDisplay.ChangeCurrentHighlight(currentWeaponID);

		foreach(WeaponMain weapon in weaponList)
		{
			weapon.gameObject.SetActive(weapon.Weapon != null);
		}
        //activate utilities
		HasRadar=false;
		HasMap=false;
		RadarRange=0;
		int overheat_limit=0;
		float accu_multi=0,def_multi=0,melee_multi=0,cooling_multi=0;
		foreach (var s in ObjData.Equipment.EquipmentSlots){
			if (s.Item==null) continue;
			if (s.Item.baseItem.type==InvBaseItem.Type.Utility){
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
			}
			else if (s.Item.baseItem.type==InvBaseItem.Type.Navigator){
				HasMap=true;
			}
			else if (s.Item.baseItem.type==InvBaseItem.Type.Radar){
				HasRadar=true;
			}
		}

		ObjData.UpperTorso.armor_multi=def_multi;
		ObjData.UpperTorso.Overheat_limit_bonus=overheat_limit;

		foreach(var w in ObjData.MechParts){
			if (w.IsWeapon){
				w.cooling_multi=cooling_multi*0.01f;
				w.accuracy_multi=accu_multi*0.01f;

				if (w.Equipment.Item.baseItem.type==InvBaseItem.Type.MeleeWeapon)
					w.attack_multi=melee_multi*0.01f;
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
		//targetingSub.Update
	}
}