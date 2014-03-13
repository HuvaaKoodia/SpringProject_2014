using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Contains the characters inventory GameItems.
/// </summary>
public class InvItemStorage
{
	public int maxItemCount = 8;
	
	public int maxRows = 4;
	public int maxColumns = 4;

    void Awake(){
        mItems = new List<InvGameItem>();
    }

    public InvItemStorage(int itemcount,int rows,int columns){
        maxItemCount=itemcount;
        maxRows=rows;
        maxColumns=columns;

        Awake();
    }

    List<InvGameItem> mItems;
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

	public bool Add(InvGameItem item){
		for (int i=0;i<maxItemCount;i++){
			if (GetItem(i)==null){
                Replace(i,item);
                return true;
			}
		}
        return false;
	}
   

    public bool HasItem(System.Func<InvGameItem,bool> test){
        foreach(var i in mItems){
            if (test(i)) return true;
        }
        return false;
    }

    public List<InvGameItem> GetItems(System.Func<InvGameItem,bool> test){
        List<InvGameItem> _items=new List<InvGameItem>();
        foreach(var i in mItems){
            if (i!=null&&test(i)) _items.Add(i);
        }
        return _items;
    }
}