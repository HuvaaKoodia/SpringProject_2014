using UnityEngine;
using System.Collections.Generic;

public class AddRandomItems : MonoBehaviour
{
	public InvItemStorage storage;
	public GameDatabase DB;

	void Start()
	{
		if (storage == null) return;
		if (DB.items.Count == 0) return;

		for (int i=0;i<5;i++){
			var gi=InvGameItem.GetRandomItem(DB);
			storage.Add(gi);
		}
	}
}