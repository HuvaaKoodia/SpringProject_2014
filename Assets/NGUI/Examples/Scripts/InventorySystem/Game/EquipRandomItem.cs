using UnityEngine;
using System.Collections.Generic;

public class EquipRandomItem : MonoBehaviour
{
	public InvEquipmentStorage equipment;

	void OnClick()
	{
		if (equipment == null) return;
		List<InvBaseItem> list = InvDatabase.list[0].items;
		if (list.Count == 0) return;

		int qualityLevels = (int)InvGameItem.Quality._Amount;
		int index = Random.Range(0, list.Count);
		InvBaseItem item = list[index];

		InvGameItem gi = new InvGameItem(index, item);
		gi.quality = (InvGameItem.Quality)Random.Range(0, qualityLevels);
		gi.itemLevel = NGUITools.RandomRange(item.minItemLevel, item.maxItemLevel);
		equipment.Equip(gi);
	}
}