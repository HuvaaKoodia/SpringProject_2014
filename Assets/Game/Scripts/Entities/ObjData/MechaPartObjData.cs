using UnityEngine;
using System.Collections;

public delegate void IntEvent(int value);

public class MechaPartObjData{

	public static int MaxHP=100;//DEV.MAGNUM

	public UIEquipmentSlot.Slot Slot {get;set;}

    //serializable data
    public int HP{get; set;}
    public int HEAT{get; set;}

    public bool OVERHEAT{get; set;}
    public bool IsWeapon{get; set;}

    //getters
    public bool USABLE{get{return HP>0;}}
    public bool CHANGEABLE{get{return HEAT<XmlDatabase.WeaponChangeableHeatThreshold;}}
    public int OVERHEAT_DISPERSE_THRESHOLD{
        get{
            if (IsWeapon)
                return XmlDatabase.WeaponOverheatDisperseThreshold;
            else
                return XmlDatabase.HullOverheatDisperseThreshold;
        }
    }

	//gamedata
	public int Overheat_limit_bonus;
	public float armor_multi,cooling_multi,attack_multi;

    public IntEvent TakeHeat;

    public MechaPartObjData(){}//serializer constructor

    public MechaPartObjData(bool Weapon){
		IsWeapon=Weapon;
        ResetHP();
    }

    public void ResetHP(){
        HP=MaxHP;
    }

    public void TakeDMG(int dmg){
		HP-=(int)(dmg*(1-armor_multi));
        if(HP<0){
            HP=0;
        }
    }

    public void AddHEAT(float heat){
        AddHEAT((int)heat);
    }

    public void AddHEAT(int heat){
        HEAT+=heat;
        HEAT=Mathf.Clamp(HEAT,0,100);
        if (HEAT==100){
            OVERHEAT=true;
        }
        if (HEAT<OVERHEAT_DISPERSE_THRESHOLD){
            OVERHEAT=false;
        }
        if (heat>0&&TakeHeat!=null) TakeHeat(heat); 
    }

    /// <summary>
    /// Increases HEAT based on the weapons heat value
    /// </summary>
    public void IncreaseHEAT(InvGameItem weapon,float multi){
        if (weapon==null) return;
        AddHEAT(weapon.GetStat(InvStat.Type.Heat)._amount*multi);
    }
    /// <summary>
    /// Reduces HEAT based on the weapons cooling value
    /// </summary>
    public void ReduceHEAT(InvGameItem weapon,float multi){
        if (weapon==null) return;
        AddHEAT(-weapon.GetStat(InvStat.Type.Cooling)._amount);
    }

    //effects

    public float GetAccuracyMulti()
    {
        if (HP<MaxHP*0.5f){
            return 0.6f;
        }
        return 1f;
    }
}