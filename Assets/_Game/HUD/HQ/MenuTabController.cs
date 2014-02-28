using UnityEngine;
using System.Collections;

public class MenuTabController : MonoBehaviour {

    public GameObject[] TabMenus;

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
