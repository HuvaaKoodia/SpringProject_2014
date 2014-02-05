using UnityEngine;
using System.Collections.Generic;

public class EquipRandomItem : MonoBehaviour
{
	public InvEquipmentStorage equipment;
	public XmlDatabase XDB;

	void OnClick()
	{
		if (equipment == null) return;
		if (XDB.items.Count == 0) return;

		var gi=InvGameItem.GetRandomItem(XDB);
		equipment.Equip(gi);
	}
}