using UnityEngine;
using System.Collections;

public class CreateOnCollisionEnter : MonoBehaviour {
	
	public GameObject prefab;

	void OnCollisionEnter(Collision other){
		Instantiate(prefab,transform.position,Quaternion.identity);
	}
}
