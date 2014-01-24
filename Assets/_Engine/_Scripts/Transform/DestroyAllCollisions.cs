using UnityEngine;
using System.Collections;

public class DestroyAllCollisions : MonoBehaviour{

	void OnCollisionStay(Collision other){
		Destroy(other.collider.gameObject);
	}
	void OnTriggerStay(Collider other){
		Destroy(other.gameObject);
	}
}
