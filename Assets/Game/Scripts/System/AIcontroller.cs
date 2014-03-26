﻿using UnityEngine;
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

	public AIcontroller(GameController gc)
	{
		this.GC = gc;
		enemies = new List<EnemyMain>();
		finishedEnemies = new List<EnemyMain>();
		enemiesFinishedTurn = 0;
		turnsPlayed = 0;
	}

	// Update is called once per frame
	public void UpdateAI(TurnState currentTurn)
	{/*
		if (currentTurn == TurnState.AIMovement)
		{
	        foreach (EnemyMain enemy in enemies)
	        {
	            enemy.PlayMovementTurn();

	            if (!enemy.HasUsedTurn())
					didntUseTurn.Add(enemy);
	        }

			for (int i = didntUseTurn.Count-1; i >= 0; i--)
				didntUseTurn[i].PlayMovementTurn();

			didntUseTurn.Clear();

            foreach (EnemyMain enemy in enemies)
            {
                enemy.StartMoving();
            }
   

			GC.ChangeTurn(TurnState.WaitingAIToFinish);
		}
        else if (currentTurn == TurnState.WaitingAIToFinish && !CheckForMovingEnemies())
		{
            if (finishedEnemies.Count < enemies.Count)
            {
                GC.ChangeTurn(TurnState.AIMovement);
            }
            else
            {
				finishedEnemies.Clear();

				if (enemies.Count != 0)
                	enemies[currentAttacker].PlayAttackingPhase();

                GC.ChangeTurn(TurnState.AIAttack);
            }
		}
		else if (currentTurn == TurnState.AIAttack && currentAttacker >= enemies.Count)
		{
            currentAttacker = 0;
			GC.ChangeTurn(TurnState.StartPlayerTurn);
		}*/
		if (currentTurn == TurnState.AITurn)
		{
			bool enemyAnimating = false;

			foreach(EnemyMain enemy in enemies)
			{
				enemy.PlayTurn();

				if (enemy.ai.Animating || enemy.ai.Rotating)
					enemyAnimating = true;
			}

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
					Debug.Log("AI new turn");
					enemiesFinishedTurn = 0;
					finishedEnemies.Clear();
					GC.ChangeTurn(TurnState.AITurn);
				}
				else
					GC.ChangeTurn(TurnState.StartPlayerTurn);
			}
		}
		else if (currentTurn == TurnState.StartAITurn)
		{
			turnsPlayed = 0;
			enemiesFinishedTurn = 0;
			finishedEnemies.Clear();
			Debug.Log("AI start");
			foreach (EnemyMain enemy in enemies)
			{
				enemy.StartEnemyTurn();
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