﻿using UnityEngine;

using System.Linq;

using System.Collections;
using System.Collections.Generic;

public enum WeaponID
{
    LeftHand,
	LeftShoulder,
    RightHand,
	RightShoulder
}

public class WeaponMain : MonoBehaviour {

	public PlayerMain player;
	public WeaponID weaponID;

	public InvGameItem Weapon{get;private set;}
    public InvEquipmentSlot WeaponSlot{get;private set;}

	public string GunName { get;protected set;}

	public int MinDamage { get; protected set;}
	public int MaxDamage { get; protected set;}

    public int MaxAmmo { get; protected set;}
	public int RateOfFire { get; protected set;}
	public int Accuracy { get; protected set;}
    public int Range { get; protected set;}

	public int HeatGeneration { get; protected set;}
	public int CoolingRate { get; protected set;}
	
    public int CurrentAmmo { 
        get{
            if (NoAmmoConsumption) return 1;
            return player.ObjData.GetAmmoAmount(Weapon.baseItem.ammotype);
        }
        protected set{
            if (!NoAmmoConsumption){
                player.ObjData.SetAmmoAmount(Weapon.baseItem.ammotype,value);
            }
        }
    }
    public int CurrentHeat { get{return WeaponSlot.ObjData.HEAT;}}

    public bool Overheat{get{return WeaponSlot.ObjData.OVERHEAT;}}

	public Dictionary<EnemyMain, int> targets { get; private set;}

	public Quaternion targetRotation;
	public float rotationSpeed;

    public bool NoAmmoConsumption{get;private set;}

    public bool Usable(){
        return Weapon!=null&&!Overheat&&CurrentAmmo>0;
    }

	public void SetWeapon(InvEquipmentSlot slot){
        WeaponSlot=slot;

        if (Weapon==slot.Item) return;
        Weapon=slot.Item;

        if (Weapon==null) return;

		GunName=Weapon.baseItem.name;
	    
		var dam=WeaponSlot.ObjData.GetDamage();
		MinDamage = dam;
		MaxDamage = dam;
		
        RateOfFire = Weapon.GetStat(InvStat.Type.Firerate)._amount;
        Accuracy = Weapon.GetStat(InvStat.Type.Accuracy)._amount;
        Range = Weapon.GetStat(InvStat.Type.Range)._amount;

        HeatGeneration = Weapon.GetStat(InvStat.Type.Heat)._amount;
        CoolingRate = Weapon.GetStat(InvStat.Type.Cooling)._amount;
		
        MaxAmmo = player.ObjData.GetAmmoData(Weapon.baseItem.ammotype).MaxAmount;
        if(MaxAmmo==-1){
            NoAmmoConsumption=true;
        }
        else{
            NoAmmoConsumption=false;
        }
	}

	public bool HasTargets
	{
		get { return targets.Count > 0; }
	}

	// Use this for initialization
	void Awake () {
		rotationSpeed = 50;
		targetRotation = Quaternion.identity;
		targets = new Dictionary<EnemyMain, int>();
	}
	
	// Update is called once per frame
	void Update () {
	}

    public virtual void TargetEnemy(EnemyMain enemy,bool increase_amount)
	{
        if (increase_amount)
		{
			IncreaseShotsAtTarget(enemy);
		}
        else{
			DecreaseShotsAtTarget(enemy);
        }
		SetTargetRotation();
	}

	protected void IncreaseShotsAtTarget(EnemyMain enemy)
	{
        if (targets.ContainsKey(enemy)){
            if (GetNumShotsTargetedTotal() < RateOfFire)
                targets[enemy]++;
            else
                RemoveTarget(enemy);
        }
        else{
            if (GetNumShotsTargetedTotal() < RateOfFire){
                targets.Add(enemy, 0);
                targets[enemy]++;
            }
        }
	}

