using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GatlingEnemySub : EnemyMain {

	public Transform TurretTransform;
	public Transform BaseTransform;
	public GameObject Gibs;
		
	public List<BoxCollider> HitboxesHidden;
	public List<BoxCollider> HitboxesVisible;
	
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
			Remove();
		}
		else
		{	
		}
	}

	protected override void OnDeath(){
		Gibs.SetActive(true);
		Gibs.transform.parent=null;

		BaseTransform.parent=null;
	}
}
