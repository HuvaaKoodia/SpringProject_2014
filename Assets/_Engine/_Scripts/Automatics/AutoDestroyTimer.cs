using UnityEngine;
using System.Collections;

public class AutoDestroyTimer : MonoBehaviour {
	
	public GameObject Target;
	public int Delay;
	Timer time;
	// Use this for initialization
	void Start () {
		time=new Timer(Delay,destroy);
	}
	
	void Update(){
		time.Update();
	}
	
	// Update is called once per frame
	void destroy(){
		Destroy(Target);
	}
}
