﻿using UnityEngine;
using System.Collections;

using System.Collections.Generic;

[RequireComponent(typeof(PlayerMain))]
public class PlayerTargetingSub : MonoBehaviour {

	PlayerMain player;

	List<EnemyMain> allEnemies;

	Dictionary<EnemyMain, TargetMarkHandler> targetableEnemies;

	UISprite insightPrefab;

	public Rect TargetingArea 
	{ 
		get { return new Rect(60, 40, Screen.width-120, Screen.height-80); }
	}

	// Use this for initialization
	void Awake () {
		player = gameObject.GetComponent<PlayerMain>();

		allEnemies = player.GC.aiController.enemies;

		targetableEnemies = new Dictionary<EnemyMain, TargetMarkHandler>();

		insightPrefab = player.GC.SS.PS.InsightSprite;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void CheckTargetableEnemies(Vector3 gunPosition)
	{
		UnsightAllEnemies();

		int screenWidth = (int)Camera.main.pixelWidth;
		int screenHeight = (int)Camera.main.pixelHeight;
		
		Plane[] planes;
		planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
		foreach(EnemyMain enemy in allEnemies)
		{
			Vector3 adjustedEnemyPos = enemy.transform.position + Vector3.up*0.6f;

			Vector3 enemyPosInScreen = Camera.main.WorldToScreenPoint(adjustedEnemyPos);
		
			//if enemy is near edges of screen, ignore it (because its under hud)
			if (!TargetingArea.Contains(enemyPosInScreen))
				continue;

			//check if enemy can be seen
			if (GeometryUtility.TestPlanesAABB(planes, enemy.collider.bounds))
			{
				Ray ray = new Ray(gunPosition, adjustedEnemyPos - gunPosition);
				RaycastHit hitInfo;
				
				Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 2.0f);
				if (Physics.Raycast(ray, out hitInfo, 20))
				{
					if (hitInfo.transform == enemy.transform)
					{
						AddEnemyToTargetables(enemy, enemyPosInScreen);
					}
				}
			}
		}
	}

	public void UnsightAllEnemies()
	{
		foreach(WeaponMain gun in player.gunList)
		{
			gun.ClearTargets();
		}

		foreach (KeyValuePair<EnemyMain, TargetMarkHandler> enemyPair in targetableEnemies)
			enemyPair.Value.DeInit();

		targetableEnemies.Clear();
	}
	
	public void AddEnemyToTargetables(EnemyMain enemy, Vector3 enemyPosInScreen)
	{
		enemyPosInScreen.z = 1;

		/*UISprite targetSprite = GameObject.Instantiate(insightPrefab) as UISprite;
		targetSprite.transform.parent = player.GC.menuHandler.targetMarkPanel.transform;
		targetSprite.spriteName = "crosshair_gray";
		targetSprite.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		targetSprite.transform.position = player.GC.menuHandler.NGUICamera.ScreenToWorldPoint(enemyPosInScreen);
		targetSprite.enabled = true;*/
		TargetMarkHandler tmHandler = new TargetMarkHandler(player.GC, enemyPosInScreen);
		targetableEnemies.Add(enemy, tmHandler);
	}

	public void TargetAtMousePosition(bool left_click)
	{
		Component target;
		if (player.targetingMode && Subs.GetObjectMousePos(out target, 20, "Enemy"))
		{
			EnemyMain enemyTargeted = target.transform.gameObject.GetComponent<EnemyMain>();

			if (targetableEnemies.ContainsKey(enemyTargeted))
			{
				player.GetCurrentWeapon().ToggleTarget(enemyTargeted,left_click);

				targetableEnemies[enemyTargeted].ChangeNumShots(
					player.currentGunID, 
					player.GetCurrentWeapon().GetNumShotsAtTarget(enemyTargeted));
			}
		}
	}

	public void CheckGunTargets()
	{
		foreach(KeyValuePair<EnemyMain, TargetMarkHandler> enemyPair in targetableEnemies)
		{
			enemyPair.Value.ChangeNumShots(player.currentGunID, 
			               player.GetCurrentWeapon().GetNumShotsAtTarget(enemyPair.Key));
		}
	}

	public bool HasAnyTargets()
	{
		foreach (WeaponMain gun in player.gunList)
		{
			if (gun.HasTargets)
				return true;
		}

		return false;
	}
}