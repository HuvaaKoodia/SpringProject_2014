﻿using UnityEngine;
using TreeSharp;
using System.Collections;
using System.Collections.Generic;

public class GatlingAI : AIBase {

	EntityMovementSub movement;

	public const int APmax = 2;
	public const int AttackCost = 2;
	
	public int Damage = 20;

	public int TurnsBeforeClose = 2;

	public LayerMask PlayerSeeMask;
	
	Point3D MyPosition;

	public bool open { get; private set;}

	public Animation baseAnimation;
	public Animation turretAnimation;

	public string GunAppearAnimation;
	public string GunHideAnimation;
	public string GunShootAnimation;

	public string BaseOpenAnimation;
	public string BaseCloseAnimation;

	public GameObject spotLightObject;
	public Light spotLight;
	public Color spotColorPlayerSeen;
	public Color spotColorPlayerNotSeen;

	public Light pointLight;
	public Color pointColorPlayerSeen;
	public Color pointColorPlayerNotSeen;

	public GameObject RedHalo;
	public GameObject GreenHalo;

	Quaternion lookToPlayerRot;

	public Transform movingPartTransform;
	public Transform turretTransform;

	public float horizontalTurnSpeed = 120;
	public float verticalTurnSpeed = 90;

	public AudioClip ShootSoundFX;
    public AudioClip OpenSoundFX;
    public AudioClip CloseSoundFX;
    public AudioClip VerticalRotationSoundFX;

	public List<ParticleSystem> shootEffects;
	public MuzzleFlashSystem muzzleFlashSys;

	// Use this for initialization
	void Start()
	{
		parent = gameObject.GetComponent<EnemyMain>();

		tilemap = parent.CurrentFloor.TileMainMap;
		
		movement = parent.movement;
		
		blackboard.owner = this;
		player = parent.GC.Player;

		parent.movement.turnSpeed = horizontalTurnSpeed;

		HasUsedTurn = false;
		foundMove = false;
		Animating = false;

		AP = APmax;

		open = false;

		MyPosition = new Point3D(movement.currentGridX, movement.currentGridY);

		LightsOff();
		
		CreateBehaviourTree();
		behaviourTree.Start (null);
	}

	public override void PlayAiTurn()
	{
		if (AP == 0 || Animating || Rotating)
			return;

		behaviourTree.Tick(blackboard);
		
		if (behaviourTree.LastStatus != RunStatus.Running)
		{
			behaviourTree.Stop(blackboard);
			behaviourTree.Start(blackboard);
		}
	}
	
	public override void Reset()
	{
		AP = APmax;
		HasUsedTurn = false;
		foundMove = false;
	}

	private RunStatus CheckForPlayerPresence()
	{
		if (PathFinder.CanSeeFromTileToTile(player, parent, MyPosition, parent.rangedRange*MapGenerator.TileSize.x, PlayerSeeMask, false))
		{
			if (blackboard.AwareOfPlayer)
			{
				GreenHalo.SetActive(true);
				RedHalo.SetActive(false);
			}

			blackboard.AwareOfPlayer = true;
			blackboard.TurnsWithoutPlayerAwareness = 0;
			blackboard.LastKnownPlayerPosition = new Point3D(player.movement.currentGridX, player.movement.currentGridY);
			spotLight.color = spotColorPlayerSeen;
			pointLight.color = pointColorPlayerSeen;

			return RunStatus.Success;
		}

		blackboard.AwareOfPlayer = false;
		blackboard.TurnsWithoutPlayerAwareness++;
		blackboard.Berserk = false;
		AP = 0;

		spotLight.color = spotColorPlayerNotSeen;
		pointLight.color = pointColorPlayerNotSeen;
		GreenHalo.SetActive(false);
		RedHalo.SetActive(true);

		return RunStatus.Failure;
	}

	void Open()
	{
		if (!open)
		{
			GatlingEnemySub parentGatling = parent as GatlingEnemySub;
			parentGatling.SwitchHitboxes(true);

			open = true;
			Animating = true;
			Invoke("AnimationFinished", turretAnimation[GunAppearAnimation].clip.length);
			Invoke("LightsOn", turretAnimation[GunAppearAnimation].clip.length - 0.2f);

			baseAnimation[BaseOpenAnimation].normalizedTime = 0;
			baseAnimation[BaseOpenAnimation].speed = 1;
			baseAnimation.Play(BaseOpenAnimation);
			
			turretAnimation[GunAppearAnimation].normalizedTime = 0;
			turretAnimation[GunAppearAnimation].speed = 1;
			turretAnimation.Play(GunAppearAnimation);

            if (OpenSoundFX != null)
                audio.PlayOneShot(OpenSoundFX);
	
			StartCoroutine(MoveDown());
		}
	}

