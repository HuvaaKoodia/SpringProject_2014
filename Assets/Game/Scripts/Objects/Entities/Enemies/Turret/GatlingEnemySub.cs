using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GatlingEnemySub : EnemyMain {

	public Transform TurretTransform;
	public Transform BaseTransform;
		
	public List<BoxCollider> HitboxesHidden;
	public List<BoxCollider> HitboxesVisible;
	
	// Use this for initialization
	void Start () {
		hitboxes = new List<BoxCollider>();
		HitboxesHidden = new List<BoxCollider>();
		hitboxes = HitboxesHidden;
	}
	
	// Update is called once per frame
	void Update () {
	
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
}
