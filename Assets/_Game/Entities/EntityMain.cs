using UnityEngine;
using System.Collections;

public class EntityMain : MonoBehaviour
{

    public GameController GC;

	// Use this for initialization
	void Start ()
    {
        transform.GetComponent<EntityMovementSub>().parent = this.transform;
        GC = GameObject.Find("GameSystems").GetComponent<GameController>();
	}
	
	// Update is called once per frame
	void Update()
    {
	
	}
}
