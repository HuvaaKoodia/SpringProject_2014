﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMain : EntityMain {

	AIcontroller aiController;
	public AIBase ai;

    public bool waitingForAttackPhase;

	public GameObject graphics;
	public List<BoxCollider> hitboxes;

	MeshRenderer meshRenderer;

	public int rangedRange = 3;
	public int rangedAngleMax = 50;

	float normalMovementSpeed;
	float normalTurnSpeed;
	float culledSpeedMultiplier = 7;

	public bool Dead { get; protected set; }

	// Use this for initialization
	public override void Awake()
	{
		base.Awake();

		if (graphics != null)
			meshRenderer = graphics.GetComponent<MeshRenderer>();

		Dead = false;

		aiController = GC.aiController;
		ai = transform.root.GetComponent<AIBase>();

		normalMovementSpeed = movement.movementSpeed;
		normalTurnSpeed = movement.turnSpeed;
	}

	void Start ()
    {
        waitingForAttackPhase = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

	public virtual void StartEnemyTurn()
	{
		ai.Reset();
        waitingForAttackPhase = false;
		MovedLastPhase = false;
        //Debug.Log("new ai turn");
	}

    public void PlayTurn()
    {
		if (Dead)
			return;

        if (!ai.HasUsedTurn)
        {
            if (ai.AP > 0)
			{
				ai.PlayAiTurn();

				if (movement.currentMovement == MovementState.NotMoving)
				{

					FinishedMoving(false);
				}
				else
				{
					StartMoving();
				}
			}
            else
                OutOfAP();
        }
		else
		{
			FinishTurn();
		}
    }

    public void StartMoving()
    {
    	movement.StartMoving();

        if (ai.AP <= 0)
            FinishedMoving(true);

        ai.HasUsedTurn = false;
        ai.foundMove = false;
    }

    void OutOfAP()
    {
        ai.HasUsedTurn = true;
		FinishTurn();
    }

	public override void FinishedMoving(bool wontMoveAnymore)
    {
		FinishTurn();
    }

	public void FinishedAttacking()
	{
		FinishTurn();
	}

	/*
    public void PlayAttackingPhase()
    {
		ai.PlayAttackPhase();
    }

	public void FinishedAttacking()
	{
        aiController.EnemyFinishedAttacking();
	}
	*/

	public override void TakeDamage(int damage)
	{
		Health -= damage;

		//temp
		if (meshRenderer != null)
		{
	        Color oldColor = meshRenderer.material.color;
			Color newColor = new Color(oldColor.r, oldColor.g-0.2f, oldColor.b-0.2f);
				meshRenderer.material.color = newColor;
		}

        if (Health <= 0)
		{
			Die();
		}
	}

	protected virtual void Die()
	{
		Dead = true;
		Remove();
	}

	void Remove()
	{
		movement.GetCurrenTile().LeaveTile();
		CurrentFloor.Enemies.Remove(this);
		Destroy(this.gameObject);
	}

    public bool HasUsedTurn()
    {
        return ai.HasUsedTurn;
    }

	void FinishTurn()
	{
		ai.blackboard.InformedOfPlayer = false;
		aiController.EnemyFinishedTurn(this);
	}

	public void CullShow()
	{
		graphics.SetActive(true);

		movement.movementSpeed = normalMovementSpeed;
		movement.turnSpeed = normalTurnSpeed;
	}

	public void CullHide()
	{
		graphics.SetActive(false);

		movement.movementSpeed = normalMovementSpeed * culledSpeedMultiplier;
		movement.turnSpeed = normalTurnSpeed * culledSpeedMultiplier;
	}

	void OnParticleCollision(GameObject other)
	{
		int lol;
		lol = 0;
		lol++;
	}
}
