using UnityEngine;
using System.Collections;

public class FollowTargetRotation : MonoBehaviour {

	public Transform Target;
	
	void LateUpdate () {
		transform.rotation=Target.rotation;	
	}
}
