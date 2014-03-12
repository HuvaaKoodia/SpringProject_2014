using UnityEngine;
using System.Collections;

public class EnemyMain : EntityMain {

	AIcontroller aiController;
	public AIBase ai;

    public bool waitingForAttackPhase;

	public GameObject graphics;
	public MeshRenderer meshRenderer;

	public int rangedRange = 3;
	public int rangedAngleMax = 50;

	// Use this for initialization
	public override void Awake()
	{
		base.Awake();

		meshRenderer = graphics.GetComponent<MeshRenderer>();

		aiController = GC.aiController;
		ai = transform.root.GetComponent<AIBase>();
	}

	void Start ()
    {
        waitingForAttackPhase = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

	public void StartEnemyTurn()
	{
		ai.Reset();
        waitingForAttackPhase = false;
        //Debug.Log("new ai turn");
	}

    public void PlayTurn()
    {
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
        Color oldColor = meshRenderer.material.color;
		Color newColor = new Color(oldColor.r, oldColor.g-0.2f, oldColor.b-0.2f);

		meshRenderer.material.color = newColor;

        if (Health <= 0)
		{
			Die();
		}
	}

	void Die()
	{
		movement.GetCurrenTile().LeaveTile();
		GC.aiController.enemies.Remove(this);
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
}
