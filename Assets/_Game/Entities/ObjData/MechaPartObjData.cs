using UnityEngine;
using System.Collections;

public class MechaPartObjData{
    public int HP{get;private set;}
    public int HEAT{get;private set;}

    public bool USABLE{get{return HP>0;}}
    public bool CHANGABLE{get{return HEAT<10;}}

    public MechaPartObjData(){
        HP=100;
    }

    public void TakeDMG(int dmg){
        HP-=dmg;
        if(HP<0){
            HP=0;
        }
    }

    public void AddHEAT(int heat){
        HEAT+=heat;
        HEAT=Mathf.Clamp(HEAT,0,100);
    }

    /// <summary>
    /// Increases HEAT based on the weapons heat value
    /// </summary>
    public void IncreaseHEAT(InvGameItem weapon){
        if (weapon==null) return;
        AddHEAT(weapon.GetStat(InvStat.Type.Heat)._amount);
    }
    /// <summary>
    /// Reduces HEAT based on the weapons cooling value
    /// </summary>
    public void ReduceHEAT(InvGameItem weapon){
        if (weapon==null) return;
        AddHEAT(-weapon.GetStat(InvStat.Type.Cooling)._amount);
    }
}