using UnityEngine;
using System.Collections;

public class WorldObjectSound : MonoBehaviour {

	public bool ConstantLoop;

	public float WaitTimeBetweenLoops;

	float currentWaitTime;

	// Use this for initialization
	void Awake() {
		currentWaitTime = Subs.RandomFloat() * WaitTimeBetweenLoops;

		if (ConstantLoop)
		{
			audio.loop = true;
			audio.Play();
		}
		else
			audio.loop = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (ConstantLoop) return;

		currentWaitTime += Time.deltaTime;

		if (currentWaitTime > WaitTimeBetweenLoops)
		{
			audio.Play();
			currentWaitTime = 0.0f;
		}
	}
}
