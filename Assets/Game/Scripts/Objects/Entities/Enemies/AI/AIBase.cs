using UnityEngine;
using System.Collections;

using TreeSharp;

public abstract class AIBase : MonoBehaviour
{
	public EnemyMain parent;
	
	protected TileMain[,] tilemap;
	protected PlayerMain player;

	protected Composite behaviourTree;

	public AiLookupTable blackboard;

	public int AP;

	public bool HasUsedTurn;
	public bool foundMove;	

	public bool Animating;
	public bool Rotating;

	public bool waitingForAttackToHitPlayer;

	public virtual void PlayAiTurn(){}

	public virtual void Reset(){}
	protected abstract void CreateBehaviourTree();

	protected RunStatus SuccessReturner()
	{
		return RunStatus.Success;
	}
	
	protected virtual void AnimationFinished()
	{
		Animating = false;
	}

	public virtual void ReactToDamage(int weaponSlot)
	{
			parent.StartDamageReact(weaponSlot);
	}

	public virtual void DamageParticleHitPlayer()
	{
		waitingForAttackToHitPlayer = false;
	}
}
