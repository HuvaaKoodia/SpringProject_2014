using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GunDisplayMain : MonoBehaviour {

	public GameController GC;
	public List<GunDisplayScreenSub> gunInfoScreens;

	// Use this for initialization
	void Start () {
	
	}

	public void SetWeaponToDisplay(WeaponID ID, WeaponMain weapon)
	{
		gunInfoScreens[(int)ID].weapon = weapon;
		UpdateDisplay(ID);
	}

	public void UpdateAllDisplays()
	{
		for (int i = 0; i < 4; i++)
		{
			UpdateDisplay((WeaponID)i);
		}
	}

	public void UpdateDisplay(WeaponID id)
	{
		gunInfoScreens[(int)id].UpdateGunInfo();
	}
}
