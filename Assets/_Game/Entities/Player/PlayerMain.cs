﻿using UnityEngine;
using System.Collections;

public class PlayerMain : EntityMain
{
	public PlayerInputSub inputSub;

	public bool INVINCIBLE=false;
	public int ap;
	const int apMax = 2;
	const int movementCost = 1;
	const int attackCost = 2;

	// Use this for initialization
	public override void Awake()
	{
		base.Awake();
		
		GC = GameObject.Find("GameSystems").GetComponent<GameController>();
		inputSub = GetComponent<PlayerInputSub>();
	}

	void Start()
    {
		ap = apMax;
	}
	
	// Update is called once per frame
	void Update()
    {
	
	}

    public void StartPlayerPhase()
    {
		ap = apMax;
        StartTurn();
    }

    void StartTurn()
    {
        inputSub.enabled = true;
    }

    public void EndPlayerPhase()
    {
		inputSub.enabled = false;
		GC.ChangeTurn(TurnState.StartAITurn);
    }

    public override void FinishedMoving(bool wontMoveAnymore)
	{
        if (ap <= 0)
            EndPlayerPhase();
        else
            StartTurn();
	}

	public void StartedMoving()
	{
		inputSub.enabled = false;
		ap -= movementCost;
	}

	public void Attack(EnemyMain enemy)
	{
		if (ap < attackCost)
			return;

		enemy.TakeDamage(34);
		ap -= attackCost;
		
		//Debug.Log("Shot enemy");
		if (ap <= 0)
		{
			Debug.Log("AP run out after shot");
			EndPlayerPhase();
		}
		else
		{
			StartTurn();
		}
			
		
	}

	public void Attack()
	{
		if (ap < attackCost)
			return;

		Component target;
		if (Subs.GetObjectMousePos(out target, 50, "Enemy"))
	    {
			if (target.tag == "AI")
			{
				EnemyMain enemy = target.gameObject.GetComponent<EnemyMain>();
				enemy.TakeDamage(34);
				ap -= attackCost;

				//Debug.Log("Shot enemy");
				if (ap <= 0)
				{
					Debug.Log("AP run out after shot");
					EndPlayerPhase();
				}
				else
				{

					StartTurn();
				}
			}
		}
	}

	public override void TakeDamage(int damage)
	{
		if (INVINCIBLE) return;
		health -= damage;

		if (health <= 0)
		{
			Debug.Log("Kuoli saatana");
			health = 100;
			int[] pos = {1, 1};
			movement.SetPositionInGrid(pos);
		}
	}
}
