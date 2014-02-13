using UnityEngine;
using System.Collections;

using System.Collections.Generic;



public class PlayerMain : EntityMain
{
    public PlayerObjData ObjData{get;private set;}

	public PlayerInputSub inputSub;
	public PlayerTargetingSub targetingSub;
	public PlayerInteractSub interactSub;

	public List<WeaponMain> gunList;
	public WeaponID currentGunID;
	
	public bool targetingMode { get; private set; }

	public bool INVINCIBLE=false;

	public int ap;
	public const int apMax = 2;
	public const int movementCost = 1;
	public const int interactCost = 1;
	public const int attackCost = 2;
	public const int disperseHeatCost = 1;

	public const int DisperseHeatButtonMultiplier = 3;

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
		interactSub.CheckForInteractables();

        ActivateEquippedItems();
	}
	
	// Update is called once per frame
	void Update()
    {
		foreach (WeaponMain weapon in gunList)
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
		inputSub.enabled = false;
		EndTargetingMode();
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
		inputSub.enabled = false;
		ap -= movementCost;
	}

    public void Attack()
    {
		ap = 0;

		foreach(WeaponMain gun in gunList)
		{
			if (gun.HasTargets)
			{
				gun.Shoot();
			}
		}

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

        Health = ObjData.Equipment.UpperTorso.ObjData.HP;
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
                var w=gunList[i];
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
		targetingSub.CheckTargetableEnemies(Camera.main.transform.position);
		targetingSub.CheckGunSightToEnemies(GetCurrentWeapon().transform.position);
		GC.menuHandler.CheckTargetingModePanel();
		GC.menuHandler.gunInfoDisplay.ChangeCurrentHighlight(currentGunID);
		return true;
	}

	public void EndTargetingMode()
	{
		targetingMode = false;
		targetingSub.UnsightAllEnemies();
		GC.menuHandler.CheckTargetingModePanel();
	}

	public void DisperseWeaponHeat(int multiplier)
	{
		for (int i = 0; i < multiplier; i++)
		{
			foreach(WeaponMain gun in gunList)
			{
				gun.ReduceHeat();
			}
		}
		GC.menuHandler.gunInfoDisplay.UpdateAllDisplays();
	}

	public void ChangeWeapon(WeaponID id)
	{
        var weapon=gunList[(int)id];
        if (!weapon.Usable()) 
			return;

		GetCurrentWeapon().Unselected();
		currentGunID = id;
		targetingSub.CheckGunTargets();
		targetingSub.CheckGunSightToEnemies(GetCurrentWeapon().transform.position);
		GC.menuHandler.CheckTargetingModePanel();
		GC.menuHandler.gunInfoDisplay.ChangeCurrentHighlight(currentGunID);
		targetingSub.ShowTargetMarks(GetCurrentWeapon().Weapon != null);
	}

	public WeaponMain GetWeapon (WeaponID id)
	{
			return gunList[(int)id];
	}
	
	public WeaponMain GetCurrentWeapon()
	{
			return gunList[(int)currentGunID];
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
	
    public void ActivateEquippedItems()
    {
        //activate weapons
        ActivateEquipment(WeaponID.LeftHand,UIEquipmentSlot.Slot.WeaponLeftHand);
        ActivateEquipment(WeaponID.LeftShoulder,UIEquipmentSlot.Slot.WeaponLeftShoulder);
        ActivateEquipment(WeaponID.RightHand,UIEquipmentSlot.Slot.WeaponRightHand);
        ActivateEquipment(WeaponID.RightShoulder,UIEquipmentSlot.Slot.WeaponRightShoulder);
        
		if (GetCurrentWeapon().Usable())
			GC.menuHandler.gunInfoDisplay.ChangeCurrentHighlight(currentGunID);

		foreach(WeaponMain weapon in gunList)
		{
			weapon.gameObject.SetActive(weapon.Weapon != null);
		}
        //activate utilities
        //DEV.TODO
    }
}
