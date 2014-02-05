﻿using UnityEngine;
using System.Collections;

public class PlayerObjData{

    //inventory
    public InvItemStorage Items{get;set;}
    public InvEquipmentStorage Equipment{get;set;}

	// Use this for initialization
	public PlayerObjData(){
        Items=new InvItemStorage(8,4,2);
        Equipment=new InvEquipmentStorage();
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
        EquipmentStorageSlot target=null;
        //randomize target

        //sides
        if (dir==0){
            if (Subs.RandomPercent()<30){
                target=Equipment.UpperTorso;
            }
            else
            {
                target=GetRandomWeaponRight();
            }
        }
        else
        if (dir==2){
            if (Subs.RandomPercent()<30){
                target=Equipment.UpperTorso;
            }
            else {
                target=GetRandomWeaponLeft();
            }
        }
        else//front and back
        if (dir==1||dir==3){
            if (Subs.RandomPercent()<50){
                target=Equipment.UpperTorso;
            }
            else if (Subs.RandomPercent()<50){
                target=GetRandomWeaponRight();
            }
            else{
                target=GetRandomWeaponLeft();
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

    private EquipmentStorageSlot GetRandomWeaponLeft(){
        return GetRandomWeapon(UIEquipmentSlot.Slot.WeaponLeftHand,UIEquipmentSlot.Slot.WeaponLeftShoulder);
    }

    private EquipmentStorageSlot GetRandomWeaponRight(){
        return GetRandomWeapon(UIEquipmentSlot.Slot.WeaponRightHand,UIEquipmentSlot.Slot.WeaponRightShoulder);
    }

    private EquipmentStorageSlot GetRandomWeapon(UIEquipmentSlot.Slot s1,UIEquipmentSlot.Slot s2){
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

}