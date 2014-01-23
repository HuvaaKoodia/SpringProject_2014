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
			enemiesWontMoveAnymore = 0;

			foreach (EnemyMain enemy in enemies)
			{
				enemy.PlayMovementPhase();
			}

			foreach (EnemyMain enemy in enemies)
			{
				enemy.StartMoving();
			}

			GC.ChangeTurn(TurnState.WaitingAIToFinish);
		}
		else if (currentTurn == TurnState.WaitingAIToFinish && enemies.Count <= finishedEnemies)
		{
			finishedEnemies = 0;

            if (enemiesWontMoveAnymore < enemies.Count)
                GC.ChangeTurn(TurnState.AIMovement);
            else
            {
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
}
