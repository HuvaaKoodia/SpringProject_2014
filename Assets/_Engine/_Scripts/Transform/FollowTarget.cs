using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour {
	
	public GameObject Target;
	public Vector3 Offset=Vector3.zero;

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
			transform.position=pos;
		}
	}
	
	public void SetTarget(GameObject target){
		Target=target;

		updatePos();
	}
}
