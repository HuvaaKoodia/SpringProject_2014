using UnityEngine;
using TreeSharp;
using System.Collections;

public class GatlingAI : AIBase {

	EntityMovementSub movement;

	public const int APmax = 2;
	public const int AppearCost = 1;
	public const int AttackCost = 2;
	
	public int Damage = 20;

	public int TurnsBeforeClose = 2;

	public const int AttackRadius = 16;
	public LayerMask PlayerSeeMask;
	
	Point3D MyPosition;

	bool open;

	public Animation baseAnimation;
	public Animation turretAnimation;

	public string appearAnimation;
	public string hideAnimation;

	public string shootAnimation;
	
	public GameObject spotLightObject;
	public Light spotLight;
	public Color spotColorPlayerSeen;
	public Color spotColorPlayerNotSeen;

	public Light pointLight;
	public Color pointColorPlayerSeen;
	public Color pointColorPlayerNotSeen;

	Quaternion lookToPlayerRot;
	public Transform turretTransform;

	// Use this for initialization
	void Start()
	{
		parent = gameObject.GetComponent<EnemyMain>();
		tilemap = parent.CurrentFloor.TileMainMap;
		
		movement = parent.movement;
		
		blackboard.owner = this;
		player = parent.GC.Player;
		
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
		if (AP == 0 || Animating)
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
		if (PathFinder.CanSeeFromTileToTile(player, parent, MyPosition, AttackRadius, PlayerSeeMask))
		{
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

		return RunStatus.Failure;
	}

	void Open()
	{
		if (!open)
		{
			open = true;
			Animating = true;
			Invoke("AnimationFinished", turretAnimation[appearAnimation].clip.length);
			Invoke("LightsOn", turretAnimation[appearAnimation].clip.length - 0.2f);

			baseAnimation[appearAnimation].normalizedTime = 0;
			baseAnimation[appearAnimation].speed = 1;
			baseAnimation.Play(appearAnimation);
			
			turretAnimation[appearAnimation].normalizedTime = 0;
			turretAnimation[appearAnimation].speed = 1;
			turretAnimation.Play(appearAnimation);
	
			AP -= AppearCost;
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
			open = false;
			Animating = true;
			Invoke("AnimationFinished", turretAnimation[hideAnimation].clip.length);

			baseAnimation[hideAnimation].normalizedTime = 1;
			baseAnimation[hideAnimation].speed = -1;
			baseAnimation.Play(hideAnimation);
			
			turretAnimation[hideAnimation].normalizedTime = 1;
			turretAnimation[hideAnimation].speed = -1;
			turretAnimation.Play(hideAnimation);

			LightsOff();
		}
		
		AP = 0;
	}

	void Shoot()
	{
		Animating = true;
		Invoke("AnimationFinished", turretAnimation[shootAnimation].clip.length);
		turretAnimation[shootAnimation].normalizedTime = 0;
		turretAnimation[shootAnimation].speed = 1;
		turretAnimation.Play(shootAnimation);

		Invoke("DamagePlayer", turretAnimation[shootAnimation].clip.length / 2.0f);
		AP -= AttackCost;
	}

	void DamagePlayer()
	{
		player.TakeDamage(Damage, MyPosition.X, MyPosition.Y);
	}

	RunStatus IsOpen()
	{
		if (open)
			return RunStatus.Success;
		else
			return RunStatus.Failure;
	}

	void FacePlayer()
	{
		lookToPlayerRot = Quaternion.LookRotation(player.transform.position - turretTransform.position);

		if (movement.currentMovement == MovementState.NotMoving)
			movement.Turn((int)lookToPlayerRot.eulerAngles.y - 90);

		//StartCoroutine(RotateVertically());
	}

	RunStatus FacingPlayer()
	{
		lookToPlayerRot = Quaternion.LookRotation(player.transform.position - turretTransform.position);

		if (movement.parentTransform.rotation.eulerAngles.y == lookToPlayerRot.eulerAngles.y)
			return RunStatus.Success;

		return RunStatus.Failure;
	}

	void LightsOff()
	{
		spotLightObject.SetActive(false);
		pointLight.enabled = false;
	}

	void LightsOn()
	{
		spotLightObject.SetActive(true);
		pointLight.enabled = true;
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
		return null;
	}
}
