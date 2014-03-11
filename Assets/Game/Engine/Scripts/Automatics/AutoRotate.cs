using UnityEngine;
using System.Collections;

public class AutoRotate : MonoBehaviour {

	public Vector3 speed;
	public Space space=Space.World;

	void Update () {
		transform.Rotate(speed,space);
	}
}
