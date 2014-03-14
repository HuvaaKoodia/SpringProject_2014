using UnityEngine;
using System.Collections.Generic;

public class InvEquipmentSlot{
	public UIEquipmentSlot.Slot Slot {get{return ObjData.Slot;}}
	public List<InvBaseItem.Type> TypeList {get; set;}
	public InvGameItem Item{get;set;}

	//reference
	public MechaPartObjData ObjData{get; set;}

	public InvEquipmentSlot(){}

	public InvEquipmentSlot(MechaPartObjData data,params InvBaseItem.Type[] types){
		ObjData=data;

		TypeList=new List<InvBaseItem.Type>();
		foreach (var t in types){
			TypeList.Add(t);
		}
	}

	public bool HasType(InvBaseItem.Type type)
	{
		return TypeList.Contains(type);
	}
}

public class InvEquipmentStorage
{
    //serializable data
    public InvEquipmentSlot UpperTorso{get; set;}
    public InvEquipmentSlot LowerTorso{get; set;}
    public InvEquipmentSlot[] EquipmentSlots {get; set;}

	public InvEquipmentStorage(){}

    public InvEquipmentStorage(PlayerObjData player){
		//Inventory Slots
        EquipmentSlots=new InvEquipmentSlot[8];
        AddSlot(player.GetPart(UIEquipmentSlot.Slot.WeaponLeftHand),InvBaseItem.Type.LightWeapon,InvBaseItem.Type.MeleeWeapon);
		AddSlot(player.GetPart(UIEquipmentSlot.Slot.WeaponRightHand),InvBaseItem.Type.LightWeapon,InvBaseItem.Type.MeleeWeapon);
        AddSlot(player.GetPart(UIEquipmentSlot.Slot.WeaponLeftShoulder),InvBaseItem.Type.HeavyWeapon,InvBaseItem.Type.LightWeapon);
        AddSlot(player.GetPart(UIEquipmentSlot.Slot.WeaponRightShoulder),InvBaseItem.Type.HeavyWeapon,InvBaseItem.Type.LightWeapon);

        AddSlot(player.GetPart(UIEquipmentSlot.Slot.Utility2),InvBaseItem.Type.Utility,InvBaseItem.Type.Radar,InvBaseItem.Type.Navigator);
        AddSlot(player.GetPart(UIEquipmentSlot.Slot.Utility3),InvBaseItem.Type.Utility,InvBaseItem.Type.Radar,InvBaseItem.Type.Navigator);
        AddSlot(player.GetPart(UIEquipmentSlot.Slot.Utility4),InvBaseItem.Type.Utility,InvBaseItem.Type.Radar,InvBaseItem.Type.Navigator);
        AddSlot(player.GetPart(UIEquipmentSlot.Slot.Utility1),InvBaseItem.Type.Utility,InvBaseItem.Type.Radar,InvBaseItem.Type.Navigator);

        //Hidden slots
        UpperTorso =new InvEquipmentSlot(player.UpperTorso);
        //LowerTorso =new InvEquipmentSlot(UIEquipmentSlot.Slot.LowerTorso,false);

        foreach(var s in EquipmentSlots){
            s.ObjData.TakeHeat+=HullTakeHeat;
        }
	}


    private void HullTakeHeat(int heat){
        UpperTorso.ObjData.AddHEAT(heat*XmlDatabase.HullHeatAddMultiplier);
    }

    private void AddSlot(MechaPartObjData data,params InvBaseItem.Type[] types){
		var eqp=new InvEquipmentSlot(data,types);
		EquipmentSlots[(int)data.Slot]=eqp;
		data.Equipment=eqp;
	}

    public InvEquipmentSlot GetSlot(UIEquipmentSlot.Slot slot){
		return EquipmentSlots[(int)slot];
	}

    public InvGameItem Replace (UIEquipmentSlot.Slot slot, InvGameItem item)
    {
        var Slot=EquipmentSlots[(int)slot];
        
        if (item==null){
            var prev=Slot.Item;
            Slot.Item=null;
            return prev;
        }
        else{
            // If the item is not of appropriate type, we shouldn't do anything
            if (!Slot.HasType(item.baseItem.type)) return item;
            
            // Equip this item
            var prev = Slot.Item;
            Slot.Item=item;
            
            return prev;
        }
    }

	/// <summary>
	/// Equip the specified item automatically replacing an existing one.
	/// </summary>

	public bool CanReplace (UIEquipmentSlot.Slot slot, InvGameItem item)
	{
		var Slot=EquipmentSlots[(int)slot];

		if (item==null){
			return true;
		}
		else{
			// If the item is not of appropriate type, we shouldn't do anything
			if (!Slot.HasType(item.baseItem.type)) return false;
		}
        return true;
	}

	/// <summary>
	/// Equips the specified item in a free slot. Returns the item that was replaced.
	/// </summary>

	public InvGameItem Equip (InvGameItem item)
	{
		if (item == null) return item;
		InvBaseItem baseItem = item.baseItem;
		if (baseItem == null) return item;

		//find first free slot
        InvEquipmentSlot Slot=null;
		foreach (var slot in EquipmentSlots){
			if (slot.HasType(item.baseItem.type)&&slot.Item==null){
				Slot=slot;
				break;
			}
		}
		if (Slot==null) return item;

		return Replace(Slot.Slot,item);
	}

	/// <summary>
	/// Unequip the specified item, returning it if the operation was successful.
	/// </summary>
	public InvGameItem Unequip (InvGameItem item)
	{
		if (item == null) return item;
		InvBaseItem baseItem = item.baseItem;
		if (baseItem == null) return item;

		//find if item present
        InvEquipmentSlot Slot=null;
		foreach (var slot in EquipmentSlots){
			if (slot.Item==item){
				Slot=slot;
				break;
			}
		}
		return Replace(Slot.Slot, null);

	}

	/// <summary>
	/// Unequip the item from the specified slot, returning the item that was unequipped.
	/// </summary>

	public InvGameItem Unequip (UIEquipmentSlot.Slot slot) { return Replace(slot, null); }

	/// <summary>
	/// Whether the specified item is currently equipped.
	/// </summary>

	public bool HasEquipped (InvGameItem item)
	{
		foreach (var slot in EquipmentSlots){
			if (slot.Item==item){
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Whether the specified slot currently has an item equipped.
	/// </summary>

	public bool HasEquipped (UIEquipmentSlot.Slot slot)
	{
		return EquipmentSlots[(int)slot].Item!=null;
	}


	/// <summary>
	/// Retrieves the item in the specified slot.
	/// </summary>

	public InvGameItem GetItem (UIEquipmentSlot.Slot slot)
	{
		foreach (var s in EquipmentSlots){
			if (s.Slot==slot){
				return s.Item;
			}
		}
		return null;
	}

    public bool HasEmptySlots(){
        foreach (var slot in EquipmentSlots){
            if (slot.Item==null){
                return true;
            }
        }
        return false;
    }

    //static

    /// <summary>
    /// DEV. WARNING WARNING infinite loop running the world!
    /// </summary>
    public static void EquipRandomItem(InvEquipmentStorage equipment,XmlDatabase XDB){
        if (equipment == null) return;
        if (XDB.items.Count == 0) return;
        if (!equipment.HasEmptySlots()) return;

        while(true){
            var gi=InvGameItem.GetRandomItem(XDB);
            if (equipment.Equip(gi)==null) break;
        }
    }
}