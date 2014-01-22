using UnityEngine;
using System.Collections;

public class EnemyMain : MonoBehaviour {

	AIcontroller aiController;
	public EntityMain parent;
	public EntityMovementSub movement;
	public TestAI ai;

	// Use this for initialization
	void Start ()
    {
        parent = transform.root.gameObject.GetComponent<EntityMain>();
		aiController = parent.GC.aiController;

		movement = parent.gameObject.GetComponent<EntityMovementSub>();

		ai = transform.root.GetComponent<TestAI>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

	public void StartEnemyTurn()
	{
		ai.ResetAP();
	}
	public void PlayMovementPhase()
	{
		if (!ai.HasUsedTurn)
			ai.PlayMovementPhase();
	}

	public void PlayAttackingPhase()
	{

	}

	public void StartMoving()
	{
		ai.HasUsedTurn = false;
		movement.StartMoving();
	}

    public void FinishedMoving(bool wontMoveAnymore = false)
    {
		aiController.EnemyFinishedTurn(wontMoveAnymore);
    }

	public void FinishedAttacking()
	{
	
	}
}
