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

	public override void TakeDamage(int amount)
	{
		base.TakeDamage(amount);

		if (Dead)
		{
			Invoke("Remove", deathDelay);
			spiderAI.PlayDeathAnimation();
		}
		else
		{	
			spiderAI.PlayDamageAnimation(amount);
		}
	}

	protected override void Die()
	{
		Dead = true;
	}
}
