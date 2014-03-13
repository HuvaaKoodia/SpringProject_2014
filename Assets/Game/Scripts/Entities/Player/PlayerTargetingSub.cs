using UnityEngine;
using System.Collections;

using System.Collections.Generic;

[RequireComponent(typeof(PlayerMain))]
public class PlayerTargetingSub : MonoBehaviour {

	PlayerMain player;

	List<EnemyMain> allEnemies;

	Dictionary<EnemyMain, TargetMarkHandler> targetableEnemies;

	//UISprite insightPrefab;

	public LayerMask targetingRayMask;

	public Rect TargetingArea 
	{ 
		get { return new Rect(150, 80, Screen.width-300, Screen.height-160); }
	}

	// Use this for initialization
	void Awake () {
		player = gameObject.GetComponent<PlayerMain>();

		allEnemies = player.GC.aiController.enemies;

		targetableEnemies = new Dictionary<EnemyMain, TargetMarkHandler>();

		//insightPrefab = player.GC.SS.PS.InsightSprite;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void CheckTargetableEnemies()
	{
		UnsightAllEnemies();

		//int screenWidth = (int)Camera.main.pixelWidth;
		//int screenHeight = (int)Camera.main.pixelHeight;
		
		Plane[] planes;
		planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
		
		foreach(EnemyMain enemy in allEnemies)
		{
			Vector3 adjustedEnemyPos = enemy.hitbox.bounds.center;

			Vector3 enemyPosInScreen = Camera.main.WorldToScreenPoint(adjustedEnemyPos);
		
			//if enemy is near edges of screen, ignore it (because its under hud)
			if (!TargetingArea.Contains(enemyPosInScreen))
				continue;

			//check if enemy can be seen
			if (GeometryUtility.TestPlanesAABB(planes, enemy.hitbox.bounds))
			{
				for (int i = 0; i < 4; ++i)
				{
					Vector3 gunPosition = player.GetWeapon((WeaponID)i).transform.position;

					Ray ray = new Ray(gunPosition, adjustedEnemyPos - gunPosition);
					RaycastHit hitInfo;
					
					Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 2.0f);
					if (Physics.Raycast(ray, out hitInfo, 20, targetingRayMask))
					{
						if (hitInfo.transform == enemy.hitbox.transform)
						{
							AddEnemyToTargetables(enemy, enemyPosInScreen);
						}
					}
				}
			}
		}

		ShowTargetMarks(player.GetCurrentWeapon().Usable());
	}

	public void CheckGunSightToEnemies(Vector3 gunPosition)
	{
		foreach(KeyValuePair<EnemyMain, TargetMarkHandler> enemyPair in targetableEnemies)
		{
			Vector3 adjustedEnemyPos = enemyPair.Key.hitbox.bounds.center;

			Ray ray = new Ray(gunPosition, adjustedEnemyPos - gunPosition);
			RaycastHit hitInfo;

			Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 2.0f);
			if (Physics.Raycast(ray, out hitInfo, 20, targetingRayMask))
			{
				if (hitInfo.transform == enemyPair.Key.hitbox.transform)
				{
					enemyPair.Value.SetCrosshairVisible(true);
					continue;
				}
			}

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
	
	public void AddEnemyToTargetables(EnemyMain enemy, Vector3 enemyPosInScreen)
	{
		enemyPosInScreen.z = 1;

		/*UISprite targetSprite = GameObject.Instantiate(insightPrefab) as UISprite;
		targetSprite.transform.parent = player.GC.menuHandler.targetMarkPanel.transform;
		targetSprite.spriteName = "crosshair_gray";
		targetSprite.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		targetSprite.transform.position = player.GC.menuHandler.NGUICamera.ScreenToWorldPoint(enemyPosInScreen);
		targetSprite.enabled = true;*/

		if (targetableEnemies.ContainsKey(enemy))
			return;

		TargetMarkHandler tmHandler = new TargetMarkHandler(player.GC, enemyPosInScreen);
		targetableEnemies.Add(enemy, tmHandler);
	}

	public void TargetAtMousePosition(bool increase_amount)
	{
		Component target;
		if (player.targetingMode && Subs.GetObjectMousePos(out target, 20, "Enemy"))
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
		player.GC.menuHandler.targetMarkPanel.gameObject.SetActive(show);
	}
}