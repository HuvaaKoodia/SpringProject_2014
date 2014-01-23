using UnityEngine;
using System.Collections;

public class EnemyMain : EntityMain {

	AIcontroller aiController;
	public TestAI ai;

    public bool waitingForAttackPhase;

	public GameObject graphics;
	public MeshRenderer renderer;

	// Use this for initialization
	public override void Awake()
	{
		base.Awake();

		renderer = graphics.GetComponent<MeshRenderer>();

		aiController = GC.aiController;
		ai = transform.root.GetComponent<TestAI>();
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
		//graphics.SetActive(true);
        waitingForAttackPhase = false;
	}

	public void PlayMovementPhase()
	{
		if (!ai.HasUsedTurn)
			ai.PlayMovementPhase();
	}

	public void StartMoving()
	{
		ai.HasUsedTurn = false;
		movement.StartMoving();
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
		health -= damage;

		//temp
		Color oldColor = renderer.material.color;
		Color newColor = new Color(oldColor.r, oldColor.g-0.2f, oldColor.b-0.2f);

		renderer.material.color = newColor;

		if (health <= 0)
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
}