	protected void DecreaseShotsAtTarget(EnemyMain enemy)
	{
        if (targets.ContainsKey(enemy)){

            if (targets[enemy]>1){
                targets[enemy]--;
            }
            else
                RemoveTarget(enemy);
        }
        else{
            if (GetNumShotsTargetedTotal() < RateOfFire){
                targets.Add(enemy, 0);
                targets[enemy]=RateOfFire-GetNumShotsTargetedTotal();
            }
        }
	}

	protected void RemoveTarget(EnemyMain enemy)
	{
		targets.Remove(enemy);
	}

	public void ClearTargets()
	{
		targets.Clear();
	}

	public void Shoot()
	{
		foreach(KeyValuePair<EnemyMain, int> enemyPair in targets)
		{
			//shoot all shots at one enemy consecutively
			for (int i = 0; i < enemyPair.Value; i++)
			{
                if (Overheat || CurrentAmmo == 0) return;

                if (!NoAmmoConsumption) CurrentAmmo--;
                if (CurrentAmmo<0) CurrentAmmo=0;

                IncreaseHeat(1);

				if (Accuracy > Subs.RandomPercent())
				{
                    //hit
					int dmg = (int)Random.Range(MinDamage, MaxDamage);
					enemyPair.Key.TakeDamage(dmg);
				}
			}
		}
	}

    public void IncreaseHeat(float multi)
    {
		if (WeaponSlot != null)
        	WeaponSlot.ObjData.IncreaseHEAT(Weapon,multi);
    }

	public void ReduceHeat(float multi)
	{
		if (WeaponSlot != null)
       		WeaponSlot.ObjData.ReduceHEAT(Weapon,multi);
	}

	public void AddAmmo(int amount)
	{
        if (NoAmmoConsumption) return;
		CurrentAmmo = Mathf.Min(CurrentAmmo+amount, MaxAmmo);
	}

	public int GetNumShotsTargetedTotal()
	{
		int numShots = 0; 
		foreach(KeyValuePair<EnemyMain, int> enemyPair in targets)
			numShots += enemyPair.Value;

		return numShots;
	}

	public int GetNumShotsAtTarget(EnemyMain enemy)
	{
		if (targets.ContainsKey(enemy))
			return targets[enemy];
		else
			return 0;
	}

	public void RotateGraphics()
	{
		if (HasTargets)
		{
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime*rotationSpeed);
		}
		else
		{
			transform.rotation = Quaternion.RotateTowards(transform.rotation, player.transform.rotation, Time.deltaTime*rotationSpeed);
		}
	}

	public void LookAtMouse(Rect bounds)
	{
		if (bounds.Contains(new Vector2(Input.mousePosition.x, Input.mousePosition.y)))
		{
			Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit info;

			float mouseDistance = 10;
			if (Physics.Raycast(ray, out info))
			{
				mouseDistance = info.distance;
			}

			Vector3 mouseToWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x , Input.mousePosition.y, mouseDistance));

			targetRotation = Quaternion.LookRotation((mouseToWorld - transform.position));
		}
		else
			targetRotation = transform.rotation = 
				Quaternion.RotateTowards(transform.rotation, player.transform.rotation, Time.deltaTime*rotationSpeed);

		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime*rotationSpeed);
	}
	
	public void Unselected()
	{
		SetTargetRotation();
	}

	void SetTargetRotation()
	{
		if (HasTargets)
		{
			targetRotation = Quaternion.LookRotation((targets.Keys.First().transform.position + Vector3.up *0.6f)- transform.position);
		}
	}

    public int HitChancePercent(EnemyMain enemy)
    {
        var distance=Vector3.Distance(transform.position,enemy.transform.position);
        var multi=WeaponSlot.ObjData.GetAccuracyMulti();
        return (int)Mathf.Clamp((Accuracy-((distance-MapGenerator.TileSize.x)/(Range*0.01f)))*multi,0,100);
    }

    public float HitChance(EnemyMain enemy)
    {
        return HitChance(enemy)*0.01f;
    }
}