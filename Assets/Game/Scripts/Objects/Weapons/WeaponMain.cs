using UnityEngine;

using System.Linq;

using System.Collections;
using System.Collections.Generic;

public enum WeaponID
{
    LeftHand=0,
	LeftShoulder=1,
    RightHand=2,
	RightShoulder=3
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

	public LayerMask lookAtLayer;

	public GameObject graphics;

	public GameObject verticalMovement;
	public GameObject horizontalMovement;

	public Transform barrelEstimate;

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

	public Dictionary<EnemyMain, TargetInfo> targets { get; private set;}

	public Quaternion targetRotation;
	public float rotationSpeed;

    public bool NoAmmoConsumption{get;private set;}

    public bool Usable(){
		return Weapon!=null&&WeaponSlot.ObjData.USABLE&&!Overheat&&CurrentAmmo>0;
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

		if (weaponID == WeaponID.LeftShoulder || weaponID == WeaponID.RightShoulder)
			return;

		bool graphicsFound = player.GC.SS.PS.weaponGraphics.ContainsKey(Weapon.baseItem.mesh);
		GameObject.Destroy(graphics);

		if (graphicsFound)
		{
			graphics = GameObject.Instantiate(player.GC.SS.PS.weaponGraphics[Weapon.baseItem.mesh]) as GameObject;
			graphics.transform.parent = horizontalMovement.transform;
			graphics.transform.localPosition = Vector3.zero;
			graphics.transform.rotation = transform.rotation;
		}
		else
		{
			graphics = GameObject.Instantiate(player.GC.SS.PS.weaponGraphics["NotFound"]) as GameObject;
			graphics.transform.parent = horizontalMovement.transform;
			graphics.transform.localPosition = Vector3.zero;
			graphics.transform.rotation = transform.rotation;
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
		targets = new Dictionary<EnemyMain, TargetInfo>();
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
                targets[enemy].numShots++;
            else
                RemoveTarget(enemy);
        }
        else{
            if (GetNumShotsTargetedTotal() < RateOfFire){
				TargetInfo newInfo = new TargetInfo();
				newInfo.numShots = 1;
				newInfo.targetPosition = player.targetingSub.targetPositions[this][enemy];
				targets.Add(enemy, newInfo);
            }
        }
	}

	protected void DecreaseShotsAtTarget(EnemyMain enemy)
	{
        if (targets.ContainsKey(enemy)){

            if (targets[enemy].numShots >1){
                targets[enemy].numShots--;
            }
            else
                RemoveTarget(enemy);
        }
        else{
            if (GetNumShotsTargetedTotal() < RateOfFire){
				TargetInfo newInfo = new TargetInfo();
				newInfo.numShots = RateOfFire-GetNumShotsTargetedTotal();
				newInfo.targetPosition = player.targetingSub.targetPositions[this][enemy];
				targets.Add(enemy, newInfo);
            }
        }
	}

	protected void RemoveTarget(EnemyMain enemy)
	{
		targets[enemy].numShots = 0;
	}

	public void ClearTargets()
	{
		targets.Clear();
	}

	bool waitingForShot;

	public IEnumerator Shoot()
	{
		waitingForShot = false;

		foreach(KeyValuePair<EnemyMain, TargetInfo> enemyPair in targets)
		{
			//shoot all shots at one enemy consecutively
			for (int i = 0; i < enemyPair.Value.numShots; i++)
			{
				if (Overheat || CurrentAmmo == 0) break;
				if (enemyPair.Value.numShots == 0 || enemyPair.Key == null) continue;

				while (waitingForShot)
				{
					yield return null;
				}

				StartCoroutine(ShootEnemy(enemyPair.Key));

				yield return null;
			}
		}

		while (waitingForShot)
		{
			yield return null;
		}

		SetTargetRotationToDefault();
		player.GunFinishedShooting();
	}

	IEnumerator ShootEnemy(EnemyMain enemy)
	{
		SetTargetRotation(enemy);
		waitingForShot = true;

		while (!lookingAtEnemy(enemy))
			yield return null;

		if (!NoAmmoConsumption) CurrentAmmo--;
		if (CurrentAmmo<0) CurrentAmmo=0;
		
		IncreaseHeat(1);
		
		if (HitChancePercent(enemy) > Subs.RandomPercent())
		{
			//hit
			int dmg = (int)Random.Range(MinDamage, MaxDamage);
			enemy.TakeDamage(dmg);
		}

		waitingForShot = false;
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
		foreach(KeyValuePair<EnemyMain, TargetInfo> enemyPair in targets)
			numShots += enemyPair.Value.numShots;

		return numShots;
	}

