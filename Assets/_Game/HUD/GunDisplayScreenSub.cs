using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class GunDisplayScreenSub : MonoBehaviour {

	public GunDisplayMain displayMain;
	public WeaponMain weapon;

	public UILabel infoLabel;
	public UISprite highlight;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateGunInfo()
	{

		InvGameItem gun = weapon.Weapon;
		if (gun != null)
		{
			string info = weapon.GunName;

			info += "\n";

			info += "ammo: " + weapon.CurrentAmmo + "/" + weapon.MaxAmmo;

			info += "\n";

			info += "heat: " + weapon.CurrentHeat + "/100";

			info += "\n";
		
			info += "ROF: " + weapon.GetNumShotsTargetedTotal() + "/" + weapon.RateOfFire;

			infoLabel.text = info;
		}
		else
		{
			infoLabel.text = "";
			SetHighlightColor(displayMain.inActiveColor);
		}
	}

	public void SetHighlightColor(Color color)
	{
		highlight.color = color;
	}
}
