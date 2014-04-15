using UnityEngine;
using System.Collections;

public class HelpPanelMain : MonoBehaviour {

    int current=0;
    public GameObject[] Pages;
	
	void Start () {
		SetCurrent(0);
	}

    void Next(){
		SetCurrent(current+1);
    }

    void Prev(){
		SetCurrent(current-1);
    }

	void SetCurrent(int index){
		current=Mathf.Clamp(index,0,Pages.Length);
		foreach (var p in Pages){
			p.SetActive(p==Pages[current]);
		}
	}
}
