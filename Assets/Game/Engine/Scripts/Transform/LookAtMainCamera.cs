using UnityEngine;
using System.Collections;

public class LookAtMainCamera : MonoBehaviour {
	
	public Vector3 additional_rotation;
	public Vector3 rotation_speed;
	public float depth=0;
	
	Vector3 rotation,start_pos;
	Quaternion add_rot;
	
	// Use this for initialization
	void Start () {
		
		start_pos=transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		
		add_rot=Quaternion.Euler(additional_rotation);
		transform.LookAt(Camera.main.transform,Camera.main.transform.TransformDirection(Vector3.up));
		transform.rotation*=add_rot;
		if (rotation_speed!=Vector3.zero){
			transform.rotation*=Quaternion.Euler(rotation.x,rotation.y,rotation.z);
			rotation+=Time.deltaTime*rotation_speed;
		}
		if (depth!=0){
			transform.localPosition=start_pos+(Camera.main.transform.position-transform.position).normalized*depth;
		}
	}
}
