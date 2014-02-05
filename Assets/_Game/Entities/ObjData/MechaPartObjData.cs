using UnityEngine;
using System.Collections;

public class MechaPartObjData{
    public int HP{get;private set;}
    public int HEAT{get;private set;}
    public bool USABLE{get;private set;}

    public MechaPartObjData(){
        USABLE=true;
        HP=100;
    }

    public void TakeDMG(int dmg){
        HP-=dmg;
        if(HP<0){
            HP=0;
            USABLE=false;
        }
    }
}