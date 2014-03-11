using UnityEngine;
using System.Collections;

public class AutoTilt : MonoBehaviour {
	
	public float max_angle=35,speed_multi=1;
	float angle=0;
	
	// Update is called once per frame
	void Update () {
		angle=(Mathf.Deg2Rad*max_angle)*Mathf.Cos(Time.time*speed_multi);
		transform.Rotate(Vector3.up,angle);
	}
}
