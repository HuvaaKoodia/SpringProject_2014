using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GatlingEnemySub : EnemyMain {

	public Transform TurretTransform;
	public Transform BaseTransform;
	public GameObject Gibs;
		
	public List<BoxCollider> HitboxesHidden;
	public List<BoxCollider> HitboxesVisible;

	public GameObject DeathSoundObj;
	
	// Use this for initialization
	void Start () {
		hitboxes = new List<BoxCollider>();
		HitboxesHidden = new List<BoxCollider>();
		hitboxes = HitboxesHidden;
	}

	public void SwitchHitboxes(bool visible)
	{
		if (visible)
		{
			hitboxes = HitboxesVisible;
		}
		else
		{
			hitboxes = HitboxesHidden;
		}
	}

	protected override void ReactToDamage(int amount)
	{
		if (Dead)
		{
			PlayDeathSound();
			Remove();
		}
		else
		{	
		}
	}

	void PlayDeathSound()
	{
		DeathSoundObj.SetActive(true);
	}

	public override void CullShow()
	{
		graphics.SetActive(true);
		
		movement.movementSpeed = normalMovementSpeed;
		movement.turnSpeed = normalTurnSpeed;
	}
	
	public override void CullHide()
	{
		if (((GatlingAI)ai).open) return;

		graphics.SetActive(false);
		
		movement.movementSpeed = normalMovementSpeed * culledSpeedMultiplier;
		movement.turnSpeed = normalTurnSpeed * culledSpeedMultiplier;
	}

	protected override void OnDeath(){
		Gibs.SetActive(true);
		Gibs.transform.parent=null;

		BaseTransform.parent=null;
	}
}
