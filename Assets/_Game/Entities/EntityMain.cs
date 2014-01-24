using UnityEngine;
using System.Collections;

public class EntityMain : MonoBehaviour
{
    public GameController GC;
    public EntityMovementSub movement;

	public int health =  100;

	// Use this for initialization
	public virtual void Awake ()
    {
        GC = GameObject.Find("GameSystems").GetComponent<GameController>();
	}
	
	// Update is called once per frame
	void Update()
    {
	
	}

    public virtual void FinishedMoving(bool wontMoveAnymore) 
    { 
    }

	public virtual void TakeDamage(int damage)
	{

	}
}
