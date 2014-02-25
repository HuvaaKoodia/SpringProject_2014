using UnityEngine;
using System.Collections;

using TreeSharp;

public abstract class AIBase : MonoBehaviour
{
	protected EnemyMain parent;
	
	protected TileMain[,] tilemap;
	protected PlayerMain player;

	protected Composite behaviourTree;

	public virtual void PlayMovementPhase(){}
	public virtual void PlayAttackPhase(){}
	protected abstract void CreateBehaviourTree();
}
