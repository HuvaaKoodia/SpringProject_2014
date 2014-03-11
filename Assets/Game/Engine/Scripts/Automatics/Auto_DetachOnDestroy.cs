using UnityEngine;
using System.Collections;

public class Auto_DetachOnDestroy : MonoBehaviour {

	public Transform Target;
	public bool ActivateCollisionObjects=false;
	
	void OnDestroy(){
		Target.parent=null;
		if (ActivateCollisionObjects){
			var c=Target.GetComponent<Collider>();
			if (c)
				c.enabled=true;
			var r=Target.GetComponent<Rigidbody>();
			if (r)
				r.isKinematic=false;
		}
	}
}
