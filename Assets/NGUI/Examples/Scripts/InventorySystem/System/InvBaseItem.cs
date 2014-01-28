﻿using UnityEngine;
using System.Collections.Generic;

public class InvBaseItem
{
	public enum Slot
	{
		None,
		WeaponRightHand,
		WeaponLeftHand,
		WeaponRightShoulder,
		WeaponLeftShoulder,
		Utility1,
		Utility2,
		Utility3,
		Utility4,
		_Amount
	}

	/// <summary>
	/// 16-bit item ID, generated by the system.
	/// Not to be confused with a 32-bit item ID, which actually contains the ID of the database as its prefix.
	/// </summary>

	public int id16;
	
	public string name;
	public string description;
	public Slot slot = Slot.None;

	/// <summary>
	/// Minimum and maximum allowed level for this item. When random loot gets generated,
	/// only items within appropriate level should be considered.
	/// </summary>

	public int minItemLevel = 1;
	public int maxItemLevel = 50;

	/// <summary>
	/// And and all base stats this item may have at a maximum level (50).
	/// Actual object's stats are calculated based on item's level and quality.
	/// </summary>

	public List<InvStat> stats = new List<InvStat>();

	/// <summary>
	/// Game Object that will be created and attached to the specified slot on the body.
	/// This should typically be a prefab with a renderer component, such as a sword,
	/// a bracer, shield, etc.
	/// </summary>

	public GameObject attachment;

	/// <summary>
	/// Object's main material color.
	/// </summary>

	public Color color = Color.white;

	/// <summary>
	/// Atlas used for the item's icon.
	/// </summary>

	public UIAtlas iconAtlas;

	/// <summary>
	/// Name of the icon's sprite within the atlas.
	/// </summary>

	public string iconName = "";
}