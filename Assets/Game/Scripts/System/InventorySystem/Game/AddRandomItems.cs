using UnityEngine;
using System.Collections.Generic;

public class AddRandomItems : MonoBehaviour
{
	public InvItemStorage storage;
	public XmlDatabase XDB;

	void Start()
	{
		if (storage == null) return;
		if (XDB.items.Count == 0) return;

		for (int i=0;i<5;i++){
            var gi=InvGameItem.GetRandomItem(XDB);
			storage.Add(gi);
		}
	}
}