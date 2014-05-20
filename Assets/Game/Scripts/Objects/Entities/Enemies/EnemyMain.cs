using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMain : EntityMain {

	public enum Type{Alien,Turret,Mecha};

	public Type MyType;

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

	public bool WaitingForDamageReaction { get; private set; }
	bool reactToDamage = false;

	// Use this for initialization
	public override void Awake()
	{
		base.Awake();

		if (graphics != null)
			meshRenderer = graphics.GetComponent<MeshRenderer>();

		Dead = false;
		reactToDamage = false;

		aiController = GC.aiController;
		ai = transform.root.GetComponent<AIBase>();

		normalMovementSpeed = movement.movementSpeed;
		normalTurnSpeed = movement.turnSpeed;
	}

	void Start ()
    {
        waitingForAttackPhase = false;
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

	public IEnumerator TakeDamage(int damage)
	{
		WaitingForDamageReaction = true;

		while (!reactToDamage)
		{
			yield return null;
		}

		WaitingForDamageReaction = false;

		Health -= damage;

		ReactToDamage(damage);
	}

	protected virtual void ReactToDamage(int amount)
	{ 
		if (Health <= 0)
		{
			Die();
		}
	}

	public void DamageReactOn()
	{
		reactToDamage = true;
	}

	public void DamageReactOff()
	{
		reactToDamage = false;
	}

	protected virtual void Die()
	{
		Dead = true;
		OnDeath();
		Remove();
	}

	void Remove()
	{
		movement.GetCurrenTile().LeaveTile();
		CurrentFloor.Enemies.Remove(this);
		OnDeath();
		Destroy(this.gameObject);
	}

	protected virtual void OnDeath(){}

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
