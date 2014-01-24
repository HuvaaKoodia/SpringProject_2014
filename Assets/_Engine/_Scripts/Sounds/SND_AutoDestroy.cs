using UnityEngine;
using System.Collections;

public class SND_AutoDestroy : MonoBehaviour {
	
	public GameObject Target;
	public AudioSource source;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (!source.isPlaying){
			Destroy(Target);
		}
	}
}
