using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InvGameItem
{
	public bool VendorItem{get;set;}
	/// <summary>
	/// Item's effective level.
	/// </summary>
	public int itemLevel{get;set;}

	// Cached for speed
	InvBaseItem mBaseItem;
	public string BaseItemName {get;set;}

	public InvBaseItem baseItem
	{
		get
		{
			return mBaseItem;
		}
	}

	public string name
	{
		get
		{
			if (baseItem == null) return null;
			return baseItem.name;
		}
	}

	/// <summary>
	/// Put your formula for calculating the item stat modifier here.
	/// Simplest formula -- scale it with quality and item level.
	/// Since all stats on base items are specified at max item level,
	/// calculating the effective multiplier is as simple as itemLevel/maxLevel.
	/// </summary>

	public int StatValue(InvStat stat)
	{
		float multi=((itemLevel-1) / 9f);
		float itemStat =
			(Mathf.Pow ((1 - multi), 2) * stat.min_amount) + 
				(2 * (1 - multi) * multi * ControllerMultiplier (stat)) + 
				(Mathf.Pow ((multi), 2) * stat.max_amount);

		return Mathf.RoundToInt(itemStat);
	}
	public float ControllerMultiplier(InvStat stat)
	{
		float ctrlMultiplier = 0.5f;

		switch(stat.type) 
		{
		case InvStat.Type.Damage: 	ctrlMultiplier = 0.9f; break;
		case InvStat.Type.Accuracy: ctrlMultiplier = 0.6f; break;
		case InvStat.Type.Range: 	ctrlMultiplier = 1.0f; break;
		case InvStat.Type.Firerate:	ctrlMultiplier = 0.5f; break;
		case InvStat.Type.Heat: 	ctrlMultiplier = 0.8f; break;
		case InvStat.Type.Cooling: 	ctrlMultiplier = 0.4f; break;
		//case InvStat.Type.MaxAmmo: 	ctrlMultiplier = 0.8f; break;
		}

		return stat.min_amount + ((stat.max_amount - stat.min_amount) * ctrlMultiplier);

	}
	/// <summary>
	/// Item's color based on quality. You will likely want to change this to your own colors.
	/// </summary>

	public Color color
	{
		get
		{
			Color c = Color.white;

			return c;
		}
	}

	public List<InvStat> Stats{get;private set;}

	public InvStat GetStat (InvStat.Type type)
	{
		foreach(var s in Stats){
			if (s.type==type) return s;
		}
		return null;
	}

	public InvGameItem (){} 

	public InvGameItem (InvBaseItem bi) 
	{ 
		VendorItem=false;
		itemLevel = 1;

		mBaseItem = bi; 
		BaseItemName=mBaseItem.name;
        Stats=new List<InvStat>();
	}

	public void InitBaseItem(){
		mBaseItem=XmlDatabase.GetBaseItem(BaseItemName);
		BaseItemName=mBaseItem.name;

		RecalculateStats();
	}

    public void RecalculateStats(){
        Stats=CalculateStats();
    }

	/// <summary>
	/// Calculate and return the list of effective stats based on item level and quality.
	/// </summary>

	public List<InvStat> CalculateStats ()
	{
		List<InvStat> stats = new List<InvStat>();

		if (baseItem != null)
		{

			List<InvStat> baseStats = baseItem.Stats;

			for (int i = 0, imax = baseStats.Count; i < imax; ++i)
			{

				InvStat bs = baseStats[i];

				int amount = StatValue(bs);
				if (amount == 0) continue;

				bool found = false;

				for (int b = 0, bmax = stats.Count; b < bmax; ++b)
				{
					InvStat s = stats[b];

					if (s.type== bs.type && s.modifier == bs.modifier)
					{
						s._amount = amount;
						found = true;
						break;
					}
				}

				if (!found)
				{
					InvStat stat = new InvStat();
					stat.type = bs.type;
					stat._amount = amount;
					stat.modifier = bs.modifier;
					stats.Add(stat);
				}
			}

			// This would be the place to determine if it's a weapon or armor and sort stats accordingly
			stats.Sort(InvStat.CompareWeapon);
		}
		return stats;
	}

    public static InvGameItem GetRandomItem ()
	{
		InvBaseItem item = Subs.GetRandom(XmlDatabase.items.Values);
		
		InvGameItem gi = new InvGameItem(item);
		gi.itemLevel = NGUITools.RandomRange(item.minItemLevel, item.maxItemLevel);
        gi.RecalculateStats();
		return gi;
	}

	public static InvGameItem GetRandomItem (string lootpool)
	{
		var i=Subs.GetRandom(XmlDatabase.LootPool[lootpool]);
		InvBaseItem item = XmlDatabase.items[i];
		
		InvGameItem gi = new InvGameItem(item);
		gi.itemLevel = NGUITools.RandomRange(item.minItemLevel, item.maxItemLevel);
		gi.RecalculateStats();
		return gi;
	}

}