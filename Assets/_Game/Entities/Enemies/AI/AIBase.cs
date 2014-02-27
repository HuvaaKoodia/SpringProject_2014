using UnityEngine;
using System.Collections;

using TreeSharp;

public abstract class AIBase : MonoBehaviour
{
	protected EnemyMain parent;
	
	protected TileMain[,] tilemap;
	protected PlayerMain player;

	protected Composite behaviourTree;

	public int AP;

	public bool HasUsedTurn;
	public bool foundMove;	

	public virtual void PlayAiTurn(){}

	public virtual void Reset(){}
	protected abstract void CreateBehaviourTree();
}
