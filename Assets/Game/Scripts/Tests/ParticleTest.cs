using UnityEngine;
using System.Collections;

public class ParticleTest : MonoBehaviour {

	ParticleSystem PS;
	// Use this for initialization
	void Start () {
		PS=GetComponent<ParticleSystem>();
		StartCoroutine(StopParticles());
	}
	
	IEnumerator StopParticles(){
		yield return new WaitForSeconds(3);
		PS.Stop();
		Debug.Log("Stop!");
	}
}
