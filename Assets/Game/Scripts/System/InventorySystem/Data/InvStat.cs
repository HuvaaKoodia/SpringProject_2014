using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InvStat
{
	public enum Type
	{
		Value,
		Damage,
		Crit,
		Heat,
		Cooling,
		Firerate,
		Accuracy,
        Range,
		SystemCooling,
		HullOverheatLimit,
		HullArmor,
		RadarRange,
		AccuracyBoost,
		MeleeDamage,
		Hacking
	}

	/// <summary>
	/// Formula for final stats: [sum of raw amounts] * (1 + [sum of percent amounts])
	/// </summary>

	public enum Modifier
	{
		Added,
		Percent,
	}

	public Type type;
	public Modifier modifier;
	public int min_amount,max_amount,_amount;
	
	static public string GetName (Type i)
	{
		return i.ToString();
	}

	/// <summary>
	/// Comparison function for sorting weapons. Damage value will show up first, followed by heat.
	/// </summary>

	static public int CompareWeapon (InvStat a, InvStat b)
	{
		int ia = (int)a.type;
		int ib = (int)b.type;

		if		(a.type == Type.Damage) ia -= 10000;
		else if (a.type == Type.Heat)  ia -= 5000;
		if		(b.type == Type.Damage) ib -= 10000;
		else if (b.type == Type.Heat)  ib -= 5000;

		if (a._amount < 0) ia += 1000;
		if (b._amount < 0) ib += 1000;
		
		if (a.modifier == Modifier.Percent) ia += 100;
		if (b.modifier == Modifier.Percent) ib += 100;
		
		if (ia < ib) return -1;
		if (ia > ib) return 1;
		return 0;
	}
}