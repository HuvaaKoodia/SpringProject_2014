using UnityEngine;
using System.Collections;

public class EnemyInflictRangedDamageSub : MonoBehaviour {

	public AIBase ai;

	// Use this for initialization
	void OnParticleCollision(GameObject other)
	{
		if (other.name == "Player" || other.name == "Player(Clone)")
			ai.DamageParticleHitPlayer();
	}
}
