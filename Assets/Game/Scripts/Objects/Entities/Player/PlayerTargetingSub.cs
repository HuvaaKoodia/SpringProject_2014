using UnityEngine;
using System.Collections;

using System.Collections.Generic;

[RequireComponent(typeof(PlayerMain))]
public class PlayerTargetingSub : MonoBehaviour {

	PlayerMain player;

	List<EnemyMain> allEnemies{get{
			return player.GC.CurrentFloorData.Enemies;
		}
	}

	Dictionary<EnemyMain, TargetMarkHandler> targetableEnemies;
	public Dictionary<EnemyMain, List<WeaponMain>> shootingOrders { get; private set; }
	public Dictionary<WeaponMain, Dictionary<EnemyMain, Vector3>> targetPositions { get; private set; }

	//UISprite insightPrefab;

	public LayerMask targetingRayMask;
	public LayerMask canSeeMask;

	float cameraFarZ;

	public float deadZoneX, deadZoneY;

	public Rect TargetingArea 
	{
		get { return new Rect(deadZoneX, deadZoneY, Screen.width-deadZoneX*2, Screen.height-deadZoneY*2); }
	}

	void Awake(){
		player = gameObject.GetComponent<PlayerMain>();
		targetableEnemies = new Dictionary<EnemyMain, TargetMarkHandler>();
		shootingOrders = new Dictionary<EnemyMain, List<WeaponMain>>();
		targetPositions = new Dictionary<WeaponMain, Dictionary<EnemyMain, Vector3>>();
	}

	void Start() {
		cameraFarZ = Camera.main.farClipPlane;
	}

	// Update is called once per frame
	void Update () {
		foreach(KeyValuePair<EnemyMain, TargetMarkHandler> enemyPair in targetableEnemies)
		{
			enemyPair.Value.Update(Time.deltaTime);
		}

		deadZoneX = Screen.width * 0.025f;
		deadZoneY = Screen.height * 0.07f;
	}

	public void CheckTargetableEnemies()
	{
		UnsightAllEnemies();

		//int screenWidth = (int)Camera.main.pixelWidth;
		//int screenHeight = (int)Camera.main.pixelHeight;
		
		Plane[] planes;
		planes = GeometryUtility.CalculateFrustumPlanes(player.TargetingCamera);

		//List<int> wallIndices = new List<int>();

		for(int e = 0; e < allEnemies.Count; e++)
		{
			EnemyMain enemy = allEnemies[e];

			for (int i = 0; i < enemy.hitboxes.Count; i++)
			{
				BoxCollider hitbox = enemy.hitboxes[i];

				Vector3 adjustedEnemyPos = hitbox.bounds.center;

				Vector3 enemyPosInScreen = player.TargetingCamera.WorldToScreenPoint(adjustedEnemyPos);
			
				//if enemy is near edges of screen, ignore it (because its under hud)
				if (!TargetingArea.Contains(enemyPosInScreen))
					continue;
			
				enemyPosInScreen = player.PlayerCamera.WorldToScreenPoint(adjustedEnemyPos);
				Ray ray = player.PlayerCamera.ScreenPointToRay(enemyPosInScreen);
				RaycastHit hitInfo;

				//Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 2.0f);
				if (Physics.Raycast(ray, out hitInfo, 20, canSeeMask))
				{
					if (hitInfo.transform.parent != hitbox.transform.parent)
						continue;
				}

				bool enemyAdded = false;
				//check if enemy can be seen
				if (GeometryUtility.TestPlanesAABB(planes, hitbox.bounds))
				{
					for (int j = 0; j < 4; ++j)
					{
						Vector3 gunPosition = player.GetWeapon((WeaponID)j).transform.position;

						ray = new Ray(gunPosition, adjustedEnemyPos - gunPosition);

						//Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 2.0f);

						if (Physics.Raycast(ray, out hitInfo, 20, targetingRayMask))
						{
							if (hitInfo.transform.parent == hitbox.transform.parent)
							{
								enemyAdded = true;
						
								AddEnemyToTargetables(enemy, hitbox.transform.position, enemyPosInScreen);
								break;
							}
						}
					}
				}

				if (enemyAdded)
					break;
			}
		}

		ShowTargetMarks(player.GetCurrentWeapon().Usable());
	}

