using UnityEngine;
using System.Collections;

public class Auto_DisableOnAwake : MonoBehaviour {
	void Awake () {
		gameObject.SetActive(false);
	}
}
