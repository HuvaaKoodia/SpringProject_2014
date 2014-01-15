using UnityEngine;
using System.Collections;

public class PS_AutoDestroy : MonoBehaviour {
	
	public ParticleSystem PS;
	
	void Update () {
		if (PS.isStopped)
			Destroy(gameObject);
	}
}
