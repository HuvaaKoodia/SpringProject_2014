using UnityEngine;
using System.Collections.Generic;

public class EquipRandomItem : MonoBehaviour
{
	public InvEquipmentStorage equipment;
	public XmlDatabase XDB;

	void OnClick()
	{
        InvEquipmentStorage.EquipRandomItem(equipment,XDB);
	}
}