	public void CheckGunSightToEnemies(Vector3 gunPosition)
	{
		foreach(KeyValuePair<EnemyMain, TargetMarkHandler> enemyPair in targetableEnemies)
		{
			bool wasSeen = false;

			for (int i = 0; i < enemyPair.Key.hitboxes.Count; i++)
			{
				Vector3 adjustedEnemyPos = enemyPair.Key.hitboxes[i].bounds.center;

				Ray ray = new Ray(gunPosition, adjustedEnemyPos - gunPosition);
				RaycastHit hitInfo;

				if (Physics.Raycast(ray, out hitInfo, 20, targetingRayMask))
				{
					if (hitInfo.transform.parent == enemyPair.Key.hitboxes[i].transform.parent)
					{
						enemyPair.Value.SetCrosshairVisible(true);

						Vector3 targetPosition;
					
						if (enemyPair.Key.MyType == EnemyMain.Type.Alien)
						{
							targetPosition = enemyPair.Key.hitboxes[i].bounds.center + Vector3.down*0.2f;
						}
						else
						{
							targetPosition = enemyPair.Key.hitboxes[i].bounds.center + enemyPair.Key.transform.forward*0.3f;
						}

						if (targetPositions.ContainsKey(player.GetCurrentWeapon()))
						{
							if (targetPositions[player.GetCurrentWeapon()].ContainsKey(enemyPair.Key))
							{
								targetPositions[player.GetCurrentWeapon()][enemyPair.Key] = targetPosition;
							}
							else
							{
								targetPositions[player.GetCurrentWeapon()].Add(
									enemyPair.Key, targetPosition);
							}
						}
						else
						{
							targetPositions.Add(player.GetCurrentWeapon(), new Dictionary<EnemyMain, Vector3>());
							targetPositions[player.GetCurrentWeapon()].Add(
								enemyPair.Key, targetPosition);
						}

						wasSeen = true;
						break;
					}
				}
			}
			
			if (!wasSeen)
				enemyPair.Value.SetCrosshairVisible(false);
		}
	}

	public void UnsightAllEnemies()
	{
		foreach(WeaponMain gun in player.weaponList)
		{
			gun.ClearTargets();
		}

		foreach (KeyValuePair<EnemyMain, TargetMarkHandler> enemyPair in targetableEnemies)
			enemyPair.Value.DeInit();

		shootingOrders.Clear();

		targetableEnemies.Clear();

		player.HUD.gunInfoDisplay.UpdateAllDisplays();
	}

    public void UnsightWeapon(WeaponMain weapon)
    {
        weapon.ClearTargets();

        foreach (var enemyPair in targetableEnemies){
            enemyPair.Value.ChangeNumShots(weapon.weaponID,0,0);
        }

		foreach (var enemyPair in shootingOrders)
		{
			while (shootingOrders[enemyPair.Key].Remove(weapon))
			{
			}
		}

		player.HUD.gunInfoDisplay.UpdateAllDisplays();
    }

	public void RemoveWeaponFromAllShootingOrders(WeaponMain weapon)
	{
		foreach (var enemyPair in shootingOrders)
		{
			while (shootingOrders[enemyPair.Key].Remove(weapon))
			{
			}
		}

		player.HUD.gunInfoDisplay.UpdateAllDisplays();
	}

	public void RemoveWeaponFromEnemysOrder(WeaponMain weapon, EnemyMain enemy)
	{
		while (shootingOrders[enemy].Remove(weapon))
		{
		}

		player.HUD.gunInfoDisplay.UpdateAllDisplays();
	}

	public void WeaponShotEnemy(WeaponMain weapon, EnemyMain enemy)
	{
		if (shootingOrders[enemy][0] != weapon)
		{
			Debug.Log("WRONG SHOOTING ORDER");
		}

		shootingOrders[enemy].Remove(weapon);
	}

