using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuTabController : MonoBehaviour {

    public List<GameObject> TabMenus;

    public void OpenTab1(){
        ActivateMenu(0);
    }

    public void OpenTab2(){
        ActivateMenu(1);
    }

    public void OpenTab3(){
        ActivateMenu(2);
    }

    public void OpenTab4(){
        ActivateMenu(3);
    }

    public void OpenTab5(){
        ActivateMenu(4);
    }

	public void OpenTab6(){
		ActivateMenu(5);
	}

    public void ActivateMenu(int index){
        foreach (var m in TabMenus){
            m.SetActive(m==TabMenus[index]?true:false);
        }
    }

    public void ActivateMenu(GameObject menu){
        foreach (var m in TabMenus){
            m.SetActive(m==menu?true:false);
        }
    }
}