	public int GetNumShotsAtTarget(EnemyMain enemy)
	{
		if (targets.ContainsKey(enemy))
			return targets[enemy].numShots;
		else
			return 0;
	}

	public void RotateGraphics()
	{
		if (HasTargets)
		{
			if (verticalMovement != null)
			{
				verticalMovement.transform.rotation = 
					Quaternion.RotateTowards(verticalMovement.transform.rotation, 
					                         Quaternion.Euler(targetRotation.eulerAngles.x, player.transform.rotation.eulerAngles.y, 0.0f),// * player.transform.rotation, 
					                         Time.deltaTime*rotationSpeed);
			}

			if (horizontalMovement)
			{
				horizontalMovement.transform.rotation = 
					Quaternion.RotateTowards(horizontalMovement.transform.rotation, 
				    	                     Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, 0.0f),// * player.transform.rotation, 
				        	                 Time.deltaTime*rotationSpeed);
			}
		}
		else
		{
			if (verticalMovement != null)
			{
				verticalMovement.transform.rotation = 
					Quaternion.RotateTowards(verticalMovement.transform.rotation, 
				    	                     player.transform.rotation, 
				        	                 Time.deltaTime*rotationSpeed);
			}

			if (horizontalMovement != null)
			{
				horizontalMovement.transform.rotation = 
					Quaternion.RotateTowards(horizontalMovement.transform.rotation, 
				    	                     player.transform.rotation, 
				        	                 Time.deltaTime*rotationSpeed);
				                         
			}
		}
	}

	public void LookAtMouse(Rect bounds)
	{
		if (bounds.Contains(new Vector2(Input.mousePosition.x, Input.mousePosition.y)))
		{
			Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit info;

			float mouseDistance = 12;
			if (Physics.Raycast(ray, out info, mouseDistance, lookAtLayer))
			{
				mouseDistance = info.distance;
			}

			Vector3 mouseToWorld = player.GameCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x , Input.mousePosition.y, mouseDistance));

			targetRotation = Quaternion.LookRotation(mouseToWorld - barrelEstimate.position);
		}
		else
			targetRotation = transform.rotation = 
				Quaternion.RotateTowards(transform.rotation, player.transform.rotation, Time.deltaTime*rotationSpeed);

		if (verticalMovement != null)
		{
			verticalMovement.transform.rotation = 
				Quaternion.RotateTowards(verticalMovement.transform.rotation, 
			    	                     Quaternion.Euler(targetRotation.eulerAngles.x, player.transform.rotation.eulerAngles.y, 0.0f),// * player.transform.rotation, 
			        	                 Time.deltaTime*rotationSpeed);
		}

		if (verticalMovement != null)
		{
			horizontalMovement.transform.rotation = 
				Quaternion.RotateTowards(horizontalMovement.transform.rotation, 
		    		                     Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, 0.0f),// * player.transform.rotation, 
			            	             Time.deltaTime*rotationSpeed);
		}
	}
	
	public void Unselected()
	{
		SetTargetRotation();
	}

	void SetTargetRotation()
	{
		if (HasTargets)
		{
			//targetRotation = Quaternion.LookRotation((targets.Keys.First().transform.position + Vector3.up *0.6f)- transform.position);
			targetRotation = Quaternion.LookRotation(targets[targets.Keys.First()].targetPosition - transform.position);
		}
		else
		{
			SetTargetRotationToDefault();
		}
	}

	void SetTargetRotation(EnemyMain enemy)
	{
		targetRotation = Quaternion.LookRotation(targets[enemy].targetPosition - transform.position);
	}

	void SetTargetRotationToDefault()
	{
		targetRotation = player.transform.rotation;
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

	bool lookingAtTarget()
	{
		return horizontalMovement.transform.rotation == Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, 0.0f);
	}

	bool lookingAtEnemy(EnemyMain enemy)
	{
		return horizontalMovement.transform.rotation == Quaternion.LookRotation(targets[enemy].targetPosition - transform.position);
	}

	public void SetEnemyTargetPosition(EnemyMain enemy, Vector3 position)
	{
		if (targets.ContainsKey(enemy))
		    targets[enemy].targetPosition = position;
	    else
	    {
			TargetInfo newInfo = new TargetInfo();
			newInfo.numShots = 0;
			newInfo.targetPosition = position;

			targets.Add(enemy, newInfo);
		}
	}
}

public class TargetInfo
{
	public Vector3 targetPosition;
	public int numShots;
}
