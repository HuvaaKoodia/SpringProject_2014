using UnityEngine;
using System.Collections;

public class PlayerMain : MonoBehaviour
{
	// Use this for initialization
	void Start()
    {
        transform.GetComponent<PlayerMovementSub>().parent = this.gameObject;
	}
	
	// Update is called once per frame
	void Update()
    {
	
	}
}
