using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class GunDisplayScreenSub : MonoBehaviour {

	public GunDisplayMain displayMain;
	public WeaponMain weapon;

	public UILabel infoLabel;
	public UISprite highlight,heat_slider_spr;

	GameObject data_panel,overheat_icon,no_ammo_icon,broken_icon;

	// Use this for initialization
	void Awake () {
		overheat_icon=transform.Find("OverheatIcon").gameObject;
		no_ammo_icon=transform.Find("NoAmmoIcon").gameObject;
		broken_icon=transform.Find("BrokenIcon").gameObject;
	
		data_panel=transform.Find("DataPanel").gameObject;

		overheat_icon.SetActive(false);
		no_ammo_icon.SetActive(false);
		broken_icon.SetActive(false);
	}

	public void UpdateGunInfo(PlayerMain player)
	{
		if (weapon.Weapon != null)
		{
			string info = weapon.GunName;

			info += "\n";

            if (weapon.NoAmmoConsumption)
                info += "";
            else
			    info += "ammo: " + weapon.CurrentAmmo + "/" + weapon.MaxAmmo;

			info += "\n";

			if (!weapon.WeaponSlot.ObjData.OVERHEAT)
			{
				info += "heat: " + weapon.CurrentHeat + "/100";
			}
			else
			{
				info += "[FF0000]OVERHEAT![FFFFFF]";
			}
			overheat_icon.SetActive(weapon.WeaponSlot.ObjData.OVERHEAT);
			no_ammo_icon.SetActive(player.ObjData.GetAmmoAmount(weapon.Weapon.baseItem.ammotype)==0);
			broken_icon.SetActive(!weapon.WeaponSlot.ObjData.USABLE);

			//hax fix
			if (broken_icon.activeSelf){
				no_ammo_icon.SetActive(false);
				overheat_icon.SetActive(false);
			}
			else  if (no_ammo_icon.activeSelf){
				overheat_icon.SetActive(false);
			}


			heat_slider_spr.fillAmount=weapon.WeaponSlot.ObjData.HeatPercent();

			info += "\n";
			info += "ROF: " + weapon.GetNumShotsTargetedTotal() + "/" + weapon.RateOfFire;

			infoLabel.text = info;

			if (overheat_icon.activeSelf||no_ammo_icon.activeSelf||broken_icon.activeSelf){
				data_panel.SetActive(false);
			}
			else
				data_panel.SetActive(true);
		}
		else
		{
			data_panel.SetActive(false);
			SetHighlightColor(displayMain.inActiveColor);
		}
	}

	public void SetHighlightColor(Color color)
	{
		highlight.color = color;
	}
}
