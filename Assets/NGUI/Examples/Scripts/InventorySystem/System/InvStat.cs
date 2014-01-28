using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InvStat
{
	public enum Identifier
	{
		Value,
		Damage,
		Crit,
		Heat,
		Other
	}

	/// <summary>
	/// Formula for final stats: [sum of raw amounts] * (1 + [sum of percent amounts])
	/// </summary>

	public enum Modifier
	{
		Added,
		Percent,
	}

	public Identifier id;
	public Modifier modifier;
	public int amount;
	
	static public string GetName (Identifier i)
	{
		return i.ToString();
	}

	static public string GetDescription (Identifier i)
	{
		switch (i)
		{
			case Identifier.Damage:			return "Amount of damage done per hit.";
			case Identifier.Crit:			return "Increases critical hit chance.";
			case Identifier.Heat:			return "Amount of heat generated per use.";
		}
		return null;
	}

	/// <summary>
	/// Comparison function for sorting weapons. Damage value will show up first, followed by heat.
	/// </summary>

	static public int CompareWeapon (InvStat a, InvStat b)
	{
		int ia = (int)a.id;
		int ib = (int)b.id;

		if		(a.id == Identifier.Damage) ia -= 10000;
		else if (a.id == Identifier.Heat)  ia -= 5000;
		if		(b.id == Identifier.Damage) ib -= 10000;
		else if (b.id == Identifier.Heat)  ib -= 5000;

		if (a.amount < 0) ia += 1000;
		if (b.amount < 0) ib += 1000;
		
		if (a.modifier == Modifier.Percent) ia += 100;
		if (b.modifier == Modifier.Percent) ib += 100;
		
		if (ia < ib) return -1;
		if (ia > ib) return 1;
		return 0;
	}
}