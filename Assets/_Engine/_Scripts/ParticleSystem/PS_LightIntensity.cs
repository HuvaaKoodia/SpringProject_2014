using UnityEngine;
using System.Collections;

public class PS_LightIntensity : MonoBehaviour {

	// Use this for initialization
	ParticleSystem PS; 
	void Start () {
		PS=GetComponent("ParticleSystem") as ParticleSystem;
	}
	
	// Update is called once per frame
	void Update (){
		light.intensity=1-(PS.time/PS.duration);
	}
}
