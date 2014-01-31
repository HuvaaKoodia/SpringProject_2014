using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public enum WeaponID
{
	LeftShoulder = 0,
	LeftHand = 1,
	RightShoulder = 2,
	RightHand = 3
}

public class PlayerMain : EntityMain
{
	public InvItemStorage items;
	public InvEquipmentStorage equipment;

	public PlayerInputSub inputSub;
	public PlayerTargetingSub targetingSub;

	public List<WeaponMain> gunList;
	public WeaponID currentGunID;
	
	public bool targetingMode { get; private set; }

	public bool INVINCIBLE=false;

	public int ap;
	const int apMax = 2;
	const int movementCost = 1;
    const int lootPickupCost = 1;
	const int attackCost = 2;

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
	}
	
	// Update is called once per frame
	void Update()
    {
		if (GC.currentTurn != TurnState.PlayerTurn)
			return;

		foreach (WeaponMain weapon in gunList)
		{
			if (weapon == GetCurrentWeapon() && targetingMode)
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
        if (ap < attackCost)
            return;

		ap -= attackCost;

		foreach(WeaponMain gun in gunList)
		{
			if (gun.HasTargets)
			{
				gun.Shoot();
			}
		}

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

	public override void TakeDamage(int damage)
	{
		if (INVINCIBLE) return;

		Health -= damage;

		GC.menuHandler.UpdateHealthText(Health);

        if (Health <= 0)
		{
			Debug.Log("Kuoli saatana");
            Health = 100;
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
		currentGunID = id;
		targetingSub.CheckGunTargets();
		GC.menuHandler.CheckTargetingModePanel();
	}

	public WeaponMain GetWeapon (WeaponID id)
	{
		return gunList[(int)id];
	}
	
	public WeaponMain GetCurrentWeapon()
	{
		return gunList[(int)currentGunID];
	}

	
}
