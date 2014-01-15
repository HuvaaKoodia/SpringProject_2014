using UnityEngine;
using System.Collections;

public class PlaySoundRandom : MonoBehaviour {
	
	public AudioSource source;
	
	public AudioClip[] clips;
	
	// Use this for initialization
	void Start () {
		source.clip=clips[Random.Range(0,clips.Length)];
		source.Play();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
