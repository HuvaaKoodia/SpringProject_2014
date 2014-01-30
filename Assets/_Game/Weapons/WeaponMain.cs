using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class WeaponMain : MonoBehaviour {

	public string GunName { get; protected set;}

	public int MinDamage { get; protected set;}
	public int MaxDamage { get; protected set;}

	public int RateOfFire { get; protected set;}

	public int Accuracy { get; protected set;}

	public int HeatGeneration { get; protected set;}
	public int CoolingRate { get; protected set;}
	public int CurrentHeat { get; protected set;}

	public int MaxAmmo { get; protected set;}
	public int CurrentAmmo { get; protected set;}

	public List<EnemyMain> targets { get; private set;}

	public bool HasTargets
	{
		get { return targets.Count > 0; }
	}

	// Use this for initialization
	void Awake () {
		targets = new List<EnemyMain>();
	}
	
	// Update is called once per frame
	void Update () {
	}

	//return true if added, false if removed
	public virtual bool ToggleTarget(EnemyMain enemy)
	{
		if (targets.Contains(enemy))
		{
			RemoveTarget(enemy);
			return false;
		}
		else
		{
			AddTarget(enemy);
			return true;
		}
	}

	protected void AddTarget(EnemyMain enemy)
	{
		if (targets.Count < RateOfFire)
		{
			targets.Add(enemy);
		}
	}

	public void ClearTargets()
	{
		targets.Clear();
	}

	public void RemoveTarget(EnemyMain enemy)
	{
		if (targets.Contains(enemy))
		{
			targets.Remove(enemy);
		}
	}

	public void Shoot()
	{
		int shotsPerTarget = Mathf.CeilToInt((float)RateOfFire / targets.Count);

		int totalShots = 0;

		if (CurrentHeat >= 100 || CurrentAmmo <= 0)
			return;

		CurrentHeat += HeatGeneration;

		foreach(EnemyMain enemy in targets)
		{
			//shoot all shots at one enemy concecutively
			for (int i = 0; i < shotsPerTarget; i++)
			{
				//stop shooting if all shots for turn have been shot, this should be true
				//only at last enemy if amount of shots divided by targets isn't even
				if (totalShots == RateOfFire)
					break;

				totalShots++;
				CurrentAmmo--;

				if (Accuracy > Subs.RandomPercent())
				{
					int dmg = (int)Random.Range(MinDamage, MaxDamage);

					enemy.TakeDamage(dmg);
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
}
