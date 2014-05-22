using UnityEngine;
using System.Collections;

public class EnemyDamageEffectSub : MonoBehaviour {

	public AIBase ai;

	public ParticleSystem BloodEffect;
	public ParticleSystem ElectricShockEffect;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnParticleCollision(GameObject other)
	{
		WeaponParticles damageMaker = other.GetComponent<WeaponParticles>();

		if (damageMaker != null && damageMaker.DidHit)
		{
			ai.ReactToDamage(damageMaker.WeaponSlot);

			if (damageMaker.EffectReference.EffectType == DamageEffectType.Blood)
			{
				BloodEffect.Play();
			}
			else
			{
				ElectricShockEffect.Play();
			}
		}
	}
}
