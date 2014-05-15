using UnityEngine;
using System.Collections;

public delegate void IntEvent(int value);

public class MechaPartObjData{

	public static int MaxHP=100,MaxHEAT=100;//DEV.MAGNUM

	public UIEquipmentSlot.Slot Slot {get;set;}

	//lazy reference
	public InvEquipmentSlot Equipment{get; set;}

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
                return XmlDatabase.WeaponOverheatEndThreshold;
            else
				return XmlDatabase.HullOverheatEndThreshold;
		}
    }

	public float ConditionPercent(){
		return Mathf.Clamp01((float)HP/MaxHP);
	}

	public float HeatPercent(){
		return Mathf.Clamp01((float)HEAT/MaxHEAT);
	}

	//gamedata
	public int Overheat_limit_bonus=0;
	public float armor_multi=0,cooling_multi=0,attack_multi=0,accuracy_multi=0;

	bool can_disperse_heat=true;//Dev.lazy shouldn't be here necessarily

    public IntEvent TakeHeat;

    public MechaPartObjData(){}//serializer constructor

	public MechaPartObjData(UIEquipmentSlot.Slot slot,bool Weapon){
		Slot=slot;
		IsWeapon=Weapon;
        ResetHP();
    }

    public void ResetHP(){
        HP=MaxHP;
    }

    public void TakeDMG(int dmg){
		HP-=(int)(dmg*(1f-armor_multi));
        if(HP<0){
            HP=0;
        }
    }

	public int GetDamage(){
		return (int)(Equipment.Item.GetStat(InvStat.Type.Damage)._amount*(1f+attack_multi));
	}
	
    public void AddHEAT(float heat){
        AddHEAT((int)heat);
    }

    public void AddHEAT(int heat){
		if (heat<0){
			if (!can_disperse_heat){
				can_disperse_heat=true;
				return;
			}
		}
		else{
			can_disperse_heat=false;
		}
        HEAT+=heat;
		var overheatlimit=MaxHEAT+Overheat_limit_bonus;
		HEAT=Mathf.Clamp(HEAT,0,overheatlimit);
		if (HEAT==overheatlimit){
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
		AddHEAT(-weapon.GetStat(InvStat.Type.Cooling)._amount*(1f+cooling_multi));
	}

    //effects
    public float GetAccuracyMulti()
    {
		if (HP <= MechaPartObjData.MaxHP*XmlDatabase.MechaPartConditionFairThreshold)
			return XmlDatabase.MechaPartConditionFairThreshold+accuracy_multi;
		if (HP <= MechaPartObjData.MaxHP*XmlDatabase.MechaPartConditionBadThreshold)
			return XmlDatabase.MechaPartConditionBadThreshold+accuracy_multi;
		return accuracy_multi*100;
    }
}