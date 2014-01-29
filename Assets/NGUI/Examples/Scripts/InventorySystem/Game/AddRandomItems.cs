using UnityEngine;
using System.Collections.Generic;

public class AddRandomItems : MonoBehaviour
{
	public InvItemStorage storage;

	void Start()
	{
		if (storage == null) return;
		List<InvBaseItem> list = InvDatabase.list[0].items;
		if (list.Count == 0) return;

		for (int i=0;i<5;i++){
			int qualityLevels = (int)InvGameItem.Quality._Amount;
			int index = Random.Range(0, list.Count);
			InvBaseItem item = list[index];

			InvGameItem gi = new InvGameItem(index, item);
			gi.quality = (InvGameItem.Quality)Random.Range(0, qualityLevels);
			gi.itemLevel = NGUITools.RandomRange(item.minItemLevel, item.maxItemLevel);
			storage.Add(gi);
		}
	}
}