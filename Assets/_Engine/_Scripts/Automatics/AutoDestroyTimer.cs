using UnityEngine;
using System.Collections;

public class AutoDestroyTimer : MonoBehaviour {
	public float Delay;
    GameObject Target;
	void Start () {
        Destroy(Target,Delay);
	}
}
