using UnityEngine;
using System.Collections;

public class EntityMain : MonoBehaviour
{
    public GameController GC;
    public EntityMovementSub movement;

    private int health;
    public int Health
    {
        get { return health; }
        set
        {
            health = Mathf.Clamp(value, 0, maxHealth);
        }
    }
    public int maxHealth = 100;

	// Use this for initialization
	public virtual void Awake ()
    {
        health = maxHealth;
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
