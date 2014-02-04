using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Contains the characters GameItems.
/// </summary>
public class InvItemStorage : MonoBehaviour
{
	public int maxItemCount = 8;
	
	public int maxRows = 4;
	public int maxColumns = 4;

	List<InvGameItem> mItems = new List<InvGameItem>();
	public List<InvGameItem> items { get { while (mItems.Count < maxItemCount) mItems.Add(null); return mItems; } }

	/// <summary>
	/// Convenience function that returns an item at the specified slot.
	/// </summary>

	public InvGameItem GetItem (int slot) { return (slot < items.Count) ? mItems[slot] : null; }

	/// <summary>
	/// Replace an item in the container with the specified one.
	/// </summary>
	/// <returns>An item that was replaced.</returns>

	public InvGameItem Replace (int slot, InvGameItem item)
	{
		if (slot < maxItemCount)
		{
			InvGameItem prev = items[slot];
			mItems[slot] = item;
			return prev;
		}
		return item;
	}

	public void Add(InvGameItem item){
		for (int i=0;i<maxItemCount;i++){
			if (GetItem(i)==null){
				if (Replace(i,item)!=item) break;
			}
		}
	}
}