using UnityEngine;
using System.Collections;

public class EntityMain : MonoBehaviour
{
    public GameController GC;
    public EntityMovementSub movement;

	public bool BlocksMovement = true;

	int floor_index=0;
	public int CurrentFloorIndex{
		get{return floor_index;}
		set{
			floor_index=value;
			UpdateFloor();
		}
	}

	public FloorObjData CurrentFloor{
		get{return GC.GetFloor(CurrentFloorIndex);}
	}

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

    public virtual void FinishedMoving(bool wontMoveAnymore){}

	public virtual void TakeDamage(int damage){}

	protected virtual void UpdateFloor(){
		movement.UpdateFloor();
	}
}
