using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponMesh : MonoBehaviour {

	public GameObject graphics;
	public GameObject particleParent;

	public float ShootEffectTime = 0.7f;
	public float MuzzleEffectTime = 0.8f;

	ParticleSystem bulletParticles;
	ParticleSystem muzzleParticles;

	Animation shootAnimation;
	string shootAnimationName = "Shoot";

	ParticleSystem.Particle[] bulletParticleArray;
	int bulletParticleCount;

	List<ParticleSystem> additionalParticles;

	public bool IsEmiting { get; private set; }

	// Use this for initialization
	void Awake () {
		additionalParticles = new List<ParticleSystem>();
		shootAnimation = GetComponentInChildren<Animation>();
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	public void SetBulletParticles(GameObject particleEmitter)
	{
		setParentToEmitter(particleEmitter);
		bulletParticles = particleEmitter.GetComponent<ParticleSystem>();
		bulletParticles.enableEmission = true;
		ShootEffectTime = bulletParticles.duration;

		bulletParticles.Play();
		bulletParticles.Stop();
	}

	public void SetMuzzleParticles(GameObject particleEmitter)
	{
		setParentToEmitter(particleEmitter);
		muzzleParticles = particleEmitter.GetComponent<ParticleSystem>();
		muzzleParticles.enableEmission = true;
		MuzzleEffectTime = muzzleParticles.duration;

		muzzleParticles.Play();
		muzzleParticles.Stop();
	}

	public void SetAdditionalParticleSystem(ParticleSystem particleEmitter)
	{
		particleEmitter.enableEmission = true;

		additionalParticles.Add(particleEmitter);

		particleEmitter.Play();
		particleEmitter.Stop();
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
			bulletParticles.transform.position = particleParent.transform.position;
			bulletParticles.transform.rotation = particleParent.transform.rotation;
			bulletParticles.time = 0;
			bulletParticles.Play();
			Invoke("StopShootEffect", ShootEffectTime);
			IsEmiting = true;
		}

		if (muzzleParticles != null)
		{
			muzzleParticles.transform.position = particleParent.transform.position;
			muzzleParticles.transform.rotation = particleParent.transform.rotation;
			muzzleParticles.time = 0;
			muzzleParticles.Play();
		}

		for (int i = 0; i < additionalParticles.Count; i++)
		{
			additionalParticles[i].time = 0;
			additionalParticles[i].Play();
		}
	}

	void StopMuzzleEffect()
	{
		muzzleParticles.Stop();
	}

	void StopShootEffect()
	{
		bulletParticles.Stop();
		IsEmiting = false;
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