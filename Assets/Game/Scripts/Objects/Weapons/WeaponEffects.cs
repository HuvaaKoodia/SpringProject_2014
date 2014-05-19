using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponEffects : MonoBehaviour {

	public GameObject graphics;
	public GameObject particleParent;

	public GameObject tempParent;

	public float BulletEffectTime = 0.7f;
	public float MuzzleEffectTime = 0.8f;

	List<ParticleSystem> bulletParticles;
	int currentBulletIndex = 0;
	List<ParticleSystem> muzzleParticles;
	int currentMuzzleIndex = 0;

	Animation shootAnimation;
	string shootAnimationName = "Shoot";

	ParticleSystem.Particle[] bulletParticleArray;
	int bulletParticleCount;

	List<ParticleSystem> additionalParticles;

	public bool IsEmiting { get; private set; }

	AudioClip shootSoundFX;

	// Use this for initialization
	void Awake () {
		additionalParticles = new List<ParticleSystem>();
		shootAnimation = GetComponentInChildren<Animation>();

		bulletParticles = new List<ParticleSystem>();
		muzzleParticles = new List<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void SetParticleTempParent(GameObject tp)
	{
		tempParent = tp;
	}
	
	public void AddBulletParticles(GameObject particleEmitter)
	{
		ParticleSystem bulletParticle = particleEmitter.GetComponent<ParticleSystem>();
		BulletEffectTime = bulletParticle.duration;

		bulletParticles.Add(bulletParticle);

		particleEmitter.transform.parent = tempParent.transform;
	}

	public void AddMuzzleParticles(GameObject particleEmitter)
	{
		ParticleSystem muzzleParticle = particleEmitter.GetComponent<ParticleSystem>();
		MuzzleEffectTime = muzzleParticle.duration;

		muzzleParticles.Add(muzzleParticle);

		particleEmitter.transform.parent = tempParent.transform;
	}

	public void AddAdditionalParticleSystem(ParticleSystem particleEmitter)
	{
		additionalParticles.Add(particleEmitter);
	}

	public void SetShootSoundFX(AudioClip sfx)
	{
		shootSoundFX = sfx;
	}

	public void PlayShootAnimation()
	{
		if (shootAnimation == null) return;

		shootAnimation[shootAnimationName].normalizedTime = 0;
		shootAnimation.Play(shootAnimationName);
	}

	public void StartShootEffect()
	{
		if (bulletParticles.Count > 0)
		{
			ParticleSystem currentBullet = bulletParticles[currentBulletIndex];
			currentBullet.transform.parent = particleParent.transform;
			currentBullet.transform.position = particleParent.transform.position;
			currentBullet.transform.rotation = particleParent.transform.rotation;
			currentBullet.transform.parent = tempParent.transform;

			currentBullet.time = 0;
			currentBullet.Play();
			Invoke("StopAndChangeBullet", BulletEffectTime);
			IsEmiting = true;
		}

		if (muzzleParticles.Count > 0)
		{
			ParticleSystem currentMuzzle = muzzleParticles[currentMuzzleIndex];
			currentMuzzle.transform.parent = particleParent.transform;
			currentMuzzle.transform.position = particleParent.transform.position;
			currentMuzzle.transform.rotation = particleParent.transform.rotation;
			currentMuzzle.transform.parent = tempParent.transform;

			currentMuzzle.time = 0;
			currentMuzzle.Play();

			Invoke("StopAndChangeMuzzle", MuzzleEffectTime);
		}

		for (int i = 0; i < additionalParticles.Count; i++)
		{
			additionalParticles[i].time = 0;
			additionalParticles[i].Play();
		}

		if (shootSoundFX != null)
		{
			audio.PlayOneShot(shootSoundFX);
		}
	}

	void StopAndChangeMuzzle()
	{
		muzzleParticles[currentMuzzleIndex].Stop();
		currentMuzzleIndex = (currentMuzzleIndex+1) % muzzleParticles.Count;
	}

	void StopAndChangeBullet()
	{
		bulletParticles[currentBulletIndex].Stop();
		currentBulletIndex = (currentBulletIndex+1) % bulletParticles.Count;
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

	public bool IsShootEmiterPlaying
	{
		get
		{
			if (bulletParticles == null)
				return false;

			return bulletParticles[currentBulletIndex].isPlaying;
		}
	}
}