﻿using UnityEngine;

using System.Linq;

using System.Collections;
using System.Collections.Generic;

public class WeaponMain : MonoBehaviour {

	public PlayerMain player;

	public InvGameItem Weapon{get;private set;}

	public string GunName { get;protected set;}

	public int MinDamage { get; protected set;}
	public int MaxDamage { get; protected set;}

	public int RateOfFire { get; protected set;}

	public int Accuracy { get; protected set;}

	public int HeatGeneration { get; protected set;}
	public int CoolingRate { get; protected set;}
	public int CurrentHeat { get; protected set;}

	public int MaxAmmo { get; protected set;}
	public int CurrentAmmo { get; protected set;}

	public Dictionary<EnemyMain, int> targets { get; private set;}

	public Quaternion targetRotation;
	public float rotationSpeed;

	public void SetWeapon(InvGameItem weapon){
			Weapon=weapon;
			GunName=Weapon.baseItem.name;
			
			//DEV.temp calc
			var dam=weapon.GetStat(InvStat.Type.Damage)._amount;
			MinDamage = dam-2;
			MaxDamage = dam+2;
			
			RateOfFire = weapon.GetStat(InvStat.Type.Firerate)._amount;
			Accuracy = weapon.GetStat(InvStat.Type.Accuracy)._amount;
			
			HeatGeneration = weapon.GetStat(InvStat.Type.Heat)._amount;
			CoolingRate = weapon.GetStat(InvStat.Type.Cooling)._amount;
			CurrentHeat = 0;
			
			MaxAmmo = 20;
			CurrentAmmo = MaxAmmo;
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

	//return true if added, false if removed
	public virtual bool ToggleTarget(EnemyMain enemy,bool left_click)
	{
		if (targets.ContainsKey(enemy))
		{
			if (left_click)
			{
				if (GetNumShotsTargetedTotal() < RateOfFire)
					IncreaseShotsAtTarget(enemy);
				else
					RemoveTarget(enemy);
			}
			else if (!left_click)
				DecreaseShotsAtTarget(enemy);

			SetTargetRotation();
			return false;
		}
		else
		{
			AddTarget(enemy);

			if (!left_click)
			{
				while(GetNumShotsTargetedTotal() < RateOfFire)
				{
					IncreaseShotsAtTarget(enemy);
				}
			}

			SetTargetRotation();
			return true;
		}
	}

	protected void AddTarget(EnemyMain enemy)
	{
		if (GetNumShotsTargetedTotal() < RateOfFire)
		{
			targets.Add(enemy, 1);
		}
	}

	protected void IncreaseShotsAtTarget(EnemyMain enemy)
	{
		targets[enemy]++;
	}

	protected void DecreaseShotsAtTarget(EnemyMain enemy)
	{
		targets[enemy]--;
		if (targets[enemy]<=0){
			RemoveTarget(enemy);
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
		if (CurrentHeat >= 100 || CurrentAmmo <= 0)
			return;

		CurrentHeat += HeatGeneration;

		foreach(KeyValuePair<EnemyMain, int> enemyPair in targets)
		{
			//shoot all shots at one enemy concecutively
			for (int i = 0; i < enemyPair.Value; i++)
			{
				CurrentAmmo--;

				if (Accuracy > Subs.RandomPercent())
				{
					int dmg = (int)Random.Range(MinDamage, MaxDamage);

					enemyPair.Key.TakeDamage(dmg);
				}
			}
		}
	}

	public void ReduceHeat()
	{
		CurrentHeat = Mathf.Max(0, CurrentHeat - CoolingRate);
	}

	public void AddAmmo(int amount)
	{
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
}
