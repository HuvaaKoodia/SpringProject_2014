using UnityEngine;
using System.Collections;

public class  LookAndSpinMovement: MonoBehaviour {
	
	public Transform target;
	public bool spin=false;
	public float spin_speed=100;
	public Vector3 additional_rotation=Vector3.zero;
	
	// Use this for initialization
	void Start () {
		setToRot();
	}
	
	// Update is called once per frame
	void Update () {
		setToRot();
	}
	void setToRot(){
		var dir_pos=target.transform.position + (target.rigidbody.velocity);

		target.transform.LookAt(dir_pos);
		if (spin)
			target.transform.Rotate(Vector3.forward*Time.time*spin_speed);
		target.transform.rotation*=Quaternion.Euler(additional_rotation);
	}
}
