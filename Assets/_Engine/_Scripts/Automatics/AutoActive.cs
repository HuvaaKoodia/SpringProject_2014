using UnityEngine;
using System.Collections;

public class AutoActive : MonoBehaviour {

	
	// Update is called once per frame
	void Update () {
		rigidbody.WakeUp();
	}
}
