﻿using UnityEngine;
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
	public Dictionary<WeaponMain, Dictionary<EnemyMain, Vector3>> targetPositions { get; private set; }

	//UISprite insightPrefab;

	public LayerMask targetingRayMask;
	public LayerMask canSeeMask;
	public int wallMask;

	float cameraFarZ;

	public float deadZoneX, deadZoneY;

	public Rect TargetingArea 
	{
		get { return new Rect(deadZoneX, deadZoneY, Screen.width-deadZoneX*2, Screen.height-deadZoneY*2); }
	}

	void Awake(){
		player = gameObject.GetComponent<PlayerMain>();
		targetableEnemies = new Dictionary<EnemyMain, TargetMarkHandler>();
		targetPositions = new Dictionary<WeaponMain, Dictionary<EnemyMain, Vector3>>();

		wallMask = LayerMask.NameToLayer("Wall");
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

						Vector3 targetPosition = enemyPair.Key.hitboxes[i].bounds.center+ Vector3.down*0.2f;

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

		targetableEnemies.Clear();
	}

    public void UnsightWeapon(WeaponMain weapon)
    {
        weapon.ClearTargets();

        foreach (var enemyPair in targetableEnemies){
            enemyPair.Value.ChangeNumShots(weapon.weaponID,0,0);
        }
    }
	
	public void AddEnemyToTargetables(EnemyMain enemy, Vector3 hitboxPosition, Vector3 enemyPosInScreen)
	{
		if (targetableEnemies.ContainsKey(enemy))
			return;

		
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
		return Subs.GetObjectMousePos(out target, 20, "TargetingClick", player.HudCamera);
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

                player.GetCurrentWeapon().TargetEnemy(enemyTargeted,increase_amount);

				targetableEnemies[enemyTargeted].ChangeNumShots(
					player.currentWeaponID, 
                    player.GetCurrentWeapon().GetNumShotsAtTarget(enemyTargeted),
                    player.GetCurrentWeapon().HitChancePercent(enemyTargeted)
                );
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
}