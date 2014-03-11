using UnityEngine;
using System.Collections;

public class Auto_DisableOnAwake : MonoBehaviour {
    public GameObject Target;
	void Awake () {
        Target.SetActive(false);
	}
}
