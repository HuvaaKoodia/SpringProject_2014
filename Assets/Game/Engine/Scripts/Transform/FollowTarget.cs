using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour {
	
	public GameObject Target;
	public Vector3 Offset=Vector3.zero;

	public float speed = 10;
	public bool instant;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		updatePos();
	}
	
	void updatePos(){

		if (Target!=null){
			var pos=Target.transform.position+Offset;


			if (instant)
			{
				transform.position=pos;
			}
			else
			{
				transform.position = Vector3.Slerp(transform.position, pos, speed * Time.deltaTime);
			}
		}
	}
	
	public void SetTarget(GameObject target){
		Target=target;

		updatePos();
	}
}
