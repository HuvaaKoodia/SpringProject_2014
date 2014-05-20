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

	bool[] WaitingForDamageReaction;

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

		WaitingForDamageReaction = new bool[] { false, false, false, false };
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

	public IEnumerator TakeDamage(int damage, int weaponSlot)
	{
		Health -= damage;

		if (Health <= 0)
		{
			Dead = true;
		}

		WaitingForDamageReaction[weaponSlot] = true;

		float timeWaited = 0.0f;

		while (WaitingForDamageReaction[weaponSlot])
		{
			timeWaited += Time.deltaTime;

			if (timeWaited > 0.9f)
				break;

			yield return null;
		}

		ReactToDamage(damage);
		WaitingForDamageReaction[weaponSlot] = false;
	}

	protected virtual void ReactToDamage(int amount)
	{ 
	}

	public void StartDamageReact(int weaponSlot)
	{
		WaitingForDamageReaction[weaponSlot] = false;
	}

	public bool GetWaitingForDamageReaction(int weaponSlot)
	{
		return WaitingForDamageReaction[weaponSlot];
	}

	public bool GetWaitingForDamageReaction()
	{
		return WaitingForDamageReaction[0] ||
			   WaitingForDamageReaction[1] ||
			   WaitingForDamageReaction[2] ||
			   WaitingForDamageReaction[3];
	}

	protected void Remove()
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
}
