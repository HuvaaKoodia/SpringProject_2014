using UnityEngine;
using System.Collections;

public class CameraMouseRotation: MonoBehaviour {
	
	public Vector3 target;
	public float distance=5,x_speed=1,y_speed=1;
	
	float x=0,y=0,new_distance;

	void Start (){
		setDistance(distance);
		
		x=transform.rotation.eulerAngles.y;
		y=transform.rotation.eulerAngles.x;
	}

	void Update () {
	 	if (Input.GetMouseButton(2)){
			Rotate();
		}
				
		if (distance!=new_distance){
			distance+=Mathf.Clamp(new_distance-distance,-1,1)*Time.deltaTime;
		}
		
		//Move the camera to look at the target
		var position = target+ transform.rotation * Vector3.forward*-distance;
		transform.position = position;
	}
	
	void Rotate(){

		//Change the angles by the mouse movement
		x += Input.GetAxis("Mouse X") * x_speed * 0.02f;
		y -= Input.GetAxis("Mouse Y") * y_speed * 0.02f;
		 
		//Rotate the camera correctly
		var rotation = Quaternion.Euler(y, x, 0);
		transform.rotation = rotation;
		
	}
	
	public void setDistance(float distance){
		new_distance=distance;
	}
	/// <summary>
	/// +||- multiplier
	/// </param>
	public void setDistanceMulti(float multiplier){
		new_distance=distance+distance*multiplier;
	}
}

