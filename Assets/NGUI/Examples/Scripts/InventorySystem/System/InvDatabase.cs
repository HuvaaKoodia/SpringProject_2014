using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]

public class InvDatabase : MonoBehaviour
{
	// Cached list of all available item databases
	static InvDatabase[] mList;
	static bool mIsDirty = true;

	/// <summary>
	/// Retrieves the list of item databases, finding all instances if necessary.
	/// </summary>

	static public InvDatabase[] list
	{
		get
		{
			if (mIsDirty)
			{
				mIsDirty = false;
				mList = NGUITools.FindActive<InvDatabase>();
			}
			return mList;
		}
	}

	/// <summary>
	/// Each database should have a unique 16-bit ID. When the items are saved, database IDs
	/// get combined with item IDs to create 32-bit IDs containing both values.
	/// </summary>

	public int databaseID = 0;

	/// <summary>
	/// List of items in this database.
	/// </summary>

	public List<InvBaseItem> items = new List<InvBaseItem>();

	/// <summary>
	/// UI atlas used for icons.
	/// </summary>

	public UIAtlas iconAtlas;

	/// <summary>
	/// Add this database to the list.
	/// </summary>

	void OnEnable () { mIsDirty = true; }

	/// <summary>
	/// Remove this database from the list.
	/// </summary>

	void OnDisable () { mIsDirty = true; }

	/// <summary>
	/// Find a database given its ID.
	/// </summary>

	static InvDatabase GetDatabase (int dbID)
	{
		for (int i = 0, imax = list.Length; i < imax; ++i)
		{
			InvDatabase db = list[i];
			if (db.databaseID == dbID) return db;
		}
		return null;
	}

	/// <summary>
	/// Find the item with the specified name.
	/// </summary>

	static public InvBaseItem FindByName (string exact)
	{
		for (int i = 0, imax = list.Length; i < imax; ++i)
		{
			InvDatabase db = list[i];

			for (int b = 0, bmax = db.items.Count; b < bmax; ++b)
			{
				InvBaseItem item = db.items[b];

				if (item.name == exact)
				{
					return item;
				}
			}
		}
		return null;
	}

}