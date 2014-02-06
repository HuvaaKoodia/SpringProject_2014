using UnityEngine;
using System.Collections.Generic;

public class InvEquipmentSlot{
	public UIEquipmentSlot.Slot Slot {get;private set;}
	public List<InvBaseItem.Type> TypeList {get;private set;}
	public InvGameItem Item;

    public InvEquipmentSlot(UIEquipmentSlot.Slot slot,params InvBaseItem.Type[] types){
		Slot=slot;

		TypeList=new List<InvBaseItem.Type>();
		foreach (var t in types){
			TypeList.Add(t);
		}

        ObjData=new MechaPartObjData();
	}

	public bool HasType(InvBaseItem.Type type)
	{
		return TypeList.Contains(type);
	}

    //stats

    public MechaPartObjData ObjData{get;private set;}
}

public class InvEquipmentStorage
{
    public InvEquipmentSlot UpperTorso{get;private set;}
    public InvEquipmentSlot LowerTorso{get;private set;}
    public InvEquipmentSlot[] EquipmentSlots {get;private set;}
	InvAttachmentPoint[] mAttachments;

    public InvEquipmentStorage(){
        Awake();
    }

	void Awake(){
		//Inventory Slots
        EquipmentSlots=new InvEquipmentSlot[8];
		AddSlot(UIEquipmentSlot.Slot.WeaponLeftHand,InvBaseItem.Type.LightWeapon);
		AddSlot(UIEquipmentSlot.Slot.WeaponRightHand,InvBaseItem.Type.LightWeapon);
		AddSlot(UIEquipmentSlot.Slot.WeaponLeftShoulder,InvBaseItem.Type.HeavyWeapon,InvBaseItem.Type.LightWeapon);
		AddSlot(UIEquipmentSlot.Slot.WeaponRightShoulder,InvBaseItem.Type.HeavyWeapon,InvBaseItem.Type.LightWeapon);

		AddSlot(UIEquipmentSlot.Slot.Utility1,InvBaseItem.Type.Utility);
		AddSlot(UIEquipmentSlot.Slot.Utility2,InvBaseItem.Type.Utility);
		AddSlot(UIEquipmentSlot.Slot.Utility3,InvBaseItem.Type.Utility);
		AddSlot(UIEquipmentSlot.Slot.Utility4,InvBaseItem.Type.Utility);

        //Hidden slots
        UpperTorso =new InvEquipmentSlot(UIEquipmentSlot.Slot.UpperTorso);
        LowerTorso =new InvEquipmentSlot(UIEquipmentSlot.Slot.LowerTorso);
	}

	private void AddSlot(UIEquipmentSlot.Slot slot,params InvBaseItem.Type[] type){
        EquipmentSlots[(int)slot]=new InvEquipmentSlot(slot,type);
	}

    public InvEquipmentSlot GetSlot(UIEquipmentSlot.Slot slot){
		return EquipmentSlots[(int)slot];
	}

	/// <summary>
	/// Equip the specified item automatically replacing an existing one.
	/// </summary>

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


    //statics

    public static void EquipRandomItem(InvEquipmentStorage equipment,XmlDatabase XDB){
        if (equipment == null) return;
        if (XDB.items.Count == 0) return;
        
        var gi=InvGameItem.GetRandomItem(XDB);
        equipment.Equip(gi);
    }
    
}