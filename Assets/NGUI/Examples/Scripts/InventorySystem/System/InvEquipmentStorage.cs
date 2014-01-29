using UnityEngine;
using System.Collections.Generic;

public class EquipmentStorageSlot{
	public UIEquipmentSlot.Slot Slot {get;private set;}
	public InvBaseItem.Type Type {get;private set;}
	public InvGameItem Item;

	public EquipmentStorageSlot(UIEquipmentSlot.Slot slot,InvBaseItem.Type type){
		Slot=slot;
		Type=type;
	}
}

public class InvEquipmentStorage : MonoBehaviour
{
	public EquipmentStorageSlot[] EquipmentSlots {get;private set;}
	InvAttachmentPoint[] mAttachments;

	public void Awake(){
		//create Slots. HARDCODED
		EquipmentSlots=new EquipmentStorageSlot[8];
		AddSlot(UIEquipmentSlot.Slot.WeaponLeftHand,InvBaseItem.Type.WeaponHand);
		AddSlot(UIEquipmentSlot.Slot.WeaponRightHand,InvBaseItem.Type.WeaponHand);
		AddSlot(UIEquipmentSlot.Slot.WeaponLeftShoulder,InvBaseItem.Type.WeaponShoulder);
		AddSlot(UIEquipmentSlot.Slot.WeaponRightShoulder,InvBaseItem.Type.WeaponShoulder);

		AddSlot(UIEquipmentSlot.Slot.Utility1,InvBaseItem.Type.Utility);
		AddSlot(UIEquipmentSlot.Slot.Utility2,InvBaseItem.Type.Utility);
		AddSlot(UIEquipmentSlot.Slot.Utility3,InvBaseItem.Type.Utility);
		AddSlot(UIEquipmentSlot.Slot.Utility4,InvBaseItem.Type.Utility);
	}

	private void AddSlot(UIEquipmentSlot.Slot slot,InvBaseItem.Type type){
		EquipmentSlots[(int)slot]=new EquipmentStorageSlot(slot,type);
	}

	/// <summary>
	/// Equip the specified item automatically replacing an existing one.
	/// </summary>

	public InvGameItem Replace (UIEquipmentSlot.Slot slot, InvGameItem item)
	{
		//InvBaseItem baseItem = (item != null) ? item.baseItem : null;
		var Slot=EquipmentSlots[(int)slot];

		if (item==null){
			var prev=Slot.Item;
			Slot.Item=null;
			return prev;
		}
		else{
			// If the item is not of appropriate type, we shouldn't do anything
			if (item.baseItem.type != Slot.Type) return item;

			// Equip this item
			var prev = Slot.Item;
			Slot.Item=item;

			/*
			// Get the list of all attachment points
			if (mAttachments == null) mAttachments = GetComponentsInChildren<InvAttachmentPoint>();

			// Equip the item visually
			for (int i = 0, imax = mAttachments.Length; i < imax; ++i)
			{
				InvAttachmentPoint ip = mAttachments[i];

				if (ip.slot == slot)
				{
					GameObject go = ip.Attach(baseItem != null ? baseItem.attachment : null);

					if (baseItem != null && go != null)
					{
						Renderer ren = go.renderer;
						if (ren != null) ren.material.color = baseItem.color;
					}
				}
			}
			*/
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
		EquipmentStorageSlot Slot=null;
		foreach (var slot in EquipmentSlots){
			if (slot.Type==item.baseItem.type&&slot.Item==null){
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
		EquipmentStorageSlot Slot=null;
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
}