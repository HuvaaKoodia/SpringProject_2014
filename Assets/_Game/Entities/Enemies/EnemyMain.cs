using UnityEngine;
using System.Collections;

public enum TargetState
{
	NotInSight, CanTarget, Targeted
}

public class EnemyMain : EntityMain {

	AIcontroller aiController;
	public TestAI ai;

    public bool waitingForAttackPhase;

	public GameObject graphics;
	public MeshRenderer renderer;

	public TargetState currentTargetState;
	UISprite targetSprite;

	// Use this for initialization
	public override void Awake()
	{
		base.Awake();

		renderer = graphics.GetComponent<MeshRenderer>();

		aiController = GC.aiController;
		ai = transform.root.GetComponent<TestAI>();

		currentTargetState = TargetState.NotInSight;
		targetSprite = GameObject.Instantiate(GC.SS.PS.InsightSprite) as UISprite;
		targetSprite.transform.parent = GC.menuHandler.targetMarkPanel.transform;
		targetSprite.panel = GC.menuHandler.targetMarkPanel;
		targetSprite.enabled = false;
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
		ai.ResetAP();
        waitingForAttackPhase = false;
        //Debug.Log("new ai turn");
	}

    public void PlayMovementTurn()
    {
        if (!ai.HasUsedTurn)
        {
            if (ai.ap > 0)
                ai.PlayMovementPhase();
            else
                OutOfAP();
        }
    }

    public void StartMoving()
    {
        if (ai.foundMove)
        {
            movement.StartMoving();
        }
        else
        {
            FinishedMoving(true);
        }

        if (ai.ap <= 0)
            FinishedMoving(true);

        ai.HasUsedTurn = false;
        ai.waitedForOthersToMoveThisTurn = false;
        ai.foundMove = false;
    }

    void OutOfAP()
    {
        ai.HasUsedTurn = true;
        FinishedMoving(true);
    }

	public override void FinishedMoving(bool wontMoveAnymore)
    {
		aiController.EnemyFinishedMoving(wontMoveAnymore);

        if (wontMoveAnymore)
		{
			//graphics.SetActive(false);
            waitingForAttackPhase = true;
		}
    }


    public void PlayAttackingPhase()
    {
		ai.PlayAttackPhase();
    }

	public void FinishedAttacking()
	{
        aiController.EnemyFinishedAttacking();
	}

	public override void TakeDamage(int damage)
	{
		Health -= damage;

		//temp
		Color oldColor = renderer.material.color;
		Color newColor = new Color(oldColor.r, oldColor.g-0.2f, oldColor.b-0.2f);

		renderer.material.color = newColor;

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

	public void InTargetSight()
	{
		Vector3 enemyPosInScreen = Camera.main.WorldToScreenPoint(transform.position);
		InTargetSight(enemyPosInScreen);
	}

	public void InTargetSight(Vector3 positionInCamera)
	{
		currentTargetState = TargetState.CanTarget;
		positionInCamera.z = 1;
		targetSprite.spriteName = "crosshair_gray";
		targetSprite.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		targetSprite.transform.position = GC.menuHandler.NGUICamera.ScreenToWorldPoint(positionInCamera);
		targetSprite.enabled = true;
	}

	public void BeingTargeted()
	{
		currentTargetState = TargetState.Targeted;
		targetSprite.spriteName = "crosshair_red";
	}
	
	public void NotInTargetSight()
	{
		currentTargetState = TargetState.NotInSight;
		targetSprite.enabled = false;
	}
}
