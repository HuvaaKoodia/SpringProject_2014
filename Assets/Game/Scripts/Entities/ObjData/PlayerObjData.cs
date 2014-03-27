using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerObjData{

    //inventory
    public InvItemStorage Items{get;set;}
    public InvEquipmentStorage Equipment{get;set;}

    public int Money{get;set;}

    public Dictionary<string,int> Ammo{get;set;}
	public MechaPartObjData[] MechParts {get; set;}
	public MechaPartObjData UpperTorso{get;set;}

	public PlayerObjData(){
        Items=new InvItemStorage(8,4,2);
        
        Ammo=new Dictionary<string ,int>();
		foreach(var a in XmlDatabase.AmmoTypes){
            Ammo.Add(a.Key,a.Value.StartAmount);
        }

		//Inventory Slots
		MechParts=new MechaPartObjData[8];
		AddPart(UIEquipmentSlot.Slot.WeaponLeftHand,true);
		AddPart(UIEquipmentSlot.Slot.WeaponRightHand,true);
		AddPart(UIEquipmentSlot.Slot.WeaponLeftShoulder,true);
		AddPart(UIEquipmentSlot.Slot.WeaponRightShoulder,true);
		
		AddPart(UIEquipmentSlot.Slot.Utility1,false);
		AddPart(UIEquipmentSlot.Slot.Utility2,false);
		AddPart(UIEquipmentSlot.Slot.Utility3,false);
		AddPart(UIEquipmentSlot.Slot.Utility4,false);
		
		//Hidden slots
		UpperTorso =new MechaPartObjData(UIEquipmentSlot.Slot.UpperTorso,false);

		Equipment=new InvEquipmentStorage(this);
		//LowerTorso =new InvEquipmentSlot(UIEquipmentSlot.Slot.LowerTorso,false);
	}

	private void AddPart(UIEquipmentSlot.Slot slot,bool weapon){
		MechParts[(int)slot]=new MechaPartObjData(slot,weapon);
	}

	public MechaPartObjData GetPart (UIEquipmentSlot.Slot slot)
	{
		return MechParts[(int)slot];
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
        MechaPartObjData target=null;
        //randomize target

        target=UpperTorso;

        //sides
        if (dir==0){
			if (Subs.RandomPercent()<XmlDatabase.AttackLeftRightPlayerTorsoHitChance){
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
			if (Subs.RandomPercent()<XmlDatabase.AttackLeftRightPlayerTorsoHitChance){
                //torso
            }
            else {
                var w=GetRandomWeaponLeft();
                if (w!=null) target=w;
            }
        }
        else//front and back
        if (dir==1||dir==3){
			if (Subs.RandomPercent()<XmlDatabase.AttackFrontBackPlayerTorsoHitChance){
                target=UpperTorso;
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
        target.TakeDMG(dmg);

        if (target.Slot==UIEquipmentSlot.Slot.UpperTorso){
            //Randomly damage utility slots
			if (Subs.RandomPercent()<XmlDatabase.AttackTorsoUtilityDamageChance){
                var util=GetPart(
                    Subs.GetRandom(new UIEquipmentSlot.Slot[]
                    {UIEquipmentSlot.Slot.Utility1,UIEquipmentSlot.Slot.Utility2,UIEquipmentSlot.Slot.Utility3,UIEquipmentSlot.Slot.Utility4}
                ));

                util.TakeDMG(35);
            }
        }
    }

	private MechaPartObjData GetRandomWeaponLeft(){
        return GetRandomWeapon(UIEquipmentSlot.Slot.WeaponLeftHand,UIEquipmentSlot.Slot.WeaponLeftShoulder);
    }

	private MechaPartObjData GetRandomWeaponRight(){
        return GetRandomWeapon(UIEquipmentSlot.Slot.WeaponRightHand,UIEquipmentSlot.Slot.WeaponRightShoulder);
    }

    private MechaPartObjData GetRandomWeapon(UIEquipmentSlot.Slot s1,UIEquipmentSlot.Slot s2){
        var t1=GetPart(s1);
		var t2=GetPart(s2);
        
        if (t1.USABLE&&Subs.RandomPercent()<50){
            return t1;
        }
        else if (t2.USABLE){
            return t2;
        }
        return null;
    }

    public int GetAmmoAmount(string ammotype)
    {
        return Ammo[ammotype];
    }
    public int SetAmmoAmount(string ammotype,int amount)
    {
        return Ammo[ammotype]=amount;
    }

    public AmmoXmlData GetAmmoData(string ammotype)
    {
		return XmlDatabase.AmmoTypes[ammotype];
    }

    public void FillAmmo(string ammotype)
    {
		Ammo[ammotype]=XmlDatabase.AmmoTypes[ammotype].MaxAmount;
    }
}
