using UnityEngine;
using System.Collections;

public class EnemyMain : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        transform.GetComponent<EntityMovementSub>().parent = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
