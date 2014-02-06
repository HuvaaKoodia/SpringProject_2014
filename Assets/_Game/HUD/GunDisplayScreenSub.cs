using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class GunDisplayScreenSub : MonoBehaviour {

	public GunDisplayMain displayMain;
	public WeaponMain weapon;

	public UILabel infoLabel;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateGunInfo()
	{
		if (weapon == null)
		{
			infoLabel.text = "";
			return;
		}

		string info = weapon.GunName;

		info += "\n";

		info += "ammo: " + weapon.CurrentAmmo + "/" + weapon.MaxAmmo;

		info += "\n";

		info += "heat: " + weapon.CurrentHeat + "/100";

		info += "\n";
	
		info += "ROF: " + weapon.GetNumShotsTargetedTotal() + "/" + weapon.RateOfFire;

		infoLabel.text = info;
	}

	public Color GetGunColor(WeaponID id)
	{
		return Color.white;//return weaponSelectionButtons[(int)id].color;
	}
}
