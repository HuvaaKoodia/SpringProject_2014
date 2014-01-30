using UnityEngine;
using System.Collections;

using System.Collections.Generic;

[RequireComponent(typeof(PlayerMain))]
public class PlayerTargetingSub : MonoBehaviour {

	PlayerMain player;

	List<EnemyMain> allEnemies;

	Dictionary<EnemyMain, UISprite> targetableEnemies;

	UISprite insightPrefab;

	// Use this for initialization
	void Awake () {
		player = gameObject.GetComponent<PlayerMain>();

		allEnemies = player.GC.aiController.enemies;

		targetableEnemies = new Dictionary<EnemyMain, UISprite>();

		insightPrefab = player.GC.SS.PS.InsightSprite;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void CheckTargetableEnemies(Vector3 gunPosition)
	{
		UnsightAllEnemies();

		int border = 20;
		int screenWidth = (int)Camera.main.pixelWidth;
		int screenHeight = (int)Camera.main.pixelHeight;
		
		Plane[] planes;
		planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
		foreach(EnemyMain enemy in allEnemies)
		{
			Vector3 adjustedEnemyPos = enemy.transform.position + Vector3.up*0.6f;

			Vector3 enemyPosInScreen = Camera.main.WorldToScreenPoint(adjustedEnemyPos);
		
			//if enemy is near edges of screen, ignore it (because its under hud)
			if (Subs.insideArea(screenWidth, screenHeight, 
			                    border, border, screenWidth-border, screenHeight-border))
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

		foreach(KeyValuePair<EnemyMain, UISprite> targetSprite in targetableEnemies)
		{
			NGUITools.Destroy(targetSprite.Value);
			Destroy(targetSprite.Value.gameObject);
		}

		targetableEnemies.Clear();
	}
	
	void UnsightEnemy(EnemyMain enemy)
	{
		player.currentGun.RemoveTarget(enemy);

		NGUITools.Destroy(targetableEnemies[enemy]);
		targetableEnemies.Remove(enemy);
	}
	
	public void AddEnemyToTargetables(EnemyMain enemy, Vector3 enemyPosInScreen)
	{
		enemyPosInScreen.z = 1;

		UISprite targetSprite = GameObject.Instantiate(insightPrefab) as UISprite;
		targetSprite.transform.parent = player.GC.menuHandler.targetMarkPanel.transform;
		targetSprite.spriteName = "crosshair_gray";
		targetSprite.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		targetSprite.transform.position = player.GC.menuHandler.NGUICamera.ScreenToWorldPoint(enemyPosInScreen);
		targetSprite.enabled = true;

		targetableEnemies.Add(enemy, targetSprite);
	}

	public void TargetAtMousePosition()
	{
		Component target;
		if (player.targetingMode && Subs.GetObjectMousePos(out target, 20, "Enemy"))
		{
			EnemyMain enemyTargeted = target.transform.gameObject.GetComponent<EnemyMain>();

			if (targetableEnemies.ContainsKey(enemyTargeted))
			{
				bool wasAdded = player.currentGun.ToggleTarget(enemyTargeted);

				if (wasAdded)
				{
					targetableEnemies[enemyTargeted].spriteName = "crosshair_red";
				}
				else
				{
					targetableEnemies[enemyTargeted].spriteName = "crosshair_gray";
				}
			}
		}
	}

	public void CheckGunTargets()
	{
		foreach(KeyValuePair<EnemyMain, UISprite> enemyPair in targetableEnemies)
		{
			if (player.currentGun.targets.Contains(enemyPair.Key))
			{
				enemyPair.Value.spriteName = "crosshair_red";
			}
			else
			{
				enemyPair.Value.spriteName = "crosshair_gray";
			}
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
