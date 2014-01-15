using UnityEngine;
using System.Collections;

public class ColliderDetector : MonoBehaviour {
	
	public string[] tags,not_tags;//DEV:change these to layers!
	
	
	public bool Colliding{get;private set;}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Colliding=false;
	}
	
	void OnCollisionStay(Collision other){
		foreach (var t in not_tags){
			if (other.collider.gameObject.tag==t)
				return;
		}
		Colliding=true;
	}
	
	void OnTriggerStay(Collider other){
		
		foreach (var t in not_tags){
			if (other.gameObject.tag==t)
				return;
		}
		bool not_cool=true;
		foreach (var t in tags){
			if (other.gameObject.tag==t)
			{
				not_cool=false;
				break;
			}
		}
		
		if (not_cool)
			return;
		
		Colliding=true;
	}
}
