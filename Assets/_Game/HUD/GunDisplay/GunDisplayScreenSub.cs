using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class GunDisplayScreenSub : MonoBehaviour {

	public GunDisplayMain displayMain;
	public WeaponMain weapon;

	public UILabel infoLabel;
	public UISprite highlight;

	public UISprite overheat;

	// Use this for initialization
	void Start () {
		overheat.enabled = false;
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

			if (!weapon.WeaponSlot.ObjData.OVERHEAT)
			{
				info += "heat: " + weapon.CurrentHeat + "/100";
				overheat.enabled = false;
			}
			else
			{
				info += "[FF0000]OVERHEAT![FFFFFF]";
				overheat.enabled = true;
			}
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
