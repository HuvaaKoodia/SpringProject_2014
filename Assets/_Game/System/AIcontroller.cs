using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIcontroller
{
	GameController GC;

	int currentAttacker = 0;
	int finishedEnemies = 0;
	int enemiesWontMoveAnymore = 0;
	public List<EnemyMain> enemies;

	public AIcontroller(GameController gc)
	{
		this.GC = gc;
		enemies = new List<EnemyMain>();
	}
	// Update is called once per frame
	public void UpdateAI(TurnState currentTurn)
	{
		if (currentTurn == TurnState.AIMovement)
		{
            while (true)
            {
                int enemiesUsedTurn = 0;
                foreach (EnemyMain enemy in enemies)
                {
                    enemy.PlayMovementTurn();

                    if (enemy.HasUsedTurn())
                        enemiesUsedTurn++;
                }

                if (enemiesUsedTurn >= enemies.Count)
                    break;
            }

            foreach (EnemyMain enemy in enemies)
            {
                enemy.StartMoving();
            }
   

			GC.ChangeTurn(TurnState.WaitingAIToFinish);
		}
        else if (currentTurn == TurnState.WaitingAIToFinish && !CheckForMovingEnemies())
		{
            if (enemiesWontMoveAnymore < enemies.Count)
            {
                GC.ChangeTurn(TurnState.AIMovement);
            }
            else
            {
                enemiesWontMoveAnymore = 0;
				if (enemies.Count != 0)
                	enemies[currentAttacker].PlayAttackingPhase();

                GC.ChangeTurn(TurnState.AIAttack);
            }
		}
		else if (currentTurn == TurnState.AIAttack && currentAttacker >= enemies.Count)
		{
            currentAttacker = 0;
			GC.ChangeTurn(TurnState.StartPlayerTurn);
		}
		else if (currentTurn == TurnState.StartAITurn)
		{
			foreach (EnemyMain enemy in enemies)
			{
				UntargetEnemy(enemy);
				enemy.StartEnemyTurn();
			}
			GC.ChangeTurn(TurnState.AIMovement);
		}
	}

	public void EnemyFinishedMoving(bool wontMoveAnymore)
	{
		finishedEnemies++;

		if (wontMoveAnymore)
		{
			enemiesWontMoveAnymore++;
		}
	}

    public void EnemyFinishedAttacking()
    {
        currentAttacker++;
        if (currentAttacker < enemies.Count)
            enemies[currentAttacker].PlayAttackingPhase();
    }

    bool CheckForMovingEnemies()
    {
        foreach (EnemyMain enemy in enemies)
        {
            if (enemy.movement.currentMovement != MovementState.NotMoving)
                return true;
        }

        return false;
    }

	public void AddEnemy (EnemyMain enemy)
	{
		enemies.Add(enemy);
		enemy.GC=GC;
	}

	public void CheckTargetableEnemies(Vector3 gunPosition)
	{
		int border = 20;
		int screenWidth = (int)Camera.main.pixelWidth;
		int screenHeight = (int)Camera.main.pixelHeight;

		Plane[] planes;
		planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
		foreach(EnemyMain enemy in enemies)
		{
			enemy.currentTargetState = TargetState.NotInSight;
			Vector3 adjustedEnemyPos = enemy.transform.position + Vector3.up*0.6f;
			//check if enemy can be seen
			Vector3 enemyPosInScreen = Camera.main.WorldToScreenPoint(adjustedEnemyPos);
			if (GeometryUtility.TestPlanesAABB(planes, enemy.collider.bounds))
			{
				Ray ray = new Ray(gunPosition, adjustedEnemyPos - gunPosition);
				RaycastHit hitInfo;

				Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 2.0f);
				if (Physics.Raycast(ray, out hitInfo, 20))
				{
					if (hitInfo.transform == enemy.transform)
					{
						enemy.InTargetSight(enemyPosInScreen);
					}
				}
			}
		}
	}

	public void UntargetAllEnemies()
	{
		foreach(EnemyMain enemy in enemies)
		{
			UntargetEnemy(enemy);
		}
	}

	void UntargetEnemy(EnemyMain enemy)
	{
		enemy.NotInTargetSight();
	}
}