	RunStatus NeedsClosing()
	{
		if (open && blackboard.TurnsWithoutPlayerAwareness > TurnsBeforeClose)
		{
			return RunStatus.Success;
		}

		return RunStatus.Failure;
	}

	void Close()
	{
		if (open)
		{
			GatlingEnemySub parentGatling = parent as GatlingEnemySub;
			parentGatling.SwitchHitboxes(false);

			open = false;
			Animating = true;
			Invoke("AnimationFinished", turretAnimation[GunHideAnimation].clip.length);

			/*
			turretAnimation[GunHideAnimation].normalizedTime = 0;
			turretAnimation[GunHideAnimation].speed = 1;
			turretAnimation.Play(GunHideAnimation);
			*/
			StartCoroutine(FaceGround());
			StartCoroutine(MoveUp());

            if (CloseSoundFX != null)
                audio.PlayOneShot(CloseSoundFX);

			LightsOff();
		}
		
		AP = 0;
	}

	void Shoot()
	{
		Animating = true;
		Invoke("AnimationFinished", turretAnimation[GunShootAnimation].clip.length);
		turretAnimation[GunShootAnimation].normalizedTime = 0;
		turretAnimation[GunShootAnimation].speed = 1;
		turretAnimation.Play(GunShootAnimation);

		audio.PlayOneShot(ShootSoundFX);

		muzzleFlashSys.Play();

		for (int i = 0; i < shootEffects.Count; i++)
		{
			shootEffects[i].Play();
		}

		waitingForAttackToHitPlayer = true;
		StartCoroutine(DamagePlayer());
		AP -= AttackCost;
	}

	IEnumerator DamagePlayer()
	{
		while (waitingForAttackToHitPlayer)
		{
			if (!shootEffects[0].isPlaying)
				break;

			yield return null;
		}

		waitingForAttackToHitPlayer = false;
		player.TakeDamage(Damage, MyPosition.X, MyPosition.Y);
	}

	RunStatus IsOpen()
	{
		if (open)
			return RunStatus.Success;
		else
			return RunStatus.Failure;
	}

	Vector3 playerLookPosition { get { return player.PlayerCamera.transform.position + Vector3.down*0.75f; } }

	void FacePlayer()
	{
		lookToPlayerRot = Quaternion.LookRotation(playerLookPosition - turretTransform.position);
		lookToPlayerRot = Quaternion.Euler(lookToPlayerRot.eulerAngles.x-90,
		                                   lookToPlayerRot.eulerAngles.y,
		                                   lookToPlayerRot.eulerAngles.z-90);

		if (movement.currentMovement == MovementState.NotMoving)
			StartCoroutine(RotateHorizontally());

		StartCoroutine(RotateVertically());
	}

	Vector3 lastCheckedLookPos = Vector3.zero;

	RunStatus FacingPlayer()
	{
		Vector3 lookPos = playerLookPosition;

		bool check = lookPos == lastCheckedLookPos;

		lastCheckedLookPos = lookPos;

		if (check)
		{
			return RunStatus.Success;
		}

		return RunStatus.Failure;
	}

	void LightsOff()
	{
		spotLightObject.SetActive(false);
		pointLight.enabled = false;

		RedHalo.SetActive(false);
		GreenHalo.SetActive(false);
	}

	void LightsOn()
	{
		spotLightObject.SetActive(true);
		pointLight.enabled = true;

		GreenHalo.SetActive(true);
		RedHalo.SetActive(false);
	}

	protected override void CreateBehaviourTree()
	{
		Action CheckPlayerPresenceAction = new Action(action => CheckForPlayerPresence());

		Action CheckIfOpenAction = new Action(action => IsOpen());
		Action CheckIfFacingPlayer = new Action(action => FacingPlayer());
		Action ShootAction = new Action(action => Shoot());

		Action OpenAction = new Action(action => Open());
		Action FacePlayerAction = new Action(action => FacePlayer());

		Sequence AttackSequence = new Sequence(CheckIfOpenAction, CheckIfFacingPlayer, ShootAction);
		Sequence PrepareForAttackSequence = new Sequence(OpenAction, FacePlayerAction);

		PrioritySelector AttackOrOpenSelector = new PrioritySelector(AttackSequence, PrepareForAttackSequence);

		Sequence PlayerPresentSequence = new Sequence(CheckPlayerPresenceAction, AttackOrOpenSelector);

		Action CheckClosingConditions = new Action(action => NeedsClosing());
		Action CloseAction = new Action(action => Close());

		Sequence PlayerNotPresentSequence = new Sequence(CheckClosingConditions, CloseAction);

		behaviourTree = new PrioritySelector(PlayerPresentSequence, PlayerNotPresentSequence, new Action(action => SuccessReturner()));
	}

