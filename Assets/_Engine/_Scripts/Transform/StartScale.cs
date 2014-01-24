using UnityEngine;
using System.Collections;

public class  StartScale: MonoBehaviour {
	
	public Transform Target;
	public Vector3 
		start_scale_percentages=Vector3.zero,
		end_scale_percentages=Vector3.one,
		speed=Vector3.one;
	Vector3 target_scale;
	// Use this for initialization
	void Start () {
		target_scale=Subs.Vector3Multi(Target.localScale,end_scale_percentages);
		Target.localScale=Subs.Vector3Multi(Target.localScale,start_scale_percentages);
	}
	
	// Update is called once per frame
	void Update () {
		Target.localScale+=speed*Time.deltaTime;
		Target.localScale=Subs.ClampVector3(Target.localScale,target_scale);
	}

}
