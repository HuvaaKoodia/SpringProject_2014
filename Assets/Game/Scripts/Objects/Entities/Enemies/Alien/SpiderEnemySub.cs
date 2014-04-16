using UnityEngine;
using System.Collections;

public class SpiderEnemySub : EnemyMain {

	int totalDamage;

	AlienAI spiderAI;

	// Use this for initialization
	void Start () {
		totalDamage = 0;

		spiderAI = ai as AlienAI;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void StartEnemyTurn ()
	{
		if (totalDamage > 0)
		{
			spiderAI.PlayDamageAnimation(totalDamage);
			base.TakeDamage(totalDamage);

			totalDamage = 0;
		}

		base.StartEnemyTurn ();
	}

	public override void TakeDamage(int amount)
	{
		totalDamage += amount;
	}
}
