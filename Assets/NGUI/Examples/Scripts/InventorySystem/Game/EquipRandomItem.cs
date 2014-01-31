using UnityEngine;
using System.Collections.Generic;

public class EquipRandomItem : MonoBehaviour
{
	public InvEquipmentStorage equipment;
	public GameDatabase DB;

	void OnClick()
	{
		if (equipment == null) return;
		if (DB.items.Count == 0) return;

		var gi=InvGameItem.GetRandomItem(DB);
		equipment.Equip(gi);
	}
}