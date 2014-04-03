using UnityEngine;
using System.Collections;

public class HelpPanelMain : MonoBehaviour {

    int current=0;
    public GameObject[] Pages;

	// Use this for initialization
	void Start () {
        Pages[0].SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Next(){
        current++;
        if (current>Pages.Length-1) current=0;

        foreach (var p in Pages){
            p.SetActive(false);
        }

        Pages[current].SetActive(true);
    }

    void Prev(){
        current--;
        if (current<0) current=Pages.Length-1;

        foreach (var p in Pages){
            p.SetActive(false);
        }
        
        Pages[current].SetActive(true);
    }


}
