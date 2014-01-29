using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class WeaponMain : MonoBehaviour {

	public int MinDamage { get; protected set;}
	public int MaxDamage { get; protected set;}

	public int RateOfFire { get; protected set;}

	public int Accuracy { get; protected set;}

	public int HeatGeneration { get; protected set;}
	public int CoolingRate { get; protected set;}
	public int CurrentHeat { get; protected set;}

	public int MaxAmmo { get; protected set;}
	public int CurrentAmmo { get; protected set;}

	public List<EnemyMain> targets;

	public bool HasTargets
	{
		get { return targets.Count > 0; }
	}

	// Use this for initialization
	void Start () {
		targets = new List<EnemyMain>();
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public virtual void ToggleTarget(EnemyMain enemy)
	{
		if (targets.Contains(enemy))
		{
			RemoveTarget(enemy);
		}
		else
		{
			AddTarget(enemy);
		}
	}

	public void AddTarget(EnemyMain enemy)
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

	protected void RemoveTarget(EnemyMain enemy)
	{
		targets.Remove(enemy);
	}

	public void Shoot()
	{
		int shotsPerTarget = RateOfFire / targets.Count;
		int totalShots = 0;

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

				CurrentHeat += HeatGeneration;

				if (Accuracy > Subs.RandomPercent())
				{
					int dmg = (int)Random.Range(MinDamage, MaxDamage);

					enemy.TakeDamage(dmg);
				}
			}
		}
	}
}