	IEnumerator RotateVertically()
	{
		while (Animating || movement.currentMovement != MovementState.NotMoving)
			yield return null;

		Rotating = true;

        if (VerticalRotationSoundFX != null)
            audio.PlayOneShot(VerticalRotationSoundFX);

		Quaternion verticalRot = Quaternion.LookRotation(playerLookPosition - turretTransform.position);
		Quaternion ninetyCCW = Quaternion.Euler(0, -90, 0);
		verticalRot *= ninetyCCW;

		Quaternion lastTransform;

		while (turretTransform.rotation != verticalRot)
		{
			lastTransform = turretTransform.rotation; 

			turretTransform.rotation = Quaternion.RotateTowards(turretTransform.rotation,
			                                                    verticalRot, horizontalTurnSpeed * Time.deltaTime);

			if (lastTransform == turretTransform.rotation)
				break;

			bool x = Subs.ApproximatelySame(turretTransform.rotation.eulerAngles.x, verticalRot.eulerAngles.x, 0.03f);
			bool y = Subs.ApproximatelySame(turretTransform.rotation.eulerAngles.y, verticalRot.eulerAngles.y, 0.03f);
			bool z = Subs.ApproximatelySame(turretTransform.rotation.eulerAngles.z, verticalRot.eulerAngles.z, 0.03f);

			if (x && y && z)
				break;
			
			yield return null;
		}

        audio.Stop();
		StopRotating();
	}

	IEnumerator MoveDown()
	{
		int frames = 40;
		Vector3 UpPoint = new Vector3(0, 0.8f, 0);
		Vector3 DownPoint = new Vector3(0, -0.5f, 0);

		for (int i = 0; i < frames; i++)
		{
			movingPartTransform.localPosition = Vector3.Lerp(UpPoint, DownPoint, i  / (float)frames);
			yield return null;
		}
	}

	IEnumerator MoveUp()
	{
		while (turretAnimation.isPlaying || Rotating)
		{
			yield return null;
		}

		int frames = 20;
		Vector3 UpPoint = new Vector3(0, 0.8f, 0);
		Vector3 DownPoint = new Vector3(0, -0.5f, 0);

		for (int i = 0; i < frames; i++)
		{
			movingPartTransform.localPosition = Vector3.Lerp(DownPoint, UpPoint, i / (float)frames);
			yield return null;
		}

		movingPartTransform.localPosition = new Vector3(0, 0.8f, 0);
		CloseFrame();
	}

	IEnumerator RotateHorizontally()
	{
		Rotating = true;

		while (Animating)
			yield return null;
	
		movement.Turn((int)lookToPlayerRot.eulerAngles.y - 90);

		Invoke("StopRotating", horizontalTurnSpeed * Time.deltaTime);
	}

	void CloseFrame()
	{
		baseAnimation[BaseCloseAnimation].normalizedTime = 0.0f;
		baseAnimation[BaseCloseAnimation].speed = 1.0f;

		baseAnimation.Play(BaseCloseAnimation);

		Invoke("ResetRotations", baseAnimation[BaseCloseAnimation].length);
	}

	IEnumerator FaceGround()
	{
		Rotating = true;

		Quaternion lookToGround = Quaternion.LookRotation(Vector3.down, turretTransform.up);

		Quaternion ninetyCCW = Quaternion.Euler(0, -90, 0);
		lookToGround *= ninetyCCW;

		while (turretTransform.rotation != lookToGround)
		{
			turretTransform.rotation = Quaternion.RotateTowards(turretTransform.rotation,
			                                                    lookToGround, 90.0f * Time.deltaTime);
			
			yield return null;
		}

		StopRotating();
	}

	void StopRotating()
	{
		Rotating = false;
	}

	void ResetRotations()
	{
		turretAnimation[GunHideAnimation].speed = 10;
		turretAnimation[GunHideAnimation].normalizedTime = 1;

		turretAnimation.Play(GunHideAnimation);

		Quaternion ninetyCCW = Quaternion.Euler(0,0, 90);
		turretTransform.rotation *= ninetyCCW;
	}
}