	public void AddEnemyToTargetables(EnemyMain enemy, Vector3 hitboxPosition, Vector3 enemyPosInScreen)
	{
		if (targetableEnemies.ContainsKey(enemy))return;
		
		enemyPosInScreen.z = 0.7f + (enemyPosInScreen.z / cameraFarZ);

		TargetMarkHandler tmHandler = new TargetMarkHandler(player.GC, enemyPosInScreen, (int)Quaternion.LookRotation(hitboxPosition - player.HudCamera.transform.position).eulerAngles.y);
		targetableEnemies.Add(enemy, tmHandler);
	}

	public bool HasTargetAtMousePosition()
	{
		Component target;
		return HasTargetAtMousePosition(out target);
	}

	private bool HasTargetAtMousePosition(out Component target)
	{
		int mask=1<<LayerMask.NameToLayer("TargetingClick")|1<<LayerMask.NameToLayer("Wall");//|1<<LayerMask.NameToLayer("Interactable"); breaks targeting
		Subs.GetObjectMousePos(out target, 20, mask, player.HudCamera);
		if (target!=null&&LayerMask.LayerToName(target.gameObject.layer)=="TargetingClick"){
			return true;
		}
		return false;
	}

	public void ClickTargetAtMousePosition(bool increase_amount)
	{
		Component target;

		if (HasTargetAtMousePosition(out target))
		{
			Transform trans = target.transform;
			EnemyMain enemyTargeted = trans.gameObject.GetComponent<EnemyMain>();

			while (enemyTargeted == null)
			{
				enemyTargeted = trans.gameObject.GetComponent<EnemyMain>();
				trans = trans.parent;
			}

			if (targetableEnemies.ContainsKey(enemyTargeted) && player.GetCurrentWeapon().Weapon != null)
			{
				if (!targetableEnemies[enemyTargeted].IsVisible)
					return;

				WeaponMain currentWeapon = player.GetCurrentWeapon();

				int startNumShots = currentWeapon.GetNumShotsAtTarget(enemyTargeted);

				currentWeapon.TargetEnemy(enemyTargeted,increase_amount);

				targetableEnemies[enemyTargeted].ChangeNumShots(
					player.currentWeaponID, 
					currentWeapon.GetNumShotsAtTarget(enemyTargeted),
					currentWeapon.HitChancePercent(enemyTargeted)
                );

				int currentNumShots = currentWeapon.GetNumShotsAtTarget(enemyTargeted);

				if (currentNumShots == 0) //untarget
				{
					while (shootingOrders[enemyTargeted].Remove(currentWeapon))
					{}
				}
				else if (currentNumShots > startNumShots) //added
				{
					if (!shootingOrders.ContainsKey(enemyTargeted))
					{
						shootingOrders.Add(enemyTargeted, new List<WeaponMain>());
					}
					int amount = currentNumShots - startNumShots;
					for (int i = 0; i < amount;i++)
					{
						shootingOrders[enemyTargeted].Add(currentWeapon);
					}
				}
				else if (currentNumShots < startNumShots) // removed
				{
					if (!shootingOrders.ContainsKey(enemyTargeted)) return;

					int amount = startNumShots - currentNumShots;
					for (int i = 0; i < amount; i++)
					{
						shootingOrders[enemyTargeted].Remove(currentWeapon);
					}
				}
			}
		}
	}

	public void CheckGunTargets()
	{
		if (player.GetCurrentWeapon().Weapon == null)
			return;

		foreach(KeyValuePair<EnemyMain, TargetMarkHandler> enemyPair in targetableEnemies)
		{
			enemyPair.Value.ChangeNumShots(
               player.currentWeaponID, 
               player.GetCurrentWeapon().GetNumShotsAtTarget(enemyPair.Key),
               player.GetCurrentWeapon().HitChancePercent(enemyPair.Key)
            );
		}
	}

	public bool HasAnyTargets()
	{
		foreach (WeaponMain gun in player.weaponList)
		{
			if (gun.HasTargets)
				return true;
		}

		return false;
	}

	public void ShowTargetMarks(bool show)
	{
		player.HUD.targetMarkPanel.gameObject.SetActive(show);
	}

	public void PlayerStartedShooting()
	{
		ShowTargetMarks(false);
	}

	public void PlayerPhaseStart()
	{
		ShowTargetMarks(true);
	}
}