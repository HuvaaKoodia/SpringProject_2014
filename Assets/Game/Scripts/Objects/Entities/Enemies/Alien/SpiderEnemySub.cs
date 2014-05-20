using UnityEngine;
using System.Collections;

public class SpiderEnemySub : EnemyMain {

	AlienAI spiderAI;

	public float deathDelay = 53.0f/60.0f;

	// Use this for initialization
	void Start () {
		spiderAI = ai as AlienAI;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void StartEnemyTurn ()
	{
		if (!Dead)
			base.StartEnemyTurn ();
	}

	protected override void ReactToDamage(int amount)
	{
		if (Dead)
		{
			Invoke("Remove", deathDelay);
			OnDeath();
			spiderAI.PlayDeathAnimation();
		}
		else
		{	
			spiderAI.PlayDamageAnimation(amount);
		}
	}
}
