using UnityEngine;
using System.Collections;

public class PlayerMain : EntityMain
{
	public PlayerInputSub inputSub;

	public bool targetingMode { get; private set; }

	public bool INVINCIBLE=false;
	public int ap;
	const int apMax = 2;
	const int movementCost = 1;
    const int lootPickupCost = 1;
	const int attackCost = 2;

	// Use this for initialization
	public override void Awake()
	{
		base.Awake();
		
		GC = GameObject.Find("GameSystems").GetComponent<GameController>();
		inputSub = GetComponent<PlayerInputSub>();

		targetingMode = false;
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
		EndTargetingMode();
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

		enemy.TakeDamage(50);
        //ap -= attackCost;

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

    public void PickupLoot(GameObject loot)
    {
        if (ap < lootPickupCost)
            return;

        Destroy(loot);

        Health += 20;
    }

	public override void TakeDamage(int damage)
	{
		if (INVINCIBLE) return;
		Health -= damage;

        if (Health <= 0)
		{
			Debug.Log("Kuoli saatana");
            Health = 100;
			int[] pos = {1, 1};
			movement.SetPositionInGrid(pos);
		}
	}

	public void StartTargetingMode()
	{
		targetingMode = true;
		GC.aiController.CheckTargetableEnemies(Camera.main.transform.position);
		GC.menuHandler.CheckTargetingModePanel();
	}

	public void EndTargetingMode()
	{
		targetingMode = false;
		GC.aiController.UntargetAllEnemies();
		GC.menuHandler.CheckTargetingModePanel();
	}
}
