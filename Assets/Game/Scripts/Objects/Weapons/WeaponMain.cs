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
	WeaponEffects meshData;

	public GameObject verticalMovement;
	public GameObject horizontalMovement;

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

	List<EnemyMain> enemyOrder;
	public Dictionary<EnemyMain, TargetInfo> targets { get; private set;}

	public Quaternion targetVerticalRotation;
	public Quaternion targetHorizontalRotation;
	public float rotationSpeed;

	public float NumParticleEmitters = 2;

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

		bool graphicsFound = player.GC.SS.PS.weaponGraphics.ContainsKey(Weapon.baseItem.mesh);
		GameObject.Destroy(graphics);

		if (graphicsFound && (weaponID != WeaponID.LeftShoulder && weaponID != WeaponID.RightShoulder))
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

		meshData = graphics.GetComponent<WeaponEffects>();
		meshData.SetParticleTempParent(player.ParticleTempParent);

		ParticleSystem[] additionalParticleSystems = graphics.GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < additionalParticleSystems.Count(); i++)
		{
			meshData.AddAdditionalParticleSystem(additionalParticleSystems[i]);
		}

		if (player.GC.SS.PS.weaponParticleEmitters.ContainsKey(Weapon.baseItem.mesh + "Bullets"))
		{
			for (int i = 0; i < NumParticleEmitters; i++)
			{
			GameObject bulletEmitter = 
				GameObject.Instantiate(player.GC.SS.PS.weaponParticleEmitters[Weapon.baseItem.mesh + "Bullets"]) as GameObject;
			meshData.AddBulletParticles(bulletEmitter, (int)weaponID);
			}
		}

		if (player.GC.SS.PS.weaponParticleEmitters.ContainsKey(Weapon.baseItem.mesh + "Muzzle"))
		{	
			for (int i = 0; i < NumParticleEmitters; i++)
			{
				GameObject muzzleEmitter = 
					GameObject.Instantiate(player.GC.SS.PS.weaponParticleEmitters[Weapon.baseItem.mesh + "Muzzle"]) as GameObject;
				meshData.AddMuzzleParticles(muzzleEmitter);
			}
		}

		string name=Weapon.baseItem.mesh + "Flash";
		if (player.GC.SS.PS.weaponFlashes.ContainsKey(name))
		{	
			GameObject obj = GameObject.Instantiate(player.GC.SS.PS.weaponFlashes[Weapon.baseItem.mesh + "Flash"]) as GameObject;
			meshData.SetFlash(obj.GetComponent<MuzzleFlashSystem>());
		}

		if (player.GC.SS.PS.weaponSoundFX.ContainsKey(Weapon.baseItem.mesh))
		{
			meshData.SetShootSoundFX(player.GC.SS.PS.weaponSoundFX[Weapon.baseItem.mesh]);
		}

		if (Weapon.baseItem.mesh == "Melee")
		{
			meshData.SetDamageEffectType(DamageEffectType.ElectricShock);
		}
		else
		{
			meshData.SetDamageEffectType(DamageEffectType.Blood);
		}
	}

	public bool HasTargets
	{
        get { return targets.Count > 0; }
	}

	// Use this for initialization
	void Awake () {
		rotationSpeed = 50;
		targetVerticalRotation = Quaternion.identity;
		targetHorizontalRotation = Quaternion.identity;
		targets = new Dictionary<EnemyMain, TargetInfo>();

		enemyOrder = new List<EnemyMain>();
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
				AddTarget(enemy, 1);
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
        else
		{
            if (GetNumShotsTargetedTotal() < RateOfFire)
			{
				AddTarget(enemy, RateOfFire-GetNumShotsTargetedTotal());
            }
        }
	}

	protected void AddTarget(EnemyMain enemy, int numshots)
	{
		TargetInfo newInfo = new TargetInfo();
		newInfo.numShots = numshots;
		newInfo.targetPosition = player.targetingSub.targetPositions[this][enemy];
		targets.Add(enemy, newInfo);
		enemyOrder.Add(enemy);
	}

	protected void RemoveTarget(EnemyMain enemy)
	{
		targets.Remove(enemy);
		enemyOrder.Remove(enemy);
	}

	public void ClearTargets()
	{
		targets.Clear();
		enemyOrder.Clear();
	}

	bool waitingForShot;

	public IEnumerator Shoot()
	{
		waitingForShot = false;

		for (int i = 0; i < enemyOrder.Count; i++)
		{
			for (int x = 0; x < targets[enemyOrder[i]].numShots; x++)
			{
				if (Overheat || CurrentAmmo == 0) break;
				if (targets[enemyOrder[i]].numShots == 0 || IsEnemyDead(enemyOrder[i]))
				{
					continue;
				}
				
				while (waitingForShot || enemyOrder[i].GetWaitingForDamageReaction((int)weaponID))
				{
					yield return null;
				}
				
				StartCoroutine(ShootEnemy(enemyOrder[i]));
				
				yield return null;
			}
		}

		while (waitingForShot)
		{
			yield return null;
		}

		SetTargetRotationToDefault();
		player.targetingSub.RemoveWeaponFromAllShootingOrders(this);
		enemyOrder.Clear();
		player.GunFinishedShooting();
	}

	IEnumerator ShootEnemy(EnemyMain enemy)
	{
		waitingForShot = true;

		if (!player.InstaShoot)
		{
			SetTargetRotation(enemy);

			while ((enemy != null) && (!lookingAtEnemy(enemy) || player.targetingSub.shootingOrders[enemy][0] != this))
			{
				if (IsEnemyDead(enemy))
				{
					waitingForShot = false;
					player.targetingSub.RemoveWeaponFromEnemysOrder(this, enemy);
					yield break;
				}
				yield return null;
			}
		}
		else
		{
			while (player.targetingSub.shootingOrders[enemy][0] != this)
			{
				if (IsEnemyDead(enemy))
				{
					waitingForShot = false;
					player.targetingSub.RemoveWeaponFromEnemysOrder(this, enemy);
					yield break;
				}
				yield return null;
			}
		}

        if (IsEnemyDead(enemy))
        {
            waitingForShot = false;
            player.targetingSub.RemoveWeaponFromEnemysOrder(this, enemy);
            yield break;
        }

		if (!NoAmmoConsumption) CurrentAmmo--;
		if (CurrentAmmo<0) CurrentAmmo=0;
		
		IncreaseHeat();

		bool hit = HitChancePercent(enemy) > Subs.RandomPercent();

		if (hit)
		{
			//hit
			player.HUD.gunInfoDisplay.HideMissText(weaponID);
			int dmg = (int)Random.Range(MinDamage, MaxDamage);
			StartCoroutine(enemy.TakeDamage(dmg, (int)weaponID, player.InstaShoot));
		}
		else
		{
			player.HUD.gunInfoDisplay.ShowMissText(weaponID);
		}

		if (!player.InstaShoot)
		{
			if (meshData != null)
			{
				meshData.PlayShootAnimation();
				meshData.StartShootEffect(hit);

				while (meshData.IsEmiting || meshData.IsAnimating)
				{
					yield return null;
				}
			}
		}

		player.targetingSub.shootingOrders[enemy].RemoveAt(0);
		waitingForShot = false;
	}

	bool IsEnemyDead(EnemyMain enemy)
	{
		return enemy == null || enemy.GoingToDie || enemy.Dead;
	}
	
    public void IncreaseHeat()
    {
		if (WeaponSlot != null) WeaponSlot.ObjData.IncreaseHEAT(Weapon);
    }

	public void ReduceHeat()
	{
		if (WeaponSlot != null) WeaponSlot.ObjData.ReduceHEAT(Weapon);
	}

	public void DisperseHeat()
	{
		if (WeaponSlot != null) WeaponSlot.ObjData.DisperceHEAT_Weapon(Weapon);
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
					                         Quaternion.Euler(targetVerticalRotation.eulerAngles.x, player.transform.rotation.eulerAngles.y, 0.0f),// * player.transform.rotation, 
					                         Time.deltaTime*rotationSpeed);
			}

			if (horizontalMovement)
			{
				horizontalMovement.transform.rotation = 
					Quaternion.RotateTowards(horizontalMovement.transform.rotation, 
					                         Quaternion.Euler(targetVerticalRotation.eulerAngles.x, targetHorizontalRotation.eulerAngles.y, 0.0f),// * player.transform.rotation, 
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
		
		verticalMovement.transform.rotation = Quaternion.Euler(verticalMovement.transform.rotation.eulerAngles.x,
		                                                       verticalMovement.transform.rotation.eulerAngles.y,
		                                                       0.0f);

		horizontalMovement.transform.rotation = Quaternion.Euler(verticalMovement.transform.rotation.eulerAngles.x,
		                                                         horizontalMovement.transform.rotation.eulerAngles.y,
		                                                         0.0f);
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

			targetVerticalRotation = Quaternion.LookRotation(mouseToWorld - verticalMovement.transform.position);
			targetHorizontalRotation = Quaternion.LookRotation(mouseToWorld - horizontalMovement.transform.position);
		}
		else
		{
			targetVerticalRotation = transform.rotation = 
				Quaternion.RotateTowards(verticalMovement.transform.rotation, player.transform.rotation, Time.deltaTime*rotationSpeed);

			targetHorizontalRotation = transform.rotation = 
				Quaternion.RotateTowards(horizontalMovement.transform.rotation, player.transform.rotation, Time.deltaTime*rotationSpeed);
		}

		if (verticalMovement != null)
		{
			verticalMovement.transform.rotation = 
				Quaternion.RotateTowards(verticalMovement.transform.rotation, 
				                         Quaternion.Euler(targetVerticalRotation.eulerAngles.x, player.transform.rotation.eulerAngles.y, 0.0f),// * player.transform.rotation, 
			        	                 Time.deltaTime*rotationSpeed);
		}

		if (horizontalMovement != null)
		{
			horizontalMovement.transform.rotation = 
				Quaternion.RotateTowards(horizontalMovement.transform.rotation, 
				                         Quaternion.Euler(targetVerticalRotation.eulerAngles.x, targetHorizontalRotation.eulerAngles.y, 0.0f),// * player.transform.rotation, 
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
			targetHorizontalRotation = Quaternion.LookRotation(targets[enemyOrder[0]].targetPosition - horizontalMovement.transform.position);
			targetVerticalRotation = Quaternion.LookRotation(targets[enemyOrder[0]].targetPosition - verticalMovement.transform.position);
		}
		else
		{
			SetTargetRotationToDefault();
		}
	}

	void SetTargetRotation(EnemyMain enemy)
	{
		targetHorizontalRotation = Quaternion.LookRotation(targets[enemy].targetPosition - horizontalMovement.transform.position);
		targetVerticalRotation = Quaternion.LookRotation(targets[enemy].targetPosition - verticalMovement.transform.position);
	}

	void SetTargetRotationToDefault()
	{
		targetVerticalRotation = player.transform.rotation;
		targetHorizontalRotation = player.transform.rotation;
	}

    public int HitChancePercent(EnemyMain enemy)
    {
        var distance=Vector3.Distance(new Vector3(player.transform.position.x, player.transform.position.z),
		                              new Vector3(enemy.transform.position.x, enemy.transform.position.z));
        var multi=WeaponSlot.ObjData.GetAccuracyMulti();
		var bonus=WeaponSlot.ObjData.GetAccuracyBonus();
		return XmlDatabase.GetAccuracy(Accuracy,Range,distance,bonus,multi);
    }

	bool lookingAtTarget()
	{
		return Subs.ApproximatelySame(verticalMovement.transform.rotation.eulerAngles.x, targetVerticalRotation.eulerAngles.x, 0.04f) &&
			   Subs.ApproximatelySame(horizontalMovement.transform.rotation.eulerAngles.y, targetHorizontalRotation.eulerAngles.y, 0.04f);
	}

	bool lookingAtEnemy(EnemyMain enemy)
	{
		Quaternion lookToEnemyX =  Quaternion.LookRotation(targets[enemy].targetPosition - verticalMovement.transform.position);
		Quaternion lookToEnemyY =  Quaternion.LookRotation(targets[enemy].targetPosition - horizontalMovement.transform.position);

		bool x = Subs.ApproximatelySame(verticalMovement.transform.rotation.eulerAngles.x, lookToEnemyX.eulerAngles.x, 0.4f);
		bool y = Subs.ApproximatelySame(horizontalMovement.transform.rotation.eulerAngles.y, lookToEnemyY.eulerAngles.y, 0.4f);

		return x && y;
	}
	
	public void SetEnemyTargetPosition(EnemyMain enemy, Vector3 position)
	{
		if (targets.ContainsKey(enemy))
		    targets[enemy].targetPosition = position;
	    else
	    {
			AddTarget(enemy, 0);
		}
	}
}

public class TargetInfo
{
	public Vector3 targetPosition;
	public int numShots;
}
