using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIcontroller
{
	GameController GC;

	List<EnemyMain> enemies;
	int enemiesFinishedTurn;

	public const int MaxTurnsPerPhase = 5;
	int	turnsPlayed;

	public List<EnemyMain> finishedEnemies;

	bool HaxMaxEnemyWaitTimerOn = true;
	Dictionary<EnemyMain, int> enemyAP;
	float timeWaitedForEnemies;
	public float EnemyWaitTime = 4.0f;
	float lastAiTurnTime;

	public AIcontroller(GameController gc)
	{
		this.GC = gc;
		enemies = new List<EnemyMain>();
		finishedEnemies = new List<EnemyMain>();
		enemyAP = new Dictionary<EnemyMain, int>();
		enemiesFinishedTurn = 0;
		turnsPlayed = 0;
	}

	// Update is called once per frame
	public void UpdateAI(TurnState currentTurn)
	{
		if (currentTurn == TurnState.AITurn)
		{
			bool enemyAnimating = false;
			bool situationChanged = false;

			for (int i = enemies.Count-1; i >= 0; i--)
			{
				EnemyMain enemy = enemies[i];

				enemy.PlayTurn();

				if (enemy != null && (enemy.ai.Animating || enemy.ai.Rotating || enemy.GetWaitingForDamageReaction() || enemy.ai.waitingForAttackToHitPlayer))
				{
					enemyAnimating = true;

					if (enemyAP.ContainsKey(enemy))
					{
						if (enemyAP[enemy] != enemy.ai.AP)
						{
							enemyAP[enemy] = enemy.ai.AP;
							situationChanged = true;
						}
					}
					else
					{
						enemyAP.Add(enemy, enemy.ai.AP);
						situationChanged = true;
					}
				}
			}


			if (situationChanged)
			{
				timeWaitedForEnemies = 0.0f;
			}
			else
			{
				timeWaitedForEnemies += Time.time - lastAiTurnTime;
			}

			lastAiTurnTime = Time.time;

			if (!enemyAnimating)
				turnsPlayed++;

			GC.ChangeTurn(TurnState.WaitingAIToFinish);
		}
		else if (currentTurn == TurnState.WaitingAIToFinish)
		{
			if (enemiesFinishedTurn >= enemies.Count)
			{
				if (turnsPlayed < MaxTurnsPerPhase)
				{
					//Debug.Log("AI new turn");
					enemiesFinishedTurn = 0;
					finishedEnemies.Clear();
					GC.ChangeTurn(TurnState.AITurn);
				}
				else
					GC.ChangeTurn(TurnState.StartPlayerTurn);
			}

			if (HaxMaxEnemyWaitTimerOn && timeWaitedForEnemies >= EnemyWaitTime)
			{
				GC.ChangeTurn(TurnState.StartPlayerTurn);
			}
		}
		else if (currentTurn == TurnState.StartAITurn)
		{
			turnsPlayed = 0;
			enemiesFinishedTurn = 0;
			finishedEnemies.Clear();

			enemyAP.Clear();
			timeWaitedForEnemies = 0.0f;
			lastAiTurnTime = Time.time;

			//Debug.Log("AI start");
			for (int i = enemies.Count-1; i >= 0; i--)
			{
				enemies[i].StartEnemyTurn();
			}
			GC.ChangeTurn(TurnState.AITurn);
		}
	}

	/*
	public void EnemyFinishedMoving(bool wontMoveAnymore, EnemyMain enemy)
	{
		if (wontMoveAnymore)
		{
			bool added = finishedEnemies.Add(enemy);
			int i = 0;
			i++;
		}
	}
	*/

	public void EnemyFinishedTurn(EnemyMain enemy)
	{
		enemiesFinishedTurn++;
		finishedEnemies.Add(enemy);

		//Debug.Log("Enemy finished at pos " + enemy.movement.currentGridX + "," + enemy.movement.currentGridY);
	}

	public void SetFloor(FloorObjData floor){
		enemies=floor.Enemies;
	}
}
