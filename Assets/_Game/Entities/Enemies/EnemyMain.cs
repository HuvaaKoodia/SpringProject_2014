using UnityEngine;
using System.Collections;

public class EnemyMain : MonoBehaviour {

    EntityMain parent;

	// Use this for initialization
	void Start ()
    {
        parent = transform.root.gameObject.GetComponent<EntityMain>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void FinishedMoving()
    {
        parent.GC.EnemyFinishedTurn();
    }
}
