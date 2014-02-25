using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerObjData{

    //inventory
    public InvItemStorage Items{get;set;}
    public InvEquipmentStorage Equipment{get;set;}

    public int Money{get;set;}

    public Dictionary<string,int> Ammo{get;set;}

    XmlDatabase rXDB;

	// Use this for initialization
	public PlayerObjData(XmlDatabase XDB){
        rXDB=XDB;

        Items=new InvItemStorage(8,4,2);
        Equipment=new InvEquipmentStorage();
        Ammo=new Dictionary<string ,int>();
        foreach(var a in XDB.AmmoTypes){
            Ammo.Add(a.Key,a.Value.StartAmount);
        }
	}

    /// <summary>
    /// Takes damage from a certain direction.
    /// Uses absolute directions.
    /// 0=right
    /// 1=front
    /// 2=left
    /// 3=back
    ///
    /// Determine real direction in PlayerMain
    /// </summary>
    public void TakeDMG(int dmg,int dir){
        //DEV.TODO 
        InvEquipmentSlot target=null;
        //randomize target

        target=Equipment.UpperTorso;

        //sides
        if (dir==0){
            if (Subs.RandomPercent()<30){
                //torso
            }
            else
            {
                var w=GetRandomWeaponRight();
                if (w!=null) target=w;
            }
        }
        else
        if (dir==2){
            if (Subs.RandomPercent()<30){
                //torso
            }
            else {
                var w=GetRandomWeaponLeft();
                if (w!=null) target=w;
            }
        }
        else//front and back
        if (dir==1||dir==3){
            if (Subs.RandomPercent()<50){
                target=Equipment.UpperTorso;
            }
            else if (Subs.RandomPercent()<50){
                var w=GetRandomWeaponRight();
                if (w!=null) target=w;
            }
            else{
                var w=GetRandomWeaponLeft();
                if (w!=null) target=w;
            }
        }

        target.ObjData.TakeDMG(dmg);

        if (target.Slot==UIEquipmentSlot.Slot.UpperTorso){
            //Randomly damage utility slots
            if (Subs.RandomPercent()<25){
                var util=Equipment.GetSlot(
                    Subs.GetRandom(new UIEquipmentSlot.Slot[]
                    {UIEquipmentSlot.Slot.Utility1,UIEquipmentSlot.Slot.Utility2,UIEquipmentSlot.Slot.Utility3,UIEquipmentSlot.Slot.Utility4}
                ));

                util.ObjData.TakeDMG(35);
            }
        }
    }

    private InvEquipmentSlot GetRandomWeaponLeft(){
        return GetRandomWeapon(UIEquipmentSlot.Slot.WeaponLeftHand,UIEquipmentSlot.Slot.WeaponLeftShoulder);
    }

    private InvEquipmentSlot GetRandomWeaponRight(){
        return GetRandomWeapon(UIEquipmentSlot.Slot.WeaponRightHand,UIEquipmentSlot.Slot.WeaponRightShoulder);
    }

    private InvEquipmentSlot GetRandomWeapon(UIEquipmentSlot.Slot s1,UIEquipmentSlot.Slot s2){
        var t1=Equipment.GetSlot(s1);
        var t2=Equipment.GetSlot(s2);
        
        if (t1.ObjData.USABLE&&Subs.RandomPercent()<50){
            return t1;
        }
        else if (t2.ObjData.USABLE){
            return t2;
        }
        return null;
    }

    public int GetAmmoAmount(string ammotype)
    {
        return Ammo[ammotype];
    }

    public AmmoXmlData GetAmmoData(string ammotype)
    {
        return rXDB.AmmoTypes[ammotype];
    }

    public void FillAmmo(string ammotype)
    {
        Ammo[ammotype]=rXDB.AmmoTypes[ammotype].MaxAmount;
    }
}
