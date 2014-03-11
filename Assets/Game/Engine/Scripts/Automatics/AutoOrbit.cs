using UnityEngine;
using System.Collections;

public class AutoOrbit : MonoBehaviour {
	
	public Transform point;
	public Vector3 axis;
	public float speed;
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround(point.position,axis,speed*Time.deltaTime);
	}
}
