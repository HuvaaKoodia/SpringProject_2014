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
	const int apMax = 2;
	const int movementCost = 1;
    const int lootPickupCost = 1;
	const int attackCost = 2;

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
		GC.menuHandler.UpdateHealthText(maxHealth);

		interactSub.CheckForInteractables();

        ActivateEquippedItems();

		if (GetCurrentWeapon().Weapon != null)
			GC.menuHandler.gunInfoDisplay.ChangeCurrentHighlight(currentGunID);
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
		foreach(WeaponMain gun in gunList)
		{
			gun.ReduceHeat();
		}
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

    public void PickupLoot(LootCrateMain loot)
    {
        if (ap < lootPickupCost)
            return;

		GC.Inventory.SetLoot(loot);
		GC.Inventory.ToggleInventory();
		loot.Looted();
    }
    /// <summary>
    /// Inflicts damage from grid position x,y
    /// </summary>
	public void TakeDamage(int damage,int x,int y)
	{
		if (INVINCIBLE) return;

		//calculate correct attack direction
        int dir=0;
        int xo=x-movement.currentGridX,yo=y-movement.currentGridY;

        if (yo==0){
            dir=xo>0?dir=0:dir=2;
        }
        else{
            dir=yo>0?dir=1:dir=3;
        }
        //rotate according to player rotation
        var rot=Mathf.FloorToInt(transform.rotation.eulerAngles.y/90f);
        dir=(dir+rot)%3;


        Debug.Log("Damage from dir: "+dir+" rot "+rot);
        ObjData.TakeDMG(damage,dir);


        Health = ObjData.Equipment.UpperTorso.ObjData.HP;
        GC.menuHandler.UpdateHealthText(Health);

        if (Health <= 0)
		{
			Debug.Log("Kuoli saatana");
			GC.EngCont.Restart();
		}
	}

	public void StartTargetingMode()
	{
		targetingMode = true;
		targetingSub.CheckTargetableEnemies(Camera.main.transform.position);
		GC.menuHandler.CheckTargetingModePanel();
	}

	public void EndTargetingMode()
	{
		targetingMode = false;
		targetingSub.UnsightAllEnemies();
		GC.menuHandler.CheckTargetingModePanel();
	}

	public void ChangeWeapon(WeaponID id)
	{
		if (GetWeapon(id).Weapon == null || gunList[(int)id].Overheat)
			return;

		currentGunID = id;
		targetingSub.CheckGunTargets();
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
        
		if (GetCurrentWeapon().Weapon != null)
			GC.menuHandler.gunInfoDisplay.ChangeCurrentHighlight(currentGunID);

		foreach(WeaponMain weapon in gunList)
		{
			weapon.gameObject.SetActive(weapon.Weapon != null);
		}
        //activate utilities
        //DEV.TODO
    }
}
