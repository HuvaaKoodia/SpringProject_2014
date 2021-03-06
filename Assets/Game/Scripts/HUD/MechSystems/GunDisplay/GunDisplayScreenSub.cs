﻿using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class GunDisplayScreenSub : MonoBehaviour {

	public GunDisplayMain displayMain;
	public WeaponMain weapon;

	public UILabel infoLabel, missedLabel;

	Timer missHideTimer;

	public UISprite highlight,heat_slider_spr;

	GameObject data_panel,overheat_icon,no_ammo_icon,broken_icon;

	public List<UISprite> ROFempty;
	public List<UISprite> ROFfilled;

	// Use this for initialization
	void Awake () {
		overheat_icon=transform.Find("OverheatIcon").gameObject;
		no_ammo_icon=transform.Find("NoAmmoIcon").gameObject;
		broken_icon=transform.Find("BrokenIcon").gameObject;
	
		data_panel=transform.Find("DataPanel").gameObject;

		missedLabel.gameObject.SetActive(false);

		overheat_icon.SetActive(false);
		no_ammo_icon.SetActive(false);
		broken_icon.SetActive(false);
	}

	public void CreateMissTimer(float waitTime)
	{
		missHideTimer = new Timer((int)(waitTime * 1000), new Timer.TimerEvent(HideMissedText));
		missHideTimer.Active = false;
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

			if (!missHideTimer.Active)
			{
				overheat_icon.SetActive(weapon.WeaponSlot.ObjData.OVERHEAT);
				no_ammo_icon.SetActive(player.ObjData.GetAmmoAmount(weapon.Weapon.baseItem.ammotype)==0);
				broken_icon.SetActive(!weapon.WeaponSlot.ObjData.USABLE);
			}

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
			//info += "ROF: " + weapon.GetNumShotsTargetedTotal() + "/" + weapon.RateOfFire;


			infoLabel.text = info;

			int i = 0;
			for (; i < weapon.GetNumShotsTargetedTotal(); i++)
			{
				ROFfilled[i].enabled = true;
			}
			for (; i < 5; i++)
			{
				ROFfilled[i].enabled = false;
			}

			if (overheat_icon.activeSelf||no_ammo_icon.activeSelf||broken_icon.activeSelf || missHideTimer.Active){
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

	public void ShowMissedText()
	{
		missedLabel.gameObject.SetActive(true);
		data_panel.SetActive(false);
		missHideTimer.Active = true;
	}

	public void HideMissedText()
	{
		missedLabel.gameObject.SetActive(false);
		missHideTimer.Reset();
		missHideTimer.Active = false;
		UpdateGunInfo(weapon.player);
	}

	public void SetMaxROF(int maxROF)
	{
		int i = 0;

		for (; i < maxROF; i++)
		{
			ROFempty[i].enabled = true;
		}
		for (; i < 5; i++)
		{
			ROFempty[i].enabled = false;
		}
	}

	void Update()
	{
		missHideTimer.Update(Time.deltaTime);
	}
}
