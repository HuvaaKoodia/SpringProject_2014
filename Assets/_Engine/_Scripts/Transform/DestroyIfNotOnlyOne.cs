using UnityEngine;
using System.Collections;

public class DestroyIfNotOnlyOne : MonoBehaviour {
	
	void Awake(){
		foreach (var eDB in GameObject.FindGameObjectsWithTag(gameObject.tag)){
			if (eDB!=gameObject){
				DestroyImmediate(gameObject);
				break;
			}
		}
	}
}
