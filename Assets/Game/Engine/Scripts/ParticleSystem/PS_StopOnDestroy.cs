using UnityEngine;
using System.Collections;

public class PS_StopOnDestroy : MonoBehaviour {

	// Use this for initialization
	public ParticleSystem PS;
	// Update is called once per frame
	void OnDestroy(){
		PS.Stop();
	}
}
