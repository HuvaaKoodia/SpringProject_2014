using UnityEngine;
using System.Collections;

public class WeaponMesh : MonoBehaviour {

	public GameObject graphics;
	public GameObject particleParent;

	public float ShootEffectTime = 0.7f;
	public float MuzzleEffectTime = 0.8f;

	ParticleSystem bulletParticles;
	ParticleSystem muzzleParticles;

	Animation shootAnimation;
	string shootAnimationName = "Shoot";

	// Use this for initialization
	void Start () {
		shootAnimation = GetComponentInChildren<Animation>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void SetBulletParticles(GameObject particleEmitter)
	{
		setParentToEmitter(particleEmitter);
		bulletParticles = particleEmitter.GetComponent<ParticleSystem>();
	}

	public void SetMuzzleParticles(GameObject particleEmitter)
	{
		setParentToEmitter(particleEmitter);
		muzzleParticles = particleEmitter.GetComponent<ParticleSystem>();
	}

	public void SetEffectTimes(float bulletTime, float muzzleTime)
	{
		ShootEffectTime = bulletTime;
		MuzzleEffectTime = muzzleTime;
	}

	public void PlayShootAnimation()
	{
		if (shootAnimation == null) return;

		shootAnimation[shootAnimationName].normalizedTime = 0;
		shootAnimation.Play(shootAnimationName);
	}

	public void StartShootEffect()
	{
		if (bulletParticles != null)
		{
			bulletParticles.Play();
			Invoke("StopShootEffect",ShootEffectTime);
		}

		if (muzzleParticles != null)
		{
			muzzleParticles.Play();
			Invoke("StopMuzzleEffect", MuzzleEffectTime);
		}
	}

	void StopMuzzleEffect()
	{
		muzzleParticles.Stop();
	}

	void StopShootEffect()
	{
		bulletParticles.Stop();
	}

	public bool IsEmiting
	{
		get 
		{ 
			if (bulletParticles == null)
				return false;

			return bulletParticles.isPlaying; 
		}
	}

	public bool IsAnimating
	{
		get
		{ 
			if (shootAnimation == null)
				return false;

			return shootAnimation.isPlaying;
		}
	}

	void setParentToEmitter(GameObject particleEmitter)
	{
		particleEmitter.transform.parent = particleParent.transform;
		particleEmitter.transform.localPosition = Vector3.zero;
		particleEmitter.transform.localRotation = Quaternion.identity;
		particleEmitter.transform.localScale = Vector3.one;
	}
}