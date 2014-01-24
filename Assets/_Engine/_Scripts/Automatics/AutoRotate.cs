using UnityEngine;
using System.Collections;

public class AutoRotate : MonoBehaviour {
	
	
	public Vector3 speed;
	public Space space=Space.World;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(speed,space);
	}
}
