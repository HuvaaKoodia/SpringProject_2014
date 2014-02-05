using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InvGameItem
{
	public enum Quality
	{
		Inferior,
		Normal,
		Superior,
		_Amount,
	}

	// ID of the base item used to create this game item
	//[SerializeField] int mBaseItemID = 0;

	/// <summary>
	/// Item quality -- applies a penalty or bonus to all base stats.
	/// </summary>

	public Quality quality = Quality.Normal;

	/// <summary>
	/// Item's effective level.
	/// </summary>

	public int itemLevel = 1;

	// Cached for speed
	InvBaseItem mBaseItem;


	public InvBaseItem baseItem
	{
		get
		{
			return mBaseItem;
		}
	}

	public string name_long
	{
		get
		{
			if (baseItem == null) return null;
			return "[" + NGUIText.EncodeColor(color) + "]"+quality.ToString() + " [FFFFFF]" + baseItem.name;
		}
	}

	public string name_linefeed
	{
		get
		{
			if (baseItem == null) return null;
			return "[" + NGUIText.EncodeColor(color) + "]"+quality.ToString() + "\n[FFFFFF]" + baseItem.name;
		}
	}

	/// <summary>
	/// Put your formula for calculating the item stat modifier here.
	/// Simplest formula -- scale it with quality and item level.
	/// Since all stats on base items are specified at max item level,
	/// calculating the effective multiplier is as simple as itemLevel/maxLevel.
	/// </summary>

	public float statMultiplier
	{
		get
		{
			float mult = 0f;

			switch (quality)
			{
				case Quality.Inferior:	mult = .5f;	break;
				case Quality.Normal:	mult = 1f;	break;
				case Quality.Superior:	mult = 1.5f;break;
			}

			// Take item's level into account
			float linear = (float)itemLevel / baseItem.maxItemLevel;

			// Add a curve for more interesting results
			mult *= Mathf.Lerp(linear, linear * linear, 0.5f);
			return mult;
		}
	}

	/// <summary>
	/// Item's color based on quality. You will likely want to change this to your own colors.
	/// </summary>

	public Color color
	{
		get
		{
			Color c = Color.white;

			switch (quality)
			{
				case Quality.Inferior:	c = Color.red; break;
				case Quality.Normal:	c = Color.cyan; break;
				case Quality.Superior:	c = Color.green; break;
			}
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
	
	public InvGameItem (InvBaseItem bi) 
	{ 
		mBaseItem = bi; 
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
			float mult = statMultiplier;
			List<InvStat> baseStats = baseItem.stats;

			for (int i = 0, imax = baseStats.Count; i < imax; ++i)
			{
				InvStat bs = baseStats[i];
				int amount = bs.min_amount+Mathf.RoundToInt(mult * (bs.max_amount-bs.min_amount));
				if (amount == 0) continue;

				bool found = false;

				for (int b = 0, bmax = stats.Count; b < bmax; ++b)
				{
					InvStat s = stats[b];

					if (s.type== bs.type && s.modifier == bs.modifier)
					{
						s._amount += amount;
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

    public static InvGameItem GetRandomItem (XmlDatabase XDB)
	{
		int qualityLevels = (int)InvGameItem.Quality._Amount;
		int index = Random.Range(0, XDB.items.Count);
		InvBaseItem item = Subs.GetRandom(XDB.items);
		
		InvGameItem gi = new InvGameItem(item);
		gi.quality = (InvGameItem.Quality)Random.Range(0, qualityLevels);
		gi.itemLevel = NGUITools.RandomRange(item.minItemLevel, item.maxItemLevel);
        gi.RecalculateStats();
		return gi;
	}

}